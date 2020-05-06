using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SimpleQuery.Tests
{
    [TestClass]
    public class UnitTestPerformance
    {
       

        private void DropTableCliente(SQLiteConnection conn)
        {
            conn.Execute("Drop table Cliente"); 
        }

        private void CreateTableCliente(SQLiteConnection connection)
        {
            var scriptBuilder = connection.GetScriptBuild();
            connection.Execute(scriptBuilder.GetCreateTableCommand<Cliente>());
        }
        [TestMethod]
        public void TestSimpleQuerySqliteInsert()
        {
            var conn = new SQLiteConnection($"Data Source={UnitTestExtentions.GetFileNameDb()}");

            CreateTableCliente(conn);

            var dateTimeBefore = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                var cliente = new Cliente { Nome = $"Cliente {i}" };
                conn.InsertReturningId(cliente);
            }
            var dateTimeAfter = DateTime.Now;

            DropTableCliente(conn);

            System.Diagnostics.Debug.WriteLine(dateTimeAfter.Subtract(dateTimeBefore).Seconds);
        }
    }
}
