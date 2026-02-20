using Newtonsoft.Json;
using Osrm.Client.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace Osrm.Client.v5
{
    public class Osrm5x
    {
        public string Url { get; set; }

        private string _fullUrl = string.Empty;
        public string FullUrl { get { return _fullUrl; } }

        /// <summary>
        /// Version of the protocol implemented by the service.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Mode of transportation, is determined by the profile that is used to prepare the data
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// Timeout for web request. If not specified default value will be used.
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// Serviço para localizar a rota mais rápida entre os pontos informados.
        /// </summary>
        protected readonly string RouteServiceName = "route";
        /// <summary>
        /// Serviço para localizar locais mais próximos de determinado ponto.
        /// </summary>
        protected readonly string NearestServiceName = "nearest";
        /// <summary>
        /// Equivalente ao distance Matrix do google maps onde passando várias coordenadas retorna a matriz de distãncia e tempos entre todos os pontos informados.
        /// </summary>
        protected readonly string TableServiceName = "table";
        protected readonly string MatchServiceName = "match";
        /// <summary>
        /// Serviço que resolve o caixeiro viajante nem sempre retornando a rota mais rápida e sim a mais curta.
        /// </summary>
        protected readonly string TripServiceName = "trip";
        protected readonly string TileServiceName = "tile";

        public Osrm5x(string url, string version = "v1", string profile = "driving")
        {
            Url = url;
            Version = version;
            Profile = profile;
        }

        /// <summary>
        /// This service provides shortest path queries with multiple via locations.
        /// It supports the computation of alternative paths as well as giving turn instructions.
        /// </summary>
        /// <param name="locs"></param>
        /// <returns></returns>
        public RouteResponse Route(Location[] locs)
        {
            return Route(new RouteRequest()
            {
                Coordinates = locs
            });
        }

        /// <summary>
        /// This service provides shortest path queries with multiple via locations.
        /// It supports the computation of alternative paths as well as giving turn instructions.
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        public RouteResponse Route(RouteRequest requestParams)
        {
            return Send<RouteResponse>(RouteServiceName, requestParams);
        }

        public Models.Responses.NearestResponse Nearest(params Location[] locs)
        {
            return Nearest(new NearestRequest()
            {
                Coordinates = locs
            });
        }

        public Models.Responses.NearestResponse Nearest(NearestRequest requestParams)
        {
            return Send<Models.Responses.NearestResponse>(NearestServiceName, requestParams);
        }

        public Models.Responses.TableResponse Table(params Location[] locs)
        {
            return Table(new TableRequest()
            {
                Coordinates = locs
            });
        }

        public Models.Responses.TableResponse Table(TableRequest requestParams)
        {
            return Send<Models.Responses.TableResponse>(TableServiceName, requestParams);
        }

        public Models.Responses.MatchResponse Match(params Location[] locs)
        {
            return Match(new MatchRequest()
            {
                Coordinates = locs
            });
        }

        public Models.Responses.MatchResponse Match(MatchRequest requestParams)
        {
            return Send<Models.Responses.MatchResponse>(MatchServiceName, requestParams);
        }

        public Models.Responses.TripResponse Trip(params Location[] locs)
        {
            return Trip(new TripRequest()
            {
                Coordinates = locs
            });
        }

        public Models.Responses.TripResponse Trip(TripRequest requestParams)
        {
            return Send<Models.Responses.TripResponse>(TripServiceName, requestParams);
        }

        protected T Send<T>(string service, BaseRequest request)
        {
            var coordinatesStr = request.CoordinatesUrlPart;
            if (request.SendCoordinatesAsPolyline && (coordinatesStr.Contains("/") || coordinatesStr.Contains(@"\")))
            {
                request.SendCoordinatesAsPolyline = false;
                coordinatesStr = request.CoordinatesUrlPart;
            }
            List<Tuple<string, string>> urlParams = request.UrlParams;
            _fullUrl = OsrmRequestBuilder.GetUrl(Url, service, Version, Profile, coordinatesStr, urlParams);
            string json = null;
            using (var client = new OsrmWebClient(Timeout))
            {
                json = client.DownloadString(new Uri(_fullUrl));
                // Ajuste técnico pois o serviço table retorna algumas distâncias e tempos null
                json = json.Replace(",null,", ",0,");
                json = json.Replace(",null]", ",0]");
                json = json.Replace("[null,", "[0,");
            }

            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        private class OsrmWebClient : WebClient
        {
            private readonly int? _specificTimeout;

            public OsrmWebClient(int? timeout = null)
            {
                _specificTimeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);

                if (request != null && _specificTimeout.HasValue)
                    request.Timeout = _specificTimeout.Value;

                return request;
            }
        }

    }
}