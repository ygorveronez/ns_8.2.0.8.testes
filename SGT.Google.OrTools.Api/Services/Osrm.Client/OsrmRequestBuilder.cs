using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Osrm.Client
{
    internal static class OsrmRequestBuilder
    {
        public static string GetUrl(string baseUrl, string service, List<Tuple<string, string>> urlParams)
        {
            var uriBuilder = new UriBuilder(baseUrl);
            uriBuilder.Path += service;
            var url = uriBuilder.Uri.ToString();

            var encodedParams = urlParams
                .Select(x => string.Format("{0}={1}", HttpUtility.UrlEncode(x.Item1), HttpUtility.UrlEncode(x.Item2)))
                .ToList();

            var result = url + "?" + string.Join("&", encodedParams);

            return result;
        }

        public static string GetUrl(string server, string service, string version, string profile, string coordinatesString, List<Tuple<string, string>> urlParams)
        {
            var uriBuilder = new UriBuilder(server);
            uriBuilder.Path += service + "/" + version + "/" + profile + "/" + coordinatesString;
            var url = uriBuilder.Uri.ToString();
            string result = url;

            if (urlParams != null && urlParams.Count > 0)
            {
                var encodedParams = urlParams
                    .Select(x => string.Format("{0}={1}", HttpUtility.UrlEncode(x.Item1), HttpUtility.UrlEncode(x.Item2)))
                    .ToList();

                result += "?" + string.Join("&", encodedParams);
            }
            return result;
        }

        public static Tuple<string, string>[] CreateLocationParams(string key, Location[] locations, bool combineToOneAsPolyline = false)
        {
            if (locations == null)
            {
                return new Tuple<string, string>[0];
            }

            if (combineToOneAsPolyline)
            {
                var encodedLocs = OsrmPolylineConverter.Encode(locations);
                return new Tuple<string, string>[] { new Tuple<string, string>(key, encodedLocs) };
            }
            else
            {
                return locations.Select(x =>
                    new Tuple<string, string>(key, x.Latitude.ToString("", CultureInfo.InvariantCulture)
                        + "," + x.Longitude.ToString("", CultureInfo.InvariantCulture))).ToArray();
            }
        }

        public static string CreateCoordinatesUrl(Location[] locations, bool combineToOneAsPolyline = false)
        {
            if (locations == null)
            {
                return string.Empty;
            }
            if (combineToOneAsPolyline)
            {
                var encodedLocs = OsrmPolylineConverter.Encode(locations, 1E5);
                //encodedLocs = HttpUtility.UrlEncode(encodedLocs);
                //Problema de O servidor remoto retornou um erro: (400) Solicitação Incorreta ao conter \ ou / na polyline
                //encodedLocs = encodedLocs.Replace(@"\", "%5C");
                //encodedLocs = encodedLocs.Replace("/", "%2F");
                return "polyline(" + encodedLocs + ")";
            }
            else
            {
                return string.Join(";", locations.Select(x => x.Longitude.ToString("", CultureInfo.InvariantCulture)
                        + "," + x.Latitude.ToString("", CultureInfo.InvariantCulture)));
            }
        }

        public static Tuple<string, string>[] CreateLocationParams4x(string key, LocationWithTimestamp[] locations, bool combineToOneAsPolyline = false)
        {
            if (locations == null)
            {
                return new Tuple<string, string>[0];
            }

            if (combineToOneAsPolyline)
            {
                var encodedLocs = OsrmPolylineConverter.Encode(locations);
                return new Tuple<string, string>[] { new Tuple<string, string>(key, encodedLocs) };
            }
            else
            {
                var res = new List<Tuple<string, string>>();
                locations.ToList().ForEach(x =>
                {
                    res.Add(new Tuple<string, string>(key, x.Latitude.ToString("", CultureInfo.InvariantCulture)
                        + "," + x.Longitude.ToString("", CultureInfo.InvariantCulture)));
                    if (x.UnixTimeStamp.HasValue)
                    {
                        res.AddStringParameter("t", x.UnixTimeStamp.Value.ToString());
                    }
                });

                return res.ToArray();
            }
        }

        public static List<Tuple<string, string>> AddBoolParameter(this List<Tuple<string, string>> urlParams, string urlKey, bool param, bool defaultValue)
        {
            if (param != defaultValue)
            {
                urlParams.Add(new Tuple<string, string>(urlKey, param ? "true" : "false"));
            }

            return urlParams;
        }

        public static List<Tuple<string, string>> AddStringParameter(this List<Tuple<string, string>> urlParams, string urlKey, string value, Func<bool> condition = null)
        {
            if (!string.IsNullOrEmpty(value) && (condition == null || condition()))
            {
                urlParams.Add(new Tuple<string, string>(urlKey, value));
            }

            return urlParams;
        }

        public static List<Tuple<string, string>> AddParams(this List<Tuple<string, string>> urlParams, string urlKey, string[] values, string defaultIfEmpty = null)
        {
            if (values != null && values.Length > 0)
            {
                urlParams.Add(new Tuple<string, string>(urlKey, string.Join(";", values)));
            }
            else if (!string.IsNullOrEmpty(defaultIfEmpty))
            {
                urlParams.Add(new Tuple<string, string>(urlKey, defaultIfEmpty));
            }

            return urlParams;
        }
    }
}