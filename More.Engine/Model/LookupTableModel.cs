using System.Dynamic;

namespace More.Engine.Model
{
    public class LookupTableModel
    {
        public bool Active { get; set; }

        public string ChangeId { get; set; }

        public string Columns { get; set; }

        public string Description { get; set; }

        public int ExceptionId { get; set; }

        public string Id { get; set; }

        public string IsoName { get; set; }

        public string Keys { get; set; }

        public string Name { get; set; }

        public LookupTableModel()
        {
            Properties = new ExpandoObject();
        }

        #region RatingEngineExtensions

        public dynamic Properties { get; set; }

        #endregion RatingEngineExtensions

        #region GetHashCode and Equals

        public override bool Equals(object obj)
        {
            var compare = obj as LookupTableModel;
            if (compare == null)
                return false;

            return Id == compare.Id &&
                   ChangeId == compare.ChangeId &&
                   Name == compare.Name &&
                   Description == compare.Description &&
                   IsoName == compare.IsoName &&
                   Active == compare.Active;
        }

        /// <summary>
        /// http://stackoverflow.com/a/263416
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = !string.IsNullOrEmpty(Id) ? (hash * 23) + Id.GetHashCode() : hash;
                hash = !string.IsNullOrEmpty(ChangeId) ? (hash * 23) + ChangeId.GetHashCode() : hash;
                hash = !string.IsNullOrEmpty(Name) ? (hash * 23) + Name.GetHashCode() : hash;
                hash = !string.IsNullOrEmpty(Description) ? (hash * 23) + Description.GetHashCode() : hash;
                hash = !string.IsNullOrEmpty(IsoName) ? (hash * 23) + IsoName.GetHashCode() : hash;
                hash = (hash * 23) + Active.GetHashCode();

                return hash;
            }
        }

        #endregion GetHashCode and Equals
    }
}