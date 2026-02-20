using Dominio.Enumeradores;
using Newtonsoft.Json.Linq;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool ConsultarMdfe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            bool sucesso = false;
            string mensagemErro = string.Empty;

            try
            {
                object envioWS = null;

                //Transmitir
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.GET, envioWS, $"mdfe-v3?by=externalId&externalId={mdfe.Codigo}", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPIMDFe);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Emissor NSTech: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    sucesso = false;
                }
                else
                {
                    dynamic retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<dynamic>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar consulta MDFe Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao efetuar a consulta do mdfe; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        var RetornoConsulta = retorno[0];
                        sucesso = true;

                        if (mdfe.Status == StatusMDFe.Enviado)
                        {
                            if (!ProcessarMDFe(out mensagemErro, "com.nstech.issuance-engine.mdfe", mdfe, RetornoConsulta, null, Auditado, tipoServicoMultisoftware, unitOfWork))
                                sucesso = false;
                        }
                        else if (mdfe.Status == StatusMDFe.EmCancelamento)
                        {
                            if (RetornoConsulta?.events != null)
                            {
                                // Filtrando eventos com eventName "cancel"
                                var eventoCancelado = ((JArray)RetornoConsulta["events"])
                                    .FirstOrDefault(e => (string)e["eventName"] == "cancel");

                                if (eventoCancelado != null)
                                {
                                    if (!ProcessarEventoMDFe(out mensagemErro, "cancel", mdfe, eventoCancelado, null, Auditado, tipoServicoMultisoftware, unitOfWork))
                                        sucesso = false;
                                }
                            }
                        }
                        else if (mdfe.Status == StatusMDFe.EmEncerramento)
                        {
                            if (RetornoConsulta?.events != null)
                            {
                                // Filtrando eventos com eventName "close"
                                var eventoEncerramento = ((JArray)RetornoConsulta["events"])
                                    .FirstOrDefault(e => (string)e["eventName"] == "close");

                                if (eventoEncerramento != null)
                                {
                                    if (!ProcessarEventoMDFe(out mensagemErro, "close", mdfe, eventoEncerramento, null, Auditado, tipoServicoMultisoftware, unitOfWork))
                                        sucesso = false;
                                }
                            }
                        }
                        else if (mdfe.Status == StatusMDFe.EventoInclusaoMotoristaEnviado)
                        {
                            if (RetornoConsulta?.events != null)
                            {
                                // Filtrando eventos com eventName "close"
                                var eventoInclusaoMotorista = ((JArray)RetornoConsulta["events"])
                                    .FirstOrDefault(e => (string)e["eventName"] == "include_driver");

                                if (eventoInclusaoMotorista != null)
                                {
                                    if (!ProcessarEventoMDFe(out mensagemErro, "include_driver", mdfe, eventoInclusaoMotorista, null, Auditado, tipoServicoMultisoftware, unitOfWork))
                                        sucesso = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Exception Sincronizar Documento");
                Servicos.Log.TratarErro(ex);

                throw;
            }

            return sucesso;
        }

        public Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave ConsultarMdfeEmissorExterno(string chave)
        {
            try
            {
                object envioWS = null;
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.GET, envioWS, $"mdfe-v3/status?key={chave}", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPIMDFe);

                if (retornoWS.erro || string.IsNullOrWhiteSpace(retornoWS.jsonRetorno))
                    return null;

                JObject retorno = null;

                try
                {
                    retorno = retornoWS.jsonRetorno.FromJson<JObject>();

                    if (retorno == null)
                    {
                        string jsonInterno = retornoWS.jsonRetorno.FromJson<string>();
                        if (!string.IsNullOrWhiteSpace(jsonInterno))
                            retorno = jsonInterno.FromJson<JObject>();
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar status MDFe externo Nstech: {ex}", "CatchNoAction");
                }

                if (retorno == null)
                    return null;

                return new Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave
                {
                    Ambiente = retorno["environment"]?.Value<int?>() ?? retorno["Ambiente"]?.Value<int?>() ?? 0,
                    CodigoStatus = retorno["statusCode"]?.ToString() ?? retorno["CodigoStatus"]?.ToString() ?? string.Empty,
                    MensagemStatus = retorno["statusMessage"]?.ToString() ?? retorno["MensagemStatus"]?.ToString() ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
        }

        public byte[] ObterXMLMdfe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork, bool retornarXMLCancelamento = false)
        {

            string mensagemErro = string.Empty;

            try
            {
                object envioWS = null;

                //Transmitir
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.GET, envioWS, $"mdfe-v3?by=externalId&externalId={mdfe.Codigo}", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPIMDFe);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Emissor NSTech: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    throw new Exception(mensagemErro);

                }
                else
                {
                    dynamic retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<dynamic>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar segunda consulta MDFe Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao efetuar a consulta do mdfe; RetornoWS {0}.", retornoWS.jsonRetorno);
                        throw new Exception(mensagemErro);
                    }
                    else
                    {
                        var RetornoConsulta = retorno[0];


                        if (mdfe.Status == StatusMDFe.Autorizado || mdfe.Status == StatusMDFe.Cancelado)
                        {
                            string retornoXml = ProcessarXMLMDFe(out mensagemErro, "com.nstech.issuance-engine.mdfe", mdfe, codigoEmpresa, RetornoConsulta, null, unitOfWork);

                            if (!string.IsNullOrWhiteSpace(retornoXml))
                            {
                                byte[] data = System.Text.Encoding.Default.GetBytes(retornoXml);
                                return data;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Exception Baixar XML");
                Servicos.Log.TratarErro(ex);

                throw;
            }

            return null;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        #endregion Métodos Privados
    }
}
