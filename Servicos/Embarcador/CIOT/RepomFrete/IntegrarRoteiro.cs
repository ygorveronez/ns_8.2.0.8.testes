using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        #endregion

        #region Métodos Privados

        private bool ConsultarRoteiro(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.RotaFrete rota, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out string codigoRoteiro, out string codigoPercurso, string branchCode, out string jsonEnvio, out string jsonRetorno)
        {

            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;

            codigoPercurso = null;
            codigoRoteiro = null;
            mensagemErro = null;

            if (rota == null)
            {
                mensagemErro = "A rota não foi informada.";
                return false;
            }

            Dominio.Entidades.Localidade origem = null;
            Dominio.Entidades.Localidade destino = null;

            if (rota.Remetente != null)
                origem = rota.Remetente.Localidade;
            else if (rota.LocalidadesOrigem?.Count > 0)
                origem = rota.LocalidadesOrigem.FirstOrDefault();

            if (rota.Destinatarios?.Count > 0)
                destino = rota.Destinatarios.First().Cliente.Localidade;
            else if (rota.Localidades?.Count > 0)
                destino = rota.Localidades.OrderByDescending(x => x.Ordem).ThenByDescending(o => o.Codigo).FirstOrDefault()?.Localidade;

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

            string cEnvio = $"{rota.Codigo.ToString()}/0";

            //Transmite o arquivo
            var retornoWS = this.TransmitirRepom(enumTipoWS.GET, cEnvio, @"Route/ByTraceIdentifier", this.tokenAutenticacao);

            if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
            {
                mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                return false;
            }
            else
            {
                retRouteByTraceIdentifier retorno = null;

                try
                {
                    retorno = retornoWS.jsonRetorno.FromJson<retRouteByTraceIdentifier>();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar consulta de rota RepomFrete: {ex.ToString()}", "CatchNoAction");
                }

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar a consulta da rota; RetornoWS {0}.", retornoWS.jsonRetorno);
                    return false;
                }
                else
                {
                    if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                    {
                        codigoRoteiro = retorno?.Result.RouteCode.ToString();
                        codigoPercurso = retorno?.Result.TraceCode.ToString();

                        return true;
                    }
                    else if (retorno?.Response?.StatusCode == 404)
                    {
                        if (CadastrarRoteiro(carga, rota, origem, destino, unitOfWork, out mensagemErro, out codigoRoteiro, out codigoPercurso, branchCode, out jsonEnvio, out jsonRetorno))
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        string mensagemRetorno = "Erro:";

                        if (retorno.Errors != null && retorno.Errors.Count() > 0)
                            retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                        else
                            mensagemRetorno += " Ocorreu uma falha ao efetuar a consulta da rota.";

                        mensagemErro = mensagemRetorno;
                        return false;
                    }
                }
            }
        }

        private bool CadastrarRoteiro(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.RotaFrete rota, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out string codigoRoteiro, out string codigoPercurso, string branchCode, out string jsonEnvio, out string jsonRetorno)
        {
            mensagemErro = null;
            codigoRoteiro = null;
            codigoPercurso = null;

            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;

            try
            {
                var envioWS = ObterCadastroRoteiro(carga, rota, origem, destino, branchCode);

                //Transmite o arquivo
                var retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, @"RouteRequest\RouteRequestAutomatic", this.tokenAutenticacao);

                jsonEnvio = retornoWS.jsonEnvio;
                jsonRetorno = retornoWS.jsonRetorno;

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    return false;
                }
                else
                {
                    retRouteRequestAutomatic retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retRouteRequestAutomatic>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar cadastro de roteiro automático RepomFrete: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do roteiro; RetornoWS {0}.", retornoWS.jsonRetorno);
                        return false;
                    }
                    else
                    {
                        if (retorno.Data != null && !string.IsNullOrEmpty(retorno.Data.RouteCode))
                        {
                            codigoRoteiro = retorno.Data?.RouteCode.ToString();
                            codigoPercurso = retorno.Data?.TraceCodeRepom.ToString();

                            return true;
                        }
                        else
                        {
                            string mensagemRetorno = "Falha ao efetuar o cadastro do roteiro:";

                            if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                            else
                                mensagemRetorno += " Ocorreu uma falha ao efetuar o cadastro do roteiro.";

                            mensagemErro = mensagemRetorno;
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do roteiro do CIOT.";
                return false;
            }
        }

        private envRouteRequestAutomatic ObterCadastroRoteiro(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.RotaFrete rota, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, string branchCode)
        {
            envRouteRequestAutomatic retorno = new envRouteRequestAutomatic();

            int codigoIBGEOrigem = origem.CodigoIBGE;
            int codigoIBGEDestino = this.ObterUltimaRotaDestino(rota, destino);

            retorno.BranchCode = branchCode;
            retorno.OriginIBGECode = codigoIBGEOrigem.ToString();
            retorno.DestinyIBGECode = codigoIBGEDestino.ToString();
            retorno.RoundTrip = rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.IdaVolta ? true : false;
            retorno.TraceIdentifier = rota.Codigo.ToString();
            retorno.RouteStopIBGE = this.RouteRequestAutomaticRouteStopIBGE(rota, codigoIBGEOrigem, codigoIBGEDestino);

            return retorno;
        }

        private List<string> RouteRequestAutomaticRouteStopIBGE(Dominio.Entidades.RotaFrete rota, int codigoIBGEOrigem, int codigoIBGEDestino)
        {
            List<string> retorno = null;

            foreach (var pontoPassagem in rota.Localidades?.OrderBy(o => o.Ordem).ThenBy(o => o.Codigo).Select(o => o.Localidade).ToList())
            {
                if (pontoPassagem.CodigoIBGE != 0 && pontoPassagem.CodigoIBGE != codigoIBGEOrigem && pontoPassagem.CodigoIBGE != codigoIBGEDestino)
                {
                    if (retorno == null)
                        retorno = new List<string>();

                    retorno.Add(pontoPassagem.CodigoIBGE.ToString());
                }
            }

            return retorno;
        }

        private int ObterUltimaRotaDestino(Dominio.Entidades.RotaFrete rota, Dominio.Entidades.Localidade destino)
        {
            if (destino.Estado?.Sigla != "EX")
                return destino.CodigoIBGE;


            foreach (var pontoPassagem in rota.Localidades?.OrderByDescending(o => o.Ordem).ThenBy(o => o.Codigo).Select(o => o.Localidade).ToList())
            {
                if (pontoPassagem.Estado?.Sigla != "EX")
                {
                    return destino.CodigoIBGE;
                }
            }

            return destino.CodigoIBGE;
        }
        #endregion
    }
}
