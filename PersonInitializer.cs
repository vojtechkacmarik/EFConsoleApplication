using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace EFConsoleApplication
{
    public class PersonInitializer
    {
        private static readonly object s_Lock = new object();
        private static IWindsorContainer s_Container;
        private static bool s_Disposed;

        /// <summary>
        /// Bootstraps this instance.
        /// </summary>
        /// <returns>Instance of Windsor Container.</returns>
        public static void Bootstrap()
        {
            lock (s_Lock)
            {
                if (s_Container != null) return;

                s_Container = new WindsorContainer();

                s_Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(s_Container.Kernel, true));
                s_Container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LazyOfTComponentLoader>());
                s_Container.Register(Component.For<IWindsorContainer>().Instance(s_Container));
                s_Container.Install(FromAssembly.This());

                Init();
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public static void Dispose()
        {
            lock (s_Lock)
            {
                if (s_Container != null && !s_Disposed)
                {
                    s_Container.Dispose();
                }

                s_Disposed = true;
            }
        }

        public static void Init()
        {
            AddDependencyResolver();
            AddInterceptors();
            InitializeDatabase();
        }

        private static void AddDependencyResolver()
        {
            DbConfiguration.Loaded += (s, a) =>
            {
                a.AddDependencyResolver(new WindsorDependencyResolver(s_Container), false);
            };
        }

        private static void AddInterceptors()
        {
            var interceptors = s_Container.ResolveAll<IDbCommandTreeInterceptor>();
            if (interceptors == null) return;

            foreach (var interceptor in interceptors)
            {
                DbInterception.Add(interceptor);
            }
        }

        private static void InitializeDatabase()
        {
            using (var context = new PersonDbContext())
            {
                context.Database.Initialize(force: true);
            }
        }
    }
}