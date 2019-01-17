using System;
using System.Configuration;
using System.Data.SqlClient;
using NUnit.Framework;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Converters;
using ServiceStack.OrmLite.SqlServer.Converters;

namespace OrmLite.AliasBindingIssue
{
    public class TableAliasIssueTest
    {
        private OrmLiteConnectionFactory DbFactory { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var dialectProvider = SqlServer2016Dialect.Provider;
            dialectProvider.RegisterConverter<DateTime>(new SqlServerDateTime2Converter());
            dialectProvider.RegisterConverter<decimal>(new DecimalConverter(28, 6));
            dialectProvider.GetStringConverter().UseUnicode = true;

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
                conn.DropTable<LRDContenitore>();
                
                conn.CreateTable<LRDContenitore>();
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
        public void Table_Alias()
        {
            using (var conn = DbFactory.OpenDbConnection())
            {
                var q = conn.From<LRDAnalisi>()
                    .Join<LRDAnalisi, LRDContenitore>((dana, dcont) => dana.ContenitoreId == dcont.Id, conn.TableAlias("c"))
                    .Join<LRDAnalisi, LRDProfiloAnalisi>((dana, dprofana) => dana.Id == dprofana.AnalisiId, conn.TableAlias("dprofana"))
                    .Where<LRDProfiloAnalisi>(dprofana => Sql.TableAlias(dprofana.ProfiloAnalisiId, "dprofana") == null)
                    .SelectDistinct<LRDAnalisi, LRDProfiloAnalisi, LRDContenitore>((dana, dprofana, dcont) =>
                        new //ProfiloAnalisiDTO
                        {
                            Id = Sql.TableAlias(dprofana.Id, "dprofana"),
                            AnalisiId = dana.Id,
                            Codice = dana.Codice,
                            Descrizione = dana.Descrizione,
                            ContenitoreId = Sql.TableAlias(dcont.Id, "c"),
                            ContenitoreCodice = Sql.TableAlias(dcont.Codice, "c"),
                            ContenitoreDescrizione = Sql.TableAlias(dcont.Descrizione, "c"),
                            VersioneRecord = Sql.TableAlias(dprofana.VersioneRecord, "dprofana")
                        });
                
                var result = conn.Select<ProfiloAnalisiDTO>(q);
            }
        }
        
        [Test]
        public void Join_Alias()
        {
            using (var conn = DbFactory.OpenDbConnection())
            {
                var q = conn.From<LRDAnalisi>()
                    .Join<LRDAnalisi, LRDContenitore>((dana, dcont) => dana.ContenitoreId == dcont.Id, conn.JoinAlias("c"))
                    .Join<LRDAnalisi, LRDProfiloAnalisi>((dana, dprofana) => dana.Id == dprofana.AnalisiId, conn.JoinAlias("dprofana"))
                    .Where<LRDProfiloAnalisi>(dprofana => Sql.JoinAlias(dprofana.ProfiloAnalisiId, "dprofana") == null)
                    .SelectDistinct<LRDAnalisi, LRDProfiloAnalisi, LRDContenitore>((dana, dprofana, dcont) =>
                        new //ProfiloAnalisiDTO
                        {
                            Id = Sql.JoinAlias(dprofana.Id, "dprofana"),
                            AnalisiId = dana.Id,
                            Codice = dana.Codice,
                            Descrizione = dana.Descrizione,
                            ContenitoreId = Sql.JoinAlias(dcont.Id, "c"),
                            ContenitoreCodice = Sql.JoinAlias(dcont.Codice, "c"),
                            ContenitoreDescrizione = Sql.JoinAlias(dcont.Descrizione, "c"),
                            VersioneRecord = Sql.JoinAlias(dprofana.VersioneRecord, "dprofana")
                        });
                
                var result = conn.Select<ProfiloAnalisiDTO>(q);
            }
        }
    }
}