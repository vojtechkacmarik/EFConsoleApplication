using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFConsoleApplication.Enums;
using EFConsoleApplication.Models;

namespace EFConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PersonInitializer.Bootstrap();

            try
            {
                // PerformDatabaseOperations();

                //var persons = TestSynchronouslyAsync().Result;
                //foreach (var person in persons)
                //{
                //    Console.WriteLine("Person: " + person.FirstName);
                //}

                var persons = TestParalelsAsync().Result;
                foreach (var person in persons)
                {
                    Console.WriteLine("Person: " + person.FirstName);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            Console.Write("Done!");
            Console.ReadLine();

            PersonInitializer.Dispose();
        }

        private static async Task<List<Person>> TestSynchronouslyAsync()
        {
            List<Person> personsA;
            List<Person> personsB;

            Console.WriteLine("DbContext creating ... ThreadId = " + Thread.CurrentThread.ManagedThreadId);
            using (var dbContext = new PersonDbContext())
            {
                Console.WriteLine("DbContext created.");
                personsA = await dbContext.Persons.Where(p => p.LastName == "Horak").ToListAsync();
                Console.WriteLine("Step 1 end. ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                personsB = await dbContext.Persons.Where(p => p.LastName == "Novak").ToListAsync();
                Console.WriteLine("Step 1 end. ThreadId=" + Thread.CurrentThread.ManagedThreadId);
            }
            Console.WriteLine("DbContext finish.");

            return personsA.Concat(personsB).ToList();
        }

        private static async Task<List<Person>> TestParalelsAsync()
        {
            List<Person> personsA = null;
            List<Person> personsB = null;

            Console.WriteLine("DbContext creating ... ThreadId=" + Thread.CurrentThread.ManagedThreadId);
            using (var dbContext = new PersonDbContext())
            {
                Console.WriteLine("DbContext created.");

                var task1 = Task.Run(async () =>
                {
                    Console.WriteLine("Step 1 run. ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                    personsA = await dbContext.Persons.Where(p => p.LastName == "Horak").ToListAsync();
                    Console.WriteLine("Step 1 end. ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                });

                var task2 = Task.Run(async () =>
                {
                    Console.WriteLine("Step 2 run. ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                    personsB = await dbContext.Persons.Where(p => p.LastName == "Novak").ToListAsync();
                    Console.WriteLine("Step 2 end. ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                });
                var tasks = new List<Task> { task1, task2 };

                await Task.WhenAll(tasks);
            }
            Console.WriteLine("DbContext finish.");

            return personsA.Concat(personsB).ToList();
        }

        private static void PerformDatabaseOperations()
        {
            //CreatePersonWithAddress();

            var personsWithAddress = GetPersonWithAddress(p => p.LastName == "Horak");
            var personsWithoutAddress = GetPersonWithoutAddress(p => p.LastName == "Horak");

            //UpdatePerson(personsWithAddress.FirstOrDefault());
            //DeletePersonAddress(p => p.Id == 9, a => a.Id == 2);
        }

        private static void UpdatePerson(Person personsWithAddress)
        {
            if (personsWithAddress == null) return;

            using (var dbContext = new PersonDbContext())
            {
                personsWithAddress.BirthDate = DateTime.Today.AddYears(-30);

                dbContext.Persons.Attach(personsWithAddress);
                dbContext.Entry(personsWithAddress).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        private static void DeletePersonAddress(Func<Person, bool> singlePersonPredicate, Func<Address, bool> singleAddressPredicate)
        {
            using (var dbContext = new PersonDbContext())
            {
                var person = dbContext.Persons
                    .Include(nameof(Person.Addresses))
                    .Single(singlePersonPredicate);

                var addressToDelete = person.Addresses.Single(singleAddressPredicate);
                person.Addresses.Remove(addressToDelete);
                dbContext.SaveChanges();
            }
        }

        private static void CreatePersonWithAddress()
        {
            using (var dbContext = new PersonDbContext())
            {
                var person = new Person
                {
                    FirstName = "Karel",
                    LastName = "Horak",
                    BirthDate = DateTime.Now.AddYears(-22)
                };

                var address1 = new Address
                {
                    City = "Ostrava",
                    Number = "20",
                    PostalCode = "45678",
                    Country = "Czech",
                    AddressType = AddressType.Home
                };

                person.Addresses = new List<Address> { address1 };

                dbContext.Persons.Add(person);
                dbContext.SaveChanges();
            }
        }

        private static IEnumerable<Person> GetPersonWithoutAddress(Func<Person, bool> predicate)
        {
            using (var dbContext = new PersonDbContext())
            {
                return dbContext.Persons.Where(predicate).ToList();
            }
        }

        private static IEnumerable<Person> GetPersonWithAddress(Func<Person, bool> predicate)
        {
            using (var dbContext = new PersonDbContext())
            {
                return dbContext.Persons
                    .Include(nameof(Person.Addresses))
                    .Where(predicate)
                    .ToList();
            }
        }
    }
}