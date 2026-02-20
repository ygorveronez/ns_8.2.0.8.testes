using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SGT.BackgroundWorkers;
using SGTAdmin.Controllers;
using System.Drawing;
using System.Text;

namespace SGT.WebAdmin.Controllers.Cargas.MontagemCarga
{
    [CustomAuthorize(new string[] { "BuscarResumoAprovacao", "DetalhesAutorizacao", "PesquisaAutorizacoes", "BuscarCarregamentos" }, "Cargas/MontagemCarga", "Cargas/MontagemCargaMapa")]
    public class MontagemCargaController : BaseController
    {
        #region Construtores

        public MontagemCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carregamento.NumeroCarregamento, "NumeroCarregamento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carregamento.Filial, "Filial", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carregamento.Transportador, "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carregamento.Destino, "Destinos", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carregamento.Destinatario, "Destinatarios", 20, Models.Grid.Align.left, false);

                if ((filtrosPesquisa.SituacoesCarregamento?.Count ?? 0) == 0)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 10, Models.Grid.Align.center, false);

                bool exibirHoraDataCarregamentoEDescarregamento = this.ConfiguracaoEmbarcador.InformaApoliceSeguroMontagemCarga || this.ConfiguracaoEmbarcador.InformaHorarioCarregamentoMontagemCarga;
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork, cancellationToken);
                int totalRegistros = await repositorioCarregamento.ContarConsultaAsync(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> listaCarregamento = (totalRegistros > 0) ? await repositorioCarregamento.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();

                var listaCarregamentoretornar = (
                    from p in listaCarregamento
                    select new
                    {
                        p.Codigo,
                        p.NumeroCarregamento,
                        p.Descricao,
                        Empresa = p.Empresa?.Descricao ?? "",
                        DataCarregamentoCarga = exibirHoraDataCarregamentoEDescarregamento ? (p.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "") : (p.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? ""),
                        Destinatarios = p.Destinatarios,
                        Destinos = p.Destinos,
                        Filial = p.Filiais,
                        Situacao = p.SituacaoCarregamento.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaCarregamentoretornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarCarregamentos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "asc",
                    InicioRegistros = Request.GetIntParam("Inicio"),
                    LimiteRegistros = Request.GetIntParam("Limite"),
                    PropriedadeOrdenar = "NumeroCarregamento"
                };

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork, cancellationToken);
                int totalRegistros = await repositorioCarregamento.ContarConsultaAsync(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = (totalRegistros > 0) ? await repositorioCarregamento.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();

                bool montagemCargaPorPedidoProduto = false;
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial> carregamentosDadosPorFiliais = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCarregamentos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidosCarregamentos = null; // new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao> roteirizacoes = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> clientesRotas = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete> simulacoesFrete = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete>();
                List<Dominio.Entidades.Embarcador.Logistica.Locais> balancas = new List<Dominio.Entidades.Embarcador.Logistica.Locais>();
                List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = null;
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira> carregamentosFronteiras = null;

                if (carregamentos?.Count > 0)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial repositorioCarregamentoFilial = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repositorioCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete repositorioSimulacaoFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete(unitOfWork, cancellationToken);
                    List<int> codigosCarregamentos = (from o in carregamentos select o.Codigo).Distinct().ToList();

                    pedidosCarregamentos = await repositorioCarregamentoPedido.BuscarPorCarregamentosAsync(codigosCarregamentos);

                    List<int> codigosPedidosCarregamentos = (from o in pedidosCarregamentos select o.Pedido.Codigo).Distinct().ToList();

                    carregamentosDadosPorFiliais = await repositorioCarregamentoFilial.BuscarPorCarregamentosAsync(codigosCarregamentos);
                    produtosPedidosCarregamentos = await repositorioCarregamentoPedidoProduto.BuscarPorPedidosAsync(codigosCarregamentos, codigosPedidosCarregamentos);
                    simulacoesFrete = await repositorioSimulacaoFrete.BuscarPorCarregamentosAsync(codigosCarregamentos);
                    roteirizacoes = await repositorioCarregamentoRoteirizacao.BuscarPorCarregamentosAsync(codigosCarregamentos);

                    if (roteirizacoes?.Count > 0)
                    {
                        List<int> codigosRoteirizacoes = (from item in roteirizacoes select item.Codigo).Distinct().ToList();
                        clientesRotas = await repositorioCarregamentoRoteirizacaoClientesRota.BuscarPorCarregamentoRoteirizacoesAsync(codigosRoteirizacoes);
                    }

                    if (pedidosCarregamentos?.Count > 0)
                    {
                        List<int> codigosFiliaisPedidos = (from cp in pedidosCarregamentos where cp.Pedido?.Filial != null select cp.Pedido.Filial.Codigo).Distinct().ToList();

                        if (codigosFiliaisPedidos?.Count > 0)
                        {
                            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork, cancellationToken);
                            Repositorio.Embarcador.Logistica.Locais repositorioLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork, cancellationToken);
                            Repositorio.Embarcador.Logistica.PracaPedagio repositorioPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork, cancellationToken);
                            centrosCarregamento = await repositorioCentroCarregamento.BuscarPorFiliaisAsync(codigosFiliaisPedidos);
                            montagemCargaPorPedidoProduto = centrosCarregamento.Exists(x => x.MontagemCarregamentoPedidoProduto);
                            balancas = await repositorioLocais.BuscarPorTipoDeLocalEFiliaisAsync(TipoLocal.Balanca, codigosFiliaisPedidos);
                            pracasPedagio = await repositorioPracaPedagio.BuscarTodosAtivasAsync();
                        }
                    }

                    carregamentosFronteiras = await new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira(unitOfWork, cancellationToken).BuscarPorCarregamentosAsync(codigosCarregamentos);
                }

                List<int> codigosPedidos = (from obj in pedidosCarregamentos
                                            select obj.Pedido.Codigo).Distinct().ToList();
                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork, cancellationToken);

                if (!montagemCargaPorPedidoProduto && pedidosCarregamentos != null)
                    pedidosProdutos = await repPedidoProduto.BuscarPorPedidosAsync(codigosPedidos);

                if (centrosCarregamento == null)
                {
                    List<int> cfiliais = (from cp in pedidosCarregamentos
                                          where cp.Pedido?.Filial != null
                                          select cp.Pedido.Filial.Codigo).Distinct().ToList();
                    if (cfiliais?.Count > 0)
                    {
                        Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                        centrosCarregamento = await repCentroCarregamento.BuscarPorFiliaisAsync(cfiliais);
                    }
                }

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                List<string> filiaisAtivas = await repFilial.BuscarListaCNPJAtivasAsync();

                List<dynamic> lista = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento in carregamentos)
                {
                    lista.Add(new
                    {
                        carregamento.Codigo,
                        carregamento.CarregamentoRedespacho,
                        GerandoCargaBackground = (carregamento.SituacaoCarregamento == SituacaoCarregamento.GerandoCargaBackground ? true : false),
                        carregamento.NumeroCarregamento,
                        Filial = carregamento.Filiais,
                        QuantidadeEntregras = 0,
                        Rotas = "",
                        Transportador = carregamento.Empresa?.Descricao ?? string.Empty,
                        Distancia = 0,
                        Peso = carregamento.PesoCarregamento,
                        DataProgramada = carregamento.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? "",
                        Carregamento = await ObterMontagemCarga(carregamento, pedidosProdutos, unitOfWork, cancellationToken, ConfiguracaoEmbarcador, false, pedidosCarregamentos, produtosPedidosCarregamentos, montagemCargaPorPedidoProduto, roteirizacoes, clientesRotas, balancas, carregamentosDadosPorFiliais, centrosCarregamento, filiaisAtivas, carregamentosFronteiras, pracasPedagio),
                        PedidoViagemNavio = carregamento.PedidoViagemNavio?.Codigo,
                        // Analisar para remover daqui essa informação pois a mesma será retornada dentro do .Carregamento.
                        Roteirizacao = ParseInfoCarregamentoRoteirizacao(roteirizacoes, clientesRotas, carregamento, pedidosCarregamentos, balancas, pracasPedagio),
                        Frete = ParseInfoCarregamentoSimulacaoFrete(simulacoesFrete, carregamento),
                        CanaisDeEntrega = ParseCanaisEntregaCarregamento(pedidosCarregamentos, carregamento),
                        TipoOperacao = carregamento.TipoOperacao?.Descricao ?? string.Empty,
                        ValorFreteMontagem = carregamento.ValorFrete.ToString("c")
                    });
                }

                return new JsonpResult(lista, totalRegistros);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> OcupacaoCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarregamento = Request.GetIntParam("Carregamento");
                int codigoCanalEntrega = Request.GetIntParam("CanalEntrega");

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repCarregamento.BuscarPorCodigoAsync(codigoCarregamento, false);

                if (carregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNaoEncontrado);

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidoProdutos = repCarregamentoPedidoProduto.BuscarPorCarregamento(codigoCarregamento);

                if (carregamentoPedidoProdutos.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> pedidosCanalEntrega = (
                        from pedido in carregamentoPedidoProdutos
                        where (pedido.CarregamentoPedido.Pedido?.CanalEntrega?.Codigo ?? 0) == codigoCanalEntrega
                        select pedido
                    ).ToList();

                    decimal ocupacaoCubicaPaletes = (carregamento.TipoDeCarga?.Paletizado ?? false) ? carregamento.ModeloVeicularCarga?.ObterOcupacaoCubicaPaletes() ?? 0m : 0m;
                    decimal totalCubagem = carregamentoPedidoProdutos.Sum(x => x.MetroCubico) + ocupacaoCubicaPaletes;

                    var info = new
                    {
                        TotalPeso = carregamentoPedidoProdutos.Sum(x => x.Peso),
                        TotalPesoPallet = carregamentoPedidoProdutos.Sum(x => x.Peso),
                        TotalPesoComPallet = carregamentoPedidoProdutos.Sum(x => x.Peso) + (from ped in carregamentoPedidoProdutos select ped.CarregamentoPedido).Distinct().Sum(x => x.PesoPallet),
                        TotalCubagem = totalCubagem,
                        TotalPallets = carregamentoPedidoProdutos.Sum(x => x.QuantidadePallet),
                        TotalPesoCanalEntrega = pedidosCanalEntrega.Sum(x => x.Peso),
                        TotalCubagemCanalEntrega = pedidosCanalEntrega.Sum(x => x.MetroCubico),
                        TotalPalletsCanalEntrega = pedidosCanalEntrega.Sum(x => x.QuantidadePallet),
                        TotalPesoPalletCanalEntrega = (from ped in carregamentoPedidoProdutos select ped.CarregamentoPedido).Distinct().Sum(x => x.PesoPallet)
                    };

                    return new JsonpResult(info);
                }
                else
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = await repCarregamentoPedido.BuscarPorCarregamentoAsync(codigoCarregamento);

                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCanalEntrega = (
                        from pedido in carregamentoPedidos
                        where (pedido.Pedido?.CanalEntrega?.Codigo ?? 0) == codigoCanalEntrega
                        select pedido
                    ).ToList();

                    decimal ocupacaoCubicaPaletes = (carregamento.TipoDeCarga?.Paletizado ?? false) ? carregamento.ModeloVeicularCarga?.ObterOcupacaoCubicaPaletes() ?? 0m : 0m;
                    decimal totalCubagem = carregamentoPedidos.Sum(x => x.Pedido.CubagemTotal) + ocupacaoCubicaPaletes;

                    var info = new
                    {
                        TotalPeso = carregamentoPedidos.Sum(x => x.Pedido.PesoTotal),
                        TotalPesoPallet = carregamentoPedidos.Sum(x => x.PesoPallet),
                        TotalPesoComPallet = carregamentoPedidos.Sum(x => x.Pedido.PesoTotal) + carregamentoPedidos.Sum(x => x.PesoPallet),
                        TotalCubagem = totalCubagem,
                        TotalPallets = carregamentoPedidos.Sum(x => x.Pedido.TotalPallets),
                        TotalPesoCanalEntrega = pedidosCanalEntrega.Sum(x => x.Peso),
                        TotalCubagemCanalEntrega = pedidosCanalEntrega.Sum(x => x.Pedido.CubagemTotal),
                        TotalPalletsCanalEntrega = pedidosCanalEntrega.Sum(x => x.Pallet),
                        TotalPesoPalletCanalEntrega = pedidosCanalEntrega.Sum(x => x.PesoPallet)
                    };

                    return new JsonpResult(info);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoCalcularOcupacaoDoVeiculo);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> OcupacaoCarregamentoLoja(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarregamento = Request.GetIntParam("Carregamento");
                int codigoCanalEntrega = Request.GetIntParam("CanalEntrega");
                long cliente = Request.GetLongParam("Cliente");

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repCarregamento.BuscarPorCodigoAsync(codigoCarregamento, false);

                if (carregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNaoEncontrado);

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCarregamento = await repCarregamentoPedido.BuscarPorCarregamentoAsync(codigoCarregamento);
                List<int> codigosPedidos = (from ped in pedidosCarregamento
                                            select ped.Pedido.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidosCarregamento = repCarregamentoPedidoProduto.BuscarPorPedidos(codigoCarregamento, codigosPedidos);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCanalEntrega = (
                    from pedido in pedidosCarregamento
                    where (pedido.Pedido?.CanalEntrega?.Codigo ?? 0) == codigoCanalEntrega
                    select pedido
                ).ToList();

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidosCanalEntrega = (
                    from produto in produtosPedidosCarregamento
                    where (produto.PedidoProduto.Pedido?.CanalEntrega?.Codigo ?? 0) == codigoCanalEntrega
                    select produto
                ).ToList();

                //Inicialmente vai receber todos os pedidos do carregamento, caso o canal de entrega seja informado.. 
                // iremos filtrar os pedidos do cliente no canal de entrega..
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> tmp = pedidosCarregamento;
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> tmpp = produtosPedidosCarregamento;

                if (codigoCanalEntrega > 0)
                {
                    tmp = pedidosCanalEntrega;
                    tmpp = produtosPedidosCanalEntrega;
                }

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosLoja = (
                    from pedido in tmp
                    where (pedido.Pedido?.Destinatario?.CPF_CNPJ ?? 0) == cliente
                    select pedido
                ).ToList();

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidosLoja = (
                    from produto in tmpp
                    where (produto.PedidoProduto.Pedido?.Destinatario?.CPF_CNPJ ?? 0) == cliente
                    select produto
                ).ToList();

                var totalPeso = pedidosCarregamento.Sum(x => x.Peso);
                var totalCubagem = pedidosCarregamento.Sum(x => x.Pedido.CubagemTotal);
                var totalPallets = pedidosCarregamento.Sum(x => x.Pedido.NumeroPaletes + x.Pedido.NumeroPaletesFracionado);
                var totalPesoCanalEntrega = pedidosLoja.Sum(x => x.Peso);
                var totalCubagemCanalEntrega = pedidosLoja.Sum(x => x.Pedido.CubagemTotal);
                var totalPalletsCanalEntrega = pedidosLoja.Sum(x => x.Pedido.NumeroPaletes + x.Pedido.NumeroPaletesFracionado);
                var ocupacaoCubicaPaletes = (carregamento.TipoDeCarga?.Paletizado ?? false) ? carregamento.ModeloVeicularCarga?.ObterOcupacaoCubicaPaletes() ?? 0m : 0m;

                if (produtosPedidosCarregamento?.Count > 0)
                {
                    totalPeso = produtosPedidosCarregamento.Sum(x => x.Peso);
                    totalCubagem = produtosPedidosCarregamento.Sum(x => x.MetroCubico);
                    totalPallets = produtosPedidosCarregamento.Sum(x => x.QuantidadePallet);
                    totalPesoCanalEntrega = produtosPedidosLoja.Sum(x => x.Peso);
                    totalCubagemCanalEntrega = produtosPedidosLoja.Sum(x => x.MetroCubico);
                    totalPalletsCanalEntrega = produtosPedidosLoja.Sum(x => x.QuantidadePallet);
                }

                totalCubagem += ocupacaoCubicaPaletes;

                var info = new
                {
                    TotalPeso = totalPeso,
                    TotalCubagem = totalCubagem,
                    TotalPallets = totalPallets,
                    TotalPesoCanalEntrega = totalPesoCanalEntrega,
                    TotalCubagemCanalEntrega = totalCubagemCanalEntrega,
                    TotalPalletsCanalEntrega = totalPalletsCanalEntrega
                };

                return new JsonpResult(info);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoCalcularOcupacaoDoVeiculo);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExportarListaResumoCarregamentos(CancellationToken cancellationToken)
        {
            try
            {
                var grid = await ObterGridResumoCarregamentos(cancellationToken);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        public async Task<IActionResult> ListaResumoCarregamentos(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridResumoCarregamentos(cancellationToken));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> ListasResumosSessaoRoteirizador(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoSessao = Request.GetIntParam("SessaoRoteirizador");

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repositorioSessao = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao = await repositorioSessao.BuscarPorCodigoAsync(codigoSessao, false);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentosSessao = await repositorioCarregamento.ConsultarAsync(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento()
                {
                    ProgramaComSessaoRoteirizador = true,
                    CodigoSessaoRoteirizador = codigoSessao
                }, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    LimiteRegistros = 1000
                });

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = await repositorioCentroCarregamento.BuscarPorFiliaisAsync(new List<int> { sessao?.Filial?.Codigo ?? 0 });

                List<int> codigosCarregamento = (from obj in carregamentosSessao
                                                 select obj.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos = await repositorioCarregamentoPedido.BuscarPorCarregamentosAsync(codigosCarregamento);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao> roteirizacoes = await repositorioCarregamentoRoteirizacao.BuscarPorCarregamentosAsync(codigosCarregamento);


                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = null;

                if (!string.IsNullOrEmpty(sessao?.Parametros))
                    sessaoRoteirizadorParametros = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros>(sessao.Parametros);

                dynamic frota = new List<dynamic>();
                dynamic cargas = new List<dynamic>();
                dynamic valorFrete = new List<dynamic>();

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoResumoCarregamento tipoResumo = centrosCarregamento.FirstOrDefault()?.TipoResumoCarregamento ?? TipoResumoCarregamento.ModeloValorFrete;

                if (tipoResumo == TipoResumoCarregamento.ModeloCargas)
                {
                    frota = (from obj in sessaoRoteirizadorParametros.DisponibilidadesFrota
                             select new
                             {
                                 obj.CodigoModeloVeicular,
                                 obj.DescricaoModeloVeicular,
                                 obj.QuantidadeUtilizar,
                                 QuantidadeUtilizado = (from car in carregamentosSessao
                                                        where car.ModeloVeicularCarga.Codigo == obj.CodigoModeloVeicular
                                                        select car.Codigo).Count(),
                                 QuantidadeDisponivel = obj.QuantidadeUtilizar - (from car in carregamentosSessao
                                                                                  where car.ModeloVeicularCarga.Codigo == obj.CodigoModeloVeicular
                                                                                  select car.Codigo).Count()
                             }).OrderBy(x => x.DescricaoModeloVeicular).ToList();

                    cargas = (from carga in carregamentosSessao
                              select new
                              {
                                  carga.NumeroCarregamento,
                                  DescricaoModeloVeicular = carga.ModeloVeicularCarga?.Descricao ?? string.Empty,
                                  NomeTransportadora = carga.Empresa?.Descricao ?? string.Empty,
                                  KM = roteirizacoes.Where(roteirizacao => roteirizacao.Carregamento.Codigo == carga.Codigo).FirstOrDefault()?.DistanciaKM.ToString("n2") ?? string.Empty,
                                  ValorMercadoria = (from carPed in carregamentosPedidos
                                                     where carPed.Carregamento.Codigo == carga.Codigo
                                                     select carPed.Pedido.ValorTotalNotasFiscais).Sum().ToString("n2"),
                                  Cubagem = (from carPed in carregamentosPedidos
                                             where carPed.Carregamento.Codigo == carga.Codigo
                                             select carPed.Pedido.CubagemTotal).Sum().ToString("n2"),
                                  PesoCarregamento = (from carPed in carregamentosPedidos
                                                      where carPed.Carregamento.Codigo == carga.Codigo
                                                      select carPed.Peso).Sum().ToString("n2"),
                                  TaxaOcupacaoPeso = ((100 / (carga.ModeloVeicularCarga?.CapacidadePesoTransporte ?? 1)) * (from carPed in carregamentosPedidos
                                                                                                                            where carPed.Carregamento.Codigo == carga.Codigo
                                                                                                                            select carPed.Peso).Sum()).ToString("n2") + "%",
                                  QtdeEntregas = (from carPed in carregamentosPedidos
                                                  where carPed.Carregamento.Codigo == carga.Codigo
                                                  select (carPed.Pedido.Recebedor != null ? carPed.Pedido.Recebedor.Codigo : carPed.Pedido.Destinatario.Codigo)).Distinct().Count(),
                                  RotaDeEntrega = string.Join(", ", (from carPed in carregamentosPedidos
                                                                     where carPed.Carregamento.Codigo == carga.Codigo
                                                                     select (carPed.Pedido.Recebedor != null ? carPed.Pedido.Recebedor.Localidade.Descricao : carPed.Pedido.Destinatario.Localidade.Descricao)).Distinct())

                              }).OrderBy(x => x.NumeroCarregamento).ToList();

                }
                else if (tipoResumo == TipoResumoCarregamento.ModeloValorFrete)
                {
                    decimal valorTotalFretes = (from ped in carregamentosPedidos
                                                select ped.Pedido.ValorFreteCotado).Sum();

                    valorFrete = (from carga in carregamentosSessao
                                  select new
                                  {
                                      carga.NumeroCarregamento,
                                      DescricaoModeloVeicular = carga.ModeloVeicularCarga?.Descricao ?? string.Empty,
                                      PesoCarregamento = (from carPed in carregamentosPedidos
                                                          where carPed.Carregamento.Codigo == carga.Codigo
                                                          select carPed.Peso).Sum(),
                                      ValorPedido = (from ped in carregamentosPedidos
                                                     where ped.Carregamento.Codigo == carga.Codigo
                                                     select ped.Pedido.ValorTotalCarga).Sum(),
                                      ValorFrete = (from ped in carregamentosPedidos
                                                    where ped.Carregamento.Codigo == carga.Codigo
                                                    select ped.Pedido.ValorFreteCotado).Sum(),
                                      QtdeEntregas = (from carPed in carregamentosPedidos
                                                      where carPed.Carregamento.Codigo == carga.Codigo
                                                      select (carPed.Pedido.Recebedor != null ? carPed.Pedido.Recebedor.Codigo : carPed.Pedido.Destinatario.Codigo)).Distinct().Count(),
                                      PercentualFrete = (100 / (valorTotalFretes > 0 ? valorTotalFretes : 1) * (from ped in carregamentosPedidos
                                                                                                                where ped.Carregamento.Codigo == carga.Codigo
                                                                                                                select ped.Pedido.ValorFreteCotado).Sum()).ToString("n2") + "%",

                                  }).OrderBy(x => x.NumeroCarregamento).ToList();
                }

                return new JsonpResult(new { tipoResumo, frota, cargas, valorFrete });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaBlocoCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("LinhasOrdemCarregamentoExpandir", false);
                grid.AdicionarCabecalho("LinhasCarregamentoExpandir", false);
                grid.AdicionarCabecalho("LinhasCarregamentoSegundoTrechoExpandir", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamento, "NumeroCarregamento", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Placa, "PlacaVeiculo", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.SequenciaCarregamento, "OrdemCarregamento", 4, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.SequenciaEntrega, "OrdemEntrega", 4, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamentoSegundoTrecho, "NumeroCarregamentoSegundoTrecho", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.PlacaSegundoTrecho, "PlacaVeiculoSegundoTrecho", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.SequenciaCarregamentoSegundoTrecho, "OrdemCarregamentoSegundoTrecho", 4, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.SequenciaEntregaSegundoTrecho, "OrdemEntregaSegundoTrecho", 4, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.TipoDeOperacao, "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Cliente, "Cliente", 16, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Cidade, "Cidade", 16, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Pedido, "Pedido", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Cubagem, "Cubagem", 6, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.CubagemBloco, "CubagemBloco", 6, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Peso, "Peso", 6, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.PesoBloco, "PesoBloco", 6, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Recebedor, "Recebedor", 6, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Bloco, "Bloco", 5, Models.Grid.Align.center, false).Editable(new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, "aa", 2));

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                int carregamento = Request.GetIntParam("Carregamento");
                Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(unitOfWork);
                int totalRegistros = repBlocoCarregamento.ContarPorCarregamento(carregamento);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> listaBloco = totalRegistros > 0 ? repBlocoCarregamento.BuscarPorCarregamento(carregamento) : new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();

                switch (parametrosConsulta.PropriedadeOrdenar)
                {
                    case "OrdemCarregamento":
                        listaBloco = (parametrosConsulta.DirecaoOrdenar == "asc" ? listaBloco.OrderBy(x => x.OrdemCarregamento).ThenBy(x => x.BlocoCarregamentoSegundoTrecho?.OrdemCarregamento).ToList() : listaBloco.OrderByDescending(x => x.OrdemCarregamento).ThenBy(x => x.BlocoCarregamentoSegundoTrecho?.OrdemCarregamento).ToList());
                        break;
                    case "OrdemEntrega":
                        listaBloco = (parametrosConsulta.DirecaoOrdenar == "asc" ? listaBloco.OrderBy(x => x.OrdemEntrega).ThenBy(x => x.BlocoCarregamentoSegundoTrecho?.OrdemEntrega).ToList() : listaBloco.OrderByDescending(x => x.OrdemEntrega).ThenBy(x => x.BlocoCarregamentoSegundoTrecho?.OrdemEntrega).ToList());
                        break;
                    default:
                        listaBloco = listaBloco.OrderBy(x => x.OrdemCarregamento).ThenBy(x => x.BlocoCarregamentoSegundoTrecho?.OrdemCarregamento).ToList();
                        break;
                }

                List<dynamic> listaBlocoRetornar = new List<dynamic>();
                Dictionary<string, decimal> pesoBloco = new Dictionary<string, decimal>();
                Dictionary<string, decimal> cubagemBloco = new Dictionary<string, decimal>();
                Dictionary<int, int> blocosAgrupadosPorOrdemCarregamento = new Dictionary<int, int>();
                Dictionary<int, int> blocosAgrupadosPorCarregamentoSegundoTrecho = new Dictionary<int, int>();

                var listaBlocosAgrupadosPorOrdemCarregamento = (from bloco in listaBloco group bloco by bloco.OrdemCarregamento into grupo select new { OrdemCarregamento = grupo.Key, TotalRegistros = grupo.Count() }).ToList();
                var listaBlocosAgrupadosPorCarregamentoSegundoTrecho = (from bloco in listaBloco where bloco.BlocoCarregamentoSegundoTrecho != null group bloco by bloco.BlocoCarregamentoSegundoTrecho.Codigo into grupo select new { CarregamentoSegundoTrecho = grupo.Key, TotalRegistros = grupo.Count() }).ToList();
                int linhasCarregamentoExpandir = listaBloco.Count();
                int qtdeRegistrosPorPagina = parametrosConsulta.LimiteRegistros;

                foreach (var bloco in listaBloco)
                {
                    pesoBloco.TryGetValue(bloco.Bloco, out decimal peso);
                    peso += bloco.Pedido.PesoTotal;
                    pesoBloco[bloco.Bloco] = peso;

                    cubagemBloco.TryGetValue(bloco.Bloco, out decimal cubagem);
                    cubagem += bloco.Pedido.CubagemTotal;
                    cubagemBloco[bloco.Bloco] = cubagem;

                    var blocoAgrupadoPorOrdemCarregamento = listaBlocosAgrupadosPorOrdemCarregamento.Where(o => o.OrdemCarregamento == bloco.OrdemCarregamento).FirstOrDefault();
                    int linhasOrdemCarregamentoExpandir = blocoAgrupadoPorOrdemCarregamento.TotalRegistros;

                    if (blocosAgrupadosPorOrdemCarregamento.ContainsKey(bloco.OrdemCarregamento))
                        linhasOrdemCarregamentoExpandir = 0;
                    else
                        blocosAgrupadosPorOrdemCarregamento.Add(bloco.OrdemCarregamento, linhasOrdemCarregamentoExpandir);

                    int linhasCarregamentoSegundoTrechoExpandir = 1;

                    if (bloco.BlocoCarregamentoSegundoTrecho != null)
                    {
                        var blocoAgrupadoPorCarregamentoSegundoTrecho = listaBlocosAgrupadosPorCarregamentoSegundoTrecho.Where(o => o.CarregamentoSegundoTrecho == bloco.BlocoCarregamentoSegundoTrecho.Codigo).FirstOrDefault();

                        linhasCarregamentoSegundoTrechoExpandir = blocoAgrupadoPorCarregamentoSegundoTrecho.TotalRegistros;

                        if (blocosAgrupadosPorCarregamentoSegundoTrecho.ContainsKey(bloco.BlocoCarregamentoSegundoTrecho.Codigo))
                            linhasCarregamentoSegundoTrechoExpandir = 0;
                        else
                            blocosAgrupadosPorCarregamentoSegundoTrecho.Add(bloco.BlocoCarregamentoSegundoTrecho.Codigo, linhasCarregamentoSegundoTrechoExpandir);
                    }

                    if (bloco.Pedido.NumeroPedidoEmbarcador == "0082000048")
                    {
                        var testes = "";
                    }

                    bool primeiroRegistroPagina = (listaBlocoRetornar.Count % qtdeRegistrosPorPagina == 0);

                    if (bloco.BlocoCarregamentoSegundoTrecho == null)
                    {
                        listaBlocoRetornar.Add(new
                        {
                            bloco.Codigo,
                            bloco.OrdemCarregamento,
                            bloco.OrdemEntrega,
                            bloco.Bloco,
                            Cliente = bloco.Pedido.Destinatario.Descricao,
                            Cidade = bloco.Pedido.Destinatario.Localidade.Descricao,
                            Pedido = bloco.Pedido.NumeroPedidoEmbarcador,
                            Recebedor = bloco.Pedido?.Recebedor?.Nome ?? string.Empty,
                            Expedidor = bloco.Pedido?.Expedidor?.Nome ?? string.Empty,
                            Peso = bloco.Pedido.PesoTotal.ToString("n2"),
                            PesoBloco = peso.ToString("n2"),
                            Cubagem = bloco.Pedido.CubagemTotal.ToString("n2"),
                            CubagemBloco = cubagem.ToString("n2"),
                            NumeroCarregamento = bloco.Carregamento.NumeroCarregamento,
                            PlacaVeiculo = bloco.Carregamento.Veiculo?.Placa ?? "",
                            TipoOperacao = bloco.Carregamento.TipoOperacao?.Descricao ?? "",
                            NumeroCarregamentoSegundoTrecho = "",
                            OrdemCarregamentoSegundoTrecho = "",
                            OrdemEntregaSegundoTrecho = "",
                            PlacaVeiculoSegundoTrecho = "",
                            TipoCargaSegundoTrecho = "",
                            LinhasOrdemCarregamentoExpandir = linhasOrdemCarregamentoExpandir,
                            LinhasCarregamentoExpandir = (primeiroRegistroPagina ? linhasCarregamentoExpandir : 0),
                            LinhasCarregamentoSegundoTrechoExpandir = linhasCarregamentoSegundoTrechoExpandir
                        });
                    }
                    else
                    {
                        listaBlocoRetornar.Add(new
                        {
                            bloco.Codigo,
                            bloco.OrdemCarregamento,
                            bloco.OrdemEntrega,
                            bloco.Bloco,
                            Cliente = bloco.Pedido.Destinatario.Descricao,
                            Cidade = bloco.Pedido.Destinatario.Localidade.Descricao,
                            Pedido = bloco.Pedido.NumeroPedidoEmbarcador,
                            Recebedor = bloco.Pedido?.Recebedor?.Nome ?? string.Empty,
                            Expedidor = bloco.Pedido?.Expedidor?.Nome ?? string.Empty,
                            Peso = bloco.Pedido.PesoTotal.ToString("n2"),
                            PesoBloco = peso.ToString("n2"),
                            Cubagem = bloco.Pedido.CubagemTotal.ToString("n2"),
                            CubagemBloco = cubagem.ToString("n2"),
                            NumeroCarregamento = bloco.Carregamento.NumeroCarregamento,
                            PlacaVeiculo = bloco.Carregamento.Veiculo?.Placa ?? "",
                            TipoOperacao = bloco.BlocoCarregamentoSegundoTrecho.Carregamento.TipoOperacao?.Descricao ?? "",
                            NumeroCarregamentoSegundoTrecho = bloco.BlocoCarregamentoSegundoTrecho.Carregamento.NumeroCarregamento,
                            OrdemCarregamentoSegundoTrecho = bloco.BlocoCarregamentoSegundoTrecho.OrdemCarregamento,
                            OrdemEntregaSegundoTrecho = bloco.BlocoCarregamentoSegundoTrecho.OrdemEntrega,
                            PlacaVeiculoSegundoTrecho = bloco.BlocoCarregamentoSegundoTrecho.Carregamento.Veiculo?.Placa ?? "",
                            LinhasOrdemCarregamentoExpandir = linhasOrdemCarregamentoExpandir,
                            LinhasCarregamentoExpandir = (primeiroRegistroPagina ? linhasCarregamentoExpandir : 0),
                            LinhasCarregamentoSegundoTrechoExpandir = linhasCarregamentoSegundoTrechoExpandir
                        });
                    }
                    if (primeiroRegistroPagina)
                        linhasCarregamentoExpandir -= (qtdeRegistrosPorPagina < linhasCarregamentoExpandir) ? qtdeRegistrosPorPagina : linhasCarregamentoExpandir;
                    //linhasCarregamentoExpandir = 0;
                }

                switch (parametrosConsulta.PropriedadeOrdenar)
                {
                    case "OrdemCarregamento":
                        listaBlocoRetornar = (parametrosConsulta.DirecaoOrdenar == "asc" ? listaBlocoRetornar.OrderBy(x => x.OrdemCarregamento).ThenBy(z => z.OrdemCarregamentoSegundoTrecho).ToList() : listaBlocoRetornar.OrderByDescending(x => x.OrdemCarregamento).ThenByDescending(z => z.OrdemCarregamentoSegundoTrecho).ToList());
                        break;
                    case "OrdemEntrega":
                        listaBlocoRetornar = (parametrosConsulta.DirecaoOrdenar == "asc" ? listaBlocoRetornar.OrderBy(x => x.OrdemEntrega).ThenBy(z => z.OrdemEntregaSegundoTrecho).ToList() : listaBlocoRetornar.OrderByDescending(x => x.OrdemEntrega).ThenByDescending(z => z.OrdemEntregaSegundoTrecho).ToList());
                        break;
                    default:
                        listaBlocoRetornar = listaBlocoRetornar.OrderBy(x => x.OrdemCarregamento).ThenBy(z => z.OrdemCarregamentoSegundoTrecho).ToList();
                        break;
                }

                listaBlocoRetornar = listaBlocoRetornar.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros).ToList();

                grid.AdicionaRows(listaBlocoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception escecao)
            {
                Servicos.Log.TratarErro(escecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarBlocos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarregamento = Request.GetIntParam("Carregamento");
                decimal pesoBlocos = Request.GetDecimalParam("Peso");
                decimal cubagem = Request.GetDecimalParam("Cubagem");
                Servicos.Embarcador.Carga.MontagemCarga.BlocoCarregamento servicoBlocoCarregamento = new Servicos.Embarcador.Carga.MontagemCarga.BlocoCarregamento(unitOfWork);

                servicoBlocoCarregamento.Gerar(codigoCarregamento, pesoBlocos, cubagem);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarPorCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCarga(codigo);
                var retorno = new
                {
                    encontrou = carregamento != null ? true : false,
                    carregamento = carregamento != null ? await ObterMontagemCarga(carregamento, null, unitOfWork, cancellationToken, this.ConfiguracaoEmbarcador) : null
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarPorPedido(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                bool redespacho = false;
                bool.TryParse(Request.Params("CarregamentoRedespacho"), out redespacho);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorPedido(codigo, redespacho);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repPedido.BuscarPorCodigoAsync(codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = repPedidoProduto.BuscarPorPedido(codigo);

                int codigoFilial = pedido?.Filial?.Codigo ?? 0;
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = codigoFilial > 0 ? repCentroCarregamento.BuscarPorFilial(codigoFilial) : null;

                var retorno = new
                {
                    encontrou = carregamento != null ? true : false,
                    carregamento = carregamento != null ? await ObterMontagemCarga(carregamento, produtos, unitOfWork, cancellationToken, this.ConfiguracaoEmbarcador) : null,
                    CodigoPedidoViagemNavio = pedido?.PedidoViagemNavio?.Codigo ?? 0,
                    DescricaoPedidoViagemNavio = pedido?.PedidoViagemNavio?.Descricao ?? "",
                    CentroCarregamento = centroCarregamento == null ? null : new
                    {
                        centroCarregamento.EscolherHorarioCarregamentoPorLista
                    }
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BaixarEDICarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repCarregamento.BuscarPorCodigoAsync(codigo, false);

                Dominio.ObjetosDeValor.EDI.Carregamento.Carga carga = new Dominio.ObjetosDeValor.EDI.Carregamento.Carga();
                carga.Pedidos = new List<Dominio.ObjetosDeValor.EDI.Carregamento.Pedido>();
                carga.CNPJEmbarcador = carregamento.Pedidos.FirstOrDefault().Pedido.Remetente.CPF_CNPJ_SemFormato;
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido pedidoCarregamento in carregamento.Pedidos)
                {
                    Dominio.ObjetosDeValor.EDI.Carregamento.Pedido pedido = new Dominio.ObjetosDeValor.EDI.Carregamento.Pedido();
                    pedido.NumeroRomaneio = carregamento.NumeroCarregamento;
                    pedido.NumeroPedido = pedidoCarregamento.Pedido.NumeroPedidoEmbarcador;
                    pedido.Peso = pedidoCarregamento.Pedido.PesoTotal;
                    pedido.SeriePedido = "P";
                    pedido.Situacao = "1";
                    pedido.Volumes = pedidoCarregamento.Pedido.QtVolumes;
                    pedido.Cubagem = pedidoCarregamento.Pedido.CubagemTotal;
                    carga.Pedidos.Add(pedido);
                }


                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.CONEMB, "CARREGAMENTO").FirstOrDefault();
                Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI);

                System.IO.MemoryStream edi = serGeracaoEDI.GerarArquivoRecursivo(carga);

                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(carregamento, layoutEDI, "");

                return Arquivo(edi, "plain/text", nomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoEnviarIntegracao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/MontagemCarga");
            bool usuarioPossuiPermissaoSobreporRegrasCarregamento = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.JanelaDescarga_SobreporRegras);

            try
            {
                //Programa MontagemCargaMapa, não passa o parametro "Codigo", passa todos os dados do carregamento para salvar.
                int codigoCarregamento = Request.GetIntParam("Codigo", 0);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = null;

                bool limparRoteirizacao = false;

                if (codigoCarregamento > 0)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                    carregamento = await repositorioCarregamento.BuscarPorCodigoAsync(codigoCarregamento, false);

                    if (IsMontagemCarregamentoSessaoFinalizada(carregamento))
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNumeroEstaEmUmaSessaoDeRoteirizacaoNaoPodeSerAlterado, carregamento.NumeroCarregamento, carregamento.SessaoRoteirizador.DescricaoSituacao));

                }
                else
                    carregamento = SalvarDadosCarregamento(unitOfWork, ref limparRoteirizacao);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork, configuracaoEmbarcador);

                if (limparRoteirizacao)
                    servicoMontagemCarga.RemoverDadosRelacionadosAosPedidos(carregamento, limparRoteirizacao);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = ObterConfiguracaoMontagemCarga(unitOfWork);

                ValidarCarregamento(carregamento, (carregamento.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false), configuracaoMontagemCarga, unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = await repCarregamentoPedido.BuscarPorCarregamentoAsync(carregamento.Codigo);
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = (from obj in carregamentoPedidos select obj.Pedido.Filial).Distinct().ToList();

                bool gerarCargaBackground = false;
                if ((carregamento.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false) == true)
                    gerarCargaBackground = true;

                if (!gerarCargaBackground)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
                    {
                        MontagemCargaPedidoProduto = Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto.Validar,
                        Usuario = Usuario,
                        PermitirGerarCargaSemJanelaDescarregamento = Request.GetBoolParam("PermitirGerarCargaSemJanelaDescarregamento"),
                        PermitirHorarioCarregamentoComLimiteAtingido = Request.GetBoolParam("PermitirHorarioCarregamentoComLimiteAtingido"),
                        PermitirHorarioCarregamentoInferiorAoAtual = Request.GetBoolParam("PermitirHorarioCarregamentoInferiorAoAtual"),
                        PermitirHorarioDescarregamentoComLimiteAtingido = Request.GetBoolParam("PermitirHorarioDescarregamentoComLimiteAtingido"),
                        PermitirHorarioDescarregamentoInferiorAoAtual = Request.GetBoolParam("PermitirHorarioDescarregamentoInferiorAoAtual")
                    };

                    await unitOfWork.StartAsync(cancellationToken);

                    Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaRetorno retorno = servicoMontagemCarga.GerarCarga(carregamento, filiais, carregamentoPedidos, TipoServicoMultisoftware, Cliente, Auditado, propriedades, ClienteAcesso.URLAcesso, permissoesPersonalizadas);
                    servicoMontagemCarga.VerificarPedidosColetaEntrega(carregamento);

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from obj in carregamentoPedidos select obj.Pedido).ToList();

                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, string.Format(Localization.Resources.Cargas.Carga.AdicionouPedidoCarga, pedido.CodigoCargaEmbarcador), unitOfWork);

                    servicoCarga.VisibilidadesDasCargas(pedidos, carregamento, unitOfWork, false);

                    if (carregamento.DataCarregamentoCarga.HasValue)
                        ProcessarPedidosEmSeparacao(pedidos, unitOfWork);

                    await unitOfWork.CommitChangesAsync(cancellationToken);

                    //#43261
                    // Devemos enviar um email, quando os pedidos possuem um transportador que esteja com a flag Contratante marcada...
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repositorioConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repositorioConfiguracaoTransportador.BuscarConfiguracaoPadrao();
                    int codigoTransportadoraPadraoContratacao = configuracaoTransportador?.TransportadorPadraoContratacao?.Codigo ?? 0;

                    bool existeTransportadoraPadraoContratacaoPedidos = carregamentoPedidos.Any(x => (x.Pedido?.Empresa?.Codigo ?? 0) == codigoTransportadoraPadraoContratacao);
                    if (existeTransportadoraPadraoContratacaoPedidos)
                    {
                        if ((configuracaoTransportador?.ExisteTransportadorPadraoContratacao ?? false) && (codigoTransportadoraPadraoContratacao == (configuracaoTransportador?.TransportadorPadraoContratacao?.Codigo ?? 0)) && ((carregamento?.Empresa?.Codigo ?? 0) != (configuracaoTransportador?.TransportadorPadraoContratacao?.Codigo ?? 0)))
                            Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEmailTransportadorRetira(carregamento, carregamentoPedidos, "Agendamento coleta", unitOfWork);
                    }

                    return new JsonpResult(new
                    {
                        retorno.CarregamentoAguardandoAprovacao,
                        retorno.NumerosCargasGeradas,
                        GerandoCargaBackground = false
                    });
                }
                else
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                    Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento repositorioMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from obj in carregamentoPedidos select obj.Pedido).ToList();

                    await unitOfWork.StartAsync(cancellationToken);

                    carregamento.SituacaoCarregamento = SituacaoCarregamento.GerandoCargaBackground;
                    await repositorioCarregamento.AtualizarAsync(carregamento);

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento montagemCarregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento()
                    {
                        Carregamento = carregamento
                    };
                    await repositorioMontagemCarregamento.InserirAsync(montagemCarregamento);

                    if (carregamento.DataCarregamentoCarga.HasValue)
                        ProcessarPedidosEmSeparacao(pedidos, unitOfWork);

                    await unitOfWork.CommitChangesAsync(cancellationToken);

                    return new JsonpResult(new
                    {
                        CarregamentoAguardandoAprovacao = false,
                        NumerosCargasGeradas = string.Empty,
                        GerandoCargaBackground = true
                    });
                }

            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                if (usuarioPossuiPermissaoSobreporRegrasCarregamento)
                {
                    if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.HorarioCarregamentoInferiorAtual)
                        return new JsonpResult(new { HorarioCarregamentoInferiorAtual = true }, true, excecao.Message);

                    if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.HorarioLimiteCarregamentoAtingido)
                        return new JsonpResult(new { HorarioLimiteCarregamentoAtingido = true }, true, excecao.Message);

                    if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.HorarioDescarregamentoInferiorAtual)
                        return new JsonpResult(new { HorarioDescarregamentoInferiorAtual = true }, true, excecao.Message);

                    if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.HorarioLimiteDescarregamentoAtingido)
                        return new JsonpResult(new { HorarioLimiteDescarregamentoAtingido = true }, true, excecao.Message);

                    if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.HorarioDescarregamentoIndisponivel)
                        return new JsonpResult(new { HorarioDescarregamentoIndisponivel = true }, true, Localization.Resources.Cargas.MontagemCargaMapa.NehumHorarioDeDescarregamentoDisponivel);
                }

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoGerarCarga);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarCargaEmLote(CancellationToken cancellationToken)
        {
            List<int> codigosCarregamento = Request.GetListParam<int>("codigosCarregamento");

            if (codigosCarregamento.Count == 0)
                return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NenhumCarregamentoSelecionadoParaGerarAs + " ");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoSessaoRoteirizador = Request.GetIntParam("SessaoRoteirizador");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = ObterConfiguracaoMontagemCarga(unitOfWork);

                codigosCarregamento = (from codigo in codigosCarregamento select codigo).Distinct().ToList();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamentos(codigosCarregamento);

                if (carga != null)
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.NaoPossivelGerarCargasJaExistemCargasGeradas);

                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento repositorioMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento primeiroCarregamento = repositorioMontagemCarregamento.BuscarPrimerioCarregamento(codigoSessaoRoteirizador);

                if (primeiroCarregamento != null)
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemCargasSendoProcessadasAguardeAlgunsInstantes);

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = repositorioCarregamento.BuscarCodigos(codigosCarregamento);

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento in carregamentos)
                {
                    ValidarCarregamento(carregamento, (carregamento?.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false), configuracaoMontagemCarga, unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento montagemCarregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento()
                    {
                        Carregamento = carregamento
                    };
                    await repositorioMontagemCarregamento.InserirAsync(montagemCarregamento);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoGerarCarga);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarCarregamentoAutomatico(CancellationToken cancellationToken)
        {
            bool cancelarGerarNovamenteCarregamentosAutomaticos = Request.GetBoolParam("CancelarGerar");
            List<int> codigosCarregamentosCancelar = Request.GetListParam<int>("CodigosCarregamentosCancelar");

            if (cancelarGerarNovamenteCarregamentosAutomaticos && codigosCarregamentosCancelar.Count > 0)
            {
                string msg = string.Empty;
                bool valida = false;
                var result = this.CancelarCarregamentos(codigosCarregamentosCancelar, ref valida, ref msg);
                if (!result)
                    return new JsonpResult(false, valida, msg);
            }

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosPedidos = Request.GetListParam<int>("pedidos");
                TipoMontagemCarregamentoPedidoProduto tipoMontagemCarregamentoPedidoProduto = Request.GetEnumParam<TipoMontagemCarregamentoPedidoProduto>("TipoMontagemCarregamentoPedidoProduto", TipoMontagemCarregamentoPedidoProduto.AMBOS);
                PrioridadeMontagemCarregamentoPedidoProduto prioridadeMontagemCarregamentoPedidoProduto = Request.GetEnumParam<PrioridadeMontagemCarregamentoPedidoProduto>("PrioridadeMontagemCarregamentoPedidoProduto", PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoPedido);
                PrioridadeMontagemCarregamentoPedido prioridadeMontagemCarregamentoPedido = Request.GetEnumParam<PrioridadeMontagemCarregamentoPedido>("PrioridadeMontagemCarregamentoPedido", PrioridadeMontagemCarregamentoPedido.PrevisaoEntregaCanalEntrega);
                TipoStatusEstoqueMontagemCarregamentoPedidoProduto tipoStatusEstoqueMontagemCarregamentoPedidoProduto = Request.GetEnumParam<TipoStatusEstoqueMontagemCarregamentoPedidoProduto>("TipoStatusEstoque", TipoStatusEstoqueMontagemCarregamentoPedidoProduto.Ambos);
                int codigoSessaoRoteirizador = Request.GetIntParam("SessaoRoteirizador");

                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido repMontagemCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repSessao = null;

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador = null;

                if (codigosPedidos.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NenhumPedidoSelcionadoParaGerarCarregamento);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido primereiroCarregamentoPedido = await repMontagemCarregamentoPedido.BuscarPrimerioCarregamentoPedidoAsync(codigoSessaoRoteirizador);

                if (primereiroCarregamentoPedido != null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.ExistemCarregamentosSendoProcessadosAguardeAlgunsInstantes);

                if (!LongRunningProcessFactory.Instance.IsProccessActive("IntegracaoMontagemCarga"))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.ProcessoDeGeracaoAutomaticaDeCarregamentosDesabilitadoNesteAmbientePorFavorSoliciteAtivacaoParaProsseguirComProcesso);

                List<int> pedidos = new List<int>();
                bool validarPedidosEmCarregamentos = true;
                if (codigoSessaoRoteirizador > 0)
                {
                    repSessao = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork, cancellationToken);
                    sessaoRoteirizador = await repSessao.BuscarPorCodigoAsync(codigoSessaoRoteirizador, true);

                    if ((sessaoRoteirizador?.Filial?.Codigo ?? 0) > 0)
                    {
                        List<int> codigoFiliais = new List<int>() { sessaoRoteirizador.Filial.Codigo };

                        Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork, cancellationToken);
                        List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = await repCentroCarregamento.BuscarPorFiliaisAsync(codigoFiliais);
                        bool montagemCargaPorPedidoProduto = centrosCarregamento.Exists(x => x.MontagemCarregamentoPedidoProduto == true);
                        if (montagemCargaPorPedidoProduto)
                            validarPedidosEmCarregamentos = false;
                    }
                }

                if (validarPedidosEmCarregamentos)
                {
                    pedidos = await repPedidos.BuscarCodigosPorCodigosSemCarregamentoAsync(codigosPedidos, codigoSessaoRoteirizador);
                    if (pedidos.Count == 0)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.JaFoiGeradoCarregamentoParaOsPedidos);

                    codigosPedidos = (from ped in pedidos select ped).Distinct().ToList();
                }

                if (codigoSessaoRoteirizador > 0)
                {

                    if (sessaoRoteirizador.SituacaoSessaoRoteirizador != SituacaoSessaoRoteirizador.Iniciada)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarCarregamentosParaUmaSessaoDeRoteirizacaoComSituacaoDiferenteDeIniciada);

                    if (sessaoRoteirizador.UsuarioAtual != null && sessaoRoteirizador.UsuarioAtual.Codigo != this.Usuario.Codigo)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarCarregamentosAutomaticosPoisSessaoDeRoteirizacaoEstaAbertaPeloUsuario, sessaoRoteirizador.UsuarioAtual.Nome));

                    if (sessaoRoteirizador.TipoMontagemCarregamentoPedidoProduto != tipoMontagemCarregamentoPedidoProduto ||
                        sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto != prioridadeMontagemCarregamentoPedidoProduto ||
                        sessaoRoteirizador.PrioridadeMontagemCarregamentoPedido != prioridadeMontagemCarregamentoPedido ||
                        sessaoRoteirizador.TipoStatusEstoqueMontagemCarregamentoPedidoProduto != tipoStatusEstoqueMontagemCarregamentoPedidoProduto)
                    {
                        sessaoRoteirizador.TipoMontagemCarregamentoPedidoProduto = tipoMontagemCarregamentoPedidoProduto;
                        sessaoRoteirizador.PrioridadeMontagemCarregamentoPedidoProduto = prioridadeMontagemCarregamentoPedidoProduto;
                        sessaoRoteirizador.PrioridadeMontagemCarregamentoPedido = prioridadeMontagemCarregamentoPedido;
                        sessaoRoteirizador.TipoStatusEstoqueMontagemCarregamentoPedidoProduto = tipoStatusEstoqueMontagemCarregamentoPedidoProduto;
                        await repSessao.AtualizarAsync(sessaoRoteirizador);
                    }
                }

                await unitOfWork.StartAsync(cancellationToken);

                await repMontagemCarregamentoPedido.InserirAsync(sessaoRoteirizador, codigosPedidos);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoGerarCarregamentoAutomatico);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> CancelarCarregamento()
        {
            int codigo = Request.GetIntParam("Codigo");
            string msg = string.Empty;
            bool valida = false;
            var result = this.CancelarCarregamentos(new List<int> { codigo }, ref valida, ref msg);
            if (!result)
                return new JsonpResult(false, valida, msg);
            return new JsonpResult(true);
        }

        public async Task<IActionResult> CancelarCarregamentos()
        {
            List<int> codigos = Request.GetListParam<int>("Codigos");
            string msg = string.Empty;
            bool valida = false;
            var result = this.CancelarCarregamentos(codigos, ref valida, ref msg);
            if (!result)
                return new JsonpResult(false, valida, msg);
            return new JsonpResult(true);
        }

        public async Task<IActionResult> RemoverPedidosCarregamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                List<int> codigosPedidos = Request.GetListParam<int>("PedidosCodigo");

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repCarregamento.BuscarPorCodigoAsync(codigo, true) ?? throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoInformadoNaoFoiLocalizada);

                if (carregamento.SituacaoCarregamento == SituacaoCarregamento.EmMontagem)
                {
                    var carga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork).BuscarPorCarregamentos(new List<int>() { codigo });
                    if (carga != null)
                        return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoJaPossuiUmaCargaNaoPermitidoRemoverEntregas);
                    else
                    {
                        if (carregamento.SessaoRoteirizador != null)
                            if (carregamento.SessaoRoteirizador.UsuarioAtual.Codigo != this.Usuario.Codigo)
                                return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoRemoverPedidosDoCarregamentoPoisSessaoDeRoteirizacaoEstaAbertaPeloUsuario, carregamento.SessaoRoteirizador.UsuarioAtual.Nome));

                        await unitOfWork.StartAsync(cancellationToken);
                        Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = await repPedido.BuscarPorCodigosAsync(codigosPedidos);

                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = await repCarregamentoPedido.BuscarPorCarregamentoAsync(carregamento.Codigo);
                        bool removeu = false;
                        bool montagemPorPedidoProduto = IsMontagemCargaPorPedido(pedidos, unitOfWork);

                        for (int i = 0; i < codigosPedidos.Count; i++)
                        {
                            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = carregamentoPedidos.Find(x => x.Pedido.Codigo == codigosPedidos[i]);
                            if (carregamentoPedido != null)
                            {
                                removeu = true;
                                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
                                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto = repositorioCarregamentoPedidoProduto.BuscarPorCarregamentoPedido(carregamentoPedido.Codigo);
                                repCarregamentoPedido.ExcluirCarregamentoPedido(carregamentoPedido.Codigo);
                            }
                        }

                        if (removeu)
                            servicoMontagemCarga.RemoverDadosRelacionadosAosPedidos(carregamento, true);

                        await unitOfWork.CommitChangesAsync(cancellationToken);
                    }
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoRemoverPedidoDeUmCarregamentoComSituacao, carregamento.SituacaoCarregamento.ToString()));

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoRemoverPedidoDoCarregamento);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AdicionarPedidosCarregamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                List<int> pedidos = Request.GetListParam<int>("PedidosCodigo");

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                var carregamento = await repCarregamento.BuscarPorCodigoAsync(codigo, true) ?? throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoInformadoNaoFoiLocalizado);
                if (carregamento.SituacaoCarregamento == SituacaoCarregamento.EmMontagem)
                {
                    bool montagemCarregamentoPedidoProduto = false;
                    if (carregamento.SessaoRoteirizador != null)
                    {
                        montagemCarregamentoPedidoProduto = carregamento.SessaoRoteirizador.MontagemCarregamentoPedidoProduto;
                        if (carregamento.SessaoRoteirizador.UsuarioAtual.Codigo != this.Usuario.Codigo)
                            return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAdicionarPedidosAoCarreagmentoPoisSessaoDeRoteirizacaoEstaAbertaPeloUsuario, carregamento.SessaoRoteirizador.UsuarioAtual.Nome));
                    }

                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
                    Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                    Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosExiste = carregamento.Pedidos != null ? carregamento.Pedidos.ToList() : new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPorPedidos = repCarregamentoPedido.BuscarOutroCarregamentoPorPedidos(pedidos, carregamento.CarregamentoRedespacho, carregamento.Codigo, carregamento.CarregamentoColeta);
                    if (carregamentoPorPedidos != null)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PedidoJaEstaSendoUtilizadoNoCarregamento, carregamentoPorPedidos.Pedido.NumeroPedidoEmbarcador, carregamentoPorPedidos.Carregamento.NumeroCarregamento));

                    var carga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork).BuscarPorCarregamentos(new List<int>() { codigo });
                    if (carga != null)
                        return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoJaPossuiUmaCargaNaoPemritidoAdicionarEntregas);
                    else
                    {
                        await unitOfWork.StartAsync(cancellationToken);
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);
                        bool adicionou = false;

                        Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                        List<string> filiais = await repFilial.BuscarListaCNPJAtivasAsync();

                        Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> tiposPallet = repositorioTipoDetalhe.BuscarPorTipo(TipoTipoDetalhe.TipoPallet);

                        for (int i = 0; i < pedidos.Count; i++)
                        {
                            var codigoPedido = pedidos[i];
                            var pedido = await repPedido.BuscarPorCodigoAsync(codigoPedido);
                            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = (from obj in carregamentoPedidosExiste where obj.Pedido.Codigo == codigoPedido select obj).FirstOrDefault();

                            if (carregamentoPedido == null)
                            {
                                carregamentoPedido = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido
                                {
                                    Carregamento = carregamento,
                                    Pedido = pedido,
                                    NumeroReboque = NumeroReboque.SemReboque,
                                    Peso = pedido.PesoTotal,
                                    Pallet = pedido.TotalPallets,
                                    TipoCarregamentoPedido = TipoCarregamentoPedido.Normal
                                };

                                if ((pedido?.TipoPaleteCliente ?? TipoPaleteCliente.NaoDefinido) != TipoPaleteCliente.NaoDefinido)
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhePalete = (from o in tiposPallet where o.TipoPaleteCliente == pedido.TipoPaleteCliente select o).FirstOrDefault();
                                    if (tipoDetalhePalete != null)
                                        carregamentoPedido.PesoPallet = (carregamentoPedido.Pallet * tipoDetalhePalete?.Valor ?? 0);
                                }

                                await repCarregamentoPedido.InserirAsync(carregamentoPedido, null, null);// historicoObjeto != null ? Auditado : null, historicoObjeto);
                                adicionou = true;

                                //Vamos adicionar os produtos do pedido..
                                foreach (var produto in pedido.Produtos)
                                {
                                    decimal metro = produto.MetroCubico;
                                    decimal peso = produto.PesoTotal;
                                    decimal qtde = produto.Quantidade;
                                    decimal pallet = produto.QuantidadePalet;
                                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto()
                                    {
                                        CarregamentoPedido = carregamentoPedido,
                                        MetroCubico = metro,
                                        PedidoProduto = produto,
                                        Peso = peso,
                                        Quantidade = qtde,
                                        QuantidadePallet = pallet,
                                        QuantidadeOriginal = qtde,
                                        QuantidadePalletOriginal = pallet,
                                        MetroCubicoOriginal = metro
                                    };
                                    await repCarregamentoPedidoProduto.InserirAsync(carregamentoPedidoProduto);
                                }
                            }

                        }

                        servicoCarga.VisibilidadesDasCargas(await repPedido.BuscarPorCodigosAsync(pedidos), carregamento, unitOfWork, false);

                        if (adicionou)
                            servicoMontagemCarga.RemoverDadosRelacionadosAosPedidos(carregamento, true);

                        await unitOfWork.CommitChangesAsync(cancellationToken);
                    }
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAdicionarPedidosUmCarregamentoComSituacao, carregamento.SituacaoCarregamento.ToString()));

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoAdicionarPedidoAoCarregamento);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AdicionarPedidosCarregamentoBipagem(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoPedido = Request.GetIntParam("CodigoPedido");
                decimal BipagemVolume = Request.GetDecimalParam("BipagemPedido");
                decimal BipagemVolumeTotal = Request.GetDecimalParam("BipagemPedidoTotal");

                Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                var carregamento = await repCarregamento.BuscarPorCodigoAsync(codigo, true) ?? throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNaoFoiLocalizado);
                if (carregamento.SituacaoCarregamento == SituacaoCarregamento.EmMontagem)
                {
                    if (carregamento.SessaoRoteirizador != null)
                    {
                        if (carregamento.SessaoRoteirizador.UsuarioAtual.Codigo != this.Usuario.Codigo)
                            return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAdicionarPedidosAoCarregamentoPoisSessaoDeRoteirizacaoEstaAbertaPeloUsuario, carregamento.SessaoRoteirizador.UsuarioAtual.Nome));
                    }

                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
                    Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosExiste = carregamento.Pedidos != null ? carregamento.Pedidos.ToList() : new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
                    List<int> pedidos = new List<int>();
                    pedidos.Add(codigoPedido);

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPorPedidos = repCarregamentoPedido.BuscarOutroCarregamentoPorPedidos(pedidos, carregamento.CarregamentoRedespacho, carregamento.Codigo, carregamento.CarregamentoColeta);
                    if (carregamentoPorPedidos != null)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PedidoJaEstaSendoUtilizadoNoCarregamento, carregamentoPorPedidos.Pedido.NumeroPedidoEmbarcador, carregamentoPorPedidos.Carregamento.NumeroCarregamento));

                    var carga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork).BuscarPorCarregamentos(new List<int>() { codigo });
                    if (carga != null)
                        return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoJaPossuiUmaCargaNaoPremitidoAdicionarEntregas);
                    else
                    {
                        await unitOfWork.StartAsync(cancellationToken);
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);
                        bool adicionou = false;
                        var pedido = await repPedido.BuscarPorCodigoAsync(codigoPedido);
                        // O serviço serPedido.ObterDetalhesPedidoParaMontagemCarga, retorna os valores padrão para o numeroReboque e TipoCarregamentoPedido
                        NumeroReboque numeroReboque = NumeroReboque.SemReboque;
                        TipoCarregamentoPedido tipoCarregamentoPedido = TipoCarregamentoPedido.Normal;

                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = (from obj in carregamentoPedidosExiste where obj.Pedido.Codigo == codigoPedido select obj).FirstOrDefault();

                        if (carregamentoPedido == null)
                        {
                            carregamentoPedido = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido
                            {
                                Carregamento = carregamento,
                                Pedido = pedido,
                                NumeroReboque = numeroReboque,
                                Peso = pedido.PesoTotal,
                                Pallet = pedido.TotalPallets,
                                VolumeBipado = BipagemVolume,
                                VolumeTotal = BipagemVolumeTotal,
                                TipoCarregamentoPedido = tipoCarregamentoPedido
                            };

                            if ((pedido?.TipoPaleteCliente ?? TipoPaleteCliente.NaoDefinido) != TipoPaleteCliente.NaoDefinido)
                            {
                                List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> tiposPallet = repositorioTipoDetalhe.BuscarPorTipo(TipoTipoDetalhe.TipoPallet);
                                Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhePalete = (from o in tiposPallet where o.TipoPaleteCliente == pedido.TipoPaleteCliente select o).FirstOrDefault();
                                if (tipoDetalhePalete != null)
                                    carregamentoPedido.PesoPallet = (carregamentoPedido.Pallet * tipoDetalhePalete?.Valor ?? 0);
                            }

                            await repCarregamentoPedido.InserirAsync(carregamentoPedido, null, null);
                            adicionou = true;

                            //Vamos adicionar os produtos do pedido..
                            foreach (var produto in pedido.Produtos)
                            {
                                decimal metro = produto.MetroCubico;
                                decimal peso = produto.PesoTotal;
                                decimal qtde = produto.Quantidade;
                                decimal pallet = produto.QuantidadePalet;
                                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto()
                                {
                                    CarregamentoPedido = carregamentoPedido,
                                    MetroCubico = metro,
                                    PedidoProduto = produto,
                                    Peso = peso,
                                    Quantidade = qtde,
                                    QuantidadePallet = pallet,
                                    QuantidadeOriginal = qtde,
                                    QuantidadePalletOriginal = pallet,
                                    MetroCubicoOriginal = metro
                                };
                                await repCarregamentoPedidoProduto.InserirAsync(carregamentoPedidoProduto);
                            }
                        }
                        else
                        {
                            carregamentoPedido.VolumeTotal = BipagemVolumeTotal;
                            carregamentoPedido.VolumeBipado = BipagemVolume;
                            await repCarregamentoPedido.AtualizarAsync(carregamentoPedido, null, null);
                        }

                        if (adicionou)
                            servicoMontagemCarga.RemoverDadosRelacionadosAosPedidos(carregamento, true);

                        await unitOfWork.CommitChangesAsync(cancellationToken);
                    }
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAdicionarPedidosUmCarregamentoComSituacao, carregamento.SituacaoCarregamento.ToString()));

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoAdicionarPedidoAoCarregamento);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AlterarBloco(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarregamento = Request.GetIntParam("Carregamento");
                int codigoBloco = Request.GetIntParam("Codigo");
                string bloco = Request.GetStringParam("Bloco").ToUpper();

                Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repositorioBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento blocoCarregamentos = repositorioBlocoCarregamento.BuscarPorCarregamentoECodigo(codigoCarregamento, codigoBloco);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repositorioCarregamento.BuscarPorCodigoAsync(codigoCarregamento, false);

                if (blocoCarregamentos == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (blocoCarregamentos.Carregamento.SituacaoCarregamento == SituacaoCarregamento.AguardandoAprovacaoSolicitacao)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoEstaAguardandoAprovacaoParaGerarCargaNaoPodeSerAlterado);

                await unitOfWork.StartAsync(cancellationToken);

                string blocoAnterior = blocoCarregamentos.Bloco;
                blocoCarregamentos.Bloco = bloco;

                if (blocoCarregamentos.BlocoCarregamentoSegundoTrecho != null)
                {
                    blocoCarregamentos.BlocoCarregamentoSegundoTrecho.Bloco = bloco;

                    await repositorioBlocoCarregamento.AtualizarAsync(blocoCarregamentos.BlocoCarregamentoSegundoTrecho);
                }

                await repositorioBlocoCarregamento.AtualizarAsync(blocoCarregamentos);

                string msgAuditoria = $"Bloco do carregamento {blocoCarregamentos.Pedido.NumeroPedidoEmbarcador} alterado de {blocoAnterior} para {bloco}";
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carregamento, msgAuditoria, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarMontagemPorPedido);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> RemoverProdutosCarregamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarregamento = Request.GetIntParam("Carregamento");
                long cliente = Request.GetLongParam("Cliente");
                List<int> codigos = Request.GetListParam<int>("Codigos");

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repostiorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosClienteCarregamento = await repositorioCarregamentoPedidoProduto.BuscarPorCarregamentoEDestinatarioAsync(codigoCarregamento, cliente);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = (from car in produtosClienteCarregamento select car.CarregamentoPedido.Carregamento).FirstOrDefault();

                if (IsMontagemCarregamentoSessaoFinalizada(carregamento))
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNumeroEstaEmUmaSessaoDeRoteirizacaoNaoPodeSerAlterado, carregamento.NumeroCarregamento, carregamento.SessaoRoteirizador.DescricaoSituacao));

                if (carregamento?.SessaoRoteirizador != null)
                    if (carregamento.SessaoRoteirizador.UsuarioAtual.Codigo != this.Usuario.Codigo)
                        return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoRemoverProdutosDoCarregamentoPoisSessaoDeRoteirizacaoEstaAbertaPeloUsuario, carregamento.SessaoRoteirizador.UsuarioAtual.Nome));

                if (carregamento.SituacaoCarregamento != SituacaoCarregamento.EmMontagem)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoRemoverProdutosDeUmCarregamentoComSituacao, carregamento.SituacaoCarregamento.ToString()));

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> remover = (from item in produtosClienteCarregamento
                                                                                                             orderby item.CarregamentoPedido.Pedido.Codigo
                                                                                                             where codigos.Contains(item.Codigo)
                                                                                                             select item).ToList();

                List<int> pedidosRemovidosCarregamento = new List<int>();

                if (remover?.Count > 0)
                {
                    IList<double> destinatariosExistentes = repostiorioCarregamentoPedido.DestinatariosCarregamento(codigoCarregamento);

                    //Filtrando os carregamentos pedido para remover ou atualizar a qtde...
                    List<int> codigosCarregamentoPedido = (from carregamentoPedido in remover
                                                           select carregamentoPedido.CarregamentoPedido.Codigo).Distinct().ToList();

                    await unitOfWork.StartAsync(cancellationToken);

                    int codigoPedidoAnterior = 0;
                    for (int i = 0; i < remover.Count; i++)
                    {
                        //Caso não permita quebrar em multiplos carregamentos, precisamos ver se todos os "produtos" do pedido estão vindo para serem cancelados.
                        if (!remover[i].CarregamentoPedido.Pedido.QuebraMultiplosCarregamentos)
                        {
                            if (codigoPedidoAnterior != remover[i].CarregamentoPedido.Pedido.Codigo)
                            {
                                codigoPedidoAnterior = remover[i].CarregamentoPedido.Pedido.Codigo;
                                //Vamos buscar todos os produtos do pedido para ver se estão sendo removidos...
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = repositorioPedidoProduto.BuscarPorPedido(codigoPedidoAnterior);
                                List<int> codigosPedidoProdutos = (from obj in pedidoProdutos select obj.Codigo).ToList();
                                List<int> codigosPedidoProdutosRemovendo = (from obj in remover where obj.CarregamentoPedido.Pedido.Codigo == codigoPedidoAnterior select obj.PedidoProduto.Codigo).ToList();
                                var z = codigosPedidoProdutos.Except(codigosPedidoProdutosRemovendo).ToList();
                                if (codigosPedidoProdutos.Count != codigosPedidoProdutosRemovendo.Count || z.Count > 0)
                                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.ConfiguracaoDoPedidoNaoPermiteQueEleSejaquebradoEmMaisDeUmCarreagmento);
                            }
                        }
                        await repositorioCarregamentoPedidoProduto.DeletarAsync(remover[i]);
                    }

                    for (int i = 0; i < codigosCarregamentoPedido.Count; i++)
                    {
                        // Localizando o registro do carregamento pedido...
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carPedido = await repostiorioCarregamentoPedido.BuscarPorCodigoAsync(codigosCarregamentoPedido[i], false);

                        // Localizando se restou produtos do pedido no carregamento para atualizar o saldo ou remover o pedido...
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidoCarregamento = repositorioCarregamentoPedidoProduto.BuscarPorCarregamentoPedido(codigosCarregamentoPedido[i]);
                        if (produtosPedidoCarregamento?.Count > 0)
                        {
                            carPedido.Peso = (from produto in produtosPedidoCarregamento
                                              select produto.Peso).Sum();

                            await repostiorioCarregamentoPedido.AtualizarAsync(carPedido);
                        }
                        else
                        {
                            pedidosRemovidosCarregamento.Add(carPedido.Pedido.Codigo);
                            await repostiorioCarregamentoPedido.DeletarAsync(carPedido);
                        }
                    }

                    unitOfWork.Flush();

                    IList<double> destinatariosFinal = repostiorioCarregamentoPedido.DestinatariosCarregamento(codigoCarregamento);
                    if (destinatariosExistentes.Count != destinatariosFinal.Count)
                    {
                        Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                        servicoMontagemCarga.RemoverDadosRelacionadosAosPedidos(carregamento, true);
                    }

                    await unitOfWork.CommitChangesAsync(cancellationToken);
                }

                return new JsonpResult(pedidosRemovidosCarregamento);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarMontagemPorPedido);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AlterarPedidoProduto(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarregamento = Request.GetIntParam("Carregamento");
                int codigoPedidoProduto = Request.GetIntParam("Codigo");
                int codigoPedidoProdutoCarregamento = Request.GetIntParam("CodigoPedidoProdutoCarregamento");
                List<int> codigosPedidoProdutoCarregamento = Request.GetListParam<int>("CodigosPedidoProdutoCarregamento");
                List<int> codigosPedidosProdutos = Request.GetListParam<int>("CodigosPedidosProdutos");
                decimal peso = Request.GetDecimalParam("Peso");
                decimal qtde = Request.GetDecimalParam("Qtde");
                decimal pallet = Request.GetDecimalParam("Pallet");
                decimal cubico = Request.GetDecimalParam("Cubico");
                int operacao = Request.GetIntParam("Operacao");
                bool basicDataTable = Request.GetBoolParam("BasicDataTable");
                // 1 = Qtde, 2 = Peso, 3 = Pallet
                int alterando = Request.GetIntParam("Alterando");

                int codigoCarregamentoAntigo = codigoCarregamento;

                // Pode vir o codigo do produto no carregamento, e uma lista de selecionados.
                // Quando vier um pelo menos selecionado, vamos desconsiderar o que está vindo sozinho, pois ao arrastar,, 
                // se ele marcar 2 itens na lista e arrastar um item não selecionados.. vamos considerar apenas os marcados
                // Se a lista vier vazia.. não tem nenhum rebisro selecionado, pode ser que ele esteja removendo o item no menu... ou arrastando um item qualquer..
                if (codigoPedidoProdutoCarregamento > 0 && !codigosPedidoProdutoCarregamento.Contains(codigoPedidoProdutoCarregamento)) //codigosPedidoProdutoCarregamento.Count == 0) 
                    codigosPedidoProdutoCarregamento.Add(codigoPedidoProdutoCarregamento);

                //Ajuste técnico, pois pode vir um pedidoproduto ou um pedidoprodutocarregamento...
                if (codigosPedidoProdutoCarregamento.Count == 0 && codigoPedidoProduto > 0)
                    codigosPedidoProdutoCarregamento.Add(0);

                if (codigosPedidosProdutos.Count == 0)
                    codigosPedidosProdutos.Add(codigoPedidoProduto);

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repCarregamento.BuscarPorCodigoAsync(codigoCarregamento, false);
                if (carregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNaoEncontrado);

                if (carregamento.SessaoRoteirizador != null)
                {
                    if (IsMontagemCarregamentoSessaoFinalizada(carregamento))
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNumeroEstaEmUmaSessaoDeRoteirizacaoNaoPodeSerAlterado, carregamento.NumeroCarregamento, carregamento.SessaoRoteirizador.DescricaoSituacao));

                    if (carregamento.SessaoRoteirizador.UsuarioAtual.Codigo != this.Usuario.Codigo)
                        return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAlterarProdutosDoCarregamentoPoissessaoDeRoteirizacaoEstaAbertaPeloUsuario, carregamento.SessaoRoteirizador.UsuarioAtual.Nome));
                }

                if (carregamento.SituacaoCarregamento != SituacaoCarregamento.EmMontagem)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAlterarPedidoProdutoDeUmCarregamentoComSituacao, carregamento.SituacaoCarregamento.ToString()));

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarrPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarrPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = ObterConfiguracaoMontagemCarga(unitOfWork);

                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamentoAntigo = null;

                IList<double> destinatariosExistentes = repCarrPedido.DestinatariosCarregamento(carregamento.Codigo);
                IList<double> destinatariosExistentesAntigo = new List<double>();

                List<int> pedidosRemovidosCarregamento = new List<int>();
                List<int> pedidosAdicionadosCarregamento = new List<int>();

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoEditado = null;


                for (int i = 0; i < codigosPedidoProdutoCarregamento.Count; i++)
                {
                    codigoPedidoProdutoCarregamento = codigosPedidoProdutoCarregamento[i];

                    for (int p = 0; p < codigosPedidosProdutos.Count; p++)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = null;
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto = null;

                        int codigoPedidoProdutoFor = codigosPedidosProdutos[p];

                        if (codigoPedidoProdutoCarregamento > 0)
                            carregamentoPedidoProduto = await repCarrPedidoProduto.BuscarPorCodigoAsync(codigoPedidoProdutoCarregamento, false);

                        if (carregamentoPedidoProduto != null)
                        {
                            pedidoProduto = carregamentoPedidoProduto.PedidoProduto;
                            codigoPedidoProdutoFor = pedidoProduto.Codigo;
                            if (operacao == 1) // Troca de produto entre carregamentos
                            {
                                peso = carregamentoPedidoProduto.Peso;
                                qtde = carregamentoPedidoProduto.Quantidade;
                                pallet = carregamentoPedidoProduto.QuantidadePallet;
                                cubico = carregamentoPedidoProduto.MetroCubico;
                                codigoCarregamentoAntigo = carregamentoPedidoProduto.CarregamentoPedido.Carregamento.Codigo;
                                carregamentoAntigo = carregamentoPedidoProduto.CarregamentoPedido.Carregamento;
                                destinatariosExistentesAntigo = repCarrPedido.DestinatariosCarregamento(codigoCarregamentoAntigo);
                            }
                        }
                        else
                        {
                            pedidoProduto = await repositorioPedidoProduto.BuscarPorCodigoAsync(codigoPedidoProdutoFor, false);

                            //Problema com casas decimais...pois no grid não atendidos está arredondaod o peso
                            if (operacao == 2 && peso > 0 && !basicDataTable) // Adiciona um produto a um carregamento..
                            {
                                if (Math.Abs(peso - pedidoProduto.PesoTotal) < (decimal)0.01)
                                {
                                    peso = pedidoProduto.PesoTotal;
                                    qtde = pedidoProduto.Quantidade;
                                    pallet = pedidoProduto.QuantidadePalet;
                                    cubico = pedidoProduto.MetroCubico;
                                }
                            }
                            else if (operacao == 2 && basicDataTable)
                            {
                                //Vamos obter o saldo do pedido produto para adicionar no carregamento.
                                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> saldoProdutos = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(pedidoProduto.Pedido.Codigo, unitOfWork);
                                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto saldoProduto = (from obj in saldoProdutos where obj.CodigoPedidoProduto == codigoPedidoProdutoFor select obj).FirstOrDefault();
                                if (saldoProduto == null)
                                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiPossivelValidarSaldoDoPedidoProduto);

                                peso = saldoProduto.SaldoPeso;
                                qtde = saldoProduto.SaldoQtde;
                                pallet = saldoProduto.SaldoPallet;
                                cubico = saldoProduto.SaldoMetro;
                            }
                        }

                        if (pedidoProduto == null)
                            return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiPossivelEncontrarRegistroDoPedidoProduto);
                        else if (!pedidoProduto.Pedido.QuebraMultiplosCarregamentos)
                        {
                            //Ver se o pedido não existe em outro carregamento... se não existir .. permite..
                            var carregamentosPedido = repCarrPedidoProduto.BuscarPorPedido(pedidoProduto.Pedido.Codigo);
                            if (carregamentosPedido.Exists(x => x.CarregamentoPedido.Carregamento.Codigo != codigoCarregamento) && operacao != 1 && pedidoProduto.FilialArmazem == null) // Troca de produto entre carregamentos
                                return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.ConfiguracaoDoPedidoNaoPermiteQueEleSejaquebradoEmMaisDeUmCarreagmento);

                            //Ver se não digitou uma quantidade diferente do que tem no produto..
                            if ((qtde != pedidoProduto.Quantidade || pallet != pedidoProduto.QuantidadePalet) && pedidoProduto.FilialArmazem == null)
                                return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.ConfiguracaoDoPedidoNaoPermiteQueEleSejaquebradoEmMaisDeUmCarreagmento);

                            // Validar se está arrastando 1 produto do pedido em um carregamento.. deve arrastar todos...
                            if (operacao == 2)
                            {
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedido = repositorioPedidoProduto.BuscarPorPedido(pedidoProduto.Pedido.Codigo);
                                List<int> codigosProdutosPedido = (from obj in produtosPedido select obj.Codigo).ToList();
                                var z = codigosProdutosPedido.Except(codigosPedidosProdutos).ToList();
                                if ((codigosProdutosPedido.Count != codigosPedidosProdutos.Count || z.Count > 0) && pedidoProduto.FilialArmazem == null)
                                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.ConfiguracaoDoPedidoNaoPermiteQueEleSejaquebradoEmMaisDeUmCarreagmento);
                            }
                        }

                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = repCarrPedido.BuscarCarregamentoPorPedidos(new List<int>() { pedidoProduto.Pedido.Codigo }, false, codigoCarregamento, false);

                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto = repCarrPedidoProduto.BuscarPorPedidoProduto(pedidoProduto.Codigo);

                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProdutoAntigo = null;
                        if (operacao == 1) // Troca de produtos entre carregamento
                        {
                            carregamentoPedidoProdutoAntigo = (from item in carregamentosPedidoProduto
                                                               where item.PedidoProduto.Codigo == codigoPedidoProdutoFor &&
                                                                     item.CarregamentoPedido.Carregamento.Codigo == codigoCarregamentoAntigo
                                                               select item).FirstOrDefault();

                            if (carregamentoPedidoProdutoAntigo != null)
                                if (carregamentoPedidoProdutoAntigo.CarregamentoPedido.Carregamento.SituacaoCarregamento != SituacaoCarregamento.EmMontagem)
                                    return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoRemoverPedidoProdutoDeUmCarregamentoComSituacao, carregamento.SituacaoCarregamento.ToString()));
                        }
                        else if (operacao == 2 && !pedidoProduto.Pedido.QuebraMultiplosCarregamentos)
                            if ((carregamentosPedidoProduto?.Count ?? 0) > 0 && codigoCarregamento != ((from o in carregamentosPedidoProduto select o.CarregamentoPedido.Carregamento.Codigo)?.FirstOrDefault() ?? 0))
                                return new JsonpResult(false, true, string.Format("Pedido não pode ser adicionado em outro carregamento. Pedido pertence ao carregamento {0}.", (from o in carregamentosPedidoProduto select o.CarregamentoPedido.Carregamento.NumeroCarregamento).FirstOrDefault()));

                        carregamentoPedidoProduto = (from item in carregamentosPedidoProduto
                                                     where item.PedidoProduto.Codigo == codigoPedidoProdutoFor &&
                                                           item.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento
                                                     select item).FirstOrDefault();

                        if (carregamentoPedidoProduto == null && peso == 0)
                            return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PesoDoProdutoDoPedidoInformadoParaCarregamentoDeveSerMaiorQueZero, pedidoProduto.Produto.Descricao, pedidoProduto.Pedido.NumeroPedidoEmbarcador));

                        decimal pesoOutrosCarregamentos = (from ic in carregamentosPedidoProduto
                                                           where ic.PedidoProduto.Codigo == codigoPedidoProdutoFor &&
                                                                 ic.CarregamentoPedido.Carregamento.Codigo != codigoCarregamentoAntigo
                                                           select ic.Peso).Sum();

                        decimal qtdeOutrosCarregamentos = (from ic in carregamentosPedidoProduto
                                                           where ic.PedidoProduto.Codigo == codigoPedidoProdutoFor &&
                                                                 ic.CarregamentoPedido.Carregamento.Codigo != codigoCarregamentoAntigo
                                                           select ic.Quantidade).Sum();

                        // 1 = Qtde, 2 = Peso, 3 = Pallet
                        if (pedidoProduto.Quantidade + (decimal)0.5 < qtdeOutrosCarregamentos + qtde && alterando == 1)
                            return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.QtdeDoProdutoDoPedidoNaoPodeSerSuperiorNesteCarregamento, pedidoProduto.Produto.Descricao, pedidoProduto.Pedido.NumeroPedidoEmbarcador, (pedidoProduto.Quantidade - qtdeOutrosCarregamentos).ToString("n3")));
                        if (pedidoProduto.PesoTotal + (decimal)0.5 < pesoOutrosCarregamentos + peso && alterando != 1)
                            return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PesoDoProdutoDoPedidoNaoPodeSerSuperiorNesteCarregamento, pedidoProduto.Produto.Descricao, pedidoProduto.Pedido.NumeroPedidoEmbarcador, (pedidoProduto.PesoTotal - pesoOutrosCarregamentos).ToString("n3")));

                        await unitOfWork.StartAsync(cancellationToken);

                        decimal pesoAtualizarCarregamentoPedido = 0;
                        decimal palletAtualizarCarregamentoPedido = 0;

                        //Verificando se o pedido já consta no carregamento
                        if (carregamentoPedido == null)
                        {
                            carregamentoPedido = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido()
                            {
                                Carregamento = carregamento,
                                Pedido = pedidoProduto.Pedido,
                                NumeroReboque = NumeroReboque.SemReboque,
                                Peso = 0,
                                Pallet = 0,
                                TipoCarregamentoPedido = TipoCarregamentoPedido.Normal
                            };
                            await repCarrPedido.InserirAsync(carregamentoPedido);

                            if (!pedidosAdicionadosCarregamento.Contains(pedidoProduto.Pedido.Codigo))
                                pedidosAdicionadosCarregamento.Add(pedidoProduto.Pedido.Codigo);
                        }

                        if (operacao == 1 && carregamentoPedidoProdutoAntigo != null) // Troca entre carregamentos
                        {
                            decimal pesoRemovido = carregamentoPedidoProdutoAntigo.Peso;
                            await repCarrPedidoProduto.DeletarAsync(carregamentoPedidoProdutoAntigo);
                            var carrPedido = await repCarrPedido.BuscarPorCodigoAsync(carregamentoPedidoProdutoAntigo.CarregamentoPedido.Codigo, false);
                            carrPedido.Peso -= pesoRemovido;
                            if (carrPedido.Peso > 0)
                                await repCarrPedido.AtualizarAsync(carrPedido);
                            else
                                await repCarrPedido.DeletarAsync(carrPedido);
                        }

                        //Verificando se o produto do pedido já consta no carregamento..
                        if (carregamentoPedidoProduto == null)
                        {
                            carregamentoPedidoProduto = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto()
                            {
                                CarregamentoPedido = carregamentoPedido,
                                PedidoProduto = pedidoProduto,
                                Peso = peso,
                                Quantidade = qtde,
                                QuantidadePallet = pallet,
                                MetroCubico = cubico,
                                QuantidadeOriginal = qtde,
                                QuantidadePalletOriginal = pallet,
                                MetroCubicoOriginal = cubico
                            };
                            if (carregamentoPedidoProduto.Quantidade > pedidoProduto.Quantidade)
                                throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.UmCarregamentoDoPedidoProdutoNaoPodeSerMaiorQueQuantidadeDoPedidoProduto, carregamentoPedidoProduto.Quantidade, pedidoProduto.Quantidade));

                            await repCarrPedidoProduto.InserirAsync(carregamentoPedidoProduto);
                            pesoAtualizarCarregamentoPedido += peso;
                            palletAtualizarCarregamentoPedido += pallet;
                        }
                        else
                        {
                            if (peso != carregamentoPedidoProduto.Peso && operacao != 2)
                                pesoAtualizarCarregamentoPedido = (peso - carregamentoPedidoProduto.Peso);
                            else if (operacao == 2)
                                pesoAtualizarCarregamentoPedido += peso;

                            if (pallet != carregamentoPedidoProduto.QuantidadePallet && operacao != 2)
                                palletAtualizarCarregamentoPedido = (pallet - carregamentoPedidoProduto.QuantidadePallet);
                            else if (operacao == 2)
                                palletAtualizarCarregamentoPedido += pallet;

                            if (peso == 0 && pallet == 0 && qtde == 0) // Remover o pedido/produto do carregamento
                            {
                                await repCarrPedidoProduto.DeletarAsync(carregamentoPedidoProduto);

                                carregamentoPedidoProduto.PedidoProduto.Pedido.ItensAtualizados = true;
                                await repositorioPedido.AtualizarAsync(carregamentoPedidoProduto.PedidoProduto.Pedido);
                            }
                            else
                            {
                                carregamentoPedidoProduto.Peso = (operacao != 2 ? peso : carregamentoPedidoProduto.Peso + peso);
                                carregamentoPedidoProduto.Quantidade = (operacao != 2 ? qtde : carregamentoPedidoProduto.Quantidade + qtde);
                                carregamentoPedidoProduto.QuantidadePallet = (operacao != 2 ? pallet : carregamentoPedidoProduto.QuantidadePallet + pallet);
                                carregamentoPedidoProduto.MetroCubico = (operacao != 2 ? cubico : carregamentoPedidoProduto.MetroCubico + cubico);
                                if (carregamentoPedidoProduto.Quantidade > pedidoProduto.Quantidade)
                                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.DoisCarregamentoDoPedidoProdutoNaoPodeSerMaiorQueQuantidadeDoPedidoProduto, carregamentoPedidoProduto.Quantidade, pedidoProduto.Quantidade));

                                await repCarrPedidoProduto.AtualizarAsync(carregamentoPedidoProduto);

                                // Se a configuração geral estiver habilitada, vamos atualizar as informações do produto e pedido.
                                if (configuracaoMontagemCarga.PermitirEditarPedidosAtravesTelaMontagemCargaMapa)
                                {
                                    carregamentoPedidoProduto.PedidoProduto.Quantidade = carregamentoPedidoProduto.Quantidade;
                                    carregamentoPedidoProduto.PedidoProduto.QuantidadePalet = carregamentoPedidoProduto.QuantidadePallet;
                                    carregamentoPedidoProduto.PedidoProduto.MetroCubico = carregamentoPedidoProduto.MetroCubico;
                                    await repositorioPedidoProduto.AtualizarAsync(carregamentoPedidoProduto.PedidoProduto);

                                    if (pesoAtualizarCarregamentoPedido != 0 || palletAtualizarCarregamentoPedido != 0)
                                    {
                                        carregamentoPedidoProduto.PedidoProduto.Pedido.PesoTotal += pesoAtualizarCarregamentoPedido;
                                        carregamentoPedidoProduto.PedidoProduto.Pedido.PesoSaldoRestante += pesoAtualizarCarregamentoPedido;
                                        if (carregamentoPedidoProduto.PedidoProduto.Pedido.NumeroPaletesFracionado != 0)
                                            carregamentoPedidoProduto.PedidoProduto.Pedido.NumeroPaletesFracionado += palletAtualizarCarregamentoPedido;
                                        else
                                            carregamentoPedidoProduto.PedidoProduto.Pedido.NumeroPaletes += (int)palletAtualizarCarregamentoPedido;
                                        carregamentoPedidoProduto.PedidoProduto.Pedido.PalletSaldoRestante += palletAtualizarCarregamentoPedido;

                                        await repositorioPedido.AtualizarAsync(carregamentoPedidoProduto.PedidoProduto.Pedido);

                                        pedidoEditado = carregamentoPedidoProduto.PedidoProduto.Pedido;
                                    }
                                }
                            }
                        }

                        if (pesoAtualizarCarregamentoPedido != 0 || palletAtualizarCarregamentoPedido != 0)
                        {
                            carregamentoPedido.Peso += pesoAtualizarCarregamentoPedido;
                            carregamentoPedido.Pallet += palletAtualizarCarregamentoPedido;

                            //Vamos obter o peso de acordo com os itens do pedido
                            if (carregamentoPedido.Peso > 0)
                                await repCarrPedido.AtualizarAsync(carregamentoPedido);
                            else
                            {
                                pedidosRemovidosCarregamento.Add(carregamentoPedido.Pedido.Codigo);
                                await repCarrPedido.DeletarAsync(carregamentoPedido);
                            }
                        }
                    }
                }

                unitOfWork.Flush();

                IList<double> destinatariosFinal = repCarrPedido.DestinatariosCarregamento(carregamento.Codigo);

                if (destinatariosExistentes.Count != destinatariosFinal.Count)
                    servicoMontagemCarga.RemoverDadosRelacionadosAosPedidos(carregamento, true);

                if (operacao == 1 && carregamentoAntigo != null)
                {
                    IList<double> destinatariosFinalAntigo = repCarrPedido.DestinatariosCarregamento(codigoCarregamentoAntigo);
                    if (destinatariosExistentesAntigo.Count != destinatariosFinalAntigo.Count)
                        servicoMontagemCarga.RemoverDadosRelacionadosAosPedidos(carregamentoAntigo, true);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                dynamic resultado = new
                {
                    adicionados = pedidosAdicionadosCarregamento,
                    removidos = pedidosRemovidosCarregamento,
                    CodigoPedido = pedidoEditado?.Codigo ?? 0,
                    TipoPaleteCliente = pedidoEditado?.TipoPaleteCliente ?? TipoPaleteCliente.NaoDefinido,
                    NumeroPaletesFracionado = (pedidoEditado?.NumeroPaletesFracionado ?? 0).ToString("n3"),
                    PesoTotalPaletes = (pedidoEditado?.PesoTotalPaletes ?? 0).ToString("n2"),
                    Peso = (pedidoEditado?.PesoTotal ?? 0).ToString("n4"),
                    PesoSaldoRestante = (pedidoEditado?.PesoSaldoRestante ?? 0).ToString("n2")
                };

                return new JsonpResult(resultado);
            }
            catch (ControllerException ce)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ce);
                return new JsonpResult(false, ce.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarMontagemPorPedido);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AutorizarVeiculo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/MontagemCarga");

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repCarregamento.BuscarPorCodigoAsync(codigo, true);

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.MontagemCarga_LiberacaoVeiculo))
                    return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.VoceNaoPossuiPermissaoParaLiberarVeiculo);

                await unitOfWork.StartAsync(cancellationToken);

                carregamento.VeiculoBloqueado = false;
                await repCarregamento.AtualizarAsync(carregamento, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalharAoGerarCarga);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> RelatorioTroca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);

                // Valida
                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                // Formata retorno
                string mensagem = string.Empty;
                byte[] arquivo = new Servicos.Embarcador.Carga.Carregamento().RelatorioTroca(carga, false, unitOfWork, ref mensagem);

                if (arquivo == null && !string.IsNullOrWhiteSpace(mensagem))
                    return new JsonpResult(false, mensagem);

                // Retorna informacoes
                return Arquivo(arquivo, "application/pdf", string.Concat(Localization.Resources.Cargas.MontagemCargaMapa.RelatorioDeTroca + " - ", carga.CodigoCargaEmbarcador, ".pdf"));
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

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.Locais repositorioLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                Repositorio.Embarcador.Logistica.PracaPedagio repositorioPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repCarregamento.BuscarPorCodigoAsync(codigo, false);

                //#33780
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao> roteirizacoes = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> clientesRotas = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>();
                List<Dominio.Entidades.Embarcador.Logistica.Locais> balancas = repositorioLocais.BuscarPorTipoDeLocal(TipoLocal.Balanca);
                List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio = await repositorioPracaPedagio.BuscarTodosAtivasAsync();
                if ((carregamento?.Codigo ?? 0) > 0)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork, cancellationToken);
                    roteirizacoes = await repCarregamentoRoteirizacao.BuscarPorCarregamentosAsync(new List<int> { carregamento.Codigo });
                    if (roteirizacoes?.Count > 0)
                    {
                        List<int> codigosRoteirizacoes = (from item in roteirizacoes
                                                          select item.Codigo).Distinct().ToList();

                        Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(unitOfWork, cancellationToken);
                        clientesRotas = await repCarregamentoRoteirizacaoClientesRota.BuscarPorCarregamentoRoteirizacoesAsync(codigosRoteirizacoes);
                    }
                }

                return new JsonpResult(await ObterMontagemCarga(carregamento, null, unitOfWork, cancellationToken, this.ConfiguracaoEmbarcador, true, null, null, carregamento?.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false, roteirizacoes, clientesRotas, balancas, null, null, null, null, pracasPedagio));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> SimularCalculoFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                dynamic dadosCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Carregamento"));
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repositorioCarregamento.BuscarPorCodigoAsync(((string)dadosCarregamento.Carregamento).ToInt(), false);

                if (carregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoObterOsDados);

                if (carregamento.SituacaoCarregamento == SituacaoCarregamento.AguardandoAprovacaoSolicitacao)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoEstaAguardandoAprovacaoParaGerarCargaNaoPodeSerAlterado);

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = await repositorioCarregamentoRoteirizacao.BuscarPorCarregamentoAsync(carregamento.Codigo);

                if (carregamentoRoteirizacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NecessarioRoteirizarCarregamentoAntesDeSimularFrete);

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = await repositorioPreCarga.BuscarPorCodigoAsync(((string)dadosCarregamento.PreCarga).ToInt(), false);
                dynamic dadosTransporte = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Transporte"));

                Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento cotacaoFreteCarregamento = new Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento()
                {
                    CarregamentoRedespacho = carregamento.CarregamentoRedespacho,
                    Distancia = (int)carregamentoRoteirizacao.DistanciaKM,
                    ModeloVeicularCarga = ((string)dadosCarregamento.ModeloVeicularCarga).ToInt(),
                    Pedidos = (from o in carregamento.Pedidos select o.Pedido.Codigo).ToList(),
                    TipoDeCarga = preCarga?.TipoDeCarga?.Codigo ?? ((string)dadosTransporte.TipoDeCarga).ToInt(),
                    TipoOperacao = ((string)dadosTransporte.TipoOperacao).ToInt(),
                    Transportador = ((string)dadosTransporte.Empresa).ToInt(),
                    Veiculo = ((string)dadosTransporte.Veiculo).ToInt(),
                    Filial = (carregamento?.SessaoRoteirizador?.Filial?.Codigo ?? carregamento?.Filial?.Codigo) ?? 0
                };

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacaoFrete = ObterSimulacaoFrete(carregamento.Codigo, cotacaoFreteCarregamento, unitOfWork, out string msgFrete, ConfiguracaoEmbarcador, out Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo);

                var retorno = new
                {
                    Simulacao = new
                    {
                        PesoFrete = simulacaoFrete.PesoFrete.ToString("n4"),
                        PesoLiquidoFrete = simulacaoFrete.PesoLiquidoFrete.ToString("n4"),
                        ValorMercadoria = simulacaoFrete.ValorMercadoria.ToString("n4"),
                        ValorFrete = simulacaoFrete.ValorFrete.ToString("n4"),
                        Distancia = simulacaoFrete.Distancia.ToString(),
                        ValorPorPeso = simulacaoFrete.ValorPorPeso.ToString("n4"),
                        PercentualSobValorMercadoria = simulacaoFrete.PercentualSobValorMercadoria.ToString("n4") + "%",
                        RetornoCalculo = string.IsNullOrWhiteSpace(msgFrete) ? Localization.Resources.Cargas.MontagemCargaMapa.FreteCalculadoComsucesso : msgFrete,
                        RetornoSucesso = string.IsNullOrWhiteSpace(msgFrete)
                    },
                    DetalhesFrete = (from obj in dadosCalculo.ComposicaoFrete
                                     select new
                                     {
                                         obj.Formula,
                                         DescricaoComponente = obj.Descricao,
                                         obj.ValoresFormula,
                                         obj.Valor,
                                         obj.ValorCalculado,
                                         CodigoTabela = dadosCalculo.TabelaFreteCliente?.CodigoIntegracao ?? string.Empty,
                                         Origem = dadosCalculo.TabelaFreteCliente?.DescricaoOrigem ?? string.Empty,
                                         Destino = dadosCalculo.TabelaFreteCliente?.DescricaoDestino ?? string.Empty,
                                     }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoSimularCalculoDoFrete);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> SalvarCarregamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                bool limparDadosRoteirizacao = false;
                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = SalvarDadosCarregamento(unitOfWork, ref limparDadosRoteirizacao);

                servicoMontagemCarga.RemoverDadosRelacionadosAosPedidos(carregamento, limparDadosRoteirizacao);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                string polilinhaRotaFrete = string.Empty;
                if (ConfiguracaoEmbarcador.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento && !string.IsNullOrWhiteSpace(carregamento?.Rota?.PolilinhaRota ?? string.Empty))
                    polilinhaRotaFrete = carregamento.Rota.PolilinhaRota;

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = await new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork).BuscarPorCarregamentoAsync(carregamento.Codigo);

                var retorno = new
                {
                    carregamento.Codigo,
                    carregamento.Descricao,
                    carregamento.VeiculoBloqueado,
                    PolilinhaRoteirizacao = (carregamentoRoteirizacao?.PolilinhaRota ?? null),
                    UtilizarPolilinhaRotaFrete = !string.IsNullOrWhiteSpace(polilinhaRotaFrete),
                    PolilinhaRotaFrete = polilinhaRotaFrete,
                    NaoExigeRoteirizacaoMontagemCarga = carregamento.TipoOperacao?.NaoExigeRoteirizacaoMontagemCarga ?? false
                };

                return new JsonpResult(retorno);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterDetalhesPedidosMontagemCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosPedidos = Request.GetListParam<int>("Pedidos");

                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = await repTipoOperacao.BuscarPorCodigoAsync(codigoTipoOperacao);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = await repPedido.BuscarPorCodigosParaMontagemCargaAsync(codigosPedidos);

                if (tipoOperacao == null)
                    tipoOperacao = pedidos.Where(o => o.TipoOperacao != null).Select(o => o.TipoOperacao).FirstOrDefault();

                string mensagem = null;

                if (tipoOperacao != null && tipoOperacao.UsarConfiguracaoEmissao)
                {
                    mensagem = tipoOperacao.ObservacaoEmissaoCarga;
                }
                else
                {
                    Dominio.Entidades.Cliente tomador = pedidos.FirstOrDefault()?.ObterTomador();

                    if (tomador != null)
                    {
                        if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                            mensagem = tomador.ObservacaoEmissaoCarga;
                        else if (tomador.GrupoPessoas != null)
                            mensagem = tomador.GrupoPessoas.ObservacaoEmissaoCarga;
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaPedido = pedidos.Where(o => o.TipoDeCarga != null).FirstOrDefault()?.TipoDeCarga;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoPedido = pedidos.Where(o => o.TipoOperacao != null).FirstOrDefault()?.TipoOperacao;
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga moedaloVeicularPedido = pedidos.Where(o => o.ModeloVeicularCarga != null).FirstOrDefault()?.ModeloVeicularCarga;
                Dominio.Entidades.Empresa empresa = pedidos.Where(o => o.Empresa != null).FirstOrDefault()?.Empresa;
                Dominio.Entidades.Veiculo veiculoPedido = pedidos.Where(o => o.VeiculoTracao != null).Select(o => o.VeiculoTracao).FirstOrDefault() ?? pedidos.Where(o => o.Veiculos.Count > 0).FirstOrDefault()?.Veiculos.FirstOrDefault();
                List<Dominio.Entidades.Usuario> motoristas = pedidos.Where(o => o.Motoristas.Count > 0).Select(o => o.Motoristas).FirstOrDefault()?.ToList() ?? new List<Dominio.Entidades.Usuario>();

                var retorno = new
                {
                    TipoCarga = new { Codigo = tipoCargaPedido?.Codigo ?? 0, Descricao = tipoCargaPedido?.Descricao ?? string.Empty },
                    TipoOperacao = new { Codigo = tipoOperacaoPedido?.Codigo ?? 0, Descricao = tipoOperacaoPedido?.Descricao ?? string.Empty },
                    ModeloVeicular = new { Codigo = moedaloVeicularPedido?.Codigo ?? 0, Descricao = moedaloVeicularPedido?.Descricao ?? string.Empty },
                    Veiculo = new { Codigo = veiculoPedido?.Codigo ?? 0, Descricao = veiculoPedido?.Descricao ?? string.Empty },
                    Empresa = new { Codigo = empresa?.Codigo ?? 0, Descricao = empresa?.Descricao ?? string.Empty },
                    RaizCNPJEmpresa = empresa?.RaizCnpj ?? string.Empty,
                    Motoristas = motoristas?.Select(o => new { Codigo = o.Codigo, Nome = o.Nome, CPF = o.CPF_Formatado }).ToList()
                };

                return new JsonpResult(new { Mensagem = mensagem, DadosTransporte = retorno });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoObterOsDetalhesParaMontagemDaCarga);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterDadosRotaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRota = Request.GetIntParam("rota");

                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

                Dominio.Entidades.RotaFrete rotaFrete = await repRotaFrete.BuscarPorCodigoAsync(codigoRota);

                if (rotaFrete == null)
                    return new JsonpResult(null);


                var pontos = new
                {
                    rotaFrete.PolilinhaRota,
                    PontosDaRota = Servicos.Embarcador.Carga.RotaFrete.ObterRotaFreteSerializada(rotaFrete, unitOfWork)
                };


                return new JsonpResult(pontos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoObterOsDetalhesParaMontagemDaCarga);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Importar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();
                if (files.Count <= 0)
                    return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.SelecioneUmArquivoParaEnvio);

                Servicos.DTO.CustomFile file = files[0];

                string extensao = Path.GetExtension(file.FileName).ToLowerInvariant();
                string nomeArquivo = file.FileName;

                if (extensao != ".xls" && extensao != ".xlsx" && extensao != ".xlsm")
                    return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.ExtensaoDoArquivoInvalidaSelecioneUmArquivoComExtensaoXlsOuXlsx);

                bool importarMesmoSemCTeAbsorvidoAnteriormente = Request.GetBoolParam("ImportarMesmoSemCTeAbsorvidoAnteriormente");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Servicos.Embarcador.Carga.MontagemFeeder.MontagemFeeder serMontagemFeeder = new Servicos.Embarcador.Carga.MontagemFeeder.MontagemFeeder();

                ExcelPackage package = new ExcelPackage(file.InputStream);
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                StringBuilder erros = new StringBuilder();
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

                bool planilhaFeeder = (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 2].Text)).ToLower() == "booking") &&
                     (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 3].Text)).ToLower() == "proposta  agreement") &&
                      (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 4].Text)).ToLower() == "manifesto  manifest") &&
                       (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 5].Text)).ToLower() == "protocolo antaq id") &&
                        (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 6].Text)).ToLower() == "numero ce  ce number");

                if (!planilhaFeeder)
                {
                    planilhaFeeder = (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 2].Text)).ToLower() == "booking") &&
                     (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 3].Text)).ToLower() == "proposta") &&
                      (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 4].Text)).ToLower() == "manifesto") &&
                       (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 5].Text)).ToLower() == "protocolo antaq");
                }

                bool planilhaFeederSubcontratacao = (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 2].Text)).ToLower() == "booking") &&
                     (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 3].Text)).ToLower() == "proposta") &&
                      (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 4].Text)).ToLower() == "modalidade") &&
                       (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 5].Text)).ToLower() == "chave cte") &&
                        (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 6].Text)).ToLower() == "chave nfe");

                bool planilhaFeederSubcontratacaoTipoUm = (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 1].Text)).ToLower() == "booking") &&
                    (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 2].Text)).ToLower() == "proposta") &&
                     (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 3].Text)).ToLower() == "modalidade") &&
                      (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 4].Text)).ToLower() == "chave cte") &&
                       (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 5].Text)).ToLower() == "chave nfe");

                string strEmbarque = Utilidades.String.RemoveDiacritics(worksheet.Cells[33, 5].Text).ToLower();
                if (planilhaFeeder)
                {
                    if (string.IsNullOrWhiteSpace(strEmbarque) || ((strEmbarque != "sim") && (strEmbarque != "nao")))
                        return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.PlanilhaImportadaNaoPossuiInformacaoSeEmbarqueUmAfretamento);
                }
                bool embarqueAfretamento = strEmbarque == "sim";

                Dominio.Entidades.Cliente destinatario = null;
                Dominio.Entidades.Cliente expedidor = null;
                Dominio.Entidades.Cliente tomador = null;

                string cnpjDestinatario = "", cnpjExpedidor = "", cnpjTomador = "";
                if (planilhaFeeder || planilhaFeederSubcontratacao)
                {
                    cnpjDestinatario = Utilidades.String.RemoveDiacritics(worksheet.Cells[24, 11].Text);
                    cnpjExpedidor = Utilidades.String.RemoveDiacritics(worksheet.Cells[24, 3].Text);
                    cnpjTomador = Utilidades.String.RemoveDiacritics(worksheet.Cells[30, 3].Text);
                }
                else if (planilhaFeederSubcontratacaoTipoUm)
                {
                    cnpjDestinatario = Utilidades.String.RemoveDiacritics(worksheet.Cells[24, 10].Text);
                    cnpjExpedidor = Utilidades.String.RemoveDiacritics(worksheet.Cells[24, 2].Text);
                    cnpjTomador = Utilidades.String.RemoveDiacritics(worksheet.Cells[30, 2].Text);
                }
                else
                    return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.PlanilhaImportadaNaoPossuiNenhumLayoutConfiguradoPorGentilezaVerifiqueAsColunasSuasPosicoes);

                double.TryParse(cnpjDestinatario, out double cnpj_cpf_destinatario);
                if (cnpj_cpf_destinatario > 0)
                    destinatario = await repCliente.BuscarPorCPFCNPJAsync(cnpj_cpf_destinatario);

                double.TryParse(cnpjExpedidor, out double cnpj_cpf_expedidor);
                if (cnpj_cpf_expedidor > 0)
                    expedidor = await repCliente.BuscarPorCPFCNPJAsync(cnpj_cpf_expedidor);

                double.TryParse(cnpjTomador, out double cnpj_cpf_tomador);
                if (cnpj_cpf_tomador > 0)
                    tomador = await repCliente.BuscarPorCPFCNPJAsync(cnpj_cpf_tomador);

                if (destinatario == null)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.DestinatarioDeCNPJNaoLocalizadoNoSistemaPrimeiroDeveSeCadastrarMesmo, cnpjDestinatario));
                if (planilhaFeeder)
                {
                    if (!serMontagemFeeder.ImportarPlanilhaCTNRFeeder(worksheet, unidadeDeTrabalho, ref erros, destinatario, cnpj_cpf_destinatario, embarqueAfretamento, expedidor, cnpj_cpf_expedidor, tomador, importarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, Auditado, Usuario, TipoServicoMultisoftware, 0, ClienteAcesso.URLAcesso, false))
                    {
                        package.Dispose();
                        return new JsonpResult(false, true, erros.ToString());
                    }
                    else
                    {
                        package.Dispose();
                        return new JsonpResult(true);
                    }
                }
                else if (planilhaFeederSubcontratacao)
                {
                    if (!serMontagemFeeder.ImportarPlanilhaCTNRFeederSubcontratacao(worksheet, unidadeDeTrabalho, ref erros, destinatario, cnpj_cpf_destinatario, embarqueAfretamento, expedidor, cnpj_cpf_expedidor, tomador, importarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, Auditado, Usuario, TipoServicoMultisoftware, 0, ClienteAcesso.URLAcesso, false))
                    {
                        package.Dispose();
                        return new JsonpResult(false, true, erros.ToString());
                    }
                    else
                    {
                        package.Dispose();
                        return new JsonpResult(true);
                    }
                }
                else if (planilhaFeederSubcontratacaoTipoUm)
                {
                    if (!serMontagemFeeder.ImportarPlanilhaCTNRFeederSubcontratacaoTipoUm(worksheet, unidadeDeTrabalho, ref erros, destinatario, cnpj_cpf_destinatario, embarqueAfretamento, expedidor, cnpj_cpf_expedidor, tomador, importarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, Auditado, Usuario, TipoServicoMultisoftware, 0, ClienteAcesso.URLAcesso, false))
                    {
                        package.Dispose();
                        return new JsonpResult(false, true, erros.ToString());
                    }
                    else
                    {
                        package.Dispose();
                        return new JsonpResult(true);
                    }
                }
                else
                    return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiEncontradaNenhumaConfiguracaoParaImportacaoDestaPlanilhaFavorVerifiqueLayout);
            }
            catch (Exception ex)
            {
                await unidadeDeTrabalho.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }

        /// <summary>
        /// Procedimento para gerar/retornar uma imagem/marker com um número
        /// Permite chamada anonima.
        /// </summary>
        /// <param name="number">Número a ser gerado no marker.</param>
        /// <param name="color">Cor a checar/gerar marcador.</param>
        /// <returns>Path com o caminho da imagem/marker gerado.</returns>
        [AllowAnonymous]
        public async Task<IActionResult> GetMarkerColorNumber(int number, string color = "_red")
        {
            //https://github.com/Concept211/Google-Maps-Markers/blob/master/source/ImageMagick.vbs
            if (!color.ToLower().Contains(".png"))
                color += ".png";
            string path = Servicos.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory);
            path = Utilidades.IO.FileStorageService.Storage.Combine(path, "img");
            path = Utilidades.IO.FileStorageService.Storage.Combine(path, "montagem-carga-mapa");
            path = Utilidades.IO.FileStorageService.Storage.Combine(path, "markers");
            path = Utilidades.IO.FileStorageService.Storage.Combine(path, "colors");
            path = Utilidades.IO.FileStorageService.Storage.Combine(path, "numbers");
            string file = Utilidades.IO.FileStorageService.Storage.Combine(path, number.ToString() + color);
            if (!Utilidades.IO.FileStorageService.Storage.Exists(file))
            {
                try
                {
                    //Vamos precisar gerar
                    string icon = path = Utilidades.IO.FileStorageService.Storage.Combine(path, color);
                    PointF point = new PointF(5f, 2f);
                    if (number > 99)
                        number = number % 99;
                    if (number >= 10)
                        point = new PointF(1f, 2f);

                    var cor = Brushes.Black;
                    if (color.ToLower().Contains("_red") ||
                        color.ToLower().Contains("_black") ||
                        color.ToLower().Contains("_blue") ||
                        color.ToLower().Contains("_green") ||
                        color.ToLower().Contains("_purple"))
                        cor = Brushes.White;

                    using (Bitmap bitmap = (Bitmap)System.Drawing.Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(icon)))
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            using (Font font = new Font("Arial Black", 9f))
                            {
                                graphics.DrawString(number.ToString(), font, cor, point);
                            }
                        }

                        Utilidades.IO.FileStorageService.Storage.SaveImage(file, bitmap);
                    }
                }
                catch (Exception ex) 
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao salvar imagem na montagem de carga: {ex.ToString()}", "CatchNoAction");
                }
            }
            file = "../" + file.Replace(Servicos.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory), "").Replace(@"\", "/");
            //return new Services.JsonHttpStatusResult(file, System.Net.HttpStatusCode.OK);
            return new JsonpResult(file);
        }

        public async Task<IActionResult> BuscarResumoAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repositorioCarregamento.BuscarPorCodigoAsync(codigoCarregamento, false);

                if (carregamento == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao = repositorioCarregamentoSolicitacao.BuscarUltimaPorCarregamento(carregamento.Codigo);

                if (carregamentoSolicitacao == null)
                    return new JsonpResult(new
                    {
                        carregamento.Codigo,
                        PossuiAlcada = false
                    });

                Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento repositorioAprovacao = new Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento(unitOfWork);
                int aprovacoes = await repositorioAprovacao.ContarAprovacoesAsync(carregamentoSolicitacao.Codigo);
                int aprovacoesNecessarias = await repositorioAprovacao.ContarAprovacoesNecessariasAsync(carregamentoSolicitacao.Codigo);
                int reprovacoes = await repositorioAprovacao.ContarReprovacoesAsync(carregamentoSolicitacao.Codigo);

                return new JsonpResult(new
                {
                    carregamento.Codigo,
                    AprovacoesNecessarias = aprovacoesNecessarias,
                    Aprovacoes = aprovacoes,
                    Reprovacoes = reprovacoes,
                    DescricaoSituacao = carregamentoSolicitacao.Situacao.ObterDescricao(),
                    PossuiAlcada = true,
                    PossuiRegras = carregamentoSolicitacao.Situacao != SituacaoCarregamentoSolicitacao.SemRegraAprovacao
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento repositorioAprovacao = new Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento autorizacao = await repositorioAprovacao.BuscarPorCodigoAsync(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    PodeAprovar = autorizacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Usuario, "Usuario", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Prioridade, "PrioridadeAprovacao", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 20, Models.Grid.Align.center, false);

                int codigoCarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repositorioCarregamento.BuscarPorCodigoAsync(codigoCarregamento, false);
                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento> listaAutorizacao;
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao = null;

                if (carregamento != null)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(unitOfWork);
                    carregamentoSolicitacao = repositorioCarregamentoSolicitacao.BuscarUltimaPorCarregamento(carregamento.Codigo);
                }

                if (carregamentoSolicitacao != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                    Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento repositorioAprovacao = new Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento(unitOfWork);
                    totalRegistros = repositorioAprovacao.ContarAutorizacoes(carregamentoSolicitacao.Codigo);
                    listaAutorizacao = totalRegistros > 0 ? repositorioAprovacao.ConsultarAutorizacoes(carregamentoSolicitacao.Codigo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento>();
                }
                else
                    listaAutorizacao = new List<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento>();

                var lista = (
                    from autorizacao in listaAutorizacao
                    select new
                    {
                        autorizacao.Codigo,
                        PrioridadeAprovacao = autorizacao.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                        Situacao = autorizacao.Situacao.ObterDescricao(),
                        Usuario = autorizacao.Usuario?.Nome,
                        Regra = autorizacao.Descricao,
                        Data = autorizacao.Data.HasValue ? autorizacao.Data.ToString() : string.Empty,
                        Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                        DT_RowColor = autorizacao.ObterCorGrid()
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ReprocessarRegras(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                int codigoCarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = await repositorioCarregamento.BuscarPorCodigoAsync(codigoCarregamento, false);

                if (carregamento == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao = repositorioCarregamentoSolicitacao.BuscarUltimaPorCarregamento(carregamento.Codigo);

                if (carregamentoSolicitacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (carregamentoSolicitacao.Situacao != SituacaoCarregamentoSolicitacao.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                await unitOfWork.StartAsync(cancellationToken);

                Servicos.Embarcador.Carga.MontagemCarga.CarregamentoAprovacao servicoCarregamentoAprovacao = new Servicos.Embarcador.Carga.MontagemCarga.CarregamentoAprovacao(unitOfWork);

                servicoCarregamentoAprovacao.CriarAprovacao(carregamentoSolicitacao, TipoServicoMultisoftware);
                await repositorioCarregamento.AtualizarAsync(carregamentoSolicitacao.Carregamento);
                await repositorioCarregamentoSolicitacao.AtualizarAsync(carregamentoSolicitacao);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(new { RegraReprocessada = carregamentoSolicitacao.Situacao != SituacaoCarregamentoSolicitacao.SemRegraAprovacao });
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoReprocessarCarregamento);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConsultaInconsistenciaGrupoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<dynamic> dynPedidos = Request.GetListParam<dynamic>("Pedidos");
                List<int> codigosPedidos = (from pedido in dynPedidos select ((string)pedido.Codigo).ToInt()).ToList();
                int codigoModeloVeicular = Request.GetIntParam("ModeloVeicularCarga");

                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);

                return new JsonpResult(new
                {
                    quantidadePedidosInconsistentes = servicoMontagemCarga.ObterPedidosInconsistenciaGrupoProduto(codigoModeloVeicular, codigosPedidos).Count,
                });
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterPedidoAtravesChaveNFe(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string chaveNFe = Request.GetStringParam("ChaveNotaFiscalEletronica");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                (int CodigoPedido, int CodigoFilial, string DescricaoFilial)? resultado = await repPedido.BuscarPorChaveNFeAsync(chaveNFe);

                if (resultado == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.ChaveNotaFiscalEletronicaSemPedido);

                return new JsonpResult(new
                {
                    resultado.Value.CodigoPedido,
                    resultado.Value.CodigoFilial,
                    resultado.Value.DescricaoFilial
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.ChaveNotaFiscalEletronicaInvalida);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracaoMontagemCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = ObterConfiguracaoMontagemCarga(unitOfWork);

                var retorno = new
                {
                    UtilizarFiliaisHabilitadasTransportarMontagemCargaMapa = configuracaoMontagemCarga?.UtilizarFiliaisHabilitadasTransportarMontagemCargaMapa ?? false
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                return new JsonpResult(false, "Erro ao obter configurações de montagem de carga");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarValorMinimoPorCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTipoOperacao = Request.GetIntParam("CodigoTipoOperacao");
                List<int> codigosPedido = Request.GetListParam<int>("CodigosPedidos");

                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = await repositorioTipoOperacao.BuscarPorCodigoAsync(codigoTipoOperacao);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = await repositorioPedido.BuscarPorCodigosAsync(codigosPedido);

                if (tipoOperacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (pedidos.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                await unitOfWork.StartAsync(cancellationToken);

                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = (from obj in pedidos select obj.Filial).Distinct().ToList();

                if (tipoOperacao.ConfiguracaoCarga?.ValidarValorMinimoCarga ?? false)
                {
                    decimal valorMinimoPorCarga = tipoOperacao.ConfiguracaoCarga?.ValorMinimoCarga ?? 0;
                    List<int> CodigosPedidosRemoverCarregamento = new List<int>();
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaValorMinimoPorCarga> cargasNaoAtingiramValorMinimo = ValidarValorMinimoPorCarga(pedidos, filiais, valorMinimoPorCarga, ref CodigosPedidosRemoverCarregamento);

                    if (cargasNaoAtingiramValorMinimo.Count > 0)
                    {
                        return new JsonpResult(new
                        {
                            CargasNaoAtingiramValorMinimo = cargasNaoAtingiramValorMinimo,
                            PedidosParaRemoverCarregamento = CodigosPedidosRemoverCarregamento
                        }, true, "Cargas não atingiram valor minimo");
                    }
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarMontagemPorPedido);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga ObterConfiguracaoMontagemCarga(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);
            return repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();
        }

        private string FormatarPercentual(decimal capacidade, decimal quantidade)
        {
            if (capacidade == 0)
                return "0 %";
            return ((int)Math.Round((100 / capacidade) * quantidade)).ToString() + " %";
        }

        private Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento SalvarDadosCarregamento(Repositorio.UnitOfWork unitOfWork, ref bool limparDadosRoteirizacao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Transportadores.GrupoTransportador repositorioGrupoTransportador = new Repositorio.Embarcador.Transportadores.GrupoTransportador(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repositorioConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = ObterConfiguracaoMontagemCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repositorioConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Seguro.Seguro servicoSeguro = new Servicos.Embarcador.Seguro.Seguro(unitOfWork);
            Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork, configuracaoEmbarcador);

            dynamic dynCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Carregamento"));

            dynamic dynTransporte = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Transporte"));

            int codigo = ((string)dynCarregamento.Carregamento?.ToString() ?? "").ToInt();
            int pedidoViagemNavio = ((string)dynCarregamento.PedidoViagemNavio?.ToString() ?? "").ToInt();
            int modeloVeicular = ((string)dynCarregamento.ModeloVeicularCarga?.ToString() ?? "").ToInt();
            int tipoSeparacao = ((string)dynCarregamento.TipoSeparacao?.ToString() ?? "").ToInt();
            int codigoPreCarga = ((string)dynCarregamento.PreCarga?.ToString() ?? "").ToInt();
            DateTime? dataPrevisaoSaida = ((string)dynCarregamento.DataPrevisaoSaida?.ToString() ?? "").ToNullableDateTime();
            DateTime? dataPrevisaoRetorno = ((string)dynCarregamento.DataPrevisaoRetorno?.ToString() ?? "").ToNullableDateTime();
            double recebedor = dynCarregamento.Recebedor != null ? (double)dynCarregamento.Recebedor : 0;
            DateTime? dataInicioViagemPrevista = ((string)dynCarregamento.DataInicioViagemPrevista?.ToString() ?? "").ToNullableDateTime();
            bool inserir = false;

            if (recebedor <= 0)
                recebedor = dynTransporte.Recebedor != null ? (double)dynTransporte.Recebedor : 0;

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = null;

            if (codigo > 0)
            {
                carregamento = repCarregamento.BuscarPorCodigo(codigo, true) ?? throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoInformadoNaoFoiLocalizado);

                if (carregamento.SituacaoCarregamento == SituacaoCarregamento.AguardandoAprovacaoSolicitacao)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNumeroPrecisaSerAprovadoParaGerarCarga, carregamento.NumeroCarregamento));

                if (carregamento.SituacaoCarregamento == SituacaoCarregamento.Fechado)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNumeroEstaFechadoNaoPodeSerAlterado, carregamento.NumeroCarregamento));

                if (carregamento.SituacaoCarregamento == SituacaoCarregamento.GerandoCargaBackground)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNumeroEstaEmProcessoDeGeracaoDeCargaNaoPodeSerAlterado, carregamento.NumeroCarregamento));

                if (IsMontagemCarregamentoSessaoFinalizada(carregamento))
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNumeroEstaEmUmaSessaoDeRoteirizacaoNaoPodeSerAlterado, carregamento.NumeroCarregamento, carregamento.SessaoRoteirizador.DescricaoSituacao));

                Dominio.Entidades.Embarcador.Cargas.Carga cargaCarregamento = repositorioCarga.BuscarPorCarregamento(codigo);
                if (cargaCarregamento != null)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoNumeroEstaFechadoNaoPodeSerAlterado, carregamento.NumeroCarregamento));
            }
            else
            {
                inserir = true;
                limparDadosRoteirizacao = true;
                carregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento();

                int.TryParse((string)dynCarregamento.TipoMontagemCarga, out int intTipoMontagemCarga);
                carregamento.TipoMontagemCarga = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga)intTipoMontagemCarga;
                carregamento.DataCriacao = DateTime.Now;

                if (carregamento.TipoMontagemCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga.NovaCarga)
                {
                    carregamento.AutoSequenciaNumero = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).ObterProximoCodigoCarregamento();
                    carregamento.NumeroCarregamento = carregamento.AutoSequenciaNumero.ToString();
                }

                carregamento.SituacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem;

                int cod_sessao_roteirizador = Request.GetIntParam("SessaoRoteirizador");
                if (cod_sessao_roteirizador > 0)
                    carregamento.SessaoRoteirizador = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork).BuscarPorCodigo(cod_sessao_roteirizador, true);
            }

            if (carregamento.SessaoRoteirizador != null)
            {
                if (carregamento.SessaoRoteirizador.SituacaoSessaoRoteirizador != SituacaoSessaoRoteirizador.Iniciada)
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoSalvarCarregamentoParaUmaSessaoDeRoteirizacaoComSituacaoDiferenteDeIniciada);

                if (carregamento.SessaoRoteirizador.UsuarioAtual.Codigo != this.Usuario.Codigo)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoSalvarCarregamentoPoisSessaoDeRoteirizacaoEstaAbertaPeloUsuario, carregamento.SessaoRoteirizador.UsuarioAtual.Nome));
            }

            if (!dataPrevisaoRetorno.IsNullOrMinValue())
                carregamento.DataPrevisaoRetorno = dataPrevisaoRetorno;

            if (!dataPrevisaoSaida.IsNullOrMinValue())
                carregamento.DataPrevisaoSaida = dataPrevisaoSaida;

            if (!dataInicioViagemPrevista.IsNullOrMinValue())
                carregamento.DataInicioViagemPrevista = dataInicioViagemPrevista;

            carregamento.Observacao = (string)dynCarregamento.Observacao;

            if ((carregamento.Recebedor?.CPF_CNPJ ?? 0) != recebedor)
                limparDadosRoteirizacao = true;

            carregamento.Recebedor = recebedor > 0 ? repCliente.BuscarPorCPFCNPJ(recebedor) : null;
            carregamento.TipoCondicaoPagamento = ((string)dynCarregamento.TipoCondicaoPagamento).ToNullableEnum<Dominio.Enumeradores.TipoCondicaoPagamento>();
            carregamento.TipoSeparacao = (tipoSeparacao > 0) ? new Repositorio.Embarcador.Cargas.TipoSeparacao(unitOfWork).BuscarPorCodigo(tipoSeparacao, false) : null;

            if (!carregamento.TipoCondicaoPagamento.HasValue && ConfiguracaoEmbarcador.InformarTipoCondicaoPagamentoMontagemCarga)
                throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.TipoDeFreteObrigatorio);

            carregamento.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(modeloVeicular);
            carregamento.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(pedidoViagemNavio);

            if ((codigoPreCarga > 0) && (carregamento.TipoMontagemCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga.NovaCarga))
                carregamento.PreCarga = repositorioPreCarga.BuscarPorCodigo(codigoPreCarga);
            else
                carregamento.PreCarga = null;

            // VALIDAR AQUI A PRE CARGA
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamentoValidacao = repCarregamento.BuscarPorPreCarga(carregamento.PreCarga?.Codigo ?? 0, carregamento.Codigo);
            if (carregamentoValidacao != null)
                throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PreCargaSelecionadoJaEstaSelecionadaNoCarregamento, carregamentoValidacao.Descricao));

            int codigoEmpresa = ((string)dynTransporte.Empresa?.ToString() ?? "").ToInt();
            int codigoGrupoTransportador = ((string)dynTransporte.GrupoTransportador?.ToString() ?? "").ToInt();
            int codigoVeiculo = ((string)dynTransporte.Veiculo?.ToString() ?? "").ToInt();
            int tipoCarga = ((string)dynTransporte.TipoDeCarga?.ToString() ?? "").ToInt();
            int tipoOperacao = ((string)dynTransporte.TipoOperacao?.ToString() ?? "").ToInt();
            double expedidor = dynTransporte.Expedidor != null ? (double)dynTransporte.Expedidor : 0;

            bool camposAlterados = false;

            if (configuracaoMontagemCarga?.NaoRetornarIntegracaoCarregamentoSeSomenteDadosTransporteForemAlterados ?? false && carregamento.Codigo > 0)
            {
                if (carregamento?.CarregamentoIntegradoERP ?? false)
                {
                    bool modeloVeicularAlterado = modeloVeicular > 0 ? ((carregamento?.ModeloVeicularCarga?.Codigo ?? 0) != (repModeloVeicularCarga.BuscarPorCodigo(modeloVeicular)?.Codigo ?? 0)) : false;
                    bool empresaAlterado = codigoEmpresa > 0 ? ((carregamento?.Empresa?.Codigo ?? 0) != (repEmpresa.BuscarPorCodigo(codigoEmpresa)?.Codigo ?? 0)) : false;
                    bool tipoDeCargaAlterado = tipoCarga > 0 ? ((carregamento?.TipoDeCarga?.Codigo ?? 0) != (repTipoDeCarga.BuscarPorCodigo(tipoCarga)?.Codigo ?? 0)) : false;
                    bool tipoOperacaoAlterado = tipoOperacao > 0 ? ((carregamento?.TipoOperacao?.Codigo ?? 0) != (repTipoOperacao.BuscarPorCodigo(tipoOperacao)?.Codigo ?? 0)) : false;
                    bool veiculoAlterado = codigoVeiculo > 0 ? ((carregamento?.Veiculo?.Codigo ?? 0) != (repVeiculo.BuscarPorCodigo(codigoVeiculo)?.Codigo ?? 0)) : false;

                    camposAlterados = (modeloVeicularAlterado || empresaAlterado || tipoDeCargaAlterado || tipoOperacaoAlterado || veiculoAlterado);
                }
                else
                    carregamento.CarregamentoIntegradoERP = false;
            }
            else
                carregamento.CarregamentoIntegradoERP = false;

            carregamento.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            carregamento.GrupoTransportador = codigoGrupoTransportador > 0 ? repositorioGrupoTransportador.BuscarPorCodigo(codigoGrupoTransportador) : null;
            carregamento.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;

            if ((carregamento.Expedidor?.CPF_CNPJ ?? 0) != expedidor)
                limparDadosRoteirizacao = true;

            carregamento.Expedidor = expedidor > 0 ? repCliente.BuscarPorCPFCNPJ(expedidor) : null;

            int codigoRotaFrte = ((string)dynTransporte.RotaFrete?.ToString() ?? "").ToInt();
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            carregamento.Rota = codigoRotaFrte > 0 ? repRotaFrete.BuscarPorCodigo(codigoRotaFrte) : null;

            bool redespacho = false;
            bool.TryParse(Request.Params("CarregamentoRedespacho"), out redespacho);

            carregamento.CarregamentoRedespacho = redespacho;
            carregamento.TipoDeCarga = tipoCarga > 0 ? repTipoDeCarga.BuscarPorCodigo(tipoCarga) : null;

            if (carregamento.TipoDeCarga == null && ConfiguracaoEmbarcador.TipoCargaObrigatorioMontagemCarga)
                throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.TipoDeCargaObrigatorio);

            carregamento.TipoOperacao = tipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(tipoOperacao) : null;

            if (carregamento.TipoOperacao == null && ConfiguracaoEmbarcador.TipoOperacaoObrigatorioMontagemCarga)
                throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.TipoDeOperacaoObrigatorio);

            if ((carregamento.TipoOperacao?.ExigirVeiculoComRastreador ?? false) && (carregamento.Veiculo != null && (carregamento.Veiculo?.TipoVeiculo ?? "1") == "0" && !(carregamento.Veiculo?.PossuiRastreador ?? false)))
                throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ParaTipoDeOperacaoObrigatorioInformarUmVeiculoComRastreadorCadastrado, carregamento.TipoOperacao.Descricao));

            if (configuracaoVeiculo?.NaoPermitirCadastrarVeiculoSemRastreador ?? false)
                servicoSeguro.ValidarRastreadorVeiculoEValorSeguroApolice(null, carregamento, unitOfWork);

            carregamento.CarregamentoColeta = carregamento.Recebedor != null && !(carregamento.TipoOperacao?.ConfiguracaoMontagemCarga?.MontagemComRecebedorNaoGerarCargaComoColeta ?? false);

            if (carregamento.SessaoRoteirizador != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentosSessaoColeta = repCarregamento.CarregamentosSessaoRoteirizador(carregamento.SessaoRoteirizador.Codigo, !carregamento.CarregamentoColeta);
                if (carregamentosSessaoColeta.Count > 0)
                {
                    if (carregamento.CarregamentoColeta)
                        throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoSalvarCarregamentoColetaPoisASessaoJaPossuiCarregamentosSemColeta);
                    else
                        throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoSalvarCarregamentoSemColetaPoisASessaoJaPossuiCarregamentosDeColeta);
                }
            }


            SalvarListaMotorista(ref carregamento, dynTransporte, unitOfWork, camposAlterados);
            SalvarListaAjudante(ref carregamento, dynTransporte, unitOfWork, camposAlterados);

            Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = null;

            if (inserir)
                repCarregamento.Inserir(carregamento, Auditado);
            else
                historicoObjeto = repCarregamento.Atualizar(carregamento, Auditado);

            DateTime? dataCarregamentoOriginal = carregamento.DataCarregamentoCarga;
            SalvarDadosCarregamentoFilial(carregamento, unitOfWork, camposAlterados);

            carregamento.CarregamentoIntegradoERP = camposAlterados;

            if (inserir)
                repCarregamento.Atualizar(carregamento);
            else
                historicoObjeto = repCarregamento.Atualizar(carregamento, Auditado);

            repCarregamento.Atualizar(carregamento);

            List<Dominio.Entidades.Cliente> fronteiras = SalvarFronteiras(carregamento, dynTransporte, unitOfWork);

            SalvarApolices(carregamento, dynTransporte, unitOfWork);

            if (carregamento.GrupoTransportador == null)
            {
                carregamento.ValorFreteManual = 0m;

                if ((carregamento.Empresa == null) && ConfiguracaoEmbarcador.TransportadorObrigatorioMontagemCarga)
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.TransportadorObrigatorio);

                if (ConfiguracaoEmbarcador.InformaApoliceSeguroMontagemCarga && !repositorioCarregamentoApolice.ExistePorCarregamento(carregamento.Codigo))
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ApoliceDeSeguroObrigatoria);
            }
            else
                carregamento.ValorFreteManual = ((string)dynCarregamento.ValorFreteManual).ToDecimal();

            decimal pesoCarregamento = 0;
            decimal palletCarregamento = 0;
            decimal palletInformadoCarregamento = Utilidades.Decimal.Converter((string)dynCarregamento.PalletCarregamento);

            if (carregamento.TipoMontagemCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga.NovaCarga)
                pesoCarregamento = SalvarPedidos(carregamento, fronteiras, historicoObjeto, configuracaoEmbarcador, configuracaoMontagemCarga, ref limparDadosRoteirizacao, ref palletCarregamento, palletInformadoCarregamento, unitOfWork);
            else
            {
                limparDadosRoteirizacao = true;
                SalvarCargas(ref carregamento, dynCarregamento.Cargas, historicoObjeto, unitOfWork);
            }

            //TODO: REFAZER
            decimal pesoNotas = SalvarNotasFiscais(carregamento, unitOfWork);

            if (pesoNotas > 0 && carregamento.ModeloVeicularCarga != null && pesoNotas > carregamento.ModeloVeicularCarga.CapacidadePesoTransporte)
                throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PesoDasNotasMaiorQueCapacidadeDoVeiculo, pesoNotas.ToString("n2"), carregamento.ModeloVeicularCarga.CapacidadePesoTransporte.ToString("n2")));

            if (pesoCarregamento == 0)
                pesoCarregamento = Utilidades.Decimal.Converter((string)dynCarregamento.PesoCarregamento);

            carregamento.PesoCarregamento = pesoCarregamento;
            carregamento.PalletCarregamento = palletCarregamento;

            servicoMontagemCarga.ValidarCapacidadeModeloVeicularCarga(carregamento);

            var veiculoPossuiLicenca = false;
            carregamento.VeiculoBloqueado = false;

            //TODO: REFAZER
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (carregamento.Veiculo != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);
                    for (int i = 0; i < pedidos.Count; i++)
                    {
                        for (int j = 0; j < pedidos[i].Pedido.Produtos.Count; j++)
                        {
                            for (int k = 0; k < pedidos[i].Pedido.Produtos[j].PedidoProdutoONUs.Count; k++)
                            {
                                if (carregamento.Veiculo.LicencasVeiculo == null || carregamento.Veiculo.LicencasVeiculo.Count == 0)
                                {
                                    carregamento.VeiculoBloqueado = true;
                                    break;
                                }
                                veiculoPossuiLicenca = false;
                                for (int l = 0; l < carregamento.Veiculo.LicencasVeiculo.Count; l++)
                                {
                                    if (carregamento.Veiculo.LicencasVeiculo[l].ClassificacaoRiscoONU != null)
                                    {
                                        if (!veiculoPossuiLicenca)
                                        {
                                            if (pedidos[i].Pedido.Produtos[j].PedidoProdutoONUs[k].ClassificacaoRiscoONU.Codigo == carregamento.Veiculo.LicencasVeiculo[l].ClassificacaoRiscoONU.Codigo)
                                            {
                                                veiculoPossuiLicenca = true;
                                                if (carregamento.Veiculo.LicencasVeiculo[l].DataVencimento < DateTime.Now.Date)
                                                {
                                                    carregamento.VeiculoBloqueado = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        carregamento.VeiculoBloqueado = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            bool ocupacaoExcedenteBalanca = Request.GetBoolParam("OcupacaoExcedenteBalanca");
            if (ocupacaoExcedenteBalanca)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carregamento, null, Localization.Resources.Cargas.MontagemCargaMapa.UsuarioAceitouGerarCargaComPercursoDeRotaComBalancaOcupacaoDoVeiculoExcedendoSuaCapacidade, unitOfWork);
            }

            return carregamento;
        }

        private void SalvarDadosCarregamentoFilial(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork, bool camposAlterados)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial repositorioCarregamentoFilial = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial(unitOfWork);
            dynamic dadosPorFilialPadrao = string.IsNullOrWhiteSpace(Request.Params("DadosPorFilialPadrao")) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("DadosPorFilialPadrao"));
            List<dynamic> dadosPorFiliais = Request.GetListParam<dynamic>("DadosPorFiliais");
            int anoAnterior = DateTime.Today.AddYears(-1).Year;
            int anoSubsequente = DateTime.Today.AddYears(+1).Year;

            repositorioCarregamentoFilial.DeletarPorCarregamento(carregamento.Codigo);

            if (dadosPorFiliais.Count > 0)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial> listaCarregamentoFilial = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial>();

                foreach (var dadosPorFilial in dadosPorFiliais)
                {
                    int codigoEmpresa = ((string)dadosPorFilial.Empresa).ToInt();
                    int codigoFilial = ((string)dadosPorFilial.Filial).ToInt();

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial carregamentoFilial = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial()
                    {
                        Carregamento = carregamento,
                        DataCarregamentoCarga = ((string)dadosPorFilial.DataCarregamento).ToNullableDateTime(),
                        DataDescarregamentoCarga = ((string)dadosPorFilial.DataDescarregamento).ToNullableDateTime(),
                        EncaixarHorario = ((string)dadosPorFilial.EncaixarHorario).ToBool(),
                        Filial = null
                    };

                    if (codigoFilial > 0)
                        carregamentoFilial.Filial = repositorioFilial.BuscarPorCodigo(codigoFilial);

                    if (codigoEmpresa > 0)
                        carregamentoFilial.Empresa = repositorioEmpresa.BuscarPorCodigo(codigoEmpresa);

                    if (carregamentoFilial.Filial == null)
                        throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiPossivelEncontrarFilial);

                    if (!carregamentoFilial.DataDescarregamentoCarga.HasValue && ConfiguracaoEmbarcador.InformaApoliceSeguroMontagemCarga)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.DataDeDescarregamentoDaFilialDeveSerInformada, carregamentoFilial.Filial.Descricao));

                    if (carregamentoFilial.DataCarregamentoCarga.HasValue && (carregamentoFilial.DataCarregamentoCarga.Value.Year <= anoAnterior || carregamentoFilial.DataCarregamentoCarga.Value.Year > anoSubsequente))
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.DataCarregamentoDaFilialNaoPodeSerMenorQueAnoAtualOuMaiorQueSubsequente, carregamentoFilial.Filial.Descricao));

                    if (carregamentoFilial.DataDescarregamentoCarga.HasValue && (carregamentoFilial.DataDescarregamentoCarga.Value.Year <= anoAnterior || carregamentoFilial.DataDescarregamentoCarga.Value.Year > anoSubsequente))
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.DataDescarregamentoDaFilialNaoPodeSerMenorQueAnoAtualOuMaiorQueSubsequente, carregamentoFilial.Filial.Descricao));

                    repositorioCarregamentoFilial.Inserir(carregamentoFilial);
                    listaCarregamentoFilial.Add(carregamentoFilial);
                }

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial carregamentoFilialMenorDataCarregamento = listaCarregamentoFilial.OrderBy(o => o.DataCarregamentoCarga).FirstOrDefault();

                carregamento.DataCarregamentoCarga = carregamentoFilialMenorDataCarregamento.DataCarregamentoCarga;
                carregamento.DataDescarregamentoCarga = carregamentoFilialMenorDataCarregamento.DataDescarregamentoCarga;
                carregamento.EncaixarHorario = carregamentoFilialMenorDataCarregamento.EncaixarHorario;
            }
            else
            {
                DateTime? dataCarregamento = ((string)dadosPorFilialPadrao.DataCarregamento).ToNullableDateTime();
                camposAlterados = dataCarregamento != carregamento.DataCarregamentoCarga && camposAlterados;
                carregamento.DataCarregamentoCarga = dataCarregamento;
                carregamento.DataDescarregamentoCarga = ((string)dadosPorFilialPadrao.DataDescarregamento).ToNullableDateTime();
                carregamento.EncaixarHorario = ((string)dadosPorFilialPadrao.EncaixarHorario).ToBool();
                carregamento.DataInicioViagemPrevista = ((string)dadosPorFilialPadrao.DataInicioViagemPrevista).ToNullableDateTime();

                if (!carregamento.DataDescarregamentoCarga.HasValue && ConfiguracaoEmbarcador.InformaApoliceSeguroMontagemCarga)
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.DataDoDescarregamentoObrigatorio);

                if (carregamento.DataCarregamentoCarga.HasValue && (carregamento.DataCarregamentoCarga.Value.Year <= anoAnterior || carregamento.DataCarregamentoCarga.Value.Year > anoSubsequente))
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.DataCarregamentoNaoPodeSerMenorQueAnoAtualOuMaiorQueSubsequente);

                if (carregamento.DataDescarregamentoCarga.HasValue && (carregamento.DataDescarregamentoCarga.Value.Year <= anoAnterior || carregamento.DataDescarregamentoCarga.Value.Year > anoSubsequente))
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.DataDescarregamentoNaoPodeSerMenorQueAnoAtualOuMaiorQueSubsequente);
            }
        }

        private List<Dominio.Entidades.Cliente> SalvarFronteiras(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, dynamic dynTransporte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira repCarregamentoFronteira = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira(unitOfWork);

            List<Dominio.Entidades.Cliente> fronteirasAdicionadas = new List<Dominio.Entidades.Cliente>();
            List<double> codigos = new List<double>();

            dynamic listaFronteira = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)dynTransporte.Fronteira);
            foreach (dynamic codigo in listaFronteira)
                codigos.Add((double)codigo);

            repCarregamentoFronteira.DeletarPorCarregamento(carregamento.Codigo);
            if (codigos.Count > 0)
            {
                List<Dominio.Entidades.Cliente> fronteiras = repCliente.BuscarPorVariosCPFCNPJ(codigos);
                foreach (Dominio.Entidades.Cliente fronteira in fronteiras)
                {
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira novaFronteiras = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira
                    {
                        Carregamento = carregamento,
                        Fronteira = fronteira
                    };

                    repCarregamentoFronteira.Inserir(novaFronteiras);
                    fronteirasAdicionadas.Add(fronteira);
                }
            }

            return fronteirasAdicionadas;
        }

        private static void SalvarApolices(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, dynamic dynTransporte, Repositorio.UnitOfWork unitOfWork)
        {
            if (dynTransporte.Apolice == null)
                return;

            Repositorio.Embarcador.Seguros.ApoliceSeguro repositorioApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice> carregamentosApolice = repositorioCarregamentoApolice.BuscarPorCarregamento(carregamento.Codigo);

            dynamic listaApolices = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)dynTransporte.Apolice);

            if (carregamentosApolice.Count > 0)
            {
                List<int> codigosApolice = new List<int>();

                foreach (dynamic item in listaApolices)
                    codigosApolice.Add((int)item);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice> carregamentosApoliceRemover = carregamentosApolice.Where(x => !codigosApolice.Contains(x.ApoliceSeguro.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice apoliceRemover in carregamentosApoliceRemover)
                    repositorioCarregamentoApolice.Deletar(apoliceRemover);
            }

            if (listaApolices.Count == 0)
                return;

            foreach (dynamic codigoApolice in listaApolices)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice carregamentoApolice = carregamentosApolice.Find(x => x.ApoliceSeguro.Codigo == (int)codigoApolice);

                if (carregamentoApolice != null)
                    continue;

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice = repositorioApoliceSeguro.BuscarPorCodigo((int)codigoApolice) ?? throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioInformarApoliceDeSeguroParaSeguir);

                carregamentoApolice = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice
                {
                    Carregamento = carregamento,
                    ApoliceSeguro = apolice
                };

                repositorioCarregamentoApolice.Inserir(carregamentoApolice);
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        private bool CancelarCarregamentos(List<int> listaCodigoCarregamento, ref bool valida, ref string msg_erro)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga serMontagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                unitOfWork.Start();
                bool retorno = serMontagem.CancelarCarregamentos(listaCodigoCarregamento, ref valida, ref msg_erro, this.Usuario, unitOfWork);

                if (retorno)
                {
                    unitOfWork.CommitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                msg_erro = Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoCancelarCarregamento;
                return false;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private object ParseInfoCarregamentoSimulacaoFrete(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete> simulacoes, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacao = simulacoes.Find(x => x.Carregamento.Codigo == carregamento.Codigo);
            if (simulacao != null)
            {
                return new
                {
                    Sucesso = simulacao.SucessoSimulacao,
                    simulacao.ValorFrete
                };
            }
            else
            {
                return new
                {
                    Sucesso = false,
                    ValorFrete = 0
                };
            }
        }

        private object ParseCanaisEntregaCarregamento(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCarregamentos, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            var pedidosCarrregamento = pedidosCarregamentos.FindAll(x => x.Carregamento.Codigo == carregamento.Codigo);
            var canaisEntrega = (from tipo in pedidosCarrregamento
                                 where tipo.Pedido.CanalEntrega != null
                                 select tipo.Pedido.CanalEntrega
                                 ).Distinct().OrderBy(x => x.Descricao).ToList();
            if (canaisEntrega != null)
                return from p in canaisEntrega
                       select new
                       {
                           p.Codigo,
                           p.Descricao
                       };
            else
                return null;
        }

        private object ParseInfoCarregamentoRoteirizacao(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao> roteirizacoes, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> clientesRotas, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos, List<Dominio.Entidades.Embarcador.Logistica.Locais> balancas, List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio)
        {
            List<Dominio.Entidades.Embarcador.Logistica.Locais> balancasRota = new List<Dominio.Entidades.Embarcador.Logistica.Locais>();
            List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagioRota = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao rot = roteirizacoes?.Find(x => x.Carregamento.Codigo == carregamento.Codigo) ?? null;


            if (rot != null)
            {
                string polilinha = rot.PolilinhaRota;
                decimal quilometros = rot.DistanciaKM;
                int tempoMinutos = rot.TempoDeViagemEmMinutos;

                if (ConfiguracaoEmbarcador.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento && !string.IsNullOrWhiteSpace(carregamento?.Rota?.PolilinhaRota ?? string.Empty))
                {
                    polilinha = carregamento.Rota.PolilinhaRota;
                    quilometros = carregamento.Rota.Quilometros;
                    tempoMinutos = carregamento.Rota.TempoDeViagemEmMinutos;
                }

                if (!string.IsNullOrEmpty(polilinha))
                {
                    foreach (Dominio.Entidades.Embarcador.Logistica.Locais balanca in balancas)
                    {
                        var areas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(balanca.Area);
                        for (int i = 0; i < areas.Count; i++)
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPolilinha = Servicos.Embarcador.Logistica.Polilinha.Decodificar(polilinha);

                            switch (areas[i].type)
                            {
                                case "circle":
                                    var pontoRaio = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = areas[i].center.lat, Longitude = areas[i].center.lng };
                                    coordenadasPolilinha = coordenadasPolilinha.FindAll(x => Servicos.Embarcador.Logistica.Distancia.ValidarNoRaio(x, pontoRaio, areas[i].radius / 1000)).ToList();
                                    if (coordenadasPolilinha?.Count > 0)
                                        balancasRota.Add(balanca);
                                    break;

                                case "rectangle":
                                    var listaPontosRetangulo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                                    listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = areas[i].bounds.north, Longitude = areas[i].bounds.east });
                                    listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = areas[i].bounds.north, Longitude = areas[i].bounds.west });
                                    listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = areas[i].bounds.south, Longitude = areas[i].bounds.west });
                                    listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = areas[i].bounds.south, Longitude = areas[i].bounds.east });
                                    coordenadasPolilinha = coordenadasPolilinha.FindAll(x => Servicos.Embarcador.Logistica.Distancia.ValidarPoligono(x, listaPontosRetangulo.ToArray())).ToList();
                                    if (coordenadasPolilinha?.Count > 0)
                                        balancasRota.Add(balanca);
                                    break;

                                case "polygon":
                                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> listaPontosPolygon = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                                    foreach (var ponto in areas[i].paths)
                                        listaPontosPolygon.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = ponto.lat, Longitude = ponto.lng });

                                    coordenadasPolilinha = coordenadasPolilinha.FindAll(x => Servicos.Embarcador.Logistica.Distancia.ValidarPoligono(x, listaPontosPolygon.ToArray())).ToList();
                                    if (coordenadasPolilinha?.Count > 0)
                                        balancasRota.Add(balanca);
                                    break;

                                case "marker":
                                    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoChecar = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint()
                                    {
                                        Latitude = areas[0].position.lat,
                                        Longitude = areas[0].position.lng
                                    };
                                    if (Servicos.Embarcador.Logistica.Polilinha.VerificarSePontoEstaProximoDaRota(polilinha, pontoChecar, 10))
                                        balancasRota.Add(balanca);
                                    break;
                            }
                        }
                    }

                    foreach (Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio in pracasPedagio)
                    {

                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPolilinha = Servicos.Embarcador.Logistica.Polilinha.Decodificar(polilinha);

                        Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoChecar = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint()
                        {
                            Latitude = pracaPedagio?.Latitude?.ToDouble() ?? 0,
                            Longitude = pracaPedagio?.Longitude?.ToDouble() ?? 0
                        };

                        if (Servicos.Embarcador.Logistica.Polilinha.VerificarSePontoEstaProximoDaRota(polilinha, pontoChecar, 10))
                            pracasPedagioRota.Add(pracaPedagio);
                        break;
                    }
                }

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> carregamentoRoteirizacaoClientesRota = clientesRotas.FindAll(c => c.CarregamentoRoteirizacao.Codigo == rot.Codigo && c.Cliente != null);
                if (carregamentoRoteirizacaoClientesRota?.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = (from o in carregamentosPedidos where o.Carregamento.Codigo == carregamento.Codigo && o.Pedido.Destinatario != null select o).ToList();
                    return new
                    {
                        Balancas = (from bal in balancasRota select new { bal.Codigo, bal.Descricao }).Distinct().ToList(),
                        PracasPedagio = (from p in pracasPedagioRota
                                         select new
                                         {
                                             p.Codigo,
                                             p.CodigoIntegracao,
                                             p.Descricao,
                                             Valor = (from v in p.PracaPedagioTarifa
                                                      where (v.ModeloVeicularCarga?.Codigo ?? 0) == (carregamento.ModeloVeicularCarga?.Codigo ?? 0) && v.Data <= DateTime.Now
                                                      orderby v.Data descending
                                                      select v.Tarifa)?.FirstOrDefault() ?? 0
                                         }).ToList(),
                        DistanciaKM = quilometros,
                        TempoDeViagemEmMinutos = tempoMinutos,
                        PolilinhaRota = polilinha,
                        Pedidos = (from c in carregamentoRoteirizacaoClientesRota
                                   join p in carregamentoPedidos on c.Cliente.CPF_CNPJ equals p.Pedido.Destinatario.CPF_CNPJ
                                   where c.Cliente != null
                                   orderby c.Ordem
                                   select p.Pedido.Codigo
                                   ).Distinct().ToArray()
                    };
                }
                else
                {
                    return new
                    {
                        Balancas = (from bal in balancasRota select new { bal.Codigo, bal.Descricao }).Distinct().ToList(),
                        PracasPedagio = (from p in pracasPedagioRota
                                         select new
                                         {
                                             p.Codigo,
                                             p.CodigoIntegracao,
                                             p.Descricao,
                                             Valor = (from v in p.PracaPedagioTarifa
                                                      where (v.ModeloVeicularCarga?.Codigo ?? 0) == (carregamento.ModeloVeicularCarga?.Codigo ?? 0) && v.Data <= DateTime.Now
                                                      orderby v.Data descending
                                                      select v.Tarifa)?.FirstOrDefault() ?? 0
                                         }).ToList(),
                        DistanciaKM = quilometros,
                        TempoDeViagemEmMinutos = tempoMinutos,
                        PolilinhaRota = polilinha,
                        Pedidos = (from o in carregamentosPedidos where o.Carregamento.Codigo == carregamento.Codigo select o.Pedido.Codigo).ToArray()
                    };
                }
            }
            else
                return new
                {
                    Balancas = (from bal in balancasRota
                                select new
                                {
                                    bal.Codigo,
                                    bal.Descricao
                                }).Distinct().ToList(),
                    PracasPedagio = (from p in pracasPedagioRota
                                     select new
                                     {
                                         p.Codigo,
                                         p.CodigoIntegracao,
                                         p.Descricao,
                                         Valor = (from v in p.PracaPedagioTarifa
                                                  where (v.ModeloVeicularCarga?.Codigo ?? 0) == (carregamento.ModeloVeicularCarga?.Codigo ?? 0) && v.Data <= DateTime.Now
                                                  orderby v.Data descending
                                                  select v.Tarifa)?.FirstOrDefault() ?? 0
                                     }).ToList(),
                    DistanciaKM = (!ConfiguracaoEmbarcador.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento ? 0 : carregamento?.Rota?.Quilometros ?? 0),
                    TempoDeViagemEmMinutos = (!ConfiguracaoEmbarcador.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento ? 0 : carregamento?.Rota?.TempoDeViagemEmMinutos ?? 0),
                    PolilinhaRota = (!ConfiguracaoEmbarcador.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento ? "" : carregamento?.Rota?.PolilinhaRota ?? ""),
                    Pedidos = (from o in carregamentosPedidos where o.Carregamento.Codigo == carregamento.Codigo select o.Pedido.Codigo).ToArray()
                };
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoPedido = Request.GetIntParam("Pedidos");
            List<int> codigosCanalEntregas = Request.GetListParam<int>("CanalEntrega");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosFilialVenda = Request.GetListParam<int>("FilialVenda");
            List<int> codigosPedido = Request.GetListParam<int>("Pedidos");
            List<int> codigosCanalEntrega = Request.GetListParam<int>("CodigosCanalEntrega");
            List<int> codigosTipoDeCarga = Request.GetListParam<int>("CodigosTipoDeCarga");

            if ((codigoFilial > 0) && (codigosFilial.Count == 0))
                codigosFilial.Add(codigoFilial);

            if ((codigoPedido > 0) && (codigosPedido.Count == 0))
                codigosPedido.Add(codigoPedido);

            if (codigosCanalEntregas.Count > 0 && codigosCanalEntrega.Count == 0)
            {
                foreach (int codigoCanal in codigosCanalEntregas)
                    codigosCanalEntrega.Add(codigoCanal);
            }

            string numeroCarga = Request.GetStringParam("NumeroCarga");

            if (string.IsNullOrWhiteSpace(numeroCarga))
            {
                numeroCarga = Request.GetStringParam("CodigoCargaEmbarcador");
            }

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento()
            {
                CarregamentosDeColeta = Request.GetBoolParam("GerarCargasDeColeta"),
                CodigoCargaEmbarcador = numeroCarga,
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoPaisDestino = Request.GetIntParam("PaisDestino"),
                CodigoPedidoViagemNavio = Request.GetIntParam("PedidoViagemNavio"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjExpedidor = Request.GetDoubleParam("Expedidor"),
                CpfCnpjRecebedor = Request.GetDoubleParam("Recebedor"),
                NotaFiscal = Request.GetListParam<int>("NotaFiscal"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                DataFim = Request.GetNullableDateTimeParam("DataFim"),
                DataInclusaoPCPInicial = Request.GetNullableDateTimeParam("DataInclusaoPCPInicial"),
                DataInclusaoPCPLimite = Request.GetNullableDateTimeParam("DataInclusaoPCPLimite"),
                DataInclusaoBookingInicial = Request.GetNullableDateTimeParam("DataInclusaoBookingInicial"),
                DataInclusaoBookingLimite = Request.GetNullableDateTimeParam("DataInclusaoBookingLimite"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DeliveryTerm = Request.GetStringParam("DeliveryTerm"),
                FiltrarPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                IdAutorizacao = Request.GetStringParam("IdAutorizacao"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroCarregamento = Request.GetStringParam("NumeroCarregamento"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroPedido = Request.GetIntParam("NumeroPedido"),
                NumeroPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador"),
                Ordem = Request.GetStringParam("Ordem"),
                PortoSaida = Request.GetStringParam("PortoSaida"),
                Reserva = Request.GetStringParam("Reserva"),
                SituacoesCarregamento = Request.GetListEnumParam<SituacaoCarregamento>("SituacaoCarregamento"),
                SomenteComReserva = Request.GetBoolParam("SomenteComReserva"),
                TipoEmbarque = Request.GetStringParam("TipoEmbarque"),
                TipoMontagemCarga = Request.GetEnumParam<TipoMontagemCarga>("TipoMontagemCarga"),
                ProgramaComSessaoRoteirizador = Request.GetBoolParam("ComSessaoRoteirizador"),
                OpcaoSessaoRoteirizador = Request.GetEnumParam("OpcaoSessaoRoteirizador", OpcaoSessaoRoteirizador.NENHUM),
                CodigoSessaoRoteirizador = Request.GetIntParam("SessaoRoteirizador"),
                SituacaoSessaoRoteirizador = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador>("SituacaoSessaoRoteirizador", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Todas),
                CodigoProdutoEmbarcador = Request.GetIntParam("Produto"),
                CodigosLinhaSeparacao = Request.GetListParam<int>("CodigosLinhaSeparacao"),
                CodigosProdutos = Request.GetListEnumParam<int>("CodigosProdutos"),
                CodigosGrupoProdutos = Request.GetListParam<int>("CodigosGrupoProdutos"),
                CodigosCategoriaClientes = Request.GetListParam<int>("CodigosCategoriaClientes"),
                CodigosCanalEntrega = codigosCanalEntrega,
                ExigeAgendamento = Request.GetBoolParam("ExigeAgendamento"),
                CodigosRegiaoDestino = Request.GetListParam<int>("RegiaoDestino")
            };

            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoEmpresa = Request.GetIntParam("Empresa");

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosFilialVenda = codigosFilialVenda.Count == 0 ? ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork) : codigosFilialVenda;
            filtrosPesquisa.CodigosPedido = codigosPedido;
            filtrosPesquisa.CodigosTipoCarga = codigoTipoCarga > 0 ? new List<int>() { codigoTipoCarga } : (codigosTipoDeCarga.Count == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : codigosTipoDeCarga);
            filtrosPesquisa.CodigosTipoOperacao = codigoTipoOperacao > 0 ? new List<int>() { codigoTipoOperacao } : ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosEmpresa = codigoEmpresa > 0 ? new List<int>() { codigoEmpresa } : ObterListaCodigoTransportadorPermitidosOperadorLogistica(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (Empresa.Matriz.Any())
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Matriz.FirstOrDefault().Codigo;
                else
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Codigo;
            }

            if (filtrosPesquisa.ProgramaComSessaoRoteirizador && filtrosPesquisa.CodigoSessaoRoteirizador > 0 && filtrosPesquisa.SituacaoSessaoRoteirizador == SituacaoSessaoRoteirizador.Finalizada)
                filtrosPesquisa.SituacoesCarregamento.Add(SituacaoCarregamento.Fechado);

            return filtrosPesquisa;
        }

        private async Task<dynamic> ObterMontagemCarga(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool load_motoristas = true, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCarregamentos = null, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidosCarregamentos = null, bool montagemCargaPorPedidoProduto = false, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao> roteirizacoes = null, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> clientesRotas = null, List<Dominio.Entidades.Embarcador.Logistica.Locais> balancas = null, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial> carregamentosDadosPorFiliais = null, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = null, List<string> filiaisAtivas = null, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira> carregamentosFronteiras = null, List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio = null)
        {
            if (carregamento == null) return null;

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial> dadosPorFiliais;
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidosCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> listaCarregamentoPedidoNotaFiscal = montagemCargaPorPedidoProduto ? new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>() : await new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork, cancellationToken).BuscarPorCarregamentoAsync(carregamento.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDoCarregamento = listaCarregamentoPedidoNotaFiscal.Count > 0 ? listaCarregamentoPedidoNotaFiscal.Select(x => x.NotasFiscais.Select(h => h).ToList()).FirstOrDefault() : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega> restricoesEntregas = new List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega>();

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice> carregamentosApolice = await repositorioCarregamentoApolice.BuscarPorCarregamentoAsync(carregamento.Codigo);

            //TelhaNorte..
            bool possuiProdutosPalletFechado = false;

            if (pedidosCarregamentos?.Count > 0)
                pedidosCarregamento = pedidosCarregamentos.FindAll(x => x.Carregamento.Codigo == carregamento.Codigo);
            else
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork, cancellationToken);
                pedidosCarregamento = await repCarregamentoPedido.BuscarPorCarregamentoAsync(carregamento.Codigo);
            }

            if (carregamentosDadosPorFiliais == null)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial repositorioCarregamentoFilial = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial(unitOfWork, cancellationToken);
                dadosPorFiliais = await repositorioCarregamentoFilial.BuscarPorCarregamentoAsync(carregamento.Codigo);
            }
            else
                dadosPorFiliais = carregamentosDadosPorFiliais.Where(o => o.Carregamento.Codigo == carregamento.Codigo).ToList();

            pedidosCarregamento = pedidosCarregamento.OrderBy(x => x.Ordem).ThenBy(x => x.Pedido.Codigo).ToList();

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = null;
            bool exibirHoraDataCarregamentoEDescarregamento = configuracao.InformaApoliceSeguroMontagemCarga || configuracao.InformaHorarioCarregamentoMontagemCarga;

            if (pedidosCarregamento?.Count > 0)
            {
                List<int> codigos = (from cp in pedidosCarregamento select cp.Pedido.Codigo).Distinct().ToList();

                if (produtosPedidosCarregamentos != null)
                {
                    produtosPedidosCarregamento = (from ppc in produtosPedidosCarregamentos
                                                   where codigos.Contains(ppc.CarregamentoPedido.Pedido.Codigo) && ppc.CarregamentoPedido.Carregamento.Codigo == carregamento.Codigo
                                                   select ppc).ToList();
                }
                else
                    produtosPedidosCarregamento = await new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork, cancellationToken).BuscarPorPedidosAsync(carregamento.Codigo, codigos);

                List<int> cfiliais = (from cp in pedidosCarregamento
                                      where cp.Pedido?.Filial != null
                                      select cp.Pedido.Filial.Codigo).Distinct().ToList();
                if (cfiliais?.Count > 0 && centrosCarregamento == null)
                {
                    Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                    centrosCarregamento = await repCentroCarregamento.BuscarPorFiliaisAsync(cfiliais);
                    montagemCargaPorPedidoProduto = centrosCarregamento.Exists(x => x.MontagemCarregamentoPedidoProduto);
                    centroCarregamento = centrosCarregamento.FirstOrDefault();
                }
                else if (cfiliais?.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamentoFiliais = (from obj in centrosCarregamento
                                                                                                                  where cfiliais.Contains(obj.Filial.Codigo)
                                                                                                                  select obj).ToList();
                    montagemCargaPorPedidoProduto = centrosCarregamentoFiliais.Exists(x => x.MontagemCarregamentoPedidoProduto);
                    centroCarregamento = centrosCarregamentoFiliais.FirstOrDefault();
                }

                possuiProdutosPalletFechado = produtosPedidosCarregamento.Any(x => x.PedidoProduto.PalletFechado);

                //Problema com carregamentos que não estão mais salvando o carregamentoPedidoProduto...
                if (produtosPedidosCarregamento.Count == 0 && pedidosProdutos?.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = (from obj in pedidosProdutos
                                                                                               where codigos.Contains(obj.Pedido.Codigo)
                                                                                               select obj).ToList();
                    possuiProdutosPalletFechado = pedidoProdutos.Any(x => x.PalletFechado);
                }

                //List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega> restricaoEntrega = clientesRota.Cliente?.ClienteDescargas?.FirstOrDefault()?.RestricoesDescarga.ToList();
                var destinatarios = (from obj in pedidosCarregamento where obj.Pedido.Destinatario != null select obj.Pedido.Destinatario).Distinct().ToList();
                foreach (var destinatario in destinatarios)
                {
                    foreach (var clienteDescarga in destinatario.ClienteDescargas)
                    {
                        foreach (var restricaoDescarga in clienteDescarga.RestricoesDescarga)
                        {
                            if (!restricoesEntregas.Any(x => x.Codigo == restricaoDescarga.Codigo))
                                restricoesEntregas.Add(restricaoDescarga);
                        }
                    }
                }
            }

            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork, cancellationToken);
            Dominio.Entidades.Usuario motorista = (!load_motoristas) ? null : carregamento.Motoristas?.FirstOrDefault();

            Dominio.Entidades.Usuario veiculoMotorista = null;
            if (carregamento.Veiculo != null)
                veiculoMotorista = await repVeiculoMotorista.BuscarMotoristaPrincipalAsync(carregamento.Veiculo.Codigo, cancellationToken);

            if (filiaisAtivas == null)
            {
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                filiaisAtivas = await repFilial.BuscarListaCNPJAtivasAsync();
            }

            List<Dominio.Entidades.Cliente> fronteirasCarregamento = null;
            if (carregamentosFronteiras != null)
                fronteirasCarregamento = (from obj in carregamentosFronteiras
                                          where obj.Carregamento.Codigo == carregamento.Codigo
                                          select obj.Fronteira).ToList();
            else
                fronteirasCarregamento = carregamento.Fronteiras.ToList();

            var retorno = new
            {
                Transporte = new
                {
                    Empresa = new { Codigo = carregamento.Empresa?.Codigo ?? 0, Descricao = carregamento.Empresa?.Descricao ?? "" },
                    RaizCNPJEmpresa = carregamento.Empresa?.RaizCnpj ?? "",
                    GrupoTransportador = new { Codigo = carregamento.GrupoTransportador?.Codigo ?? 0, Descricao = carregamento.GrupoTransportador?.Descricao ?? "" },
                    Veiculo = new { Codigo = carregamento.Veiculo?.Codigo ?? 0, Descricao = carregamento.Veiculo?.Descricao ?? "" },
                    TipoOperacao = new
                    {
                        Codigo = carregamento.TipoOperacao?.Codigo ?? 0,
                        Descricao = carregamento.TipoOperacao?.Descricao ?? "",
                        PermitirInformarRecebedorMontagemCarga = carregamento.TipoOperacao?.PermitirInformarRecebedorMontagemCarga ?? false,
                        ControlarCapacidadePorUnidade = carregamento.TipoOperacao?.ConfiguracaoMontagemCarga?.ControlarCapacidadePorUnidade ?? false,
                        PermitirInformarAjudantesNaCarga = carregamento.TipoOperacao?.ConfiguracaoCarga?.PermitirInformarAjudantesNaCarga ?? false,
                        ValidarValorMinimoCarga = carregamento.TipoOperacao?.ConfiguracaoCarga?.ValidarValorMinimoCarga ?? false,
                        NaoExigeRoteirizacaoMontagemCarga = carregamento.TipoOperacao?.NaoExigeRoteirizacaoMontagemCarga ?? false,
                    },
                    Recebedor = new { Codigo = carregamento.Recebedor?.Codigo ?? 0, Descricao = carregamento.Recebedor?.Descricao ?? "" },
                    Expedidor = new { Codigo = carregamento.Expedidor?.Codigo ?? 0, Descricao = carregamento.Expedidor?.Descricao ?? "" },
                    Fronteira = (
                        from o in fronteirasCarregamento
                        select new
                        {
                            o.Codigo,
                            o.Nome,
                            o.Descricao,
                            o.Latitude,
                            o.Longitude,
                        }
                    ).ToList(),
                    Apolice = (
                        from o in carregamentosApolice
                        select new
                        {
                            o.ApoliceSeguro.Codigo,
                            Descricao = o.ApoliceSeguro.DescricaoComSeguradora
                        }
                    ).ToList(),
                    RotaFrete = new { Codigo = carregamento.Rota?.Codigo ?? 0, Descricao = carregamento.Rota?.Descricao ?? "" },
                    TipoDeCarga = new
                    {
                        Codigo = carregamento.TipoDeCarga?.Codigo ?? 0,
                        Descricao = carregamento.TipoDeCarga?.Descricao ?? "",
                        Paletizado = carregamento.TipoDeCarga?.Paletizado ?? false
                    },
                    CodigoMotoristaVeiculo = veiculoMotorista != null ? veiculoMotorista.Codigo : 0,
                    NomeMotoristaVeiculo = veiculoMotorista != null ? veiculoMotorista.Nome : string.Empty,
                    Motorista = motorista != null ? new
                    {
                        motorista.Codigo,
                        Descricao = motorista.Descricao + " (" + motorista.CPF_Formatado + ")",
                    } : null,
                    ListaMotoristas = (!load_motoristas) ? null : carregamento.Motoristas != null ? (
                        from obj in carregamento.Motoristas
                        orderby obj.Nome
                        select new
                        {
                            obj.Codigo,
                            CPF = obj.CPF_Formatado,
                            obj.Nome
                        }
                    ).ToList() : null,
                    ListaAjudantes = carregamento.Ajudantes != null ? (
                        from obj in carregamento.Ajudantes
                        orderby obj.Nome
                        select new
                        {
                            obj.Codigo,
                            CPF = obj.CPF_Formatado,
                            obj.Nome
                        }
                    ).ToList() : null,
                    TempoLimiteConfirmacaoMotorista = carregamento.TipoOperacao?.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false ? carregamento.TempoLimiteConfirmacaoMotorista.TotalSeconds > 0 ? carregamento.TempoLimiteConfirmacaoMotorista : carregamento.TipoOperacao?.ConfiguracaoMobile?.TempoLimiteConfirmacaoMotorista : new TimeSpan(),
                    NecessarioConfirmacaoMotorista = carregamento.TipoOperacao?.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false
                },
                Carregamento = new
                {
                    Carregamento = new { carregamento.Codigo, carregamento.Descricao },
                    EscolherHorarioCarregamentoPorLista = centroCarregamento?.EscolherHorarioCarregamentoPorLista ?? false,
                    carregamento.EncaixarHorario,
                    carregamento.TipoMontagemCarga,
                    Situacao = carregamento.SituacaoCarregamento,
                    carregamento.VeiculoBloqueado,
                    PreCarga = new { Codigo = carregamento.PreCarga?.Codigo ?? 0, Descricao = carregamento.PreCarga?.NumeroPreCarga ?? "" },
                    Recebedor = new { Codigo = carregamento.Recebedor?.Codigo ?? 0, Descricao = carregamento.Recebedor?.Descricao ?? "" },
                    Expedidor = new { Codigo = carregamento.Expedidor?.Codigo ?? 0, Descricao = carregamento.Expedidor?.Descricao ?? "" },
                    PedidoViagemNavio = new { Codigo = carregamento.PedidoViagemNavio?.Codigo ?? 0, Descricao = carregamento.PedidoViagemNavio?.Descricao ?? "" },
                    carregamento.CarregamentoRedespacho,
                    GerandoCargaBackground = (carregamento.SituacaoCarregamento == SituacaoCarregamento.GerandoCargaBackground ? true : false),
                    PesoCarregamento = carregamento.PesoCarregamento.ToString("n4"),
                    PesoPalletCarregamento = (from p in pedidosCarregamento select p.PesoPallet).Sum().ToString("n4"),
                    PesoComPalletCarregamento = (carregamento.PesoCarregamento + (from p in pedidosCarregamento select p.PesoPallet).Sum()).ToString("n4"),
                    PalletCarregamento = carregamento.PalletCarregamento.ToString("n2"),
                    DataCarregamento = exibirHoraDataCarregamentoEDescarregamento ? (carregamento.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "") : (carregamento.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? ""),
                    DataDescarregamento = exibirHoraDataCarregamentoEDescarregamento ? carregamento.DataDescarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty : carregamento.DataDescarregamentoCarga?.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataInicioViagemPrevista = carregamento.DataInicioViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DadosPorFiliais = (
                        from o in dadosPorFiliais
                        select new
                        {
                            Filial = new { Codigo = o.Filial.Codigo, Descricao = o.Filial.Descricao },
                            Empresa = new { Codigo = o.Empresa?.Codigo ?? 0, Descricao = o.Empresa?.Descricao ?? "" },
                            DataCarregamento = exibirHoraDataCarregamentoEDescarregamento ? (o.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "") : (o.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? ""),
                            DataDescarregamento = exibirHoraDataCarregamentoEDescarregamento ? (o.DataDescarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "") : (o.DataDescarregamentoCarga?.ToString("dd/MM/yyyy") ?? ""),
                            o.EncaixarHorario
                        }
                    ).ToList(),
                    ModeloVeicularCarga = new
                    {
                        Codigo = carregamento.ModeloVeicularCarga?.Codigo ?? 0,
                        Descricao = carregamento.ModeloVeicularCarga?.Descricao ?? "",
                        CapacidadePesoTransporte = carregamento.ModeloVeicularCarga?.CapacidadePesoTransporte.ToString("n4") ?? "0,00",
                        ToleranciaPesoMenor = carregamento.ModeloVeicularCarga?.ToleranciaPesoMenor.ToString("n4") ?? "0,00",
                        ModeloControlaCubagem = carregamento.ModeloVeicularCarga?.ModeloControlaCubagem ?? false,
                        Cubagem = carregamento.ModeloVeicularCarga?.Cubagem.ToString("n2") ?? "0,00",
                        ToleranciaMinimaCubagem = carregamento.ModeloVeicularCarga?.ToleranciaMinimaCubagem.ToString("n2") ?? "0,00",
                        VeiculoPaletizado = carregamento.ModeloVeicularCarga?.VeiculoPaletizado ?? false,
                        NumeroPaletes = carregamento.ModeloVeicularCarga?.NumeroPaletes?.ToString() ?? "0",
                        NumeroReboques = carregamento.ModeloVeicularCarga?.NumeroReboques?.ToString() ?? "0",
                        ToleranciaMinimaPaletes = carregamento.ModeloVeicularCarga?.ToleranciaMinimaPaletes.ToString() ?? "0",
                        OcupacaoCubicaPaletes = carregamento.ModeloVeicularCarga?.OcupacaoCubicaPaletes.ToString("n2") ?? "0,00",
                        ExigirDefinicaoReboquePedido = carregamento.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false,
                        UnidadeCapacidade = (carregamento.ModeloVeicularCarga?.UnidadeCapacidade ?? UnidadeCapacidade.Peso) == UnidadeCapacidade.Unidade
                    },
                    TipoSeparacao = new
                    {
                        Codigo = carregamento.TipoSeparacao?.Codigo ?? 0,
                        Descricao = carregamento.TipoSeparacao?.Descricao ?? ""
                    },
                    carregamento.TipoCondicaoPagamento,
                    ValorFreteManual = (carregamento.ValorFreteManual > 0m) ? carregamento.ValorFreteManual.ToString("n2") : "",
                    carregamento.Observacao,
                    Pedidos = (from obj in pedidosCarregamento select serPedido.ObterDetalhesPedido(obj, produtosPedidosCarregamento, filiaisAtivas, null, configuracao, unitOfWork, montagemCargaPorPedidoProduto, pedidosCarregamentos, null)).ToList(),
                    Cargas = new List<dynamic>()
                },
                ClientesCarregamento = (
                    from c in pedidosCarregamento
                    where c.Pedido.Destinatario != null
                    select new
                    {
                        c.Pedido.Destinatario.CPF_CNPJ,
                        c.Pedido.Destinatario.Nome,
                        Categoria = c.Pedido.Destinatario?.Categoria?.Descricao ?? string.Empty,
                        Cor = c.Pedido.Destinatario?.Categoria?.Cor ?? string.Empty
                    }
                ).Distinct().OrderBy(x => x.Nome).ToList(),
                RestricoesClientesCarregamento = (from r in restricoesEntregas
                                                  select new
                                                  {
                                                      r.Codigo,
                                                      r.Descricao,
                                                      r.CorVisualizacao,
                                                      DT_FontColor = r.CorVisualizacao
                                                  }).Distinct().ToList(),
                //CanaisDeEntrega = ParseCanaisEntregaCarregamento(pedidosCarregamento, carregamento),
                MontagemCarregamentoPedidoProduto = montagemCargaPorPedidoProduto,
                PossuiProdutoPalletFechado = (montagemCargaPorPedidoProduto ? false : possuiProdutosPalletFechado),
                NotasFiscaisEnviar = (
                    from o in notasFiscaisDoCarregamento
                    select new
                    {
                        CodigoPedido = (from h in listaCarregamentoPedidoNotaFiscal where h.NotasFiscais.Contains(o) select h?.CarregamentoPedido?.Pedido?.Codigo).FirstOrDefault(),
                        o.Codigo,
                        o.Numero,
                        o.Chave,
                        DataEmissao = o.DataEmissao != DateTime.MinValue ? o.DataEmissao.ToString("dd/MM/yyyy") : "",
                        ValorTotal = o.ValorTotalProdutos.ToString("n2")
                    }
                ).ToList(),
                Roteirizacao = ParseInfoCarregamentoRoteirizacao(roteirizacoes, clientesRotas, carregamento, pedidosCarregamento, balancas, pracasPedagio)
            };

            return retorno;
        }

        private async Task<Models.Grid.Grid> ObterGridResumoCarregamentos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // pegar a sessão..
                // usar todos os carregamentos da sessão
                // ver todos os tipos pedidos distintos para gerar as colunas dinamicas do grid.. (Ver se é o canal de entrega.. o que gera as abas das ocupações)
                // gerar os dados.. rertornar o grid
                int codigoSessao = Request.GetIntParam("SessaoRoteirizador");

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repositorioSessao = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork, cancellationToken);
                //Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao = await repositorioSessao.BuscarPorCodigoAsync(codigoSessao, false);
                var carregamentosSessao = await repositorioCarregamento.ConsultarAsync(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamento()
                {
                    ProgramaComSessaoRoteirizador = true,
                    CodigoSessaoRoteirizador = codigoSessao
                }, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    LimiteRegistros = 1000
                });

                bool exibirHoraDataCarregamentoEDescarregamento = this.ConfiguracaoEmbarcador.InformaApoliceSeguroMontagemCarga || this.ConfiguracaoEmbarcador.InformaHorarioCarregamentoMontagemCarga;

                List<int> codigosCarregamentos = (from car in carregamentosSessao select car.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = await repositorioCarregamentoPedido.BuscarPorCarregamentosAsync(codigosCarregamentos);

                //List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao> roteirizacoes = await repositorioCarregamentoRoteirizacao.BuscarPorCarregamentosAsync(codigosCarregamentos);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto = await repositorioCarregamentoPedidoProduto.BuscarPorSessaoRoteirizadorAsync(codigoSessao);

                List<int> codigosPedidos = (from ped in carregamentoPedidos
                                            select ped.Pedido.Codigo).Distinct().ToList();

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = await repositorioPedidoProduto.BuscarPorPedidosAsync(codigosPedidos);

                List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> canaisEntrega = (from cp in carregamentoPedidos
                                                                                         where cp.Pedido?.CanalEntrega?.CanalEntregaPrincipal != null
                                                                                         select cp.Pedido.CanalEntrega.CanalEntregaPrincipal).Distinct().OrderBy(x => x?.Descricao).ToList();

                List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> canaisEntregaNaoPrincipais = (from cp in carregamentoPedidos
                                                                                                      where cp.Pedido?.CanalEntrega?.CanalEntregaPrincipal == null
                                                                                                      select cp.Pedido.CanalEntrega).Distinct().OrderBy(x => x?.Descricao).ToList();

                // Adicionando os canais de entrega que não possuem um canal de entrega principal.
                canaisEntrega.AddRange(canaisEntregaNaoPrincipais);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoLinhaSeparacao", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamento, "NumeroCarregamento", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.LinhaSeparacao, "LinhaSeparacao", 7, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Trasportadora", false);
                //grid.AdicionarCabecalho("KM", "KM", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Destinos, "Destinos", 16, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Perfil, "Perfil", 7, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.QuantidadePedidos, "QtdePedidos", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeCaixas, "QtdeCaixas", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeSKUs, "QtdeSKUs", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Peso, "OcupacaoPeso", 8, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Pallets, "OcupacaoPallet", 8, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Metro, "OcupacaoMetro", 8, Models.Grid.Align.right);

                foreach (var anal in canaisEntrega)
                    grid.AdicionarCabecalho(anal?.Descricao ?? "SEM C.E", "CE_" + anal?.Codigo.ToString(), 5, Models.Grid.Align.right);

                var lista = (
                    from p in carregamentosSessao
                    select new
                    {
                        p.Codigo,
                        CodigoLinhaSeparacao = 0,
                        p.NumeroCarregamento,
                        LinhaSeparacao = "",
                        Trasportadora = p.Empresa?.Descricao ?? "",
                        //KM = (from rot in roteirizacoes where rot.Carregamento.Codigo == p.Codigo select rot.DistanciaKM).FirstOrDefault().ToString("n3"),
                        Destinos = string.Join(",", (from cp in carregamentoPedidos where cp.Carregamento.Codigo == p.Codigo select cp.Pedido?.Destinatario?.Descricao).Distinct().ToArray()),
                        Perfil = p.ModeloVeicularCarga?.Descricao,
                        Data = exibirHoraDataCarregamentoEDescarregamento ? (p.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "") : (p.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? ""),
                        QtdePedidos = (from cp in carregamentoPedidos where cp.Carregamento.Codigo == p.Codigo select cp).Count(),
                        QtdeCaixas = (from cpp in carregamentosPedidoProduto
                                      where cpp.CarregamentoPedido.Carregamento.Codigo == p.Codigo
                                      select cpp.Quantidade).Sum(),
                        QtdeSKUs = (from cpp in carregamentosPedidoProduto
                                    where cpp.CarregamentoPedido.Carregamento.Codigo == p.Codigo
                                    select cpp.Quantidade * cpp.PedidoProduto.Produto.QuantidadeCaixa).Sum(),
                        OcupacaoPeso = (from cpp in carregamentosPedidoProduto where cpp.CarregamentoPedido.Carregamento.Codigo == p.Codigo select cpp.Peso).Sum().ToString("n2") + " | " +
                                          FormatarPercentual(p.ModeloVeicularCarga?.CapacidadePesoTransporte ?? 0, (from cpp in carregamentosPedidoProduto where cpp.CarregamentoPedido.Carregamento.Codigo == p.Codigo select cpp.Peso).Sum()),
                        OcupacaoPallet = (from cpp in carregamentosPedidoProduto where cpp.CarregamentoPedido.Carregamento.Codigo == p.Codigo select cpp.QuantidadePallet).Sum().ToString("n2") + " | " +
                                          FormatarPercentual(p.ModeloVeicularCarga?.NumeroPaletes ?? 0, (from cpp in carregamentosPedidoProduto where cpp.CarregamentoPedido.Carregamento.Codigo == p.Codigo select cpp.QuantidadePallet).Sum()),
                        OcupacaoMetro = (
                            (
                                from cpp in carregamentosPedidoProduto
                                where cpp.CarregamentoPedido.Carregamento.Codigo == p.Codigo
                                select cpp.MetroCubico
                            ).Sum().ToString("n2") + " | " +
                            FormatarPercentual(
                                p.ModeloVeicularCarga?.Cubagem ?? 0,
                                (
                                    from cpp in carregamentosPedidoProduto
                                    where cpp.CarregamentoPedido.Carregamento.Codigo == p.Codigo
                                    select cpp.MetroCubico
                                ).Sum() - ((p.TipoDeCarga?.Paletizado ?? false) ? p.ModeloVeicularCarga?.ObterOcupacaoCubicaPaletes() ?? 0m : 0m)
                            )
                        ),
                        DT_RowColor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Cinza,
                        DT_FontColor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco
                    }).ToList();

                List<dynamic> rows = new List<dynamic>();
                for (int i = 0; i < lista.Count; i++)
                {
                    rows.Add(lista[i]);
                    var linhas = (from cpp in carregamentosPedidoProduto
                                  where cpp.CarregamentoPedido.Carregamento.Codigo == lista[i].Codigo &&
                                        cpp.PedidoProduto.LinhaSeparacao != null
                                  select new { cpp.PedidoProduto.LinhaSeparacao.Codigo, cpp.PedidoProduto.LinhaSeparacao.Descricao }).Distinct().OrderBy(o => o.Descricao).ToList();

                    var modeloVeicular = (from car in carregamentosSessao
                                          where car.Codigo == lista[i].Codigo
                                          select car.ModeloVeicularCarga).FirstOrDefault();

                    for (int l = 0; l < linhas.Count; l++)
                    {
                        if (!string.IsNullOrEmpty(linhas[l].Descricao))
                        {
                            rows.Add(new
                            {
                                lista[i].Codigo,
                                CodigoLinhaSeparacao = linhas[l].Codigo,
                                lista[i].NumeroCarregamento,
                                LinhaSeparacao = linhas[l].Descricao,
                                Trasportadora = "",
                                Destinos = "",
                                Perfil = "",
                                Data = "",
                                QtdePedidos = (from cp in carregamentoPedidos
                                               join pedidoProduto in pedidoProdutos on cp.Pedido.Codigo equals pedidoProduto.Pedido.Codigo
                                               where cp.Carregamento.Codigo == lista[i].Codigo && pedidoProduto.LinhaSeparacao.Codigo == linhas[l].Codigo
                                               select cp).Count(),
                                QtdeCaixas = (from cpp in carregamentosPedidoProduto
                                              where cpp.CarregamentoPedido.Carregamento.Codigo == lista[i].Codigo &&
                                                    cpp.PedidoProduto.LinhaSeparacao.Codigo == linhas[l].Codigo
                                              select cpp.Quantidade).Sum(),
                                QtdeSKUs = (from cpp in carregamentosPedidoProduto
                                            where cpp.CarregamentoPedido.Carregamento.Codigo == lista[i].Codigo &&
                                                  cpp.PedidoProduto.LinhaSeparacao.Codigo == linhas[l].Codigo
                                            select cpp.Quantidade * cpp.PedidoProduto.Produto.QuantidadeCaixa).Sum(),
                                OcupacaoPeso = (from cpp in carregamentosPedidoProduto
                                                where cpp.CarregamentoPedido.Carregamento.Codigo == lista[i].Codigo && cpp.PedidoProduto.LinhaSeparacao.Codigo == linhas[l].Codigo
                                                select cpp.Peso).Sum().ToString("n2") + " | " +
                                              FormatarPercentual(modeloVeicular?.CapacidadePesoTransporte ?? 0, (from cpp in carregamentosPedidoProduto
                                                                                                                 where cpp.CarregamentoPedido.Carregamento.Codigo == lista[i].Codigo &&
                                                                                                                       cpp.PedidoProduto.LinhaSeparacao.Codigo == linhas[l].Codigo
                                                                                                                 select cpp.Peso).Sum()),
                                OcupacaoPallet = (from cpp in carregamentosPedidoProduto
                                                  where cpp.CarregamentoPedido.Carregamento.Codigo == lista[i].Codigo && cpp.PedidoProduto.LinhaSeparacao.Codigo == linhas[l].Codigo
                                                  select cpp.QuantidadePallet).Sum().ToString("n2") + " | " +
                                          FormatarPercentual(modeloVeicular?.NumeroPaletes ?? 0, (from cpp in carregamentosPedidoProduto
                                                                                                  where cpp.CarregamentoPedido.Carregamento.Codigo == lista[i].Codigo &&
                                                                                                        cpp.PedidoProduto.LinhaSeparacao.Codigo == linhas[l].Codigo
                                                                                                  select cpp.QuantidadePallet).Sum()),
                                OcupacaoMetro = ((from cpp in carregamentosPedidoProduto
                                                  where cpp.CarregamentoPedido.Carregamento.Codigo == lista[i].Codigo && cpp.PedidoProduto.LinhaSeparacao.Codigo == linhas[l].Codigo
                                                  select cpp.MetroCubico).Sum()).ToString("n2") + " | " +
                                          FormatarPercentual(modeloVeicular?.Cubagem ?? 0, (from cpp in carregamentosPedidoProduto
                                                                                            where cpp.CarregamentoPedido.Carregamento.Codigo == lista[i].Codigo &&
                                                                                                  cpp.PedidoProduto.LinhaSeparacao.Codigo == linhas[l].Codigo
                                                                                            select cpp.MetroCubico).Sum())
                            });
                        }
                    }
                }

                //paginacao... rsrsrss
                int quantidadeTotal = rows?.Count ?? 0;

                //Se não for exportação
                if (grid.limite > 0)
                    rows = rows.Skip(grid.inicio).Take(grid.limite).ToList();

                grid.AdicionaRows(rows);

                //Agora vamos fazer o totalizador por Canal de entrega
                for (int i = 0; i < grid.data.Count; i++)
                {
                    var codigo = int.Parse(grid.data[i]["Codigo"]);
                    var codigoLinhaSeparacao = int.Parse(grid.data[i]["CodigoLinhaSeparacao"]);

                    var pedidosCarregamento = (from ped in carregamentoPedidos
                                               where ped.Carregamento.Codigo == codigo
                                               select ped).ToList();

                    var produtosPedidosCarregamento = (from prod in carregamentosPedidoProduto
                                                       where prod.CarregamentoPedido.Carregamento.Codigo == codigo
                                                       select prod).ToList();

                    foreach (var anal in canaisEntrega)
                    {
                        if (codigoLinhaSeparacao == 0)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCanalEntrega = null;
                            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidosCanalEntrega = null;

                            if (anal?.Principal ?? false)
                            {
                                pedidosCanalEntrega = (from pedido in pedidosCarregamento
                                                       where (pedido.Pedido?.CanalEntrega?.CanalEntregaPrincipal?.Codigo ?? 0) == anal?.Codigo
                                                       select pedido).ToList();

                                produtosPedidosCanalEntrega = (from produto in produtosPedidosCarregamento
                                                               where (produto.PedidoProduto.Pedido?.CanalEntrega?.CanalEntregaPrincipal?.Codigo ?? 0) == anal?.Codigo
                                                               select produto).ToList();
                            }
                            else
                            {
                                pedidosCanalEntrega = (from pedido in pedidosCarregamento
                                                       where (pedido.Pedido?.CanalEntrega?.Codigo ?? 0) == anal?.Codigo
                                                       select pedido).ToList();

                                produtosPedidosCanalEntrega = (from produto in produtosPedidosCarregamento
                                                               where (produto.PedidoProduto.Pedido?.CanalEntrega?.Codigo ?? 0) == anal?.Codigo
                                                               select produto).ToList();
                            }

                            grid.data[i]["CE_" + anal?.Codigo.ToString()] = FormatarPercentual(produtosPedidosCarregamento.Sum(x => x.Peso), produtosPedidosCanalEntrega.Sum(x => x.Peso));
                        }
                        else
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> pedidosCanalEntrega = null;
                            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidosCanalEntrega = null;

                            if (anal?.Principal ?? false)
                            {
                                pedidosCanalEntrega = (from pedido in pedidosCarregamento
                                                       join pedidoProduto in pedidoProdutos on pedido.Codigo equals pedidoProduto.Pedido.Codigo
                                                       where (pedido.Pedido?.CanalEntrega?.CanalEntregaPrincipal?.Codigo ?? 0) == anal?.Codigo && pedidoProduto.LinhaSeparacao.Codigo == codigoLinhaSeparacao
                                                       select pedido).ToList();

                                produtosPedidosCanalEntrega = (from produto in produtosPedidosCarregamento
                                                               where (produto.PedidoProduto.Pedido?.CanalEntrega?.CanalEntregaPrincipal?.Codigo ?? 0) == anal?.Codigo &&
                                                                produto.PedidoProduto.LinhaSeparacao.Codigo == codigoLinhaSeparacao
                                                               select produto).ToList();
                            }
                            else
                            {
                                pedidosCanalEntrega = (from pedido in pedidosCarregamento
                                                       join pedidoProduto in pedidoProdutos on pedido.Codigo equals pedidoProduto.Pedido.Codigo
                                                       where (pedido.Pedido?.CanalEntrega?.Codigo ?? 0) == anal?.Codigo && pedidoProduto.LinhaSeparacao.Codigo == codigoLinhaSeparacao
                                                       select pedido).ToList();

                                produtosPedidosCanalEntrega = (from produto in produtosPedidosCarregamento
                                                               where (produto.PedidoProduto.Pedido?.CanalEntrega?.Codigo ?? 0) == anal?.Codigo &&
                                                                produto.PedidoProduto.LinhaSeparacao.Codigo == codigoLinhaSeparacao
                                                               select produto).ToList();
                            }
                            grid.data[i]["CE_" + anal?.Codigo.ToString()] = FormatarPercentual(produtosPedidosCarregamento.Sum(x => x.Peso), produtosPedidosCanalEntrega.Sum(x => x.Peso));
                        }
                    }
                }
                grid.setarQuantidadeTotal(quantidadeTotal);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private bool IsMontagemCargaPorPedido(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if ((pedidos.Count == 0))
                return false;

            return IsMontagemCargaPorPedidoProduto(pedidos, unitOfWork);
        }

        private bool IsMontagemCargaPorPedidoProduto(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Repositorio.UnitOfWork unitOfWork)
        {
            List<int> listaCodigoFilial = (from o in pedidos where o?.Filial != null select o.Filial.Codigo).Distinct().ToList();

            if ((listaCodigoFilial == null) || (listaCodigoFilial.Count == 0))
                return false;

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = repositorioCentroCarregamento.BuscarPorFiliais(listaCodigoFilial);

            return centrosCarregamento.Exists(x => x.MontagemCarregamentoPedidoProduto);
        }

        private bool IsMontagemCarregamentoSessaoFinalizada(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            if (carregamento?.SessaoRoteirizador == null)
                return false;
            else if (carregamento.SessaoRoteirizador.SituacaoSessaoRoteirizador == SituacaoSessaoRoteirizador.Finalizada || carregamento.SessaoRoteirizador.SituacaoSessaoRoteirizador == SituacaoSessaoRoteirizador.Cancelada)
                return true;
            else
                return false;
        }

        private decimal SalvarNotasFiscais(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repositorioCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);

            decimal pesoNf = 0;
            decimal pesoCubado = 0;

            string notasParaEnviar = Request.Params("NotasParaEnviar");

            if (string.IsNullOrWhiteSpace(notasParaEnviar))
                return 0;

            dynamic dynNotasParaEnviar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(notasParaEnviar);

            List<int> codigosNotasParaEnviar = new List<int>();
            List<int> codigosPedidos = new List<int>();

            foreach (var notaParaEnviar in dynNotasParaEnviar)
            {
                codigosNotasParaEnviar.Add(((string)notaParaEnviar.Codigo).ToInt());
                if (!codigosPedidos.Contains(((string)notaParaEnviar.CodigoPedido).ToInt()))
                    codigosPedidos.Add(((string)notaParaEnviar.CodigoPedido).ToInt());
            }

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> listaCarregamentoPedido = repositorioCarregamentoPedido.BuscarPorCarregamentoEPedidos(carregamento.Codigo, codigosPedidos);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> listaCarregamentoPedidoNotaFiscalDeletar = repositorioCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(carregamento.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal registroDeletar in listaCarregamentoPedidoNotaFiscalDeletar)
            {
                registroDeletar.NotasFiscais.Clear();
                repositorioCarregamentoPedidoNotaFiscal.Deletar(registroDeletar);
            }

            //TODO: REFAZER
            if (codigosNotasParaEnviar.Count == 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasDesconsiderar = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentosPedidoNotaFiscalExistentes = repositorioCarregamentoPedidoNotaFiscal.BuscarPorPedidoDoCarregamento(carregamento.Codigo);
                carregamentosPedidoNotaFiscalExistentes.ForEach(x => notasDesconsiderar.AddRange(x.NotasFiscais));

                // Alterado por acreditar que está incorreto, primeiro é consultado todos os carregamentosPedidoNF e depois estava consultando pedido a pedido novamente...
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal carregamentoPedidoNF in carregamentosPedidoNotaFiscalExistentes)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in carregamentoPedidoNF.NotasFiscais)
                    {
                        if (!notasDesconsiderar.Contains(notaFiscal))
                        {
                            notasDesconsiderar.Add(notaFiscal);
                            pesoNf += notaFiscal.Peso;
                            pesoCubado += notaFiscal.PesoCubado;
                        }
                    }
                }

                return (pesoCubado > pesoNf ? pesoCubado : pesoNf);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido in listaCarregamentoPedido)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal carregamentoPedidoNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal();
                carregamentoPedidoNotaFiscal.CarregamentoPedido = carregamentoPedido;
                carregamentoPedidoNotaFiscal.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in carregamentoPedido.Pedido.NotasFiscais)
                {
                    if (codigosNotasParaEnviar.Contains(notaFiscal.Codigo))
                    {
                        pesoNf += notaFiscal.Peso;
                        pesoCubado += notaFiscal.PesoCubado;
                        carregamentoPedidoNotaFiscal.NotasFiscais.Add(notaFiscal);
                    }
                }

                if (carregamentoPedidoNotaFiscal.NotasFiscais.Count > 0)
                    repositorioCarregamentoPedidoNotaFiscal.Inserir(carregamentoPedidoNotaFiscal);
            }

            return (pesoCubado > pesoNf ? pesoCubado : pesoNf);
        }

        private decimal SalvarPedidos(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, dynamic fronteiras, Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga, ref bool limparDadosRoteirizacao, ref decimal palletCarregamento, decimal palletInformadoCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            decimal pesoTotalCarregamento = 0;

            List<dynamic> dynPedidos = Request.GetListParam<dynamic>("Pedidos");
            List<int> listaCodigoPedido = (from pedido in dynPedidos select ((string)pedido.Codigo).ToInt()).ToList();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repositorioCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);

            // Lista que contem os pedidos do carregamento inicial... para validação se algum pedido foi removido do carregamento para excluir.
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosExiste = repCarregamentoPedido.BuscarPorCarregamentoFetchPedido(carregamento.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCodigos(listaCodigoPedido);

            if (!ValidarPedidos(pedidos, unitOfWork))
                throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemUmOuMaisPedidosSelecionadosQueNaoPossuemNFe);

            if (pedidos.Any(x => x.PedidoBloqueado) && !configuracaoMontagemCarga.PermitirGerarCarregamentoPedidoBloqueado)
                throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemUmOuMaisPedidosBloqueadosCarregamentoNaoPodeSerGerado);

            //#65051 e #66206-Primeiro pedem para bloquear.. depois para liberar...
            //if (pedidos.Any(x => x.PedidoRestricaoData) && !configuracaoMontagemCarga.PermitirGerarCarregamentoPedidoBloqueado)
            //    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemUmOuMaisPedidosBloqueadosCarregamentoNaoPodeSerGerado);

            if (pedidos.Any(x => !x.PedidoLiberadoMontagemCarga) && !configuracaoMontagemCarga.PermitirGerarCarregamentoPedidoBloqueado)
                throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemUmOuMaisPedidosNaoLiberadosParaMontagemCarregamentoNaoPodeSerGerado);

            if (pedidos.Any(x => x.SituacaoPedido == SituacaoPedido.Cancelado))
                throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ExistemPedidosCanceladosNaoSendoPossivelFinalizarCarregamento, string.Join(",", (from obj in pedidos where obj.SituacaoPedido == SituacaoPedido.Cancelado select obj.NumeroPedidoEmbarcador))));

            if (configuracaoMontagemCarga.NaoPermitirPedidosTomadoresDiferentesMesmoCarregamento)
            {
                List<Dominio.Enumeradores.TipoTomador> tipoTomadorPedidos = pedidos.Select(o => o.TipoTomador).Distinct().ToList();

                if (tipoTomadorPedidos.Count > 1)
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.NaoEPermitidoGerarCarregamentoParaPedidosDeTomadoresDiferentes);
            }

            bool montagemCargaPorPedidoProduto = IsMontagemCargaPorPedido(pedidos, unitOfWork);

            if (!montagemCargaPorPedidoProduto)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPorPedidos = repCarregamentoPedido.BuscarOutroCarregamentoPorPedidos(listaCodigoPedido, carregamento.CarregamentoRedespacho, carregamento.Codigo, carregamento.CarregamentoColeta);
                if (carregamentoPorPedidos != null)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PedidoJaEstaSendoUtilizadoNoCarregamento, carregamentoPorPedidos.Pedido.NumeroPedidoEmbarcador, carregamentoPorPedidos.Carregamento.NumeroCarregamento));
            }

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
            bool exigirDefinicaoReboquePedido = (carregamento.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false) && (carregamento.ModeloVeicularCarga?.NumeroReboques > 1);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProdutos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProdutosPorCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            // Vamos veriicar se houve alteração 
            IList<double> destinatariosExistentes = new List<double>();
            IList<double> destinatariosFinal = new List<double>();

            // Se já estiver true.... é inclusão de novo carregamento
            if (!limparDadosRoteirizacao)
                destinatariosExistentes = repCarregamentoPedido.DestinatariosCarregamento(carregamento.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPorPedidos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            if (montagemCargaPorPedidoProduto)
            {
                //Vamos consutar os produtos dos pedidos.. para relacionar ao carregamento pedido produto.
                produtosPedidos = repPedidoProduto.BuscarPorPedidos(listaCodigoPedido);
                List<int> codigosProdutosPedido = (from p in produtosPedidos select p.Codigo).ToList();
                carregamentosPedidoProdutos = repCarregamentoPedidoProduto.BuscarPorPedidoProdutos(codigosProdutosPedido);
                carregamentosPedidoProdutosPorCarregamento = repCarregamentoPedidoProduto.BuscarPorCarregamentoPedidoProdutos(carregamento.Codigo, listaCodigoPedido);

                //Obtendo todos os carregamentos do pedido para validar o peso total a ser carregado...
                carregamentosPorPedidos = repCarregamentoPedido.BuscarTodosCarregamentosPorPedidos(listaCodigoPedido, carregamento.CarregamentoRedespacho, carregamento.CarregamentoColeta);
            }
            else if (configuracaoMontagemCarga.TipoControleSaldoPedido == TipoControleSaldoPedido.Pallet)
            {
                carregamentosPorPedidos = repCarregamentoPedido.BuscarTodosCarregamentosPorPedidos(listaCodigoPedido, carregamento.CarregamentoRedespacho, carregamento.CarregamentoColeta);
            }

            Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> tiposPallet = repositorioTipoDetalhe.BuscarPorTipo(TipoTipoDetalhe.TipoPallet);

            foreach (dynamic pedido in dynPedidos)
            {
                int codigoPedido = ((string)pedido.Codigo).ToInt();
                //Peso a ser carregado do pedido
                decimal pesoCarregarPedido = Utilidades.Decimal.Converter((string)pedido.PesoPedidoCarregamento);
                decimal palletCarregarPedido = Utilidades.Decimal.Converter((string)pedido.PalletPedidoCarregamento);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> sessaoRoteirizadorPedidoProdutos = null;
                bool temControlePorArmazem = produtosPedidos.Exists(pr => pr.FilialArmazem != null);

                if (temControlePorArmazem)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto repositorioSessaoRoteirizadorPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto(unitOfWork);
                    sessaoRoteirizadorPedidoProdutos = repositorioSessaoRoteirizadorPedidoProduto.BuscarSessaoRoteirizadorPorPedido(codigoPedido);
                }

                int ordem = ((string)pedido.Ordem).ToInt();

                if (pesoCarregarPedido == 0)
                    pesoCarregarPedido = Utilidades.Decimal.Converter((string)pedido.PesoTotal);

                if (palletCarregarPedido == 0)
                    palletCarregarPedido = Utilidades.Decimal.Converter((string)pedido.PalletTotal);

                if (dynPedidos.Count == 1 && palletInformadoCarregamento > 0 && palletInformadoCarregamento != palletCarregarPedido)
                    palletCarregarPedido = palletInformadoCarregamento;

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedido = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

                if (montagemCargaPorPedidoProduto)
                {
                    pesoCarregarPedido = Utilidades.Decimal.Converter((string)pedido.PesoPedidoCarregamento);
                    produtosPedido = (from prod in produtosPedidos
                                      where prod.Pedido.Codigo == codigoPedido
                                      select prod).ToList();        ////repPedidoProduto.BuscarPorPedido(codigoPedido);
                    List<int> codigosProdutosPedido = (from p in produtosPedido select p.Codigo).ToList();
                    //Pega todos os carregamentos dos produtos do pedido
                    carregamentosPedidoProduto = (from cpp in carregamentosPedidoProdutos where codigosProdutosPedido.Contains(cpp.PedidoProduto.Codigo) select cpp).ToList();
                }

                NumeroReboque numeroReboque = ((string)pedido.NumeroReboque).ToEnum<NumeroReboque>();
                TipoCarregamentoPedido tipoCarregamentoPedido = ((string)pedido.TipoCarregamentoPedido).ToEnum(TipoCarregamentoPedido.Normal);
                DateTime? dataPrevisaoEntrega = ((string)pedido.DataPrevisaoEntrega).ToNullableDateTime();
                DateTime? dataCarregamentoCarga = ((string)pedido.DataCarregamento).ToNullableDateTime();
                DateTime? dataDescarregamentoCarga = ((string)pedido.DataDescarregamento).ToNullableDateTime();

                double codigoRecebedorPedido = ((string)pedido.CodRecebedor).ToDouble();

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = (from obj in carregamentoPedidosExiste where obj.Pedido.Codigo == codigoPedido select obj).FirstOrDefault();

                //Se adicionou o pedido no mapa.... vamos relacionar os produtos..
                bool criouCarregamentoPedido = false;

                Dominio.Entidades.Embarcador.Pedidos.Pedido objPedido = (from obj in pedidos where obj.Codigo == codigoPedido select obj).FirstOrDefault();

                if (configuracaoMontagemCarga.TipoControleSaldoPedido == TipoControleSaldoPedido.Pallet && palletCarregarPedido > 0 && objPedido.TotalPallets > 0)
                {
                    // Vamos calcular o peso proporcional;;;
                    pesoCarregarPedido = (palletCarregarPedido / objPedido.TotalPallets) * objPedido.PesoTotal;
                }
                else if (pesoCarregarPedido > 0 && objPedido.PesoTotal > 0)
                {
                    // Vamos calcular o pallet proporcional..
                    palletCarregarPedido = (pesoCarregarPedido / objPedido.PesoTotal) * objPedido.TotalPallets;
                }

                if (carregamentoPedido == null)
                {
                    criouCarregamentoPedido = true;
                    carregamentoPedido = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido
                    {
                        Carregamento = carregamento,
                        Pedido = objPedido,
                        NumeroReboque = numeroReboque,
                        TipoCarregamentoPedido = tipoCarregamentoPedido,
                        Peso = pesoCarregarPedido,
                        Pallet = palletCarregarPedido,
                        Ordem = ordem
                    };

                    if ((objPedido?.TipoPaleteCliente ?? TipoPaleteCliente.NaoDefinido) != TipoPaleteCliente.NaoDefinido)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhePalete = (from o in tiposPallet where o.TipoPaleteCliente == objPedido.TipoPaleteCliente select o).FirstOrDefault();
                        if (tipoDetalhePalete != null)
                            carregamentoPedido.PesoPallet = (carregamentoPedido.Pallet * tipoDetalhePalete?.Valor ?? 0);
                    }

                    if (carregamento.CarregamentoRedespacho && carregamentoPedido.Pedido.Expedidor == null)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPossivelAdicionarNoCarregamentoPoisMesmoNaoPossuiUmExpedidorCarregamentoUmCarregamentoDeRedespacho, carregamentoPedido.Pedido.NumeroPedidoEmbarcador));

                    if (pesoCarregarPedido == 0) carregamentoPedido.Peso = carregamentoPedido.Pedido.PesoTotal;

                    if (palletCarregarPedido == 0) carregamentoPedido.Pallet = carregamentoPedido.Pedido.TotalPallets;

                    if (carregamentoPedido.Pedido?.CanalEntrega?.NaoUtilizarCapacidadeVeiculoMontagemCarga ?? false)
                        carregamentoPedido.Peso = 0;

                    repCarregamentoPedido.Inserir(carregamentoPedido, historicoObjeto != null ? Auditado : null, historicoObjeto);

                    bool precisaDeFronteira = (carregamentoPedido.Pedido.Expedidor == null && carregamentoPedido.Pedido.Remetente != null && carregamentoPedido.Pedido.Remetente.Tipo == "E") ||
                        (carregamentoPedido.Pedido.Expedidor != null && carregamentoPedido.Pedido.Expedidor.Tipo == "E") ||
                        (carregamentoPedido.Pedido.Destinatario != null && carregamentoPedido.Pedido.Destinatario.Tipo == "E" && carregamentoPedido.Pedido.Recebedor == null);

                    if (
                        precisaDeFronteira
                        && fronteiras.Count == 0
                        && !ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal
                        && ConfiguracaoEmbarcador.FronteiraObrigatoriaMontagemCarga
                    )
                    {
                        throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioInformarFronteiraNoCarregamento);
                    }
                }
                else
                {
                    carregamentoPedido.Initialize();
                    carregamentoPedido.NumeroReboque = numeroReboque;
                    carregamentoPedido.TipoCarregamentoPedido = tipoCarregamentoPedido;
                    carregamentoPedido.Ordem = ordem;

                    if (pesoCarregarPedido > 0)
                        carregamentoPedido.Peso = pesoCarregarPedido;
                    else
                        carregamentoPedido.Peso = carregamentoPedido.Pedido.PesoTotal;

                    if (palletCarregarPedido > 0)
                        carregamentoPedido.Pallet = palletCarregarPedido;
                    else
                        carregamentoPedido.Pallet = carregamentoPedido.Pedido.TotalPallets;

                    if (carregamentoPedido.Pedido?.CanalEntrega?.NaoUtilizarCapacidadeVeiculoMontagemCarga ?? false)
                        carregamentoPedido.Peso = 0;

                    if ((carregamentoPedido.Pedido?.TipoPaleteCliente ?? TipoPaleteCliente.NaoDefinido) != TipoPaleteCliente.NaoDefinido)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhePalete = (from o in tiposPallet where o.TipoPaleteCliente == carregamentoPedido.Pedido.TipoPaleteCliente select o).FirstOrDefault();
                        if (tipoDetalhePalete != null)
                            carregamentoPedido.PesoPallet = (carregamentoPedido.Pallet * tipoDetalhePalete?.Valor ?? 0);
                    }

                    repCarregamentoPedido.Atualizar(carregamentoPedido, historicoObjeto != null && !montagemCargaPorPedidoProduto ? Auditado : null, historicoObjeto);
                }

                if (montagemCargaPorPedidoProduto)
                {
                    //Buscando todos os produtos do pedido em carregamentos
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidoCarregamentoExistente = (from ppc in carregamentosPedidoProdutosPorCarregamento
                                                                                                                                             where ppc.CarregamentoPedido.Pedido.Codigo == carregamentoPedido.Pedido.Codigo
                                                                                                                                             select ppc).ToList();
                    decimal pesoCarregarProdutosPedido = 0;
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in produtosPedido)
                    {
                        if (pedidoProduto.FilialArmazem != null)
                        {
                            bool produtoEstaNaSessaoRoteirizador = sessaoRoteirizadorPedidoProdutos.Exists(sr => sr.PedidoProduto.Codigo == pedidoProduto.Codigo);

                            if (!produtoEstaNaSessaoRoteirizador) continue;
                        }
                        //Filtrando o produto no carregamento
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto registro = (from pp in produtosPedidoCarregamentoExistente
                                                                                                                where pp.PedidoProduto.Codigo == pedidoProduto.Codigo && pp.CarregamentoPedido.Carregamento.Codigo == carregamento.Codigo
                                                                                                                select pp).FirstOrDefault();
                        temControlePorArmazem = pedidoProduto.FilialArmazem != null;
                        if (registro == null)
                        {
                            decimal qtdeCarregar = (pedidoProduto.Quantidade - (from r in carregamentosPedidoProduto
                                                                                where r.PedidoProduto.Codigo == pedidoProduto.Codigo
                                                                                select r.Quantidade).Sum());

                            decimal pesoCarregar = (pedidoProduto.PesoTotal - (from r in carregamentosPedidoProduto
                                                                               where r.PedidoProduto.Codigo == pedidoProduto.Codigo
                                                                               select r.Peso).Sum());

                            decimal palletCarregar = (pedidoProduto.QuantidadePalet - (from r in carregamentosPedidoProduto
                                                                                       where r.PedidoProduto.Codigo == pedidoProduto.Codigo
                                                                                       select r.QuantidadePallet).Sum());

                            decimal metroCubico = pedidoProduto.MetroCubico;

                            if ((pesoCarregar > 0 || palletCarregar > 0) && criouCarregamentoPedido)
                            {
                                registro = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto()
                                {
                                    CarregamentoPedido = carregamentoPedido,
                                    MetroCubico = metroCubico,
                                    PedidoProduto = pedidoProduto,
                                    Peso = pesoCarregar,
                                    Quantidade = qtdeCarregar,
                                    QuantidadePallet = palletCarregar,
                                    QuantidadeOriginal = qtdeCarregar,
                                    QuantidadePalletOriginal = palletCarregar,
                                    MetroCubicoOriginal = metroCubico
                                };

                                if (registro.Quantidade > pedidoProduto.Quantidade && !temControlePorArmazem)
                                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.TresCarregamentoDoPedidoProdutoNaoPodeSerMaiorQueQuantidadeDoPedidoProduto, registro.Quantidade, pedidoProduto.Quantidade));

                                repCarregamentoPedidoProduto.Inserir(registro);
                                pesoCarregarProdutosPedido += pesoCarregar;
                            }
                        }
                        else
                        {
                            if (temControlePorArmazem) continue;

                            pesoCarregarProdutosPedido += registro.Peso;
                        }
                    }

                    if (pesoCarregarProdutosPedido > 0 && carregamentoPedido.Peso != pesoCarregarProdutosPedido)
                    {
                        carregamentoPedido.Peso = pesoCarregarProdutosPedido;
                        repCarregamentoPedido.Atualizar(carregamentoPedido, historicoObjeto != null && !montagemCargaPorPedidoProduto ? Auditado : null, historicoObjeto);
                    }

                    if (pesoCarregarProdutosPedido > 0)
                        pesoTotalCarregamento += pesoCarregarProdutosPedido;

                    palletCarregamento += palletCarregarPedido;

                    // Aki, vamos fazer a validação do peso máximo do pedido no carregamento..
                    // peso total do pedido - peso total de outros carregamentos = peso máximo.
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> outrosCarregamentosDoPedido = (from car in carregamentosPorPedidos
                                                                                                                              where car.Pedido.Codigo == codigoPedido && car.Carregamento.Codigo != carregamento.Codigo
                                                                                                                              select car).ToList();

                    //Contem o peso total dos carregamentos dos pedidos
                    decimal pesoTotalOutrosCarregamentosPedido = outrosCarregamentosDoPedido?.Sum(x => x.Peso) ?? 0;

                    if (montagemCargaPorPedidoProduto)
                    {
                        // Aki, vamos fazer a validação do peso máximo do pedido no carregamento..
                        // peso total do pedido - peso total de outros carregamentos = peso máximo.
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> outrosCarregamentosDoPedidoProduto = (from car in carregamentosPedidoProdutos
                                                                                                                                                join cap in outrosCarregamentosDoPedido on car.CarregamentoPedido.Codigo equals cap.Codigo
                                                                                                                                                select car).ToList();

                        pesoTotalOutrosCarregamentosPedido = outrosCarregamentosDoPedidoProduto.Sum(x => x.Peso);
                    }

                    if (pesoCarregarProdutosPedido + pesoTotalOutrosCarregamentosPedido - (decimal)0.5 > carregamentoPedido.Pedido.PesoTotal)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PesoMaximoPermitidoParaPedidoParaEsteCarregamento, carregamentoPedido.Pedido.NumeroPedidoEmbarcador, carregamentoPedido.Pedido.PesoTotal - pesoTotalOutrosCarregamentosPedido));
                }
                else
                {
                    if (configuracaoMontagemCarga.TipoControleSaldoPedido == TipoControleSaldoPedido.Pallet)
                    {
                        // Aki, vamos fazer a validação do peso máximo do pedido no carregamento..
                        // peso total do pedido - peso total de outros carregamentos = peso máximo.
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> outrosCarregamentosDoPedido = (from car in carregamentosPorPedidos
                                                                                                                                  where car.Pedido.Codigo == codigoPedido && car.Carregamento.Codigo != carregamento.Codigo
                                                                                                                                  select car).ToList();

                        //Contem o peso total dos carregamentos dos pedidos
                        decimal palletTotalOutrosCarregamentosPedido = outrosCarregamentosDoPedido?.Sum(x => x.Pallet) ?? 0;

                        if (palletCarregarPedido + palletTotalOutrosCarregamentosPedido - (decimal)0.1 > carregamentoPedido.Pedido.TotalPallets)
                            throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PalletMaximoPermitidoParaPedidoParaEsteCarregamento, carregamentoPedido.Pedido.NumeroPedidoEmbarcador, carregamentoPedido.Pedido.TotalPallets - palletTotalOutrosCarregamentosPedido));
                    }
                    //Somando o peso carregado do pedido e não dos produtos.. problema NATONE onde o peso do pedido é divergente do peso dos produtos..
                    pesoTotalCarregamento += carregamentoPedido.Peso;
                    palletCarregamento += carregamentoPedido.Pallet;
                }

                decimal pesoPedidoCarregamento = Math.Round(carregamentoPedido.Pedido.PesoTotal, 3, MidpointRounding.ToEven);
                decimal pesoPedidoTotal = Math.Round(carregamentoPedido.Pedido.PesoTotal, 3, MidpointRounding.ToEven);
                if (pesoPedidoCarregamento > pesoPedidoTotal) // O peso {0} do pedido {1} a carregar neste carregamento não pode ser superior ao peso total do pedido {2}..
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PesoCarregarNesteCarregamentoNaoPodeSerSuperior, pesoPedidoCarregamento, carregamentoPedido.Pedido.NumeroPedidoEmbarcador, pesoPedidoTotal));

                if (exigirDefinicaoReboquePedido && (numeroReboque == NumeroReboque.SemReboque))
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ReboqueNaoFoiDefinidoParaPedido, carregamentoPedido.Pedido.NumeroPedidoEmbarcador));

                carregamentoPedido.Pedido.Initialize();

                if (codigoRecebedorPedido > 0)
                {
                    if ((carregamentoPedido.Pedido.Recebedor?.Codigo ?? 0) != codigoRecebedorPedido)
                        limparDadosRoteirizacao = true;

                    carregamentoPedido.Pedido.Recebedor = repCliente.BuscarPorCPFCNPJ(codigoRecebedorPedido);
                }

                if (pedido.CodRecebedor == 0 && carregamentoPedido.Pedido.Recebedor != null)
                {
                    carregamentoPedido.Pedido.Recebedor = null;
                    limparDadosRoteirizacao = true;
                }

                if (dataPrevisaoEntrega.HasValue)
                    carregamentoPedido.Pedido.PrevisaoEntrega = dataPrevisaoEntrega;

                if (carregamento.DataPrevisaoRetorno.HasValue)
                    carregamentoPedido.Pedido.PrevisaoEntrega = carregamento.DataPrevisaoRetorno;

                if (carregamento.DataPrevisaoSaida.HasValue)
                    carregamentoPedido.Pedido.DataPrevisaoSaida = carregamento.DataPrevisaoSaida;

                if (carregamento.Motoristas.Count > 0)
                {
                    carregamentoPedido.Pedido.Motoristas.Clear();
                    foreach (Dominio.Entidades.Usuario motorista in carregamento.Motoristas)
                        carregamentoPedido.Pedido.Motoristas.Add(motorista);
                }

                if (carregamento.ModeloVeicularCarga != null && carregamento.ModeloVeicularCarga.Codigo > 0)
                    carregamentoPedido.Pedido.ModeloVeicularCarga = carregamento.ModeloVeicularCarga;

                if (pedido.TipoPaleteCliente != null)
                    carregamentoPedido.Pedido.TipoPaleteCliente = pedido.TipoPaleteCliente;

                repPedido.Atualizar(carregamentoPedido.Pedido);
                Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, carregamentoPedido.Pedido, carregamentoPedido.Pedido.GetChanges(), "", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

                if (carregamentoPedido.Pedido.Filial != null)
                {
                    if (!filiais.Contains(carregamentoPedido.Pedido.Filial))
                        filiais.Add(carregamentoPedido.Pedido.Filial);
                }

                if (carregamentoPedido.Pedido.Destinatario != null)
                {
                    if (carregamentoPedido.Pedido.Recebedor != null && !carregamento.CarregamentoRedespacho)
                    {
                        if (!destinatarios.Contains(carregamentoPedido.Pedido.Recebedor))
                            destinatarios.Add(carregamentoPedido.Pedido.Recebedor);
                    }
                    else
                    {
                        if (!destinatarios.Contains(carregamentoPedido.Pedido.Destinatario))
                            destinatarios.Add(carregamentoPedido.Pedido.Destinatario);
                    }
                }

                if (carregamentoPedido.Pedido.TipoPaleteCliente != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhePalete = (from o in tiposPallet where o.TipoPaleteCliente == carregamentoPedido.Pedido.TipoPaleteCliente select o).FirstOrDefault();
                    if (tipoDetalhePalete != null)
                    {
                        carregamentoPedido.PesoPallet = (carregamentoPedido.Pallet * tipoDetalhePalete?.Valor ?? 0);
                        repCarregamentoPedido.Atualizar(carregamentoPedido);
                    }
                }
            }

            if ((carregamento.TipoMontagemCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga.NovaCarga) && (carregamento.PreCarga == null))
            {
                bool exigirPreCargaMontagemCarga = (from filial in filiais where filial.ExigirPreCargaMontagemCarga select filial.ExigirPreCargaMontagemCarga).FirstOrDefault();

                if (exigirPreCargaMontagemCarga)
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.PreCargaDeveSerInformada);
            }

            carregamento.Filiais = string.Join(",", (from obj in filiais select obj.Descricao).ToList());
            carregamento.Filial = carregamento.Filial ?? filiais.FirstOrDefault();
            carregamento.Destinatarios = string.Join(",", (from obj in destinatarios select obj.Descricao).ToList());
            carregamento.Destinos = string.Join(",", (from obj in destinatarios select obj.Localidade.DescricaoCidadeEstado).Distinct().ToList());
            repCarregamento.Atualizar(carregamento);

            //Removendo os pedidos do carregamentos que não estão selecionados no carregamento.
            for (int i = 0; i < carregamentoPedidosExiste.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedidoExistente = carregamentoPedidosExiste[i];
                if (listaCodigoPedido.Contains(carregamentoPedidoExistente.Pedido.Codigo))
                    continue;

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidoProdutos = repCarregamentoPedidoProduto.BuscarPorCarregamentoPedido(carregamentoPedidoExistente.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidosNotasFiscais = repositorioCarregamentoPedidoNotaFiscal.BuscarPorCarregamentoPedido(carregamentoPedidoExistente.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto in carregamentoPedidoProdutos)
                    repCarregamentoPedidoProduto.Deletar(carregamentoPedidoProduto);

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal carregamentoPedidoNf in carregamentoPedidosNotasFiscais)
                {
                    carregamentoPedidoNf.NotasFiscais.Clear();
                    repositorioCarregamentoPedidoNotaFiscal.Deletar(carregamentoPedidoNf);
                }

                //carregamentoPedidoExistente.Pedido.PedidoTotalmenteCarregado = false;

                //repPedido.Atualizar(carregamentoPedidoExistente.Pedido);

                repCarregamentoPedido.Deletar(carregamentoPedidoExistente, historicoObjeto != null && !montagemCargaPorPedidoProduto ? Auditado : null, historicoObjeto);
            }

            if (montagemCargaPorPedidoProduto)
            {
                //Agora vamos remover.. todos os pedidos sem produtos.
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosSemProdutos = repCarregamentoPedido.BuscarPorCarregamentoSemProdutos(carregamento.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedidoRemover in carregamentoPedidosSemProdutos)
                    repCarregamentoPedido.Deletar(carregamentoPedidoRemover);
            }

            if (!limparDadosRoteirizacao)
            {
                unitOfWork.Flush();

                destinatariosFinal = repCarregamentoPedido.DestinatariosCarregamento(carregamento.Codigo);

                limparDadosRoteirizacao = destinatariosExistentes.Count != destinatariosFinal.Count;
            }

            return pesoTotalCarregamento;
        }

        private void SalvarCargas(ref Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, dynamic dynCargas, Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoCarga repCarregamentoCarga = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga> carregamentoCargasExiste = carregamento.Cargas != null ? carregamento.Cargas.ToList() : new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga>();

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);

            List<int> codigos = new List<int>();

            foreach (dynamic dynCarga in dynCargas)
                codigos.Add((int)dynCarga.Codigo);

            if (codigos.Count <= 0)
                throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioInformarAsCargasDoCarregamento);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga> carregamentosCargaOutros = repCarregamentoCarga.BuscarPorCargas(codigos);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentosAfetados = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();

            for (int i = 0; i < carregamentosCargaOutros.Count; i++)
            {
                int codigoCarregamentoModificado = 0;
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoCarga = carregamentosCargaOutros[i];
                if (carregamentoCarga.Carregamento.Codigo != carregamento.Codigo)
                {
                    codigoCarregamentoModificado = carregamentoCarga.Carregamento.Codigo;
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = carregamentoCarga.Carga;

                    if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPossivelRemoverCargaPoisSuaAtualSituacaoNaoPermiteModificacoes, carga.CodigoCargaEmbarcador, carga.DescricaoSituacaoCarga));

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carregamentoCarga.Carregamento, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CargaFoiRemovidaDesteCarregamentoFoiVinculadoAoCarregamento, carga.CodigoCargaEmbarcador, carregamento.NumeroCarregamento), unitOfWork);
                    repCarregamentoCarga.Deletar(carregamentoCarga);

                    carga.NaoGerarMDFe = false;
                    carga.Carregamento = null;

                    if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    {
                        carga.NaoExigeVeiculoParaEmissao = true;
                        carga.NaoGerarMDFe = true;
                    }
                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    {
                        carga.DataInicioCalculoFrete = DateTime.Now;
                        carga.CalculandoFrete = true;
                    }
                    else
                    {
                        if (carga.NaoExigeVeiculoParaEmissao && (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe))
                        {
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                            carga.DataInicioCalculoFrete = DateTime.Now;
                            carga.CalculandoFrete = true;
                        }
                    }

                    repCarga.Atualizar(carga);

                    if (!carregamentosAfetados.Contains(carregamentoCarga.Carregamento))
                        carregamentosAfetados.Add(carregamentoCarga.Carregamento);
                }

                if (codigoCarregamentoModificado > 0)
                {
                    int numeroCargas = repCarregamentoCarga.ContarPorCarregamento(codigoCarregamentoModificado);
                    if (numeroCargas <= 0)
                    {
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamentoModificado = repCarregamento.BuscarPorCodigo(codigoCarregamentoModificado);
                        carregamentoModificado.SituacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado;
                        repCarregamento.Atualizar(carregamentoModificado);
                    }
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamentoAfetado in carregamentosAfetados)
            {
                for (int i = 0; i < carregamentoAfetado.Cargas.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = carregamentoAfetado.Cargas.ToList()[i].Carga;
                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    {
                        carga.DataInicioCalculoFrete = DateTime.Now;
                        carga.CalculandoFrete = true;
                        repCarga.Atualizar(carga);
                    }
                    else
                    {
                        if (carga.NaoExigeVeiculoParaEmissao && (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe))
                        {
                            carga.DataInicioCalculoFrete = DateTime.Now;
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                            carga.CalculandoFrete = true;
                        }
                    }
                }
            }

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<string> destinatarios = new List<string>();
            List<string> destinos = new List<string>();
            List<string> codigosAgrupados = new List<string>();

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAuditar = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            bool podeCalcularFrete = true;
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasRecalcularFrete = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            for (int i = 0; i < codigos.Count; i++)
            {
                int codigoCarga = codigos[i];

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoCarga = (from obj in carregamentoCargasExiste where obj.Carga.Codigo == codigoCarga select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, true);

                if (carregamentoCarga == null)
                {
                    if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAgruparCargasComFreteInformadoPeloEmbarcadorCargasComFreteCalculadoPelaMultisoftware);

                    carregamentoCarga = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga
                    {
                        Carregamento = carregamento,
                        Carga = carga
                    };

                    carga.NaoGerarMDFe = true;
                    carga.Carregamento = carregamento;

                    if (carga.Empresa == null)
                        carga.Empresa = carregamento.Empresa;

                    if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    {
                        carga.NaoExigeVeiculoParaEmissao = true;
                        carga.NaoGerarMDFe = true;
                    }

                    if (carga.Veiculo == null)
                    {
                        carga.Veiculo = carregamento.Veiculo;
                        //carga.VeiculosVinculados = carregamento.Veiculo.VeiculosVinculados;
                        if (carregamento.Veiculo.VeiculosVinculados != null && carregamento.Veiculo.VeiculosVinculados.Count > 0)
                        {
                            carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                            foreach (Dominio.Entidades.Veiculo veiculo in carregamento.Veiculo.VeiculosVinculados)
                                carga.VeiculosVinculados.Add(veiculo);
                        }
                    }

                    carga.ModeloVeicularCarga = carregamento.ModeloVeicularCarga;
                    carga.PedidoViagemNavio = carregamento.PedidoViagemNavio;
                    bool possuiMotorista = carregamento.Motoristas?.Count > 0;

                    if (possuiMotorista)
                        servicoCargaMotorista.AtualizarMotoristas(carga, carregamento.Motoristas.ToList());

                    if (carga.CalculandoFrete)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPossivelSalvarCarregamentoPoisCargaEstaCalculandoFreteAguardeTenteNovamente, carga.CodigoCargaEmbarcador));

                    if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPossivelSalvarCarregamentoPoisAtualSituacaoDaCargaNaoPermiteModificaoes, carga.DescricaoSituacaoCarga, carga.CodigoCargaEmbarcador));

                    if (carga.TipoOperacao != null && carga.TipoDeCarga != null && carga.ModeloVeicularCarga != null && carga.Empresa != null && carga.Veiculo != null && possuiMotorista && carga.ModeloVeicularCarga != null)
                    {
                        if (carga.ExigeNotaFiscalParaCalcularFrete)
                        {
                            if (carga.DataEnvioUltimaNFe.HasValue || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                            {
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                                cargasRecalcularFrete.Add(carga);

                            }
                            else
                            {
                                podeCalcularFrete = false;
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                            }
                        }
                        else
                        {
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                            cargasRecalcularFrete.Add(carga);
                        }
                    }
                    else
                        podeCalcularFrete = false;

                    carga.Carregamento = carregamento;
                    carga.NaoGerarMDFe = true;

                    if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    {
                        carga.NaoExigeVeiculoParaEmissao = true;
                        carga.NaoGerarMDFe = true;
                    }

                    repCarga.Atualizar(carga, Auditado);

                    cargasAuditar.Add(carga);
                    repCarregamentoCarga.Inserir(carregamentoCarga, historicoObjeto != null ? Auditado : null, historicoObjeto);
                }
                else
                {
                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        cargasRecalcularFrete.Add(carga);
                    else
                        podeCalcularFrete = false;
                }
                codigosAgrupados.Add(carregamentoCarga.Carga.CodigoCargaEmbarcador);

                if (carregamentoCarga.Carga.Filial != null)
                {
                    if (!filiais.Contains(carregamentoCarga.Carga.Filial))
                        filiais.Add(carregamentoCarga.Carga.Filial);
                }

                string[] destinatariosSplit = carregamentoCarga.Carga?.DadosSumarizados.Destinatarios.Split('/');
                if (destinatariosSplit != null)
                {
                    for (int d = 0; d < destinatariosSplit.Length; d++)
                    {
                        string destinatario = destinatariosSplit[d];
                        if (!destinatarios.Contains(destinatario))
                            destinatarios.Add(destinatario);
                    }
                }
                string[] destinosSplit = carregamentoCarga.Carga?.DadosSumarizados.Destinos.Split('/');
                if (destinosSplit != null)
                {
                    for (int d = 0; d < destinosSplit.Length; d++)
                    {
                        string destino = destinosSplit[d];
                        if (!destinos.Contains(destino))
                            destinos.Add(destino);
                    }
                }
            }

            carregamento.Filiais = string.Join(",", (from obj in filiais select obj.Descricao).ToList());
            carregamento.Filial = carregamento.Filial ?? filiais.FirstOrDefault();
            carregamento.Destinatarios = string.Join("/", (from obj in destinatarios select obj).ToList());
            carregamento.Destinos = string.Join("/", (from obj in destinatarios select obj).ToList());
            repCarregamento.Atualizar(carregamento);

            if (podeCalcularFrete)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargasRecalcularFrete)
                {
                    carga.DataInicioCalculoFrete = DateTime.Now;
                    carga.CalculandoFrete = true;
                    repCarga.Atualizar(carga);
                }
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargasRecalcularFrete)
                {
                    carga.PossuiPendencia = true;
                    carga.MotivoPendencia = Localization.Resources.Cargas.MontagemCargaMapa.AlgumasCargasDesteCarregamentoNaoEstaoNaetapaDeCalculoDefretePorIssoNaoPossivelCalcularEsseFreteAinda;
                    repCarga.Atualizar(carga);
                }
            }

            for (int i = 0; i < carregamentoCargasExiste.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoCargaExiste = carregamentoCargasExiste[i];
                if (!codigos.Contains(carregamentoCargaExiste.Carga.Codigo))
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = carregamentoCargaExiste.Carga;

                    if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPossivelRemoverCargaPoisSuaAtualSituacaoNaoPermiteModificacoes, carga.CodigoCargaEmbarcador, carga.DescricaoSituacaoCarga));

                    carga.NaoGerarMDFe = false;
                    carga.Carregamento = null;
                    if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    {
                        carga.NaoExigeVeiculoParaEmissao = true;
                        carga.NaoGerarMDFe = true;
                    }

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    {
                        carga.DataInicioCalculoFrete = DateTime.Now;
                        carga.CalculandoFrete = true;
                    }
                    else
                    {
                        if (carga.NaoExigeVeiculoParaEmissao && (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe))
                        {
                            carga.DataInicioCalculoFrete = DateTime.Now;
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                            carga.CalculandoFrete = true;
                        }
                    }
                    repCarga.Atualizar(carga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, Localization.Resources.Cargas.MontagemCargaMapa.RemovidaDoCarregamento + " " + carregamento.NumeroCarregamento + ".", unitOfWork);
                    repCarregamentoCarga.Deletar(carregamentoCargaExiste, historicoObjeto != null ? Auditado : null, historicoObjeto);
                }
            }

            string codigoCarregamento = codigosAgrupados[0];
            string diferenca = "";
            int indiceDife = 0;
            for (int i = 1; i < codigosAgrupados.Count; i++)
            {
                for (int j = 0; j < codigosAgrupados[i].Length; j++)
                {
                    if (codigosAgrupados[i].Length > j && codigosAgrupados[i][j] != codigosAgrupados[0][j])
                    {
                        indiceDife = j;
                        break;
                    }
                }
                for (int j = indiceDife; j < codigosAgrupados[i].Length; j++)
                    diferenca += codigosAgrupados[i][j];

                codigoCarregamento += "/" + diferenca;
            }

            carregamento.NumeroCarregamento = Utilidades.String.Left(codigoCarregamento, 300);
            repCarregamento.Atualizar(carregamento);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaAutitar in cargasAuditar)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaAutitar, null, Localization.Resources.Cargas.MontagemCargaMapa.IncluidaNoCarregamento + " " + carregamento.NumeroCarregamento + ".", unitOfWork);
        }

        private void SalvarListaMotorista(ref Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, dynamic dynTransporte, Repositorio.UnitOfWork unidadeDeTrabalho, bool camposAlterados)
        {
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);

            if (carregamento.Motoristas == null)
                carregamento.Motoristas = new List<Dominio.Entidades.Usuario>();

            int quantidadeMotoristas = carregamento.Motoristas.Count;

            carregamento.Motoristas.Clear();

            TimeSpan tempoConfirmacaoMotorista = new TimeSpan();
            if (!string.IsNullOrWhiteSpace(dynTransporte?.TempoLimiteConfirmacaoMotorista?.ToString() ?? ""))
                tempoConfirmacaoMotorista = dynTransporte.TempoLimiteConfirmacaoMotorista;

            if (carregamento.TipoOperacao?.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false && tempoConfirmacaoMotorista.TotalSeconds > 0)
                carregamento.TempoLimiteConfirmacaoMotorista = tempoConfirmacaoMotorista;

            if (ConfiguracaoEmbarcador.DesativarMultiplosMotoristasMontagemCarga)
            {
                int codigoMotorista = ((string)dynTransporte.Motorista?.ToString() ?? "").ToInt();
                Dominio.Entidades.Usuario mototista = repMotorista.BuscarPorCodigo(codigoMotorista);

                if (mototista != null)
                    carregamento.Motoristas.Add(mototista);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Request.Params("ListaMotoristas")))
                    return;

                dynamic listaMotorista = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaMotoristas"));

                if (listaMotorista == null)
                    return;

                foreach (var motorista in listaMotorista)
                {
                    int.TryParse((string)motorista.Motorista.Codigo, out int codigo);
                    Dominio.Entidades.Usuario mototista = repMotorista.BuscarPorCodigo(codigo);

                    if (mototista != null)
                        carregamento.Motoristas.Add(mototista);
                }
            }

            camposAlterados = quantidadeMotoristas != carregamento.Motoristas.Count && camposAlterados;
        }

        private void SalvarListaAjudante(ref Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, dynamic dynTransporte, Repositorio.UnitOfWork unidadeDeTrabalho, bool camposAlterados)
        {
            Repositorio.Usuario repAjudante = new Repositorio.Usuario(unidadeDeTrabalho);

            if (carregamento.Ajudantes == null)
                carregamento.Ajudantes = new List<Dominio.Entidades.Usuario>();

            int quantidadeAjudantes = carregamento.Ajudantes.Count;

            carregamento.Ajudantes.Clear();

            if (string.IsNullOrEmpty(Request.Params("ListaAjudantes")))
                return;

            dynamic listaAjudante = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAjudantes"));

            if (listaAjudante == null)
                return;

            foreach (var ajudanteItem in listaAjudante)
            {
                int.TryParse((string)ajudanteItem.Ajudante.Codigo, out int codigo);
                Dominio.Entidades.Usuario ajudante = repAjudante.BuscarPorCodigo(codigo);

                if (ajudante != null)
                    carregamento.Ajudantes.Add(ajudante);
            }
        }

        private Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete ObterSimulacaoFrete(int codigoCarregamento, Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento cotacaoFreteCarregamento, Repositorio.UnitOfWork unitOfWork, out string msgFrete, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, out Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete repositorioSimulacaoFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacaoFrete = repositorioSimulacaoFrete.BuscarPorCarregamento(codigoCarregamento);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork).BuscarPorCarregamento(codigoCarregamento);

            if (simulacaoFrete == null)
                simulacaoFrete = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete();
            else
                simulacaoFrete.Initialize();

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCodigos(cotacaoFreteCarregamento.Pedidos);
            int leadTime = 0;
            decimal valorFrete = Servicos.Embarcador.Carga.Frete.CalcularFretePorCarregamento(cotacaoFreteCarregamento, null, out msgFrete, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, configuracaoTMS, ref leadTime, out dadosCalculo);
            decimal pesoTotal = carregamentoPedidos?.Sum(c => c.Peso) ?? 0;
            decimal pesoLiquidoTotal = carregamentoPedidos?.Sum(c => c.Pedido?.PesoLiquidoTotal ?? 0m) ?? 0m;
            if (pesoTotal == 0)
                pesoTotal = pedidos.Sum(p => p.PesoTotal);
            decimal valorTotalCarga = (pedidos.Sum(obj => obj.Produtos.Sum(p => p.ValorProduto * p.Quantidade)));
            decimal percentualSobValorMercadoria = (valorTotalCarga > 0) ? (valorFrete * 100) / valorTotalCarga : 0;

            simulacaoFrete.PesoFrete = pesoTotal;
            simulacaoFrete.PesoLiquidoFrete = pesoLiquidoTotal;
            simulacaoFrete.ValorMercadoria = valorTotalCarga;
            simulacaoFrete.ValorFrete = valorFrete;
            simulacaoFrete.LeadTime = leadTime;
            simulacaoFrete.Distancia = cotacaoFreteCarregamento.Distancia;
            simulacaoFrete.ValorPorPeso = valorFrete / (pesoTotal == 0 ? 1 : pesoTotal);
            simulacaoFrete.PercentualSobValorMercadoria = percentualSobValorMercadoria;
            simulacaoFrete.Carregamento = repositorioCarregamento.BuscarPorCodigo(codigoCarregamento);
            simulacaoFrete.SucessoSimulacao = string.IsNullOrWhiteSpace(msgFrete);

            if (simulacaoFrete.Codigo == 0)
                repositorioSimulacaoFrete.Inserir(simulacaoFrete);
            else
                repositorioSimulacaoFrete.Atualizar(simulacaoFrete);

            return simulacaoFrete;
        }

        private void ValidarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, bool montagemCarregamentoPedidoProduto, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carregamento.VeiculoBloqueado)
                throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.VeiculoSelecionadoDoCarregamentoNaoPossuiLicencaAtivaParaTransporteFavorSoliciteLiberacaoDaViagem, carregamento.NumeroCarregamento));

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);

            if ((TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && ConfiguracaoEmbarcador.RoteirizacaoObrigatoriaMontagemCarga)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                //Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.APIGoogle);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repositorioCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);

                //#33331
                bool roteirizacaoOpcionalTipoOperacao = false;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = carregamento.TipoOperacao;
                if (tipoOperacao == null)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes = (from o in carregamentoPedidos
                                                                                              where o.Pedido?.TipoOperacao != null
                                                                                              select o.Pedido?.TipoOperacao
                                                                                             ).Distinct().ToList();

                    roteirizacaoOpcionalTipoOperacao = tiposOperacoes.Any(x => x.NaoExigeRoteirizacaoMontagemCarga);
                }
                else
                    roteirizacaoOpcionalTipoOperacao = tipoOperacao.NaoExigeRoteirizacaoMontagemCarga;

                if ((ConfiguracaoEmbarcador.TipoMontagemCargaPadrao == TipoMontagemCarga.NovaCarga) && (carregamentoRoteirizacao == null) && !roteirizacaoOpcionalTipoOperacao)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioRoteirizarCarregamentoAntesDeSeguirComCarga, carregamento.NumeroCarregamento));
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && carregamento != null && carregamento.TipoOperacao != null)
                repositorioCarregamentoPedido.AtualizarTipoOperacaoPedidosDoCarregamento(carregamento.Codigo, carregamento.TipoOperacao.Codigo);

            if (ConfiguracaoEmbarcador.ObrigatorioGeracaoBlocosParaCarregamento && !carregamento.CarregamentoRedespacho)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repositorioBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(unitOfWork);
                int qtdeBlocosCarregamento = repositorioBlocoCarregamento.ContarPorCarregamento(carregamento.Codigo);

                if (qtdeBlocosCarregamento == 0)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioGerarBlocosDeCarregamentoAntesDeSeguirComCarga, carregamento.NumeroCarregamento));
            }

            ////#49354-Validação para não permitir gerar carga cquando valor > limite..
            decimal valorTotalMercadoriasPedidos = 0;
            decimal valorLimiteCarga = 99999999;

            if (carregamentoPedidos.Count > 0)
            {
                valorTotalMercadoriasPedidos = (from ped in carregamentoPedidos
                                                select ped.Pedido.ValorTotalNotasFiscais).Sum();

                var tiposOperacoes = (
                    from o in carregamentoPedidos
                    where o.Pedido?.TipoOperacao != null
                    select o.Pedido?.TipoOperacao?.Descricao
                ).Distinct().ToList();

                if ((TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && tiposOperacoes?.Count > 1 && !ConfiguracaoEmbarcador.PermitirTiposOperacoesDistintasMontagemCarga)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarCargaDoCarregamentoComPedidosDeTipoDeOperacoesDiferentes, carregamento.NumeroCarregamento, string.Join(",", tiposOperacoes)));

                var tiposCargas = (
                    from o in carregamentoPedidos
                    where o.Pedido?.TipoDeCarga != null
                    select o.Pedido?.TipoDeCarga?.Descricao
                ).Distinct().ToList();

                if (tiposCargas?.Count > 1 && carregamento.TipoDeCarga == null)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarCargaDoCarregamentoComPedidosDeTiposDeCargasDiferentes, carregamento.NumeroCarregamento, string.Join(",", tiposCargas)));

                var pedidosCancelados = (
                    from o in carregamentoPedidos
                    where o.Pedido?.SituacaoPedido == SituacaoPedido.Cancelado
                    select o.Pedido.NumeroPedidoEmbarcador
                ).Distinct().ToList();

                if (pedidosCancelados?.Count > 0)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.OsPedidosDoCarregamentoForamCancelados, carregamento.NumeroCarregamento, string.Join(",", pedidosCancelados)));

                if (configuracaoMontagemCarga.TipoControleSaldoPedido == TipoControleSaldoPedido.Pallet)
                {
                    List<string> pedidosTotalmenteCarregados = (
                        from o in carregamentoPedidos
                        where o.Carregamento.Codigo != carregamento.Codigo && (o.Pedido.PedidoTotalmenteCarregado || o.Pedido.PalletSaldoRestante < 0) && (o.Carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false) == (carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false)
                        select o.Pedido.NumeroPedidoEmbarcador
                    ).Distinct().ToList();

                    if (pedidosTotalmenteCarregados?.Count > 0)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.OsPedidosDoCarregamentoJaForamProgramadoCarga, carregamento.NumeroCarregamento, string.Join(",", pedidosTotalmenteCarregados)));
                }
                else
                {
                    List<string> pedidosTotalmenteCarregados = (
                            from o in carregamentoPedidos
                            where o.Carregamento.Codigo != carregamento.Codigo && (o.Pedido.PedidoTotalmenteCarregado || o.Pedido.PesoSaldoRestante < 0) && (o.Carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false) == (carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false)
                            select o.Pedido.NumeroPedidoEmbarcador
                        ).Distinct().ToList();

                    if (pedidosTotalmenteCarregados?.Count > 0)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.OsPedidosDoCarregamentoJaForamProgramadoCarga, carregamento.NumeroCarregamento, string.Join(",", pedidosTotalmenteCarregados)));
                }

                if (carregamentoPedidos.Any(x => x.Pedido.PedidoBloqueado))
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemUmOuMaisPedidosBloqueadosCargaNaoPodeSerGerada);

                //#65051 e #66206-Primeiro pedem para bloquear.. depois para liberar...
                //if (carregamentoPedidos.Any(x => x.Pedido.PedidoRestricaoData))
                //    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemUmOuMaisPedidosBloqueadosCargaNaoPodeSerGerada);

                if (carregamentoPedidos.Any(x => !x.Pedido.PedidoLiberadoMontagemCarga))
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.ExistemUmOuMaisPedidosNaoLiberadosParaMontagemCarregamentoNaoPodeSerGerado);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && carregamento.ModeloVeicularCarga != null)
                {
                    //#17664 Validar se os destinatários são fornecedores... e se não possui restrição para o modelo veicular
                    List<double> cnpjsDesnatarios = (from ped in carregamentoPedidos
                                                     where ped.Pedido.Destinatario != null
                                                     select ped.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList();

                    Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> modalidadePessoasFornecedores = repModalidadePessoas.BuscarPorTipo(TipoModalidade.Fornecedor, cnpjsDesnatarios);

                    if (modalidadePessoasFornecedores.Count > 0)
                    {
                        Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular repModalidadeFornecedorPessoasRestricaoModeloVeicular = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular(unitOfWork);
                        List<int> codigosModalidades = (from modalidade in modalidadePessoasFornecedores select modalidade.Codigo).ToList();
                        List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular = repModalidadeFornecedorPessoasRestricaoModeloVeicular.BuscarPorModalidades(codigosModalidades);

                        if (modalidadeFornecedorPessoasRestricaoModeloVeicular.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular modalidade in modalidadeFornecedorPessoasRestricaoModeloVeicular)
                            {
                                if (modalidade.ModeloVeicular.Codigo == carregamento.ModeloVeicularCarga.Codigo &&
                                    (modalidade.TipoOperacao == null || (modalidade.TipoOperacao?.Codigo ?? 0) == (carregamento.TipoOperacao?.Codigo ?? 0)))
                                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.OsFornecedoresDoCarregamentoPossuemRestricoesDeModeloVeicular, modalidade?.ModalidadeFornecedorPessoa?.ModalidadePessoas?.Cliente?.NomeCNPJ ?? "", carregamento.NumeroCarregamento, carregamento.ModeloVeicularCarga.Descricao));
                            }
                        }
                    }

                    //#14515 validar se o veículo possui Tolerância minima para carregamento... e o peso total for inferior.., não permitir gerar..
                    //#23986 Não permitir peso acima da capacidade                    
                    if (ConfiguracaoEmbarcador.ValidarCapacidadeModeloVeicularCargaNaMontagemCarga)
                    {
                        decimal toleranciaMinima = carregamento.ModeloVeicularCarga.ToleranciaPesoMenor;
                        decimal toleranciaMaxima = carregamento.ModeloVeicularCarga.ToleranciaPesoExtra;

                        decimal pesoMaximo = carregamento.ModeloVeicularCarga.CapacidadePesoTransporte + toleranciaMaxima;

                        decimal pesoCarregamento = carregamento.ModeloVeicularCarga.UnidadeCapacidade == UnidadeCapacidade.Peso ? carregamento.PesoCarregamento : (from obj in carregamentoPedidos select obj.Pedido.QtVolumes).Sum();
                        if (pesoCarregamento < toleranciaMinima)
                            throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarUmaCargaDoCarregamentoComPesoInferiorToleranciaMinimaDoModeloVeicular, carregamento.NumeroCarregamento, toleranciaMinima.ToString("n4"), carregamento.ModeloVeicularCarga.Descricao));

                        if (pesoCarregamento > pesoMaximo)
                            throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarUmaCargaDoCarregamentoComPesoSuperiorToleranciaMaximaDoModeloVeicular, carregamento.NumeroCarregamento, toleranciaMaxima.ToString("n4"), carregamento.ModeloVeicularCarga.Descricao));

                    }
                }

                //#40069
                // Quando os pedidos possui um número de pedido de devolução.. e o não possui o pedido de devolução relacionado,
                // não possibilitar gerar a carga até que o pedido de devolução não seja relacionado.
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosComPedidoDevolucaoSemPedidoDevolucao = (from obj in carregamentoPedidos
                                                                                                                 where !string.IsNullOrWhiteSpace(obj.Pedido.NumeroPedidoDevolucao) && obj.Pedido.PedidoDevolucao == null
                                                                                                                 select obj.Pedido).ToList();

                if (pedidosComPedidoDevolucaoSemPedidoDevolucao.Count > 0)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoGerarUmaCargaPoisPedidoDeDevolucaoAindaNaoFoiInformado, (from obj in pedidosComPedidoDevolucaoSemPedidoDevolucao
                                                                                                                                                                                     select obj.NumeroPedidoEmbarcador).FirstOrDefault()));
            }

            if ((carregamento.Empresa == null) && (carregamento.GrupoTransportador == null) && ConfiguracaoEmbarcador.TransportadorObrigatorioMontagemCarga)
                throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.TransportadorObrigatorioDoCarregamento, carregamento.NumeroCarregamento));

            if (carregamento.GrupoTransportador == null && ConfiguracaoEmbarcador.InformaApoliceSeguroMontagemCarga && !repositorioCarregamentoApolice.ExistePorCarregamento(carregamento.Codigo))
                throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ApoliceDeSeguroObrigatoriaDoCarregamento, carregamento.NumeroCarregamento));

            if ((carregamento.TipoDeCarga == null) && ConfiguracaoEmbarcador.TipoCargaObrigatorioMontagemCarga)
                throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.TipoDeCargaObrigatorioDoCarregamento, carregamento.NumeroCarregamento));

            if ((carregamento.TipoOperacao == null) && ConfiguracaoEmbarcador.TipoOperacaoObrigatorioMontagemCarga)
                throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.TipoDeOperacaoObrigatorioaDoCarregamento, carregamento.NumeroCarregamento));

            if (ConfiguracaoEmbarcador.SimulacaoFreteObrigatorioMontagemCarga)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete repositorioSimulacaoFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacao = repositorioSimulacaoFrete.BuscarPorCarregamento(carregamento.Codigo);

                if (simulacao?.SucessoSimulacao == false)
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioGerarSimulacaoDeFreteDoCarregamentoAntesDeGerarCarga, carregamento.NumeroCarregamento));
            }

            if (carregamento.TipoOperacao != null)
                valorLimiteCarga = (carregamento.TipoOperacao?.ConfiguracaoCarga?.ValorLimiteNaCarga ?? 99999999);
            else if (carregamentoPedidos.Count > 0)
                valorLimiteCarga = (from ped in carregamentoPedidos
                                    where (ped.Pedido.TipoOperacao?.ConfiguracaoCarga?.ValorLimiteNaCarga ?? 0) > 0
                                    select ped.Pedido.TipoOperacao.ConfiguracaoCarga.ValorLimiteNaCarga)?.FirstOrDefault() ?? 99999999;

            if (!string.IsNullOrEmpty(carregamento?.SessaoRoteirizador?.Parametros ?? null))
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros>(carregamento.SessaoRoteirizador.Parametros);

                if ((sessaoRoteirizadorParametros?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum) == TipoMontagemCarregamentoVRP.SimuladorFrete)
                    if (valorTotalMercadoriasPedidos > valorLimiteCarga)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ValorLimiteExcedidoCarregamento, carregamento.NumeroCarregamento));
            }

            // #33061 - Se for montagem carregamento por pedido produto, vamos validar se tem produto relacionado ao pedido.
            if (montagemCarregamentoPedidoProduto)
            {
                //Vamos buscar todos os produtos do carregamento, para validar se algum pedido está sem produto ou sua quantidade é "0".
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidosProdutos = repositorioCarregamentoPedidoProduto.BuscarPorCarregamento(carregamento.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido in carregamentoPedidos)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidoProdutos = (from cpp in carregamentoPedidosProdutos
                                                                                                                                    where cpp.CarregamentoPedido.Codigo == carregamentoPedido.Codigo
                                                                                                                                    select cpp).ToList();
                    if (carregamentoPedidoProdutos.Count == 0)
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.PedidoDoCarregamentoEstaSemNenhumProdutoRelacionadoPorFavorVerifique, carregamentoPedido.Pedido.NumeroPedidoEmbarcador, carregamento.NumeroCarregamento));

                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto in carregamentoPedidoProdutos)
                        if (carregamentoPedidoProduto.Quantidade <= 0)
                            throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ProdutoDoPedidoEstaComQuantidadeIncorretaNoCarregamentoPorFavorVerifique, carregamentoPedidoProduto.PedidoProduto.Produto.Descricao, carregamentoPedido.Pedido.NumeroPedidoEmbarcador, carregamento.NumeroCarregamento));

                }
            }

            List<double> destinatariosFinal = (from obj in carregamentoPedidos
                                               where obj.Pedido.Destinatario != null
                                               select obj.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList();

            this.ValidarRestricaoVeiculos(carregamento, carregamentoPedidos, unitOfWork);
        }

        private void ValidarRestricaoVeiculos(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (carregamento.SessaoRoteirizador == null)
                return;

            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = null;

            if (!string.IsNullOrEmpty(carregamento.SessaoRoteirizador.Parametros))
                sessaoRoteirizadorParametros = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros>(carregamento.SessaoRoteirizador.Parametros);

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = repositorioCentroCarregamento.BuscarPorFiliais(new List<int> { carregamento.SessaoRoteirizador.Filial.Codigo });

            if ((centrosCarregamento?.Count ?? 0) == 0)
                return;

            if (carregamento.ModeloVeicularCarga == null)
                return;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoVRP vrp = (sessaoRoteirizadorParametros != null ? sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP : centrosCarregamento.FirstOrDefault().TipoMontagemCarregamentoVRP);

            if (vrp != TipoMontagemCarregamentoVRP.VrpCapacity && vrp != TipoMontagemCarregamentoVRP.VrpTimeWindows)
                return;

            List<double> destinatariosFinal = (from obj in carregamentoPedidos
                                               where obj.Pedido.Recebedor != null
                                               select obj.Pedido.Recebedor.CPF_CNPJ).Distinct().ToList();

            destinatariosFinal.AddRange((from obj in carregamentoPedidos
                                         where obj.Pedido.Recebedor == null
                                         select obj.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList());

            //Agora vamos ver se for fornecedor.. para validar as restriçoes de veiculos..
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> modalidadePessoasFornecedores = repModalidadePessoas.BuscarPorTipo(TipoModalidade.Fornecedor, destinatariosFinal);

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>();
            if (modalidadePessoasFornecedores.Count > 0)
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular repModalidadeFornecedorPessoasRestricaoModeloVeicular = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular(unitOfWork);
                List<int> codigosModalidades = (from modalidade in modalidadePessoasFornecedores select modalidade.Codigo).ToList();
                modalidadeFornecedorPessoasRestricaoModeloVeicular = repModalidadeFornecedorPessoasRestricaoModeloVeicular.BuscarPorModalidades(codigosModalidades);
            }

            foreach (Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular modal in modalidadeFornecedorPessoasRestricaoModeloVeicular)
            {
                if (modal.ModeloVeicular.Codigo == carregamento.ModeloVeicularCarga.Codigo &&
                    (modal.TipoOperacao == null || (modal.TipoOperacao?.Codigo ?? 0) == (carregamento.TipoOperacao?.Codigo ?? 0)))
                    throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.DestinatarioPossuiRestricaoDoModeloVeicularNaAbaFornecedorDoCadastroDeClientesPorFavorVerifique, (from obj in modalidadePessoasFornecedores
                                                                                                                                                                                                                  where obj.Codigo == modal.ModalidadeFornecedorPessoa.ModalidadePessoas.Codigo
                                                                                                                                                                                                                  select obj.Cliente.Descricao).FirstOrDefault(), carregamento.ModeloVeicularCarga.Descricao));
            }

            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centroDescarregamentos = new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            centroDescarregamentos = repositorioCentroDescarregamento.BuscarPorDestinatarios(destinatariosFinal);

            List<int> codigoCanaisEntrega = (from canal in carregamentoPedidos select canal.Pedido.CanalEntrega?.Codigo ?? 0).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> temp = repositorioCentroDescarregamento.BuscarPorCanaisEntrega(codigoCanaisEntrega);

            temp = temp.FindAll(x => !destinatariosFinal.Contains(x.Destinatario?.CPF_CNPJ ?? 0)).ToList();
            centroDescarregamentos.AddRange(temp);

            //Se não possui restrição.. vamos ver os veiculos permitidos da Janela de Descarga.
            foreach (double cnpjDestinatario in destinatariosFinal)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = (from centro in centroDescarregamentos
                                                                                                      where (centro.Destinatario?.CPF_CNPJ ?? 0) == cnpjDestinatario
                                                                                                      select centro).FirstOrDefault();

                //Se o destinatário possuir um centro de descarregamento.
                if (centroDescarregamento != null && centroDescarregamento.VeiculosPermitidos.Count > 0)
                {
                    if (!centroDescarregamento.VeiculosPermitidos.Any(x => x.Codigo == carregamento.ModeloVeicularCarga.Codigo))
                        throw new ControllerException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.DestinatarioPossuiRestricaoDoModeloVeicularNaAbaFornecedorDoCadastroDeClientesPorFavorVerifique, centroDescarregamento.Destinatario.Descricao, carregamento.ModeloVeicularCarga.Descricao));
                }
            }
        }

        private bool ValidarPedidos(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || pedidos.Count == 0)
                return true;

            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarFirstOrDefaultPorPedidos(pedidos.Select(x => x.Codigo).ToList());

            if (agendamentoColeta == null || agendamentoColeta.TipoCarga == null)
                return true;

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarTipoOperacaoPorTipoDeCarga(agendamentoColeta.TipoCarga.Codigo);

            if (tipoOperacao == null || !tipoOperacao.BloquearMontagemCargaSemNotaFiscal)
                return true;

            return !pedidos.Any(pedido => pedido.NotasFiscais == null || pedido.NotasFiscais.Count == 0);
        }

        private void ProcessarPedidosEmSeparacao(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pedido.OcorrenciaPedido servicoOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);
            servicoOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoEmSeparacao, pedidos, ConfiguracaoEmbarcador, null);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaValorMinimoPorCarga> ValidarValorMinimoPorCarga(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosEmCarregamento, List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais, decimal valorMinimoPorCarga, ref List<int> CodigosPedidosRemoverCarregamento)
        {
            decimal valorTotalCarga = 0;
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaValorMinimoPorCarga> cargasQueNaoAtingiramValorMinimo = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaValorMinimoPorCarga>();
            foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in filiais)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from obj in pedidosEmCarregamento where obj.Filial == filial || obj.Filial == null select obj).ToList();

                valorTotalCarga = (from ped in pedidos
                                   select ped.ValorTotalNotasFiscais).Sum();

                if (valorMinimoPorCarga > valorTotalCarga)
                {
                    cargasQueNaoAtingiramValorMinimo.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaValorMinimoPorCarga
                    {
                        NumerosPedido = String.Join(",", (from ped in pedidos select ped.NumeroPedidoEmbarcador).ToList()),
                        Filial = filial.Descricao,
                        ValorTotal = valorTotalCarga.ToString("n2"),
                        ValorMinimoCarga = valorMinimoPorCarga.ToString("n2"),
                    });

                    CodigosPedidosRemoverCarregamento.AddRange((from ped in pedidos select ped.Codigo).ToList());
                }
            }

            return cargasQueNaoAtingiramValorMinimo;
        }

        #endregion Métodos Privados
    }
}

