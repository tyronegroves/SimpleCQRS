using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Polly;

namespace EventSourcingCQRS.EventStore.MongoDb.RetryPolicies
{
    public class EventStoreRetryPolicy
    {
        // This set of policies is based on this article, with customisation to allow for logging
        // https://www.nuomiphp.com/eplan/en/10737.html
        //
        private readonly ILogger<EventStoreRetryPolicy> _logger;
        public static string RetryPolicyName => $"{nameof(EventStoreRetryPolicy)}Name";

        public const int HttpThrottleErrorCode = 429;
        public const int HttpServiceIsUnavailable = 1;
        public const int HttpOperationExceededTimeLimit = 50;
        public const int RateLimitCode = 16500;
        public const string RetryAfterToken = "RetryAfterMs=";
        public const int MaxRetries = 10;
        public static readonly int RetryAfterTokenLength = RetryAfterToken.Length;

        private static readonly Random JitterSeed = new Random();

        public EventStoreRetryPolicy(ILogger<EventStoreRetryPolicy> logger)
        {
            _logger = logger;
        }

        public IAsyncPolicy GetPolicy()
        {
            return Policy.WrapAsync(
                MongoCommandExceptionPolicy(),
                ExecutionTimeoutPolicy(),
                MongoWriteExceptionPolicy(),
                MongoBulkWriteExceptionPolicy());
        }

        public IAsyncPolicy MongoCommandExceptionPolicy()
        {
            return Policy
                .Handle<MongoCommandException>(e =>
                {
                    if (e.Code != RateLimitCode || !(e.Result is BsonDocument bsonDocument))
                    {
                        return false;
                    }

                    if (bsonDocument.TryGetValue("StatusCode", out var statusCode) && statusCode.IsInt32)
                    {
                        switch (statusCode.AsInt32)
                        {
                            case HttpThrottleErrorCode:
                            case HttpServiceIsUnavailable:
                            case HttpOperationExceededTimeLimit:
                                return true;

                            default:
                                return false;
                        }
                    }

                    if (bsonDocument.TryGetValue("IsValid", out var isValid) && isValid.IsBoolean)
                    {
                        return isValid.AsBoolean;
                    }

                    return true;
                })
                .WaitAndRetryAsync(
                    retryCount: MaxRetries,
                    DefaultSleepDurationProviderWithJitter,
                    onRetryAsync: (exception, timeSpan, retryCount, context) =>
                    {
                        var methodThatRaisedException = string.Empty;
                        if (context.ContainsKey("methodName"))
                        {
                            methodThatRaisedException = context["methodName"].ToString();
                        }
                        _logger.LogWarning(
                            exception,
                            "Error talking to Event store database in {methodThatRaisedException}, retrying after {RetryTimeSpan}. Retry attempt {RetryCount}",
                            timeSpan, retryCount, methodThatRaisedException

                        );
                        return Task.CompletedTask;
                    }
                );
        }

        public IAsyncPolicy ExecutionTimeoutPolicy()
        {
            return Policy
                .Handle<MongoExecutionTimeoutException>(e =>
                    e.Code == RateLimitCode || e.Code == HttpOperationExceededTimeLimit
                )
                .WaitAndRetryAsync(
                    retryCount: MaxRetries,
                    DefaultSleepDurationProviderWithJitter,
                    onRetryAsync: (exception, timeSpan, retryCount, context) =>
                    {
                        var methodThatRaisedException = string.Empty;
                        if (context.ContainsKey("methodName"))
                        {
                            methodThatRaisedException = context["methodName"].ToString();
                        }
                        _logger.LogWarning(
                            exception,
                            "Error talking to Event store database in {methodThatRaisedException}, retrying after {RetryTimeSpan}. Retry attempt {RetryCount}",
                            timeSpan, retryCount, methodThatRaisedException

                        );
                        return Task.CompletedTask;
                    });
        }

        public IAsyncPolicy MongoWriteExceptionPolicy()
        {
            return Policy
                    .Handle<MongoWriteException>(e =>
                    {
                        return e.WriteError?.Code == RateLimitCode
                               || (e.InnerException is MongoBulkWriteException bulkException &&
                                   bulkException.WriteErrors.Any(error => error.Code == RateLimitCode));
                    })
                    .WaitAndRetryAsync(
                        retryCount: MaxRetries,
                        sleepDurationProvider: (retryAttempt, e, ctx) =>
                        {
                            var timeToWaitInMs = ExtractTimeToWait(e.Message);
                            if (!timeToWaitInMs.HasValue && e.InnerException != null)
                            {
                                timeToWaitInMs = ExtractTimeToWait(e.InnerException.Message);
                            }

                            return timeToWaitInMs ?? DefaultSleepDurationProviderWithJitter(retryAttempt);
                        },
                        onRetryAsync: (exception, timeSpan, retryCount, context) =>
                        {
                            var methodThatRaisedException = string.Empty;
                            if (context.ContainsKey("methodName"))
                            {
                                methodThatRaisedException = context["methodName"].ToString();
                            }
                            _logger.LogWarning(
                                exception,
                                "Error talking to Event store database in {methodThatRaisedException}, retrying after {RetryTimeSpan}. Retry attempt {RetryCount}",
                                timeSpan, retryCount, methodThatRaisedException

                            );
                            return Task.CompletedTask;
                        });
        }

        public IAsyncPolicy MongoBulkWriteExceptionPolicy()
        {
            return Policy
                .Handle<MongoBulkWriteException>(e =>
                {
                    return e.WriteErrors.Any(error => error.Code == RateLimitCode);
                })
                .WaitAndRetryAsync(
                    retryCount: MaxRetries,
                    sleepDurationProvider: (retryAttempt, e, ctx) =>
                    {
                        var timeToWaitInMs = ExtractTimeToWait(e.Message);
                        return timeToWaitInMs ?? DefaultSleepDurationProviderWithJitter(retryAttempt);
                    },
                    onRetryAsync: (exception, timeSpan, retryCount, context) =>
                    {
                        var methodThatRaisedException = string.Empty;
                        if (context.ContainsKey("methodName"))
                        {
                            methodThatRaisedException = context["methodName"].ToString();
                        }
                        _logger.LogWarning(
                            exception,
                            "Error talking to Event store database in {methodThatRaisedException}, retrying after {RetryTimeSpan}. Retry attempt {RetryCount}",
                            timeSpan, retryCount, methodThatRaisedException

                        );
                        return Task.CompletedTask;
                    });
        }

        /// <summary>
        /// It doesn't seem like RetryAfterMs is a property value - so unfortunately, we have to extract it from a string... (crazy??!)
        /// </summary>
        private static TimeSpan? ExtractTimeToWait(string messageToParse)
        {
            var retryPos = messageToParse.IndexOf(RetryAfterToken, StringComparison.OrdinalIgnoreCase);
            if (retryPos < 0)
            {
                return default;
            }

            retryPos += RetryAfterTokenLength;
            var endPos = messageToParse.IndexOf(',', retryPos);
            if (endPos <= 0)
            {
                return default;
            }

            var timeToWaitInMsString = messageToParse.Substring(retryPos, endPos - retryPos);
            if (int.TryParse(timeToWaitInMsString, out var timeToWaitInMs))
            {
                return TimeSpan.FromMilliseconds(timeToWaitInMs)
                       + TimeSpan.FromMilliseconds(JitterSeed.Next(100, 1000));
            }

            return default;
        }

        public static Func<int, TimeSpan> SleepDurationProviderWithJitter(double exponentialBackoffInSeconds, int maxBackoffTimeInSeconds) =>
            retryAttempt =>
                TimeSpan.FromSeconds(Math.Min(Math.Pow(exponentialBackoffInSeconds, retryAttempt),
                    maxBackoffTimeInSeconds)) // exponential back-off: 2, 4, 8 etc
                + TimeSpan.FromMilliseconds(JitterSeed.Next(0, 1000)); // plus some jitter: up to 1 second

        public static readonly Func<int, TimeSpan> DefaultSleepDurationProviderWithJitter =
            SleepDurationProviderWithJitter(1.5, 23);
    }
}