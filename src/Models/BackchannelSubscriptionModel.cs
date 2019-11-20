//-----------------------------------------------------------------------
// <copyright file="BackchannelSubscriptionModel.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.IdentityServer.Backchannel.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains a model used for sending backchannel information about a tenant subscription to a calling application.
    /// </summary>
    public class BackchannelSubscriptionModel : BackchannelSubscriptionSeatModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackchannelSubscriptionModel"/> class.
        /// </summary>
        public BackchannelSubscriptionModel()
        {
            this.Settings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the associated application data source.
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Gets or sets the application database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the application database user id.
        /// </summary>
        public string DatabaseUserId { get; set; }

        /// <summary>
        /// Gets or sets the application database password.
        /// </summary>
        public string DatabasePassword { get; set; }

        /// <summary>
        /// Gets or sets the extended connection string settings.
        /// </summary>
        public string ExtendedConnectionString { get; set; }
        
        /// <summary>
        /// Gets or sets a list of application subscription settings.
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }
    }
}