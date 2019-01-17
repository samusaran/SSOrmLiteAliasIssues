using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.Model;

namespace OrmLite.AliasBindingIssue
{
    [Alias("LRDPROFILOANALISI")]
    [CompositeIndex("DPROFILOANALISIID", "DANALISIID", Unique = true, Name = "IDXPROFILO")]
    public class LRDProfiloAnalisi : DBObject, IHasId<int>
    {
        [Alias("IDDPROFILOANALISI")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [ApiMember(Description = "Analisi profilo a cui appartiene l'analisi")]
        [Alias("DPROFILOANALISIID")]
        [References(typeof(LRDProfiloAnalisi))]
        public int? ProfiloAnalisiId { get; set; } // dove NULL allora DANALISIID e' l'analisi profilo

        [ApiMember(Description = "Analisi dal dizionario")]
        [Alias("DANALISIID")]
        [References(typeof(LRDAnalisi))]
        public int AnalisiId { get; set; }
    }
}