using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Text;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ConsolidacaoSolicitacaoGas")]
    public class ConsolidacaoSolicitacaoGasController : BaseController
    {
        #region Construtores

        public ConsolidacaoSolicitacaoGasController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                (int totalRegistros, dynamic retorno) pesquisa = ExecutarPesquisa(filtrosPesquisa, parametrosConsulta, unitOfWork);

                grid.AdicionaRows(pesquisa.retorno);
                grid.setarQuantidadeTotal(pesquisa.totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarQuantidades()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaQuantidades();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                (int totalRegistros, dynamic retorno) pesquisa = ExecutarPesquisaQuantidades(filtrosPesquisa, parametrosConsulta, unitOfWork);

                grid.AdicionaRows(pesquisa.retorno);
                grid.setarQuantidadeTotal(pesquisa.totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarInformacoesConsolidacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas repositorioConsolidacao = new Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas(unitOfWork);
                Repositorio.Embarcador.Filiais.SuprimentoDeGas repositorioSuprimentoGas = new Repositorio.Embarcador.Filiais.SuprimentoDeGas(unitOfWork);
                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacaoGas = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);

                int codigoSolicitacao = Request.GetIntParam("CodigoSolicitacao");

                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoGas = repositorioSolicitacaoGas.BuscarPorCodigo(codigoSolicitacao);

                List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas> consolidacoesGeradas = repositorioConsolidacao.BuscarPorSolicitacao(codigoSolicitacao);
                Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas suprimentoDeGas = repositorioSuprimentoGas.BuscarPorProdutoCliente(solicitacaoGas?.Produto?.Codigo ?? 0, solicitacaoGas?.ClienteBase?.CPF_CNPJ ?? 0);

                return new JsonpResult(new
                {
                    CapacidadeVeiculo = suprimentoDeGas?.ModeloVeicularPadrao?.CapacidadePesoTransporte ?? 0m,
                    ConsolidacoesGeradas = (from consolidacao in consolidacoesGeradas
                                            select new
                                            {
                                                consolidacao.Codigo,
                                                Carga = consolidacao.Carga.CodigoCargaEmbarcador,
                                                Origem = consolidacao.ClienteBaseSupridora?.Localidade?.Descricao ?? "",
                                                Quantidade = $"{consolidacao.QuantidadeCarga.ToString("n4")} TON",
                                                ModeloVeicular = consolidacao.ModeloVeicular.Descricao,
                                                SituacaoCarga = consolidacao.Carga.SituacaoCarga.ObterDescricao()
                                            }).ToList(),
                    InformacoesBaseSatelite = new
                    {
                        BaseSupridora = new { Codigo = suprimentoDeGas?.SupridorPadrao?.Codigo ?? 0, Descricao = suprimentoDeGas?.SupridorPadrao?.Descricao ?? "" },
                        ModeloVeicular = new { Codigo = suprimentoDeGas?.ModeloVeicularPadrao?.Codigo ?? 0, Descricao = suprimentoDeGas?.ModeloVeicularPadrao?.Descricao ?? "" },
                        TipoDeCarga = new { Codigo = suprimentoDeGas?.TipoCargaPadrao?.Codigo ?? 0, Descricao = suprimentoDeGas?.TipoCargaPadrao?.Descricao ?? "" },
                        TipoOperacao = new { Codigo = suprimentoDeGas?.TipoOperacaoPadrao?.Codigo ?? 0, Descricao = suprimentoDeGas?.TipoOperacaoPadrao?.Descricao ?? "" }
                    }
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarConsolidacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas repositorioConsolidacao = new Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga cargaTrechoAnterior = null;
                Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas consolidacao = new Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas();

                decimal quantidadePorModeloVeicular = ObterQuantidadeCargaPorModeloVeicular(unitOfWork);

                decimal pesoTotal = Request.GetDecimalParam("QuantidadeCarga") * quantidadePorModeloVeicular;
                int codigoProduto = Request.GetIntParam("Produto");
                int codigocargaTrechoAnterior = Request.GetIntParam("CargaTrechoAnterior");
                if (codigocargaTrechoAnterior > 0)
                    cargaTrechoAnterior = repCarga.BuscarPorCodigo(codigocargaTrechoAnterior);

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repositorioProdutoEmbarcador.BuscarPorCodigo(codigoProduto);

                int consolidacoesGeradas = 0;

                List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas> consolidacoes = new List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas>();

                PreencherConsolidacao(consolidacao, pesoTotal, unitOfWork);
                List<string> cargasGeradas = new List<string>();

                while (pesoTotal > 0)
                {
                    Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas consolidacaoAdicionar = consolidacao.Clonar<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas>();
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga();

                    decimal quantidade = pesoTotal > quantidadePorModeloVeicular ? quantidadePorModeloVeicular : pesoTotal;
                    consolidacaoAdicionar.QuantidadeCarga = quantidade / 1000.0m;

                    pedido = CriarPedido(consolidacaoAdicionar, quantidade, cargaTrechoAnterior, unitOfWork);
                    pedidoProduto = PreencherPedidoProduto(pedido, produtoEmbarcador);

                    if (pedidoProduto != null)
                        repositorioPedidoProduto.Inserir(pedidoProduto);

                    GerarCarga(consolidacaoAdicionar, carga, pedido, unitOfWork);

                    consolidacoes.Add(consolidacaoAdicionar);
                    cargasGeradas.Add(consolidacaoAdicionar.Carga.CodigoCargaEmbarcador);

                    pesoTotal -= quantidade;
                    consolidacoesGeradas += 1;
                }

                consolidacoes.ForEach(obj => repositorioConsolidacao.Inserir(obj));

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, consolidacoesGeradas > 1 ? $"Consolidações {string.Join(", ", cargasGeradas)} geradas com sucesso!" : $"Consolidação {cargasGeradas.Select(obj => obj).FirstOrDefault()} gerada com sucesso!");
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu um erro ao adicionar consolidação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarConsolidacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoConsolidacao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas repositorioConsolidacao = new Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas consolidacao = repositorioConsolidacao.BuscarPorCodigo(codigoConsolidacao, false);

                if (consolidacao == null)
                    throw new ControllerException("A consolidação não foi encontrada.");

                if (consolidacao.Carga.SituacaoCarga == SituacaoCarga.Cancelada)
                    throw new ControllerException("A consolidação já foi cancelada.");

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                {
                    Carga = consolidacao.Carga,
                    MotivoCancelamento = "Cancelamento por consolidação de gás",
                    TipoServicoMultisoftware = TipoServicoMultisoftware,
                    Usuario = this.Usuario
                };

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, ConfiguracaoEmbarcador, unitOfWork);
                Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware);

                if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                {
                    unitOfWork.CommitChanges();
                    return new JsonpResult(false, cargaCancelamento.MensagemRejeicaoCancelamento);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, consolidacao, "Consolidação cancelada.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Consolidação cancelada com sucesso");
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu um erro ao cancelar a consolidação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherConsolidacao(Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas consolidacao, decimal peso, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTipoDeCarga = Request.GetIntParam("TipoDeCarga");
            double codigoBaseSupridora = Request.GetDoubleParam("BaseSupridora");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");
            int codigoTransportadora = Request.GetIntParam("Transportadora");
            int codigoSolicitacaoAbastecimento = Request.GetIntParam("Solicitacao");

            if (codigoTipoDeCarga > 0)
            {
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                consolidacao.TipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoDeCarga);
            }

            if (codigoBaseSupridora > 0)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                consolidacao.ClienteBaseSupridora = repositorioCliente.BuscarPorCPFCNPJ(codigoBaseSupridora);
            }

            if (codigoTipoOperacao > 0)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                consolidacao.TipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);
            }

            if (codigoModeloVeicular > 0)
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                consolidacao.ModeloVeicular = repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicular);
            }

            if (codigoTransportadora > 0)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                consolidacao.Transportadora = repositorioEmpresa.BuscarPorCodigo(codigoTransportadora);
            }

            if (codigoSolicitacaoAbastecimento > 0)
            {
                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacao = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacao = repositorioSolicitacao.BuscarPorCodigo(codigoSolicitacaoAbastecimento);

                consolidacao.SolicitacaoAbastecimentoGas = solicitacao;
                solicitacao.VolumeRodoviarioCarregamentoProximoDia -= (peso / 1000.0m);
                repositorioSolicitacao.Atualizar(solicitacao);

                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoDescontarVolume = repositorioSolicitacao.BuscarSolicitacaoComDisponibilidadeTransferenciaPorDataDeMedicao(solicitacao.DataMedicao);
                if (solicitacaoDescontarVolume != null)
                {
                    decimal valorDescontar = peso / 1000.0m;

                    solicitacaoDescontarVolume.DataUltimaAlteracao = DateTime.Now;
                    solicitacaoDescontarVolume.SaldoRestante -= valorDescontar;
                    repositorioSolicitacao.Atualizar(solicitacaoDescontarVolume);
                }
            }
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido CriarPedido(Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas consolidacao, decimal quantidade, Dominio.Entidades.Embarcador.Cargas.Carga CargaAnterior, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Servicos.WebService.Carga.Pedido servicoWsPedido = new Servicos.WebService.Carga.Pedido(unitOfWork);

            StringBuilder mensagemErro = new StringBuilder();
            int protocoloPedidoExistente = 0;
            int protocoloCargaExistente = 0;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCNPJ(consolidacao.ClienteBaseSupridora.CPF_CNPJ_SemFormato);

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = PreencherObjetoCargaIntegracao(consolidacao, quantidade, unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoWsPedido.CriarPedido(cargaIntegracao, filial, consolidacao.TipoOperacao, ref mensagemErro, TipoServicoMultisoftware, ref protocoloPedidoExistente, ref protocoloCargaExistente, false, forcarGerarNovoPedido: true);

            if (!string.IsNullOrWhiteSpace(mensagemErro.ToString()))
                throw new ControllerException(mensagemErro.ToString());

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            if (consolidacao.Transportadora != null)
                pedido.Empresa = consolidacao.Transportadora;

            if (CargaAnterior != null)
            {
                pedido.Empresa = CargaAnterior.Empresa;
                pedido.VeiculoTracao = CargaAnterior.Veiculo;

                if (pedido.Motoristas != null)
                    pedido.Motoristas.Clear();

                pedido.Motoristas = new List<Dominio.Entidades.Usuario>();
                pedido.Motoristas = CargaAnterior.Motoristas;
            }

            DateTime? dataCarregamento = Request.GetNullableDateTimeParam("DataCarregamento");
            DateTime? dataEntrega = Request.GetNullableDateTimeParam("DataEntrega");

            if (dataCarregamento.HasValue)
                pedido.DataCarregamentoPedido = dataCarregamento;
            if (dataEntrega.HasValue)
                pedido.PrevisaoEntrega = dataEntrega;

            repositorioPedido.Atualizar(pedido);

            return pedido;
        }

        private Dominio.Entidades.Embarcador.Pedidos.PedidoProduto PreencherPedidoProduto(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto)
        {
            if (produto == null)
                return null;

            return new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto()
            {
                AlturaCM = produto.AlturaCM,
                Codigo = produto.Codigo,
                ComprimentoCM = produto.ComprimentoCM,
                LarguraCM = produto.LarguraCM,
                LinhaSeparacao = produto.LinhaSeparacao,
                MetroCubico = produto.MetroCubito,
                Observacao = produto.Observacao,
                Pedido = pedido,
                Produto = produto,
                Quantidade = pedido.PesoTotal,
                PesoUnitario = 1
            };
        }

        private Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao PreencherObjetoCargaIntegracao(Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas consolidacao, decimal peso, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            string numeroCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCNPJ(consolidacao.ClienteBaseSupridora?.CPF_CNPJ.ToString().ObterSomenteNumeros());
            string codigoFilialEmbarcador = filial?.CodigoFilialEmbarcador;

            int numeroPedidoEmbarcador = ConfiguracaoEmbarcador.UtilizarNumeroPreCargaPorFilial && filial != null ? repositorioPedido.ObterProximoCodigo(filial) : repositorioPedido.ObterProximoCodigo();

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao
            {
                Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = codigoFilialEmbarcador },
                NumeroCarga = numeroCarga,
                NumeroPedidoEmbarcador = numeroPedidoEmbarcador.ToString(),
                ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular() { CodigoIntegracao = consolidacao.ModeloVeicular.CodigoIntegracao },
                Recebedor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = consolidacao.SolicitacaoAbastecimentoGas.ClienteBase.CPF_CNPJ_Formatado },
                Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = consolidacao.SolicitacaoAbastecimentoGas.ClienteBase.CPF_CNPJ_Formatado },
                Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = consolidacao.ClienteBaseSupridora.CPF_CNPJ_Formatado },
                TipoCargaEmbarcador = consolidacao.TipoDeCarga != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = consolidacao.TipoDeCarga.CodigoTipoCargaEmbarcador } : null,
                DataColeta = consolidacao.SolicitacaoAbastecimentoGas.DataMedicao.ToString("dd/MM/yyyy HH:mm:ss"),
                PesoBruto = peso,
                ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto() { DescricaoProduto = consolidacao.SolicitacaoAbastecimentoGas.Produto?.Descricao ?? "", CodigoProduto = consolidacao.SolicitacaoAbastecimentoGas.Produto?.CodigoProdutoEmbarcador ?? "" }
            };

            return cargaIntegracao;
        }

        private void GerarCarga(Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas consolidacao, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagemRetornoCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out carga, new List<Dominio.Entidades.Embarcador.Pedidos.Pedido> { pedido }, unitOfWork, TipoServicoMultisoftware, null, ConfiguracaoEmbarcador, true, false, false, false);

            carga.ModeloVeicularCarga = consolidacao.ModeloVeicular;
            int numeroSequencia = 0;
            Int32.TryParse(carga.CodigoCargaEmbarcador, out numeroSequencia);

            carga.NumeroSequenciaCarga = numeroSequencia;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            repositorioCarga.Atualizar(carga);

            if (!string.IsNullOrWhiteSpace(mensagemRetornoCarga))
                throw new ControllerException(mensagemRetornoCarga);

            consolidacao.Carga = carga;
        }

        private (int totalRegistros, dynamic registros) ExecutarPesquisaQuantidades(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork)
        {
            filtrosPesquisa.PossuiDisponibilidadeDeTransferencia = true;
            filtrosPesquisa.AgruparPorDia = true;

            (int totalRegistros, dynamic registros) retorno;

            Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacaoAbastecimentoGas = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
            Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas repositorioConsolidacao = new Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas(unitOfWork);

            retorno.totalRegistros = repositorioSolicitacaoAbastecimentoGas.ContarConsulta(filtrosPesquisa, parametrosConsulta);

            List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas> solicitacoes = retorno.totalRegistros > 0 ? repositorioSolicitacaoAbastecimentoGas.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();

            List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas> consolidacoes = repositorioConsolidacao.BuscarPorDataMedicao(filtrosPesquisa.DataSolicitacao.Value);
            consolidacoes = consolidacoes.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada).ToList();

            retorno.registros = (from solicitacao in solicitacoes
                                 select new
                                 {
                                     solicitacao.Codigo,
                                     CodigoBase = solicitacao.ClienteBase?.CPF_CNPJ ?? 0,
                                     CodigoProduto = solicitacao.Produto?.Codigo ?? 0,
                                     Produto = solicitacao.Produto?.Descricao,
                                     Unidade = solicitacao.ClienteBase?.Descricao ?? "",
                                     TotalDisponivel = solicitacao.DisponibilidadeTransferenciaProximoDiaTotal.ToString("n4"),
                                     SaldoRestante = (solicitacao.DisponibilidadeTransferenciaProximoDiaTotal - ObterQuantidadeAlocada(consolidacoes.Where(obj => obj.ClienteBaseSupridora?.CPF_CNPJ == solicitacao.ClienteBase?.CPF_CNPJ && obj.SolicitacaoAbastecimentoGas.Produto?.Codigo == solicitacao.Produto?.Codigo).ToList())).ToString("n2")
                                 });


            return (retorno.totalRegistros, retorno.registros);
        }

        private decimal ObterQuantidadeAlocada(List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas> consolidacoes)
        {
            return consolidacoes.Sum(x => x.QuantidadeCarga);
        }

        private (int totalRegistros, dynamic registros) ExecutarPesquisa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork)
        {
            filtrosPesquisa.PossuiVolumeRodoviario = true;

            (int totalRegistros, dynamic registros) retorno;

            Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacaoAbastecimentoGas = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
            Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas repositorioConsolidacao = new Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas(unitOfWork);
            Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas repositorioFilialSuprimentoGas = new Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas(unitOfWork);

            retorno.totalRegistros = repositorioSolicitacaoAbastecimentoGas.ContarConsulta(filtrosPesquisa, parametrosConsulta);

            List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas> solicitacoes = retorno.totalRegistros > 0 ? repositorioSolicitacaoAbastecimentoGas.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();
            List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas> consolidacoes = retorno.totalRegistros > 0 ? repositorioConsolidacao.BuscarPorSolicitacoesCanceladas(solicitacoes.Select(x => x.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas>();

            List<(double CpfCnpjBase, int CodigoProduto)> listaBasesProdutos = solicitacoes.Where(obj => obj.ClienteBase != null).Select(obj => ValueTuple.Create(
                    obj.ClienteBase.CPF_CNPJ,
                    obj.Produto?.Codigo ?? 0
                )).ToList();

            List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> listaSuprimentos = repositorioFilialSuprimentoGas.BuscarPorFiliaisProdutos(listaBasesProdutos);

            retorno.registros = (from solicitacao in solicitacoes
                                 select new
                                 {
                                     solicitacao.Codigo,
                                     CodigoIntegracaoBaseSatelite = solicitacao.ClienteBase?.CodigoIntegracao,
                                     QuantidadeFaltante = solicitacao.VolumeRodoviarioCarregamentoProximoDiaTotal.ToString("n2"),
                                     BaseSupridoraCodigo = (from o in listaSuprimentos where o.SuprimentoDeGas.ProdutoPadrao?.Codigo == solicitacao.Produto?.Codigo && o.Cliente?.CPF_CNPJ == solicitacao.ClienteBase?.CPF_CNPJ select o?.SuprimentoDeGas?.SupridorPadrao?.CPF_CNPJ)?.FirstOrDefault() ?? 0,
                                     BaseSateliteCodigo = solicitacao.ClienteBase?.CPF_CNPJ ?? 0,
                                     BaseSatelite = solicitacao.ClienteBase?.Descricao ?? "",
                                     DensidadeAberturaDia = solicitacao.DensidadeAberturaDia.ToString("n2"),
                                     Produto = solicitacao.Produto?.Descricao ?? string.Empty,
                                     Capacidade = (from o in listaSuprimentos where o.SuprimentoDeGas.ProdutoPadrao?.Codigo == solicitacao.Produto?.Codigo && o.Cliente?.CPF_CNPJ == solicitacao.ClienteBase?.CPF_CNPJ select o?.SuprimentoDeGas?.Capacidade.ToString("n2"))?.FirstOrDefault(),
                                     Data = solicitacao.DataMedicao.ToString("dd/MM/yyyy"),
                                     Abertura = solicitacao.Abertura.ToString("n4"),
                                     PrevisaoBombeio = solicitacao.PrevisaoBombeio.ToString("n2"),
                                     PrevisaoTransferenciaRecebida = solicitacao.PrevisaoTransferenciaRecebida.ToString("n2"),
                                     PrevisaoDemandaDomiciliar = solicitacao.PrevisaoDemandaDomiciliar.ToString("n2"),
                                     PrevisaoDemandaEmpresarial = solicitacao.PrevisaoDemandaEmpresarial.ToString("n2"),
                                     EstoqueUltrasystem = solicitacao.EstoqueUltrasystem.ToString("n2"),
                                     PrevisaoTransferenciaEnviada = solicitacao.PrevisaoTransferenciaEnviada.ToString("n2"),
                                     PrevisaoFechamento = solicitacao.PrevisaoFechamento.ToString("n2"),
                                     SolicitacaoVolumeProximoDia = solicitacao.VolumeRodoviarioCarregamentoProximoDiaTotal.ToString("n2"),
                                     UltimaAlteracao = solicitacao.DataUltimaAlteracao.ToString("dd/MM/yyyy"),
                                     CodigoProduto = solicitacao.Produto?.Codigo ?? 0
                                 }).ToList();


            return (retorno.totalRegistros, retorno.registros);
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("BaseSateliteCodigo", false);
            grid.AdicionarCabecalho("BaseSupridoraCodigo", false);
            grid.AdicionarCabecalho("QuantidadeFaltante", false);
            grid.AdicionarCabecalho("CodigoProduto", false);
            grid.AdicionarCabecalho("CodigoIntegracaoBaseSatelite", false);
            grid.AdicionarCabecalho("Base Satélite", "BaseSatelite", 15, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Produto", "Produto", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Capacidade", "Capacidade", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Abertura", "Abertura", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Previsão de Bombeio", "PrevisaoBombeio", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Previsão de Transferência Recebida", "PrevisaoTransferenciaRecebida", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Previsão de Demanda Domiciliar", "PrevisaoDemandaDomiciliar", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Previsão de Demanda Empresarial", "PrevisaoDemandaEmpresarial", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Estoque Ultrasystem", "EstoqueUltrasystem", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Previsão de Transferência Enviada", "PrevisaoTransferenciaEnviada", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Densidade da Abertura do Dia", "DensidadeAberturaDia", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Previsão de Fechamento", "PrevisaoFechamento", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Solicitação de Volume p/ próximo dia", "SolicitacaoVolumeProximoDia", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Ultima Alteração", "UltimaAlteracao", 15, Models.Grid.Align.left, false, true);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisaQuantidades()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoBase", false);
            grid.AdicionarCabecalho("CodigoProduto", false);
            grid.AdicionarCabecalho("Unidade", "Unidade", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Produto", "Produto", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Total Disponível", "TotalDisponivel", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Saldo Restante", "SaldoRestante", 15, Models.Grid.Align.left, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repositorioOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);

            List<double> codigosSupridoresPermitidos = new List<double>();

            if (operadorLogistica != null)
                if (operadorLogistica.Filiais?.Count > 0)
                    codigosSupridoresPermitidos = (from obj in operadorLogistica.Filiais where obj.Filial.Ativo select obj.Filial.CNPJ.ToDouble()).ToList();



            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas()
            {
                DataCriacaoInicial = Request.GetNullableDateTimeParam("DataCriacaoInicial"),
                DataCriacaoFinal = Request.GetNullableDateTimeParam("DataCriacaoFinal"),
                DataSolicitacao = Request.GetNullableDateTimeParam("DataSolicitacao"),
                CodigosBasesSatelite = Request.GetListParam<double>("BaseSatelite"),
                CodigosBasesSupridora = Request.GetListParam<double>("BaseSupridora"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                CodigosSupridoresPermitidos = codigosSupridoresPermitidos,
                Situacao = SituacaoAprovacaoSolicitacaoGas.Aprovada
            };
        }

        private decimal ObterQuantidadeCargaPorModeloVeicular(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repositorioModeloVeicular.BuscarPorCodigo(codigoModeloVeicular);

            if (modeloVeicular == null)
                throw new ControllerException("Modelo veicular não encontrado.");

            if (modeloVeicular.CapacidadePesoTransporte <= 0)
                throw new ControllerException("O modelo veicular selecionado não possui capacidade informada.");

            return modeloVeicular.CapacidadePesoTransporte;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CodigoIntegracaoBaseSatelite" || propriedadeOrdenar == "BaseSatelite")
                return "ClienteBase.CodigoIntegracao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
