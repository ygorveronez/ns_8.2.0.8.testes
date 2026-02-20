using Osrm.Client.v5;
using System;
using System.Collections.Generic;

namespace Google.OrTools.Api.Services
{
    public class DistanceMatrix
    {
        public long[][] DurationMatrix;

        public long[][] ObterDistanceMatrix(Models.Problem problema)
        {
            int indiceDeposito = problema.Locais.FindIndex(x => x.Deposito);
            Osrm5x osrm = new Osrm5x(problema.ServerOsrm);

            long[][] DistanceMatrix = null;
            try
            {
                Services.GoogleOrTools.Position[] locations = problema.Locais.ToArray();

                //Inicializando a matriz de custos
                DistanceMatrix = new long[locations.Length][];
                DurationMatrix = new long[locations.Length][];

                for (int i = 0; i < locations.Length; i++)
                {
                    DistanceMatrix[i] = new long[locations.Length];
                    DurationMatrix[i] = new long[locations.Length];
                }

                // vamos calcular uma matriz de 25x25
                int maxSize = 50;
                int qtde = locations.Length;

                List<Osrm.Client.Location> locsOrigem = new List<Osrm.Client.Location>();
                for (int i = 0; i < locations.Length; i++)
                {
                    //Adicionando as origens 
                    locsOrigem.Add(new Osrm.Client.Location(locations[i].Latitude, locations[i].Longitude));
                    //Se a quantidade for ao max de origens ou o último registro.
                    if ((i + 1) % maxSize == 0 || (i + 1 == locations.Length))
                    {
                        uint[] source = new uint[locsOrigem.Count];
                        for (int s = 0; s < locsOrigem.Count; s++)
                            source[s] = (uint)s;

                        List<Osrm.Client.Location> locsDestino = new List<Osrm.Client.Location>();
                        for (int j = 0; j < locations.Length; j++)
                        {
                            locsDestino.Add(new Osrm.Client.Location(locations[j].Latitude, locations[j].Longitude));
                            if ((j + 1) % maxSize == 0 || (j + 1 == locations.Length))
                            {
                                uint[] destinations = new uint[locsDestino.Count];
                                for (int d = 0; d < locsDestino.Count; d++)
                                    destinations[d] = (uint)(source.Length + d);

                                var locais = new List<Osrm.Client.Location>();
                                locais.AddRange(locsOrigem);
                                locais.AddRange(locsDestino);

                                //Returns a asymmetric 3x2 matrix with from the polyline encoded locations qikdcB}~dpXkkHz:
                                Osrm.Client.Models.Responses.TableResponse response = osrm.Table(new Osrm.Client.Models.TableRequest()
                                {
                                    Coordinates = locais.ToArray(),
                                    SendCoordinatesAsPolyline = true,
                                    Sources = source,
                                    Destinations = destinations
                                });

                                if (response.Code.ToUpper() == "OK")
                                {
                                    for (int r = 0; r < response.Distances.Length; r++)
                                    {
                                        for (int t = 0; t < response.Distances[r].Length; t++)
                                        {
                                            long metros = 0;
                                            metros = (long)response.Distances[r][t];
                                            double tempo = (double)response.Durations[r][t];

                                            if (metros == 0)
                                                metros = (long)Services.Geo.DistanciaRaio(locais[r].Latitude, locais[r].Longitude, locais[t].Latitude, locais[t].Longitude);
                                            int x = i - (locsOrigem.Count - 1) + r;
                                            int y = j - (locsDestino.Count - 1) + t;

                                            //if (x == indiceDeposito && problema.TipoRota == Models.EnumTipoRota.SemIda)
                                            //{
                                            //    DistanceMatrix[x][y] = 0;
                                            //    DurationMatrix[x][y] = 0;
                                            //}
                                            //else if (y == indiceDeposito && problema.TipoRota == Models.EnumTipoRota.SemRetorno)
                                            //{
                                            //    DistanceMatrix[x][y] = 0;
                                            //    DurationMatrix[x][y] = 0;
                                            //}
                                            //else
                                            //{
                                            DistanceMatrix[x][y] = metros;
                                            DurationMatrix[x][y] = (int)(tempo / 60 + (x != y ? 1 : 0));
                                            //}

                                            //if (x == indiceDeposito && problema.DesconsiderarTempoDeslocamentoPrimeiraEntrega)
                                            //    DurationMatrix[x][y] = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    Errors.GravaLogEmTxt("DistanceMatrix - OSRM status <> OK -> " + response.Code.ToUpper() + " request " + osrm.FullUrl);
                                    for (int r = 0; r < locais.Count; r++)
                                    {
                                        for (int t = 0; t < locais.Count; t++)
                                        {
                                            long metros = (long)Services.Geo.DistanciaRaio(locais[r].Latitude, locais[r].Longitude, locais[t].Latitude, locais[t].Longitude);
                                            int x = i - (locsOrigem.Count - 1) + r;
                                            int y = j - (locsDestino.Count - 1) + t;

                                            //if (x == indiceDeposito && problema.TipoRota == Models.EnumTipoRota.SemIda)
                                            //{
                                            //    DistanceMatrix[x][y] = 0;
                                            //    DurationMatrix[x][y] = 0;
                                            //}
                                            //else if (y == indiceDeposito && problema.TipoRota == Models.EnumTipoRota.SemRetorno)
                                            //{
                                            //    DistanceMatrix[x][y] = 0;
                                            //    DurationMatrix[x][y] = 0;
                                            //}
                                            //else
                                            //{
                                            DistanceMatrix[x][y] = metros;
                                            DistanceMatrix[x][y] = (long)((metros / 1000) * 40) / 60;
                                            //}

                                            //if (x == indiceDeposito && problema.DesconsiderarTempoDeslocamentoPrimeiraEntrega)
                                            //    DurationMatrix[x][y] = 0;
                                        }
                                    }
                                }

                                locsDestino = new List<Osrm.Client.Location>();
                            }
                        }
                        locsOrigem = new List<Osrm.Client.Location>();
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.GravaLog(ex, "DistanceMatrix - Exception ->  request " + osrm.FullUrl);
                throw;
            }

            return DistanceMatrix;
        }
    }
}