using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NServiceBus;
using SimpleCqrs;
using SimpleCqrs.NServiceBus;
using SimpleCqrs.Unity;

namespace Sample.JesseHouse
{
    public class MvcApplication : HttpApplication
    {
        public static SimpleCqrsRuntime<UnityServiceLocator> Runtime;

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Customer", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            Runtime = new SimpleCqrsRuntime<UnityServiceLocator>();

            Configure.WithWeb()
                .DefaultBuilder()
                .BinarySerializer()     // Must use binary serializer for types that are not known to NServiceBus (domain events, command bus)
                .SimpleCqrs(Runtime)
                    .UseNsbCommandBus() // Tells SimpleCqrs to create a NsbCommandBus instance, which sends commands over NServiceBus
                .MsmqTransport()
                .UnicastBus()
                    .CreateBus()
                    .Start();

            // The command bus is created for you. (Its an instance of NsbCommandBus)
            // The CommandBusConfig in the Web.config tells the NsbCommandBus where to send the commands,
            // the Commands element is a type name or an assembly name and the Endpoint element specifies where to send the type or types in the assembly
            // you can get it out of the service locator using the line below
            // I use MvcTurbine for website, so I would register the command bus in the MvcTurbine's
            // service locator

            // var commandBus = Runtime.ServiceLocator.Resolve<ICommandBus>();

            // TODO : Go to CustomerController next
        }
    }
}