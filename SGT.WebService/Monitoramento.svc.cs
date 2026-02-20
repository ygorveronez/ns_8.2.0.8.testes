using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Monitoramento(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IMonitoramento
    {
        #region Métodos Globais

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga>> BuscarCargas(string dataInicial, int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                DateTime dataInicialPesquisa = dataInicial.ToDateTime();

                if (dataInicialPesquisa == DateTime.MinValue)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga>>.CriarRetornoDadosInvalidos("A data inicial deve ser informada");

                if (limite > 100)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga>>.CriarRetornoDadosInvalidos("O limite não pode ser superior a 100");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                int totalRegistros = repositorioCarga.ContarConsultaWebServiceMonitoramento(dataInicialPesquisa);
                List<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga> cargasRetornar = new List<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga>();

                if (totalRegistros > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                    Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo repositorioContratoFreteTransportadorVeiculo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo(unitOfWork);
                    Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                    Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

                    Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.ConsultarWebServiceMonitoramento(dataInicialPesquisa, (int)inicio, (int)limite);
                    List<int> codigosCarga = (from o in cargas select o.Codigo).ToList();
                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentoCargas = repositorioMonitoramento.BuscarPorCargas(codigosCarga);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCargasSemFetch(codigosCarga);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> entregasPedido = repositorioCargaEntregaPedido.BuscarPorCargas(codigosCarga);
                    List<(int Transportador, int Veiculo)> contratosFreteTransportadorVeiculo = repositorioContratoFreteTransportadorVeiculo.BuscarPorCargasComContratoAtivoParaTransportadorEVeiculo(codigosCarga);
                    List<(int Carga, DateTime? DataEntradaRaio)> datasEntradaRaio = repositorioCargaEntrega.BuscarDatasEntradaRaioPorCargas(codigosCarga);
                    List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordemEmbarques = repOrdemEmbarque.BuscarAtivasPorCargas(codigosCarga);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCarga);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamento = repCargaJanelaDescarregamento.BuscarPorCarga(codigosCarga);
                    List<(int CodigoCarga, int CodigoCargaPedido, string CodigoProdutoEmbarcador, string DescricaoProduto, decimal Quantidade)> produtos = repCargaPedidoProduto.BuscarDadosPorCargas(codigosCarga);

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                        cargasRetornar.Add(ObterCarga(carga, monitoramentoCargas, cargasPedido, entregasPedido, datasEntradaRaio, contratosFreteTransportadorVeiculo, ordemEmbarques, listaCargaJanelaCarregamento, listaCargaJanelaDescarregamento, produtos));
                }

                Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga> paginacaoCargas = new Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga>()
                {
                    NumeroTotalDeRegistro = totalRegistros,
                    Itens = cargasRetornar
                };

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga>>.CriarRetornoSucesso(paginacaoCargas);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as cargas");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SolicitarReenvioEventosCarga(string numeroCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (string.IsNullOrWhiteSpace(numeroCarga))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Número da carga precisa ser informado");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> listaPedidosOcorrenciaColetaEntrega = repPedidoOcorrenciaColetaEntrega.BuscarPorCodigoCargaEmbarcador(numeroCarga);

                if (listaPedidosOcorrenciaColetaEntrega.Count == 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foram encontrados eventos para essa carga");

                List<int> codigos = listaPedidosOcorrenciaColetaEntrega.Select(o => o.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao> pedidosIntegracao = repPedidoIntegracao.BuscarIntegracoesCargaPorCodigosColeta(codigos);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao> cargaEntregaEventoIntegracoes = repCargaEntregaEventoIntegracao.BuscarPorCodigoEmbarcadorCarga(numeroCarga);

                if (pedidosIntegracao.Count == 0 && cargaEntregaEventoIntegracoes.Count == 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não há eventos pendentes de integração para esta carga");

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoIntegracao in pedidosIntegracao)
                {
                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    repPedidoIntegracao.Atualizar(pedidoIntegracao);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao cargaEntregaEvento in cargaEntregaEventoIntegracoes)
                {
                    cargaEntregaEvento.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    repCargaEntregaEventoIntegracao.Atualizar(cargaEntregaEvento);
                }

                return Retorno<bool>.CriarRetornoSucesso(true, "Eventos da carga reenviados para integração");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoDadosInvalidos("Ocorreu uma falha ao consultar a carga");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento>> BuscarHistoricoPosicoesPorCarga(string numeroCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (string.IsNullOrWhiteSpace(numeroCarga))
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento>>.CriarRetornoDadosInvalidos("É óbrigatório informar o Número da Carga.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Logistica.Posicao repositorioPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigoEmbarcador(numeroCarga);

                if (carga == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento>>.CriarRetornoDadosInvalidos("Carga não localizada.");

                if (carga.Veiculo == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento>>.CriarRetornoDadosInvalidos("Carga sem Veículo para realizar Consulta.");

                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarPorCodigoCarga(carga.Codigo);

                if (monitoramento == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento>>.CriarRetornoDadosInvalidos("Carga sem Monitoramento para realizar Consulta.");


                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = repositorioPosicao.BuscarWaypointsPorMonitoramentoVeiculo(monitoramento.Codigo, null, RestringirDataInicial(DateTime.Now.AddDays(-30), monitoramento), RestringirDataFinal(DateTime.Now, monitoramento));

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXMLNotaFiscal = repositorioCargaPedido.BuscarCargaPedidoEXMLNotaFiscal(cargaPedido.Select(o => o.Codigo).ToList());

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega = repositorioCargaEntrega.BuscarEntregasPorCarga(carga.Codigo);

                List<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento> listaHistoricoPosicoesCargaMonitoramento = new List<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento>
                {
                    PreencherHistoricoPosicoesCargaMonitoramento(carga, posicoes, listaPedidoXMLNotaFiscal, listaCargaEntrega, unitOfWork )
                };

                Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento> paginacaoHistoricoPosicoesCargaMonitoramento = new Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento>()
                {
                    NumeroTotalDeRegistro = 1,
                    Itens = listaHistoricoPosicoesCargaMonitoramento
                };

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento>>.CriarRetornoSucesso(paginacaoHistoricoPosicoesCargaMonitoramento);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento>>.CriarRetornoDadosInvalidos("Ocorreu uma falha ao consultar a Carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.VeiculoDedicado> ConsultarVeiculoDedicado(string transportador, string placa)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.ObjetosDeValor.WebService.Monitoramento.VeiculoDedicado veiculoDedicado = new Dominio.ObjetosDeValor.WebService.Monitoramento.VeiculoDedicado();
            veiculoDedicado.Dedicado = false;

            try
            {
                Dominio.ObjetosDeValor.WebService.Monitoramento.VeiculoDedicado dedicado = new Dominio.ObjetosDeValor.WebService.Monitoramento.VeiculoDedicado();
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContrato = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                List<int> codEmpresas = new List<int>();

                Dominio.Entidades.Empresa empresaParametro = repEmpresa.BuscarPorCNPJ(transportador);

                codEmpresas.Add(empresaParametro.Codigo);

                foreach(Dominio.Entidades.Empresa matriz in empresaParametro.Matriz)
                {
                    if (!codEmpresas.Contains(matriz.Codigo))
                        codEmpresas.Add(matriz.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratosEmpresa = repContrato.BuscarAtivoPorTransportadores(codEmpresas);

                foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato in contratosEmpresa)
                {
                    foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo veiculo in contrato.Veiculos)
                    {
                        if (veiculo.Veiculo.Placa == placa)
                            veiculoDedicado.Dedicado = true;
                    }
                }

                return Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.VeiculoDedicado>.CriarRetornoSucesso(veiculoDedicado);
            }
            catch(Exception e)
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.VeiculoDedicado>.CriarRetornoExcecao(e.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.WebService.Monitoramento.Carga ObterCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentoCargas, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> entregasPedido, List<(int Carga, DateTime? DataEntradaRaio)> datasEntradaRaio, List<(int Transportador, int Veiculo)> contratosFreteTransportadorVeiculo, List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordemEmbarques, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamento, List<(int CodigoCarga, int CodigoCargaPedido, string CodigoProdutoEmbarcador, string DescricaoProduto, decimal Quantidade)> produtos)
        {
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = (from o in monitoramentoCargas where o.Carga.Codigo == carga.Codigo && o.Veiculo != null && o.Veiculo.Codigo == (carga.Veiculo?.Codigo ?? 0) select o).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == carga.Codigo select o).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = (from o in listaCargaJanelaDescarregamento where o.Carga.Codigo == carga.Codigo select o).FirstOrDefault();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoPorCarga = (from o in cargasPedido where o.Carga.Codigo == carga.Codigo select o).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> entregasPedidoPorCarga = (from o in entregasPedido where o.CargaEntrega.Carga.Codigo == carga.Codigo select o).ToList();
            List<(int CodigoCarga, int CodigoCargaPedido, string CodigoProdutoEmbarcador, string DescricaoProduto, decimal Quantidade)> produtosPorCarga = (from o in produtos where o.CodigoCarga == carga.Codigo select o).ToList();

            bool veiculoDedicado = contratosFreteTransportadorVeiculo.Any(o => o.Transportador == (carga.Empresa?.Codigo ?? 0) && o.Veiculo == (carga.Veiculo?.Codigo ?? 0));
            DateTime? dataEntradaRaio = (from o in datasEntradaRaio where o.Carga == carga.Codigo select o.DataEntradaRaio).FirstOrDefault();
            decimal pesoBruto = cargasPedidoPorCarga.Sum(o => (decimal?)o.Pedido.PesoTotal) ?? 0m;
            string dataInicioCarregamento = "";

            if ((cargaJanelaCarregamento != null) && (cargaJanelaCarregamento.CentroCarregamento != null) && !cargaJanelaCarregamento.Excedente)
                dataInicioCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm");

            Dominio.ObjetosDeValor.WebService.Monitoramento.Carga cargaRetornar = new Dominio.ObjetosDeValor.WebService.Monitoramento.Carga()
            {
                DataChegada = dataEntradaRaio?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioCarregamento = dataInicioCarregamento,
                NumeroCarga = carga.CodigoCargaEmbarcador,
                Observacao = "",
                PesoBruto = pesoBruto,
                Protocolo = carga.Protocolo,
                StatusCarga = carga.SituacaoCarga,
                StatusMonitoramento = monitoramento?.StatusViagem?.Sigla ?? "",
                StatusMonitoramentoDescricao = monitoramento?.StatusViagem?.Descricao ?? "",
                TipoCarga = carga.TipoDeCarga?.Descricao ?? "",
                TipoFrete = carga.TipoCondicaoPagamento.HasValue ? carga.TipoCondicaoPagamento.Value.ObterDescricao() : "",
                StatusViagem = monitoramento?.StatusViagem?.Descricao ?? string.Empty,
            };

            cargaRetornar.Filial = new Dominio.ObjetosDeValor.WebService.Monitoramento.Filial()
            {
                Cnpj = carga.Filial?.CNPJ ?? "",
                Descricao = carga.Filial?.Descricao
            };

            cargaRetornar.Transportador = new Dominio.ObjetosDeValor.WebService.Monitoramento.Empresa()
            {
                Cnpj = carga.Empresa?.CNPJ ?? "",
                NomeFantasia = carga.Empresa?.NomeFantasia ?? "",
                RazaoSocial = carga.Empresa?.RazaoSocial ?? ""
            };

            cargaRetornar.Veiculo = new Dominio.ObjetosDeValor.WebService.Monitoramento.Veiculo()
            {
                Dedicado = veiculoDedicado,
                Placa = carga.Veiculo?.Placa ?? ""
            };

            if (carga.VeiculosVinculados.Count > 0)
            {
                cargaRetornar.Reboques = new List<Dominio.ObjetosDeValor.WebService.Monitoramento.Veiculo>();

                foreach (Dominio.Entidades.Veiculo veiculo in carga.VeiculosVinculados)
                {
                    Dominio.ObjetosDeValor.WebService.Monitoramento.Veiculo reboque = new Dominio.ObjetosDeValor.WebService.Monitoramento.Veiculo()
                    {
                        Dedicado = veiculoDedicado,
                        Placa = veiculo.Placa
                    };

                    cargaRetornar.Reboques.Add(reboque);
                }
            }

            cargaRetornar.Pedidos = ObterPedidos(cargasPedidoPorCarga, entregasPedidoPorCarga, ordemEmbarques, cargaJanelaDescarregamento, produtosPorCarga);

            return cargaRetornar;
        }

        private List<Dominio.ObjetosDeValor.WebService.Monitoramento.Pedido> ObterPedidos(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> entregasPedido, List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque, Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, List<(int CodigoCarga, int CodigoCargaPedido, string CodigoProdutoEmbarcador, string DescricaoProduto, decimal Quantidade)> produtosPorCarga)
        {
            List<Dominio.ObjetosDeValor.WebService.Monitoramento.Pedido> pedidos = new List<Dominio.ObjetosDeValor.WebService.Monitoramento.Pedido>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido entregaPedido = (from o in entregasPedido where o.CargaPedido.Codigo == cargaPedido.Codigo && !o.CargaEntrega.Coleta select o).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque = (from o in ordensEmbarque where o.NumeroReboque == cargaPedido.NumeroReboque && (o.Carga.Codigo == cargaPedido.Carga.Codigo || o.Carga.CargaAgrupamento?.Codigo == cargaPedido.Carga.Codigo) select o).FirstOrDefault();
                List<(int CodigoCarga, int CodigoCargaPedido, string CodigoProdutoEmbarcador, string DescricaoProduto, decimal Quantidade)> produtosPorCargaPedido = (from o in produtosPorCarga where o.CodigoCargaPedido == cargaPedido.Codigo select o).ToList();

                Dominio.ObjetosDeValor.WebService.Monitoramento.Pedido pedido = new Dominio.ObjetosDeValor.WebService.Monitoramento.Pedido()
                {
                    DataChegadaCliente = entregaPedido?.CargaEntrega?.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataInicioColeta = cargaPedido.Pedido.DataInicialColeta?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataPrevisaoEntrega = cargaPedido.Pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    Protocolo = cargaPedido.Pedido.Protocolo,
                    NumeroPedidoEmbarcador = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    NumeroOrdemEmbarque = ordemEmbarque?.Numero ?? "",
                    KmRestante = entregaPedido?.CargaEntrega?.DistanciaAteDestino ?? 0m,
                };

                pedido.Cliente = new Dominio.ObjetosDeValor.WebService.Monitoramento.Pessoa()
                {
                    CpfCnpj = entregaPedido?.CargaEntrega?.Cliente?.CPF_CNPJ_SemFormato ?? "",
                    Nome = entregaPedido?.CargaEntrega?.Cliente?.Nome ?? "",
                    NomeFantasia = entregaPedido?.CargaEntrega?.Cliente?.NomeFantasia ?? ""
                };

                pedido.DonoContainer = new Dominio.ObjetosDeValor.WebService.Monitoramento.Pessoa()
                {
                    CpfCnpj = cargaPedido.Pedido.ClienteDonoContainer?.CPF_CNPJ_SemFormato ?? "",
                    Nome = cargaPedido.Pedido.ClienteDonoContainer?.Nome ?? "",
                    NomeFantasia = cargaPedido.Pedido.ClienteDonoContainer?.NomeFantasia ?? ""
                };

                pedido.PortoDestino = new Dominio.ObjetosDeValor.WebService.Monitoramento.Porto()
                {
                    Codigo = cargaPedido.Pedido.CodigoPortoDestino ?? "",
                    Descricao = cargaPedido.Pedido.DescricaoPortoDestino ?? "",
                    Pais = cargaPedido.Pedido.PaisPortoDestino ?? ""
                };

                pedido.PortoOrigem = new Dominio.ObjetosDeValor.WebService.Monitoramento.Porto()
                {
                    Codigo = cargaPedido.Pedido.CodigoPortoOrigem ?? "",
                    Descricao = cargaPedido.Pedido.DescricaoPortoOrigem ?? "",
                    Pais = cargaPedido.Pedido.PaisPortoOrigem ?? ""
                };

                if (cargaJanelaDescarregamento != null)
                {
                    pedido.CentroDescarregamento = new Dominio.ObjetosDeValor.WebService.Monitoramento.CentroDescarregamento()
                    {
                        Descricao = cargaJanelaDescarregamento.CentroDescarregamento?.Descricao ?? string.Empty,
                        DataDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm")
                    };
                }

                if (produtosPorCargaPedido?.Count > 0)
                {
                    pedido.Produtos = new List<Dominio.ObjetosDeValor.WebService.Monitoramento.Produto>();

                    foreach ((int CodigoCarga, int CodigoCargaPedido, string CodigoProdutoEmbarcador, string DescricaoProduto, decimal Quantidade) produtoPorCargaPedido in produtosPorCargaPedido)
                    {
                        Dominio.ObjetosDeValor.WebService.Monitoramento.Produto produto = new Dominio.ObjetosDeValor.WebService.Monitoramento.Produto()
                        {
                            Codigo = produtoPorCargaPedido.CodigoProdutoEmbarcador,
                            Descricao = produtoPorCargaPedido.DescricaoProduto,
                            Quantidade = produtoPorCargaPedido.Quantidade
                        };

                        pedido.Produtos.Add(produto);
                    }
                }

                pedidos.Add(pedido);
            }

            return pedidos;
        }

        private Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento PreencherHistoricoPosicoesCargaMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga, IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXMLNotaFiscal, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Cliente clienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, carga);

            Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento historicoPosicoesCargaMonitoramento = new Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento()
            {
                Transporte = carga.CodigoCargaEmbarcador,
                Status = "A",
                Placa = carga.Veiculo.Placa,
                Origem = clienteOrigem != null ? clienteOrigem.Latitude.ToString() + ";" + clienteOrigem.Longitude.ToString() + ";" + "0;" : listaCargaEntrega.FirstOrDefault().Cliente.Latitude.ToString() + ";" + listaCargaEntrega.FirstOrDefault().Cliente.Longitude.ToString() + ";" + "0;",
                Cliente = PreencherCliente(listaCargaEntrega.ToArray()),
                Percursos = PreencherPercursos(posicoes.ToArray()),
                Fornecimentos = PreencherFornecimentos(carga, listaPedidoXMLNotaFiscal.ToArray()),
            };

            return historicoPosicoesCargaMonitoramento;
        }

        private Dominio.ObjetosDeValor.WebService.Monitoramento.Percursos[] PreencherPercursos(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] posicoes)
        {
            int quantiaRegistros = posicoes.Length;

            if (quantiaRegistros == 0)
                return new Dominio.ObjetosDeValor.WebService.Monitoramento.Percursos[0];

            Dominio.ObjetosDeValor.WebService.Monitoramento.Percursos[] percursos = new Dominio.ObjetosDeValor.WebService.Monitoramento.Percursos[quantiaRegistros];
            //os caracteres 0; ao fim da Coordenada adicionado por solicitacao da Arcelor; 05-07-2023 (toniazzo)
            for (int i = 0; i < quantiaRegistros; i++)
            {
                percursos[i] = new Dominio.ObjetosDeValor.WebService.Monitoramento.Percursos()
                {
                    Coordenada = posicoes[i].Latitude.ToString() + ";" + posicoes[i].Longitude.ToString() + ";0;",
                    Data = posicoes[i].DataVeiculo.ToString("ddMMyyyy"),
                    Hora = posicoes[i].DataVeiculo.ToString("HHmm")
                };
            }

            return percursos;
        }

        private Dominio.ObjetosDeValor.WebService.Monitoramento.Fornecimentos[] PreencherFornecimentos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal[] listaPedidoXMLNotaFiscal)
        {
            int quantiaRegistros = listaPedidoXMLNotaFiscal.Length;

            if (quantiaRegistros == 0)
                return new Dominio.ObjetosDeValor.WebService.Monitoramento.Fornecimentos[0];

            Dominio.ObjetosDeValor.WebService.Monitoramento.Fornecimentos[] fornecimentos = new Dominio.ObjetosDeValor.WebService.Monitoramento.Fornecimentos[quantiaRegistros];

            for (int i = 0; i < quantiaRegistros; i++)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXMLNotaFiscalFiltrada = listaPedidoXMLNotaFiscal.Where(a => a.CargaPedido.Pedido.Codigo == listaPedidoXMLNotaFiscal[i].CargaPedido.Pedido.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in listaPedidoXMLNotaFiscalFiltrada)
                {

                    fornecimentos[i] = new Dominio.ObjetosDeValor.WebService.Monitoramento.Fornecimentos()
                    {
                        Transportador = carga.Empresa?.Descricao ?? string.Empty,
                        Remessa = pedidoXMLNotaFiscal.CargaPedido?.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                        NotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal?.Numero.ToString() ?? string.Empty,
                        Cliente = pedidoXMLNotaFiscal.CargaPedido.Pedido.Recebedor != null ? pedidoXMLNotaFiscal.CargaPedido?.Pedido?.Recebedor?.Descricao ?? string.Empty : pedidoXMLNotaFiscal.CargaPedido?.Pedido?.Destinatario?.Descricao ?? string.Empty,
                        Peso = pedidoXMLNotaFiscal.XMLNotaFiscal.Peso,
                    };

                }
            }

            return fornecimentos;
        }

        private Dominio.ObjetosDeValor.WebService.Monitoramento.Cliente[] PreencherCliente(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega[] listaCargaEntrega)
        {
            int quantiaRegistros = listaCargaEntrega.Length;

            if (quantiaRegistros == 0)
                return new Dominio.ObjetosDeValor.WebService.Monitoramento.Cliente[0];

            listaCargaEntrega.OrderBy(o => o.Ordem);

            Dominio.ObjetosDeValor.WebService.Monitoramento.Cliente[] clientes = new Dominio.ObjetosDeValor.WebService.Monitoramento.Cliente[quantiaRegistros];

            //os caracteres 0; ao fim da Coordenada adicionado por solicitacao da Arcelor; 05-07-2023 (toniazzo)
            for (int i = 0; i < quantiaRegistros; i++)
            {
                clientes[i] = new Dominio.ObjetosDeValor.WebService.Monitoramento.Cliente()
                {
                    Nome = listaCargaEntrega[i].Cliente.Descricao,
                    Coordenada = listaCargaEntrega[i].Cliente.Latitude.ToString() + ";" + listaCargaEntrega[i].Cliente.Longitude.ToString() + ";0;",
                    TpRastreador = "Dinâmica",
                    DataPrevista = listaCargaEntrega[i].DataPrevista.HasValue ? listaCargaEntrega[i].DataPrevista.Value.ToString("ddMMyyyy") : string.Empty,
                    HoraPrevista = listaCargaEntrega[i].DataPrevista.HasValue ? listaCargaEntrega[i].DataPrevista.Value.ToString("HHmm") : string.Empty,
                    TpPrevisao = string.Empty,
                    DataEntrada = string.Empty,
                    HoraEntrada = string.Empty,
                    DataSaida = string.Empty,
                    Ocorrencia = string.Empty,
                    Ordem = listaCargaEntrega[i].Ordem.ToString()
                };
            }

            return clientes;
        }

        private DateTime RestringirDataInicial(DateTime dataInicial, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            if (monitoramento.DataInicio != null)
                if (monitoramento.DataInicio > dataInicial)
                    dataInicial = monitoramento.DataInicio.Value;
                else
                if (monitoramento.DataCriacao > dataInicial)
                    dataInicial = monitoramento.DataCriacao.Value;

            return dataInicial;
        }

        private DateTime RestringirDataFinal(DateTime dataFinal, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            if (monitoramento.DataFim != null && monitoramento.DataFim < dataFinal)
                dataFinal = monitoramento.DataFim.Value;
            else
                dataFinal = DateTime.Now;

            return dataFinal;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceMonitoriamento;
        }

        #endregion
    }
}
