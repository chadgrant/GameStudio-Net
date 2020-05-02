using System;

namespace GameStudio.Repository.Document.Tests
{
    [Serializable]
    public class SimpleEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = $"Simple Name - {Guid.NewGuid()}";
    }
}
