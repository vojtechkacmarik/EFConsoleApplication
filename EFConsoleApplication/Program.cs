using System;
using System.Collections.Generic;
using System.Linq;
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
                PerformDatabaseOperations();
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

        private static void PerformDatabaseOperations()
        {
            //CreatePersonWithAddress();

            var personsWithAddress = GetPersonWithAddress(p => p.LastName == "Horak");
            var personsWithoutAddress = GetPersonWithoutAddress(p => p.LastName == "Horak");
            //DeletePersonAddress(p => p.Id == 9, a => a.Id == 2);
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