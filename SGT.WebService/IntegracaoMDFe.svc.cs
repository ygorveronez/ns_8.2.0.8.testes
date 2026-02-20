using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using CoreWCF;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class IntegracaoMDFe(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IIntegracaoMDFe
    {
        #region Métodos Globais

        public Retorno<bool> IntegrarMDFeAutorizado(Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                string mensagemErro = string.Empty;

                Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoEventoCTe retornoEventoMDFe = new Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoEventoCTe();
                retornoEventoMDFe.mdfeOracle = mdfeOracle;
                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador).ReceberEventoMdfe(out mensagemErro, null, retornoEventoMDFe, Auditado, TipoServicoMultisoftware, unitOfWork))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Objeto = false;
                    retorno.Mensagem = mensagemErro;
                    return retorno;
                }

                retorno.Status = true;
                retorno.Objeto = true;

                return retorno;
            }
            catch (Exception ex)
            {
                ArmazenarLogIntegracao(mdfeOracle, unitOfWork);
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Objeto = false;
                retorno.Mensagem = "Ocorreu uma falha ao integrar o MDF-e.";
                return retorno;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> IntegrarEventoInclusaoMotorista(Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle)
        {
            Retorno<bool> retorno = new Retorno<bool>();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unitOfWork);
                Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unitOfWork);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null;

                int statusMDFe = 0;
                int.TryParse(string.IsNullOrWhiteSpace(mdfeOracle.CodStatusProtocolo) ? mdfeOracle.CodStatusEnvio : mdfeOracle.CodStatusProtocolo, out statusMDFe);

                string mensagemRetorno = string.Empty;

                DateTime dataProtocolo;
                DateTime.TryParseExact(mdfeOracle.DataProtocolo, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataProtocolo);

                if (mdfeOracle.CodigoMDFeEvento > 0)
                {
                    mdfe = repMDFe.BuscarPorCodigoIntegradorEncerramento(mdfeOracle.CodigoMDFeEvento);

                    if (mdfe == null)
                    {
                        retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                        retorno.Objeto = false;
                        retorno.Mensagem = "MDF-e " + mdfeOracle.ChaveMDFe + " não localizado na base SqlServer.";

                        return retorno;
                    }

                    if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado)
                    {
                        unitOfWork.Start();

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, "Integrou o evento de inclusão de motorista do MDF-e.", unitOfWork);

                        mdfe.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(statusMDFe, Dominio.Enumeradores.TipoErroSefaz.MDFe);

                        if (mdfeOracle.StatusIntegrador == "R")
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));

                            repMDFe.Atualizar(mdfe);

                            Dominio.Entidades.MotoristaMDFe motoristaMDFe = repMotoristaMDFe.BuscarPorTipo(mdfe.Codigo, Dominio.Enumeradores.TipoMotoristaMDFe.SolicitadoEventoInclusao);

                            if (motoristaMDFe != null)
                            {
                                motoristaMDFe.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.EventoInclusaoRejeitado;
                                repMotoristaMDFe.Atualizar(motoristaMDFe);
                            }

                            unitOfWork.CommitChanges();
                        }
                        else if (mdfeOracle.StatusIntegrador == "I")
                        {
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));

                            repMDFe.Atualizar(mdfe);

                            unitOfWork.CommitChanges();
                        }
                        else if (mdfeOracle.StatusIntegrador == "M" || mdfeOracle.StatusIntegrador == "D" || mdfeOracle.StatusIntegrador == "P")
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.Info.Mensagem, " - ", mdfeOracle.Info.MensagemOriginal));

                            Dominio.Entidades.MotoristaMDFe motoristaMDFe = repMotoristaMDFe.BuscarPorTipo(mdfe.Codigo, Dominio.Enumeradores.TipoMotoristaMDFe.SolicitadoEventoInclusao);
                            if (motoristaMDFe != null)
                            {
                                motoristaMDFe.ProtocoloEventoInclusao = mdfeOracle.NumeroProtocolo;
                                motoristaMDFe.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Incluido;
                                repMotoristaMDFe.Atualizar(motoristaMDFe);
                            }

                            svcMDFe.SalvarXMLInclusaoMotorista(mdfe, mdfeOracle, unitOfWork);

                            unitOfWork.CommitChanges();

                            string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";

                            //deleta o PDF para gerar novamente com os dados de motoristas corretos
                            Utilidades.IO.FileStorageService.Storage.Delete(caminhoPDF);
                        }
                    }
                }

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = true;
                retorno.Objeto = true;

                return retorno;
            }
            catch (Exception ex)
            {
                ArmazenarLogIntegracao(mdfeOracle, unitOfWork);
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Objeto = false;
                retorno.Mensagem = "Ocorreu uma falha ao integrar o MDF-e.";
                return retorno;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceIntegracaoMDFe;
        }

        #endregion
    }
}
