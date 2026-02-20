using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Métodos Globais

        public bool IntegrarRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.RotaFrete rota, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out int? roteiroID, bool compraValePedagio = false)
        {
            //Descontinuado conforme solicitado pelo cliente
            //return IntegrarRotaCalcRouteV2(carga, rota, unitOfWork, out mensagemErro, out roteiroID);

            return IntegrarRotaCalcRoutePolyline(carga, rota, unitOfWork, out mensagemErro, out roteiroID, compraValePedagio);
        }

        public bool IntegrarRotaCalcRoutePolyline(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.RotaFrete rota, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out int? roteiroID, bool compraValePedagio)
        {
            mensagemErro = null;
            roteiroID = null;
            bool sucesso = false;

            try
            {
                this.ObterToken(out mensagemErro);
                if (string.IsNullOrWhiteSpace(this.token))
                    return false;

                if (!string.IsNullOrEmpty(rota?.CodigoIntegracaoCIOT))
                {
                    roteiroID = int.Parse(rota.CodigoIntegracaoCIOT);
                    return true;
                }

                Dominio.Entidades.Localidade origem = null;
                Dominio.Entidades.Localidade destino = null;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();

                if (rota != null)
                {
                    if (rota.Remetente != null)
                        origem = rota.Remetente.Localidade;
                    else if (rota.LocalidadesOrigem?.Count > 0)
                        origem = rota.LocalidadesOrigem.FirstOrDefault();

                    if (rota.Destinatarios?.Count > 0)
                        destino = rota.Destinatarios.First().Cliente.Localidade;
                    else if (rota.Localidades?.Count > 0)
                        destino = rota.Localidades.OrderByDescending(x => x.Ordem).ThenByDescending(o => o.Codigo).FirstOrDefault()?.Localidade;
                }
                else
                {
                    origem = cargaPedido.Origem;
                    destino = cargaPedido.Destino;
                }

                if (origem == null)
                {
                    mensagemErro = "Não foi encontrada uma origem na rota da carga.";
                    return false;
                }

                if (destino == null)
                {
                    mensagemErro = "Não foi encontrado um destino na rota da carga.";
                    return false;
                }

                #region Incluir Rota

                string jsonRequisicao = "";
                string jsonRetorno = "";
                string urlRoteirizador = $"{this.urlWebService}mso-cargo-roteirizador/routering/polyline/calcRoute";

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envRotaPolyline enviarRotaPolyline = this.ObterObjRotaPolyline(carga, rota, origem, destino, compraValePedagio, unitOfWork);

                HttpClient requisicao = CriarRequisicao(urlRoteirizador);
                jsonRequisicao = JsonConvert.SerializeObject(enviarRotaPolyline, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = null;

                retornoRequisicao = requisicao.PostAsync(urlRoteirizador, conteudoRequisicao).Result;

                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retRouteData retIncluirRota = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retRouteData>(jsonRetorno);

                if (!retornoRequisicao.IsSuccessStatusCode)
                {
                    string mensagemRetorno = string.Empty;
                    if (string.IsNullOrEmpty(jsonRequisicao))
                        mensagemRetorno = $"Ocorreu uma falha ao efetuar o cadastro da rota: {retornoRequisicao.StatusCode}";
                    else
                        mensagemRetorno = $"Ocorreu uma falha ao efetuar o cadastro da rota: {jsonRequisicao} - {retIncluirRota?.Error ?? ""}";

                    throw new ServicoException(mensagemRetorno);
                }

                roteiroID = Convert.ToInt32(retIncluirRota.Data?.GoingTrip?.RouterTripId ?? null);

                if (rota != null && roteiroID != null)
                {
                    Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                    rota.CodigoIntegracaoCIOT = roteiroID.ToString();
                    repRotaFrete.Atualizar(rota);
                }

                #endregion

                sucesso = true;
            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;
                sucesso = false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar o cadastro da rota.";
                sucesso = false;
            }

            return sucesso;
        }

        public bool IntegrarRotaCalcRouteV2(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.RotaFrete rota, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out int? roteiroID)
        {
            mensagemErro = null;
            roteiroID = null;
            bool sucesso = false;

            try
            {
                this.ObterToken(out mensagemErro);
                if (string.IsNullOrWhiteSpace(this.token))
                    return false;

                if (!string.IsNullOrEmpty(rota?.CodigoIntegracaoCIOT))
                {
                    roteiroID = int.Parse(rota.CodigoIntegracaoCIOT);
                    return true;
                }

                Dominio.Entidades.Localidade origem = null;
                Dominio.Entidades.Localidade destino = null;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();

                if (rota != null)
                {
                    if (rota.Remetente != null)
                        origem = rota.Remetente.Localidade;
                    else if (rota.LocalidadesOrigem?.Count > 0)
                        origem = rota.LocalidadesOrigem.FirstOrDefault();

                    if (rota.Destinatarios?.Count > 0)
                        destino = rota.Destinatarios.First().Cliente.Localidade;
                    else if (rota.Localidades?.Count > 0)
                        destino = rota.Localidades.OrderByDescending(x => x.Ordem).ThenByDescending(o => o.Codigo).FirstOrDefault()?.Localidade;
                }
                else
                {
                    origem = cargaPedido.Origem;
                    destino = cargaPedido.Destino;
                }

                string cepOrigem = null;
                string cepDestino = null;

                if (rota != null && rota.Remetente != null)
                    cepOrigem = rota.Remetente.CEP?.Trim();
                else
                    cepOrigem = cargaPedido.ClienteColeta.CEP?.Trim();

                if (rota != null && rota.Destinatarios?.Count > 0)
                    cepDestino = rota.Destinatarios.First().Cliente.CEP?.Trim();
                else
                    cepDestino = cargaPedido.ClienteEntrega.CEP?.Trim();

                if (origem == null)
                {
                    mensagemErro = "Não foi encontrada uma origem na rota da carga.";
                    return false;
                }

                if (destino == null)
                {
                    mensagemErro = "Não foi encontrado um destino na rota da carga.";
                    return false;
                }

                #region Calcular Rota IDA

                string jsonRequisicao = "";
                string jsonRetorno = "";
                string urlCalcularRota = $"{this.urlWebService}mso-cargo-roteirizador/routering/calcRouteV2";
                int? goingTripId = null;

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envCalcularRota enviarCalcularRota = this.ObterObjCalcularRota(carga, rota, origem, destino);

                HttpClient requisicao = CriarRequisicao(urlCalcularRota);
                jsonRequisicao = JsonConvert.SerializeObject(enviarCalcularRota, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = null;

                retornoRequisicao = requisicao.PostAsync(urlCalcularRota, conteudoRequisicao).Result;

                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicao.IsSuccessStatusCode)
                {
                    string mensagemRetorno = string.Empty;
                    if (string.IsNullOrEmpty(jsonRetorno))
                        mensagemRetorno = $"Ocorreu uma falha ao efetuar o calculo da rota: {retornoRequisicao.StatusCode}";
                    else
                        mensagemRetorno = $"Ocorreu uma falha ao efetuar o calculo da rota: {jsonRetorno}";

                    throw new ServicoException(mensagemRetorno);
                }

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retCalcularRota retCalcularRota = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retCalcularRota>(jsonRetorno);
                
                if (retCalcularRota?.status != 200)
                    throw new ServicoException($"Ocorreu uma falha ao efetuar o calculo da rota ida status: {retCalcularRota?.status ?? 0}");

                goingTripId = retCalcularRota?.data?.fastest?.tripId;

                #endregion

                #region Calcular Rota Returno

                int? returnTripId = null;

                #endregion

                #region Incluir Rota

                string jsonRequisicaoIncluirRota = "";
                string jsonRetornoIncluirRota = "";
                string urlIncluirRota = $"{this.urlWebService}mso-cargo-roteirizador/routering/routeShipper";

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envRota enviarRota = this.ObterObjRota(carga, rota, origem, destino, cepOrigem, cepDestino, goingTripId, returnTripId);

                HttpClient requisicaoIncluirRota = CriarRequisicao(urlIncluirRota);
                jsonRequisicaoIncluirRota = JsonConvert.SerializeObject(enviarRota, Formatting.Indented);
                StringContent conteudoRequisicaoIncluirRota = new StringContent(jsonRequisicaoIncluirRota.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicaoIncluirRota = null;

                retornoRequisicaoIncluirRota = requisicaoIncluirRota.PostAsync(urlIncluirRota, conteudoRequisicaoIncluirRota).Result;

                jsonRetornoIncluirRota = retornoRequisicaoIncluirRota.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicaoIncluirRota.IsSuccessStatusCode)
                {
                    string mensagemRetorno = string.Empty;
                    if (string.IsNullOrEmpty(jsonRetornoIncluirRota))
                        mensagemRetorno = $"Ocorreu uma falha ao efetuar o cadastro da rota: {retornoRequisicaoIncluirRota.StatusCode}";
                    else
                        mensagemRetorno = $"Ocorreu uma falha ao efetuar o cadastro da rota: {jsonRetornoIncluirRota}";

                    throw new ServicoException(mensagemRetorno);
                }

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retRota retIncluirRota = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retRota>(jsonRetornoIncluirRota);
                roteiroID = retIncluirRota.data.ID;

                if (rota != null && roteiroID != null)
                {
                    Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                    rota.CodigoIntegracaoCIOT = roteiroID.ToString();
                    repRotaFrete.Atualizar(rota);
                }

                #endregion

                sucesso = true;
            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;
                sucesso = false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar o cadastro da rota.";
                sucesso = false;
            }

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envCalcularRota ObterObjCalcularRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.RotaFrete rota, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envCalcularRota retorno = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envCalcularRota();

            retorno.axis = carga.ModeloVeicularCarga?.NumeroEixos ?? 0;
            retorno.points = new List<envCalcularRotaPoint>();


            envCalcularRotaPoint pontoOrigem = new envCalcularRotaPoint();
            pontoOrigem.siteId = origem.DescricaoCidadeEstado;
            if ((origem?.Latitude ?? 0) != 0 && (origem?.Longitude ?? 0) != 0)
            {
                pontoOrigem.latitude = origem.Latitude ?? 0;
                pontoOrigem.longitude = origem.Longitude ?? 0;
            }
            else
            {
                pontoOrigem.cep = origem.CEP; 
            }
            retorno.points.Add(pontoOrigem);

            envCalcularRotaPoint pontoDestino = new envCalcularRotaPoint();
            pontoDestino.siteId = destino.DescricaoCidadeEstado;
            if ((destino?.Latitude ?? 0) != 0 && (destino?.Longitude ?? 0) != 0)
            {
                pontoDestino.latitude = destino.Latitude ?? 0;
                pontoDestino.longitude = destino.Longitude ?? 0;
            }
            else
            {
                pontoDestino.cep = destino.CEP;
            }
            retorno.points.Add(pontoDestino);

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envRota ObterObjRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.RotaFrete rota, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, string cepOrigem, string cepDestino, int? goingTripId, int? returnTripId)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envRota retorno = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envRota();

            retorno.GoingTripId = goingTripId;
            retorno.ReturnTripId = returnTripId;
            retorno.Active = true;

            if (rota != null)
            {
                retorno.TripNickName = rota.Descricao;
                retorno.HaveReturn = rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.IdaVolta ? true : false;
                retorno.Stops = null;
                retorno.ReturnStops = null;
            }
            else
            {
                retorno.TripNickName = $"{origem.Descricao} x {destino.Descricao}";
                retorno.HaveReturn = false;
                retorno.Stops = null;
                retorno.ReturnStops = null;
            }

            retorno.GoingTrip = this.ObterObjRotaGoingTrip(origem, destino, cepOrigem, cepDestino);
            retorno.ReturnTrip = this.ObterObjRotaReturnTrip(retorno.HaveReturn, destino, origem);

            return retorno;
        }

        private GoingTrip ObterObjRotaGoingTrip(Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, string cepOrigem, string cepDestino)
        {
            GoingTrip retorno = new GoingTrip();

            retorno.OriginDescription = origem.Descricao;
            retorno.OriginCep = cepOrigem != "0" ? cepOrigem : null;
            retorno.ArrivalDescription = destino.Descricao;
            retorno.ArrivalCep = cepDestino != "0" ? cepDestino : null;

            return retorno;
        }

        private ReturnTrip ObterObjRotaReturnTrip(bool idaVolta, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino)
        {
            ReturnTrip retorno = null;

            if (idaVolta)
            {
                retorno = new ReturnTrip();
                retorno.OriginDescription = origem.Descricao;
                retorno.OriginCep = origem.CEP;
                retorno.ArrivalDescription = origem.Descricao;
                retorno.ArrivalCep = origem.CEP;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envRotaPolyline ObterObjRotaPolyline(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.RotaFrete rota, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, bool compraValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envRotaPolyline retorno = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envRotaPolyline();

            retorno.shipperId = 7;

            if (rota != null)
                retorno.tripNickName = rota.Descricao;
            else
                retorno.tripNickName = $"{origem.Descricao} x {destino.Descricao} - {DateTime.Now.ToString("ddMMyyyyHHmmss")}";

            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            if (cargaRotaFrete != null && cargaRotaFrete.PolilinhaRota != null && compraValePedagio)
                retorno.polyline = cargaRotaFrete.PolilinhaRota.Replace(@"\", @"\\");
            else
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPoints = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();

                wayPoints.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint((double)(origem?.Latitude ?? 0), (double)(origem?.Longitude ?? 0)));
                wayPoints.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint((double)(destino?.Latitude ?? 0), (double)(destino?.Longitude ?? 0)));

                retorno.polyline = Servicos.Embarcador.Logistica.Polilinha.Codificar(wayPoints).Replace(@"\", @"\\");
            }

            return retorno;
        }

        #endregion
    }
}