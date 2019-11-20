//-----------------------------------------------------------------------
// <copyright file="IdentityErrorModel.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Vasont.IdentityServer.Backchannel.Models
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Contains an enumerated list of error message types.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorType
    {
        /// <summary>
        /// Errors that have unexpectedly ended the intended process.
        /// </summary>
        Fatal,

        /// <summary>
        /// Errors that were expected and ended the intended process.
        /// </summary>
        Critical,

        /// <summary>
        /// Errors that can be handled and allow a process to continue.
        /// </summary>
        Warning,

        /// <summary>
        /// Errors that occur before a process starts.
        /// </summary>
        Validation
    }

    /// <summary>
    /// Contains an enumerated list of error message types.
    /// </summary>
    public enum ErrorCategory
    {
        /// <summary>
        /// General errors not categorized.
        /// </summary>
        /// <remarks>
        /// All other errors that do not fall within the other categories.
        /// </remarks>
        General,

        /// <summary>
        /// Security related errors.
        /// </summary>
        /// <remarks>
        /// Errors related to permissions would be good candidates for Application errors.
        /// </remarks>
        Security,

        /// <summary>
        /// Application related errors not security related.
        /// </summary>
        /// <remarks>
        /// Errors related to a process failing would be good candidates for Application errors.
        /// </remarks>
        Application,

        /// <summary>
        /// System related errors that are not security related.
        /// </summary>
        /// <remarks>
        /// IO or database related issues would be good candidates for System errors.
        /// </remarks>
        System
    }

    /// <summary>
    /// This class represents an error message within the <see cref="IdentityErrorResponseModel"/> class.
    /// </summary>
    public class IdentityErrorModel
    {
        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityErrorModel"/> class.
        /// </summary>
        public IdentityErrorModel()
            : this(string.Empty, ErrorCategory.General, ErrorType.Warning, DateTime.UtcNow, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityErrorModel"/> class.
        /// </summary>
        /// <param name="message">Contains the error message text.</param>
        /// <param name="errorCategory">Contains the error category.</param>
        /// <param name="type">Contains the error message type.</param>
        /// <param name="eventDate">Contains the date time when the error was created.</param>
        /// <param name="stackTrace">Contains an error stack trace message.</param>
        /// <param name="propertyName">Contains the property name for validation errors.</param>
        public IdentityErrorModel(string message, ErrorCategory errorCategory, ErrorType type, DateTime eventDate = default, string stackTrace = "", string propertyName = "")
        {
            this.Message = message;
            this.ErrorCategory = errorCategory;
            this.ErrorType = type;
            this.PropertyName = propertyName;
            this.StackTrace = stackTrace;
            this.EventDate = eventDate == DateTime.MinValue ? DateTime.UtcNow : eventDate;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the error category.
        /// </summary>
        public ErrorCategory ErrorCategory { get; set; }

        /// <summary>
        /// Gets or sets the error message type.
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the error message text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a related property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the date time when the error was generated.
        /// </summary>
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Gets or sets the error stack trace generated if any.
        /// </summary>
        public string StackTrace { get; set; }
        #endregion
    }
}