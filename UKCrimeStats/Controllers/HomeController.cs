using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UKCrimeStats.Helpers;
using UKCrimeStats.Models;
using UKCrimeStats.ViewModels;

namespace UKCrimeStats.Controllers
{
    public class HomeController : Controller
    {

        public const int numOfMonthsAvailable = 3;
        public List<string> monthsAvailable;

        private HttpHelper httpHelper;

        public HomeController() : this(new HttpHelper()) {}

        public HomeController(HttpHelper httpHelper)
        {
            this.httpHelper = httpHelper;
            InitMonths();
        }

        // GET: Home
        public ActionResult Index()
        {
            var model = new AllCrimesViewModel()
            {
                Months = monthsAvailable,
                SelectedMonth = monthsAvailable.First()
            };
            return View("Index", model);
        }



        public ActionResult Search(AllCrimesViewModel Model)
        {
            Model.Months = monthsAvailable;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return View("Index", Model);
            }
            var response = httpHelper.GetCrimeCrimeData(Model);
            CompileData(Model, response);
            return View("Index", Model);
        }


        private List<string> GetListOfMonths()
        {
            var result = new List<string>();
            var getDateResult = httpHelper.GetLatestDateAvailable();
            //if no date returned, use current date
            var date = getDateResult.HasValue ? getDateResult.Value : DateTime.Now;
            //only use months with complete data
            bool isLastDayOfMonth = (date.Day == DateTime.DaysInMonth(date.Year, date.Month));
            var endDate = isLastDayOfMonth ? new DateTime(date.Year, date.Month, 1) : new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            var startDate = endDate.AddMonths(numOfMonthsAvailable * -1);
            while (endDate > startDate)
            {
                result.Add(endDate.Year.ToString() + "-" + endDate.Month.ToString().PadLeft(2, '0'));
                endDate = endDate.AddMonths(-1);
            }
            return result;
        }

        private void InitMonths()
        {
            monthsAvailable = GetListOfMonths();
        }


        private void CompileData(AllCrimesViewModel model, List<AllCrimeResponse> response)
        {
            if (model.result == null)
            {
                model.result = new SearchResult();
            }
            if (model.result.CrimeStats == null)
            {
                model.result.CrimeStats = new List<CrimeStat>();
            }
            var allCats = response.Select(x => x.category).Distinct();
            foreach (string cat in allCats)
            {
                var stat = new CrimeStat()
                {
                    Category = cat,
                    Count = response.Count(x => x.category == cat)
                };
                model.result.CrimeStats.Add(stat);
            }
            model.result.TotalCount = model.result.CrimeStats.Sum(x => x.Count);
        }


    }
}