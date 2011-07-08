using TechTalk.SpecFlow;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Specs
{
    [Binding]
    public class RuntimeSteps
    {
        public static TestingRuntime Runtime;

        [BeforeTestRun]
        public static void TestRun()
        {
            Runtime = new TestingRuntime();
            Runtime.Start();
        }

        [AfterTestRun]
        public static void Cleanup()
        {
            Runtime.Shutdown();
        }

        [BeforeScenario]
        public void Setup()
        {
            ScenarioContext.Current.Set<IServiceLocator>(Runtime.ServiceLocator);
        }

        [AfterScenario]
        public void Teardown()
        {
        }
    }
}