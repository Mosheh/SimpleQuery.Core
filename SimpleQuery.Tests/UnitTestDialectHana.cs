using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using SimpleQuery.Data.Dialects;
using SimpleQuery.Domain.Data.Dialects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SimpleQuery.Tests
{
    [TestClass]
    public class UnitTestDialectHana
    {
        [TestMethod]
        public void TestHanaDeleteScript()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true };

            var sqlDelete = builder.GetDeleteCommand<Cliente>(cliente, 1);
            var resultadoEsperado = "delete from \"Cliente\" where \"Id\"=1";

            Assert.AreEqual(resultadoEsperado, sqlDelete);
        }
        [TestMethod]
        public void TestHanaInsertUsingParamsScript()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true };

            var script = builder.GetInsertCommandParameters<Cliente>(cliente);
            var resultadoEsperado = "insert into \"Cliente\" (\"Nome\", \"Ativo\", \"TotalPedidos\", \"ValorTotalNotasFiscais\", \"Credito\", \"UltimoValorDeCompra\") values (@Nome, @Ativo, @TotalPedidos, @ValorTotalNotasFiscais, @Credito, @UltimoValorDeCompra)";

            Assert.AreEqual(resultadoEsperado, script.Item1, false);
        }

        [TestMethod]
        public void TestInsertOperationWithAttributeTableHana()
        {
            var hanaConnection = System.Data.Common.DbProviderFactories.GetFactory("Sap.Data.Hana").CreateConnection();
            hanaConnection.ConnectionString = ConfigurationManager.ConnectionStrings["hana"].ConnectionString;
            hanaConnection.Open();
            var trans = hanaConnection.BeginTransaction();
            using (var conn = hanaConnection)
            {
                IScriptBuilder builder = new ScriptHanaBuilder();

                var user = new UserSystem() { Name = "Moisés", Email = "moisesjosemiranda@gmail.com", LoginName = "mosheh" };

                var createTableScript = builder.GetCreateTableCommand<UserSystem>();
                builder.Execute(createTableScript, conn);

                var lastId = conn.InsertReturningId<UserSystem>(user);
                Assert.AreEqual(1, lastId);

                trans.Rollback();
                builder.Execute("drop table \"IV_MD_SystemUser\"", hanaConnection);
            }
        }

        [TestMethod]
        public void TestHanaInsertScript()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true, TotalPedidos = 20, ValorTotalNotasFiscais = 2000.95, Credito = 10, UltimoValorDeCompra = 1000.95m };

            var sqlDelete = builder.GetInsertCommand<Cliente>(cliente);
            var resultadoEsperado = "insert into \"Cliente\" (\"Nome\", \"Ativo\", \"TotalPedidos\", \"ValorTotalNotasFiscais\", \"Credito\", \"UltimoValorDeCompra\") values ('Moisés', true, 20, 2000.95, 10, 1000.95)";

            Assert.AreEqual(resultadoEsperado, sqlDelete);
        }



        [TestMethod]
        public void TestHanaInsertContractTableScript()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var contract = TestData.GetContract();

            var sqlInsertCommand = builder.GetInsertCommand(contract);
            var resultadoEsperado = $"insert into \"Contract\" (\"Status\", \"ContractDate\", \"Proposal\", \"BusinessPartner\", \"BusinessPartnerName\", \"TypeContract\", \"Undertaking\", \"UndertakingName\", \"UndertakingBlock\", \"UndertakingUnit\", \"UndertakingUnitName\", \"Property\", \"PropertyRate\", \"Classification\", \"ClassificationDate\", \"RenegotiationRate\", \"AnticipationRate\", \"AnticipationRateTP\", \"TypeFineResidue\", \"FineRateResidue\", \"DeliveryDate\", \"SignatureDate\", \"CostCenter\", \"ContractOrigin\", \"ProRataLow\", \"ProRataDiscount\", \"ProRataLowDelay\", \"ProRataResidue\", \"ProRataAnticipation\", \"DirectComission\", \"TotalComission\", \"GenerateCreeditLetterOnOverPayment\", \"StatusDate\", \"Comments\", \"TypeCalcMora\", \"DocTotal\", \"SaleValue\", \"AssignmentRights\", \"AdjustmentOnlyCalculateDueDate\", \"NotGenerateNegativeMonetaryCorrection\", \"LagOfInterestDay\", \"LagOfFine\", \"AccPeriodId\", \"MonthsOfGrowthForCorrectionYearly\", \"AccountNumber\", \"Accounted\", \"InstructionBoE\", \"BlockPaymentWithFutureDate\", \"CostUnit\", \"EnableDiscountPunctuality\", \"NumberInstallmentsPunctuality\", \"RealEstateTransferDays\", \"AdministrationRate\", \"RentalTransferGuaranteed\") values (5, '{DateTime.Now.ToString("yyyy-MM-dd")}', 5, 'C001', 'MOISÉS J. MIRANDA', 1, 2, 'Gran Ville', '15', 21, 'GRAN House', 5, 1.2, 5, null, 2, 1.5, 1.6, null, 2, '2019-01-16', '{DateTime.Now.ToString("yyyy-MM-dd")}', '1.1', null, true, false, null, null, true, 5000, 1000, false, '{DateTime.Now.ToString("yyyy-MM-dd")}', 'comments', 1, 25382000.99, 30000000.00, false, true, true, 0, 0, 1, 1, 123, true, 'payment credit card', true, 830, true, 12, 10, 2, false)";

            Assert.AreEqual(resultadoEsperado, sqlInsertCommand);
        }

        [TestMethod]
        public void TestHanaWhere()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true, TotalPedidos = 55, ValorTotalNotasFiscais = 1000.55, Credito = 2000.53m, UltimoValorDeCompra = 1035.22m };

            var whereScript = builder.GetWhereCommand<Cliente>(c => c.Id == 1);
            var resultadoEsperado = "where (\"Id\" = 1)";

            Assert.AreEqual(resultadoEsperado, whereScript);
        }

        [TestMethod]
        public void TestHanaUpdateScript()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true, TotalPedidos = 55, ValorTotalNotasFiscais = 1000.55, Credito = 2000.53m, UltimoValorDeCompra = 1035.22m };

            var sqlUpdate = builder.GetUpdateCommand<Cliente>(cliente);
            var resultadoEsperado = "update \"Cliente\" set \"Nome\"='Moisés', \"Ativo\"=true, \"TotalPedidos\"=55, \"ValorTotalNotasFiscais\"=1000.55, \"Credito\"=2000.53, \"UltimoValorDeCompra\"=1035.22 where \"Id\"=1";

            Assert.AreEqual(resultadoEsperado, sqlUpdate);
        }

        [TestMethod]
        public void TestHanaUpdateScriptComValorNulo()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true, TotalPedidos = 55, ValorTotalNotasFiscais = 1000.55, Credito = 2000.53m, UltimoValorDeCompra = null };

            var sqlUpdate = builder.GetUpdateCommand<Cliente>(cliente);
            var resultadoEsperado = "update \"Cliente\" set \"Nome\"='Moisés', \"Ativo\"=true, \"TotalPedidos\"=55, \"ValorTotalNotasFiscais\"=1000.55, \"Credito\"=2000.53, \"UltimoValorDeCompra\"=null where \"Id\"=1";

            Assert.AreEqual(resultadoEsperado, sqlUpdate);
        }

        [TestMethod]
        public void TestHanaSelectScript()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true };

            var sqlUpdate = builder.GetSelectCommand<Cliente>(cliente);
            var resultadoEsperado = "select \"Id\", \"Nome\", \"Ativo\", \"TotalPedidos\", \"ValorTotalNotasFiscais\", \"Credito\", \"UltimoValorDeCompra\" from \"Cliente\"";

            Assert.AreEqual(resultadoEsperado, sqlUpdate);
        }



        [TestMethod]
        public void TestHanaSelectWithWhereScript()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true };

            var selectWithWhereCommand = builder.GetSelectCommand<Cliente>(cliente, c => c.Id == 1);
            var resultadoEsperado = "select \"Id\", \"Nome\", \"Ativo\", \"TotalPedidos\", \"ValorTotalNotasFiscais\", \"Credito\", \"UltimoValorDeCompra\" from \"Cliente\" where (\"Id\" = 1)";

            Assert.AreEqual(resultadoEsperado, selectWithWhereCommand);
        }

        [TestMethod]
        public void TestInsertOperationHana()
        {

            var hanaConnection = System.Data.Common.DbProviderFactories.GetFactory("Sap.Data.Hana").CreateConnection();
            hanaConnection.ConnectionString = ConnectionStringReader.GetSqlServerConnstring();
            hanaConnection.Open();
            var trans = hanaConnection.BeginTransaction();
            using (var conn = hanaConnection)
            {
                IScriptBuilder builder = new ScriptHanaBuilder();

                var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true };

                var createTableScript = builder.GetCreateTableCommand<Cliente>();
                builder.Execute(createTableScript, conn);

                var lastId = conn.InsertReturningId<Cliente>(cliente);
                Assert.AreEqual(1, lastId);

                trans.Rollback();
                builder.Execute("drop table \"Cliente\"", hanaConnection);
            }
        }

        [TestMethod]
        public void TestInsertOperationHanaShortTypes()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Employee() { Id = 1, Name = "Moisés", Age = 25 };
            var hanaConnection = new HanaConnection();
            hanaConnection.ConnectionString = ConnectionStringReader.GetConnstring("hana");
            hanaConnection.Open();
            var trans = hanaConnection.BeginTransaction();

            var createTableScript = builder.GetCreateTableCommand<Employee>();
            hanaConnection.Execute(createTableScript);

            hanaConnection.Insert<Employee>(cliente);

            var employeeFromDatabase = hanaConnection.GetAll<Employee>();

            Assert.AreEqual(1, employeeFromDatabase.Count());

            hanaConnection.Execute("drop table \"Employee\"");
        }

        [TestMethod]
        public void TestSelectOperationHana()
        {
            var hanaConnection = System.Data.Common.DbProviderFactories.GetFactory("Sap.Data.Hana").CreateConnection();
            hanaConnection.ConnectionString = ConfigurationManager.ConnectionStrings["hana"].ConnectionString;
            hanaConnection.Open();
            var trans = hanaConnection.BeginTransaction();
            using (var conn = hanaConnection)
            {
                IScriptBuilder builder = new ScriptHanaBuilder();

                var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true };
                var cliente2 = new Cliente() { Id = 2, Nome = "José", Ativo = true };

                var createTableScript = builder.GetCreateTableCommand<Cliente>();
                var insertScript1 = builder.GetInsertCommand<Cliente>(cliente);
                var insertScript2 = builder.GetInsertCommand<Cliente>(cliente2);
                builder.Execute(createTableScript, conn);
                builder.Execute(insertScript1, conn);
                builder.Execute(insertScript2, conn);

                var clientes = conn.GetAll<Cliente>();
                Assert.AreEqual(2, clientes.Count());
                Assert.AreEqual("Moisés", clientes.ToList()[0].Nome);
                Assert.AreEqual("José", clientes.ToList()[1].Nome);

                trans.Rollback();
                builder.Execute("drop table \"Cliente\"", hanaConnection);
            }
        }

        [TestMethod]
        public void TestHanaCreateTableScript()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Cliente() { Id = 1, Nome = "Moisés", Ativo = true };

            var createTableScript = builder.GetCreateTableCommand<Cliente>();
            var resultadoEsperado = "create column table \"Cliente\" (\"Id\" INTEGER not null primary key generated by default as IDENTITY, \"Nome\" VARCHAR(255), \"Ativo\" BOOLEAN, \"TotalPedidos\" INTEGER, \"ValorTotalNotasFiscais\" DOUBLE, \"Credito\" DECIMAL(18,6), \"UltimoValorDeCompra\" DECIMAL(18,6))";

            Assert.AreEqual(resultadoEsperado, createTableScript);
        }

        [TestMethod]
        public void TestHanaCreateTableScriptShortTypes()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var cliente = new Employee() { Id = 1, Name = "Moisés", Age = 25 };

            var createTableScript = builder.GetCreateTableCommand<Employee>();
            var resultadoEsperado = "create column table \"Employee\" (\"Id\" SMALLINT primary key generated by default as IDENTITY, \"Age\" SMALLINT, \"Name\" VARCHAR(255))";

            Assert.AreEqual(resultadoEsperado, createTableScript);
        }

        [TestMethod]
        public void TestHanaCreateTableScriptShorts()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var connStr = ConnectionStringReader.GetConnstring("hana");
            var conn = new HanaConnection(connStr);
            var sql = $@"SELECT 
	                [OUSR].[USER_CODE], 
	                [OUSR].[USERID], 
	                [OUSR].[U_NAME], 
	                [dpto].[Name] as [DepartmentName],
	                [OUSR].[Department],
	                [OUSR].[GENDER],
	                [OUSR].[objType],
	                [OUSR].[userSign],
	                [OUSR].[SUPERUSER],
	                [OUSR].[updateDate]
                    FROM 
	                [OUSR] left join 
	                [OUDP] [dpto] on  [dpto].[Code] = [OUSR].[Department]".Replace("[", "\"").Replace("]", "\"");
            var users = conn.Query<OUSR>(sql);

        }

        [TestMethod]
        public void TestHanaCreateContractTableScript()
        {
            IScriptBuilder builder = new ScriptHanaBuilder();

            var contract = new Contract() { ID = 1, BusinessPartner = "123", DocTotal = 10000, SignatureDate = DateTime.Now, ContractDate = DateTime.Now };

            var createTableScript = builder.GetCreateTableCommand<Contract>();
            var resultadoEsperado = "create column table \"Contract\" (\"ID\" INTEGER not null primary key generated by default as IDENTITY, \"Status\" INTEGER, \"ContractDate\" Date, \"Proposal\" INTEGER, \"BusinessPartner\" VARCHAR(255), \"BusinessPartnerName\" VARCHAR(255), \"TypeContract\" INTEGER, \"Undertaking\" INTEGER, \"UndertakingName\" VARCHAR(255), \"UndertakingBlock\" VARCHAR(255), \"UndertakingUnit\" INTEGER, \"UndertakingUnitName\" VARCHAR(255), \"Property\" DECIMAL(18,6), \"PropertyRate\" DECIMAL(18,6), \"Classification\" INTEGER, \"ClassificationDate\" Date, \"RenegotiationRate\" DECIMAL(18,6), \"AnticipationRate\" DECIMAL(18,6), \"AnticipationRateTP\" DECIMAL(18,6), \"TypeFineResidue\" INTEGER, \"FineRateResidue\" DECIMAL(18,6), \"DeliveryDate\" Date, \"SignatureDate\" Date, \"CostCenter\" VARCHAR(255), \"ContractOrigin\" INTEGER, \"ProRataLow\" BOOLEAN, \"ProRataDiscount\" BOOLEAN, \"ProRataLowDelay\" BOOLEAN, \"ProRataResidue\" BOOLEAN, \"ProRataAnticipation\" BOOLEAN, \"DirectComission\" DECIMAL(18,6), \"TotalComission\" DECIMAL(18,6), \"GenerateCreeditLetterOnOverPayment\" BOOLEAN, \"StatusDate\" Date, \"Comments\" VARCHAR(255), \"TypeCalcMora\" INTEGER, \"DocTotal\" DECIMAL(18,6), \"SaleValue\" DECIMAL(18,6), \"AssignmentRights\" BOOLEAN, \"AdjustmentOnlyCalculateDueDate\" BOOLEAN, \"NotGenerateNegativeMonetaryCorrection\" BOOLEAN, \"LagOfInterestDay\" INTEGER, \"LagOfFine\" INTEGER, \"AccPeriodId\" INTEGER, \"MonthsOfGrowthForCorrectionYearly\" INTEGER, \"AccountNumber\" INTEGER, \"Accounted\" BOOLEAN, \"InstructionBoE\" VARCHAR(255), \"BlockPaymentWithFutureDate\" BOOLEAN, \"CostUnit\" DECIMAL(18,6), \"EnableDiscountPunctuality\" BOOLEAN, \"NumberInstallmentsPunctuality\" INTEGER, \"RealEstateTransferDays\" INTEGER, \"AdministrationRate\" DECIMAL(18,6), \"RentalTransferGuaranteed\" BOOLEAN)";

            Assert.AreEqual(resultadoEsperado, createTableScript);
        }

        [Table("IV_MD_SystemUser")]
        public class UserSystem
        {
            public int Id { get; set; }
            public string LoginName { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Cliente
        {
            private string HashId { get; set; }
            public int Id { get; set; }
            public string Nome { get; set; }
            public bool Ativo { get; set; }
            public int? TotalPedidos { get; set; }
            public double ValorTotalNotasFiscais { get; set; }
            public decimal Credito { get; set; }
            public decimal? UltimoValorDeCompra { get; set; }
        }

        public class Employee
        {
            public short Id { get; set; }
            public short? Age { get; set; }
            public string Name { get; set; }
        }

        [Table("IV_C1_UserSettings")]
        public class UserSettings
        {
            public static readonly int DEFAULT_MAX_RECORDS_PER_SEARCH = 100;
            public static readonly string TABLE = "IV_C1_UserSettings";
            public struct FieldNames
            {
                public static readonly string Theme = "Theme";
                public static readonly string UserCode = "UserCode";
                public static readonly string MaxRecordsPerSearch = "MaxRecordsPerSearch";
            }

            public UserSettings()
            {
                MaxRecordsPerSearch = DEFAULT_MAX_RECORDS_PER_SEARCH;
            }

            public string Theme { get; set; }
            public int MaxRecordsPerSearch { get; set; }
            public string UserCode { get; set; }
            [Key]
            public int Id { get; set; }

            public bool isUpdate()
            {
                var isUpdate = this.Id > decimal.Zero;
                return isUpdate;
            }

            public string TableName()
            {
                return "IV_C1_UserSettings";
            }

            public string GetIdentity()
            {
                return this.Id.ToString();
            }
        }

        [Table("IV_C1_CompanySource")]
        public class CompanySource
        {
            public CompanySource() { }
            [Key]
            public int Id { get; set; }
            public string CompanyDb { get; set; }
            public string CompanyName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            [NotMapped]
            public string HostDB { get; set; }
            [NotMapped]
            public string UserDB { get; set; }
            [NotMapped]
            public string PasswordDB { get; set; }
            public int? BusinessPlaceId { get; set; }
            public string BusinessPlaceName { get; set; }
            public int? ServerType { get; set; }
            public string LicenseServer { get; set; }
            public bool? Default { get; set; }




        }

        [Table("OUSR")]
        public class OUSR
        {
            public OUSR()
            {
                UserSettings = new UserSettings();
                DefaultCompany = new CompanySource();
            }
            public OUSR(string userCode, string name)
            {
                UserSettings = new UserSettings();
                DefaultCompany = new CompanySource();
            }
            public short USERID { get; set; }
            public string USER_CODE { get; set; }
            public string U_NAME { get; set; }
            public string E_Mail { get; set; }
            public short? Department { get; set; }
            public string SUPERUSER { get; set; }
            [NotMapped] public string DepartmentName { get; set; }
            public short? Branch { get; set; }
            public string Picture { get; set; }
            public string Address { get; set; }
            public string Country { get; set; }
            public string Tel1 { get; set; }
            public string GENDER { get; set; }
            public DateTime? Birthday { get; set; }
            public string objType { get; set; }
            public short? userSign { get; set; }
            public DateTime? createDate { get; set; }
            public DateTime? updateDate { get; set; }
            [NotMapped] public virtual UserSettings UserSettings { get; set; }
            [NotMapped] public CompanySource DefaultCompany { get; set; }
            [NotMapped] public Setting Setting { get; set; }
        }

        [Table("IV_C1_Setting")]
        public class Setting
        {
            [Key]
            public int Id { get; set; }
            public int? CompanyFromId { get; set; }
            public bool? IsMandatoryDimentionOnContract { get; set; }
            public bool? LockMeasurementDocDate { get; set; }
            public bool? TransValidation { get; set; }
            public bool? CheckUnitMeasureItem { get; set; }
            public bool? AllowProvisionOfRetentionsUnrelated { get; set; }
            public bool? ShowDownPaysByContract { get; set; }
            public bool? UseTaxLogon { get; set; }
            public bool? ContractForceDisapprovalReason { get; set; }
            public bool? AdditiveForceDisapprovalReason { get; set; }
            public bool? DownPayForceDisapprovalReason { get; set; }
            public bool? DisagreementForceDisapprovalReason { get; set; }
            public bool? MsrForceDisapprovalReason { get; set; }
            public bool? GuaranteeForceDisapprovalReason { get; set; }
            public decimal? ValueDiffAllowAdjustment { get; set; }
            public int? CustomerUsageId { get; set; }
            public int? SupplierUsageId { get; set; }
            public bool? EnableFreightAdditionalExpenses { get; set; }
            public int? AdditionalExpCodeFreight { get; set; }
            public bool? ContractFromQuotation { get; set; }
            public bool? GroupApportionmentItem { get; set; }
            public int? UsageOfFreight { get; set; }
            public string FreightItem { get; set; }
            public bool? HideHostCenter { get; set; }
            public bool ProjectInContract { get; set; }
            public bool? RestrictContractByProject { get; set; }
            public bool? AllowChangeTaxesApproveMeasurement { get; set; }
            public bool? GenerateRetainedTaxProvision { get; set; }
            public bool? CreateContractAutomatic { get; set; }
            public bool? ActivateContractWhenApproving { get; set; }
            public bool? LinkDocIntoApprovedMeasurement { get; set; }
            public decimal? ValueToConsiderLowOfDecimalBalance { get; set; }
            public bool? ExtendToBillingDate { get; set; }
            public bool? ReleaseDeliveryWhenApprovingContract { get; set; }
            public bool? ReleaseDeliveryWhenApprovingAdditive { get; set; }
            public bool? NotCloseContractPendingDocLinked { get; set; }
            public bool? ChangeCostCenterOnContractLine { get; set; }
            public int CompanyLogoId { get; set; }
            public decimal? ValueToConsiderLowOfDecimalQuantity { get; set; }
            public bool? CashFlowIncludeSupplierContract { get; set; }
            public bool? CashFlowIncludeSupplierContractDetail { get; set; }
            public bool? CashFlowIncludeCustomerContract { get; set; }
            public bool? CashFlowIncludeCustomerContractDetail { get; set; }
            public bool? GroupInvoiceByCustomer { get; set; }
            public bool? EditContractheaderFields { get; set; }
            public bool PurchaseRequestInSAP { get; set; }
            public string WhsCodeDefault { get; set; }
            public int DimensionProject { get; set; }

            public bool? WorkPlanRequired { get; set; }

            public bool? ProjectRequired { get; set; }

            public bool? UserViewOnlyOwnDocuments { get; set; }

            public bool? EnableScriptApproval { get; set; }

            public bool? EnableCostCenterInDocument { get; set; }


            public bool? EnableUsageMarketingDocuments { get; set; }

            public int PurchaseInvoiceDocNumber { get; set; }
            public int? PurchaseObjectType { get; set; }

            public bool ConsiderParameterReleaseInstallment { get; set; }
            public bool UseTaxDeterminationIRFSAP { get; set; }
            public bool ChangeTheDetailsOfTheContract { get; set; }
            public bool ChangeCenterCostInMensurement { get; set; }
            public bool ChangeWithholdingTaxesInProvision { get; set; }
            public bool RestrictContractByClassification { get; set; }
            public string GuaranteeTaxDefault { get; set; }
            public bool CloseContractsWithBalance { get; set; }
            public bool? GeneratePurchaseOrderWhenAprovingMsr { get; set; }
            public bool ConsiderBillingDateOnBilling { get; set; }
            public bool UsePriceListToAddProviderContracts { get; set; }
            public bool UsePriceListToAddCustomerContracts { get; set; }
            public bool UsePriceListToAddProviderAdditives { get; set; }
            public bool UsePriceListToAddCustomerAdditives { get; set; }
            public bool EnableAutomaticAlternativeNumber { get; set; }
            public bool ConsiderDueDateOnReleaseInstGroup { get; set; }

            [NotMapped] public string CustomerUsageName { get; set; }
            [NotMapped] public string GuaranteeTaxDefaultName { get; set; }
            [NotMapped] public string UsageOfFreightName { get; set; }
            [NotMapped] public string FreightItemName { get; set; }
            [NotMapped] public string SupplierUsageName { get; set; }
            [NotMapped] public bool SapMultiBranchEnabled { get; set; }

        }
    }
}
