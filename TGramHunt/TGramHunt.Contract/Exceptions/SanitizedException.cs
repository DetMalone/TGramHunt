using System;
using System.Runtime.Serialization;

namespace TGramHunt.Contract.Exceptions
{
    [Serializable]
    public class SanitizedException : Exception
    {
        public string? RawValue { get; set; }

        public SanitizedException(string message, string rawValue) : this(message)
        {
            this.RawValue = rawValue;
        }

        public SanitizedException()
        {

        }

        public SanitizedException(string message) : base(message)
        {
        }

        public SanitizedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SanitizedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}