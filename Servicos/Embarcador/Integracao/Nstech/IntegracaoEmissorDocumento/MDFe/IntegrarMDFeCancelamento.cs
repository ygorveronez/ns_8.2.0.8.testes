using Servicos.Extensions;
using System;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool CancelarMdfe(int codigoMDFe, int codigoEmpresa, string justificativa, Repositorio.UnitOfWork unitOfWork = null, string stringConexao = null, DateTime? dataCancelamento = null, string cobrarCancelamento = "")
        {
            bool sucesso = false;
            string id = string.Empty;
            string mensagemErro = string.Empty;

            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(stringConexao);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null;

            try
            {
                bool sincronizarDocumento = false;
                mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento envioWS = this.obterMdfeCancelamento(mdfe, justificativa, dataCancelamento);

                //Transmite
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.POST, envioWS, "mdfe-v3/events", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPIMDFe);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Emissor NSTech: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    Servicos.Log.TratarErro(mensagemErro);
                    sucesso = false;

                    try
                    {
                        if (retornoWS.StatusCode == 400 && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                        {
                            dynamic objetoRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retornoWS.jsonRetorno);
                            if (objetoRetorno?.type == "event_is_authorized")
                            {
                                sucesso = true;
                                sincronizarDocumento = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        mensagemErro = "Emissor NSTech: Ocorreu uma falha ao processar o retorno do envio do cancelamento do mdfe";
                        Servicos.Log.TratarErro(mensagemErro);
                        Servicos.Log.TratarErro(ex);
                    }
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar retorno JSON MDFe cancelamento Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao solicitar o cancelamento do mdfe; RetornoWS {0}.", retornoWS.jsonRetorno);
                        Servicos.Log.TratarErro(mensagemErro);
                        sucesso = false;
                    }
                    else
                    {
                        id = (string)retorno.id;
                        sucesso = true;
                    }
                }

                if (sucesso)
                {
                    mdfe.CodigoIntegradorCancelamento = 0;
                    mdfe.MensagemRetornoSefaz = "Evento em processamento.";
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmCancelamento;
                    mdfe.CobrarCancelamento = cobrarCancelamento == "Sim";
                    repMDFe.Atualizar(mdfe);

                    if (sincronizarDocumento)
                        this.ConsultarMdfe(mdfe, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unitOfWork);
                }
                else
                {
                    mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(mensagemErro);
                    mdfe.CobrarCancelamento = cobrarCancelamento == "Sim";
                    repMDFe.Atualizar(mdfe);

                    Servicos.Log.TratarErro(mensagemErro);
                }
            }
            catch (Exception ex)
            {
                mensagemErro = "Emissor NSTech: Ocorreu uma falha ao efetuar o cancelamento do mdfe";
                Servicos.Log.TratarErro(mensagemErro);
                Servicos.Log.TratarErro(ex);

                if (mdfe != null)
                {
                    mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(mensagemErro);
                    mdfe.CobrarCancelamento = cobrarCancelamento == "Sim";
                    repMDFe.Atualizar(mdfe);
                }

                sucesso = false;
            }

            return sucesso;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento obterMdfeCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, string justificativa, DateTime? dataCancelamento)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento();

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
            DateTime dtCancelamento = dataCancelamento.HasValue ? dataCancelamento.Value : DateTime.Now;
            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(dtCancelamento);
            string fusoHorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);

            // Tratamento para remover caracteres especiais
            Regex rgx = new Regex(Servicos.CTe.ReplaceMotivoRegexPatternBack);
            justificativa = rgx.Replace(justificativa, "");

            retorno.data = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoData();
            retorno.data.externalId = $"{mdfe.Codigo}_cancel";
            retorno.data.eventDate = dtCancelamento.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "T") + fusoHorario;
            retorno.data.mdfeKey = mdfe.Chave;
            retorno.data.eventSequence = 1;

            retorno.data.issuer = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuer();
            retorno.data.issuer.type = mdfe.Empresa.Tipo == "F" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
            retorno.data.issuer.document = Utilidades.String.OnlyNumbers(mdfe.Empresa.CNPJ);
            retorno.data.issuer.state = mdfe.Empresa.Localidade.Estado.Sigla;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuerCancel eventoCancelamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuerCancel();
            eventoCancelamento.protocolNumber = mdfe.Protocolo;
            eventoCancelamento.reason = justificativa;
            retorno.data.evento = eventoCancelamento;

            retorno.options = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptions();
            retorno.options.removeSpecialsChars = true;

            return retorno;
        }

        #endregion Métodos Privados
    }
}
