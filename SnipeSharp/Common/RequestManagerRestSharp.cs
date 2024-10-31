using SnipeSharp.Endpoints.Models;
using SnipeSharp.Endpoints.SearchFilters;
using RestSharp;
using SnipeSharp.Endpoints;
using SnipeSharp.Exceptions;
using RestSharp.Authenticators.OAuth2;
using System;

namespace SnipeSharp.Common
{
    public class RequestManagerRestSharp : IRequestManager
    {
        public ApiSettings ApiSettings { get; }
        
        public IQueryParameterBuilder QueryParameterBuilder { get; set; } = new QueryParameterBuilder();
        
        private readonly RestClientOptions _restClientOptions;
        private readonly RestClient _client;

        public RequestManagerRestSharp(ApiSettings apiSettings)
        {
            this.ApiSettings = apiSettings;
            this._restClientOptions = new RestClientOptions();
            CheckApiTokenAndUrl();

            this._client = new RestClient(this._restClientOptions);
            this._client.AddDefaultHeader("Accept", "application/json");
        }

        public string Delete(string path)
        {
            CheckApiTokenAndUrl();
            var req = new RestRequest(path, Method.Delete);
            var res = _client.Execute(req);

            return res.Content;
        }

        public string Get(string path)
        {
            CheckApiTokenAndUrl();
            var req = new RestRequest
            {
                Resource = path,
                Timeout = TimeSpan.FromMilliseconds(200000)
            };
            // Test
            var res = _client.Execute(req);

            return res.Content;
        }

        public string Get(string path, ISearchFilter filter)
        {
            CheckApiTokenAndUrl();
            var req = new RestRequest
            {
                Resource = path,
                Timeout = TimeSpan.FromMilliseconds(200000)
            };

            var filters = this.QueryParameterBuilder.GetParameters(filter);
            // TODO: We should probably breakup large requests
            foreach (var kvp in filters)
            {
                req.AddParameter(kvp.Key, kvp.Value);
            }

            var res = _client.Execute(req);

            return res.Content;
        }

        public string Post(string path, ICommonEndpointModel item)
        {
            CheckApiTokenAndUrl();
            var req = new RestRequest(path, Method.Post);

            var parameters = this.QueryParameterBuilder.GetParameters(item);

            foreach (var kvp in parameters)
            {
                req.AddParameter(kvp.Key, kvp.Value);
            }
            
            // TODO: Add error checking
            var res = _client.Execute(req);

            return res.Content;
        }

        public string Put(string path, ICommonEndpointModel item)
        {
            // TODO: Make one method for post and put.
            CheckApiTokenAndUrl();
            var req = new RestRequest(path, Method.Put);

            var parameters = this.QueryParameterBuilder.GetParameters(item);

            foreach (var kvp in parameters)
            {
                req.AddParameter(kvp.Key, kvp.Value);
            }
            
            // TODO: Add  error checking
            var res = _client.Execute(req);

            return res.Content;
        }

        // Since the Token and URL can be set anytime after the SnipApi object is created we need to check for these before sending a request
        private void CheckApiTokenAndUrl()
        {
            if (ApiSettings.BaseUrl == null)
            {
                throw new NullApiBaseUrlException("No API Base Url Set.");
            }

            if (ApiSettings.ApiToken == null)
            {
                throw new NullApiTokenException("No API Token Set");
            }

            if (_restClientOptions.BaseUrl == null)
            {
                _restClientOptions.BaseUrl = ApiSettings.BaseUrl;
            }

            if (_restClientOptions.Authenticator == null)
            {
                _restClientOptions.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(ApiSettings.ApiToken, "Bearer");
            }
        }
    }
}
