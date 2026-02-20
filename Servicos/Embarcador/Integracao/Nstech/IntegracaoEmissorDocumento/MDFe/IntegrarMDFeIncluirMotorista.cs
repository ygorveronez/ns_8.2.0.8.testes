using Servicos.Extensions;
using System;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool IncluirMotorista(int codigoMDFe, int codigoEmpresa, string cpfMotorista, string nomeMotorista, Repositorio.UnitOfWork unitOfWork = null, string stringConexao = null, DateTime? dataEvento = null)
        {
            bool sucesso = false;
            string id = string.Empty;
            string mensagemErro = string.Empty;

            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(stringConexao);

            Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Embarcador.Carga.CargaMotorista(unitOfWork);

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null;

            try
            {
                bool sincronizarDocumento = false;
                mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

                Dominio.Entidades.Usuario entidadeMotorista = repositorioUsuario.BuscarPorCPF(cpfMotorista);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repositorioCargaMDFe.BuscarPorMDFe(codigoMDFe);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento envioWS = this.obterMdfeIncluirMotorista(mdfe, cpfMotorista, nomeMotorista, dataEvento, unitOfWork);

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
                        mensagemErro = "Emissor NSTech: Ocorreu uma falha ao processar o retorno do envio da inclusão de motorista no mdfe";
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar retorno JSON MDFe incluir motorista Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao solicitar ao incluir o motorista no mdfe; RetornoWS {0}.", retornoWS.jsonRetorno);
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
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado;
                    repMDFe.Atualizar(mdfe);

                    if (cargaMDFe != null)
                        servicoCargaMotorista.AdicionarMotorista(cargaMDFe.Carga, entidadeMotorista);

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
                mensagemErro = "Emissor NSTech: Ocorreu uma falha ao efetuar ao incluir o motorista no mdfe";
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

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento obterMdfeIncluirMotorista(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, string cpfMotorista, string nomeMotorista, DateTime? dataEvento, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEvento();

            DateTime dtEvento = dataEvento.HasValue ? dataEvento.Value : DateTime.Now;

            retorno.data = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoData();
            retorno.data.eventSequence = this.BuscarSequencialEventoMDFe(mdfe.Codigo, unitOfWork);
            retorno.data.externalId = $"{mdfe.Codigo}_includedriver_{retorno.data.eventSequence}";
            retorno.data.eventDate = dtEvento.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "T");
            retorno.data.mdfeKey = mdfe.Chave;

            retorno.data.issuer = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuer();
            retorno.data.issuer.type = mdfe.Empresa.Tipo == "F" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
            retorno.data.issuer.document = Utilidades.String.OnlyNumbers(mdfe.Empresa.CNPJ);
            retorno.data.issuer.state = mdfe.Empresa.Localidade.Estado.Sigla;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuerIncludeDriver eventoInclusaoMotorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuerIncludeDriver();
            eventoInclusaoMotorista.protocolNumber = mdfe.Protocolo;
            eventoInclusaoMotorista.driver = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeEventoIssuerIncludeDriverDriver();
            eventoInclusaoMotorista.driver.document = cpfMotorista;
            eventoInclusaoMotorista.driver.name = nomeMotorista;

            retorno.data.evento = eventoInclusaoMotorista;

            retorno.options = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.mdfeOptions();
            retorno.options.removeSpecialsChars = true;

            return retorno;
        }

        private int BuscarSequencialEventoMDFe(int codigoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MDFeRetornoSefaz repMDFeRetornoSefaz = new Repositorio.MDFeRetornoSefaz(unitOfWork);
            int sequencialEvento = repMDFeRetornoSefaz.BuscarSequencialEventosPorMDFe(codigoMDFe);
            return sequencialEvento + 1;
        }

        #endregion Métodos Privados
    }
}
