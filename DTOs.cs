using System.Collections.Generic;

namespace OrmLite.AliasBindingIssue
{
    public class ElementoProfiloAnalisiDTO
    {
        public int Id { get; set; }

        public int ProfiloAnalisiId { get; set; }

        public int AnalisiId { get; set; }

        public string CodiceAnalisi { get; set; }

        public string DescrizioneAnalisi { get; set; }

        public int VersioneRecord { get; set; }
    }
    
    public class ProfiloAnalisiDTO
    {
        public int Id { get; set; }

        public string Codice { get; set; }

        public string Descrizione { get; set; }

        public int ContenitoreId { get; set; }

        public string ContenitoreCodice { get; set; }

        public string ContenitoreDescrizione { get; set; }

        public int VersioneRecord { get; set; }

        public int AnalisiId { get; set; }

        public IEnumerable<ElementoProfiloAnalisiDTO> Elementi { get; set; }
    }
}