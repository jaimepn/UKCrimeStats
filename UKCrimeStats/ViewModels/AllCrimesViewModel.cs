using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UKCrimeStats.ViewModels
{
    public class AllCrimesViewModel
    {

        [Required]
        [Range(50.10319, 60.15456)] //uk limits
        public double Latitude { get; set; }

        [Required]
        [Range(-7.64133, 1.75159)] //uk limits
        public double Longitude { get; set; }

        public IEnumerable<string> Months { get; set; }

        [Required]
        [Display(Name ="Month")]
        public string SelectedMonth { get; set; }

        public SearchResult result { get; set; }
    }


    public class SearchResult
    {
        public List<CrimeStat> CrimeStats { get; set; }
        public int TotalCount { get; set; }
    }

    public class CrimeStat
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
}