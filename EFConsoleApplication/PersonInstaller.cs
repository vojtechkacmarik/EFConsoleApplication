using System.Data.Entity.Infrastructure.Interception;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EFConsoleApplication.Components;

namespace EFConsoleApplication
{
    public class PersonInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IDateTimeProvider>().ImplementedBy<DateTimeProvider>());
            container.Register(Component.For<IDbCommandTreeInterceptor>().ImplementedBy<SoftDeleteInterceptor>());
            container.Register(Component.For<IDbCommandTreeInterceptor>().ImplementedBy<CreatedAndModifiedDateInterceptor>());
        }
    }
}