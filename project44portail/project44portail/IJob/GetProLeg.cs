using project44portail.Models.JsonElement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace project44portail.IJob
{
    public class GetProLeg
    {
        static string _con_sql4 = "Data Source=SQL4;Initial Catalog=A1TransportLTL;Integrated Security=True;User Id =sa; password=google08;";
        static string _con_sqlsrv01 = "Data Source=SQLSRV01;Initial Catalog=TLCmanifest/Tables;Integrated Security=True; User Id =sa; password=google08;";

        // always instantiate one HttpClient for application's lifetime and share it. 
        //     private static readonly HttpClient client = new HttpClient();

        string exsitingTrackingQuery = "select ProNum, ELDNum from P44Tracking";
        string isNewTrackingQuery = "select [pro],[reference_id] from reference_numbers where [reference_type] = 'TRUCK_ID'";
        string isDeliverOrPickUpByKraft = "select p.[pro_bill_id],p.[assignment_id],p.leg,p.[bill_of_lading],p.[pick_up],p.[deliver],p.[pick_up_date],p.[pick_up_time],p.[picked_up_on],p.[picked_up_at],p.[delivery_date],p.[delivery_time],p.[delivered_on],p.[delivered_at] from probills as p"
            + " join companies as c1 on p.deliver=c1.company_id"
            + " join companies as c2 on p.pick_up=c2.company_id"
            + " join reference_numbers as r on p.pro_bill_id = r.pro"
            + " where (c1.company_name like '%kraft%' or c2.company_name like '%kraft%')"
            + " and(p.[assignment_id] is not null and p.[assignment_id] <>' ' )"
            + " and p.[status] <> 'CANCELLED'"
           + " and r.reference_type = 'TRUCK_ID'"
        //    + " and p.[pro_bill_id] = 'PRO0767088'";
           + " and p.[pro_bill_id] = @probillNum"
          + " and r.[reference_id] = @trackId";


        //  string testQuery= "select * from probills";
        public async Task Execute(IJobExecutionContext context)
        {

            // job1 , check if there is any new Leg need to be tracked.
            // better to  put this job into a function
            // question 1, how can we make the exsitELDList list to be reuseable...


            List<ReferenceTruck> exsitingTrackingList = new List<ReferenceTruck>();
            List<ReferenceTruck> newTrackingList = new List<ReferenceTruck>();
            List<ReferenceTruck> LegForKraftList = new List<ReferenceTruck>();
            try
            {
                //using (SqlConnection con_sqlsrv01 = new SqlConnection(_con_sqlsrv01))
                //{
                //    con_sqlsrv01.Open();
                //    SqlCommand command = new SqlCommand(exsitELDQuery, con_sqlsrv01);
                //    SqlDataReader dataReader = command.ExecuteReader();
                //    if (!dataReader.HasRows)
                //    {
                //        Console.WriteLine("no info found in existELD");

                //    }
                //    else
                //    {
                //     //    List<ReferenceELD> exsitELDList= new List<ReferenceELD>();
                //        while (dataReader.Read())
                //        {
                //                ReferenceELD exsitELD = new ReferenceELD();
                //                exsitELD.probill = (string)dataReader.GetValue(0);
                //                exsitELD.ELDNum = (string)dataReader.GetValue(1);
                //                exsitELDList.Add(exsitELD);
                //        }
                //    }

                //}
                using (SqlConnection cnn_sql4 = new SqlConnection(_con_sql4))
                {
                    cnn_sql4.Open();
                    SqlCommand command = new SqlCommand(isNewTrackingQuery, cnn_sql4);
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (!dataReader.HasRows)
                    {
                        Console.WriteLine("no info found in existELD");

                    }
                    else
                    {
                        //    List<ReferenceELD> exsitELDList= new List<ReferenceELD>();
                        while (dataReader.Read())
                        {
                            ReferenceTruck newELD = new ReferenceTruck();
                            newELD.probill = (string)dataReader.GetValue(0);
                            newELD.truckId = (string)dataReader.GetValue(1);
                            newTrackingList.Add(newELD);
                        }

                        dataReader.Close();
                        foreach (ReferenceTruck r in newTrackingList)
                        {
                            if (!exsitingTrackingList.Contains(r))
                            {
                                SqlCommand commandCheckIfKraft = new SqlCommand(isDeliverOrPickUpByKraft, cnn_sql4);
                                commandCheckIfKraft.Parameters.Add(new SqlParameter("probillNum", r.probill));
                                commandCheckIfKraft.Parameters.Add(new SqlParameter("trackId", r.truckId));

                                SqlDataReader dataReaderKraft = commandCheckIfKraft.ExecuteReader(CommandBehavior.SingleRow);


                                if (dataReaderKraft.Read())
                                {

                                    string departStartDate = dataReaderKraft["picked_up_on"].ToString().Length == 0 ? HelperFunction.HelperFunction.formatDate(dataReaderKraft["pick_up_date"].ToString(), true, 0) : HelperFunction.HelperFunction.formatDate(dataReaderKraft["picked_up_on"].ToString(), true, 0);
                                    string departStartTime = dataReaderKraft["picked_up_at"].ToString().Length == 0 ? HelperFunction.HelperFunction.formatDate(dataReaderKraft["pick_up_time"].ToString(), false, 0) : HelperFunction.HelperFunction.formatDate(dataReaderKraft["picked_up_at"].ToString(), false, 0);
                                    string departEndtime = dataReaderKraft["picked_up_at"].ToString().Length == 0 ? HelperFunction.HelperFunction.formatDate((dataReaderKraft["pick_up_time"].ToString()), false, 120) : HelperFunction.HelperFunction.formatDate(dataReaderKraft["picked_up_at"].ToString(), false, 120);

                                    string arrivalStartDate = dataReaderKraft["delivered_on"].ToString().Length == 0 ? HelperFunction.HelperFunction.formatDate(dataReaderKraft["delivery_date"].ToString(), true, 0) : HelperFunction.HelperFunction.formatDate(dataReaderKraft["delivered_on"].ToString(), true, 0);
                                    string arrivalStartTime = dataReaderKraft["delivered_at"].ToString().Length == 0 ? HelperFunction.HelperFunction.formatDate(dataReaderKraft["delivery_time"].ToString(), false, 0) : HelperFunction.HelperFunction.formatDate(dataReaderKraft["delivered_at"].ToString(), false, 0);
                                    string arrivalEndtime = dataReaderKraft["delivered_at"].ToString().Length == 0 ? HelperFunction.HelperFunction.formatDate((dataReaderKraft["delivery_time"].ToString()), false, 120) : HelperFunction.HelperFunction.formatDate(dataReaderKraft["delivered_at"].ToString(), false, 120);

                                    string departLocationId = dataReaderKraft["pick_up"].ToString();
                                    string destinationLocationId = dataReaderKraft["deliver"].ToString();

                                    string billOfLading = dataReaderKraft["bill_of_lading"].ToString();

                                    string assignmentId = dataReaderKraft["assignment_id"].ToString();

                                    string conpanyInfoQuery = "select  [company_name] ,[postal],[street_number],[street_name], [city],[state],[country]from companies where company_id =@companyId";

                                    string carrierIdentifierQuery = "select distinct  r.certification , r.certification_number from [resource_certification] as r"
                                        + " join[carrier_assignments] as c on r.resource_id = c.carrier_id "
                                        + " where(certification = 'DOT Number' or  certification = 'MC#' or  certification = 'SCAC') and c.[number]=@assignmentId order by [certification]";

                                    string shipmentIdentifierQuery = "select reference_id from reference_numbers where pro =@probillNum";

                                    dataReaderKraft.Close();
                                    //start point: generate json 
                                    RootObject jsonProLeg = new RootObject();

                                    // get shipmentIdentifiers

                                    // if bill_of_lading is  NULL
                                    ShipmentIdentifier shipmentIdentifier = new ShipmentIdentifier();

                                    if (billOfLading.Length == 0)
                                    {// shipmentIdentifier will be the first po #
                                        shipmentIdentifier = GetShiomentIdentifier(shipmentIdentifierQuery, cnn_sql4, r.probill);
                                    }
                                    else
                                    {
                                        shipmentIdentifier.type = "BILL_OF_LADING";
                                        shipmentIdentifier.value = billOfLading;
                                    }

                                    // get depart location information
                                    ShipmentStop depart = new ShipmentStop();
                                    depart.stopNumber = 1;
                                    depart.appointmentWindow.startDateTime = departStartDate + "T" + departStartTime;
                                    depart.appointmentWindow.endDateTime = departStartDate + "T" + departEndtime;
                                    depart.location = GetLocation(conpanyInfoQuery, cnn_sql4, departLocationId);


                                    // get depart location information
                                    ShipmentStop destination = new ShipmentStop();
                                    destination.stopNumber = 2;
                                    destination.appointmentWindow.startDateTime = arrivalStartDate + "T" + arrivalStartTime;
                                    destination.appointmentWindow.endDateTime = arrivalStartDate + "T" + arrivalEndtime;
                                    destination.location = GetLocation(conpanyInfoQuery, cnn_sql4, destinationLocationId);

                                    // get carrierldentifier

                                    List<CarrierIdentifier> carrierIdentifierList = GetCarrierIdentifiers(carrierIdentifierQuery, cnn_sql4, assignmentId);
                                    CarrierIdentifier carrierIdentifier = new CarrierIdentifier();

                                    if (carrierIdentifierList.Count == 3)
                                    {
                                        carrierIdentifier = HelperFunction.HelperFunction.formatCarrierIdentifierType(carrierIdentifierList.ElementAt(1));
                                    }
                                    else
                                    {
                                        carrierIdentifier = HelperFunction.HelperFunction.formatCarrierIdentifierType(carrierIdentifierList.ElementAt(0));
                                    }
                                    //get equipmentIdentifier
                                    EquipmentIdentifier equipmentIdentifier = new EquipmentIdentifier();

                                    equipmentIdentifier.type = "VEHICLE_ID";
                                    equipmentIdentifier.value = r.truckId;

                                    // integrate all the infomations 
                                    jsonProLeg.carrierIdentifier = carrierIdentifier;
                                    jsonProLeg.shipmentIdentifiers.Add(shipmentIdentifier);
                                    jsonProLeg.shipmentStops.Add(depart);
                                    jsonProLeg.shipmentStops.Add(destination);
                                    jsonProLeg.equipmentIdentifiers.Add(equipmentIdentifier);

                                    var json = JsonConvert.SerializeObject(jsonProLeg);

                                    RestClient restClient = new RestClient("https://test-v2.p-44.com/api/v4/tl/shipments");
                                    //   RestClient restClient = new RestClient("https://cloud.p-44.com/api/v4/tl/shipments");

                                    RestRequest restRequest = new RestRequest(Method.POST);

                                    string base64Auth = HelperFunction.HelperFunction.Base64Encode(ConfigStrings.auth);
                                    //string base64Auth = HelperFunction.HelperFunction.Base64Encode("project44!");
                                    restRequest.AddHeader("Authorization:Basic", base64Auth);
                                    // restRequest.AddHeader("Password", HelperFunction.HelperFunction.Base64Encode("project44!"));

                                    restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
                                    IRestResponse restResponse = restClient.Execute(restRequest);


                                    //       Console.WriteLine(t.Result);
                                    //    Console.ReadLine();

                                    // end point.....
                                    var test = "test";
                                }
                                else
                                {
                                    dataReaderKraft.Close();
                                }

                            }
                        }

                    }
                }

            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("------------can not connect to database-----------------.");
                Console.WriteLine(e.Message);

                Console.ReadKey();
            }

        }

        public Location GetLocation(string conpanyInfoQuery, SqlConnection cnn_sql4, string companyId)
        {

            Location location = new Location();

            SqlCommand commandGetDepart = new SqlCommand(conpanyInfoQuery, cnn_sql4);
            commandGetDepart.Parameters.Add(new SqlParameter("companyId", companyId));

            SqlDataReader getlocation = commandGetDepart.ExecuteReader(CommandBehavior.SingleRow);
            if (getlocation.Read())
            {
                location.address.country = getlocation["country"].ToString();
                location.address.postalCode = location.address.country == "US" ? getlocation["postal"].ToString() : HelperFunction.HelperFunction.formatPostalCode(getlocation["postal"].ToString());
                location.address.addressLines.Add(getlocation["street_number"].ToString() + " " + getlocation["street_name"].ToString());
                location.address.city = getlocation["city"].ToString();
                location.address.state = getlocation["state"].ToString();
                location.contact.companyName = getlocation["company_name"].ToString();
            }

            getlocation.Close();

            return location;

        }


        public ShipmentIdentifier GetShiomentIdentifier(string shipmentIdentifierQuery, SqlConnection cnn_sql4, string probillNum)
        {

            ShipmentIdentifier shipmentIdentifier = new ShipmentIdentifier();

            SqlCommand commandGetShipmentIdentifier = new SqlCommand(shipmentIdentifierQuery, cnn_sql4);
            commandGetShipmentIdentifier.Parameters.Add(new SqlParameter("probillNum", probillNum));


            SqlDataReader getShiomentIdentifier = commandGetShipmentIdentifier.ExecuteReader(CommandBehavior.SingleRow);
            if (getShiomentIdentifier.Read())
            {
                shipmentIdentifier.type = "ORDER";
                shipmentIdentifier.value = getShiomentIdentifier["reference_id"].ToString();

            }


            getShiomentIdentifier.Close();

            return shipmentIdentifier;

        }

        public List<CarrierIdentifier> GetCarrierIdentifiers(string carrierIdentifierQuery, SqlConnection cnn_sql4, string assignmentId)
        {

            List<CarrierIdentifier> carrierIdentifierList = new List<CarrierIdentifier>();

            try
            {
                SqlCommand commandCarrierIdentifier = new SqlCommand(carrierIdentifierQuery, cnn_sql4);
                commandCarrierIdentifier.Parameters.Add(new SqlParameter("assignmentId", assignmentId));

                SqlDataReader getCarrierIdentifier = commandCarrierIdentifier.ExecuteReader();

                if (getCarrierIdentifier.HasRows)
                {
                    while (getCarrierIdentifier.Read())
                    {
                        CarrierIdentifier carrierIdentifier = new CarrierIdentifier();
                        carrierIdentifier.type = getCarrierIdentifier["certification"].ToString();
                        carrierIdentifier.value = getCarrierIdentifier["certification_number"].ToString();
                        carrierIdentifierList.Add(carrierIdentifier);
                    }

                }
                else
                {



                }

                getCarrierIdentifier.Close();

            }
            catch (ArgumentOutOfRangeException e)
            {

                Console.WriteLine(e.Message);
            }

            return carrierIdentifierList;
        }


    }
}