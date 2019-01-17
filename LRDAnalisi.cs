using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.Model;

namespace OrmLite.AliasBindingIssue
{
    [Alias("LRDANALISI")]
    public class LRDAnalisi : DBObject, IHasId<int>
    {
        [Alias("IDDANALISI")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Alias("CODICE")]
        [Required]
        [Index(Unique = true)]
        public string Codice { get; set; }

        [Alias("DESCRIZIONE")]
        [Required]
        public string Descrizione { get; set; }

        [Alias("DESCRIZIONEESTESA")]
        public string DescrizioneEstesa { get; set; }

        [Alias("CODICEREGIONALE")]
        public string CodiceRegionale { get; set; }

        [Alias("DCONTENITOREID")]
        public int ContenitoreId { get; set; }

        [Alias("ORDINE")]
        public int Ordine { get; set; }

        [Alias("DMETODOID")]
        public int? MetodoId { get; set; }

        [Alias("DPANNELLOANALISIID")]
        public int? PannelloAnalisiId { get; set; }

        [Alias("DCLASSEANALISIID")]
        public int? ClasseAnalisiId { get; set; }

        [Alias("QCREGISTRAZIONERISULTATI")]
        public int QCRegistrazioneRisultati { get; set; }

        [Alias("QCVERIFICA")]
        public int QCVerifica { get; set; }

        [Alias("QCOREINTERVALLOVERIFICA")]
        public int? QCOreIntervalloVerifica { get; set; }
    }
}