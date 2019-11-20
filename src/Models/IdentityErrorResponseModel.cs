//-----------------------------------------------------------------------
// <copyright file="IdentityErrorResponseModel.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.IdentityServer.Backchannel.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// This class is used as the Error Response Model 
    /// </summary>
    public class IdentityErrorResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityErrorResponseModel"/> class.
        /// </summary>
        public IdentityErrorResponseModel()
        {
            this.Messages = new List<IdentityErrorModel>();
            this.HasUnhandledException = false;
        }

        /// <summary>
        /// Gets the error messages.
        /// </summary>
        public List<IdentityErrorModel> Messages { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has unhandled exception.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has unhandled exception; otherwise, <c>false</c>.
        /// </value>
        public bool HasUnhandledException { get; set; }
    }        
}