using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Logistica
{
    public class OpcoesRoteirizar
    {
        public OpcoesRoteirizar()
        {
            Ordenar = false;
            AteOrigem = false;
            PontosNaRota = false;
            MathRaio = 50;
            AgruparPosicoesPorDistancia = false;
        }
        public bool Ordenar { get; set; }
        public bool AteOrigem { get; set; }
        public bool PontosNaRota { get; set; }
        //Quando Pontos em Rota, é útilizado o serviço Match para validar/aproximar o ponto na rua
        // e esse parametro é o limite da aproximação..
        public int MathRaio { get; set; }
        public bool AgruparPosicoesPorDistancia { get; set; }
    }

    public class Roteirizacao
    {
        #region Propriedades Privadas

        private readonly string _apiUrl;
        private List<WayPoint> _listaWayPoint = new List<WayPoint>();

        #endregion

        #region Construtores

        public Roteirizacao(string apiUrl = null)
        {
            _apiUrl = !string.IsNullOrWhiteSpace(apiUrl)
                ? PadronizarApiUrl(apiUrl)
                : "https://2404.routeasy.com.br";

        }

        #endregion

        #region Métodos Privados para URL

        private string PadronizarApiUrl(string url)
        {
            url = url.TrimEnd('/');

            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "http://" + url;
            }

            return url;
        }

        private string MontarApiUrl(string endpoint, string queryParams = null)
        {
            string url = _apiUrl;

            if (!string.IsNullOrEmpty(endpoint))
            {
                if (!endpoint.StartsWith("/"))
                    url += "/";

                url += endpoint;
            }

            if (!string.IsNullOrEmpty(queryParams))
                url += queryParams.StartsWith("?") ? queryParams : "?" + queryParams;

            return url;
        }

        #endregion

        #region Métodos Públicos

        public RespostaRoteirizacao Roteirizar(OpcoesRoteirizar opcoesRoteirizar)
        {

            if (opcoesRoteirizar.AgruparPosicoesPorDistancia)
                _listaWayPoint = GetPosicoesAgrupadasPorDistancia(_listaWayPoint);

            if (_listaWayPoint.Count < 2)
                return ObterRespostaErro();

            if (opcoesRoteirizar.PontosNaRota)
                _listaWayPoint = GetPontosNaRota(opcoesRoteirizar.MathRaio);

            List<WayPoint> pontosRetorno;
            if (opcoesRoteirizar.Ordenar)
            {
                pontosRetorno = GetPontosEmOrdem(opcoesRoteirizar.AteOrigem, false);
            }
            else
            {
                pontosRetorno = _listaWayPoint;
            }

            ResponseRouter resposta = RequestRoute(pontosRetorno, false);
            if (resposta == null)
            {
                return new RespostaRoteirizacao { Status = "Erro ao roteirizar" };
            }

            GerarTempoPontoRetorno(resposta, pontosRetorno);

            var respostaRoteirizacao = new RespostaRoteirizacao()
            {
                Status = "OK",
                Distancia = Convert.ToDecimal(resposta.Routes[0].Distance / 1000),
                Polilinha = resposta.Routes[0].Geometry.ToString(),
                TempoMinutos = Convert.ToInt32(resposta.Routes[0].Duration / 60),
                TempoHoras = Convert.ToInt32(resposta.Routes[0].Duration / 60 / 60),
                PontoDaRota = GetListaPontos(pontosRetorno),
                Informacoes = GetInforamcaoes(pontosRetorno)
            };

            return respostaRoteirizacao;

        }

        public RespostaRoteirizacao Roteirizar()
        {
            if (_listaWayPoint.Count < 2) return ObterRespostaErro();

            ResponseRouter resposta = RequestRoute(_listaWayPoint, false);
            if (resposta == null)
            {
                return new RespostaRoteirizacao { Status = "Erro ao roteirizar" };
            }

            var respostaRoteirizacao = new RespostaRoteirizacao()
            {
                Status = "OK",
                Distancia = Convert.ToDecimal(resposta.Routes[0].Distance / 1000),
                Polilinha = resposta.Routes[0].Geometry.ToString(),
            };

            return respostaRoteirizacao;
        }

        public void Clear()
        {
            _listaWayPoint.Clear();

        }

        public void Add(WayPoint waypoint)
        {
            _listaWayPoint.Add(waypoint);
        }

        public void Add(List<WayPoint> waypoints)
        {
            _listaWayPoint.AddRange(waypoints);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _listaWayPoint.Count) return;
            _listaWayPoint.RemoveAt(index);
        }

        public void OrdenarPredefinicao(bool coletasSempreInicio)
        {
            if (_listaWayPoint.Any(obj => obj.SequenciaPredefinida > 0))
                _listaWayPoint = _listaWayPoint.OrderBy(obj => obj.SequenciaPredefinida).ToList();

            List<WayPoint> coletas = _listaWayPoint.FindAll(x => x.TipoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta).OrderBy(obj => obj.SequenciaPredefinida).ToList();
            if (coletasSempreInicio && coletas.Count > 1)
            {
                List<WayPoint> outros = _listaWayPoint.FindAll(x => x.TipoPonto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta).OrderBy(obj => obj.SequenciaPredefinida).ToList();
                _listaWayPoint = new List<WayPoint>();
                _listaWayPoint.AddRange(coletas);
                _listaWayPoint.AddRange(outros);
            }
        }

        public int GetUltimaOrdemPredefinicao()
        {
            return (from obj in _listaWayPoint select obj.SequenciaPredefinida).Max();
        }

        public string GetListaPontos(List<WayPoint> pontosRetorno)
        {
            List<PontosDaRota> listaPontosDaRota = new List<PontosDaRota>();
            int countLeg = 0;
            foreach (WayPoint ponto in pontosRetorno)
            {
                PontosDaRota pontos = new PontosDaRota
                {
                    descricao = ponto.Descricao,
                    lat = ponto.Lat,
                    lng = ponto.Lng,
                    pedagio = ponto.Pedagio,
                    pontopassagem = (ponto.TipoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem),
                    tempo = ponto.Tempo,
                    coletaEquipamento = ponto.ColetaEquipamento,
                    usarOutroEndereco = ponto.UsarOutroEndereco,
                    distancia = ponto.Distancia,
                    codigo = ponto.Codigo,
                    codigo_cliente = ponto.CodigoCliente,
                    tipoponto = ponto.TipoPonto,
                    primeiraEntrega = ponto.PrimeiraEntrega,
                    codigoOutroEndereco = ponto.CodigoOutroEndereco,
                    utilizaLocalidade = ponto.UtilizaLocalidade
                };

                countLeg++;

                listaPontosDaRota.Add(pontos);
            }

            return JsonConvert.SerializeObject(listaPontosDaRota);
        }

        public string GetPontosNaRotaRaios(List<WayPoint> listpoint, int raio)
        {
            var strraio = "";

            foreach (var way in listpoint)
            {
                if (strraio != "")
                    strraio = strraio + ';';

                strraio = strraio + raio.ToString();
            }

            return strraio;
        }

        public List<WayPoint> GetPontosNaRota(int radius)
        {
            try
            {
                string points = GetPontosOpenStreetMap(_listaWayPoint);
                string raios = GetPontosNaRotaRaios(_listaWayPoint, radius);
                string param = $"radiuses={raios}";

                string endpoint;
                bool isRouteEasy = _apiUrl.IndexOf("routeasy", StringComparison.OrdinalIgnoreCase) >= 0;

                if (isRouteEasy)
                    endpoint = $"match/v1/driving/{points}";
                else
                    endpoint = $"osrm/match/v1/driving/{points}";

                string url = MontarApiUrl(endpoint, param);

                string strget = GetRequest(url);
                if (string.IsNullOrEmpty(strget))
                {
                    Servicos.Log.TratarErro($"GetPontosNaRota - Empty response from API: {url}", "RoteirizacaoGetPontosNaRota");
                    return _listaWayPoint;
                }

                var responseMath = JsonConvert.DeserializeObject<ResponseMatch>(strget);

                if ((responseMath.code != "Ok") || (_listaWayPoint.Count != responseMath?.tracepoints?.Count))
                    return _listaWayPoint;

                for (var i = 0; i < _listaWayPoint.Count; i++)
                {
                    Tracepoint tracePoint = responseMath.tracepoints[i];
                    if (tracePoint != null)
                    {
                        _listaWayPoint[i].Lat = tracePoint.location[1];
                        _listaWayPoint[i].Lng = tracePoint.location[0];
                    }
                }

                return _listaWayPoint;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "RoteirizacaoGetPontosNaRota");
                return _listaWayPoint;
            }
        }

        public List<WayPoint> GetPontosEmOrdem(bool ateorigem, bool manterPontoDestino)
        {
            try
            {
                List<WayPoint> listaOrdenada = new List<WayPoint>();

                if (_listaWayPoint.Count <= 1)
                    return listaOrdenada;

                //Pontos de coleta
                WayPoint origem = _listaWayPoint[0];
                List<WayPoint> pontosColeta = GetPontosColeta(ateorigem);
                pontosColeta.Insert(0, origem);
                List<WayPoint> pontosColetaOrdenados = GetPontosEmOrdem(pontosColeta, null, null);

                List<WayPoint> postosFiscais = GetPontosPostoFiscal(ateorigem);
                List<WayPoint> postosFiscaisOrdenados = postosFiscais.Count > 1 ? GetPontosEmOrdem(postosFiscais, null, null) : postosFiscais;

                WayPoint ultimoPontoColeta = pontosColetaOrdenados.Count == 0 ? origem : pontosColetaOrdenados.LastOrDefault();

                List<WayPoint> pontosSemColeta = GetSemPontosColeta(ateorigem);
                WayPoint primeiraEntrega = (from obj in pontosSemColeta where obj.PrimeiraEntrega select obj).FirstOrDefault();

                if (primeiraEntrega != null)
                {
                    pontosSemColeta.Remove(primeiraEntrega);
                    pontosSemColeta.Insert(0, primeiraEntrega);
                    //ultimoPontoColeta = new WayPoint();
                    //ultimoPontoColeta = primeiraEntrega;
                }
                else
                {
                    pontosSemColeta.Insert(0, ultimoPontoColeta);
                }

                //Não estava otimizando corretante a sequencia de entregas quando cargas até a origem.
                // #4649 - FRIMESA
                if (ateorigem)
                    pontosSemColeta.Add(origem);

                if ((!ateorigem) && (!manterPontoDestino))
                    AlterarUltimoPonto(pontosSemColeta);

                List<WayPoint> outrosPontosOrdenados = GetPontosEmOrdem(pontosSemColeta, ultimoPontoColeta, primeiraEntrega);

                if (ateorigem)
                    outrosPontosOrdenados.RemoveAt(outrosPontosOrdenados.Count - 1);

                listaOrdenada.Add(origem);
                listaOrdenada.AddRange(pontosColetaOrdenados);
                listaOrdenada.AddRange(postosFiscaisOrdenados);
                listaOrdenada.AddRange(outrosPontosOrdenados);

                if (ateorigem)
                {
                    WayPoint retorno = (from ret in _listaWayPoint
                                        where ret.TipoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno
                                        select ret).FirstOrDefault();
                    if (retorno != null)
                        listaOrdenada.Add(retorno);
                }

                return listaOrdenada;
            }
            catch
            {
                return _listaWayPoint;
            }

        }

        public List<WayPoint> GetWayPoints()
        {
            return _listaWayPoint;
        }

        public RespostaRoteirizacao RoteirizarGoogle(bool ordenar, bool ateOrigem, string key)
        {
            List<WayPoint> pontosRetorno;

            if (ordenar)
                pontosRetorno = GetPontosEmOrdem(ateOrigem, false);
            else
                pontosRetorno = _listaWayPoint;

            var listaDivisao = DividirPontos(pontosRetorno);

            var distancia = 0;
            var tempo = 0;
            var distanciaIda = 0;
            var tempoIda = 0;
            var distanciaVolta = 0;
            var tempoVolta = 0;

            var pontospolilinha = new List<WayPoint>();
            var pontospolilinhaIda = new List<WayPoint>();
            var pontospolilinhaVolta = new List<WayPoint>();

            var countdiv = 0;
            var ehVolta = false;
            foreach (var pontosdiv in listaDivisao)
            {
                countdiv++;

                string origem = GetPointString(pontosdiv[0].Lat) + "," + GetPointString(pontosdiv[0].Lng);
                string destino = GetPointString(pontosdiv[pontosdiv.Count - 1].Lat) + "," + GetPointString(pontosdiv[pontosdiv.Count - 1].Lng);

                string waypoint = "";
                for (int i = 1; i < pontosdiv.Count - 1; i++)
                {
                    if (waypoint != "")
                        waypoint += "|";

                    waypoint += GetPointString(pontosdiv[i].Lat) + "," + GetPointString(pontosdiv[i].Lng);
                }

                string url = "https://maps.googleapis.com/maps/api/directions/json?optimize=true&origin=" + origem + "&destination=" + destino + "&waypoints=" + waypoint + $"&key={key}";

                string strresposta = GetRequest(url);

                var resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Google.Route>(strresposta);

                if (resposta.routes.Count == 0)
                    throw new Exception($"Erro ao roteirizar: {url} ");


                var j = 0;
                foreach (var leg in resposta.routes[0].legs)
                {
                    ehVolta = countdiv == listaDivisao.Count() && j >= resposta.routes[0].legs.Count - 2 && ateOrigem;
                    j++;

                    pontosRetorno[j].Distancia = leg.distance.value;
                    pontosRetorno[j].Tempo = leg.duration.value;

                    distancia = distancia + leg.distance.value;
                    tempo = tempo + leg.duration.value;

                    if (ehVolta)
                    {
                        distanciaVolta = distanciaVolta + leg.distance.value;
                        tempoVolta = tempoVolta + leg.duration.value;

                    }
                    else
                    {
                        distanciaIda = distanciaIda + leg.distance.value;
                        tempoIda = tempoIda + leg.duration.value;
                    }

                    foreach (var step in leg.steps)
                    {
                        var decode = DecodePolyline(step.polyline.points);
                        pontospolilinha.AddRange(decode);

                        if (ehVolta)
                            pontospolilinhaVolta.AddRange(decode);
                        else
                            pontospolilinhaIda.AddRange(decode);

                    }
                }
            }

            var polilinha = EncodePolyline(pontospolilinha);
            var polilinhaIda = EncodePolyline(pontospolilinhaIda);
            var polilinhaVolta = EncodePolyline(pontospolilinhaVolta);
            var pontosRetornoIda = pontosRetorno;
            var pontosRetornoVolta = new List<WayPoint>();

            if (ehVolta)
            {
                pontosRetornoIda = pontosRetorno.GetRange(0, pontosRetorno.Count - 1);
                pontosRetornoVolta = pontosRetorno.GetRange(pontosRetorno.Count - 2, 2);
            }

            var respostaRoteirizacao = new RespostaRoteirizacao()
            {
                Status = "OK",
                Distancia = Convert.ToDecimal(distancia / 1000),
                Polilinha = polilinha,
                TempoMinutos = tempo / 60,
                TempoHoras = tempo / 60 / 60,
                PontoDaRota = GetListaPontos(pontosRetorno),
                Informacoes = GetInforamcaoes(pontosRetorno),
                Ida = {
                    Distancia = Convert.ToDecimal(distanciaIda / 1000),
                    Polilinha = polilinhaIda,
                    TempoMinutos = tempoIda / 60,
                    TempoHoras = tempoIda / 60 / 60,
                    PontoDaRota = GetListaPontos(pontosRetornoIda),
                    Informacoes = GetInforamcaoes(pontosRetornoIda)
                },

                Volta =
                {
                    Distancia = Convert.ToDecimal(distanciaVolta / 1000),
                    Polilinha = polilinhaVolta,
                    TempoHoras = tempoVolta / 60,
                    TempoMinutos = tempoVolta / 60 / 60,
                    PontoDaRota = GetListaPontos(pontosRetornoVolta),
                    Informacoes = GetInforamcaoes(pontosRetornoVolta)
                }

            };

            return respostaRoteirizacao;
        }

        public ResponseRouter OsrmTable(List<WayPoint> listaWayPoint, uint[] source = null, uint[] destinations = null)
        {
            string points = GetPontosOpenStreetMap(listaWayPoint);
            string param = "annotations=distance,duration";

            if (source != null)
                param += "&sources=" + string.Join(";", source);
            if (destinations != null)
                param += "&destinations=" + string.Join(";", destinations);

            string endpoint;
            bool isRouteEasy = _apiUrl.IndexOf("routeasy", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isRouteEasy)
                endpoint = $"table/v1/driving/{points}";
            else
                endpoint = $"osrm/table/v1/driving/{points}";

            string url = MontarApiUrl(endpoint, param);

            try
            {
                string strget = GetRequest(url);
                return JsonConvert.DeserializeObject<ResponseRouter>(strget);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"OsrmTable failed - {url}", "RoteirizacaoOsrmTable");
                Servicos.Log.TratarErro(ex, "RoteirizacaoOsrmTable");
                return null;
            }
        }

        public double BuscarDistancia()
        {
            RespostaRoteirizacao resposta = Roteirizar();
            if (resposta.Status == "OK")
            {
                return (double)resposta.Distancia;
            }
            return 0;
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint ObterWaypoint(double latitude, double longitude, string descricaoPonto, int sequencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem tipoPontoPassagem)
        {
            if (latitude < -90 || latitude > 90)
                throw new ServicoException($" {Localization.Resources.Logistica.RotaFrete.LatitudeLongitudeInvalida} {descricaoPonto}");

            return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint
            {
                Lat = latitude,
                Lng = longitude,
                Descricao = descricaoPonto,
                Sequencia = sequencia,
                TipoPonto = tipoPontoPassagem,
            };
        }

        #endregion

        #region Métodos Privados

        private string GetPointString(Double ponto)
        {
            string formato = "0.00000000";
            return ponto.ToString(formato, System.Globalization.CultureInfo.InvariantCulture);
        }

        private string GetPontosOpenStreetMap(List<WayPoint> listpoint)
        {
            StringBuilder strway = new StringBuilder();

            for (int i = 0; i < listpoint.Count; i++)
            {
                if (i > 0)
                    strway.Append(';');

                strway.Append(GetPointString(listpoint[i].Lng));
                strway.Append(',');
                strway.Append(GetPointString(listpoint[i].Lat));
            }

            return strway.ToString();
        }

        private int GetIndicePontoMaisDistante(List<WayPoint> listaWayPoint)
        {
            var idx = -1;
            try
            {
                double maiordistancia = 0;
                var waypointsopen = this.OsrmTable(listaWayPoint);

                if (waypointsopen.Distances.Count > 0)
                {
                    var listadistancia = waypointsopen.Distances[0];
                    for (var i = 0; i < listadistancia.Count; i++)
                    {
                        var distancia = listadistancia[i];
                        if (distancia > maiordistancia)
                        {
                            maiordistancia = distancia;
                            idx = i;
                        }
                    }
                }
            }
            catch
            {
                return -1;
            }
            return idx;
        }

        private void AlterarUltimoPonto(List<WayPoint> listaWayPoint)
        {
            int idx = GetIndicePontoMaisDistante(listaWayPoint);

            if (idx >= 0)
            {
                WayPoint ponto = listaWayPoint[idx];

                if (idx > 0)
                {
                    listaWayPoint.RemoveAt(idx);

                    listaWayPoint.Add(ponto);
                }
            }
        }

        private string GetRequest(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"GetRequest - {url}", "RoteirizacaoRequestRoute");
                Servicos.Log.TratarErro(ex, "RoteirizacaoRequestRoute");
                return null;
            }
        }

        private string GetInforamcaoes(List<WayPoint> pontosRetorno)
        {
            string info = "";
            foreach (var ponto in pontosRetorno)
            {
                if (info != "")
                    info = info + ",";

                if (ponto.Informacao != "")
                    info = info + ponto.Informacao;
            }

            return info;
        }

        private List<List<WayPoint>> DividirPontos(List<WayPoint> pontos)
        {
            var lista = new List<List<WayPoint>>();

            int pontoini = 0;
            int pontofim = 23;
            while (pontoini < pontos.Count)
            {
                if (pontoini > pontos.Count)
                    pontoini = pontos.Count;

                if ((pontoini + pontofim) > pontos.Count)
                    pontofim = pontos.Count - pontoini;

                var range = pontos.GetRange(pontoini, pontofim);
                lista.Add(range);

                pontoini = pontoini + 22;
            }

            return lista;

        }

        private static string EncodePolyline(IEnumerable<WayPoint> points)
        {
            var str = new StringBuilder();

            var encodeDiff = (Action<int>)(diff =>
            {
                int shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;

                int rem = shifted;

                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));

                    rem >>= 5;
                }

                str.Append((char)(rem + 63));
            });

            int lastLat = 0;
            int lastLng = 0;

            foreach (var point in points)
            {
                int lat = (int)Math.Round(point.Lat * 1E5);
                int lng = (int)Math.Round(point.Lng * 1E5);

                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);

                lastLat = lat;
                lastLng = lng;
            }

            return str.ToString();
        }

        private static List<WayPoint> DecodePolyline(string encodedPoints)
        {
            var ListaWayPoint = new List<WayPoint>();
            char[] polylineChars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                var Lat = Convert.ToDouble(currentLat) / 1E5;
                var Lng = Convert.ToDouble(currentLng) / 1E5;

                ListaWayPoint.Add(new WayPoint() { Lat = Lat, Lng = Lng });

            }

            return ListaWayPoint;

        }

        private static RespostaRoteirizacao ObterRespostaErro()
        {
            var respostaRoteirizacao = new RespostaRoteirizacao()
            {
                Status = "Erro",
                Distancia = 0,
                Polilinha = "",
                TempoMinutos = 0,
                TempoHoras = 0,
                PontoDaRota = "",
                Informacoes = ""
            };

            return respostaRoteirizacao;
        }

        private void GerarTempoPontoRetorno(ResponseRouter resposta, List<WayPoint> pontosRetorno)
        {
            try
            {
                if ((resposta?.Routes == null) || (resposta?.Routes[0].Legs.Count != pontosRetorno.Count - 1))
                    return;

                bool primeiroPonto = true;
                int id = -1;
                foreach (var ponto in pontosRetorno)
                {
                    ponto.Distancia = primeiroPonto ? ponto.Distancia = 0 : ponto.Distancia = Convert.ToInt32(resposta.Routes[0].Legs[id].Distance);
                    ponto.Tempo = primeiroPonto ? ponto.Tempo = 0 : ponto.Tempo = Convert.ToInt32(resposta.Routes[0].Legs[id].Duration / 60);

                    primeiroPonto = false;
                    id++;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "RoteirizacaoRequestRoute");
            }
        }

        private ResponseRouter GetPontos(List<WayPoint> listaPontos)
        {
            if (listaPontos.Count <= 1)
                return null;

            string points = GetPontosOpenStreetMap(listaPontos);
            string param = "roundtrip=false&source=first&destination=last&steps=false&annotations=false&overview=false";

            string endpoint;
            bool isRouteEasy = _apiUrl.IndexOf("routeasy", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isRouteEasy)
                endpoint = $"trip/v1/driving/{points}";
            else
                endpoint = $"osrm/trip/v1/driving/{points}";

            string url = MontarApiUrl(endpoint, param);

            try
            {
                string strget = GetRequest(url);
                return JsonConvert.DeserializeObject<ResponseRouter>(strget);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"GetPontos failed - {url}", "RoteirizacaoGetPontos");
                Servicos.Log.TratarErro(ex, "RoteirizacaoGetPontos");
                return null;
            }
        }

        private List<WayPoint> GetPontosPostoFiscal(bool ateorigem)
        {
            List<WayPoint> postosFiscais = new List<WayPoint>();

            if (_listaWayPoint.Count <= 1)
                return postosFiscais;

            int indiceFinal = _listaWayPoint.Count;
            if (ateorigem)
                indiceFinal = _listaWayPoint.Count - 1;


            for (var i = 1; i < indiceFinal; i++)
            {
                var ponto = _listaWayPoint[i];
                if (ponto.TipoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.PostoFiscal)
                    postosFiscais.Add(ponto);
            }

            //Origem
            //if (pontosColeta.Count > 0)
            //    pontosColeta.Insert(0, _listaWayPoint[0]);

            return postosFiscais;
        }

        private List<WayPoint> GetPontosColeta(bool ateorigem)
        {
            List<WayPoint> pontosColeta = new List<WayPoint>();

            if (_listaWayPoint.Count <= 1)
                return pontosColeta;

            int indiceFinal = _listaWayPoint.Count;
            if (ateorigem)
                indiceFinal = _listaWayPoint.Count - 1;


            for (var i = 1; i < indiceFinal; i++)
            {
                var ponto = _listaWayPoint[i];
                if (ponto.TipoPonto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                    pontosColeta.Add(ponto);
            }

            //Origem
            //if (pontosColeta.Count > 0)
            //    pontosColeta.Insert(0, _listaWayPoint[0]);

            return pontosColeta;
        }

        private List<WayPoint> GetSemPontosColeta(bool ateorigem)
        {
            List<WayPoint> pontosSemColeta = new List<WayPoint>();

            if (_listaWayPoint.Count <= 1)
                return pontosSemColeta;

            for (var i = 1; i < _listaWayPoint.Count; i++)
            {
                var ponto = _listaWayPoint[i];
                if (ponto.TipoPonto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta && ponto.TipoPonto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno && ponto.TipoPonto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.PostoFiscal)
                    pontosSemColeta.Add(ponto);
            }
            //Comentado pois não entendi a logica de adicionar o mesmo ponto 2x no final da fila.. sendo que no setar pontos passagem está ficando duplicado o mesmo local.
            //if (ateorigem)
            //    pontosSemColeta.Add(_listaWayPoint[_listaWayPoint.Count - 1]);

            return pontosSemColeta;
        }

        private List<WayPoint> GetPontosEmOrdem(List<WayPoint> _listaPontos, WayPoint ultimoPontoColeta, WayPoint primeiraEntrega)
        {
            ResponseRouter pontos = null;
            if (primeiraEntrega == null)
                pontos = GetPontos(_listaPontos);
            else
            {
                _listaPontos.Add(ultimoPontoColeta);
                pontos = GetPontos(_listaPontos);
                _listaPontos.RemoveAt(_listaPontos.Count - 1);
            }

            List<WayPoint> pontosordenados = new List<WayPoint>();

            if (pontos == null)
            {
                //remove origem
                if (_listaPontos.Count > 0)
                    _listaPontos.RemoveAt(0);

                pontosordenados.AddRange(_listaPontos);
                return pontosordenados;
            }

            List<WayPoint> pontosindice = new List<WayPoint>();

            if (ultimoPontoColeta != null && primeiraEntrega != null)
            {
                pontosindice.Add(ultimoPontoColeta);
                pontosindice[0].Index = 0;
            }

            if (pontos != null)
            {
                if (pontos.Waypoints.Count != (_listaPontos.Count + (primeiraEntrega == null ? 0 : 1)))
                    throw new Exception("Erro ao obter pontos em ordem.");

                int contPrimeiraEntrega = pontosindice.Count;
                int i = 0;
                foreach (var way in pontos.Waypoints)
                {
                    if (i > _listaPontos.Count - 1) continue;
                    var wayori = _listaPontos[i];

                    pontosindice.Add(new WayPoint
                    {
                        Lat = wayori.Lat,
                        Lng = wayori.Lng,
                        Descricao = wayori.Descricao,
                        Pedagio = wayori.Pedagio,
                        Fronteira = wayori.Fronteira,
                        Index = way.Waypoint_index + contPrimeiraEntrega,
                        Informacao = wayori.Informacao,
                        Tempo = 0,
                        Distancia = 0,
                        Codigo = wayori.Codigo,
                        CodigoCliente = wayori.CodigoCliente,
                        Sequencia = wayori.Sequencia,
                        TipoPonto = wayori.TipoPonto,
                        UsarOutroEndereco = wayori.UsarOutroEndereco,
                        LocalDeParqueamento = wayori.LocalDeParqueamento,
                        tempoEstimadoPermanencia = wayori.tempoEstimadoPermanencia,
                        CodigoOutroEndereco = wayori.CodigoOutroEndereco,
                        UtilizaLocalidade = wayori?.UtilizaLocalidade ?? false
                    });

                    i++;

                }
            }

            pontosordenados.AddRange(pontosindice.OrderBy(item => item.Index));

            //remove origem
            if (pontosordenados.Count > 0)
                pontosordenados.RemoveAt(0);

            return pontosordenados;
        }

        private ResponseRouter RequestRoute(List<WayPoint> pontos, bool steps = true)
        {
            if (pontos.Count < 2) return null;

            string points = GetPontosOpenStreetMap(pontos);
            string param = $"overview=full&geometries=polyline&annotations=false&alternatives=false";
            if (steps) param += "&steps=true";

            string endpoint;
            bool isRouteEasy = _apiUrl.IndexOf("routeasy", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isRouteEasy)
                endpoint = $"route/v1/driving/{points}";
            else
                endpoint = $"osrm/route/v1/driving/{points}";

            string url = MontarApiUrl(endpoint, param);

            try
            {
                string strresposta = GetRequest(url);

                if (string.IsNullOrEmpty(strresposta))
                {
                    Servicos.Log.TratarErro($"1 - RequestRoute response null {url}", "RoteirizacaoRequestRoute");
                    return null;
                }

                return JsonConvert.DeserializeObject<ResponseRouter>(strresposta);
            }
            catch
            {
                Servicos.Log.TratarErro($"Erro ao roteirizar request {url}", "RoteirizacaoRequestRoute");
                throw;
            }
        }

        private List<WayPoint> GetPosicoesAgrupadasPorDistancia(List<WayPoint> posicoesVeiculo)
        {
            int MaxPontos = 300;
            if (posicoesVeiculo.Count < MaxPontos)
                return posicoesVeiculo;

            var listaPontos = new List<WayPoint>();
            double LngAnterior;
            double LatAnterior;
            int distanciaMinima = 20;
            int total = posicoesVeiculo.Count;
            while (true)
            {
                // Garante sempre o primeiro ponto da rota
                listaPontos.Add(new WayPoint(posicoesVeiculo[0].Lat, posicoesVeiculo[0].Lng));
                LatAnterior = posicoesVeiculo[0].Lat;
                LngAnterior = posicoesVeiculo[0].Lng;

                // A partir do segundo até o penúltimo
                for (int i = 1; i < total - 1; i++)
                {
                    // Adicionar posicoes a partir da distância mínima
                    var distancia = Distancia.CalcularDistanciaMetros(LatAnterior, LngAnterior, posicoesVeiculo[i].Lat, posicoesVeiculo[i].Lng);
                    if (distancia > distanciaMinima)
                    {
                        listaPontos.Add(new WayPoint(posicoesVeiculo[i].Lat, posicoesVeiculo[i].Lng));
                        LatAnterior = posicoesVeiculo[i].Lat;
                        LngAnterior = posicoesVeiculo[i].Lng;
                    }
                }

                // Garante sempre o último ponto da rota
                listaPontos.Add(new WayPoint(posicoesVeiculo[total - 1].Lat, posicoesVeiculo[total - 1].Lng));

                // Foi possível reduzir a quantidade de pontos para não ultrapassar o limite máximo
                if (listaPontos.Count < MaxPontos) break;

                // ... ainda está maior que o limite
                // Aumenta a distância mínima entre os pontos para simplificar ainda mais
                distanciaMinima += 50;
                listaPontos.Clear();
            }

            return listaPontos;
        }

        #endregion
    }
}
