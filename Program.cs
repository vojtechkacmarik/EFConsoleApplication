using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using EFConsoleApplication.Components;
using EFConsoleApplication.Models;

namespace EFConsoleApplication
{
    public class Program
    {
        private static readonly object s_Lock = new object();
        private static IWindsorContainer s_Container;
        private static bool s_Disposed;

        /// <summary>
        /// Bootstraps this instance.
        /// </summary>
        /// <returns>Instance of Windsor Container.</returns>
        public static IWindsorContainer Bootstrap()
        {
            lock (s_Lock)
            {
                if (s_Container != null) return s_Container;

                s_Container = new WindsorContainer();

                s_Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(s_Container.Kernel, true));
                s_Container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LazyOfTComponentLoader>());
                s_Container.Register(Component.For<IWindsorContainer>().Instance(s_Container));
                s_Container.Install(FromAssembly.This());

                return s_Container;
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
            DbConfiguration.Loaded += (s, a) =>
            {
                a.AddDependencyResolver(new WindsorDependencyResolver(s_Container), false);
            };

            var interceptors = s_Container.ResolveAll<IDbCommandTreeInterceptor>();
            foreach (var interceptor in interceptors)
            {
                DbInterception.Add(interceptor);
            }

            using (var context = new PersonDbContext())
            {
                context.Database.Initialize(force: true);
            }
        }

        public static void Main(string[] args)
        {
            Bootstrap();
            Init();

            PerformDatabaseOperations();
            Console.Write("Person saved!");
            Console.ReadLine();

            Dispose();
        }

        private static void PerformDatabaseOperations()
        {
            using (var dbContext = new PersonDbContext())
            {
                var person = new Person
                {
                    FirstName = "Josef",
                    LastName = "Novak",
                    BirthDate = DateTime.Now.AddYears(-45)
                };

                dbContext.Persons.Add(person);
                dbContext.SaveChanges();

                var persons = dbContext.Persons.Where(p => p.FirstName == "Josef").ToList();
            }
        }
    }
}