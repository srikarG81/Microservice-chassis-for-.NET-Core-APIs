// <copyright file="ErrorDetail.cs" company="NA">
//NA
// </copyright>

using System;
using System.Collections.Generic;

namespace Core.DTO
{
    /// <summary>
    /// Error Detail.
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDetail"/> class.
        /// </summary>
        /// <param name="code">error code</param>
        /// <param name="message">error message</param>
        public ErrorDetail(string code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        /// <summary>
        /// Gets error code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// overloading == operator.
        /// </summary>
        /// <param name="left">first ErrorDetail object.</param>
        /// <param name="right">second ErrorDetail object.</param>
        /// <returns>comparision result.</returns>
        public static bool operator ==(ErrorDetail left, ErrorDetail right)
            => EqualityComparer<ErrorDetail>.Default.Equals(left, right);

        /// <summary>
        /// overloading != operator.
        /// </summary>
        /// <param name="left">first ErrorDetail object.</param>
        /// <param name="right">second ErrorDetail object.</param>
        /// <returns>comparision result.</returns>
        public static bool operator !=(ErrorDetail left, ErrorDetail right) => !(left == right);

        /// <summary>
        /// overriding Equals method.
        /// </summary>
        /// <param name="obj">object to comapre.</param>
        /// <returns>comparision result.</returns>
        public override bool Equals(object obj)
            => obj is ErrorDetail detail &&
                   Code == detail.Code &&
                   Message == detail.Message;

        /// <summary>
        /// Generates hashcode.
        /// </summary>
        /// <returns>hash code.</returns>
        public override int GetHashCode() => HashCode.Combine(Code, Message);
    }
}
