using System;

namespace CarAuto.ExceptionInterceptor.CustomExceptions
{
    [Serializable]
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string? id = null)
        {
            SetId(id);
        }

        public AlreadyExistsException(string? message, string? id = null)
            : base(message)
        {
            SetId(id);
        }

        public AlreadyExistsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context,
            string? id = null)
            : base(info, context)
        {
            SetId(id);
        }

        public AlreadyExistsException(string? message, Exception? innerException, string? id = null)
            : base(message, innerException)
        {
            SetId(id);
        }

        public string Id { get; set; }

        private void SetId(string? id)
        {
            if (id != null)
            {
                Id = id;
            }
        }
    }
}