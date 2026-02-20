using System;
using System.Text.RegularExpressions;
using Servicos.Extensions;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool CancelarCte(int codigoCTe, int codigoEmpresa, string justificativa, Repositorio.UnitOfWork unitOfWork = null, string stringConexao = null, DateTime? dataCancelamento = null, bool gerarLog = false, Dominio.Entidades.Usuario usuario = null, string cobrarCancelamento = "")
        {
            bool sucesso = false;
            string id = string.Empty;

            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(stringConexao);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            try
            {
                bool sincronizarDocumento = false;
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorId(codigoCTe, codigoEmpresa);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEvento envioWS = this.obterCteCancelamento(cte, justificativa, dataCancelamento);

                //Transmite
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.POST, envioWS, "cte-v4/events", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICte);

                if (retornoWS.erro)
                {
                    Servicos.Log.TratarErro(string.Concat("Emissor NSTech: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem));
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
                        Servicos.Log.TratarErro("Emissor NSTech: Ocorreu uma falha ao processar o retorno do envio do cancelamento do cte");
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de cancelamento CTe Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        Servicos.Log.TratarErro(string.Format("Emissor NSTech: Ocorreu uma falha ao solicitar o cancelamento do cte; RetornoWS {0}.", retornoWS.jsonRetorno));
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
                    cte.ObservacaoCancelamento = justificativa;
                    cte.MensagemRetornoSefaz = "Cancelamento solicitado com sucesso.";
                    cte.CobrarCancelamento = cobrarCancelamento == "Sim";

                    if (gerarLog && usuario != null)
                        cte.Log = string.Concat(cte.Log, " Enviado cancelamento por ", usuario.CPF, " - ", usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                    cte.Status = "K";
                    repCTe.Atualizar(cte);

                    if (sincronizarDocumento)
                        this.ConsultarCte(cte, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Emissor NSTech: Ocorreu uma falha ao efetuar o cancelamento do cte");
                Servicos.Log.TratarErro(ex);
                sucesso = false;
            }

            return sucesso;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEvento obterCteCancelamento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string justificativa, DateTime? dataCancelamento)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEvento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEvento();

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(cte.Empresa.FusoHorario);
            DateTime dtCancelamento = dataCancelamento.HasValue ? dataCancelamento.Value : DateTime.Now;
            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(dtCancelamento);
            string fusoHorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);

            // Tratamento para remover caracteres especiais
            Regex rgx = new Regex(Servicos.CTe.ReplaceMotivoRegexPatternBack);
            justificativa = rgx.Replace(justificativa, "");

            retorno.data = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoData();
            retorno.data.externalId = cte.Codigo.ToString();
            retorno.data.eventDate = dtCancelamento.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "T") + fusoHorario;
            retorno.data.cteKey = cte.Chave;
            retorno.data.eventSequence = 1;
            retorno.data.issueType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType.default_;

            retorno.data.issuer = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoIssuer();
            retorno.data.issuer.type = cte.Empresa.Tipo == "F" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
            retorno.data.issuer.document = Utilidades.String.OnlyNumbers(cte.Empresa.CNPJ);
            retorno.data.issuer.state = cte.Empresa.Localidade.Estado.Sigla;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoIssuerCancel eventoCancelamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoIssuerCancel();
            eventoCancelamento.protocolNumber = cte.Protocolo;
            eventoCancelamento.reason = justificativa;
            retorno.data.evento = eventoCancelamento;

            retorno.options = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptions();
            retorno.options.removeSpecialsChars = true;

            return retorno;
        }

        #endregion Métodos Privados
    }
}
