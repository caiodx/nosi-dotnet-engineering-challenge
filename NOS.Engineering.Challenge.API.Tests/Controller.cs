using NOS.Engineering.Challenge.Managers;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NOS.Engineering.Challenge.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace NOS.Engineering.Challenge.API.Tests
{
    public class Controller
    {
        private readonly IContentsManager _manager;
        public Controller()
        {
            _manager = A.Fake<IContentsManager>();
        }

        [Fact]
        public void ContentController_GetManyContents_ReturnOK()
        {
            var controller = new ContentController(_manager);
            var result = controller.GetManyContents().GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));

        }
    }
}
