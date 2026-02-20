using Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool IncluirAtualizarInscricaoWebhook(out string mensagemErro, out string subscribeID)
        {
            mensagemErro = string.Empty;
            subscribeID = string.Empty;
            bool sucesso = false;

            try
            {
                if (!this.ConsultarInscricaoWebhook(out mensagemErro, out subscribeID))
                    return false;

                enumTipoWS tipoMetodo = string.IsNullOrEmpty(subscribeID) ? enumTipoWS.POST : enumTipoWS.PUT;
                string metodo = string.IsNullOrEmpty(subscribeID) ? @"subscribe" : $@"subscribe?by=externalId&externalId={_configuracaoIntegracaoEmissorDocumento.NSTechExternalId}";

                webhook envioWS = this.ObterIncluirAtualizarWebhook();

                //Transmite
                var retornoWS = this.TransmitirEmissor(tipoMetodo, envioWS, metodo, _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPIWebHook);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta do envio de webhook Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar ao enviar os dados do webhook; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        if (tipoMetodo == enumTipoWS.POST)
                            subscribeID = (string)retorno.subscribeId;
                        sucesso = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu um erro a enviar/atualizar a inscrição do Webhook.";
                sucesso = false;
            }

            return sucesso;
        }

        public bool RemoverInscricaoWebhook(out string mensagemErro)
        {
            mensagemErro = string.Empty;
            string subscribeID = string.Empty;
            bool sucesso = false;

            try
            {
                if (!this.ConsultarInscricaoWebhook(out mensagemErro, out subscribeID))
                    return false;

                if (string.IsNullOrEmpty(subscribeID))
                    return true;

                //Transmite
                var retornoWS = this.TransmitirEmissor(enumTipoWS.DELETE, null, $@"subscribe?by=externalId&externalId={_configuracaoIntegracaoEmissorDocumento.NSTechExternalId}", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPIWebHook);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    sucesso = false;
                }
                else
                    sucesso = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu um erro ao remover a inscrição do Webhook.";
                sucesso = false;
            }

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private bool ConsultarInscricaoWebhook(out string mensagemErro, out string subscribeID)
        {
            mensagemErro = string.Empty;
            subscribeID = string.Empty;
            bool sucesso = false;

            try
            {
                object envioWS = null;

                //Transmitir
                var retornoWS = this.TransmitirEmissor(enumTipoWS.GET, envioWS, $"subscribe?by=externalId&externalId={_configuracaoIntegracaoEmissorDocumento.NSTechExternalId}", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPIWebHook);

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    sucesso = false;
                }
                else
                {
                    if (retornoWS.erro && retornoWS.StatusCode == 404)
                    {
                        sucesso = true;
                    }
                    else if (retornoWS.erro)
                    {
                        mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                        sucesso = false;
                    }
                    else
                    {
                        List<webhook> retorno = null;

                        try
                        {
                            retorno = retornoWS.jsonRetorno.FromJson<List<webhook>>();
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar consulta de webhook Nstech: {ex.ToString()}", "CatchNoAction");
                        }

                        if (retorno == null)
                        {
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar a consulta do webhook; RetornoWS {0}.", retornoWS.jsonRetorno);
                            sucesso = false;
                        }
                        else
                        {
                            webhook retSubscribe = retorno.FirstOrDefault();
                            if (retSubscribe != null)
                                subscribeID = retSubscribe.id;
                            sucesso = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao efetuar a consulta do webhook.";
                sucesso = false;
            }

            return sucesso;
        }

        private webhook ObterIncluirAtualizarWebhook()
        {
            webhook retorno = new webhook();

            retorno.externalId = _configuracaoIntegracaoEmissorDocumento.NSTechExternalId;
            retorno.status = "enable";
            retorno.events = new List<string>()
            {
                "com.nstech.issuance-engine.cte-authorized",
                "com.nstech.issuance-engine.cte-error",
                "com.nstech.issuance-engine.cte-rejected",
                "com.nstech.issuance-engine.cte-event-authorized",
                "com.nstech.issuance-engine.cte-event-error",
                "com.nstech.issuance-engine.cte-event-rejected",
                "com.nstech.issuance-engine.cert-expiring",
                "com.nstech.issuance-engine.cte-dacte-generated",
                "com.nstech.issuance-engine.mdfe-authorized",
                "com.nstech.issuance-engine.mdfe-rejected",
                "com.nstech.issuance-engine.mdfe-error",
                "com.nstech.issuance-engine.mdfe-event-authorized",
                "com.nstech.issuance-engine.mdfe-event-error",
                "com.nstech.issuance-engine.mdfe-event-rejected",
                "com.nstech.issuance-engine.mdfe-damdfe-generated",
                "com.nstech.issuance-engine.mdfe-damdfe-error"
            };

            retorno.endpoint = new webhook.webhookEndpoint();
            retorno.endpoint.url = _configuracaoIntegracaoEmissorDocumento.NSTechUrlWebhook;
            retorno.endpoint.headers = new List<List<string>>() { new List<string>() { "token", _configuracaoIntegracaoEmissorDocumento.NSTechIntegradora.Token } };

            return retorno;
        }

        #endregion
    }
}
