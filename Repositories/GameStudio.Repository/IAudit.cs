using System;

namespace GameStudio.Repository
{
    public interface IAudit : ICreated, IUpdated
    {
    }

    public interface ICreated
    {
        DateTime Created { get; set; }
    }

    public interface IUpdated
    { 
        DateTime? Updated { get; set; }
    }

    public interface IAuditor
    {
        string CreatedBy { get; set; }
        string UpdatedBy { get; set; }
    }
}
