using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Utils
{
    public class Enums
    {
        public enum AppEnviroment
        {
            Development,
            Production
        }

        public static AppEnviroment GetAppEnviroment()
        {
           return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" ? AppEnviroment.Production : AppEnviroment.Development;
        }
    }
}
