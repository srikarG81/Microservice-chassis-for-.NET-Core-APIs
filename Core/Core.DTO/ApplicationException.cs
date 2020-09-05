// <copyright file="ApplicationException.cs" company="NA">
//NA
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.DTO
{
    /// <summary>
    /// Class for custom application error.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "not required here")]
    public class ApplicationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        public ApplicationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        /// <param name="errorDetails">list of error details.</param>
        public ApplicationException(List<ErrorDetail> errorDetails)
            : this()
            => this.Errors = this.Errors.Union(errorDetails).ToList();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        /// <param name="errorCode">status code.</param>
        /// <param name="message">message.</param>
        public ApplicationException(string errorCode, string message)
            : base(message)
        {
            this.StatusCode = errorCode;
            this.Errors.Add(new ErrorDetail(errorCode, message));
        }

        /// <summary>
        /// Gets application error id.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets error status code.
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// Gets error detail.
        /// </summary>       
        public List<ErrorDetail> Errors { get; } = new List<ErrorDetail>();

        /// <summary>
        /// Add exception detail to the list of exception details.
        /// </summary>
        /// <param name="errorDetail">Error Detal.</param>
        /// <returns>ApplicationException object.</returns>
        public ApplicationException AddExceptionDetail(ErrorDetail errorDetail)
        {
            if (!this.Errors.Contains(errorDetail))
            {
                this.Errors.Add(errorDetail);
            }

            return this;
        }
    }
}