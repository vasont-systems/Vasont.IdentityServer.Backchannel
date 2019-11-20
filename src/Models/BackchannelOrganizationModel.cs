//-----------------------------------------------------------------------
// <copyright file="BackchannelOrganizationModel.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.IdentityServer.Backchannel.Models
{
    /// <summary>
    /// This class represents an organization information that is sent via the application backchannel API endpoints.
    /// </summary>
    public class BackchannelOrganizationModel
    {
        /// <summary>
        /// Gets or sets the organization identity.
        /// </summary>
        public int OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the organization name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent organization identity.
        /// </summary>
        public int? ParentOrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the parent organization name.
        /// </summary>
        public string ParentName { get; set; }
    }
}