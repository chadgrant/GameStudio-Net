using System;

namespace GameStudio.Repository.Document
{
    /// <summary>
    /// Base exception for document repo exceptions
    /// </summary>
    public abstract class DocumentException : ApplicationException
    {
        protected DocumentException(string message, Exception ex) : base(message,ex)
        {
        }
    }

    /// <summary>
    /// Occurs when an optimistic concurrency update fails
    /// Meaning an attempt was made to update a document
    /// that something else updated since the document was last retrieved
    /// </summary>
    public class ConcurrencyException : DocumentException
    {
        public ConcurrencyException(string message, string details, Exception inner) : base(message, inner)
        {
            Details = details;
        }

        public ConcurrencyException(string message, string details) : base(message,null)
        {
            Details = details;
        }

        public string Details { get; set; }
    }

    /// <summary>
    /// Thrown when an attempt to update a document that doesn't exist
    /// </summary>
    public class DocumentNotFoundException : DocumentException
    {
        public DocumentNotFoundException(string message) : base(message, null)
        {
        }
    }

    /// <summary>
    /// Not in use yet, but place holder for when
    /// documents are locked and obtaining a lock failed.
    /// </summary>
    public class LockedException : DocumentException
    {
        protected LockedException(string message, Exception ex) : base(message, ex)
        {
        }
    }

    /// <summary>
    /// When a server throws an exception.
    /// Signals Fault Tolerance should retry
    /// </summary>
    public class DocumentServerException : ApplicationException
    {
        public DocumentServerException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
