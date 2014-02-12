using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace More.Services.WebAPI.Models
{
    [DataContract]
    public class DomainTableModel
    {
        private IList<DomainTableColumnModel> _columns;

        public DomainTableModel()
        {
            _columns = new List<DomainTableColumnModel>();
            Group = string.Empty;
        }
        
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Group { get; set; }

        [DataMember]
        public string TableType { get; set; }

        [DataMember]
        public DateTime EffectiveDate { get; set; }

        [DataMember]
        public DomainTableColumnModel[] Columns
        {
            get
            {
                return _columns != null ? _columns.ToArray() : (_columns = new List<DomainTableColumnModel>()).ToArray();
            }

            set
            {
                _columns = value != null ? value.ToList() : new List<DomainTableColumnModel>();
            }
        }

        [DataMember]
        public IList<Dictionary<string, string>> Rows { get; set; }
    }

    [DataContract]
    public class DomainTableColumnModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string MatchType { get; set; }

        [DataMember]
        public bool IsKey { get; set; }
    }
}