using System;
using Microsoft.AspNetCore.Http;
using CoreWCF;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class IntegracaoNFSe(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IIntegracaoNFSe
    {
        #region Métodos Globais

        public Retorno<bool> IntegrarNFSeAutorizado(Dominio.ObjetosDeValor.WebService.NFSe.NFSeOracle nfseOracle)
        {
            //WebServiceBase.ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoSGT = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.RPSNFSe repRPSNFSe = new Repositorio.RPSNFSe(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = nfseOracle.CodigoNFSeInterno > 0 ? repCTe.BuscarNFSePorProtocoloIntegracaoOracle(nfseOracle.CodigoNFSeInterno) : null;
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                bool autorizarAverbacao = false, cancelarAverbacao = false;
                if (cte == null)
                {
                    unitOfWork.Rollback();

                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Objeto = false;
                    retorno.Mensagem = "NFSe " + nfseOracle.CodigoNFSeInterno + " não localizado na base SqlServer";
                    return retorno;
                }
                else if (cte.Status == "E" || cte.Status == "Y")
                {
                    DateTime dataProtocolo;
                    DateTime.TryParseExact(nfseOracle.DataProtocolo, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataProtocolo);

                    bool duplicidade = false;

                    if (nfseOracle.StatusIntegrador.Equals("E"))
                    {
                        cte.Status = "R";
                        cte.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(nfseOracle.DescricaoStatusIntegrador, " - ", nfseOracle.CodigoRetornoRPS, " - ", nfseOracle.DescricaoEnvio));
                        cte.StatusIntegrador = nfseOracle.StatusIntegrador;
                    }
                    else if (nfseOracle.StatusIntegrador.Equals("I"))
                    {
                        cte.Status = "E";
                        cte.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(nfseOracle.DescricaoStatusIntegrador, " - ", nfseOracle.CodigoRetornoRPS, " - ", nfseOracle.DescricaoEnvio));
                        cte.StatusIntegrador = nfseOracle.StatusIntegrador;
                    }
                    else if (nfseOracle.StatusIntegrador.Equals("D") || nfseOracle.StatusIntegrador.Equals("M"))
                    {
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && repCTe.VerificaNFSeJaAutorizada(nfseOracle.NumeroNFSe, cte.Serie.Codigo, cte.ModeloDocumentoFiscal.Codigo, cte.Empresa.Codigo, cte.DataEmissao.Value.Year, cte.TipoAmbiente, nfseOracle.CodigoVerificacaoNFSe) > 0)
                        {
                            cte.Status = "R";
                            cte.MensagemRetornoSefaz = "NFSe não processada na prefeitura, favor aguardar e reenviar.";
                            cte.CodigoCTeIntegrador = 0;

                            Servicos.Log.TratarErro("NFSe recebida em duplicidade: " + nfseOracle.NumeroNFSe.ToString() + " transportadora " + cte.Empresa.CNPJ_SemFormato);
                        }
                        else
                        {
                            autorizarAverbacao = true;
                            cte.Status = "A";
                            cte.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(nfseOracle.Info.Mensagem, " - ", nfseOracle.Info.MensagemOriginal));
                            cte.Protocolo = nfseOracle.CodigoVerificacaoNFSe;

                            DateTime dataAutorizacao = dataProtocolo != DateTime.MinValue ? dataProtocolo : DateTime.Now;

                            cte.DataRetornoSefaz = dataAutorizacao;
                            cte.DataAutorizacao = dataAutorizacao;
                            cte.Chave = "";
                            cte.StatusIntegrador = nfseOracle.StatusIntegrador;

                            cte.RPS.Status = "A";
                            cte.RPS.CodigoRetorno = nfseOracle.CodigoRetornoRPS;
                            cte.RPS.MensagemRetorno = cte.MensagemRetornoSefaz;
                            cte.RPS.Protocolo = nfseOracle.ProtocoloRPS;
                            cte.RPS.Data = dataAutorizacao;
                            cte.Numero = nfseOracle.NumeroNFSe;

                            if (nfseOracle.NumeroRPS > 0)
                                cte.RPS.Numero = nfseOracle.NumeroRPS;
                            if (nfseOracle.DescricaoStatusIntegrador != "DANFSE emitido")
                                cte.NumeroPrefeituraNFSe = nfseOracle.DescricaoStatusIntegrador;
                            else
                                cte.NumeroPrefeituraNFSe = cte.Numero.ToString();

                            repRPSNFSe.Atualizar(cte.RPS);

                            int quantidadeNFSes = repCTe.ContarCTePorChaveUnica(cte.Numero, cte.Serie.Codigo, cte.ModeloDocumentoFiscal.Codigo, cte.Empresa.Codigo, cte.TipoAmbiente);
                            cte.TipoControle = quantidadeNFSes + 1;

                            if (configuracaoWebService.NaoPermitirGerarNFSeComMesmaNumeracao)
                            {
                                int quantidadeNFSesMesmaNumeracao = repCTe.ContarCTePorChaveUnicaEStatus(cte.Numero, cte.Serie.Codigo, cte.ModeloDocumentoFiscal.Codigo, cte.Empresa.Codigo, cte.TipoAmbiente, new string[] { "A", "R" });
                                if (quantidadeNFSesMesmaNumeracao > 1)
                                {
                                    cte.Status = "R";
                                    cte.RPS.Status = "R";
                                    cte.MensagemRetornoSefaz = "Rejeitada por duplicidade de numeração na prefeitura";
                                }
                            }


                            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);

                            if (cargaCTe != null && cargaCTe.Carga.problemaCTE)
                            {
                                cargaCTe.Carga.PossuiPendencia = false;
                                cargaCTe.Carga.problemaCTE = false;
                                cargaCTe.Carga.MotivoPendencia = string.Empty;
                                repCarga.Atualizar(cargaCTe.Carga);
                            }
                        }
                    }

                    repCTe.Atualizar(cte);

                    if (cte.Status == "A")
                    {
                        serCTe.ObterESalvarDANFSEOracle(cte.Codigo, cte.Empresa.Codigo, unitOfWork, duplicidade ? null : nfseOracle);
                        serCTe.ObterESalvarXMLAutorizacaoNFSeOracle(cte.Codigo, cte.Empresa.Codigo, false, unitOfWork, duplicidade ? null : nfseOracle);
                        //serCTe.SalvarMovimentoDoFinanceiro(cte.Codigo, cte.Empresa.Codigo, unitOfWork);
                    }

                    if (nfseOracle.StatusIntegrador != "I")
                        serCTe.SalvarRetornoSefaz(cte, "A", nfseOracle.CodigoNFSeInterno, 0, string.IsNullOrWhiteSpace(nfseOracle.DescricaoProtocolo) ? nfseOracle.DescricaoEnvio : nfseOracle.DescricaoProtocolo, unitOfWork);
                }
                else if (cte.Status == "K")
                {
                    if (nfseOracle.StatusIntegrador.Equals("C"))
                    {
                        serCTe.SalvarRetornoSefaz(cte, "C", nfseOracle.CodigoNFSeInterno, 0, string.IsNullOrWhiteSpace(nfseOracle.DescricaoProtocolo) ? nfseOracle.DescricaoEnvio : nfseOracle.DescricaoProtocolo, unitOfWork);

                        DateTime dataProtocolo;
                        DateTime.TryParseExact(nfseOracle.DataProtocolo, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataProtocolo);

                        cte.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(nfseOracle.DescricaoProtocolo);
                        cte.Status = "C";
                        cte.Cancelado = "S";

                        DateTime dataCancelamento = dataProtocolo != DateTime.MinValue ? dataProtocolo : DateTime.Now;

                        cte.DataRetornoSefaz = dataCancelamento;
                        cte.DataCancelamento = dataCancelamento;

                        cte.RPS.Status = "C";
                        cte.RPS.CodigoRetorno = nfseOracle.CodigoRetornoRPS;
                        cte.RPS.MensagemRetorno = nfseOracle.DescricaoProtocolo;
                        cte.RPS.ProtocoloCancelamento = nfseOracle.ProtocoloRPS;
                        cte.RPS.DataProtocolo = dataCancelamento;

                        repCTe.Atualizar(cte);
                        repRPSNFSe.Atualizar(cte.RPS);

                        serCTe.SalvarMovimentoDoFinanceiro(cte, cte.Empresa.Codigo, unitOfWork);

                        cancelarAverbacao = true;
                    }
                    else if (nfseOracle.StatusIntegrador.Equals("E"))
                    {
                        cte.Status = "A";
                        cte.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(nfseOracle.DescricaoProtocolo);
                    }
                    repCTe.Atualizar(cte);
                }

                if (autorizarAverbacao)
                    serCTe.AjustarAverbacoesParaAutorizacao(cte.Codigo, unitOfWork);
                else if (cancelarAverbacao)
                    serCTe.AjustarAverbacoesParaCancelamento(cte.Codigo, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, "Integrou NFS-e autorizada", unitOfWork);

                unitOfWork.CommitChanges();

                //if (autorizarAverbacao)
                //    serCTe.EmitirAverbacoesOracle(cte.Empresa.Codigo, cte.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Pendente, unitOfWork);
                //else if (cancelarAverbacao)
                //    serCTe.CancelarAverbacoesOracle(cte.Empresa.Codigo, cte.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso, unitOfWork);

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = true;
                retorno.Objeto = true;

                return retorno;
            }
            catch (Exception ex)
            {
                ArmazenarLogIntegracao(nfseOracle, unitOfWork);
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Objeto = false;
                retorno.Mensagem = "Ocorreu uma falha ao informar o CTe";
                return retorno;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceIntegracaoNFSe;
        }

        #endregion
    }
}
