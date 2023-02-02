using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Telia.GraphQL.Client;

namespace Telia.GraphQL
{
    public class DefaultNetworkClient
    {
        string endpoint;

        public DefaultNetworkClient(string endpoint)
        {
            this.endpoint = endpoint;
        }

        public string Send(GraphQLQueryInfo query)
        {
            var request = (HttpWebRequest)WebRequest.Create(this.endpoint);

            var json = JsonConvert.SerializeObject(query, new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new GraphQLObjectConverter(),
                    new StringEnumConverter()
                }
            });
            var data = Encoding.ASCII.GetBytes(json);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            using (var responseStream = response.GetResponseStream())
            {
                using (var streamReader = new StreamReader(responseStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
