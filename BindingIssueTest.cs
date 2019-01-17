using System;
using System.Configuration;
using System.Data.SqlClient;
using NUnit.Framework;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Converters;
using ServiceStack.OrmLite.SqlServer.Converters;

namespace OrmLite.AliasBindingIssue
{
    public class Tests
    {
        private OrmLiteConnectionFactory DbFactory { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var dialectProvider = SqlServer2016Dialect.Provider;
            dialectProvider.RegisterConverter<DateTime>(new SqlServerDateTime2Converter());
            dialectProvider.RegisterConverter<decimal>(new DecimalConverter(28, 6));

            var sqlConnBuilder = new SqlConnectionStringBuilder(ConfigurationManager.AppSettings["SqlServerConnString"]) // change the connection string
            {
                ApplicationName = "Test Runner"
            };

            DbFactory = new OrmLiteConnectionFactory(sqlConnBuilder.ToString(), dialectProvider);
            
            // this affects the projection! why?
            // there should be no "fallback" involved in the following tests,
            // and I don't want any fuzzy matching that you actually implemented (as my previous issue)
            // you either split the fallback in 2 modes, like normal and aggressive, or you set aliases correctly.
            OrmLiteConfig.DisableColumnGuessFallback = true; 
            
            OrmLiteConfig.IsCaseInsensitive = false;
        }

        [SetUp]
        public void Setup()
        {
            using (var conn = DbFactory.OpenDbConnection())
            {
                conn.DropTable<LRDProfiloAnalisi>();
                conn.DropTable<LRDAnalisi>();

                conn.CreateTable<LRDAnalisi>();
                conn.CreateTable<LRDProfiloAnalisi>();

                conn.Insert(new LRDAnalisi
                {
                    Codice = "TEST", 
                    Descrizione = "DESCRIPTION"
                });

                conn.Insert(new LRDProfiloAnalisi
                {
                    AnalisiId = 1,
                    DataModifica = DateTime.UtcNow,
                    VersioneRecord = 1,
                    ProfiloAnalisiId = null
                });
            }
        }

        [Test]
        public void Id_And_AnalisiId_Would_Not_Get_Set_Correctly()
        {
            // this test shows that query fields that shares the same name within the anynomous object field,
            // do not get the AS clause set, even if the actual name on the table is different, because of the AliasAttribute.
            // note that no error gets thrown by the query, but only in the asserts. the issue is "transparent" and could slip under the radar.
            
            using (var conn = DbFactory.OpenDbConnection())
            {
                var q = conn.From<LRDProfiloAnalisi>()
                    .Join<LRDProfiloAnalisi, LRDAnalisi>((dproana, dana) => dproana.AnalisiId == dana.Id)
                    .Where<LRDProfiloAnalisi>(dproana => dproana.ProfiloAnalisiId == null)
                    .Select<LRDProfiloAnalisi, LRDAnalisi>((dproana, dana) =>
                        new //ElementoProfiloAnalisiDTO
                        {
                            Id = dproana.Id, // look at this field
                            ProfiloAnalisiId = 1,
                            AnalisiId = dproana.AnalisiId, // and this field
                            CodiceAnalisi = dana.Codice,
                            DescrizioneAnalisi = dana.Descrizione,
                            VersioneRecord = dproana.VersioneRecord,
                        });
                
                var result = conn.Select<ElementoProfiloAnalisiDTO>(q);
                
                Assert.That(result, Has.Count.GreaterThan(0));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.Id)).GreaterThan(0));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.AnalisiId)).GreaterThan(0));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.CodiceAnalisi)).With.Length.GreaterThan(0));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.DescrizioneAnalisi)).With.Length.GreaterThan(0));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.ProfiloAnalisiId)).EqualTo(1));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.VersioneRecord)).GreaterThan(0));
            }
        }
        
        [Test]
        public void Id_And_AnalisiId_Will_Get_Set_Correctly_Because_I_Set_As_Clause()
        {
            // this test show how I can enforce the alias to be the same as the one on the anonymous object and everything works fine.
            
            using (var conn = DbFactory.OpenDbConnection())
            {
                var q = conn.From<LRDProfiloAnalisi>()
                    .Join<LRDProfiloAnalisi, LRDAnalisi>((dproana, dana) => dproana.AnalisiId == dana.Id)
                    .Where<LRDProfiloAnalisi>(dproana => dproana.ProfiloAnalisiId == null)
                    .Select<LRDProfiloAnalisi, LRDAnalisi>((dproana, dana) =>
                        new //ElementoProfiloAnalisiDTO
                        {
                            Id = Sql.As(dproana.Id, "Id"),
                            ProfiloAnalisiId = 1,
                            AnalisiId = Sql.As(dproana.AnalisiId, "AnalisiId"),
                            CodiceAnalisi = dana.Codice,
                            DescrizioneAnalisi = dana.Descrizione,
                            VersioneRecord = dproana.VersioneRecord,
                        });
                
                var result = conn.Select<ElementoProfiloAnalisiDTO>(q);
                
                Assert.That(result, Has.Count.GreaterThan(0));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.Id)).GreaterThan(0));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.AnalisiId)).GreaterThan(0));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.CodiceAnalisi)).With.Length.GreaterThan(0));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.DescrizioneAnalisi)).With.Length.GreaterThan(0));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.ProfiloAnalisiId)).EqualTo(1));
                Assert.That(result, Has.All.Property(nameof(ElementoProfiloAnalisiDTO.VersioneRecord)).GreaterThan(0));
            }
        }
    }
}