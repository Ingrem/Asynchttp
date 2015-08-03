using System.Web.Mvc;
using NUnit.Framework;
using WebService.Controllers;

namespace WebService.UnitTests.ControllerTests
{
    [TestFixture]
    public class CommandControllerTests
    {
        [Test]
        public void LogsViewModelNotNull()
        {
            var controller = new CommandsController();

            var result = controller.LogsView() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void IndexViewModelNotNull()
        {
            var controller = new CommandsController();

            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }
    }
}
