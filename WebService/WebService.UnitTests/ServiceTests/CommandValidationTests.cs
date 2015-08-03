using System;
using NUnit.Framework;
using WebService.Service;

namespace WebService.UnitTests.ServiceTests
{
    [TestFixture]
    public class CommandControllerTests
    {
        [Test]
        public void CommandValidation_ValidateExampleJson_ReturnTrue()
        {
            const string json = 
                "{\"CommandName\":\"delete\",\"Parameters\":[{\"Key\":\"name\",\"Value\":\"\"},{\"Key\":\"value\",\"Value\":\"\"}]}";

            CommandValidation validation = new CommandValidation();

            Assert.AreEqual(validation.Validation(json), true);
        }

        [Test]
        public void CommandValidation_ValidateEmptyString_ReturnFalse()
        {
            string json = String.Empty;

            CommandValidation validation = new CommandValidation();

            Assert.AreEqual(validation.Validation(json), false);
        }
    }
}
