//-----------------------------------------------------------------------
// <copyright file="BackchannelUserModel.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.IdentityServer.Backchannel.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Contains an enumerated list of inspire users types
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserType
    {
        /// <summary>
        /// Contains a basic application user type.
        /// </summary>
        Standard = 1,

        /// <summary>
        /// User is a review only seat license for Inspire.
        /// </summary>
        ReviewOnly = 2,

        /// <summary>
        /// User is a support seat license for Inspire.
        /// </summary>
        Support = 3
    }

    /// <summary>
    /// This model represents user information that is sent via the application backchannel API endpoints.
    /// </summary>
    public class BackchannelUserModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackchannelUserModel"/> class.
        /// </summary>
        public BackchannelUserModel()
        {
            this.Organizations = new List<BackchannelOrganizationModel>();
        }

        /// <summary>
        /// Gets or sets the unique identity of the application subscription user association.
        /// </summary>
        public int ApplicationSubscriptionUserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identity of the associated application subscription.
        /// </summary>
        public int ApplicationSubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets a list of organizations the user is associated with.
        /// </summary>
        public List<BackchannelOrganizationModel> Organizations { get; set; }

        /// <summary>
        /// Gets or sets the unique identity of the associated user.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user subscription association is locked.
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Gets or sets the inspire subscription user type.
        /// </summary>
        public UserType UserType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user uses a named seat license.
        /// </summary>
        public bool NamedSeat { get; set; }

        /// <summary>
        /// Gets or sets a date when the user lock expires.
        /// </summary>
        public DateTime? LockExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the phone of the user.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the mobile phone of the user.
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the preferred user name of the user.
        /// </summary>
        public string PreferredUserName { get; set; }

        /// <summary>
        /// Gets or sets the website of the user.
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets a profile image URL for the user.
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        /// Gets or sets a date time value when the entity was created. 
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets a date time value when the user last logged-in.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Gets or sets the time zone name.
        /// </summary>
        public string TimezoneName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is manager.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is manager; otherwise, <c>false</c>.
        /// </value>
        public bool IsManager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is admin.
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}