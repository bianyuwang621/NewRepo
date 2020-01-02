using project44portail.Models.JsonElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace project44portail.Helpers
{
    public class HelperFunction
    {

        private static List<string> stateCa
           = new List<string>()
           {  "AB","BC", "MB", "NB", "NL","NT","NS","NU","ON","PE",
                 "QC","SK","YT","PQ","NF","YK" };




        static public string formatPostalCode(string str)
        {
            string output = str.Trim().Substring(0, 3) + str.Trim().Substring(str.Length - 3, 3);
            return output.ToUpper();
        }


        static public string formatState(string str)
        {
            string output = str;
            switch (output.ToUpper())
            {
                case "PQ":
                    output = "QC";
                    break;
                case "FN":
                    output = "NL";
                    break;
                case "YK":
                    output = "YT";
                    break;
                default:
                    break;

            }
            return output.ToUpper();
        }

        public static string formatDate(string str, bool isDate, int addmins)
        {

            DateTime.TryParse(str, out DateTime dt);
            if (isDate)
                return dt.ToString("yyyy-MM-dd");
            else
            {
                dt = dt.AddMinutes(addmins);
                return dt.ToString("HH:mm:ss");
            }
        }

        public static string Base64Encode(string auth)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(auth);
            return Convert.ToBase64String(plainTextBytes);
        }


        public static string getContryCode(string str)
        {
            bool b = stateCa.Any(s => str.Contains(s));
            if (b)
                return "CA";
            else
                return "US";
        }




        public static string formatPhoneNumber(string str)
        {
            return Regex.Replace(str, "[^0-9]", "");
        }


        static public CarrierIdentifier formatCarrierIdentifierType(CarrierIdentifier carrierIdentifier)
        {
            if (carrierIdentifier.type == "MC#")
                carrierIdentifier.type = "MC_NUMBER";
            if (carrierIdentifier.type == "DOT NUMBER")
                carrierIdentifier.type = "DOT_NUMBER";
            return carrierIdentifier;
        }


    }
}