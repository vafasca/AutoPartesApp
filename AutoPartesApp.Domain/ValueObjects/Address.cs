using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.ValueObjects
{
    public class Address
    {
        public string Street { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;
        public string ZipCode { get; private set; } = string.Empty;

        private Address() { } // requerido por EF

        public Address(string street, string city, string state, string country, string zipCode)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
        }

    }
}
