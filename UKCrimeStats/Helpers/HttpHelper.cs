using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using UKCrimeStats.Models;
using UKCrimeStats.ViewModels;

namespace UKCrimeStats.Helpers
{
    public class HttpHelper
    {
        //Service base url
        string BaseUrl = "https://data.police.uk/";

        public virtual DateTime? GetLatestDateAvailable()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var Res = client.GetAsync("api/crime-last-updated").GetAwaiter().GetResult();
                CrimeLastUpdatedResponse response = null;
                if (Res.IsSuccessStatusCode)
                {
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                    response = JsonConvert.DeserializeObject<CrimeLastUpdatedResponse>(EmpResponse, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });
                    if (response.date != DateTime.MinValue)
                    {
                        return response.date;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public virtual List<AllCrimeResponse> GetCrimeCrimeData(AllCrimesViewModel model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var Res = client.GetAsync(string.Format("api/crimes-street/all-crime?lat={0}&lng={1}&date={2}", model.Latitude, model.Longitude, model.SelectedMonth)).GetAwaiter().GetResult();
                if (Res.IsSuccessStatusCode)
                {
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<List<AllCrimeResponse>>(EmpResponse);
                    return response;
                }
                return null;
            }
        }

    }
}