using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using System.Linq;
using Repositorio;
using Repositorio.Embarcador.Cargas;
using Repositorio.Embarcador.Pedidos;
using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Pedido
{
    public class RolagemContainer
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public RolagemContainer(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga ProcessarRolagemContainer(Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga retorno = new Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga();

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();

            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga cargaNova = importacaoPendente.CargaPedido?.Carga;

                if (cargaNova == null)
                    throw new ServicoException("Nova carga não foi localizada. Confira os dados da importação.");

                if (cargaNova.CargaSVM)
                    throw new ServicoException("Não é possível rolar carga SVM.");

                _unitOfWork.Start();

                ReplicarDadosTransporteMaritmoCarga(cargaNova, importacaoPendente);
                ReplicarDadosTransporteMaritmoPedido(cargaNova, importacaoPendente, repPedido, repCargaPedido, repPedidoTransbordo, _unitOfWork);
                ReplicarDocumentosEtapa2Carga(importacaoPendente, repXMLNotaFiscal, repPedidoXMLNotaFiscal, repCargaPedidoXMLNotaFiscalCTe);
                ReplicarValoresFrete(cargaNova, importacaoPendente.CargaPedidoAntigo.Carga);
                ReplicarCargaComponenteFrete(cargaNova, importacaoPendente.CargaPedidoAntigo.Carga, _unitOfWork);
                ReplicarDadosCTe(importacaoPendente, repCargaCTe, repCTe, repPedidoNavioSchedule);
                ReplicarCargaIntegracao(cargaNova, importacaoPendente.CargaPedidoAntigo.Carga, _unitOfWork);

                cargaNova.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada;
                cargaNova = servicoCarga.AtualizarStatusCustoExtra(cargaNova, svcHubCarga, repCarga);
                cargaNova.RolagemCarga = true;
                repCarga.Atualizar(cargaNova);

                AtualizarDadosCargasSVM(importacaoPendente, repCargaPedido, repPedidoTransbordo);

                _unitOfWork.CommitChanges();

                AdicionarIntegracoesCarga(cargaNova);

                retorno.Sucesso = true;
                return retorno;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                retorno.Sucesso = false;
                retorno.Mensagem = ex.Message;
                return retorno;
            }
        }

        public void CancelarCargaRolada(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga == null)
                throw new ServicoException("Carga antiga não foi localizada");

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            carga.CargaComQuebraDeContainer = true;
            carga.RolagemCarga = true;
            repCarga.Atualizar(carga);

            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
            {
                Carga = carga,
                DefinirSituacaoEmCancelamento = true,
                DuplicarCarga = false,
                MotivoCancelamento = "Carga cancelada após rolagem",
                TipoServicoMultisoftware = _tipoServicoMultisoftware,
            };

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, unitOfWork);
            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, unitOfWork.StringConexao, _tipoServicoMultisoftware);
        }

        public void ReenviarIntegracaoFaturaEMP(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao = repFaturaIntegracao.BuscarPorCTeETipo(cte.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP);

            if (faturaIntegracao != null)
            {
                faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                repFaturaIntegracao.Atualizar(faturaIntegracao);
            }
        }

        #endregion

        #region Métodos Privados

        private void ReplicarDadosTransporteMaritmoCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente)
        {
            cargaNova.PedidoViagemNavio = importacaoPendente.PedidoViagemNavio;
            cargaNova.PortoOrigem = importacaoPendente.PortoOrigem;
            cargaNova.PortoDestino = importacaoPendente.PortoDestino;
            cargaNova.TerminalOrigem = importacaoPendente.TerminalOrigem;
            cargaNova.TerminalDestino = importacaoPendente.TerminalDestino;
        }

        private void ReplicarDadosTransporteMaritmoPedido(Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente, Repositorio.Embarcador.Pedidos.Pedido repPedido, Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido, PedidoTransbordo repPedidoTransbordo, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repCargaPedido.BuscarPedidosPorCarga(cargaNova.Codigo);
            if (pedidos != null && pedidos.Count > 0)
            {
                foreach (var pedido in pedidos)
                {
                    pedido.Initialize();

                    if (importacaoPendente.PedidoViagemNavio != null)
                        pedido.PedidoViagemNavio = importacaoPendente.PedidoViagemNavio;
                    if (importacaoPendente.PortoOrigem != null)
                        pedido.Porto = importacaoPendente.PortoOrigem;
                    if (importacaoPendente.PortoDestino != null)
                        pedido.PortoDestino = importacaoPendente.PortoDestino;
                    if (importacaoPendente.TerminalOrigem != null)
                        pedido.TerminalOrigem = importacaoPendente.TerminalOrigem;
                    if (importacaoPendente.TerminalDestino != null)
                        pedido.TerminalDestino = importacaoPendente.TerminalDestino;
                    if (importacaoPendente.Container != null)
                        pedido.Container = importacaoPendente.Container;

                    AdicionarTransbordosPedidos(importacaoPendente, repPedidoTransbordo, pedido);

                    pedido.LacreContainerUm = importacaoPendente.CargaPedidoAntigo.Pedido.LacreContainerUm;
                    pedido.LacreContainerDois = importacaoPendente.CargaPedidoAntigo.Pedido.LacreContainerDois;
                    pedido.LacreContainerTres = importacaoPendente.CargaPedidoAntigo.Pedido.LacreContainerTres;
                    pedido.TaraContainer = importacaoPendente.CargaPedidoAntigo.Pedido.TaraContainer;

                    if (string.IsNullOrWhiteSpace(pedido.TaraContainer) && importacaoPendente.CargaPedidoAntigo.Pedido?.Container != null)
                        pedido.TaraContainer = importacaoPendente.CargaPedidoAntigo.Pedido?.Container?.Tara.ToString();

                    pedido.Remetente = importacaoPendente.CargaPedidoAntigo.Pedido.Remetente;
                    pedido.Destinatario = importacaoPendente.CargaPedidoAntigo.Pedido.Destinatario;
                    pedido.Tomador = importacaoPendente.CargaPedidoAntigo.Pedido.Tomador;
                    pedido.Recebedor = importacaoPendente.CargaPedidoAntigo.Pedido.Recebedor;
                    pedido.Expedidor = importacaoPendente.CargaPedidoAntigo.Pedido.Expedidor;

                    pedido.Origem = importacaoPendente.CargaPedidoAntigo.Pedido.Origem;
                    pedido.Destino = importacaoPendente.CargaPedidoAntigo.Pedido.Destino;

                    repPedido.Atualizar(pedido, _auditado);
                }

                if (unitOfWork != null)
                {
                    Servicos.Embarcador.Carga.CargaDadosSumarizados svcCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();

                    svcCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaNova, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
                }
            }
        }

        private void AdicionarTransbordosPedidos(Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente, Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (importacaoPendente.PedidoViagemNavioTransbordo1 != null)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> pedidosTransbordos = repPedidoTransbordo.BuscarPorPedido(pedido.Codigo);
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo pedidoTransbordoExistente in pedidosTransbordos)
                    repPedidoTransbordo.Deletar(pedidoTransbordoExistente);
            }

            if (importacaoPendente.PedidoViagemNavioTransbordo1 != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo pedidoTransbordo = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                {
                    Navio = importacaoPendente.PedidoViagemNavioTransbordo1?.Navio,
                    Pedido = pedido,
                    Porto = importacaoPendente.PortoTransbordo1,
                    Sequencia = 1,
                    Terminal = importacaoPendente.TerminalTransbordo1,
                    PedidoViagemNavio = importacaoPendente.PedidoViagemNavioTransbordo1,
                };
                repPedidoTransbordo.Inserir(pedidoTransbordo);
            }

            if (importacaoPendente.PedidoViagemNavioTransbordo2 != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo pedidoTransbordo = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                {
                    Navio = importacaoPendente.PedidoViagemNavioTransbordo2?.Navio,
                    Pedido = pedido,
                    Porto = importacaoPendente.PortoTransbordo2,
                    Sequencia = 2,
                    Terminal = importacaoPendente.TerminalTransbordo2,
                    PedidoViagemNavio = importacaoPendente.PedidoViagemNavioTransbordo2,
                };
                repPedidoTransbordo.Inserir(pedidoTransbordo);
            }

            if (importacaoPendente.PedidoViagemNavioTransbordo3 != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo pedidoTransbordo = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                {
                    Navio = importacaoPendente.PedidoViagemNavioTransbordo3?.Navio,
                    Pedido = pedido,
                    Porto = importacaoPendente.PortoTransbordo3,
                    Sequencia = 3,
                    Terminal = importacaoPendente.TerminalTransbordo3,
                    PedidoViagemNavio = importacaoPendente.PedidoViagemNavioTransbordo3,
                };
                repPedidoTransbordo.Inserir(pedidoTransbordo);
            }

            if (importacaoPendente.PedidoViagemNavioTransbordo4 != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo pedidoTransbordo = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                {
                    Navio = importacaoPendente.PedidoViagemNavioTransbordo4?.Navio,
                    Pedido = pedido,
                    Porto = importacaoPendente.PortoTransbordo4,
                    Sequencia = 4,
                    Terminal = importacaoPendente.TerminalTransbordo4,
                    PedidoViagemNavio = importacaoPendente.PedidoViagemNavioTransbordo4,
                };
                repPedidoTransbordo.Inserir(pedidoTransbordo);
            }

            if (importacaoPendente.PedidoViagemNavioTransbordo5 != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo pedidoTransbordo = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                {
                    Navio = importacaoPendente.PedidoViagemNavioTransbordo5?.Navio,
                    Pedido = pedido,
                    Porto = importacaoPendente.PortoTransbordo5,
                    Sequencia = 5,
                    Terminal = importacaoPendente.TerminalTransbordo5,
                    PedidoViagemNavio = importacaoPendente.PedidoViagemNavioTransbordo5,
                };
                repPedidoTransbordo.Inserir(pedidoTransbordo);
            }
        }

        private void ReplicarDocumentosEtapa2Carga(Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente, XMLNotaFiscal repXMLNotaFiscal, Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal, CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscaisPedido = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(importacaoPendente.CargaPedidoAntigo.Codigo);
            for (int n = 0; n < notasFiscaisPedido.Count; n++)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalNova = notasFiscaisPedido[n].XMLNotaFiscal.Clonar();
                Utilidades.Object.DefinirListasGenericasComoNulas(xmlNotaFiscalNova);
                xmlNotaFiscalNova.DataRecebimento = DateTime.Now;
                repXMLNotaFiscal.Inserir(xmlNotaFiscalNova);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalNovo = notasFiscaisPedido[n].Clonar();
                Utilidades.Object.DefinirListasGenericasComoNulas(pedidoXMLNotaFiscalNovo);
                pedidoXMLNotaFiscalNovo.XMLNotaFiscal = xmlNotaFiscalNova;
                pedidoXMLNotaFiscalNovo.CargaPedido = importacaoPendente.CargaPedido;
                repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscalNovo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTe in notasFiscaisPedido[n].CTes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe novaCargaPedidoXMLNotaFiscalCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                    novaCargaPedidoXMLNotaFiscalCTe.CargaCTe = cargaPedidoXMLNotaFiscalCTe.CargaCTe;
                    novaCargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal = pedidoXMLNotaFiscalNovo;
                    repCargaPedidoXMLNotaFiscalCTe.Inserir(novaCargaPedidoXMLNotaFiscalCTe);
                    repCargaPedidoXMLNotaFiscalCTe.Deletar(cargaPedidoXMLNotaFiscalCTe);
                }
            }
        }

        private void ReplicarValoresFrete(Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga)
        {
            cargaNova.ValorFrete = cargaAntiga.ValorFrete;
            cargaNova.ValorFreteAPagar = cargaAntiga.ValorFreteAPagar;
            cargaNova.ValorFreteEmbarcador = cargaAntiga.ValorFreteEmbarcador;
            cargaNova.ValorICMS = cargaAntiga.ValorICMS;
        }

        private void ReplicarCargaComponenteFrete(Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponenteFrete.BuscarTodosPorCarga(cargaAntiga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFreteAntigo in cargaComponentesFrete)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFreteNovo = cargaComponenteFreteAntigo.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(cargaComponenteFreteNovo);

                if (cargaComponenteFreteAntigo.CargaComplementoFrete != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFreteNovo = cargaComponenteFreteAntigo.CargaComplementoFrete.Clonar();
                    Utilidades.Object.DefinirListasGenericasComoNulas(cargaComplementoFreteNovo);

                    cargaComplementoFreteNovo.Carga = cargaNova;
                    cargaComplementoFreteNovo.SolicitacaoCredito = DuplicarSolicitacaoCredito(cargaComponenteFreteAntigo.CargaComplementoFrete.SolicitacaoCredito, cargaNova, unitOfWork);
                    repCargaComplementoFrete.Inserir(cargaComplementoFreteNovo);

                    cargaComponenteFreteNovo.CargaComplementoFrete = cargaComplementoFreteNovo;
                }

                cargaComponenteFreteNovo.Carga = cargaNova;
                repCargaComponenteFrete.Inserir(cargaComponenteFreteNovo);
            }
        }

        private Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito DuplicarSolicitacaoCredito(Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCreditoAntiga, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, Repositorio.UnitOfWork unitOfWork)
        {
            if (solicitacaoCreditoAntiga == null)
                return null;

            Repositorio.Embarcador.Creditos.SolicitacaoCredito repSolicitacaoCredito = new Repositorio.Embarcador.Creditos.SolicitacaoCredito(unitOfWork);

            Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCreditoNova = solicitacaoCreditoAntiga.Clonar();
            Utilidades.Object.DefinirListasGenericasComoNulas(solicitacaoCreditoNova);

            solicitacaoCreditoNova.Carga = cargaNova;
            repSolicitacaoCredito.Inserir(solicitacaoCreditoNova);

            return solicitacaoCreditoNova;
        }

        private void ReplicarDadosCTe(Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente, CargaCTe repCargaCTe, ConhecimentoDeTransporteEletronico repCTe, PedidoViagemNavioSchedule repPedidoNavioSchedule)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctes = repCargaCTe.BuscarCargaCTePorCarga(importacaoPendente.CargaPedidoAntigo?.Carga.Codigo ?? 0);

            foreach (var cte in ctes)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe camposOriginaisCTe = PreencherObjetoCamposOriginais(cte.CTe);

                cte.CTe.Initialize();

                if (!string.IsNullOrWhiteSpace(importacaoPendente.BookingNovo))
                    cte.CTe.NumeroBooking = importacaoPendente.BookingNovo;
                if (importacaoPendente.PedidoViagemNavio.Navio != null)
                    cte.CTe.Navio = importacaoPendente.PedidoViagemNavio.Navio;
                if (importacaoPendente.PedidoViagemNavio != null)
                {
                    cte.CTe.Viagem = importacaoPendente.PedidoViagemNavio;
                    cte.CTe.NumeroViagem = cte.CTe.Viagem?.NumeroViagem.ToString("D") ?? "";
                    cte.CTe.Direcao = cte.CTe.Viagem != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodalHelper.ObterAbreviacao(cte.CTe.Viagem.DirecaoViagemMultimodal) : "";
                }
                if (importacaoPendente.PortoOrigem != null)
                    cte.CTe.PortoOrigem = importacaoPendente.PortoOrigem;
                if (importacaoPendente.PortoDestino != null)
                    cte.CTe.PortoDestino = importacaoPendente.PortoDestino;
                if (importacaoPendente.TerminalOrigem != null)
                    cte.CTe.TerminalOrigem = importacaoPendente.TerminalOrigem;
                if (importacaoPendente.TerminalDestino != null)
                    cte.CTe.TerminalDestino = importacaoPendente.TerminalDestino;
                if (importacaoPendente.PortoTransbordo1 != null)
                    cte.CTe.PortoPassagemUm = importacaoPendente.PortoTransbordo1;
                if (importacaoPendente.PedidoViagemNavioTransbordo1 != null)
                    cte.CTe.ViagemPassagemUm = importacaoPendente.PedidoViagemNavioTransbordo1;
                if (importacaoPendente.PortoTransbordo2 != null)
                    cte.CTe.PortoPassagemDois = importacaoPendente.PortoTransbordo2;
                if (importacaoPendente.PedidoViagemNavioTransbordo2 != null)
                    cte.CTe.ViagemPassagemDois = importacaoPendente.PedidoViagemNavioTransbordo2;
                if (importacaoPendente.PortoTransbordo3 != null)
                    cte.CTe.PortoPassagemTres = importacaoPendente.PortoTransbordo3;
                if (importacaoPendente.PedidoViagemNavioTransbordo3 != null)
                    cte.CTe.ViagemPassagemTres = importacaoPendente.PedidoViagemNavioTransbordo3;
                if (importacaoPendente.PortoTransbordo4 != null)
                    cte.CTe.PortoPassagemQuatro = importacaoPendente.PortoTransbordo4;
                if (importacaoPendente.PedidoViagemNavioTransbordo4 != null)
                    cte.CTe.ViagemPassagemQuatro = importacaoPendente.PedidoViagemNavioTransbordo4;
                if (importacaoPendente.PortoTransbordo5 != null)
                    cte.CTe.PortoPassagemCinco = importacaoPendente.PortoTransbordo5;
                if (importacaoPendente.PedidoViagemNavioTransbordo5 != null)
                    cte.CTe.ViagemPassagemCinco = importacaoPendente.PedidoViagemNavioTransbordo5;

                cte.CTe.ObservacoesGerais += "Alteração de dados via rolagem: ";
                if (cte.CTe.Viagem != null)
                    cte.CTe.ObservacoesGerais += "Viagem: " + cte.CTe.Viagem.Descricao + " ";
                if (cte.CTe.PortoOrigem != null)
                    cte.CTe.ObservacoesGerais += "Porto Origem: " + cte.CTe.PortoOrigem.Descricao + " ";
                if (cte.CTe.PortoDestino != null)
                    cte.CTe.ObservacoesGerais += "Porto Destino: " + cte.CTe.PortoDestino.Descricao + " ";
                if (cte.CTe.TerminalOrigem != null)
                    cte.CTe.ObservacoesGerais += "Terminal Origem: " + cte.CTe.TerminalOrigem.Descricao + " ";
                if (cte.CTe.TerminalDestino != null)
                    cte.CTe.ObservacoesGerais += "Terminal Destino: " + cte.CTe.TerminalDestino.Descricao + " ";
                if (cte.CTe.PortoPassagemUm != null)
                    cte.CTe.ObservacoesGerais += "Porto Passagem 1: " + cte.CTe.PortoPassagemUm.Descricao + " ";
                if (cte.CTe.PortoPassagemDois != null)
                    cte.CTe.ObservacoesGerais += "Porto Passagem 2: " + cte.CTe.PortoPassagemDois.Descricao + " ";
                if (cte.CTe.PortoPassagemTres != null)
                    cte.CTe.ObservacoesGerais += "Porto Passagem 3: " + cte.CTe.PortoPassagemTres.Descricao + " ";
                if (cte.CTe.PortoPassagemQuatro != null)
                    cte.CTe.ObservacoesGerais += "Porto Passagem 4: " + cte.CTe.PortoPassagemQuatro.Descricao + " ";
                if (cte.CTe.PortoPassagemCinco != null)
                    cte.CTe.ObservacoesGerais += "Porto Passagem 5: " + cte.CTe.PortoPassagemCinco.Descricao + " ";

                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule pedidoViagemNavioSchedule = repPedidoNavioSchedule.BuscarPorCodigo(repCTe.BuscarCodigoSchedule(cte.CTe.ViagemPassagemCinco?.Codigo ?? 0, cte.CTe.ViagemPassagemQuatro?.Codigo ?? 0, cte.CTe.ViagemPassagemTres?.Codigo ?? 0, cte.CTe.ViagemPassagemDois?.Codigo ?? 0, cte.CTe.ViagemPassagemUm?.Codigo ?? 0, cte.CTe.Viagem?.Codigo ?? 0, cte.CTe.PortoDestino?.Codigo ?? 0, cte.CTe.TerminalDestino?.Codigo ?? 0));

                if (pedidoViagemNavioSchedule != null)
                    cte.CTe.PedidoViagemNavioSchedule = pedidoViagemNavioSchedule;

                cte.Carga = importacaoPendente.CargaPedido.Carga;

                repCTe.Atualizar(cte.CTe, _auditado);
                repCargaCTe.Atualizar(cte);

                List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> camposCartaCorrecao = GerarCamposCartaCorrecao(importacaoPendente, camposOriginaisCTe);
                if (camposCartaCorrecao.Count > 0)
                    GerarCartaCorrecao(cte.CTe, camposCartaCorrecao, camposOriginaisCTe);

                AtualizarDadosFaturasCTe(importacaoPendente, cte.CTe);
                ReenviarIntegracaoFaturaEMP(cte.CTe);
            }
        }

        private void AtualizarDadosFaturasCTe(Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Fatura.Fatura> faturasCTe = repFatura.BuscarFaturasPorNumeroCTeSituacoes(cte.Codigo);

            foreach (Dominio.Entidades.Embarcador.Fatura.Fatura fatura in faturasCTe)
            {
                if (fatura.PedidoViagemNavio != null && importacaoPendente.PedidoViagemNavio != null)
                    fatura.PedidoViagemNavio = importacaoPendente.PedidoViagemNavio;

                if (!string.IsNullOrWhiteSpace(fatura.NumeroBooking) && !string.IsNullOrWhiteSpace(importacaoPendente.BookingNovo))
                    fatura.NumeroBooking = importacaoPendente.BookingNovo;

                fatura.RolagemCarga = true;

                repFatura.Atualizar(fatura);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe PreencherObjetoCamposOriginais(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe()
            {
                CodigoViagem = cte.Viagem?.Codigo ?? 0,
                CodigoNavio = cte.Navio?.Codigo ?? 0,
                NumeroBooking = cte.NumeroBooking,
                CodigoPortoPassagemUm = cte.PortoPassagemUm?.Codigo ?? 0,
                CodigoPortoPassagemDois = cte.PortoPassagemDois?.Codigo ?? 0,
                CodigoPortoPassagemTres = cte.PortoPassagemTres?.Codigo ?? 0,
                CodigoPortoPassagemQuatro = cte.PortoPassagemQuatro?.Codigo ?? 0,
                CodigoPortoPassagemCinco = cte.PortoPassagemCinco?.Codigo ?? 0,
                ViagemPassagemUm = cte.ViagemPassagemUm?.Codigo ?? 0,
                ViagemPassagemDois = cte.ViagemPassagemDois?.Codigo ?? 0,
                ViagemPassagemTres = cte.ViagemPassagemTres?.Codigo ?? 0,
                ViagemPassagemQuatro = cte.ViagemPassagemQuatro?.Codigo ?? 0,
                ViagemPassagemCinco = cte.ViagemPassagemCinco?.Codigo ?? 0,
            };
        }

        private List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> GerarCamposCartaCorrecao(Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente, Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe camposOriginaisCTe, bool cargaSVM = false)
        {
            int sequencial = 0;
            List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> listaCampos = new List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe>();

            if (!cargaSVM)
            {
                if (camposOriginaisCTe.CodigoNavio != (importacaoPendente.PedidoViagemNavio?.Navio?.Codigo ?? 0))
                    listaCampos.Add(AdicionarCampoCCe("Observação (navio)", "xObs", "compl", TipoCampoCCe.Texto, 800, 0, 0, false, importacaoPendente.PedidoViagemNavio?.Navio?.Descricao, TipoCampoCCeAutomatico.Navio, ref sequencial));
            }

            if (camposOriginaisCTe.CodigoViagem != (importacaoPendente.PedidoViagemNavio?.Codigo ?? 0))
                listaCampos.Add(AdicionarCampoCCe("Observação Cont (navio)", "xTexto", "ObsCont", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PedidoViagemNavio?.Descricao, TipoCampoCCeAutomatico.Nenhum, ref sequencial));

            if (!string.IsNullOrWhiteSpace(camposOriginaisCTe.NumeroBooking) && !camposOriginaisCTe.NumeroBooking.Equals(importacaoPendente.BookingNovo))
                listaCampos.Add(AdicionarCampoCCe("Observação Cont( Booking)", "xTexto", "ObsCont", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.BookingNovo, TipoCampoCCeAutomatico.Booking, ref sequencial));

            if (camposOriginaisCTe.CodigoPortoPassagemUm != 0 && camposOriginaisCTe.CodigoPortoPassagemUm != (importacaoPendente.PortoTransbordo1?.Codigo ?? 0))
                listaCampos.Add(AdicionarCampoCCe("Porto Passagem", "xPass", "pass", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PortoTransbordo1?.Descricao ?? string.Empty, TipoCampoCCeAutomatico.PortoPassagem, ref sequencial));

            if (camposOriginaisCTe.CodigoPortoPassagemDois != 0 && camposOriginaisCTe.CodigoPortoPassagemDois != (importacaoPendente.PortoTransbordo2?.Codigo ?? 0))
                listaCampos.Add(AdicionarCampoCCe("Porto Passagem", "xPass", "pass", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PortoTransbordo2?.Descricao ?? string.Empty, TipoCampoCCeAutomatico.PortoPassagem2, ref sequencial));

            if (camposOriginaisCTe.CodigoPortoPassagemTres != 0 && camposOriginaisCTe.CodigoPortoPassagemTres != (importacaoPendente.PortoTransbordo3?.Codigo ?? 0))
                listaCampos.Add(AdicionarCampoCCe("Porto Passagem", "xPass", "pass", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PortoTransbordo3?.Descricao ?? string.Empty, TipoCampoCCeAutomatico.PortoPassagem3, ref sequencial));

            if (camposOriginaisCTe.CodigoPortoPassagemQuatro != 0 && camposOriginaisCTe.CodigoPortoPassagemQuatro != (importacaoPendente.PortoTransbordo4?.Codigo ?? 0))
                listaCampos.Add(AdicionarCampoCCe("Porto Passagem", "xPass", "pass", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PortoTransbordo4?.Descricao ?? string.Empty, TipoCampoCCeAutomatico.PortoPassagem4, ref sequencial));

            if (camposOriginaisCTe.CodigoPortoPassagemCinco != 0 && camposOriginaisCTe.CodigoPortoPassagemCinco != (importacaoPendente.PortoTransbordo5?.Codigo ?? 0))
                listaCampos.Add(AdicionarCampoCCe("Porto Passagem", "xPass", "pass", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PortoTransbordo5?.Descricao ?? string.Empty, TipoCampoCCeAutomatico.PortoPassagem5, ref sequencial));

            //if (camposOriginaisCTe.ViagemPassagemUm != (importacaoPendente.PedidoViagemNavioTransbordo1?.Codigo ?? 0))
            //    listaCampos.Add(AdicionarCampoCCe("Observação Cont (navio)", "xTexto", "ObsCont", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PedidoViagemNavioTransbordo1?.Descricao, TipoCampoCCeAutomatico.Nenhum, ref sequencial));

            //if (camposOriginaisCTe.ViagemPassagemDois != (importacaoPendente.PedidoViagemNavioTransbordo2?.Codigo ?? 0))
            //    listaCampos.Add(AdicionarCampoCCe("Observação Cont (navio)", "xTexto", "ObsCont", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PedidoViagemNavioTransbordo2?.Descricao, TipoCampoCCeAutomatico.Nenhum, ref sequencial));

            //if (camposOriginaisCTe.ViagemPassagemTres != (importacaoPendente.PedidoViagemNavioTransbordo3?.Codigo ?? 0))
            //    listaCampos.Add(AdicionarCampoCCe("Observação Cont (navio)", "xTexto", "ObsCont", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PedidoViagemNavioTransbordo3?.Descricao, TipoCampoCCeAutomatico.Nenhum, ref sequencial));

            //if (camposOriginaisCTe.ViagemPassagemQuatro != (importacaoPendente.PedidoViagemNavioTransbordo4?.Codigo ?? 0))
            //    listaCampos.Add(AdicionarCampoCCe("Observação Cont (navio)", "xTexto", "ObsCont", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PedidoViagemNavioTransbordo4?.Descricao, TipoCampoCCeAutomatico.Nenhum, ref sequencial));

            //if (camposOriginaisCTe.ViagemPassagemCinco != (importacaoPendente.PedidoViagemNavioTransbordo5?.Codigo ?? 0))
            //    listaCampos.Add(AdicionarCampoCCe("Observação Cont (navio)", "xTexto", "ObsCont", TipoCampoCCe.Texto, 500, 0, 0, true, importacaoPendente.PedidoViagemNavioTransbordo5?.Descricao, TipoCampoCCeAutomatico.Nenhum, ref sequencial));

            return listaCampos;
        }

        private Dominio.ObjetosDeValor.WebService.CTe.CampoCCe AdicionarCampoCCe(string descricao, string nomeCampo, string grupoCampo, TipoCampoCCe tipoCampo, int qtdeCaracteres, int qtdedecimais, int qtdeInteiros, bool repeticao, string valorAlterado, TipoCampoCCeAutomatico tipoCampoAutomatico, ref int sequencial)
        {
            sequencial++;

            return new Dominio.ObjetosDeValor.WebService.CTe.CampoCCe()
            {
                Descricao = descricao,
                GrupoCampo = grupoCampo,
                NomeCampo = nomeCampo,
                TipoCampo = tipoCampo,
                QuantidadeCaracteres = qtdeCaracteres,
                QuantidadeDecimais = qtdedecimais,
                QuantidadeInteiros = qtdeInteiros,
                IndicadorRepeticao = repeticao,
                NumeroItemAlterado = sequencial,
                ValorAlterado = valorAlterado,
                TipoCampoCCeAutomatico = tipoCampoAutomatico,
            };
        }

        public void GerarCartaCorrecao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> camposCCe, Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe camposCTeAnterior = null)
        {
            bool gerarCCe = false;
            bool buscouViagem = false;

            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(_unitOfWork);
            Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(_unitOfWork);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new PedidoViagemNavio(_unitOfWork);

            Servicos.CCe svcCCe = new Servicos.CCe(_unitOfWork);

            Dominio.Entidades.CartaDeCorrecaoEletronica cce = new Dominio.Entidades.CartaDeCorrecaoEletronica()
            {
                CTe = cte,
                DataEmissao = DateTime.Now,
                Log = "Gerado via integração WS",
                Status = Dominio.Enumeradores.StatusCCe.Pendente,
                NumeroSequencialEvento = (repCCe.BuscarUltimoNumeroSequencial(cte.Codigo) + 1)
            };

            repCCe.Inserir(cce);

            foreach (var item in camposCCe)
            {
                if (!string.IsNullOrWhiteSpace(item.Descricao) || !string.IsNullOrWhiteSpace(item.GrupoCampo) || !string.IsNullOrWhiteSpace(item.NomeCampo))
                {
                    Dominio.Entidades.CartaDeCorrecaoEletronica ultimaCarga = repCCe.BuscarUltimaCCeAutorizadaPorCTe(cce.CTe.Codigo);

                    if (ultimaCarga != null)
                    {
                        List<Dominio.Entidades.ItemCCe> itensAnteriores = repItemCCe.BuscarPorCCe(ultimaCarga.Codigo);

                        foreach (var itemAnterior in itensAnteriores)
                        {
                            Dominio.Entidades.ItemCCe itemAnteriorCCe = new Dominio.Entidades.ItemCCe();

                            itemAnteriorCCe.CCe = cce;
                            itemAnteriorCCe.CampoAlterado = itemAnterior.CampoAlterado;
                            itemAnteriorCCe.NumeroItemAlterado = itemAnterior.NumeroItemAlterado;
                            itemAnteriorCCe.ValorAlterado = itemAnterior.ValorAlterado;

                            repItemCCe.Inserir(itemAnteriorCCe);
                        }
                    }

                    Dominio.Entidades.CampoCCe campoCCe = null;
                    if (item.TipoCampoCCeAutomatico != TipoCampoCCeAutomatico.Nenhum)
                    {
                        if (item.TipoCampoCCeAutomatico == TipoCampoCCeAutomatico.PortoPassagem2 || item.TipoCampoCCeAutomatico == TipoCampoCCeAutomatico.PortoPassagem3
                            || item.TipoCampoCCeAutomatico == TipoCampoCCeAutomatico.PortoPassagem4)
                            campoCCe = repCampoCCe.BuscarPorTipoCampoCCeAutomatico(TipoCampoCCeAutomatico.PortoPassagem);
                        else
                            campoCCe = repCampoCCe.BuscarPorTipoCampoCCeAutomatico(item.TipoCampoCCeAutomatico);
                    }
                    else
                        campoCCe = repCampoCCe.BuscarPorNomeCampoGrupo(item.NomeCampo, item.GrupoCampo);

                    if (campoCCe == null)
                        throw new ServicoException($"Não foi possível gerar uma carta de correção para o CT-e {cce.CTe.Numero} porque não existe um campo cadastrado para o o item {item.NomeCampo}.");

                    if (campoCCe == null)
                    {
                        campoCCe = new Dominio.Entidades.CampoCCe()
                        {
                            Descricao = item.Descricao,
                            GrupoCampo = item.GrupoCampo,
                            IndicadorRepeticao = item.IndicadorRepeticao,
                            NomeCampo = item.NomeCampo,
                            Status = "A",
                            TipoCampo = item.TipoCampo,
                            QuantidadeCaracteres = item.QuantidadeCaracteres,
                            QuantidadeDecimais = item.QuantidadeDecimais,
                            QuantidadeInteiros = item.QuantidadeInteiros
                        };
                        repCampoCCe.Inserir(campoCCe);

                    }

                    Dominio.Entidades.ItemCCe itemCCe = new Dominio.Entidades.ItemCCe();
                    itemCCe.CCe = cce;
                    itemCCe.CampoAlterado = campoCCe;
                    itemCCe.NumeroItemAlterado = item.NumeroItemAlterado;

                    string valorAnterior = "";

                    if (item.TipoCampoCCeAutomatico == TipoCampoCCeAutomatico.Booking)
                    {
                        if (camposCTeAnterior != null && !string.IsNullOrWhiteSpace(camposCTeAnterior.NumeroBooking))
                            valorAnterior = camposCTeAnterior.NumeroBooking;
                        else
                            valorAnterior = "";

                        valorAnterior = "Onde se lê " + valorAnterior;
                    }
                    else if (item.TipoCampoCCeAutomatico == TipoCampoCCeAutomatico.PortoPassagem)
                    {
                        if (camposCTeAnterior != null && camposCTeAnterior.CodigoPortoPassagemUm > 0)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Porto portoPassagem = repPorto.BuscarPorCodigo(camposCTeAnterior.CodigoPortoPassagemUm);
                            valorAnterior = portoPassagem?.Descricao ?? "";
                            if (!string.IsNullOrWhiteSpace(valorAnterior))
                                valorAnterior = "Onde se lê " + valorAnterior;
                        }
                    }
                    else if (item.TipoCampoCCeAutomatico == TipoCampoCCeAutomatico.PortoPassagem2)
                    {
                        if (camposCTeAnterior != null && camposCTeAnterior.CodigoPortoPassagemDois > 0)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Porto portoPassagem = repPorto.BuscarPorCodigo(camposCTeAnterior.CodigoPortoPassagemDois);
                            valorAnterior += portoPassagem?.Descricao ?? "";
                            if (!string.IsNullOrWhiteSpace(valorAnterior))
                                valorAnterior = "Onde se lê " + valorAnterior;
                        }

                    }
                    else if (item.TipoCampoCCeAutomatico == TipoCampoCCeAutomatico.PortoPassagem3)
                    {
                        if (camposCTeAnterior != null && camposCTeAnterior.CodigoPortoPassagemTres > 0)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Porto portoPassagem = repPorto.BuscarPorCodigo(camposCTeAnterior.CodigoPortoPassagemTres);
                            valorAnterior += portoPassagem?.Descricao ?? "";
                            if (!string.IsNullOrWhiteSpace(valorAnterior))
                                valorAnterior = "Onde se lê " + valorAnterior;
                        }

                    }
                    else if (item.TipoCampoCCeAutomatico == TipoCampoCCeAutomatico.PortoPassagem4)
                    {
                        if (camposCTeAnterior != null && camposCTeAnterior.CodigoPortoPassagemQuatro > 0)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Porto portoPassagem = repPorto.BuscarPorCodigo(camposCTeAnterior.CodigoPortoPassagemQuatro);
                            valorAnterior += portoPassagem?.Descricao ?? "";
                            if (!string.IsNullOrWhiteSpace(valorAnterior))
                                valorAnterior = "Onde se lê " + valorAnterior;
                        }

                    }
                    else if (item.TipoCampoCCeAutomatico == TipoCampoCCeAutomatico.Navio)
                    {
                        if (camposCTeAnterior != null && camposCTeAnterior.CodigoNavio > 0)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Navio navio = repNavio.BuscarPorCodigo(camposCTeAnterior.CodigoNavio);
                            valorAnterior = navio?.Descricao ?? "";
                        }
                        else if (cce.CTe.Navio != null)
                            valorAnterior = cce.CTe.Navio?.Descricao ?? "";
                        else
                            valorAnterior = cce.CTe.Viagem?.Navio?.Descricao ?? "";

                        valorAnterior = "Onde se lê " + valorAnterior;
                    }
                    else if (campoCCe.Descricao.ToLower().Contains("navio"))
                    {
                        if (camposCTeAnterior != null && camposCTeAnterior.CodigoViagem > 0 && !buscouViagem)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = repPedidoViagemNavio.BuscarPorCodigo(camposCTeAnterior.CodigoViagem);
                            valorAnterior = viagem?.Descricao ?? "";
                            buscouViagem = true;
                        }
                        else if (cce.CTe.Navio != null && !string.IsNullOrWhiteSpace(cce.CTe.NumeroViagem) && !string.IsNullOrWhiteSpace(cce.CTe.Direcao))
                        {
                            valorAnterior = cce.CTe.Navio?.Descricao ?? "";
                            valorAnterior += ("/" + cce.CTe.NumeroViagem + cce.CTe.Direcao);
                        }
                        else
                            valorAnterior = cce.CTe.Viagem?.Descricao ?? "";

                        valorAnterior = "Onde se lê: " + valorAnterior;
                    }

                    if (!string.IsNullOrWhiteSpace(valorAnterior))
                        valorAnterior += " > Leia-se: " + item.ValorAlterado;
                    else
                        valorAnterior = item.ValorAlterado;

                    item.ValorAlterado = valorAnterior;
                    itemCCe.ValorAlterado = item.ValorAlterado;

                    repItemCCe.Inserir(itemCCe);

                    gerarCCe = true;
                }
            }

            if (gerarCCe)
                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTe.EmitirCCe(cce, cce.CTe.Empresa.Codigo, _unitOfWork))
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cce.CTe, "Gerou carta de correção via rolagem de container.", _unitOfWork);
                else
                    throw new ServicoException($"Não foi possível gerar carta correção para o CT-e {cce.CTe.Numero}.");
        }

        private void ReplicarCargaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> cargaIntegracoes = repCargaIntegracao.BuscarPorCarga(cargaAntiga.Codigo);


            foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao in cargaIntegracoes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracaoNova = cargaIntegracao.Clonar();
                Utilidades.Object.DefinirListasGenericasComoNulas(cargaIntegracaoNova);

                cargaIntegracaoNova.Carga = cargaNova;
                repCargaIntegracao.Inserir(cargaIntegracaoNova);
            }
        }

        private void AtualizarDadosCargasSVM(Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente, Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido, Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasSVM = repCargaPedido.BuscarCargasSVMBookingContainerEncerradaTransporte(importacaoPendente.BookingAnterior, importacaoPendente.Container.Numero);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaSVM in cargasSVM)
            {
                AtualizarDadosPedido(cargaSVM, importacaoPendente, repPedidoTransbordo);
                AtualizarDadosCTe(cargaSVM, importacaoPendente);
            }
        }

        private void AtualizarDadosPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaSVM, Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente, Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo)
        {
            if (!string.IsNullOrWhiteSpace(importacaoPendente.BookingNovo))
                cargaSVM.Pedido.NumeroBooking = importacaoPendente.BookingNovo;

            AdicionarTransbordosPedidos(importacaoPendente, repPedidoTransbordo, cargaSVM.Pedido);
        }

        private void AtualizarDadosCTe(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaSVM, Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(_unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCTe.BuscarCTePorCarga(cargaSVM.Carga.Codigo);

            foreach (var cte in ctes)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.ValoresOriginaisCTe camposOriginaisCTe = PreencherObjetoCamposOriginais(cte);

                cte.Initialize();

                if (!string.IsNullOrWhiteSpace(importacaoPendente.BookingNovo))
                    cte.NumeroBooking = importacaoPendente.BookingNovo;
                if (importacaoPendente.PortoOrigem != null)
                    cte.PortoOrigem = importacaoPendente.PortoOrigem;
                if (importacaoPendente.PortoDestino != null)
                    cte.PortoDestino = importacaoPendente.PortoDestino;
                if (importacaoPendente.TerminalOrigem != null)
                    cte.TerminalOrigem = importacaoPendente.TerminalOrigem;
                if (importacaoPendente.TerminalDestino != null)
                    cte.TerminalDestino = importacaoPendente.TerminalDestino;
                if (importacaoPendente.PedidoViagemNavio != null)
                    cte.Viagem = importacaoPendente.PedidoViagemNavio;

                cte.PortoPassagemUm = importacaoPendente.PortoTransbordo1;
                cte.ViagemPassagemUm = importacaoPendente.PedidoViagemNavioTransbordo1;
                cte.PortoPassagemDois = importacaoPendente.PortoTransbordo2;
                cte.ViagemPassagemDois = importacaoPendente.PedidoViagemNavioTransbordo2;
                cte.PortoPassagemTres = importacaoPendente.PortoTransbordo3;
                cte.ViagemPassagemTres = importacaoPendente.PedidoViagemNavioTransbordo3;
                cte.PortoPassagemQuatro = importacaoPendente.PortoTransbordo4;
                cte.ViagemPassagemQuatro = importacaoPendente.PedidoViagemNavioTransbordo4;
                cte.PortoPassagemCinco = importacaoPendente.PortoTransbordo5;
                cte.ViagemPassagemCinco = importacaoPendente.PedidoViagemNavioTransbordo5;

                cte.ObservacoesGerais += "Alteração de dados da carga SVM via rolagem: ";
                if (cte.Viagem != null)
                    cte.ObservacoesGerais += "Viagem: " + cte.Viagem.Descricao + " ";
                if (cte.PortoOrigem != null)
                    cte.ObservacoesGerais += "Porto Origem: " + cte.PortoOrigem.Descricao + " ";
                if (cte.PortoDestino != null)
                    cte.ObservacoesGerais += "Porto Destino: " + cte.PortoDestino.Descricao + " ";
                if (cte.TerminalOrigem != null)
                    cte.ObservacoesGerais += "Terminal Origem: " + cte.TerminalOrigem.Descricao + " ";
                if (cte.TerminalDestino != null)
                    cte.ObservacoesGerais += "Terminal Destino: " + cte.TerminalDestino.Descricao + " ";
                if (cte.PortoPassagemUm != null)
                    cte.ObservacoesGerais += "Porto Passagem 1: " + cte.PortoPassagemUm.Descricao + " ";
                if (cte.PortoPassagemDois != null)
                    cte.ObservacoesGerais += "Porto Passagem 2: " + cte.PortoPassagemDois.Descricao + " ";
                if (cte.PortoPassagemTres != null)
                    cte.ObservacoesGerais += "Porto Passagem 3: " + cte.PortoPassagemTres.Descricao + " ";
                if (cte.PortoPassagemQuatro != null)
                    cte.ObservacoesGerais += "Porto Passagem 4: " + cte.PortoPassagemQuatro.Descricao + " ";
                if (cte.PortoPassagemCinco != null)
                    cte.ObservacoesGerais += "Porto Passagem 5: " + cte.PortoPassagemCinco.Descricao + " ";

                cte.PedidoViagemNavioSchedule = repPedidoNavioSchedule.BuscarPorCodigo(repCTe.BuscarCodigoSchedule(cte.ViagemPassagemCinco?.Codigo ?? 0, cte.ViagemPassagemQuatro?.Codigo ?? 0, cte.ViagemPassagemTres?.Codigo ?? 0, cte.ViagemPassagemDois?.Codigo ?? 0, cte.ViagemPassagemUm?.Codigo ?? 0, cte.Viagem?.Codigo ?? 0, cte.PortoDestino?.Codigo ?? 0, cte.TerminalDestino?.Codigo ?? 0));

                repCTe.Atualizar(cte, _auditado);

                List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> camposCartaCorrecao = GerarCamposCartaCorrecao(importacaoPendente, camposOriginaisCTe, true);
                if (camposCartaCorrecao.Count > 0)
                    GerarCartaCorrecao(cte, camposCartaCorrecao, camposOriginaisCTe);
            }
        }

        private void AdicionarIntegracoesCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP);

            if (tipoIntegracao == null)
                return;

            AdicionarCargaIntegracaoEMP(cargaNova, tipoIntegracao);
            AdicionarCargaCargaIntegracaoEMP(cargaNova, tipoIntegracao);
        }

        private void AdicionarCargaIntegracaoEMP(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(_unitOfWork);

            if (repCargaIntegracao.ExistePorTipoIntegracao(carga.Codigo, tipoIntegracao.Codigo))
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracao()
            {
                Carga = carga,
                TipoIntegracao = tipoIntegracao
            };
            repCargaIntegracao.Inserir(cargaIntegracao);
        }

        public void AdicionarCargaCargaIntegracaoEMP(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            if (repCargaIntegracao.ExistePorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao();

            cargaIntegracao.Carga = carga;
            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas = 0;
            cargaIntegracao.ProblemaIntegracao = "";
            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            cargaIntegracao.TipoIntegracao = tipoIntegracao;
            cargaIntegracao.IntegracaoColeta = false;
            cargaIntegracao.RealizarIntegracaoCompleta = integracaoIntercab?.AtivarIntegracaoCargaAtualParaNovo ?? false;
            cargaIntegracao.IntegracaoFilialEmissora = false;
            cargaIntegracao.FinalizarCargaAnterior = false;
            repCargaIntegracao.Inserir(cargaIntegracao);

            return;
        }

        #endregion
    }
}
