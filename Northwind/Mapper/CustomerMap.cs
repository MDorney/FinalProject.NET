using Northwind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Northwind.Mapper
{
    public class CustomerMap
    {
        public static CustomerEdit Map(Customer customer)
        {
            CustomerEdit ce = new CustomerEdit()
            {
                CompanyName = customer.CompanyName,
                ContactName = customer.ContactName,
                ContactTitle = customer.ContactTitle,
                Address = customer.Address,
                City = customer.City,
                Region = customer.Region,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                Phone = customer.Phone,
                Fax = customer.Fax,
                Email = customer.Email
            };
            return ce;
        }
        public static void Map(Customer customer, CustomerEdit UpdatedCustomer)
        {
            customer.CompanyName = UpdatedCustomer.CompanyName;
            customer.Address = UpdatedCustomer.Address;
            customer.City = UpdatedCustomer.City;
            customer.ContactName = UpdatedCustomer.ContactName;
            customer.ContactTitle = UpdatedCustomer.ContactTitle;
            customer.Country = UpdatedCustomer.Country;
            customer.Email = UpdatedCustomer.Email;
            customer.Fax = UpdatedCustomer.Fax;
            customer.Phone = UpdatedCustomer.Phone;
            customer.PostalCode = UpdatedCustomer.PostalCode;
            customer.Region = UpdatedCustomer.Region;
        }
    }
}