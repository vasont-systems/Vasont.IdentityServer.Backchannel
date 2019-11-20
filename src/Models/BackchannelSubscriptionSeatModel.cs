//-----------------------------------------------------------------------
// <copyright file="BackchannelSubscriptionSeatModel.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.IdentityServer.Backchannel.Models
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Contains an enumerated list of tenant types.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApplicationSubscriptionLevel
    {
        /// <summary>
        /// Defines the tenant as a trial account which will expire.
        /// </summary>
        Trial = 0,

        /// <summary>
        /// Defines the tenant as a basic level account.
        /// </summary>
        Basic = 1,

        /// <summary>
        /// Defines the tenant as an advanced level account.
        /// </summary>
        Advanced = 2,

        /// <summary>
        /// Defines the tenant as an enterprise level account.
        /// </summary>
        Enterprise = 4,

        /// <summary>
        /// Defines the tenant as a self-hosted level account.
        /// </summary>
        SelfHosted = 512,
    }

    /// <summary>
    /// This class represents a base subscription seat model.
    /// </summary>
    public class BackchannelSubscriptionSeatModel
    {
        /// <summary>
        /// Gets or sets the unique domain key used for subscription lookup.
        /// </summary>
        public string DomainKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscription is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the associated application.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the associated subscription type.
        /// </summary>
        public string SubscriptionType { get; set; }

        /// <summary>
        /// Gets or sets the application subscription type.
        /// </summary>
        public ApplicationSubscriptionLevel SubscriptionLevel { get; set; }

        /// <summary>
        /// Gets or sets the associated organization.
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of a subscription.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the number of named seats for a standard user associated with a subscription.
        /// </summary>
        public int StandardUserNamedSeats { get; set; }

        /// <summary>
        /// Gets or sets the number of concurrent seats for a standard user associated with a subscription.
        /// </summary>
        public int StandardUserConcurrentSeats { get; set; }

        /// <summary>
        /// Gets or sets the number of named seats for a review-only user associated with a subscription.
        /// </summary>
        public int ReviewOnlyUserNamedSeats { get; set; }

        /// <summary>
        /// Gets or sets the number of concurrent seats for a review-only user associated with a subscription.
        /// </summary>
        public int ReviewOnlyUserConcurrentSeats { get; set; }
    }
}
