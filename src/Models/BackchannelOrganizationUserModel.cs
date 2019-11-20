//-----------------------------------------------------------------------
// <copyright file="BackchannelOrganizationUserModel.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.IdentityServer.Backchannel.Models
{
    /// <summary>
    /// This class contains model information for updating an organization user.
    /// </summary>
    public class BackchannelOrganizationUserModel
    {
        /// <summary>
        /// Gets or sets the user identity value.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the organization identity value of the user.
        /// </summary>
        public int OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the organization name.
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the parent organization identity.
        /// </summary>
        public int? ParentOrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the parent organization name.
        /// </summary>
        public string ParentOrganizationName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is an organization manager.
        /// </summary>
        public bool OrganizationManager { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the URL to the user profile image.
        /// </summary>
        public string ProfileImage { get; set; }

        /// <summary>
        /// Gets or sets the user first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user last name.
        /// </summary>
        public string LastName { get; set; }
    }
}
