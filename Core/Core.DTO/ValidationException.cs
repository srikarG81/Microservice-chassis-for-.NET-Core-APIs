// <copyright file="ValidationException.cs" company="NA">
//NA
// </copyright>

using System.Collections.Generic;

namespace Core.DTO
{
    /// <summary>
    /// Class for custom application error.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "not required here")]
    public class ValidationException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        public ValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="errorDetails">list of error details.</param>
        public ValidationException(List<ErrorDetail> errorDetails)
            : base(errorDetails)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="statusCode">status code.</param>
        /// <param name="message">message.</param>
        public ValidationException(string message)
            : base(null, message)
        {
        }
    }
}