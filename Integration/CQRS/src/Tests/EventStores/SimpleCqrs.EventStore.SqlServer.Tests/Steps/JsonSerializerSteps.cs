using SimpleCqrs.EventStore.SqlServer.Serializers;
using TechTalk.SpecFlow;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Steps
{
    [Binding]
    public class JsonSerializerSteps
    {
        [Given(@"I am choosing to use the Json Serializer")]
        public void GivenIAmChoosingToUseTheJsonSerializer()
        {
            ScenarioContext.Current.Set<IDomainEventSerializer>(new JsonDomainEventSerializer());
        }
    }
}