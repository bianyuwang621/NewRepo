using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project44portail.Models.JsonElement
{
    public class RootObject
    {
        public CarrierIdentifier carrierIdentifier { get; set; }
        public List<ShipmentIdentifier> shipmentIdentifiers { get; set; }
        public List<ShipmentStop> shipmentStops { get; set; }

        public List<EquipmentIdentifier> equipmentIdentifiers { get; set; }

        public RootObject()
        {
            carrierIdentifier = new CarrierIdentifier();
            shipmentIdentifiers = new List<ShipmentIdentifier>();
            shipmentStops = new List<ShipmentStop>();
            equipmentIdentifiers = new List<EquipmentIdentifier>();
        }
    }
}