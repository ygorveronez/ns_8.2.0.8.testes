using CoreWCF;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.WebService.CTe;
using Dominio.ObjetosDeValor.WebService.NFS;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class NFS(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), INFS
    {
        #region Métodos Globais

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorCarga(int protocoloIntegracaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                RequestCtePorCarga requestBuscarCte = new RequestCtePorCarga()
                {
                    TipoDocumentoRetorno = tipoDocumentoRetorno ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum,
                    Inicio = inicio ?? 0,
                    Limite = limite ?? 0,
                    ProtocoloCarga = protocoloIntegracaoCarga
                };

                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarNFSsPorCarga(requestBuscarCte, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarNFSsCompletasPorCarga(int protocoloIntegracaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                RequestCtePorCarga requestBuscarCte = new RequestCtePorCarga()
                {
                    TipoDocumentoRetorno = tipoDocumentoRetorno ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum,
                    Inicio = inicio ?? 0,
                    Limite = limite ?? 0,
                    ProtocoloCarga = protocoloIntegracaoCarga
                };

                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarNFSsCompletasPorCarga(requestBuscarCte, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSs(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;
            tipoDocumentoRetorno ??= Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (limite > 50)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50.");

                if (protocolo == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de integração.");

                if (protocolo.protocoloIntegracaoCarga <= 0 && protocolo.protocoloIntegracaoPedido <= 0)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoDadosInvalidos("Por favor, informe os códigos de integração.");

                Servicos.WebService.NFS.NFS servicoWSNFS = new Servicos.WebService.NFS.NFS(unitOfWork);
                string mensagem = "";
                Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS> objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>();

                objeto.Itens = servicoWSNFS.BuscarNFS(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno)tipoDocumentoRetorno, (int)inicio, (int)limite, ref mensagem, TipoServicoMultisoftware);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoDadosInvalidos(mensagem);

                objeto.NumeroTotalDeRegistro = servicoWSNFS.ContarNFS(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, TipoServicoMultisoftware);

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou NFS-es", unitOfWork);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoSucesso(objeto);
            }
            catch (ServicoException excecao)
            {
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoExcecao(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as Notas Fiscais de Serviço");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite, string codigoTipoOperacao, string situacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarNFSsPorPeriodo(dataInicial, dataFinal, tipoDocumentoRetorno ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, inicio ?? 0, limite ?? 0, codigoTipoOperacao, situacao, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.NFS.NFS> BuscarNFSPorProtocolo(int protocoloNFS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.NFS.NFS>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarNFSPorProtocolo(protocoloNFS, tipoDocumentoRetorno ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>> BuscarNFSesComplementaresAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>>.CreateFrom(
                    new Servicos.WebService.NFS.NFS(
                        unitOfWork,
                        TipoServicoMultisoftware,
                        Cliente,
                        Auditado,
                        ClienteAcesso,
                        Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarNFSesComplementaresAguardandoIntegracao(
                        tipoDocumentoRetorno ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum,
                        inicio ?? 0,
                        limite ?? 0
                    )
                );

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> BuscarNFSAguardandoIntegracao(int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>>.CreateFrom(new Servicos.WebService.NFS.NFS(
                    unitOfWork,
                    TipoServicoMultisoftware,
                    Cliente,
                    Auditado,
                    ClienteAcesso,
                    Conexao.createInstance(_serviceProvider).AdminStringConexao
                ).BuscarNFSAguardandoIntegracao(
                    inicio ?? 0,
                    limite ?? 0
                ));
            });
        }

        public Retorno<bool> EnviarNFSe(List<Dominio.ObjetosDeValor.WebService.NFS.NFS> notasFiscais)
        {
            Servicos.Log.TratarErro($"EnviarNFSe: {(notasFiscais != null ? Newtonsoft.Json.JsonConvert.SerializeObject(notasFiscais) : string.Empty)}", "Request");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (notasFiscais == null || notasFiscais.Count == 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Nenhuma nota fiscal informada");

                Servicos.WebService.NFS.NFS servicoNFSe = new Servicos.WebService.NFS.NFS(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                unitOfWork.Start();

                foreach (Dominio.ObjetosDeValor.WebService.NFS.NFS notaFiscal in notasFiscais)
                {
                    Dominio.Entidades.Empresa empresa = null;

                    if (notaFiscal.TransportadoraEmitente != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(notaFiscal.TransportadoraEmitente.CNPJ)))
                        empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(notaFiscal.TransportadoraEmitente.CNPJ));

                    if (empresa == null)
                        empresa = integradora.Empresa;

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico nfseGerada = repCTe.BuscarNFSe(notaFiscal.NFSe.Numero, notaFiscal.NFSe.Serie, empresa.Codigo, notaFiscal.NFSe.CodigoVerificacao);

                    if (nfseGerada == null)
                    {
                        nfseGerada = servicoNFSe.GerarNFSe(notaFiscal, empresa, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, nfseGerada, "Documento gerado por integração", unitOfWork);
                    }
                    else if (notaFiscal.Cancelada && nfseGerada.Status != "C")
                    {
                        nfseGerada.ObservacaoCancelamento = "Cancelamento da NFSe recebido por integração";
                        nfseGerada.Status = "C";
                        repCTe.Atualizar(nfseGerada);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, nfseGerada, "Cancelamento da NFSe recebido por integração", unitOfWork);
                    }
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao processar as notas fiscais de serviço");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarIntegracaoNFS(int protocoloNFS)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoNFS(protocoloNFS));
            });
        }

        public Retorno<bool> InformarRejeicaoNFS(int protocoloNFS, string msgRejeicao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<bool> retorno = new Retorno<bool>() { Status = true };

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unitOfWork);
                Repositorio.Embarcador.NFS.NFSManualIntegracaoLote repNFSManualIntegracaoLote = new Repositorio.Embarcador.NFS.NFSManualIntegracaoLote(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(protocoloNFS, true);
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCTe(protocoloNFS);

                if (cte == null || lancamentoNFSManual == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Nenhuma NFS Manual encontrada.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    retorno.Objeto = false;
                    return retorno;
                }
                if (string.IsNullOrWhiteSpace(msgRejeicao))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Favor informe uma rejeição.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    retorno.Objeto = false;
                    return retorno;
                }

                unitOfWork.Start();

                lancamentoNFSManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.FalhaIntegracao;

                Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfseManualCTeIntegracao = repNFSManualCTeIntegracao.BuscarPrimeiroPorLancamentoNFSManual(lancamentoNFSManual.Codigo);
                Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote loteIntegracao = null;
                if (nfseManualCTeIntegracao != null)
                {
                    if (nfseManualCTeIntegracao.Lote == null)
                        loteIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote();
                    else
                        loteIntegracao = nfseManualCTeIntegracao.Lote;
                }
                else
                {
                    nfseManualCTeIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao();
                    loteIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote();
                }

                loteIntegracao.CodigoConsultaIntegradora = protocoloNFS.ToString();
                loteIntegracao.DataEnvio = DateTime.Now;
                loteIntegracao.DataRecebimento = DateTime.Now;
                loteIntegracao.DataUltimaConsultaRetorno = DateTime.Now;
                loteIntegracao.NumeroTentativas += 1;
                loteIntegracao.ProblemaIntegracao = msgRejeicao;
                loteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                loteIntegracao.TipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada);
                if (loteIntegracao.Codigo > 0)
                    repNFSManualIntegracaoLote.Atualizar(loteIntegracao);
                else
                    repNFSManualIntegracaoLote.Inserir(loteIntegracao);

                nfseManualCTeIntegracao.DataIntegracao = DateTime.Now;
                nfseManualCTeIntegracao.LancamentoNFSManual = lancamentoNFSManual;
                nfseManualCTeIntegracao.Lote = loteIntegracao;
                nfseManualCTeIntegracao.NumeroTentativas += 1;
                nfseManualCTeIntegracao.ProblemaIntegracao = msgRejeicao;
                //nfseManualCTeIntegracao.SistemaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaIntegracao.NaoInformado;
                nfseManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                nfseManualCTeIntegracao.TipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada);

                if (nfseManualCTeIntegracao.Codigo > 0)
                    repNFSManualCTeIntegracao.Atualizar(nfseManualCTeIntegracao);
                else
                    repNFSManualCTeIntegracao.Inserir(nfseManualCTeIntegracao);

                repLancamentoNFSManual.Atualizar(lancamentoNFSManual);

                // Integracao com SignalR
                svcNFSManual.InformarLancamentoNFSManualAtualizada(lancamentoNFSManual.Codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, "Recebido a rejeição da NFS Manual pela integração", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, lancamentoNFSManual, "Recebido a rejeição da NFS Manual pela integração", unitOfWork);

                unitOfWork.CommitChanges();

                retorno.Objeto = true;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Objeto = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a rejeição da NFS Manual.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoNFSComplementar(int protocoloNFS)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoNFSComplementar(protocoloNFS));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorOcocorrencia(RequestNFSOcorrencia dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarNFSsPorOcocorrencia(dadosRequest, integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>> ConsultarDocumentosParaEmissaoDeNotaManual()
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConsultarDocumentosParaEmissaoDeNotaManual(integradora));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<int> GerarNotaManual(GerarNotaManual gerarNotaManual)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<int>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).GerarNotaManual(gerarNotaManual));
            });
        }

        public Retorno<int> EnviarXMLNFSManual(int protocolo, string nfseBase64)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<int>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarXMLNFSManual(protocolo, nfseBase64));
            });
        }

        public Retorno<int> EnviarPDFNFSManual(int protocolo, string pdfBase64)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<int>.CreateFrom(new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarPDFNFSManual(protocolo, pdfBase64));
            });
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceNFS;
        }

        #endregion
    }
}
