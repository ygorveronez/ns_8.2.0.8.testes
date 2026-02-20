using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool EncerrarMdfe(int codigoMDFe, int codigoEmpresa, DateTime dataEncerramento, Repositorio.UnitOfWork unitOfWork = null, string stringConexao = null, DateTime? dataEvento = null)
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

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento envioWS = this.obterMdfeEncerramento(mdfe, dataEncerramento, dataEvento, unitOfWork);

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
                        mensagemErro = "Emissor NSTech: Ocorreu uma falha ao processar o retorno do envio do encerramento do mdfe";
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar retorno JSON MDFe encerramento Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao solicitar o encerramento do mdfe; RetornoWS {0}.", retornoWS.jsonRetorno);
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
                    mdfe.CodigoIntegradorEncerramento = 0;
                    mdfe.MensagemRetornoSefaz = "Evento em processamento.";
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmEncerramento;
                    repMDFe.Atualizar(mdfe);

                    if (sincronizarDocumento)
                        this.ConsultarMdfe(mdfe, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unitOfWork);
                }
                else
                {
                    mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(mensagemErro);
                    repMDFe.Atualizar(mdfe);

                    Servicos.Log.TratarErro(mensagemErro);
                }
            }
            catch (Exception ex)
            {
                mensagemErro = "Emissor NSTech: Ocorreu uma falha ao efetuar o encerramento do mdfe";
                Servicos.Log.TratarErro(mensagemErro);
                Servicos.Log.TratarErro(ex);

                if (mdfe != null)
                {
                    mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(mensagemErro);
                    repMDFe.Atualizar(mdfe);
                }

                sucesso = false;
            }

            return sucesso;
        }

        public bool EncerrarMDFeEmissorExterno(Dominio.ObjetosDeValor.MDFe.MDFeEmissorExterno mdfeEmissorExterno, Repositorio.UnitOfWork unitOfWork)
        {
            bool sucesso = false;
            string mensagemErro = string.Empty;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento envioWS = this.obterMdfeEncerramentoEmissorExterno(mdfeEmissorExterno, unitOfWork);

                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.POST, envioWS, "mdfe-v3/events", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPIMDFe);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Emissor NSTech Emissor Externo: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    Servicos.Log.TratarErro(mensagemErro);
                    sucesso = false;

                    try
                    {
                        if (retornoWS.StatusCode == 400 && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                        {
                            dynamic objetoRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retornoWS.jsonRetorno);

                            if (objetoRetorno?.type == "event_is_authorized")
                                sucesso = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        mensagemErro = "Emissor NSTech Emissor Externo: Ocorreu uma falha ao processar o retorno do envio do encerramento do mdfe";
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
                    catch { }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech Emissor Externo: Ocorreu uma falha ao solicitar o encerramento do mdfe; RetornoWS {0}.", retornoWS.jsonRetorno);
                        Servicos.Log.TratarErro(mensagemErro);
                        sucesso = false;
                    }
                    else
                        sucesso = true;
                }

                if (!sucesso)
                    Servicos.Log.TratarErro(mensagemErro);
            }
            catch (Exception ex)
            {
                mensagemErro = "Emissor NSTech Emissor Externo: Ocorreu uma falha ao efetuar o encerramento do mdfe";
                Servicos.Log.TratarErro(mensagemErro);
                Servicos.Log.TratarErro(ex);

                sucesso = false;
            }

            return sucesso;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento obterMdfeEncerramento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, DateTime dataEncerramento, DateTime? dataEvento, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento();

            Repositorio.MunicipioDescarregamentoMDFe repMunDescMDFe = new Repositorio.MunicipioDescarregamentoMDFe(unitOfWork);
            List<Dominio.Entidades.MunicipioDescarregamentoMDFe> municipioDescarregamentoMDFe = repMunDescMDFe.BuscarPorMDFe(mdfe.Codigo);

            DateTime dtEvento = dataEvento.HasValue ? dataEvento.Value : DateTime.Now;

            retorno.data = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoData();
            retorno.data.externalId = $"{mdfe.Codigo}_close";
            retorno.data.eventDate = new DateTimeOffset(dtEvento, TimeSpan.FromHours(-3)).ToString("yyyy-MM-ddTHH:mm:sszzz");
            retorno.data.mdfeKey = mdfe.Chave;
            retorno.data.eventSequence = 1;

            retorno.data.issuer = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuer();
            retorno.data.issuer.type = mdfe.Empresa.Tipo == "F" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
            retorno.data.issuer.document = Utilidades.String.OnlyNumbers(mdfe.Empresa.CNPJ);
            retorno.data.issuer.state = mdfe.Empresa.Localidade.Estado.Sigla;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuerClose eventoEncerramento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuerClose();
            eventoEncerramento.protocolNumber = mdfe.Protocolo;
            eventoEncerramento.closeDate = dataEncerramento.ToString("yyyy-MM-dd");

            if (mdfe.MunicipioEncerramento != null && mdfe.MunicipioEncerramento.CodigoIBGE > 0)
                eventoEncerramento.closeCity = mdfe.MunicipioEncerramento.CodigoIBGE.ToString();
            else
                eventoEncerramento.closeCity = (municipioDescarregamentoMDFe != null && municipioDescarregamentoMDFe.Count > 0 && municipioDescarregamentoMDFe.FirstOrDefault().Municipio != null ? municipioDescarregamentoMDFe.FirstOrDefault().Municipio.CodigoIBGE : mdfe.Empresa.Localidade.CodigoIBGE).ToString();

            if (eventoEncerramento.closeCity == "9999999")
                eventoEncerramento.closeCity = mdfe.Empresa.Localidade.CodigoIBGE.ToString();

            if (mdfe.MunicipioEncerramento != null && mdfe.MunicipioEncerramento.Estado != null)
                eventoEncerramento.closeState = mdfe.MunicipioEncerramento.Estado.Sigla;
            else
                eventoEncerramento.closeState = municipioDescarregamentoMDFe.FirstOrDefault().Municipio.Estado.Sigla;

            if (eventoEncerramento.closeState == "EX")
                eventoEncerramento.closeState = mdfe.Empresa.Localidade.Estado.Sigla;

            retorno.data.evento = eventoEncerramento;

            retorno.options = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptions();
            retorno.options.removeSpecialsChars = true;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento obterMdfeEncerramentoEmissorExterno(Dominio.ObjetosDeValor.MDFe.MDFeEmissorExterno mdfeEmissorExterno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento();

            retorno.data = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoData();
            retorno.data.externalId = $"{mdfeEmissorExterno.Protocolo}_external_close";
            retorno.data.eventDate = new DateTimeOffset(mdfeEmissorExterno.DataEvento, TimeSpan.FromHours(-3)).ToString("yyyy-MM-ddTHH:mm:sszzz");
            retorno.data.mdfeKey = mdfeEmissorExterno.Chave;
            retorno.data.eventSequence = 1;

            retorno.data.issuer = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuer();
            retorno.data.issuer.type = mdfeEmissorExterno.Empresa.Tipo == "F" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
            retorno.data.issuer.document = Utilidades.String.OnlyNumbers(mdfeEmissorExterno.Empresa.CNPJ);
            retorno.data.issuer.state = mdfeEmissorExterno.Empresa.Localidade?.Estado.Sigla ?? string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuerClose eventoEncerramento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuerClose();
            eventoEncerramento.protocolNumber = mdfeEmissorExterno.Protocolo;
            eventoEncerramento.closeDate = mdfeEmissorExterno.DataEncerramento.ToString("yyyy-MM-dd");

            if (mdfeEmissorExterno.CodigoMunicipioEncerramento > 0)
                eventoEncerramento.closeCity = mdfeEmissorExterno.CodigoMunicipioEncerramento.ToString();

            if (string.IsNullOrWhiteSpace(eventoEncerramento.closeCity) || (eventoEncerramento.closeCity == "9999999"))
                eventoEncerramento.closeCity = mdfeEmissorExterno.Empresa.Localidade?.CodigoIBGE.ToString() ?? string.Empty;

            if (mdfeEmissorExterno.CodigoUFEncerramento > 0)
            {
                Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);
                Dominio.Entidades.Estado estadoEncerramento = repositorioEstado.BuscarPorIBGE(mdfeEmissorExterno.CodigoUFEncerramento);

                eventoEncerramento.closeState = estadoEncerramento?.Sigla ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(eventoEncerramento.closeState) || (eventoEncerramento.closeState == "EX"))
                eventoEncerramento.closeState = mdfeEmissorExterno.Empresa.Localidade?.Estado.Sigla ?? string.Empty;

            retorno.data.evento = eventoEncerramento;

            retorno.options = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptions();
            retorno.options.removeSpecialsChars = true;

            return retorno;
        }

        #endregion Métodos Privados
    }
}
