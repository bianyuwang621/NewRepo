namespace project44portail.Models.JsonElement
{
    public class ShipmentStop
    {
        public int stopNumber { get; set; }
        public AppointmentWindow appointmentWindow { get; set; }
        public Location location { get; set; }

        public ShipmentStop()
        {
            appointmentWindow = new AppointmentWindow();
            location = new Location();
        }
    }
}