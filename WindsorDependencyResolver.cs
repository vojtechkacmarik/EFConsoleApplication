using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Linq;
using Castle.Windsor;

namespace EFConsoleApplication
{
    public class WindsorDependencyResolver : IDbDependencyResolver
    {
        private readonly IWindsorContainer container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            this.container = container;
        }

        public object GetService(Type type, object key)
        {
            return container.Kernel.HasComponent(type) ? container.Resolve(type) : null;
        }

        public IEnumerable<object> GetServices(Type type, object key)
        {
            return container.Kernel.HasComponent(type) ? container.ResolveAll(type).Cast<object>() : new object[] { };
        }
    }
}