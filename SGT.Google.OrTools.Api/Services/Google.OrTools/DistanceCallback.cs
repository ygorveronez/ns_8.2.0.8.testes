//using Google.OrTools.ConstraintSolver;
//using Osrm.Client.v5;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Google.OrTools.Api.Services.GoogleOrTools
//{
//    public class DistanceCallback : NodeEvaluator2
//    {
//        public DistanceCallback(Position[] locations, Models.EnumCargaTpRota tp_rota, string url_server_osm, bool distancias_banco = true)
//        {
//            Osrm5x osrm = new Osrm5x(url_server_osm);
//            this.locations = locations;
//            Repository.DBConnection.ConexaoORACLE cnn = new Repository.DBConnection.ConexaoORACLE();

//            int i = 0;
//            int j = 0;
//            try
//            {
//                bool erro_osrm = true;
//                if (!distancias_banco)
//                {
//                    try
//                    {
//                        bool separarEmBlocos = true;

//                        if (separarEmBlocos)
//                        {
//                            //Inicializando a matriz de custos
//                            custos = new long[locations.Length][];
//                            for (i = 0; i < locations.Length; i++)
//                                custos[i] = new long[locations.Length];

//                            // vamos calcular uma matriz de 25x25
//                            int maxSize = 50;
//                            int qtde = locations.Length;

//                            List<DistanceMatrix.Client.Location> locsOrigem = new List<DistanceMatrix.Client.Location>();
//                            for (i = 0; i < locations.Length; i++)
//                            {
//                                //Adicionando as origens 
//                                locsOrigem.Add(new DistanceMatrix.Client.Location(locations[i].Latitude, locations[i].Longitude));
//                                //Se a quantidade for ao max de origens ou o último registro.
//                                if ((i + 1) % maxSize == 0 || (i + 1 == locations.Length))
//                                {
//                                    uint[] source = new uint[locsOrigem.Count];
//                                    for (int s = 0; s < locsOrigem.Count; s++)
//                                        source[s] = (uint)s;

//                                    List<DistanceMatrix.Client.Location> locsDestino = new List<DistanceMatrix.Client.Location>();
//                                    for (j = 0; j < locations.Length; j++)
//                                    {
//                                        locsDestino.Add(new DistanceMatrix.Client.Location(locations[j].Latitude, locations[j].Longitude));
//                                        if ((j + 1) % maxSize == 0 || (j + 1 == locations.Length))
//                                        {
//                                            uint[] destinations = new uint[locsDestino.Count];
//                                            for (int d = 0; d < locsDestino.Count; d++)
//                                                destinations[d] = (uint)(source.Length + d);

//                                            var locais = new List<DistanceMatrix.Client.Location>();
//                                            locais.AddRange(locsOrigem);
//                                            locais.AddRange(locsDestino);

//                                            //Returns a asymmetric 3x2 matrix with from the polyline encoded locations qikdcB}~dpXkkHz:
//                                            DistanceMatrix.Client.Models.Responses.TableResponse response = osrm.Table(new DistanceMatrix.Client.Models.TableRequest()
//                                            {
//                                                Coordinates = locais.ToArray(),
//                                                SendCoordinatesAsPolyline = true,
//                                                Sources = source,
//                                                Destinations = destinations
//                                            });

//                                            if (response.Code.ToUpper() == "OK")
//                                            {
//                                                for (int r = 0; r < response.Distances.Length; r++)
//                                                {
//                                                    for (int t = 0; t < response.Distances[r].Length; t++)
//                                                    {
//                                                        long metros = 0;
//                                                        metros = (long)response.Distances[r][t];
//                                                        if (metros == 0)
//                                                            metros = (long)Geo.DistanciaRaio(locais[r].Latitude, locais[r].Longitude, locais[t].Latitude, locais[t].Longitude);
//                                                        int x = i - (locsOrigem.Count - 1) + r;
//                                                        int y = j - (locsDestino.Count - 1) + t;
//                                                        custos[x][y] = metros;
//                                                    }
//                                                }
//                                                erro_osrm = false;
//                                            }
//                                            else
//                                            {
//                                                new Services.Errors().GravaLogEmTxt(AppDomain.CurrentDomain.BaseDirectory, new Exception("DistanceCallback - OSRM status <> OK -> " + response.Code.ToUpper() + " request " + osrm.FullUrl));
//                                                for (int r = 0; r < locais.Count; r++)
//                                                {
//                                                    for (int t = 0; t < locais.Count; t++)
//                                                    {
//                                                        long metros = (long)Geo.DistanciaRaio(locais[r].Latitude, locais[r].Longitude, locais[t].Latitude, locais[t].Longitude);
//                                                        int x = i - (locsOrigem.Count - 1) + r;
//                                                        int y = j - (locsDestino.Count - 1) + t;
//                                                        custos[x][y] = metros;
//                                                    }
//                                                }
//                                            }

//                                            locsDestino = new List<DistanceMatrix.Client.Location>();
//                                        }
//                                    }
//                                    locsOrigem = new List<DistanceMatrix.Client.Location>();
//                                }
//                            }
//                        }
//                        else
//                        {
//                            DistanceMatrix.Client.Location[] locs = new DistanceMatrix.Client.Location[locations.Length];
//                            for (i = 0; i < locations.Length; i++)
//                                locs[i] = new DistanceMatrix.Client.Location(locations[i].Latitude, locations[i].Longitude);

//                            //Executa o table request para pegar todas as distâncias entre os locais.
//                            DistanceMatrix.Client.Models.Responses.TableResponse response = osrm.Table(new DistanceMatrix.Client.Models.TableRequest()
//                            {
//                                Coordinates = locs,
//                                SendCoordinatesAsPolyline = true
//                            });
//                            if (response.Code.ToUpper() == "OK")
//                            {
//                                custos = new long[locations.Length][];
//                                for (i = 0; i < response.Distances.Length; i++)
//                                {
//                                    custos[i] = new long[locations.Length];
//                                    for (j = 0; j < response.Distances[i].Length; j++)
//                                    {
//                                        long metros = 0;
//                                        metros = (long)response.Distances[i][j];
//                                        if (metros == 0)
//                                            metros = (long)Geo.DistanciaRaio(locations[i].Latitude, locations[i].Longitude, locations[j].Latitude, locations[j].Longitude);
//                                        custos[i][j] = metros;
//                                    }
//                                }
//                                erro_osrm = false;
//                            }
//                            else
//                                new Services.Errors().GravaLogEmTxt(AppDomain.CurrentDomain.BaseDirectory, new Exception("DistanceCallback - OSRM status <> OK -> " + response.Code.ToUpper() + " request " + osrm.FullUrl));
//                        }
//                    }
//                    catch (Exception eosm)
//                    {
//                        new Services.Errors().GravaLogEmTxt(AppDomain.CurrentDomain.BaseDirectory, new Exception("DistanceCallback - OSRM Exception -> " + eosm.ToString() + Environment.NewLine + " request " + osrm.FullUrl));
//                    }
//                }
//                if (erro_osrm)
//                {
//                    cnn.Connect();
//                    custos = new long[locations.Length][];
//                    for (i = 0; i < locations.Length; i++)
//                    {
//                        custos[i] = new long[locations.Length];
//                        for (j = 0; j < locations.Length; j++)
//                        {
//                            if (i == 0 && tp_rota == Models.EnumCargaTpRota.SEM_RETORNO)
//                                custos[i][j] = 0;
//                            else if (i == j || locations[i].codigo == locations[j].codigo || (locations[i].Latitude == locations[j].Latitude && locations[i].Longitude == locations[j].Longitude))
//                                custos[i][j] = 0;
//                            else
//                            {
//                                var tmp = cnn.ExecuteScalar("select (d.km * 1000) metros from distancia_ender d where d.ender_id_orig = :orig and d.ender_id_dest = :dest", locations[i].codigo, locations[j].codigo);
//                                long metros = 0;
//                                if (tmp != null)
//                                    metros = long.Parse(tmp.ToString());
//                                //Não encontrou a distância no banco ou sua distância é "0";
//                                if (metros == 0)
//                                {
//                                    DistanceMatrix.Client.Location[] locs = new DistanceMatrix.Client.Location[2];
//                                    locs[0] = new DistanceMatrix.Client.Location(locations[i].Latitude, locations[i].Longitude);
//                                    locs[1] = new DistanceMatrix.Client.Location(locations[j].Latitude, locations[j].Longitude);
//                                    DistanceMatrix.Client.Models.RouteResponse response = osrm.Route(locs);
//                                    if (response.Code.ToUpper() == "OK")
//                                        metros = (long)response.Routes[0].Distance;
//                                    else
//                                    {
//                                        metros = (long)Geo.DistanciaRaio(locations[i].Latitude, locations[i].Longitude, locations[j].Latitude, locations[j].Longitude);
//                                        new Services.Errors().GravaLogEmTxt(AppDomain.CurrentDomain.BaseDirectory, new Exception("DistanceCallback - OSRM status <> OK -> " + response.Code.ToUpper() + " request " + osrm.FullUrl));
//                                    }
//                                }
//                                custos[i][j] = metros;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex) { throw ex; }
//            finally { cnn.Disconnect(); }
//        }

//        private Position[] locations;
//        private long[][] custos;

//        public override long Run(int i, int j)
//        {
//            if (i >= locations.Length || j >= locations.Length)
//                return 0;
//            else if (i == j)
//                return 0;
//            else
//                return custos[i][j];
//        }
//    }
//}