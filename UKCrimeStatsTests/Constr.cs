using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using UKCrimeStats.Controllers;
using FluentAssertions;
using UKCrimeStats.Helpers;

namespace UKCrimeStatsTests
{
    [TestClass]
    public class Constr
    {

        [TestMethod]
        public void NoLatestDate_ShouldUseCurrentMonth()
        {
            //prepare
            var helper = new Mock<HttpHelper>();
            helper.Setup(m => m.GetLatestDateAvailable()).Returns(value: null);
            var controller = new HomeController(helper.Object);

            //assert
            controller.monthsAvailable.Count.Should().Be(HomeController.numOfMonthsAvailable);
        }

        [TestMethod]
        public void ValidLatestDate_ShouldUseIt()
        {
            //prepare
            var helper = new Mock<HttpHelper>();
            helper.Setup(m => m.GetLatestDateAvailable()).Returns(value: new DateTime(2021, 10, 1));
            var controller = new HomeController(helper.Object);

            //assert
            controller.monthsAvailable.Count.Should().Be(HomeController.numOfMonthsAvailable);
            var endDate = new DateTime(2021, 9, 1);
            var startDate = endDate.AddMonths(HomeController.numOfMonthsAvailable * -1);
            while (endDate > startDate)
            {
                var monthStr = endDate.Year.ToString() + "-" + endDate.Month.ToString().PadLeft(2, '0');
                controller.monthsAvailable.Contains(monthStr).Should().BeTrue();
                endDate = endDate.AddMonths(-1);
            }

        }

    }
}
