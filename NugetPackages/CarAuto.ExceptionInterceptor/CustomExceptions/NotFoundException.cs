using System;

namespace CarAuto.ExceptionInterceptor.CustomExceptions
{
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException(string? id = null)
        {
            SetId(id);
        }

        public NotFoundException(string? message, string? id = null)
            : base(message)
        {
            SetId(id);
        }

        public NotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, string? id = null)
        : base(info, context)
        {
            SetId(id);
        }

        public NotFoundException(string? message, Exception? innerException, string? id = null)
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