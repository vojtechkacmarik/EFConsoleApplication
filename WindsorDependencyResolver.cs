using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Linq;
using Castle.Windsor;

namespace EFConsoleApplication
{
    public class WindsorDependencyResolver : IDbDependencyResolver
    {
        private readonly IWindsorContainer m_Container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            m_Container = container;
        }

        public object GetService(Type type, object key)
        {
            return m_Container.Kernel.HasComponent(type) ? m_Container.Resolve(type) : null;
        }

        public IEnumerable<object> GetServices(Type type, object key)
        {
            return m_Container.Kernel.HasComponent(type) ? m_Container.ResolveAll(type).Cast<object>() : new object[] { };
        }
    }
}