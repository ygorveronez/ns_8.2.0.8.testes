using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public class MapRequestApi : ServicoBase
    {
        public MapRequestApi() : base() { }        
        public MapRequestApi(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        private string key = "Fmjtd%7Cluu821uz2g%2Crx%3Do5-94bnhz";//"FUq6EsAdVp5HuCVBfOf0AVyAAaV2EP4B"; //Fmjtd%7Cluu821uz2g%2Crx%3Do5-94bnhz

        public Dominio.Entidades.Embarcador.Logistica.Rota CriarRota(Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarRotaPorOrigemDestino(origem.Codigo, destino.Codigo);

                Servicos.Embarcador.Carga.CargaRotaFrete servicoCargaRotaFrete = new Servicos.Embarcador.Carga.CargaRotaFrete(unitOfWork);
                int km = servicoCargaRotaFrete.CalcularDistanciaPorOrigemEDestino(origem, destino);

                if (rota == null)
                {
                    rota = new Dominio.Entidades.Embarcador.Logistica.Rota();
                    rota.Origem = origem;
                    rota.Destino = destino;
                    rota.Ativo = true;
                    rota.PossuiPedagio = false;
                    rota.DistanciaKM = km;

                    if (rota.DistanciaKM > 0)
                    {
                        repRota.Inserir(rota);
                    }
                    else
                    {
                        rota = null;
                        Servicos.Log.TratarErro("A API não retornou a distancia entre a origem " + origem.Descricao + ", latitude: " + origem.Latitude.ToString() + ", longitude: " + origem.Longitude.ToString() + "  e o destino" + destino.Descricao + ", latitude: " + destino.Latitude.ToString() + ", longitude: " + destino.Longitude.ToString(), "Roteirizacao");
                    }
                }
                else
                {
                    rota.DistanciaKM = km;
                    repRota.Atualizar(rota);
                }

                return rota;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
        }

        public int BuscarDistanciaMapRequestApi(string origem, string destino)
        {
            string chaveDistancia = "AIzaSyAl-uQBMy3BU2j44IQpI9xkKDB0mdXZDD4";
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create("https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + origem + "&destinations=" + destino + "&key=" + chaveDistancia);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                dynamic dynRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());
                sr.Dispose();
                sr.Close();
                return (int)Math.Round((decimal)dynRetorno.rows[0].elements[0].distance.value / 1000, 0, MidpointRounding.AwayFromZero);
            }
            catch (Exception)
            {
                return 1;
            }

        }


        //public int BuscarDistanciaMapRequestApi(string origem, string destino)
        //{
        //    string chaveDistancia = "CKU8X2Ge11Efx0VvGZxpkLOk7nV6LEHa";
        //    System.Net.WebRequest req = System.Net.WebRequest.Create("http://open.mapquestapi.com/directions/v2/routematrix?key=" + chaveDistancia + "&from=" + origem + "&to=" + destino + "&unit=k");
        //    System.Net.WebResponse resp = req.GetResponse();
        //    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
        //    dynamic dynRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());
        //    sr.Dispose();
        //    sr.Close();
        //    return (int)Math.Round((decimal)dynRetorno.distance[1], 0, MidpointRounding.AwayFromZero);
        //}

        public Dominio.ObjetosDeValor.Embarcador.Logistica.RouteMapRequestAPI BuscarRotaMapRequestApi(string origem, string destino, bool buscarPassagensEstado)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create("http://open.mapquestapi.com/directions/v2/route?key=" + key + "&from=" + origem + "&to=" + destino + "&unit=k&routeType=fastest&timeType=1&enhancedNarrative=false&generalize=0&locale=en_US");
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            dynamic dynRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());
            sr.Dispose();
            sr.Close();

            Dominio.ObjetosDeValor.Embarcador.Logistica.RouteMapRequestAPI retorno = new Dominio.ObjetosDeValor.Embarcador.Logistica.RouteMapRequestAPI();
            retorno.valido = true;
            retorno.UFPassagens = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();
            retorno.distanciaKM = (int)Math.Round((decimal)dynRetorno.route.legs[0].distance, 0, MidpointRounding.AwayFromZero);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas> listaCordenadas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas>();
            for (int i = 0; i < dynRetorno.route.legs[0].maneuvers.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas cordenada = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas();
                cordenada.longitude = (string)dynRetorno.route.legs[0].maneuvers[i].startPoint.lng;
                cordenada.latitude = (string)dynRetorno.route.legs[0].maneuvers[i].startPoint.lat;
                listaCordenadas.Add(cordenada);
            }

            if (buscarPassagensEstado)
            {
                List<string> estados = new List<string>();
                var ultimoEstado = "";

                int interacoes = (int)Math.Ceiling((decimal)listaCordenadas.Count / 100);

                for (int interacao = 1; interacao <= interacoes; interacao++)
                {
                    string parametro = "";

                    int valorInicialInteracao = (100 * interacao);

                    int inicialI = valorInicialInteracao - 100;

                    for (int i = inicialI; i < valorInicialInteracao && i < listaCordenadas.Count; i++)
                    {
                        parametro += "&location=" + listaCordenadas[i].latitude + "," + listaCordenadas[i].longitude;
                    }

                    System.Net.WebRequest reqLocal = System.Net.WebRequest.Create("http://open.mapquestapi.com/geocoding/v1/batch?key=" + key + parametro);
                    System.Net.WebResponse respLocal = reqLocal.GetResponse();
                    System.IO.StreamReader srLocal = new System.IO.StreamReader(respLocal.GetResponseStream());
                    dynamic dynRetornoLocal = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(srLocal.ReadToEnd());
                    srLocal.Dispose();
                    srLocal.Close();


                    for (int i = 0; i < dynRetornoLocal.results.Count; i++)
                    {
                        string estado = ConverterUF((string)dynRetornoLocal.results[i].locations[0].adminArea3);
                        if (ultimoEstado != estado && !string.IsNullOrWhiteSpace(estado))
                        {
                            if (validarEstado(estados, estado))
                            {
                                ultimoEstado = estado;
                                estados.Add(estado);
                            }
                            else
                            {
                                retorno.valido = false;
                                retorno.mensagem = "Não foi possível encontrar percurso valido, por favor informe o percurso Manualmente.";
                            }
                        }
                    }
                }

                if (retorno.valido)
                {
                    retorno.UFOrigem = estados[0];
                    retorno.UFDestino = estados[estados.Count - 1];

                    for (int i = 1; i < estados.Count - 1; i++)
                    {
                        retorno.UFPassagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Posicao = i, Sigla = estados[i] });
                    }
                }
            }

            return retorno;
        }

        private string ConverterUF(string estado)
        {
            if (estado.Length > 2)
            {
                var estadoTratado = Utilidades.String.RemoveDiacritics(estado.Trim()).ToLower().Replace(" ", "");
                switch (estadoTratado)
                {
                    case "riograndedosul":
                        estado = "RS";
                        break;
                    case "santacatarina":
                        estado = "SC";
                        break;
                    case "parana":
                        estado = "PR";
                        break;
                    case "saopaulo":
                        estado = "SP";
                        break;
                    case "riodejaneiro":
                        estado = "RJ";
                        break;
                    case "espiritosanto":
                        estado = "RS";
                        break;
                    case "minasgerais":
                        estado = "MG";
                        break;
                    case "bahia":
                        estado = "BA";
                        break;
                    case "sergipe":
                        estado = "SE";
                        break;
                    case "alagoas":
                        estado = "AL";
                        break;
                    case "pernambuco":
                        estado = "PE";
                        break;
                    case "paraiba":
                        estado = "PB";
                        break;
                    case "riograndedonorte":
                        estado = "RN";
                        break;
                    case "ceara":
                        estado = "CE";
                        break;
                    case "piaui":
                        estado = "PI";
                        break;
                    case "maranhao":
                        estado = "MA";
                        break;
                    case "tocantins":
                        estado = "TO";
                        break;
                    case "goias":
                        estado = "GO";
                        break;
                    case "distritofederal":
                        estado = "DF";
                        break;
                    case "matogrossodosul":
                        estado = "MS";
                        break;
                    case "matogrosso":
                        estado = "MT";
                        break;
                    case "para":
                        estado = "PA";
                        break;
                    case "amapa":
                        estado = "AP";
                        break;
                    case "roraima":
                        estado = "RR";
                        break;
                    case "amazonas":
                        estado = "AM";
                        break;
                    case "rondonia":
                        estado = "RO";
                        break;
                    case "acre":
                        estado = "AC";
                        break;
                    default:
                        break;
                }
            }
            return estado;

        }

        private bool validarEstado(List<string> estados, string estado)
        {
            bool valido = true;
            List<string> divisas = BuscarDivisas(estado);
            if (divisas.Count > 0)
            {
                if (estados.Count > 0)
                {
                    string estadoAnterior = estados[estados.Count - 1];
                    if (!divisas.Contains(estadoAnterior))
                    {
                        string estadoFaltando = "";
                        List<string> divisasEstadoAnterior = BuscarDivisas(estadoAnterior);
                        foreach (string divisaAnterior in divisasEstadoAnterior)
                        {
                            foreach (string divisa in divisas)
                            {
                                if (divisa == divisaAnterior)
                                {
                                    estadoFaltando = divisaAnterior;
                                    break;
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(estadoFaltando))
                            estados.Add(estadoFaltando);
                        else
                            valido = false;
                    }
                }
            }
            else
            {
                valido = false;
            }

            return valido;
        }

        public List<string> BuscarDivisas(string estado)
        {
            List<string> divisas = new List<string>();
            switch (estado)
            {
                case "RS":
                    divisas.Add("SC");
                    break;
                case "SC":
                    divisas.Add("RS"); divisas.Add("PR");
                    break;
                case "PR":
                    divisas.Add("SC"); divisas.Add("MS"); divisas.Add("SP");
                    break;
                case "SP":
                    divisas.Add("PR"); divisas.Add("MS"); divisas.Add("MG"); divisas.Add("RJ");
                    break;
                case "RJ":
                    divisas.Add("SP"); divisas.Add("MG"); divisas.Add("MG"); divisas.Add("ES");
                    break;
                case "ES":
                    divisas.Add("RJ"); divisas.Add("MG"); divisas.Add("BA");
                    break;
                case "MG":
                    divisas.Add("SP"); divisas.Add("RJ"); divisas.Add("ES"); divisas.Add("BA"); divisas.Add("GO"); divisas.Add("DF"); divisas.Add("MS");
                    break;
                case "BA":
                    divisas.Add("ES"); divisas.Add("MG"); divisas.Add("GO"); divisas.Add("TO"); divisas.Add("PI"); divisas.Add("PE"); divisas.Add("AL"); divisas.Add("SE");
                    break;
                case "SE":
                    divisas.Add("BA"); divisas.Add("AL");
                    break;
                case "AL":
                    divisas.Add("BA"); divisas.Add("SE"); divisas.Add("PE");
                    break;
                case "PE":
                    divisas.Add("PI"); divisas.Add("CE"); divisas.Add("BA"); divisas.Add("PB"); divisas.Add("AL");
                    break;
                case "PB":
                    divisas.Add("CE"); divisas.Add("PE"); divisas.Add("RN");
                    break;
                case "RN":
                    divisas.Add("CE"); divisas.Add("PB");
                    break;
                case "CE":
                    divisas.Add("RN"); divisas.Add("PB"); divisas.Add("PE"); divisas.Add("PI");
                    break;
                case "PI":
                    divisas.Add("CE"); divisas.Add("PE"); divisas.Add("BA"); divisas.Add("TO"); divisas.Add("MA");
                    break;
                case "MA":
                    divisas.Add("PI"); divisas.Add("TO"); divisas.Add("PA");
                    break;
                case "TO":
                    divisas.Add("MA"); divisas.Add("PI"); divisas.Add("BA"); divisas.Add("GO"); divisas.Add("BA"); divisas.Add("MT"); divisas.Add("PA");
                    break;
                case "GO":
                    divisas.Add("TO"); divisas.Add("BA"); divisas.Add("MG"); divisas.Add("DF"); divisas.Add("MS"); divisas.Add("MT");
                    break;
                case "DF":
                    divisas.Add("GO"); divisas.Add("MG");
                    break;
                case "MS":
                    divisas.Add("PR"); divisas.Add("SP"); divisas.Add("MG"); divisas.Add("GO"); divisas.Add("MT");
                    break;
                case "MT":
                    divisas.Add("MS"); divisas.Add("GO"); divisas.Add("TO"); divisas.Add("PA"); divisas.Add("AM"); divisas.Add("RO");
                    break;
                case "PA":
                    divisas.Add("MA"); divisas.Add("TO"); divisas.Add("MT"); divisas.Add("AM"); divisas.Add("RR"); divisas.Add("AP");
                    break;
                case "AP":
                    divisas.Add("PA");
                    break;
                case "RR":
                    divisas.Add("PA"); divisas.Add("AM");
                    break;
                case "AM":
                    divisas.Add("RR"); divisas.Add("PA"); divisas.Add("MT"); divisas.Add("RO"); divisas.Add("AC");
                    break;
                case "RO":
                    divisas.Add("AM"); divisas.Add("AC"); divisas.Add("MT");
                    break;
                case "AC":
                    divisas.Add("AM"); divisas.Add("RO");
                    break;
                default:
                    break;
            }
            return divisas;
        }


    }
}

