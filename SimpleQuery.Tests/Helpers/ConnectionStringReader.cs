using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleQuery.Tests
{
    public class ConnectionStringReader
    {
        public static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            var configuration = builder.Build();


            return configuration;
        }

        public static string GetSqlServerConnstring()
        {
            var config = GetConfiguration();

            var connectionstring = ConfigurationExtensions.GetConnectionString(
              config, "sqlserver");

            return connectionstring;
        }        
        
    }
}
