namespace project44portail.Models.JsonElement
{
    public class Location
    {
        public Address address { get; set; }
        public Contact contact { get; set; }

        public Location()
        {
            address = new Address();
            contact = new Contact();
        }
    }
}