using System.Globalization;
using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Carga.GestaoPedido;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.GestaoPedido
{
    [CustomAuthorize(new string[] { "DetalhesPedidoProdutos", "BuscarTiposPaletesDetalhes" }, "Cargas/GestaoPedido", "Cargas/MontagemCargaMapa")]
    public class GestaoPedidoController : BaseController
    {
        #region Construtores

        public GestaoPedidoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridPesquisa(cancellationToken));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar gestão de pedidos.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            try
            {
                Grid grid = await ObterGridPesquisa(cancellationToken);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

                return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPedidosSelecionados(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");
                List<int> codigosPedidos = Request.GetListParam<int>("ListaPedidosSelecionados");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);

                FiltroPesquisaGestaoPedido filtrosPesquisa = ObterFiltrosPesquisaPedido();
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoPedido.PedidoGestaoPedido> pedidos = await repositorioPedido.ObterSelecionadosConsultaPedidosGestaoPedidoAsync(filtrosPesquisa, selecionarTodos, codigosPedidos);

                return new JsonpResult(new
                {
                    TotalPedidos = pedidos.Count,
                    CodigosPedidos = pedidos.Select(pedido => pedido.Codigo).ToList(),
                    PesoTotalPedidos = pedidos.Sum(pedido => pedido.PesoTotalPedido).ToString("N", new CultureInfo("pt-BR")),
                    ValorTotalPedidos = pedidos.Sum(pedido => pedido.ValorTotalPedido).ToString("N", new CultureInfo("pt-BR")),
                    QuantidadePalletsPedidos = pedidos.Sum(pedido => pedido.NumeroPaletesFracionado),
                    QuantidadeEntregasSelecionados = pedidos.Select(pedido => pedido.CPF_CNPJ_Cliente).Distinct().Count()
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar gestão de pedidos.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesPedidoProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPedido = Request.GetIntParam("Codigo");

                if (codigoPedido == 0)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaBuscarDetalhes);

                Repositorio.Embarcador.Pedidos.PedidoProduto _repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem _repositorioProdutoEmbarcadorEstoqueArmazen = new Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento _repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                List<PedidoProduto> produtosPedido = _repositorioPedidoProduto.BuscarPorPedido(codigoPedido);
                List<object> colunasGridTratadas = new List<object>();

                List<int> filiaisDistintas = (from o in produtosPedido select o.Pedido?.Filial?.Codigo ?? 0).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = _repositorioCentroCarregamento.BuscarPorFiliais(filiaisDistintas);

                foreach (PedidoProduto produtoPedido in produtosPedido)
                {
                    decimal estoqueDisponivel = _repositorioProdutoEmbarcadorEstoqueArmazen.BuscarPorFilialProdutoArmazem(produtoPedido?.FilialArmazem?.Filial.Codigo ?? 0, produtoPedido.Produto.Codigo, produtoPedido?.FilialArmazem?.Codigo ?? 0)?.EstoqueDisponivel ?? 0;

                    colunasGridTratadas.Add(new
                    {
                        produtoPedido.Codigo,
                        CodigoProduto = produtoPedido.Produto.Codigo,
                        CodigoFilial = produtoPedido.Pedido.Filial?.Codigo,
                        CodigoIntegracao = produtoPedido.Produto?.CodigoProdutoEmbarcador ?? string.Empty,
                        Descricao = produtoPedido.Produto?.Descricao ?? string.Empty,
                        produtoPedido.Quantidade,
                        Estoque = estoqueDisponivel,
                        CodigoIntegracaoArmazem = produtoPedido.FilialArmazem?.CodigoIntegracao ?? string.Empty,
                        PesoUnitario = produtoPedido.PesoUnitario.ToString("n3"),
                        PesoTotal = ((produtoPedido.Quantidade * produtoPedido.PesoUnitario) + produtoPedido.PesoTotalEmbalagem).ToString("n3"),
                        QuantiadeCaixasPorPallet = produtoPedido.QuantidadeCaixaPorPallet,
                        QuantidadePallets = produtoPedido.QuantidadePalet.ToString("n3"),
                        PalletFechado = produtoPedido.PalletFechado ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                        produtoPedido.Desmembrar,
                        DT_RowColor = produtoPedido.Quantidade > estoqueDisponivel ? "#f9bfbc" : "",
                        DT_Enable = true,
                        TipoEdicaoPalletProdutoMontagemCarregamento = (from o in centrosCarregamento where (o?.Filial?.Codigo ?? 0) == (produtoPedido.Pedido?.Filial?.Codigo ?? 0) select o?.TipoEdicaoPalletProdutoMontagemCarregamento).FirstOrDefault()
                    });
                }

                return new JsonpResult(colunasGridTratadas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigoPedido(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPedido = Request.GetIntParam("Codigo");

                if (codigoPedido == 0)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.Codigo);

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido pedido = await repositorioPedido.BuscarDadosPedidoPorCodigoAsync(codigoPedido);

                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<string> filiais = await repFilial.BuscarListaCNPJAtivasAsync();
                dynamic pedidoTratado = servicoPedido.ObterDetalhesPedido(pedido, filiais, null, ConfiguracaoEmbarcador, unitOfWork);

                pedidoTratado.SituacaoComercialPedido = pedido.SituacaoComercialPedido?.Descricao ?? string.Empty;
                pedidoTratado.NumeroPaletesFracionado = pedido.NumeroPaletesFracionado;
                pedidoTratado.PesoTotalPaletes = pedido.PesoTotalPaletes;
                pedidoTratado.TipoPaleteCliente = pedido.TipoPaleteCliente;
                pedidoTratado.TipoPaleteClienteDescricao = pedido.TipoPaleteCliente?.ObterDescricao() ?? "Não Informado";
                pedidoTratado.CodigoPedidoCliente = pedido.CodigoPedidoCliente;
                pedidoTratado.Vendedor = pedido.FuncionarioVendedor?.Descricao ?? string.Empty;

                return new JsonpResult(pedidoTratado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar gestão de pedidos.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarTiposPaletesDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> tiposPallet = repositorioTipoDetalhe.BuscarPorTipo(TipoTipoDetalhe.TipoPallet);

                return new JsonpResult(tiposPallet);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar tipos paletes detatalhes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarPedido(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPedido = Request.GetIntParam("CodigoPedido");

                if (codigoPedido == 0)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.DadosInvalidos);

                int codigoSessaoRoteirizador = Request.GetIntParam("SessaoRoteirizador");
                List<int> carregamentosEnvolvidos = new List<int>();

                List<dynamic> produtosPedido = Request.GetListParam<dynamic>("ProdutosPedido");

                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repositorioPedido.BuscarPorCodigoAsync(codigoPedido, auditavel: true);

                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = await repositorioCargaPedido.BuscarPorPedidoComCargaAtivaAsync(pedido.Codigo);

                decimal totalCubagem = 0, totalPeso = 0, totalPalet = 0;
                int qtdVolumes = 0;
                bool volumesAlterados = false;
                bool itensAlterados = false;

                List<PedidoProduto> produtosPedidoRequest = new List<PedidoProduto>();

                foreach (dynamic produtoPedidoRequest in produtosPedido)
                {
                    if (produtoPedidoRequest.Codigo != null)
                    {
                        produtosPedidoRequest.Add(new PedidoProduto
                        {
                            Codigo = (int)produtoPedidoRequest.Codigo,
                            Quantidade = ((string)produtoPedidoRequest.Quantidade).ToDecimal(),
                            PesoUnitario = ((string)produtoPedidoRequest.PesoUnitario).ToDecimal(),
                            QuantidadePalet = ((string)produtoPedidoRequest.QuantidadePallets).ToDecimal(),
                            Desmembrar = (bool)produtoPedidoRequest.Desmembrar
                        });
                    }
                }

                List<int> codigosProdutosPedidos = produtosPedidoRequest.Select(produtoPedidoRequest => produtoPedidoRequest.Codigo).ToList();
                List<PedidoProduto> produtosPedidos = await repositorioPedidoProduto.BuscarPorCodigosAsync(codigosProdutosPedidos);
                List<PedidoProduto> produtosRemovidos = pedido.Produtos.Where(produto => !codigosProdutosPedidos.Contains(produto.Codigo)).ToList();

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidoSessao = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProdutoSessao = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

                if (codigoSessaoRoteirizador > 0)
                {
                    carregamentosPedidoSessao = await repositorioCarregamentoPedido.BuscarPorSessaoRoteirizacaoEPedidosAsync(codigoSessaoRoteirizador, new List<int> { codigoPedido });
                    carregamentosPedidoProdutoSessao = await repositorioCarregamentoPedidoProduto.BuscarPorSessaoRoteirizadorEPedidosAsync(codigoSessaoRoteirizador, new List<int> { codigoPedido });
                }

                await unitOfWork.StartAsync(cancellationToken);

                foreach (PedidoProduto produtoPedido in produtosPedidos)
                {
                    PedidoProduto produtoPedidoRequest = produtosPedidoRequest.Find(_produtoPedidoRequest => _produtoPedidoRequest.Codigo == produtoPedido.Codigo);

                    bool alterado = produtoPedido.Quantidade != produtoPedidoRequest.Quantidade || produtoPedido.PesoUnitario != produtoPedidoRequest.PesoUnitario;

                    if (produtoPedido.Quantidade != produtoPedidoRequest.Quantidade)
                        volumesAlterados = true;

                    if (alterado)
                    {
                        itensAlterados = true;

                        produtoPedido.Initialize();

                        produtoPedido.Quantidade = produtoPedidoRequest.Quantidade;
                        produtoPedido.PesoUnitario = produtoPedidoRequest.PesoUnitario;
                        produtoPedido.QuantidadePalet = produtoPedidoRequest.QuantidadePalet;
                        produtoPedido.Desmembrar = produtoPedidoRequest.Desmembrar;

                        await repositorioPedidoProduto.AtualizarAsync(produtoPedido);

                        if (codigoSessaoRoteirizador == 0)
                            await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, pedido, produtoPedido.GetChanges(), $"Atualizado Produto {produtoPedido.Produto?.CodigoProdutoEmbarcador ?? string.Empty} (Via Tela Gestão de Pedidos)", unitOfWork, AcaoBancoDados.Registro, cancellationToken);
                        else
                            await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, pedido, produtoPedido.GetChanges(), $"Atualizado Produto {produtoPedido.Produto?.CodigoProdutoEmbarcador ?? string.Empty} (Via Tela Sessão Roteirização {codigoSessaoRoteirizador})", unitOfWork, AcaoBancoDados.Registro, cancellationToken);

                        if (codigoSessaoRoteirizador > 0)
                        {
                            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto = (from o in carregamentosPedidoProdutoSessao
                                                                                                                                     where o.PedidoProduto.Codigo == produtoPedido.Codigo
                                                                                                                                     select o).FirstOrDefault();
                            if (carregamentoPedidoProduto != null)
                            {
                                if (carregamentoPedidoProduto.CarregamentoPedido.Carregamento.SituacaoCarregamento != SituacaoCarregamento.EmMontagem)
                                    throw new ControllerException($"O produto {carregamentoPedidoProduto.PedidoProduto?.Produto?.Descricao ?? string.Empty} não pode ser alterado pois está no carregamento {carregamentoPedidoProduto.CarregamentoPedido.Carregamento.NumeroCarregamento} com situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamentoHelper.ObterDescricao(carregamentoPedidoProduto.CarregamentoPedido.Carregamento.SituacaoCarregamento)}.");

                                decimal difPeso = (carregamentoPedidoProduto.Peso - produtoPedido.Quantidade * produtoPedido.PesoUnitario);
                                decimal difPallet = carregamentoPedidoProduto.QuantidadePallet - produtoPedido.QuantidadePalet;

                                carregamentoPedidoProduto.Quantidade = produtoPedido.Quantidade;
                                carregamentoPedidoProduto.QuantidadePallet = produtoPedido.QuantidadePalet;
                                carregamentoPedidoProduto.Peso = produtoPedido.Quantidade * produtoPedido.PesoUnitario;

                                await repositorioCarregamentoPedidoProduto.AtualizarAsync(carregamentoPedidoProduto);

                                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = (from o in carregamentosPedidoSessao
                                                                                                                           where o.Codigo == carregamentoPedidoProduto.CarregamentoPedido.Codigo
                                                                                                                           select o).FirstOrDefault();
                                if (carregamentoPedido != null)
                                {
                                    carregamentoPedido.Peso -= difPeso;
                                    carregamentoPedido.Pallet -= difPallet;
                                    await repositorioCarregamentoPedido.AtualizarAsync(carregamentoPedido);
                                }
                            }
                        }
                    }

                    totalPalet += produtoPedidoRequest.QuantidadePalet;
                    totalPeso += produtoPedidoRequest.PesoUnitario * produtoPedidoRequest.Quantidade;
                    totalCubagem += produtoPedido.MetroCubico * produtoPedidoRequest.Quantidade;

                    if (produtoPedidoRequest.Quantidade > 0)
                        qtdVolumes += (int)produtoPedidoRequest.Quantidade;
                }

                if (produtosRemovidos?.Count > 0)
                {
                    foreach (PedidoProduto produtoRemovido in produtosRemovidos)
                    {
                        if (codigoSessaoRoteirizador > 0)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidoProdutos = (from o in carregamentosPedidoProdutoSessao
                                                                                                                                            where o.PedidoProduto.Codigo == produtoRemovido.Codigo
                                                                                                                                            select o).ToList();

                            // Vamos obter as quantidades para remover do carregamentoPedidoProduto...
                            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> agrupado = carregamentoPedidoProdutos
                                .GroupBy(p => p.CarregamentoPedido.Codigo)
                                .Select(g => new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto
                                {
                                    Codigo = g.Key,
                                    Peso = g.Sum(x => x.Peso),
                                    MetroCubico = g.Sum(x => x.MetroCubico),
                                    QuantidadePallet = g.Sum(x => x.QuantidadePallet)
                                })
                                .ToList();

                            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto in carregamentoPedidoProdutos)
                            {
                                if (carregamentoPedidoProduto.CarregamentoPedido.Carregamento.SituacaoCarregamento != SituacaoCarregamento.EmMontagem)
                                    throw new ControllerException($"O produto {carregamentoPedidoProduto.PedidoProduto?.Produto?.Descricao ?? string.Empty} não pode ser alterado pois está no carregamento {carregamentoPedidoProduto.CarregamentoPedido.Carregamento.NumeroCarregamento} com situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamentoHelper.ObterDescricao(carregamentoPedidoProduto.CarregamentoPedido.Carregamento.SituacaoCarregamento)}.");

                                await repositorioCarregamentoPedidoProduto.DeletarAsync(carregamentoPedidoProduto);
                            }

                            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto cp in agrupado)
                            {
                                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = (from o in carregamentosPedidoSessao
                                                                                                                           where o.Codigo == cp.Codigo
                                                                                                                           select o).FirstOrDefault();
                                if (carregamentoPedido != null)
                                {
                                    carregamentosEnvolvidos.Add(carregamentoPedido.Carregamento.Codigo);

                                    if (!carregamentoPedidoProdutos.Exists(o => o.CarregamentoPedido.Codigo == carregamentoPedido.Codigo))
                                        await repositorioCarregamentoPedido.DeletarAsync(carregamentoPedido);
                                    else
                                    {
                                        //Localizamos o carregamento pedido, precisamos atualizar as quantidades
                                        carregamentoPedido.Pallet -= cp.QuantidadePallet;
                                        carregamentoPedido.Peso -= cp.Peso;
                                        await repositorioCarregamentoPedido.AtualizarAsync(carregamentoPedido);
                                    }
                                }
                            }
                        }

                        await repositorioPedidoProduto.DeletarAsync(produtoRemovido);

                        if (codigoSessaoRoteirizador == 0)
                            await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, pedido, $"Removido Produto {produtoRemovido.Produto?.CodigoProdutoEmbarcador ?? string.Empty} (Via Tela Gestão de Pedidos)", unitOfWork, cancellationToken);
                        else
                            await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, pedido, $"Removido Produto {produtoRemovido.Produto?.CodigoProdutoEmbarcador ?? string.Empty} (Via Tela Sessão Roteirização {codigoSessaoRoteirizador})", unitOfWork, cancellationToken);
                    }

                    itensAlterados = true;
                }

                decimal diferencaPallet = 0;
                int novaQtdePallet = 0;

                decimal novaQtdePalletFracionado = Request.GetDecimalParam("NumeroPaletesFracionado");
                decimal pesoTotalPaletes = Request.GetDecimalParam("PesoTotalPaletes");
                TipoPaleteCliente tipoPaleteCliente = Request.GetEnumParam<TipoPaleteCliente>("TipoPaleteCliente");

                diferencaPallet += (pedido.NumeroPaletes - novaQtdePallet);
                diferencaPallet += (novaQtdePalletFracionado - pedido.NumeroPaletesFracionado);

                pedido.NumeroPaletesFracionado = novaQtdePalletFracionado;
                pedido.PesoTotalPaletes = pesoTotalPaletes;
                pedido.TipoPaleteCliente = tipoPaleteCliente;
                pedido.PalletSaldoRestante += diferencaPallet;
                pedido.UltimaAtualizacao = DateTime.Now;

                if (!pedido.ItensAtualizados)
                    pedido.ItensAtualizados = itensAlterados;

                if (totalPalet > 0m)
                {
                    diferencaPallet = (totalPalet - pedido.NumeroPaletesFracionado);
                    pedido.NumeroPaletesFracionado = totalPalet;
                    pedido.PalletSaldoRestante += diferencaPallet;
                }

                if (totalCubagem > 0m)
                    pedido.CubagemTotal = totalCubagem;

                if (totalPeso > 0m)
                {
                    pedido.PesoTotal = totalPeso;

                    if (cargasPedido.Count == 0)
                        pedido.PesoSaldoRestante = totalPeso;
                }

                if (qtdVolumes > 0 && volumesAlterados)
                {
                    pedido.QtVolumes = qtdVolumes;
                    pedido.SaldoVolumesRestante = qtdVolumes;
                }

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoesPedido = pedido.GetChanges();

                await repositorioPedido.AtualizarAsync(pedido);

                if (codigoSessaoRoteirizador > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = (from o in carregamentosPedidoSessao
                                                                                                               where o.Pedido.Codigo == codigoPedido
                                                                                                               select o).FirstOrDefault();
                    if (carregamentoPedido != null)
                    {
                        if (carregamentoPedido.Carregamento.SituacaoCarregamento != SituacaoCarregamento.EmMontagem)
                            throw new ControllerException($"O Pedido {carregamentoPedido?.Pedido?.NumeroPedidoEmbarcador ?? string.Empty} não pode ser alterado pois está no carregamento {carregamentoPedido.Carregamento.NumeroCarregamento} com situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamentoHelper.ObterDescricao(carregamentoPedido.Carregamento.SituacaoCarregamento)}.");

                        carregamentosEnvolvidos.Add(carregamentoPedido.Carregamento.Codigo);

                        if (diferencaPallet > 0)
                        {
                            carregamentoPedido.Pallet -= diferencaPallet;
                            await repositorioCarregamentoPedido.AtualizarAsync(carregamentoPedido);
                        }

                        // Agora vamos atualizar tudo do carregamento...
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repositorioCarregamento.BuscarPorCodigoAsync(carregamentoPedido.Carregamento.Codigo, false);
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = await repositorioCarregamentoPedido.BuscarPorCarregamentoAsync(carregamento.Codigo);

                        carregamento.PesoCarregamento = (from o in carregamentoPedidos select o.Peso).Sum();
                        carregamento.PalletCarregamento = (from o in carregamentoPedidos select o.Pallet).Sum();

                        await repositorioCarregamento.AtualizarAsync(carregamento);
                    }
                }

                if (alteracoesPedido.Count > 0)
                {
                    if (codigoSessaoRoteirizador == 0)
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, pedido, alteracoesPedido, "Atualizado Pedido (Via Tela Gestão de Pedidos)", unitOfWork, AcaoBancoDados.Registro, cancellationToken);
                    else
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, pedido, alteracoesPedido, $"Atualizado Pedido (Via Tela Sessão Roteirização {codigoSessaoRoteirizador})", unitOfWork, AcaoBancoDados.Registro, cancellationToken);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(new
                {
                    CodigoPedido = pedido.Codigo,
                    Carregamentos = carregamentosEnvolvidos.Distinct(),
                    TipoPaleteCliente = tipoPaleteCliente,
                    NumeroPaletesFracionado = novaQtdePalletFracionado.ToString("n3"),
                    PesoTotalPaletes = pesoTotalPaletes.ToString("n2"),
                    Peso = pedido.PesoTotal.ToString("n4"),
                    PesoSaldoRestante = pedido.PesoSaldoRestante.ToString("n2")
                }, true, "Pedidos alterado com sucesso.");
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o pedido.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarQuantidadePedidosSessao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosPedidosSelecionados = Request.GetListParam<int>("CodigosPedidos");
                int sessao = Request.GetIntParam("SessaoRoteirizador");

                if (codigosPedidosSelecionados.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.DadosInvalidos);

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> pedidosSessao = repSessaoPedidos.PedidosSessaoRoteirizador(sessao, false);

                bool valido = pedidosSessao.Count == codigosPedidosSelecionados.Count;

                return new JsonpResult(valido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar quantidade pedidos sessão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarPedidosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosPedidos = Request.GetListParam<int>("CodigosPedidos");

                if (codigosPedidos.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.DadosInvalidos);

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repositorioRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCodigos(codigosPedidos);

                if (pedidos.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                List<IGrouping<int, Dominio.Entidades.Embarcador.Pedidos.Pedido>> grupoFiliais = pedidos.GroupBy(pedido => pedido.Filial.Codigo).ToList();

                if (grupoFiliais.Count > 1) return new JsonpResult(false, true, "Existem pedidos com filiais diferentes.");

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> sessoesPedidos = repositorioRoteirizadorPedido.BuscarSessoesPorPedidos(codigosPedidos);

                int grupoSessoes = sessoesPedidos.GroupBy(sp => sp.Total).Count();

                if (grupoSessoes > 1) return new JsonpResult(false, true, "Não é possivel roteirizar pedidos com sessoes diferentes.");

                int sessoes = (from o in sessoesPedidos
                               where o.Total > 0
                               select o.Total).Distinct().Count();

                if (sessoes > 0) return new JsonpResult(false, true, "Somente pedido sem sessão podem ser enviados para roteirização.");

                List<int> pedidosSemSessao = codigosPedidos.Except(sessoesPedidos.Select(o => o.Codigo)).ToList();

                Dominio.Entidades.Embarcador.Filiais.Filial filial = pedidos.FirstOrDefault().Filial;
                int sessao = sessoesPedidos.FirstOrDefault()?.Total ?? 0;
                List<string> codigosAgrupadores = pedidos.Where(pedido => !string.IsNullOrWhiteSpace(pedido.CodigoAgrupamentoCarregamento)).Select(pedido => pedido.CodigoAgrupamentoCarregamento).Distinct().ToList();

                dynamic dadosRetorno = new
                {
                    Filial = new { filial.Codigo, filial.Descricao },
                    Pedidos = codigosPedidos,
                    PedidosSemSessao = pedidosSemSessao,
                    CodigosAgrupadores = codigosAgrupadores,
                    SessaoRoteirizador = sessao
                };

                return new JsonpResult(dadosRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar roteirização de pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> TrocarFilial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosPedidos = Request.GetListParam<int>("CodigosPedidos");
                int codigoFilial = Request.GetIntParam("CodigoFilial");

                if (codigosPedidos.Count == 0 || codigoFilial == 0)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.DadosInvalidos);

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(codigoFilial) ?? throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);
                List<Pedido> pedidos = repositorioPedido.BuscarPorCodigos(codigosPedidos);

                if (pedidos.Count == 0)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                if (pedidos.Exists(pedido => pedido.Filial.Codigo == filial.Codigo))
                    throw new ControllerException("Não é possível atualizar para a mesma filial.");

                unitOfWork.Start();

                foreach (Pedido pedido in pedidos)
                {
                    pedido.Initialize();

                    pedido.Filial = filial;

                    repositorioPedido.Atualizar(pedido);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoesPedido = pedido.GetChanges();

                    if (alteracoesPedido.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, alteracoesPedido, "Atualizado Pedido (Via Tela Gestão de Pedidos)", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a filial dos pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> IntegrarPedidosRoutEasy()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosPedidos = Request.GetListParam<int>("CodigosPedidos");
                bool atualizacaoPedido = Request.GetBoolParam("AtualizacaoPedido");

                Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador servicoIntegracaoPedidoRoterizador = new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork, Auditado);

                TipoRoteirizadorIntegracao tipoRoteirizacaoIntegracao = atualizacaoPedido ? TipoRoteirizadorIntegracao.AtualizarPedido : TipoRoteirizadorIntegracao.EnviarPedido;

                servicoIntegracaoPedidoRoterizador.AdicionarEIntegrarPedidos(codigosPedidos, tipoRoteirizacaoIntegracao);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> IntegrarCancelamentoPedidosRoutEasy()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosPedidos = Request.GetListParam<int>("CodigosPedidos");

                Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador servicoIntegracaoPedidoRoterizador = new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork, Auditado);

                servicoIntegracaoPedidoRoterizador.AdicionarEIntegrarPedidos(codigosPedidos, TipoRoteirizadorIntegracao.CancelarPedido);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConsultarIntegracoesPedido(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPedido = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido repositorioRoteirizadorIntegracaoPedido = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido(unitOfWork, cancellationToken);

                Grid grid = new Grid(Request);

                grid.header = new List<Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data da Integração", "DataIntegracao", 20, Align.left, false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 20, Align.left, false);
                grid.AdicionarCabecalho("Tipo Integração", "Tipo", 20, Align.left, false);
                grid.AdicionarCabecalho("Situacao da Integração", "SituacaoIntegracao", 40, Align.left, false);
                grid.AdicionarCabecalho("Número de Tentativas", "NumeroTentativas", 20, Align.left, false);
                grid.AdicionarCabecalho("Problema Integração", "ProblemaIntegracao", 40, Align.left, false);

                List<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao> roteirizadorIntegracoes = await repositorioRoteirizadorIntegracaoPedido.BuscarRoteirizadorIntegracaoPorPedidoAsync(codigoPedido);
                grid.setarQuantidadeTotal(roteirizadorIntegracoes.Count);

                var retorno = (from obj in roteirizadorIntegracoes.OrderByDescending(o => o.DataIntegracao).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   DataIntegracao = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                   Usuario = obj.Usuario?.Descricao,
                                   Tipo = obj.Tipo.ObterDescricao(),
                                   SituacaoIntegracao = obj.SituacaoIntegracao.ObterDescricao(),
                                   obj.NumeroTentativas,
                                   obj.ProblemaIntegracao,
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRoteirizadorIntegracao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao repositorioRoteirizadorIntegracao = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao(unitOfWork, cancellationToken);

                Grid grid = new Grid(Request);

                grid.header = new List<Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Align.left, false);

                Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao integracao = await repositorioRoteirizadorIntegracao.BuscarPorCodigoAsync(codigoRoteirizadorIntegracao, false);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count);

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> DownloadArquivosIntegracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoArquivoTransacao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao repositorioRoteirizadorIntegracao = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao roteirizadorIntegracao = await repositorioRoteirizadorIntegracao.BuscarPorCodigoArquivoAsync(codigoArquivoTransacao);

                if (roteirizadorIntegracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado);

                Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoArquivo arquivoIntegracao = roteirizadorIntegracao.ArquivosTransacao.FirstOrDefault(o => o.Codigo == codigoArquivoTransacao);

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Gerais.Geral.NaoHaArquivosDisponiveisParaDownload);

                List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao> arquivosTransacao = new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>()
                {
                    arquivoIntegracao.ArquivoRequisicao,
                    arquivoIntegracao.ArquivoResposta
                };

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(arquivosTransacao);

                return Arquivo(arquivo, "application/zip", $"Arquivos Integração - {roteirizadorIntegracao.Tipo.ObterDescricao()}.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion Métodos Globais

        #region Métodos privados

        private async Task<Grid> ObterGridPesquisa(CancellationToken cancellationToken)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);

            Grid grid = await ObterGridPesquisaPedido(unitOfWork, cancellationToken);

            FiltroPesquisaGestaoPedido filtrosPesquisa = ObterFiltrosPesquisaPedido();
            int totalLinhasPedidos = await repositorioPedido.ContarConsultaPedidosGestaoPedidoAsync(filtrosPesquisa);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "GestaoPedido/Pesquisa", "grid-gestao-pedidos");
            Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciasGrid = gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo);
            grid.AplicarPreferenciasGrid(preferenciasGrid);

            if (totalLinhasPedidos == 0)
            {
                grid.AdicionaRows(new List<dynamic>() { });
                return grid;
            }

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarOuAgrupar);
            IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoPedido.PedidoGestaoPedido> pedidos = await repositorioPedido.ConsultaPedidosGestaoPedidoAsync(filtrosPesquisa, parametrosConsulta);

            grid.AdicionaRows(pedidos);
            grid.setarQuantidadeTotal(totalLinhasPedidos);

            return grid;
        }

        private FiltroPesquisaGestaoPedido ObterFiltrosPesquisaPedido()
        {
            List<int> codigosFiliais = new List<int>();
            List<double> codigosRemetente = new List<double>();
            List<int> codigosDestino = new List<int>();
            List<double> codigosDestinatario = new List<double>();
            List<int> codigosTipoCarga = new List<int>();

            int codigoFilial = Request.GetIntParam("Filial");
            double codigoRemetente = Request.GetDoubleParam("Remetente");
            int codigoDestino = Request.GetIntParam("Destino");
            int codigoDestinatario = Request.GetIntParam("Destinatario");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");

            if (codigoFilial > 0) codigosFiliais.Add(codigoFilial);
            if (codigoRemetente > 0) codigosRemetente.Add(codigoRemetente);
            if (codigoDestino > 0) codigosDestino.Add(codigoDestino);
            if (codigoDestinatario > 0) codigosDestinatario.Add(codigoDestinatario);
            if (codigoTipoCarga > 0) codigosTipoCarga.Add(codigoTipoCarga);

            FiltroPesquisaGestaoPedido filtrosPesquisa = new FiltroPesquisaGestaoPedido()
            {
                CodigosFilial = codigosFiliais,
                CodigosRemetente = codigosRemetente,
                CodigosDestino = codigosDestino,
                CodigosDestinatario = codigosDestinatario,
                CodigosTipoCarga = codigosTipoCarga,
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                ListaNumeroPedido = Request.GetListParam<int>("ListaNumeroPedido"),
                CodigoSessaoRoteirizador = Request.GetIntParam("NumeroSessao"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                NumeroCarregamento = Request.GetStringParam("NumeroCarregamento"),
                CodigoAgrupamentoCarregamento = Request.GetStringParam("CodigoAgrupamentoCarregamento"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigosRegiaoDestino = Request.GetListParam<int>("RegiaoDestino"),
                GrupoPessoaDestinatario = Request.GetIntParam("GrupoPessoaDestinatario"),
                CodigosVendedor = Request.GetListParam<int>("Vendedor"),
                CodigosGerente = Request.GetListParam<int>("Gerente"),
                CodigosSupervisor = Request.GetListParam<int>("Supervisor"),
                Situacao = Request.GetNullableEnumParam<SituacaoPedido>("Situacao"),
                SituacaoComercialPedido = Request.GetIntParam("SituacaoComercialPedido"),
                SituacaoPedido = Request.GetEnumParam<SituacaoPedidoGestaoPedido>("SituacaoPedido"),
                CodigoCanalEntrega = Request.GetIntParam("CanalEntrega"),
                EstadosOrigem = Request.GetListParam<string>("EstadoOrigem"),
                EstadosDestino = Request.GetListParam<string>("EstadoDestino"),
                SituacaoEstoqueProdutoArmazem = Request.GetEnumParam<SituacaoEstoqueProdutoArmazem>("SituacaoEstoqueProdutoArmazem"),
                SituacaoRoteirizadorIntegracao = Request.GetNullableEnumParam<SituacaoRoteirizadorIntegracao>("SituacaoRoteirizadorIntegracao"),
                Reentrega = Request.GetNullableBoolParam("Reentrega"),
            };

            return filtrosPesquisa;
        }

        private async Task<Grid> ObterGridPesquisaPedido(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy repositorioIntegracaoRouteasy = new Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy integracaoRouteasy = await repositorioIntegracaoRouteasy.BuscarPrimeiroRegistroAsync();
            bool possuiIntegracaoRouteasy = integracaoRouteasy?.PossuiIntegracao ?? false;

            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("SituacaoRoteirizadorIntegracao", false);
            grid.AdicionarCabecalho("SituacaoPedido", false);
            grid.AdicionarCabecalho("ReentregaSolicitada", false);
            grid.AdicionarCabecalho("Número Pedido Embarcador", "NumeroPedido", 10, Align.center, false);
            grid.AdicionarCabecalho("Número Carregamento", "NumeroCarregamento", 10, Align.center, false, !possuiIntegracaoRouteasy);
            grid.AdicionarCabecalho("Número Sessão", "SessaoRoteirizador", 10, Align.center, false, !possuiIntegracaoRouteasy);
            grid.AdicionarCabecalho("Situação Comercial", "SituacaoComercialPedido", 10, Align.center, false, false);

            if (possuiIntegracaoRouteasy)
                grid.AdicionarCabecalho("Situação Roteirizador Integração", "DescricaoSituacaoRoteirizadorIntegracao", 10, Align.center, false);

            grid.AdicionarCabecalho("Data Carregamento", "DataCarregamentoPedidoFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Align.left, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 10, Align.left, false);
            grid.AdicionarCabecalho("Destino", "DestinoFormatada", 10, Align.left, false, false);
            grid.AdicionarCabecalho("CEP Destino", "DestinoCep", 10, Align.left, true, false);
            grid.AdicionarCabecalho("Quantidade Pallets", "TotalPallets", 10, Align.left, false);
            grid.AdicionarCabecalho("Peso Pallets", "PesoTotalPaletes", 10, Align.left, false);
            grid.AdicionarCabecalho("Peso Total do Pedido", "PesoTotalPedido", 10, Align.left, false);
            grid.AdicionarCabecalho("Valor Total do Pedido", "ValorTotalPedidoFormatada", 10, Align.left, false);
            grid.AdicionarCabecalho("Protocolo Pedido", "Protocolo", 10, Align.left, false);
            grid.AdicionarCabecalho("Código Agrupador", "CodigoAgrupamentoCarregamento", 10, Align.left, false, false);
            grid.AdicionarCabecalho("Canal de Entrega", "CanalEntrega", 10, Align.left, false, false);
            grid.AdicionarCabecalho("Data Previsão de Entrega", "DataPrevisaoEntregaFormatada", 10, Align.center, false, false);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 15, Align.center, true, false);
            grid.AdicionarCabecalho("Número Cargas", "NumeroCargas", 15, Align.center, false, false);
            grid.AdicionarCabecalho("Reentrega", "ReentregaSolicitadaFormatada", 15, Align.center, false, false);

            return grid;
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion Métodos privados
    }
}
