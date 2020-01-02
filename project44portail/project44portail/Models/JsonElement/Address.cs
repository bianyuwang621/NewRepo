using System.Collections.Generic;

namespace project44portail.Models.JsonElement
{
    public class Address
    {
        public string postalCode { get; set; }
        public List<string> addressLines { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public Address()
        {
            addressLines = new List<string>();
        }
    }
}