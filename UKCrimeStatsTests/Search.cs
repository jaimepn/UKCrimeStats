using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using UKCrimeStats.Controllers;
using UKCrimeStats.ViewModels;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using UKCrimeStats.Models;
using UKCrimeStats.Helpers;

namespace UKCrimeStatsTests
{
    [TestClass]
    public class Search
    {
        [TestMethod]
        public void InvalidCoordinates_ShouldInvalidate()
        {
            //prepare
            var model = new AllCrimesViewModel();
            model.Latitude = 50000;
            model.Longitude = 50000;
            model.SelectedMonth = "Example";
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            //act
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            //assert
            isModelStateValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidCoordinates_ShouldBeOK()
        {
            //prepare
            var model = new AllCrimesViewModel();
            model.Latitude = 55;
            model.Longitude = 0;
            model.SelectedMonth = "Example";
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            //act
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            //assert
            isModelStateValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidRequest_EmptyResponse()
        {
            //prepare
            var helper = new Mock<HttpHelper>();
            helper.Setup(m => m.GetCrimeCrimeData(It.IsAny<AllCrimesViewModel>())).Returns(value: new List<AllCrimeResponse>());
            var controller = new HomeController(helper.Object);
            var model = new AllCrimesViewModel();
            model.Latitude = 55;
            model.Longitude = 0;
            model.SelectedMonth = controller.monthsAvailable.First();

            //act
            controller.Search(model);

            //assert
            model.result.Should().NotBeNull();
            model.result.TotalCount.Should().Be(0);
            model.result.CrimeStats.Count().Should().Be(0);
        }

        [TestMethod]
        public void ValidRequest_ValidResponse()
        {
            //prepare
            var helper = new Mock<HttpHelper>();
            var controller = new HomeController(helper.Object);
            var model = new AllCrimesViewModel();
            model.Latitude = 55;
            model.Longitude = 0;
            model.SelectedMonth = controller.monthsAvailable.First();
            var validResponse = getValidResponse(model.SelectedMonth); //create mock response
            helper.Setup(m => m.GetCrimeCrimeData(It.IsAny<AllCrimesViewModel>())).Returns(value: validResponse);

            //act
            controller.Search(model);

            //assert
            model.result.Should().NotBeNull();
            model.result.TotalCount.Should().Be(2);
            model.result.CrimeStats.Count().Should().Be(2);
            model.result.CrimeStats.Where(x => x.Category == "anti-social-behaviour").Count().Should().Be(1);
            model.result.CrimeStats.Where(x => x.Category == "shoplifting").Count().Should().Be(1);
        }


        private List<AllCrimeResponse> getValidResponse(string month)
        {
            return new List<AllCrimeResponse>
            (
                new AllCrimeResponse[]
                {
                    new AllCrimeResponse()
                    {
                        category = "anti-social-behaviour",
                        location_type = "Force",
                        location = new Location() { latitude = "52", longitude = "-0.009", street = new Street() { id = 1, name = "street1" } },
                        context = "",
                        id = 1,
                        location_subtype = "",
                        month = month,
                        outcome_status = null,
                        persistent_id = ""
                    },
                    new AllCrimeResponse()
                    {
                        category = "shoplifting",
                        location_type = "Force",
                        location = new Location() { latitude = "52.99", longitude = "-0.009", street = new Street() { id = 2, name = "street2" } },
                        context = "",
                        id = 2,
                        location_subtype = "",
                        month = month,
                        outcome_status = null,
                        persistent_id = ""
                    },
                }
            );

        }

    }
}
