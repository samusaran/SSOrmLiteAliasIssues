using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.Model;

namespace OrmLite.AliasBindingIssue
{
    [Alias("LRDCONTENITORI")]
    public class LRDContenitore : DBObject, IHasId<int>
    {
        private const int CColore = 7; // lunghezza colore HTML es. #AABBCC
        private const int CPrefisso = 5;

        [Alias("IDDCONTENITORE")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Alias("CODICE")]
        [Required]
        [Index(Unique = true)]
        public string Codice { get; set; }

        [Required]
        [Alias("DESCRIZIONE")]
        public string Descrizione { get; set; }

        [Alias("DESCRIZIONEESTESA")]
        public string DescrizioneEstesa { get; set; }

        [Alias("ORDINE")]
        [Required]
        public int Ordine { get; set; }

        [Required]
        [Alias("TIPOCONTENITORE")]
        public int TipoContenitore { get; set; }

        [Alias("COLORE")]
        [StringLength(CColore)]
        public string Colore { get; set; }

        [Alias("PREFISSO")]
        [StringLength(CPrefisso)]
        public string Prefisso { get; set; }

        [Alias("PROGRESSIVOBARCODEMIN")]
        [DecimalLength(30, 0)]
        public decimal ProgressivoBarcodeMin { get; set; }

        [Alias("PROGRESSIVOBARCODEMAX")]
        [DecimalLength(30, 0)]
        [Default(int.MaxValue)]
        public decimal ProgressivoBarcodeMax { get; set; }

        [Alias("DMATERIALEID")]
        public int? MaterialeId { get; set; }

        [Alias("DETICHETTAID")]
        public int? EtichettaId { get; set; }

        [Required]
        [Alias("EMATOLOGIA")]
        public int Ematologia { get; set; }

        [Required]
        [Alias("URINE")]
        public int Urine { get; set; }
    }
}