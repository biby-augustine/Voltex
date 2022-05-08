using System.Runtime.Serialization;

namespace API
{
    public class ErrorResponse
    {
        /// <summary>Gets or sets the messages.</summary>
        /// <value>The messages.</value>
        public IEnumerable<string> Messages { get; set; }

        /// <summary>Gets or sets the exception.</summary>
        /// <value>The exception.</value>
        [DataMember(EmitDefaultValue = false)]
        public string Exception { get; set; }
    }
}
