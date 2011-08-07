// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.7.0.0
//      SpecFlow Generator Version:1.7.0.0
//      Runtime Version:4.0.30319.235
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
namespace SimpleCqrs.EventStore.SqlServer.Tests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.7.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("In order to insert and retrieve events from the event store\nAs a Simple CQRS deve" +
        "loper\nI want the event store to be created if it does not exist")]
    public partial class CreateEventStoreFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "CreateEventStore.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Create event store", "In order to insert and retrieve events from the event store\nAs a Simple CQRS deve" +
                    "loper\nI want the event store to be created if it does not exist", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
            this.FeatureBackground();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line hidden
#line 7
 testRunner.Given("the connection string to my database is", "Data Source=.\\SQLEXPRESS;Initial Catalog=test;Integrated Security=True;MultipleAc" +
                    "tiveResultSets=True;", ((TechTalk.SpecFlow.Table)(null)));
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The event store does not exist before inserting")]
        public virtual void TheEventStoreDoesNotExistBeforeInserting()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The event store does not exist before inserting", ((string[])(null)));
#line 12
this.ScenarioSetup(scenarioInfo);
#line 13
 testRunner.Given("the EventStore table does not exist");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table1.AddRow(new string[] {
                        "EventDate",
                        "3/20/2010 3:01:04 AM"});
            table1.AddRow(new string[] {
                        "AggregateRootId",
                        "8312E92C-DF1C-4970-A9D5-6414120C3CF7"});
            table1.AddRow(new string[] {
                        "Sequence",
                        "2"});
            table1.AddRow(new string[] {
                        "ThisHappened",
                        "something"});
#line 14
 testRunner.And("I have a SomethingHappenedEvent to be added to the store with the following value" +
                    "s", ((string)(null)), table1);
#line 20
 testRunner.And("that event will serialize to \'Serialized Object\'");
#line 21
 testRunner.When("I add the domain events to the store");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "EventDate"});
            table2.AddRow(new string[] {
                        "3/20/2010 3:01:04 AM"});
#line 22
 testRunner.Then("I should have the following events in the database", ((string)(null)), table2);
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The event store does not exist before retrieving events by aggregate root")]
        public virtual void TheEventStoreDoesNotExistBeforeRetrievingEventsByAggregateRoot()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The event store does not exist before retrieving events by aggregate root", ((string[])(null)));
#line 26
this.ScenarioSetup(scenarioInfo);
#line 27
 testRunner.Given("the EventStore table does not exist");
#line 28
 testRunner.When("I retrieve the domain events for \'8312E92C-DF1C-4970-A9D5-6414120C3CF7\'");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Sequence"});
#line 29
 testRunner.Then("I should get back the following DomainEvents", ((string)(null)), table3);
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The event store does not exist before retrieving events by event type")]
        public virtual void TheEventStoreDoesNotExistBeforeRetrievingEventsByEventType()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The event store does not exist before retrieving events by event type", ((string[])(null)));
#line 32
this.ScenarioSetup(scenarioInfo);
#line 33
 testRunner.Given("the EventStore table does not exist");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Type"});
            table4.AddRow(new string[] {
                        "SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventSto" +
                            "re.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"});
#line 34
 testRunner.When("I retrieve the domain events for the following types", ((string)(null)), table4);
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Sequence"});
#line 37
 testRunner.Then("I should get back the following DomainEvents", ((string)(null)), table5);
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#endregion
