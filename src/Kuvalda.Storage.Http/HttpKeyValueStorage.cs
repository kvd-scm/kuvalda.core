using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kuvalda.Core;
using Serilog;

namespace Kuvalda.Storage.Http
{
    public class HttpKeyValueStorage : IRemoteKeyValueStorage
    {
        private readonly HttpClient _client;
        private readonly EndpointOptions _options;
        private readonly ILogger _log;

        public HttpKeyValueStorage(HttpClient client, EndpointOptions options, ILogger log)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _log = log;
        }

        public async Task<string> Get(string key)
        {
            var uri = _options.GetTagUri(key);
            
            _log?.Debug("Request http GET method for uri: {uri}", uri);
            
            var response = await _client.GetAsync(uri);
            
            _log?.Debug("Take response GET method for uri: {uri} with status code {code}", uri, response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            
            return await response.Content.ReadAsStringAsync();
        }

        public async Task Set(string key, string value)
        {
            var uri = _options.GetPushTagUri(key);
            
            _log?.Debug("Request http POST method for uri: {uri}", uri);
            
            var response = await _client.PostAsync(uri, new StringContent(value));
            
            _log?.Debug("Take response POST method for uri: {uri} with status code {code}", uri, response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpSendErrorException();
            }
        }
    }
}