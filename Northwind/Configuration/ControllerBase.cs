using AutoMapper;
using Northwind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Northwind.Configuration
{
    public abstract class ControllerBase : Controller
    {
        protected readonly IMapper Mapper;

        protected ControllerBase()
        {
            var config = new MapperConfiguration( x =>
                {
                    x.CreateMap<Customer, CustomerEdit>();
                    x.CreateMap<CustomerEdit, Customer>();
                    x.CreateMap<CustomerRegister, Customer>();
                });
            Mapper = config.CreateMapper();
        }
    }
}