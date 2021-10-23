using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UKCrimeStats.Models
{

    public class Street
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Location
    {
        public string latitude { get; set; }
        public Street street { get; set; }
        public string longitude { get; set; }
    }

    public class OutcomeStatus
    {
        public string category { get; set; }
        public string date { get; set; }
    }

    public class AllCrimeResponse
    {
        public string category { get; set; }
        public string location_type { get; set; }
        public Location location { get; set; }
        public string context { get; set; }
        public OutcomeStatus outcome_status { get; set; }
        public string persistent_id { get; set; }
        public int id { get; set; }
        public string location_subtype { get; set; }
        public string month { get; set; }
    }

}