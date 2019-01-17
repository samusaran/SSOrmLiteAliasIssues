using System;
using System.Runtime.Serialization;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace OrmLite.AliasBindingIssue
{
    /// <summary>
    /// Classe base di tutti i DBO
    /// questa classe contiene i campi (comuni a tutti i dizionari\archivi) 
    /// usati per tracciare la versione del record e il timestamp dell'ultima modifica
    /// </summary>
    public class DBObject : ICloneable
    {
        [IgnoreDataMember]
        [Alias("DATAMODIFICA")]
        public DateTime DataModifica { get; set; }

        [Default(1)]
        [Alias("VERSIONERECORD")]
        public int VersioneRecord { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}