using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NOS.Engineering.Challenge.Utils;

namespace NOS.Engineering.Challenge.Tests
{
    public class UtilsTest
    {
        [Fact]
        public void Enviroment_Should_Development_or_Production()
        {
            Enums.GetAppEnviroment().Should().BeOneOf([Enums.AppEnviroment.Production, Enums.AppEnviroment.Development]);
        }
    }
}
