using System;

namespace GameStudio.Repository.Document.Tests
{
    [Serializable]
    public class ComplexEntity : IAudit, IAuditor, IVersionable
    {
        public string Id { get; set; }
        public int IntProperty { get; set; }
        public int? NullInt { get; set; }
        public long LongProperty { get; set; }
        public long? NullLong { get; set; }
        public string String { get; set; }
        public DateTime DateProperty { get; set; }
        public DateTime? NullDateTime { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int? Version { get; set; }
    }
}
