//-----------------------------------------------------------------------
// <copyright file="IdentityClient.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.IdentityServer.Backchannel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Newtonsoft.Json;
    using Vasont.IdentityServer.Backchannel.Models;

    /// <summary>
    /// This class is used to connect to the identity API for application specific interactions
    /// </summary>
    public class IdentityClient
    {
        #region Private Constants

        /// <summary>
        /// Contains the Inspire back-end client identity.
        /// </summary>
        private const string DefaultIdentityClientId = "backchannel";

        #endregion Private Constants

        #region Private Fields

        /// <summary>
        /// Contains a value indicating whether the discovery endpoint is used for token endpoint information.
        /// </summary>
        private readonly bool useDiscoveryForEndpoint;

        /// <summary>
        /// Contains the identity API secret
        /// </summary>
        private readonly string identityClientSecret;

        /// <summary>
        /// Contains the identity of the client
        /// </summary>
        private readonly string clientId;

        /// <summary>
        /// Contains the client authentication token response.
        /// </summary>
        private TokenResponse tokenResponse;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityClient"/> class.
        /// </summary>
        /// <param name="authorityUri">Contains the URI to the Identity Authority endpoint.</param>
        /// <param name="resourceUri">Contains the URI to the Identity API endpoint.</param>
        /// <param name="identityClientSecret">Contains the API resource client secret.</param>
        /// <param name="clientId">Contains the identity of the client.</param>
        /// <param name="useDiscoveryForEndpoint">Contains a value indicating whether the discovery endpoint is used for token endpoint information.</param>
        public IdentityClient(Uri authorityUri, Uri resourceUri, string identityClientSecret, string clientId, bool useDiscoveryForEndpoint = true)
        {
            this.AuthorityUri = authorityUri;
            this.ResourceUri = resourceUri;
            this.identityClientSecret = identityClientSecret;
            this.clientId = String.IsNullOrWhiteSpace(clientId) ? DefaultIdentityClientId : clientId;
            this.ErrorResponse = new IdentityErrorResponseModel();
            this.useDiscoveryForEndpoint = useDiscoveryForEndpoint;
        }

        #endregion Public Constructors

        #region public Properties

        /// <summary>
        /// Gets the Identity Authority endpoint URI
        /// </summary>
        public Uri AuthorityUri { get; }

        /// <summary>
        /// Gets the Resource API endpoint URI
        /// </summary>
        public Uri ResourceUri { get; }

        /// <summary>
        /// Gets a value indicating whether the client has authenticated with the server at some point
        /// </summary>
        public bool HasAuthenticated => this.tokenResponse != null && !this.tokenResponse.IsError;

        /// <summary>
        /// Gets the last error response from a request.
        /// </summary>
        public IdentityErrorResponseModel ErrorResponse { get; private set; }

        /// <summary>
        /// Gets the last exception that occurred within the client.
        /// </summary>
        public Exception LastException { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the client has a recent error.
        /// </summary>
        public bool HasErrors => this.LastException != null || (this.ErrorResponse != null && this.ErrorResponse.Messages.Any(e => e.ErrorType == ErrorType.Fatal || e.ErrorType == ErrorType.Critical));

        #endregion public Properties

        #region Public Methods

        /// <summary>
        /// Discoveries this instance.
        /// </summary>
        /// <param name="cancellationToken">Contains an cancellation token.</param>
        /// <returns>Returns a new DiscoveryResponse object.</returns>
        public async Task<DiscoveryDocumentResponse> RetrieveDiscovery(CancellationToken cancellationToken)
        {
            DiscoveryDocumentResponse discoResponse;

            using (HttpClient httpClient = new HttpClient())
            {
                discoResponse = await httpClient.GetDiscoveryDocumentAsync(this.AuthorityUri.ToString(), cancellationToken).ConfigureAwait(false);

                if (discoResponse.IsError)
                {
                    this.ErrorResponse = new IdentityErrorResponseModel { HasUnhandledException = true };
                    this.ErrorResponse.Messages.Add(new IdentityErrorModel
                    {
                        ErrorCategory = ErrorCategory.General,
                        ErrorType = ErrorType.Critical,
                        Message = discoResponse.Error,
                        StackTrace = discoResponse.Exception?.StackTrace
                    });
                }
            }

            return discoResponse;
        }

        /// <summary>
        /// Requests the client credentials.
        /// </summary>
        /// <param name="tokenEndpointUrl">The token endpoint URL.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="scopes">The scopes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="TokenResponse"/> object.</returns>
        public async Task<TokenResponse> RequestClientCredentials(string tokenEndpointUrl, string clientId, string scopes, CancellationToken cancellationToken)
        {
            TokenResponse response;

            using (HttpClient client = new HttpClient())
            using (var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = tokenEndpointUrl,
                ClientId = clientId,
                Scope = scopes,
                ClientSecret = this.identityClientSecret
            })
            {
                response = await client.RequestClientCredentialsTokenAsync(
                        tokenRequest,
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            return response;
        }

        /// <summary>
        /// This method is used to retrieve and authorize the client for back-channel communication to the Identity API.
        /// </summary>
        /// <param name="scopes">Contains the scopes that are requested for the client credentials authentication.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns a value indicating whether the authentication was successful.</returns>
        public async Task<bool> AuthenticateAsync(string scopes = "vidsapi", CancellationToken cancellationToken = default)
        {
            string tokenEndpointUrl = string.Empty;

            if (this.useDiscoveryForEndpoint)
            {
                // get the discovery information for the specified Identity Server
                var disco = await this.RetrieveDiscovery(cancellationToken).ConfigureAwait(false);

                if (!disco.IsError)
                {
                    tokenEndpointUrl = disco.TokenEndpoint;
                }
                else
                {
                    this.ErrorResponse.Messages.Add(new IdentityErrorModel(disco.Error, ErrorCategory.Application, ErrorType.Warning));
                }
            }

            // if no token endpoint found or specified...
            if (string.IsNullOrEmpty(tokenEndpointUrl))
            {
                // create one via assumed connect token endpoint.
                tokenEndpointUrl = new Uri(this.AuthorityUri, "/connect/token").ToString();
            }

            this.tokenResponse = await this.RequestClientCredentials(tokenEndpointUrl, this.clientId, scopes, cancellationToken).ConfigureAwait(false);

            // an error occurred...
            if (this.tokenResponse.IsError)
            {
                // bubble up an error response.
                this.ErrorResponse.Messages.Add(new IdentityErrorModel(this.tokenResponse.Error, ErrorCategory.Security, ErrorType.Critical));
            }

            return !this.tokenResponse.IsError;
        }

        /// <summary>
        /// This method is used to retrieve identity user information if available.
        /// </summary>
        /// <param name="userId">Contains the unique subject user identity value to lookup.</param>
        /// <returns>Returns a <see cref="BackchannelUserModel"/> if the user is found.</returns>
        public BackchannelUserModel RetrieveUser(string userId)
        {
            var request = this.CreateRequest($"Backchannel/Users/{userId}", HttpMethod.Get);
            return this.RequestContent<BackchannelUserModel>(request);
        }

        /// <summary>
        /// This method is used to retrieve identity user profiles subscription information if available based on search filter criteria.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="email">The email.</param>
        /// <param name="phone">The phone.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        /// Returns a List of <see cref="BackchannelUserModel" /> if users is found.
        /// </returns>
        public List<BackchannelUserModel> RetrieveUsers(string userName = "", string email = "", string phone = "", string orderBy = "UserName", string direction = "ascending")
        {
            var request = this.CreateRequest($"Backchannel/Users?userName={userName}&email={email}&phone={phone}&orderBy={orderBy}&direction={direction}", HttpMethod.Get);
            return this.RequestContent<List<BackchannelUserModel>>(request);
        }

        /// <summary>
        /// This method is used to retrieve identity subscription information if available for a tenant instance of the application.
        /// </summary>
        /// <param name="domainKey">Contains the application domain key to find.</param>
        /// <returns>Returns a <see cref="BackchannelSubscriptionModel"/> object if the subscription is found.</returns>
        public BackchannelSubscriptionModel RetrieveSubscription(string domainKey)
        {
            var request = this.CreateRequest($"Backchannel/{domainKey}", HttpMethod.Get);
            return this.RequestContent<BackchannelSubscriptionModel>(request);
        }

        /// <summary>
        /// This method is used to retrieve identity subscription information if available for a tenant instance of the application.
        /// </summary>
        /// <param name="domainKey">Contains the application domain key to find.</param>
        /// <returns>Returns a <see cref="BackchannelSubscriptionModel"/> object if the subscription is found.</returns>
        public BackchannelSubscriptionSeatModel RetrieveSubscriptionSeats(string domainKey)
        {
            var request = this.CreateRequest($"Backchannel/{domainKey}?seats=true", HttpMethod.Get);
            return this.RequestContent<BackchannelSubscriptionSeatModel>(request);
        }

        /// <summary>
        /// This method is used to retrieve all active subscriptions on the identity server.
        /// </summary>
        /// <returns>Returns a list of <see cref="BackchannelSubscriptionSeatModel"/> models.</returns>
        public List<BackchannelSubscriptionSeatModel> RetrieveSubscriptions()
        {
            var request = this.CreateRequest("Backchannel/Subscriptions", HttpMethod.Get);
            return this.RequestContent<List<BackchannelSubscriptionSeatModel>>(request);
        }

        /// <summary>
        /// This method is used to notify the identity server that a user has been removed from an inspire subscription
        /// through the Inspire interface and should remove the user from the
        /// </summary>
        /// <param name="domainKey">Contains the domain key where the request is being sent from.</param>
        /// <param name="userId">Contains the user identity of the user to remove from a subscription.</param>
        public void UserRemovalNotification(string domainKey, string userId)
        {
            var request = this.CreateRequest($"Backchannel/{domainKey}/UserRemovalNotification/{userId}", HttpMethod.Get);
            this.RequestContent(request);
        }

        /// <summary>
        /// Retrieves all available active organizations.
        /// </summary>
        /// <returns>Returns a List of <see cref="BackchannelOrganizationModel"/> objects.</returns>
        public List<BackchannelOrganizationModel> RetrieveOrganizations()
        {
            var request = this.CreateRequest("Backchannel/Organizations", HttpMethod.Get);
            return this.RequestContent<List<BackchannelOrganizationModel>>(request);
        }

        /// <summary>
        /// Retrieves the organization.
        /// </summary>
        /// <param name="organizationId">The organization identifier.</param>
        /// <returns>Returns the <see cref="BackchannelOrganizationModel"/> if found.</returns>
        public BackchannelOrganizationModel RetrieveOrganization(int organizationId)
        {
            var request = this.CreateRequest($"Backchannel/Organizations/{organizationId}", HttpMethod.Get);
            return this.RequestContent<BackchannelOrganizationModel>(request);
        }

        /// <summary>
        /// Retrieves all available active organization users.
        /// </summary>
        /// <returns>Returns a List of <see cref="BackchannelOrganizationModel"/> objects.</returns>
        public List<BackchannelOrganizationUserModel> RetrieveAllOrganizationUsers()
        {
            var request = this.CreateRequest("Backchannel/OrganizationUsers", HttpMethod.Get);
            return this.RequestContent<List<BackchannelOrganizationUserModel>>(request);
        }

        /// <summary>
        /// This method is used to retrieves available active organization users for the requested Organization.
        /// </summary>
        /// <param name="organizationId">The organization identifier.</param>
        /// <returns>
        /// Returns a List of <see cref="BackchannelOrganizationModel" /> objects.
        /// </returns>
        public List<BackchannelOrganizationUserModel> RetrieveOrganizationUsers(string organizationId)
        {
            var request = this.CreateRequest($"Backchannel/OrganizationUsers/{organizationId}", HttpMethod.Get);
            return this.RequestContent<List<BackchannelOrganizationUserModel>>(request);
        }

        /// <summary>
        /// This method is used to retrieves the available organization users.
        /// </summary>
        /// <param name="organizationId">The organization identifier.</param>
        /// <returns>Returns a list of <see cref="BackchannelOrganizationUserModel"/> objects. </returns>
        public List<BackchannelOrganizationUserModel> RetrieveAvailableOrganizationUsers(string organizationId)
        {
            var request = this.CreateRequest($"Backchannel/OrganizationUsers/{organizationId}/AvailableUsers", HttpMethod.Get);
            return this.RequestContent<List<BackchannelOrganizationUserModel>>(request);
        }

        /// <summary>
        /// This method is used to get all managers for the specified organization.
        /// </summary>
        /// <param name="organizationId">The organization identifier.</param>
        /// <returns>Returns a list of <see cref="BackchannelOrganizationUserModel"/> objects. </returns>
        public List<BackchannelUserModel> RetrieveOrganizationManagers(string organizationId)
        {
            var request = this.CreateRequest($"Backchannel/OrganizationUsers/{organizationId}/Managers", HttpMethod.Get);
            return this.RequestContent<List<BackchannelUserModel>>(request);
        }

        /// <summary>
        /// This method is used to determine if a given user is a manager of any organization they are a member of.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Returns a boolean.</returns>
        public bool RetrieveIsManagerForOrganizations(string userId)
        {
            var request = this.CreateRequest($"Backchannel/OrganizationUsers/{userId}/IsManager", HttpMethod.Get);
            return this.RequestContent<bool>(request);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// This method is used to easily create a new WebRequest object for the Web API.
        /// </summary>
        /// <param name="relativePath">Contains the relative Uri path of the web request to make against the Web API.</param>
        /// <param name="method">Contains the HttpMethod request method object.</param>
        /// <param name="noCache">Contains a value indicating whether the URL shall contain a parameter preventing the server from returning cached content.</param>
        /// <param name="credentials">Contains optional credentials </param>
        /// <param name="contentType">Contains optional content type.</param>
        /// <returns>Returns a new WebRequest object to execute.</returns>
        protected HttpWebRequest CreateRequest(string relativePath, HttpMethod method = null, bool noCache = true, ICredentials credentials = null, string contentType = "application/json")
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            if (method == null)
            {
                method = HttpMethod.Get;
            }

            return this.CreateRequest(relativePath, method.Method, noCache, credentials, contentType);
        }

        /// <summary>
        /// This method is used to easily create a new WebRequest object for the Web API.
        /// </summary>
        /// <param name="relativePath">Contains the relative Uri path of the web request to make against the Web API.</param>
        /// <param name="method">Contains the request method as a string value.</param>
        /// <param name="noCache">Contains a value indicating whether the URL shall contain a parameter preventing the server from returning cached content.</param>
        /// <param name="credentials">Contains optional credentials </param>
        /// <param name="contentType">Contains optional content type.</param>
        /// <returns>Returns a new HttpWebRequest object to execute.</returns>
        protected HttpWebRequest CreateRequest(string relativePath, string method, bool noCache = true, ICredentials credentials = null, string contentType = "application/json")
        {
            // request /Token, on success, return and store token.
            var request = WebRequest.CreateHttp(new Uri(this.ResourceUri, relativePath));
            request.Method = method;

            if (credentials == null)
            {
                credentials = CredentialCache.DefaultCredentials;
                request.UseDefaultCredentials = true;
            }

            request.Credentials = credentials;
            request.UserAgent = this.clientId;
            request.Accept = "application/json";
            request.ContentType = contentType;

            // Set a cache policy level for the "http:" and "https" schemes.
            if (noCache)
            {
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            }

            if (this.HasAuthenticated)
            {
                request.Headers.Add("Authorization", "Bearer " + this.tokenResponse.AccessToken);
            }

            return request;
        }

        /// <summary>
        /// This method is used to execute a web request and return the results of the request as a defined object type.
        /// </summary>
        /// <typeparam name="T">Contains the type of the object that is to be sent with the request.</typeparam>
        /// <typeparam name="TOut">Contains the type of object that is returned from the request.</typeparam>
        /// <param name="request">Contains the HttpWebRequest to execute.</param>
        /// <param name="requestBodyModel">Contains the object to serialize and submit with the request.</param>
        /// <returns>Returns the content of the request response as the specified object.</returns>
        protected TOut RequestContent<T, TOut>(HttpWebRequest request, T requestBodyModel)
        {
            string content = this.RequestContent(request, requestBodyModel);
            return !string.IsNullOrWhiteSpace(content) && !this.HasErrors ? JsonConvert.DeserializeObject<TOut>(content) : default;
        }

        /// <summary>
        /// This method is used to execute a web request and return the results of the request as a defined object type.
        /// </summary>
        /// <typeparam name="TOut">Contains the type of object that is returned from the request.</typeparam>
        /// <param name="request">Contains the HttpWebRequest to execute.</param>
        /// <returns>Returns the content of the request response as the specified object.</returns>
        protected TOut RequestContent<TOut>(HttpWebRequest request)
        {
            string content = this.RequestContent(request);
            return !string.IsNullOrWhiteSpace(content) && !this.HasErrors ? JsonConvert.DeserializeObject<TOut>(content) : default;
        }

        /// <summary>
        /// This method is used to execute a web request and return the results of the request as a string.
        /// </summary>
        /// <typeparam name="T">Contains the type of the object that is to be sent with the request.</typeparam>
        /// <param name="request">Contains the HttpWebRequest to execute.</param>
        /// <param name="requestBodyModel">Contains the object to serialize and submit with the request.</param>
        /// <returns>Returns the content of the request response.</returns>
        protected string RequestContent<T>(HttpWebRequest request, T requestBodyModel)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (requestBodyModel == null)
            {
                throw new ArgumentNullException(nameof(requestBodyModel));
            }

            // check to ensure we're not trying to post data on a GET or other non-body request.
            if (request.Method != HttpMethod.Post.Method && request.Method != HttpMethod.Put.Method && request.Method != HttpMethod.Delete.Method)
            {
                throw new HttpRequestException(Properties.Resources.HttpRequestTypeGetInvalidErrorText);
            }

            byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestBodyModel));

            // write data out to the request stream
            using (var postStream = request.GetRequestStream())
            {
                postStream.Write(requestData, 0, requestData.Length);
            }

            return this.RequestContent(request);
        }

        /// <summary>
        /// This method is used to execute a web request and return the results of the request as a string.
        /// </summary>
        /// <param name="request">Contains the HttpWebRequest to execute.</param>
        /// <returns>Returns the content of the request response.</returns>
        protected string RequestContent(HttpWebRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string resultContent;
            this.ResetErrors();

            try
            {
                // execute the request
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream ?? throw new InvalidOperationException()))
                {
                    resultContent = reader.ReadToEnd();

                    // if the status code was an error and there's content...
                    if ((int)response.StatusCode >= 400 && !string.IsNullOrWhiteSpace(resultContent))
                    {
                        // set the error model
                        this.ErrorResponse = JsonConvert.DeserializeObject<IdentityErrorResponseModel>(resultContent);
                    }
                }
            }
            catch (WebException webEx)
            {
                this.LastException = webEx;

                using (var exceptionResponse = (HttpWebResponse)webEx.Response)
                using (var responseStream = exceptionResponse.GetResponseStream())
                using (var reader = new StreamReader(responseStream ?? throw new InvalidOperationException()))
                {
                    resultContent = reader.ReadToEnd();

                    // if the status code was an error and there's content...
                    if ((int)exceptionResponse.StatusCode >= 400 && !string.IsNullOrWhiteSpace(resultContent))
                    {
                        // set the error model
                        this.ErrorResponse = JsonConvert.DeserializeObject<IdentityErrorResponseModel>(resultContent);
                    }
                }
            }

            return resultContent;
        }

        /// <summary>
        /// This method is used to clear any previous error objects.
        /// </summary>
        protected void ResetErrors()
        {
            this.ErrorResponse.HasUnhandledException = false;
            this.ErrorResponse.Messages.Clear();
        }

        #endregion Protected Methods
    }
}