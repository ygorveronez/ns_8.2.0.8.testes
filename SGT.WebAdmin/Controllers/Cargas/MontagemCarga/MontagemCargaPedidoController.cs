using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Diagnostics;


namespace SGT.WebAdmin.Controllers.Cargas.MontagemCarga
{
    [CustomAuthorize(new string[] { "ProdutosCarregamento", "ProdutosNaoAtendido", "BuscarSugestacaoPedidos" }, "Cargas/MontagemCarga", "Cargas/MontagemCargaMapa")]
    public class MontagemCargaPedidoController : BaseController
    {
        #region Construtores

        public MontagemCargaPedidoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterPedidos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = await repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistroAsync();

                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltrosPesquisa(
                    configuracaoMontagemCarga,
                    unitOfWork,
                    string.IsNullOrEmpty(Request.GetStringParam("CodigoCargaEmbarcador"))
                );
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = await repCentroCarregamento.BuscarPorFiliaisAsync(filtrosPesquisa.CodigosFilial);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = centrosCarregamento?.FirstOrDefault();

                filtrosPesquisa.TipoRoteirizacaoColetaEntrega = centroCarregamento?.TipoRoteirizacaoColetaEntrega;

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "desc",
                    InicioRegistros = Request.GetIntParam("Inicio"),
                    LimiteRegistros = Request.GetIntParam("Limite"),
                    PropriedadeOrdenar = "Codigo"
                };

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                int quantidade = await repositorioPedido.ContarConsultaAsync(filtrosPesquisa);

                if (quantidade == 0 && filtrosPesquisa.OpcaoSessaoRoteirizador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador.ABRIR_SESSAO &&
                                       filtrosPesquisa.ProgramaComSessaoRoteirizador)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoExistemPedidosComOsFiltrosInformados);

                bool continuar = Request.GetBoolParam("Continuar");

                if ((centroCarregamento?.QuantidadeMaximaPedidosSessaoRoteirizar ?? 0) > 0 && (centroCarregamento?.QuantidadeMaximaPedidosSessaoRoteirizar ?? 0) < quantidade && !continuar)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.LimiteDePedidosAtingidoParaCentroDeCarregamentoTotalDePedidosDesejaContinuar, centroCarregamento.QuantidadeMaximaPedidosSessaoRoteirizar, quantidade));

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidos = quantidade > 0 ? await repositorioPedido.ConsultarDadosPedidosAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidosSemSessao = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>();

                bool montagemPedidoProduto = false;

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repSessaoRoteirizador = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao = null;
                if (filtrosPesquisa.CodigoSessaoRoteirizador > 0)
                {
                    sessao = await repSessaoRoteirizador.BuscarPorCodigoAsync(filtrosPesquisa.CodigoSessaoRoteirizador, false);
                    montagemPedidoProduto = sessao.MontagemCarregamentoPedidoProduto;

                    if (filtrosPesquisa.PedidosSemSessao.Count > 0)
                        pedidosSemSessao = await repositorioPedido.ConsultarDadosPedidosPorCodigosAsync(filtrosPesquisa.PedidosSemSessao);

                    if (filtrosPesquisa.OpcaoSessaoRoteirizador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador.ADD_PEDIDOS_SESSAO)
                    {
                        if (sessao.SituacaoSessaoRoteirizador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Iniciada)
                            return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAdicionarMaisPedidosUmaSessaoDeRoteirizacaoComSituacaoDiferenteIniciada);
                    }
                }

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Filiais.FilialArmazem repFilialArmazem = new Repositorio.Embarcador.Filiais.FilialArmazem(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork, cancellationToken);


                List<string> filiais = await repFilial.BuscarListaCNPJAtivasAsync();
                List<int> filiaisComArmazem = filtrosPesquisa.HabilitarCadastroArmazem ? await repFilialArmazem.BuscarFiliaisPorCNPJFiliaisAsync(filiais) : new List<int>();
                bool exibirCampoOrdem = pedidos.Exists(o => !string.IsNullOrWhiteSpace(o.Ordem));

                bool existePedidosSemSessao = pedidosSemSessao.Count > 0;

                if ((montagemPedidoProduto && filtrosPesquisa.ProgramaComSessaoRoteirizador && filtrosPesquisa.OpcaoSessaoRoteirizador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador.ADD_PEDIDOS_SESSAO) || existePedidosSemSessao)
                {
                    pedidos.AddRange(pedidosSemSessao);

                    pedidosProdutos = await repositorioPedidoProduto.BuscarPorPedidosSemFetchAsync(pedidos.Select(obj => obj.Codigo).ToList());

                    if (filtrosPesquisa.CodigosLinhaSeparacao?.Count > 0)
                        pedidosProdutos = pedidosProdutos.Where(obj => filtrosPesquisa.CodigosLinhaSeparacao.Contains(obj.LinhaSeparacao.Codigo)).ToList();

                    if (filtrosPesquisa.CodigosProdutos?.Count > 0)
                        pedidosProdutos = pedidosProdutos.Where(obj => filtrosPesquisa.CodigosProdutos.Contains(obj.Produto.Codigo)).ToList();
                }

                int qtdeAddSessao = 0;
                int qtdeProdutosAddSessao = 0;
                List<int> codigosPedidos = (from pedido in pedidos select pedido.Codigo).Distinct().ToList();

                if (filtrosPesquisa.ProgramaComSessaoRoteirizador && filtrosPesquisa.OpcaoSessaoRoteirizador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador.CRIAR_NOVA)
                {
                    if (filtrosPesquisa.CodigosFilial.Count == 0)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoIniciarUmaSessaoDeRoteirizacaoSemInformarUmaFilial);

                    if (filtrosPesquisa.CodigosFilial.Count > 1)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoIniciarUmaSessaoDeRoteirizacaoComMaisDeUmaFilial);

                    sessao = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador();
                    string descricao = string.Empty;
                    if (filtrosPesquisa.DataInicial.HasValue)
                    {
                        sessao.DataInicial = filtrosPesquisa.DataInicial.Value.Date;
                        descricao = " - " + filtrosPesquisa.DataInicial.Value.Date.ToString("dd/MM/yyyy");
                    }

                    if (filtrosPesquisa.DataLimite.HasValue)
                    {
                        sessao.DataFinal = filtrosPesquisa.DataLimite.Value.Date;
                        descricao += " - " + filtrosPesquisa.DataLimite.Value.Date.ToString("dd/MM/yyyy");
                    }

                    sessao.Filial = await new Repositorio.Embarcador.Filiais.Filial(unitOfWork).BuscarPorCodigoAsync(filtrosPesquisa.CodigosFilial.FirstOrDefault());
                    sessao.Inicio = DateTime.Now;
                    sessao.SituacaoSessaoRoteirizador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Iniciada;
                    sessao.Usuario = this.Usuario;
                    sessao.UsuarioAtual = this.Usuario;
                    sessao.Descricao = sessao.Filial.Descricao + descricao;
                    sessao.TipoMontagemCarregamentoPedidoProduto = TipoMontagemCarregamentoPedidoProduto.AMBOS; // Será atualizado/questionado o usuário ao clicar no gerar "Carregamentos"
                    sessao.RoteirizacaoRedespacho = filtrosPesquisa.PedidosOrigemRecebedor;
                    if (filtrosPesquisa.Expedidor == 0)
                        sessao.Expedidor = null;
                    else
                        sessao.Expedidor = await new Repositorio.Cliente(unitOfWork).BuscarPorCPFCNPJAsync(filtrosPesquisa.Expedidor);

                    List<int> codigosCentroCarregamento = centrosCarregamento.Select(o => o.Codigo).ToList();
                    Repositorio.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota repositorioCentroCarregamentoDisponibilidadeFrota = new Repositorio.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota(unitOfWork, cancellationToken);
                    DateTime dataEntrega = (sessao.DataInicial.HasValue ? sessao.DataInicial.Value : sessao.Inicio.Date);
                    DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataEntrega);

                    List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeFrotas = await repositorioCentroCarregamentoDisponibilidadeFrota.BuscarPorCentrosDeCarregamentoEDiaAsync(codigosCentroCarregamento, diaSemana);
                    List<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento> temposCarregamentoCentro = centroCarregamento?.TemposCarregamento.ToList() ?? new List<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento>();

                    // Aqui, vamos levantar todos os parametros do centro...
                    Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros()
                    {
                        AgruparPedidosMesmoDestinatario = centroCarregamento?.AgruparPedidosMesmoDestinatario ?? false,
                        CarregamentoTempoMaximoRota = centroCarregamento?.CarregamentoTempoMaximoRota ?? 0,
                        ConsiderarTempoDeslocamentoCD = centroCarregamento?.ConsiderarTempoDeslocamentoPrimeiraEntrega ?? false,
                        GerarCarregamentoDoisDias = centroCarregamento?.GerarCarregamentoDoisDias ?? false,
                        GerarCarregamentosAlemDaDispFrota = centroCarregamento?.GerarCarregamentosAlemDaDispFrota ?? false,
                        MontagemCarregamentoPedidoProduto = centroCarregamento?.MontagemCarregamentoPedidoProduto ?? false,
                        NivelQuebraProdutoRoteirizar = centroCarregamento?.NivelQuebraProdutoRoteirizar ?? NivelQuebraProdutoRoteirizar.Caixa,
                        QuantidadeMaximaEntregasRoteirizar = centroCarregamento?.QuantidadeMaximaEntregasRoteirizar ?? 999,
                        TipoMontagemCarregamentoVRP = centroCarregamento?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum,
                        TipoOcupacaoMontagemCarregamentoVRP = centroCarregamento?.TipoOcupacaoMontagemCarregamentoVRP ?? TipoOcupacaoMontagemCarregamentoVRP.Peso,
                        UtilizarDispFrotaCentroDescCliente = centroCarregamento?.UtilizarDispFrotaCentroDescCliente ?? false,
                        DisponibilidadesFrota = (from disp in disponibilidadeFrotas
                                                 select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosDisponibilidadeFrota()
                                                 {
                                                     Codigo = disp.Codigo,
                                                     CodigoModeloVeicular = disp.ModeloVeicular?.Codigo ?? 0,
                                                     DescricaoModeloVeicular = disp.ModeloVeicular?.Descricao ?? string.Empty,
                                                     CodigoTransportador = disp.Transportador?.Codigo ?? 0,
                                                     DescricaoTransportador = disp.Transportador?.Descricao ?? string.Empty,
                                                     Quantidade = disp.Quantidade,
                                                     QuantidadeUtilizar = disp.Quantidade
                                                 }).ToList(),
                        TemposCarregamento = (from tempo in temposCarregamentoCentro
                                              select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento()
                                              {
                                                  Codigo = tempo.Codigo,
                                                  CodigoModeloVeicular = tempo.ModeloVeicular?.Codigo ?? 0,
                                                  DescricaoModeloVeicular = tempo.ModeloVeicular?.Descricao ?? string.Empty,
                                                  CodigoTipoCarga = tempo.TipoCarga?.Codigo ?? 0,
                                                  DescricaoTipoCarga = tempo.TipoCarga?.Descricao ?? string.Empty,
                                                  Quantidade = tempo.QuantidadeMaximaEntregasRoteirizar,
                                                  QuantidadeUtilizar = tempo.QuantidadeMaximaEntregasRoteirizar,
                                                  QuantidadeMinima = tempo.QuantidadeMinimaEntregasRoteirizar,
                                                  QuantidadeMinimaUtilizar = tempo.QuantidadeMinimaEntregasRoteirizar
                                              }).ToList()
                    };

                    sessao.Parametros = Newtonsoft.Json.JsonConvert.SerializeObject(sessaoRoteirizadorParametros);
                    sessao.MontagemCarregamentoPedidoProduto = centrosCarregamento.Exists(x => x.MontagemCarregamentoPedidoProduto);

                    if (centrosCarregamento.Exists(x => x.MontagemCarregamentoPedidoIntegral))
                        sessao.MontagemCarregamentoPedidoProduto = false;

                    //sessao.MontagemCarregamentoColetaEntrega = centrosCarregamento.Exists(x => x.MontagemCarregamentoColetaEntrega == true);
                    sessao.TipoRoteirizacaoColetaEntrega = centrosCarregamento.FirstOrDefault()?.TipoRoteirizacaoColetaEntrega ?? TipoRoteirizacaoColetaEntrega.Entrega;
                    sessao.Codigo = (int)await repSessaoRoteirizador.InserirAsync(sessao, Auditado);

                    if (sessao.MontagemCarregamentoPedidoProduto)
                    {
                        pedidosProdutos = await repositorioPedidoProduto.BuscarPorPedidosSemFetchAsync(codigosPedidos);
                        if ((filtrosPesquisa.CodigosLinhaSeparacao?.Count ?? 0) > 0)
                            pedidosProdutos = (from obj in pedidosProdutos where filtrosPesquisa.CodigosLinhaSeparacao.Contains(obj.LinhaSeparacao.Codigo) select obj).ToList();
                    }

                    filtrosPesquisa.CodigoSessaoRoteirizador = sessao.Codigo;
                    //Agora relacionar os pedidos...
                    Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                    repSessaoPedido.RelacionarPedidosSessao(sessao, codigosPedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador.CRIAR_NOVA, pedidosProdutos, ref qtdeAddSessao, ref qtdeProdutosAddSessao, unitOfWork);

                }
                else if (filtrosPesquisa.ProgramaComSessaoRoteirizador && filtrosPesquisa.OpcaoSessaoRoteirizador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador.ADD_PEDIDOS_SESSAO || existePedidosSemSessao)
                {
                    //Aki lascou.. ver os pedidos que não estão relacionados.. que retornou na consulta... e relacionar..
                    Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                    repSessaoPedido.RelacionarPedidosSessao(sessao, codigosPedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador.ADD_PEDIDOS_SESSAO, pedidosProdutos, ref qtdeAddSessao, ref qtdeProdutosAddSessao, unitOfWork);

                    //Vamos ver se alterou o filtros de data.. vamos atualizar da sessão..
                    bool alterar = false;
                    if (filtrosPesquisa.DataInicial.HasValue)
                    {
                        if (sessao.DataInicial?.Date > filtrosPesquisa.DataInicial.Value.Date)
                        {
                            sessao.DataInicial = filtrosPesquisa.DataInicial.Value.Date;
                            alterar = true;
                        }
                    }
                    if (filtrosPesquisa.DataLimite.HasValue)
                    {
                        if (sessao.DataFinal?.Date < filtrosPesquisa.DataLimite.Value.Date)
                        {
                            sessao.DataFinal = filtrosPesquisa.DataLimite.Value.Date;
                            alterar = true;
                        }
                    }
                    if (alterar)
                        await repSessaoRoteirizador.AtualizarAsync(sessao);
                }

                if (sessao != null && qtdeAddSessao > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, sessao, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.AdicionadoPedidosSessaoDeRoteirizacao, qtdeAddSessao), unitOfWork);

                List<int> pedidosColetaEntregaCarregamentoTipoOperacao = PedidosCarregamentoTipoOperacaoPedidoColetaEntrega(codigosPedidos, unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> notasFiscaisPedidos = ObterNotasFiscaisPorPedidos(codigosPedidos, configuracaoMontagemCarga.ExibirListagemNotasFiscais, unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais> pedidoDetalhesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais>();

                if (configuracaoMontagemCarga.ExibirListagemNotasFiscais)
                    pedidoDetalhesAdicionais = ObterDetalhesAdicionaisPedido(codigosPedidos, unitOfWork);

                bool montagemCarregamentoPedidoProduto = sessao?.MontagemCarregamentoPedidoProduto ?? false;
                bool exibirPercentualSeparacaoPedido = false;

                if (!montagemCarregamentoPedidoProduto)
                    exibirPercentualSeparacaoPedido = await repositorioPedido.ExistePedidoPercentualSeparacaoInformadoAsync();

                if (sessao != null)
                {
                    // #57771 - Remover pedidos "Cancelados"
                    List<int> codigosPedidosCancelados = (
                        from pedido in pedidos
                        where pedido.Situacao == SituacaoPedido.Cancelado
                        select pedido.Codigo
                    ).ToList();

                    // Vamos remover o pedido de possíveis carregamentos...
                    // Remover os pedidos da sessão
                    if (codigosPedidosCancelados.Count > 0)
                    {
                        // Remover os pedidos dos carregamentos da sessão...
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosCancelar = await repositorioCarregamentoPedido.BuscarPorPedidosEmMontagemAsync(sessao.Codigo, codigosPedidosCancelados);

                        for (int i = 0; i < carregamentoPedidosCancelar.Count; i++)
                            repositorioCarregamentoPedido.ExcluirCarregamentoPedido(carregamentoPedidosCancelar[i].Codigo);

                        //Agora remover os pedidos da sessão...
                        Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repositorioSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                        repositorioSessaoRoteirizadorPedido.AtualizarSituacao(sessao.Codigo, codigosPedidosCancelados, SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao);

                        pedidos = (
                            from pedido in pedidos
                            where !codigosPedidosCancelados.Any(codigoPedidoCancelado => codigoPedidoCancelado == pedido.Codigo)
                            select pedido
                        ).ToList();

                        codigosPedidos = (from pedido in pedidos select pedido.Codigo).Distinct().ToList();
                        quantidade -= codigosPedidosCancelados.Count;
                    }
                }

                List<dynamic> registros = new List<dynamic>();

                decimal pesoTotal = 0, pesoLiquidoTotal = 0, pesoSaldoRestante = 0, valorTotalPedidos = 0, cubagemTotal = 0;
                int volumeTotal = 0, saldoVolumesRestante = 0;

                List<double> totalEntregas = new List<double>();
                bool tipoColeta = (sessao?.TipoRoteirizacaoColetaEntrega ?? TipoRoteirizacaoColetaEntrega.Entrega) == TipoRoteirizacaoColetaEntrega.Coleta;

                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido repositorioMontagemCarregamentoBlocoSimuladorFretePedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repositorioAcompanhamentoEntregaConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos = await repositorioCarregamentoPedido.BuscarPorPedidosAsync(codigosPedidos);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simulacoesFretePedidos = await repositorioMontagemCarregamentoBlocoSimuladorFretePedido.BuscarPorPedidosAsync(codigosPedidos);

                Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendendicas = await repositorioAcompanhamentoEntregaConfiguracao.BuscarConfiguracaoAsync();

                foreach (var pedido in pedidos)
                {
                    registros.Add(
                        ObterDetalhesPedido(
                            pedido, montagemCarregamentoPedidoProduto, pedidosColetaEntregaCarregamentoTipoOperacao.Contains(pedido.Codigo), filiais,
                            notasFiscaisPedidos, ConfiguracaoEmbarcador, unitOfWork, filiaisComArmazem.Contains(pedido?.Filial?.Codigo ?? 0),
                            carregamentosPedidos, simulacoesFretePedidos, configuracaoTempoTendendicas, pedidoDetalhesAdicionais
                        )
                    );

                    pesoTotal += pedido.PesoTotal;
                    pesoLiquidoTotal += pedido.PesoLiquidoTotal;
                    pesoSaldoRestante += pedido.PesoSaldoRestante;
                    volumeTotal += pedido.QtVolumes;
                    saldoVolumesRestante += pedido.SaldoVolumesRestante;
                    valorTotalPedidos += pedido.ValorTotalNotasFiscais;
                    cubagemTotal += pedido.CubagemTotal;
                    totalEntregas.Add(tipoColeta ? pedido.Remetente?.Codigo ?? 0 : pedido.Destinatario?.Codigo ?? 0);
                }

                stopwatch.Stop();
                Servicos.Log.TratarErro($"Método Novo para {quantidade} pedidos: {stopwatch.ElapsedMilliseconds} ms", "MontagemCarga");

                var retorno = new
                {
                    Quantidade = quantidade,
                    PesoTotal = pesoTotal,
                    PesoLiquidoTotal = pesoLiquidoTotal,
                    PesoSaldoRestante = pesoSaldoRestante,
                    VolumeTotal = volumeTotal,
                    SaldoVolumesRestante = saldoVolumesRestante,
                    ValorTotalDosPedidos = valorTotalPedidos,
                    ExibirCampoOrdem = exibirCampoOrdem,
                    SessaoRoteirizador = filtrosPesquisa?.CodigoSessaoRoteirizador ?? 0,
                    MontagemCarregamentoPedidoProduto = montagemCarregamentoPedidoProduto,
                    TipoPedidoMontagemCarregamento = (from o in centrosCarregamento select o.TipoPedidoMontagemCarregamento)?.FirstOrDefault() ?? TipoPedidoMontagemCarregamento.Card,
                    TipoEdicaoPalletProdutoMontagemCarregamento = (from o in centrosCarregamento select o.TipoEdicaoPalletProdutoMontagemCarregamento)?.FirstOrDefault() ?? TipoEdicaoPalletProdutoMontagemCarregamento.ControlePalletAbertoFechado,
                    QtdeAddSessaoRoteirizador = qtdeAddSessao,
                    QtdeProdutosAddSessaoRoteirizador = qtdeProdutosAddSessao,
                    ExibirPercentualSeparacaoPedido = exibirPercentualSeparacaoPedido,
                    Registros = registros,
                    TotalEntregas = totalEntregas.Distinct().Count(),
                    TotalCubagem = cubagemTotal,
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.NumeroPedidoEmbarcador, "NumeroPedidoEmbarcador");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Data, "DataCarregamentoPedido");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Filial, "Filial");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Remetente, "Remetente");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Expedidor, "Expedidor");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Destinatario, "Destinatario");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Recebedor, "Recebedor");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Destino, "Destino");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Destino, "DestinoRecebedor");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Peso, "Peso");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.PesoLiquidoTotal, "PesoLiquido");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.PesoSaldoRestante, "PesoSaldoRestante");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.QuantidadeVolumes, "QuantidadeVolumes");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Ordem, "Ordem");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.NumeroPedido, "NumeroPedido");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.DataPrevisaoEntrega, "DataPrevisaoEntrega");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Cubagem, "Cubagem");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Percentual, "PercentualSeparacaoPedido");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.DataAgendamento, "DataAgendamento");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Carregamentos, "Carregamentos");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.CanalEntrega, "CanalEntrega");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.TotalPallets, "TotalPallets");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.ObservacaoCliente, "ObservacaoDestinatario");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.GrupoPessoa, "GrupoPessoa");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.PrazoEntrega, "PrazoEntrega");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.ValorFrete, "ValorFrete");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.CanalVenda, "CanalVenda");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.PedidoRestricaoData, "PedidoRestricaoData");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.PedidoBloqueado, "PedidoBloqueadoNaoLiberado");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.CodigoAgrupamentoCarregamento, "CodigoAgrupamentoCarregamento");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.NomeFantasia, "DestinatarioNomeFantasia");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.SituacaoComercial, "SituacaoComercial");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.SituacaoEstoque, "SituacaoEstoque");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.TendenciaEntrega, "TendenciaEntrega");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.NotasFiscais, "NotasFiscais");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.NumeroPrimeiraCarga, "PrimeiroCodigoCargaEmbarcador");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.DataDigitalizacaoCanhotoAvulso, "DataDigitalizacaoCanhotoAvulso");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.DataEntregaNotaCanhotoCliente, "DataEntregaNotaCanhoto");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Regiao, "Regiao");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Mesorregiao, "MesoRegiao");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Reentrega, "Reentrega");
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.ValorMercadoria, "ValorTotalNotasFiscais");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = await repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistroAsync();

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltrosPesquisa(configuracaoMontagemCarga, unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = await repCentroCarregamento.BuscarPorFiliaisAsync(filtrosPesquisa.CodigosFilial);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = centrosCarregamento?.FirstOrDefault();

                filtrosPesquisa.TipoRoteirizacaoColetaEntrega = centroCarregamento?.TipoRoteirizacaoColetaEntrega;

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "desc",
                    InicioRegistros = Request.GetIntParam("Inicio"),
                    LimiteRegistros = Request.GetIntParam("Limite"),
                    PropriedadeOrdenar = "Codigo"
                };

                int quantidade = await repositorioPedido.ContarConsultaAsync(filtrosPesquisa);

                if (quantidade == 0 && filtrosPesquisa.OpcaoSessaoRoteirizador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador.ABRIR_SESSAO &&
                                       filtrosPesquisa.ProgramaComSessaoRoteirizador)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoExistemPedidosComOsFiltrosInformados);

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidos = quantidade > 0 ? await repositorioPedido.ConsultarDadosPedidosAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>();

                List<int> codigosPedidos = (from pedido in pedidos select pedido.Codigo).Distinct().ToList();

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Filiais.FilialArmazem repFilialArmazem = new Repositorio.Embarcador.Filiais.FilialArmazem(unitOfWork, cancellationToken);

                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido repositorioMontagemCarregamentoBlocoSimuladorFretePedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repositorioAcompanhamentoEntregaConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);

                List<string> filiais = await repFilial.BuscarListaCNPJAtivasAsync();
                List<int> filiaisComArmazem = await repFilialArmazem.BuscarFiliaisPorCNPJFiliaisAsync(filiais);

                List<int> pedidosColetaEntregaCarregamentoTipoOperacao = PedidosCarregamentoTipoOperacaoPedidoColetaEntrega(codigosPedidos, unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> notasFiscaisPedidos = ObterNotasFiscaisPorPedidos(codigosPedidos, configuracaoMontagemCarga.ExibirListagemNotasFiscais, unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais> pedidoDetalhesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais>();

                if (configuracaoMontagemCarga.ExibirListagemNotasFiscais)
                    pedidoDetalhesAdicionais = ObterDetalhesAdicionaisPedido(codigosPedidos, unitOfWork);

                bool montagemCarregamentoPedidoProduto = false;

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos = await repositorioCarregamentoPedido.BuscarPorPedidosAsync(codigosPedidos);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simulacoesFretePedidos = await repositorioMontagemCarregamentoBlocoSimuladorFretePedido.BuscarPorPedidosAsync(codigosPedidos);

                Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendendicas = await repositorioAcompanhamentoEntregaConfiguracao.BuscarConfiguracaoAsync();

                List<dynamic> registros = new List<dynamic>();

                foreach (var pedido in pedidos)
                {
                    registros.Add(
                        ObterDetalhesPedido(
                            pedido, montagemCarregamentoPedidoProduto, pedidosColetaEntregaCarregamentoTipoOperacao.Contains(pedido.Codigo), filiais,
                            notasFiscaisPedidos, ConfiguracaoEmbarcador, unitOfWork, filiaisComArmazem.Contains(pedido?.Filial?.Codigo ?? 0),
                            carregamentosPedidos, simulacoesFretePedidos, configuracaoTempoTendendicas, pedidoDetalhesAdicionais
                        )
                    );
                }

                grid.AdicionaRows(registros);
                grid.setarQuantidadeTotal(quantidade);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);

                return Arquivo(arquivoBinario, "application/octet-stream", "Pedidos Montagem Carga." + grid.extensaoCSV);
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

        /// <summary>
        /// O método não está sendo utilizado
        /// </summary>
        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodosPedidos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                int codigoFilial = Request.GetIntParam("Filial");

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
                {
                    CodigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial },
                    CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork),
                    CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork),
                    DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                    Destinatario = Request.GetDoubleParam("Destinatario"),
                    Destino = Request.GetIntParam("Destino"),
                    EstadoDestino = Request.GetStringParam("EstadoDestino"),
                    EstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                    NumeroCarga = Request.GetStringParam("CodigoCargaEmbarcador"),
                    NumeroPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador"),
                    Origem = Request.GetIntParam("Origem"),
                    PedidoSemCarga = true,
                    PedidosRedespacho = Request.GetBoolParam("PedidosRedespacho"),
                    Remetente = Request.GetDoubleParam("Remetente"),
                    DataCriacaoPedidoInicio = Request.GetNullableDateTimeParam("DataCriacaoPedidoInicio"),
                    DataCriacaoPedidoLimite = Request.GetNullableDateTimeParam("DataCriacaoLimitePedido"),
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto,
                    TipoServicoMultisoftware = TipoServicoMultisoftware,
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "desc",
                    InicioRegistros = Request.GetIntParam("Inicio"),
                    LimiteRegistros = Request.GetIntParam("Limite"),
                    PropriedadeOrdenar = "Codigo"
                };

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidos = await repPedido.ConsultarDadosPedidosAsync(filtrosPesquisa, parametrosConsulta);
                List<int> codigosPedidos = (from pedido in pedidos select pedido.Codigo).Distinct().ToList();
                List<int> pedidosColetaEntregaCarregamentoTipoOperacao = PedidosCarregamentoTipoOperacaoPedidoColetaEntrega(codigosPedidos, unitOfWork);

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
                List<string> filiais = await repFilial.BuscarListaCNPJAtivasAsync();

                Repositorio.Embarcador.Filiais.FilialArmazem repFilialArmazem = new Repositorio.Embarcador.Filiais.FilialArmazem(unitOfWork, cancellationToken);
                List<int> filiaisComArmazem = await repFilialArmazem.BuscarFiliaisPorCNPJFiliaisAsync(filiais);

                return new JsonpResult((from obj in pedidos select ObterDetalhesPedido(obj, false, pedidosColetaEntregaCarregamentoTipoOperacao.Contains(obj.Codigo), filiais, null, ConfiguracaoEmbarcador, unitOfWork, filiaisComArmazem.Contains(obj.Filial.Codigo))).ToList());
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigoPedido(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                int codigo = Request.GetIntParam("Codigo");
                int codigoCarregamento = Request.GetIntParam("Carregamento");
                bool? telaPlanejamentoPedido = Request.GetNullableBoolParam("PlanejamentoPedido");

                if ((telaPlanejamentoPedido ?? false) && !Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirDetalhePedido))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.VoceNaoPossuiPermissaoParaCompletarEssaAcao);


                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repositorioPedido.BuscarPorCodigoAsync(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento repositorioPedidoDadosTransporteMaritimoRoteamento = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repositorioCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo repositorioTipoOperacaoAnexo = new Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimo = await repositorioPedidoDadosTransporteMaritimo.BuscarPorPedidoAsync(codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento> pedidoDadosTransporteMaritimoRoteamentos = await repositorioPedidoDadosTransporteMaritimoRoteamento.BuscarPorPedidoAsync(codigo);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal carregamentoPedidoNotaFiscal = await repositorioCarregamentoPedidoNotaFiscal.BuscarPorPedidoECarregamentoAsync(codigo, codigoCarregamento);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> listaCarregamentoPedidoNotasFiscais = await repositorioCarregamentoPedidoNotaFiscal.BuscarPorPedidoAsync(new List<int>() { codigo });

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotasFiscais = codigoCarregamento <= 0 ?
                    pedido.NotasFiscais.Where(o => o.nfAtiva && !listaCarregamentoPedidoNotasFiscais.Select(x => x.NotasFiscais).Any(x => x.Select(h => h.Codigo).Contains(o.Codigo))).ToList() :
                    pedido.NotasFiscais.Where(o => o.nfAtiva && !listaCarregamentoPedidoNotasFiscais.Where(t => t.CarregamentoPedido.Carregamento.Codigo != codigoCarregamento).Select(x => x.NotasFiscais).Any(x => x.Select(h => h.Codigo).Contains(o.Codigo))).ToList();

                Repositorio.Embarcador.Logistica.AgendamentoColetaAnexo repositorioAgendamentoColetaAnexo = new Repositorio.Embarcador.Logistica.AgendamentoColetaAnexo(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo> listaAnexosAgendamentoColeta = await repositorioAgendamentoColetaAnexo.BuscarPorPedidoAsync(codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo> anexosTipoOperacao = pedido.TipoOperacao != null ? await repositorioTipoOperacaoAnexo.BuscarPorCodigoTipoOperacaoAsync(pedido.TipoOperacao.Codigo) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo>();

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
                List<string> filiais = await repFilial.BuscarListaCNPJAtivasAsync();

                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

                List<dynamic> listaNotasEnviar = carregamentoPedidoNotaFiscal?.NotasFiscais != null ? carregamentoPedidoNotaFiscal.NotasFiscais.ToList<dynamic>() : new List<dynamic>();

                return new JsonpResult(new
                {
                    DetalhesPedido = servicoPedido.ObterDetalhesPedido(pedido, filiais, null, ConfiguracaoEmbarcador, unitOfWork),
                    DetalhesPedidoExportacao = new
                    {
                        AcondicionamentoCarga = pedido.AcondicionamentoCarga?.ObterDescricao() ?? "",
                        CargaPaletizada = pedido.CargaPaletizada ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                        ClienteAdicional = pedido.ClienteAdicional?.Descricao ?? "",
                        ClienteDonoContainer = pedido.ClienteDonoContainer?.Descricao ?? "",
                        DataEstufagem = pedido.DataEstufagem?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                        DataDeadLCargaNavioViagem = pedido.DataDeadLCargaNavioViagem?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                        DataDeadLineNavioViagem = pedido.DataDeadLineNavioViagem?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                        Despachante = pedido.Despachante,
                        ETA = pedido.DataETA?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                        ETS = pedido.DataETS?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                        PossuiGenset = pedido.PossuiGenset ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                        InLand = pedido.InLand,
                        NavioViagem = pedido.NavioViagem,
                        NumeroBooking = pedido.NumeroBooking ?? "",
                        NumeroEXP = pedido.NumeroEXP ?? "",
                        NumeroPedidoProvisorio = pedido.NumeroPedidoProvisorio ?? "",
                        NumeroReserva = pedido.Reserva ?? "",
                        PortoViagemDestino = pedido.PortoViagemDestino ?? "",
                        PortoViagemOrigem = pedido.PortoViagemOrigem ?? "",
                        RefEXPTransferencia = pedido.RefEXPTransferencia ?? "",
                        TipoContainer = pedido.ModeloVeicularCarga?.Descricao ?? "",
                        TipoProbe = pedido.TipoProbe?.ObterDescricao() ?? "",
                        ViaTransporte = pedido.ViaTransporte?.Descricao ?? ""
                    },
                    DetalhesPedidoTransporteMaritimo = servicoPedido.ObterDetalhesPedidoTransporteMaritimo(pedidoDadosTransporteMaritimo, pedidoDadosTransporteMaritimoRoteamentos),
                    DetalheNotaFiscalPedido = new
                    {
                        PesoTotalNFe = (listaNotasFiscais.Sum(x => x.Peso)).ToString("n2"),
                        PesoTotalCubado = (listaNotasFiscais.Sum(x => x.PesoCubado)).ToString("n2")
                    },
                    NotasFiscais = (from o in listaNotasFiscais
                                    select new
                                    {
                                        CodigoPedido = pedido.Codigo,
                                        o.Codigo,
                                        o.Numero,
                                        o.Chave,
                                        Peso = o.Peso.ToString("n2"),
                                        PesoCubado = o.PesoCubado.ToString("n2"),
                                        DataEmissao = o.DataEmissao != DateTime.MinValue ? o.DataEmissao.ToString("dd/MM/yyyy") : "",
                                        ValorTotal = o.Valor.ToString("n2")
                                    }).ToList(),
                    NotasFiscaisEnviar = (from o in listaNotasEnviar
                                          select new
                                          {
                                              CodigoPedido = pedido.Codigo,
                                              o.Codigo,
                                              o.Numero,
                                              o.Chave,
                                              Peso = o.Peso.ToString("n2"),
                                              PesoCubado = o.PesoCubado.ToString("n2"),
                                              DataEmissao = o.DataEmissao != DateTime.MinValue ? o.DataEmissao.ToString("dd/MM/yyyy") : "",
                                              ValorTotal = o.Valor.ToString("n2")
                                          }).ToList(),
                    ListaAnexosAgendamentoColeta = (from o in listaAnexosAgendamentoColeta
                                                    select new
                                                    {
                                                        o.Codigo,
                                                        o.Descricao,
                                                        o.NomeArquivo
                                                    }).ToList(),
                    Produtos = ObterProdutosPedido(pedido),
                    AnexosTipoOperacao = (
                        from anx in anexosTipoOperacao
                        select new
                        {
                            anx.Codigo,
                            anx.Descricao,
                            anx.NomeArquivo,
                        }
                    ).ToList(),
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarOsDetalhesDoPedido);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ProdutosCarregamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int carregamento = Request.GetIntParam("Carregamento");
                long cliente = Request.GetLongParam("Cliente");

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto rep = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
                if (carregamento > 0 || cliente > 0)
                    produtos = await rep.BuscarPorCarregamentoEDestinatarioAsync(carregamento, cliente);

                var lista = (
                    from p in produtos
                    select new
                    {
                        p.Codigo,
                        CodigoPedido = p.CarregamentoPedido.Pedido.Codigo,
                        CodigoPedidoProduto = p.PedidoProduto.Codigo,
                        PalletFechado = (p.PedidoProduto.PalletFechado ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao),
                        Cliente = (p.CarregamentoPedido?.Pedido?.Destinatario?.CodigoIntegracao?.Length <= 0 ? p.CarregamentoPedido?.Pedido?.Destinatario?.Nome : p.CarregamentoPedido?.Pedido?.Destinatario?.CodigoIntegracao),
                        Produto = p.PedidoProduto.Produto.Descricao,
                        Categoria = p.CarregamentoPedido.Pedido?.CanalEntrega?.Descricao,
                        LinhaSeparacao = p.PedidoProduto?.LinhaSeparacao?.Descricao,
                        Pedido = p.CarregamentoPedido.Pedido.NumeroPedidoEmbarcador,
                        Qtde = p.PedidoProduto.Quantidade,
                        QtdeCarregar = p.Quantidade.ToString("n2"),
                        Peso = p.PedidoProduto.PesoTotal,
                        PesoCarregar = p.Peso.ToString("n2"),
                        Pallet = p.PedidoProduto.QuantidadePalet.ToString("n2"),
                        PalletCarregar = p.QuantidadePallet.ToString("n2"),
                        Metro = p.PedidoProduto.MetroCubico,
                        MetroCarregar = p.MetroCubico.ToString("n2"),
                        QtdeOriginal = p.QuantidadeOriginal.ToString("n2"),
                        PalletOriginal = p.QuantidadePalletOriginal.ToString("n2"),
                        MetroOriginal = p.MetroCubicoOriginal.ToString("n2"),
                        QuantidadeCaixaPorPallet = (p.PedidoProduto.QuantidadeCaixaPorPallet > 0 ? p.PedidoProduto.QuantidadeCaixaPorPallet : p.PedidoProduto.Produto.QuantidadeCaixaPorPallet),
                        DT_RowColor = ((!p.PedidoProduto.PalletFechado && p.PedidoProduto.Quantidade > p.Quantidade ||
                                         p.PedidoProduto.PalletFechado && p.PedidoProduto.QuantidadePalet > p.QuantidadePallet) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo : ""),
                        DT_Enable = true,
                        DT_RowId = p.Codigo
                    }).ToList();

                return new JsonpResult(lista);
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

        public async Task<IActionResult> ProdutosNaoAtendido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoSessao = Request.GetIntParam("SessaoRoteirizador");
                long cliente = Request.GetLongParam("Cliente");
                bool basicDataTable = Request.GetBoolParam("BasicDataTable");

                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> produtos = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(codigoSessao, cliente, unitOfWork);

                var lista = (
                    from p in produtos
                    where (p.SaldoQtde > 0 && !p.PalletFechado) || (p.SaldoPallet > 0 && p.PalletFechado)
                    select new
                    {
                        Codigo = p.CodigoPedidoProduto,
                        p.CodigoPedido,
                        p.CodigoPedidoProduto,
                        PalletFechado = (p.PalletFechado ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao),
                        p.Cliente,
                        p.Produto,
                        p.Categoria,
                        p.LinhaSeparacao,
                        Pedido = p.NumeroPedidoEmbarcador,
                        p.Qtde,
                        QtdeCarregar = p.SaldoQtde.ToString("n2"),
                        p.Peso,
                        PesoCarregar = p.SaldoPeso.ToString("n2"),
                        p.Pallet,
                        PalletCarregar = p.SaldoPallet.ToString("n2"),
                        p.Metro,
                        MetroCarregar = p.SaldoMetro.ToString("n2"),
                        DT_RowId = p.CodigoPedidoProduto
                    }).ToList();

                if (!basicDataTable)
                {
                    Models.Grid.Grid grid = this.ObterGridPedidoProduto();
                    grid.AdicionaRows(lista);
                    grid.setarQuantidadeTotal(lista?.Count() ?? 0);

                    return new JsonpResult(grid);
                }
                else
                    return new JsonpResult(lista);

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

        public async Task<IActionResult> AtualizarNotaFiscal(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                decimal pesoCubado = Request.GetDecimalParam("PesoCubado");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = await repositorioNotaFiscal.BuscarPorCodigoAsync(codigo, false);

                if (xmlNotaFiscal == null)
                    throw new ControllerException(Localization.Resources.Cargas.MontagemCargaMapa.NotaFiscalNaoEncontrada);

                xmlNotaFiscal.PesoCubado = pesoCubado;

                await repositorioNotaFiscal.AtualizarAsync(xmlNotaFiscal);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoAtualizarNotaFiscal);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigoPedidoProdutoCarregamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto rep = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto p = await rep.BuscarPorCodigoAsync(codigo, false);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto = await rep.BuscarPorPedidoProdutosAsync(new List<int>() { p.PedidoProduto.Codigo });

                var result = new
                {
                    CodigoProduto = p.PedidoProduto.Produto.CodigoProdutoEmbarcador,
                    Produto = p.PedidoProduto.Produto.Descricao,
                    CanalEntrega = p.CarregamentoPedido.Pedido?.CanalEntrega?.Descricao,
                    GrupoProduto = p.CarregamentoPedido.Pedido?.TipoOperacao?.Descricao,
                    LinhaSeparacao = p.PedidoProduto?.LinhaSeparacao?.Descricao,
                    PalletFechado = (p.PedidoProduto.PalletFechado ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao),
                    Peso = p.Peso.ToString("n2") + " / " + p.PedidoProduto.PesoTotal.ToString("n2"),
                    Qtde = p.Quantidade.ToString("n2") + " / " + p.PedidoProduto.Quantidade.ToString("n2"),
                    CxsPallet = p.PedidoProduto.QuantidadeCaixaPorPallet,
                    Pallet = p.QuantidadePallet + " / " + p.PedidoProduto.QuantidadePalet,
                    Cubagem = p.MetroCubico + " / " + p.PedidoProduto.MetroCubico,
                    Situacao = (p.Peso == p.PedidoProduto.PesoTotal ? Localization.Resources.Cargas.MontagemCargaMapa.Totalmente : Localization.Resources.Cargas.MontagemCargaMapa.Parcialmente) + " " + Localization.Resources.Cargas.MontagemCargaMapa.AtendidoNaCarga,
                    PadraoPalletizacao = "", //p.PedidoProduto.Produto.QtdPalet,
                    Embalagem = "", // p.PedidoProduto.Produto.em                    
                    Carregamentos = string.Join(", ", (from car in carregamentosPedidoProduto
                                                       select car.CarregamentoPedido.Carregamento.Descricao).Distinct().ToList())
                };
                return new JsonpResult(result);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarOsDetalhesDoPedido);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> EnviarEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repPedido.BuscarPorCodigoAsync(codigo, false);

                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiPossivelEncontrarRegistro);

                if (pedido.RestricoesDescarga.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.PedidoNaoPossuiRestricaoDeEntrega);

                string email = Request.GetStringParam("Email");
                List<string> emails = email.Split(';').ToList();

                string mensagem = "";
                bool todosSucesso = true;

                foreach (var restricao in pedido.RestricoesDescarga)
                {
                    bool sucesso = serPedido.EnviarRelatorioDetalhesPedidoPorEmail(emails, pedido, restricao, unitOfWork, out mensagem);

                    if (!sucesso)
                    {
                        todosSucesso = false;
                        Servicos.Log.TratarErro(mensagem, "EmailRestricao");
                    }
                }

                if (!todosSucesso)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoEnviarUmDosEmails);

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoEnviarOsDetalhesPorEmail);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

        }

        public async Task<IActionResult> BuscarSugestacaoPedidos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPedido = Request.GetIntParam("CodigoPedido");
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repositorioPedido.BuscarPorCodigoAsync(codigoPedido, false);

                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.PedidoInformadoInvalido);

                Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos repositorioRegrasAgrupamentoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos regrasAgrupamento = await repositorioRegrasAgrupamentoPedidos.BuscarRegraAsync(pedido.Filial?.Codigo ?? 0);

                if (regrasAgrupamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoExistemRegrasParaBuscaDePedidosAutomaticasConfiguradas);

                int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = await repModeloVeicularCarga.BuscarPorCodigoAsync(codigoModeloVeicular, false);

                if (modeloVeicularCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.ModeloVeicularInformadoInvalido);

                int codigoTipoCarga = Request.GetIntParam("TipoCarga");
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = await repositorioTipoCarga.BuscarPorCodigoAsync(codigoTipoCarga, false);
                int diasTolerancia = regrasAgrupamento.ToleranciaDiasDiferenca;
                DateTime dataCarregamento = pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value.Date : DateTime.Now.AddDays(-1).Date;
                DateTime datafim = DateTime.MinValue;

                if (diasTolerancia > 0)
                    datafim = dataCarregamento.AddDays(diasTolerancia);

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidosSugeridos = await repositorioPedido.BuscarSugestaoPedidoPorLocalidadeDestinoAsync(pedido.Filial?.Codigo ?? 0, pedido.TipoOperacao?.Codigo ?? 0, pedido.Empresa?.Codigo ?? 0, dataCarregamento, datafim);
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidosUtilizados = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>();
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidosDentroDoRaio = (from obj in pedidosSugeridos where obj.Destino.Codigo == pedido.Destino.Codigo select obj).ToList();

                if (regrasAgrupamento.RaioKMEntreCidades > 0)
                {
                    Repositorio.Embarcador.Logistica.Rota repositorioRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork, cancellationToken);
                    Servicos.Embarcador.Logistica.MapRequestApi servicoMapRequestApi = new Servicos.Embarcador.Logistica.MapRequestApi(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade> localidades = (from obj in pedidosSugeridos where obj.Destino.Codigo != pedido.Destino.Codigo select obj.Destino).Distinct().ToList();

                    for (int i = 0; i < localidades.Count; i++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade destino = localidades[i];
                        Dominio.Entidades.Embarcador.Logistica.Rota rota = await repositorioRota.BuscarRotaPorOrigemDestinoAsync(pedido.Destino.Codigo, destino.Codigo);

                        if (rota == null)
                        {
                            Dominio.Entidades.Localidade localidadeDestino = await repositorioLocalidade.BuscarPorCodigoAsync(destino.Codigo);
                            rota = servicoMapRequestApi.CriarRota(pedido.Destino, localidadeDestino, unitOfWork);

                            if (rota == null)
                                return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiPossivelEncontrarDistanciaEntre, pedido.Origem.Descricao, destino.Descricao));
                        }

                        if (rota.DistanciaKM <= regrasAgrupamento.RaioKMEntreCidades)
                            pedidosDentroDoRaio.AddRange((from obj in pedidosSugeridos where obj.Destino.Codigo == destino.Codigo select obj).ToList());
                    }

                    pedidosSugeridos = pedidosDentroDoRaio;
                }

                decimal pallets = pedido.NumeroPaletesFracionado + pedido.NumeroPaletes;
                decimal peso = pedido.PesoTotal;
                decimal ocupacaoCubicaPaletes = (tipoCarga?.Paletizado ?? false) ? modeloVeicularCarga.ObterOcupacaoCubicaPaletes() : 0m;
                decimal cubagem = pedido.CubagemTotal + ocupacaoCubicaPaletes;

                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido pedidoSugerido in pedidosSugeridos)
                {
                    if (regrasAgrupamento.NumeroMaximoEntregas > 0)
                    {
                        if (pedidosUtilizados.Count == regrasAgrupamento.NumeroMaximoEntregas)
                            break;
                    }

                    if (pedidoSugerido.Codigo != pedido.Codigo)
                    {
                        if (modeloVeicularCarga.VeiculoPaletizado)
                        {
                            if ((pedidoSugerido.NumeroPaletes + pedido.NumeroPaletesFracionado) + pallets > modeloVeicularCarga.NumeroPaletes)
                                continue;
                        }

                        if (modeloVeicularCarga.Cubagem > 0)
                        {
                            if (pedidoSugerido.CubagemTotal + cubagem > modeloVeicularCarga.Cubagem)
                                continue;
                        }

                        if (pedidoSugerido.PesoTotal + peso > modeloVeicularCarga.CapacidadePesoTransporte)
                            continue;

                        pallets += pedidoSugerido.NumeroPaletesFracionado + pedidoSugerido.NumeroPaletes;
                        peso += pedidoSugerido.PesoTotal;
                        cubagem += pedidoSugerido.CubagemTotal;

                        pedidosUtilizados.Add(pedidoSugerido);
                    }
                }

                List<int> codigosPedidos = (from obj in pedidosUtilizados select obj.Codigo).Distinct().ToList();
                List<int> pedidosColetaEntregaCarregamentoTipoOperacao = PedidosCarregamentoTipoOperacaoPedidoColetaEntrega(codigosPedidos, unitOfWork);
                List<string> filiais = await repositorioFilial.BuscarListaCNPJAtivasAsync();

                Repositorio.Embarcador.Filiais.FilialArmazem repFilialArmazem = new Repositorio.Embarcador.Filiais.FilialArmazem(unitOfWork, cancellationToken);
                List<int> filiaisComArmazem = await repFilialArmazem.BuscarFiliaisPorCNPJFiliaisAsync(filiais);

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> notasFiscaisPedidos = ObterNotasFiscaisPorPedidos(codigosPedidos, false, unitOfWork);

                return new JsonpResult((from obj in pedidosUtilizados select ObterDetalhesPedido(obj, false, pedidosColetaEntregaCarregamentoTipoOperacao.Contains(obj.Codigo), filiais, notasFiscaisPedidos, ConfiguracaoEmbarcador, unitOfWork, filiaisComArmazem.Contains(obj.Filial.Codigo))).ToList());

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarAsSugestoesDePedido);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterRecebedorRegiaoDestino()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("CodigoPedido");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repositorioPedido.BuscarPorCodigoAsync(codigoPedido, false);

                dynamic retorno = new
                {
                    Recebedor = "",
                    CodRecebedor = 0,
                    Status = false,
                    Erro = false
                };

                if (pedido?.Destino?.Regiao?.Recebedor != null)
                {
                    retorno = new
                    {
                        Recebedor = pedido.Destino.Regiao?.Recebedor?.Descricao ?? string.Empty,
                        CodRecebedor = pedido.Destino.Regiao?.Recebedor?.Codigo ?? 0,
                        Status = true,
                        Erro = false
                    };

                    return new JsonpResult(retorno);
                }

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                dynamic retorno = new
                {
                    Recebedor = "",
                    CodRecebedor = 0,
                    Status = false,
                    Erro = true
                };

                return new JsonpResult(retorno);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private enum IfSum
        {
            Igual,
            Diferente,
            Ambos
        }

        private dynamic ObterProdutosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return pedido.Produtos.Select(obj => new
            {
                Codigo = obj.Codigo,
                PesoTotal = obj.PesoUnitario * obj.Quantidade,
                CodigoProdutoEmbarcadorIntegracao = obj.Produto != null ? obj.Produto.CodigoProdutoEmbarcador : "",
                Descricao = obj.Produto != null ? obj.Produto.Descricao : string.Empty,
                Quantidade = obj.Quantidade.ToString("n3"),
                Peso = obj.PesoUnitario.ToString("n3"),
                QuantidadePalets = obj.QuantidadePalet.ToString("n3"),
                PalletFechado = obj.PalletFechado ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                MetrosCubico = obj.MetroCubico.ToString("n6"),
                obj.Observacao,
                obj.QuantidadeCaixaPorPallet,
                LinhaSeparacao = obj?.LinhaSeparacao?.Descricao ?? ""
            }).ToList();
        }

        private dynamic ObterDetalhesPedido(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido pedido, bool montagemCarregamentoPedidoProduto, bool pedidoColetaEntrega, List<string> filiais, List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> notasFiscaisPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool possuiFilialArmazem = false, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos = null, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simulacoesFretePedidos = null, Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendendicas = null, List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais> pedidoDetalhesAdicionais = null)
        {
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
            dynamic pedidoRetornar = serPedido.ObterDetalhesPedidoParaMontagemCarga(pedido, montagemCarregamentoPedidoProduto, filiais, notasFiscaisPedidos, configuracaoEmbarcador, unitOfWork, possuiFilialArmazem, carregamentosPedidos, simulacoesFretePedidos, configuracaoTempoTendendicas, pedidoDetalhesAdicionais);
            pedidoRetornar.PedidoColetaEntrega = pedidoColetaEntrega;
            pedidoRetornar.PedidoBloqueadoNaoLiberado = pedidoRetornar.PedidoBloqueado && !pedidoRetornar.LiberadoMontagemCarga;
            return pedidoRetornar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido ObterFiltrosPesquisa(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga, Repositorio.UnitOfWork unitOfWork)
        {
            return ObterFiltrosPesquisa(configuracaoMontagemCarga, unitOfWork, true);
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido ObterFiltrosPesquisa(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga, Repositorio.UnitOfWork unitOfWork, bool semCarga)
        {
            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoPedido = Request.GetIntParam("Pedidos");
            codigoPedido = codigoPedido > 0 ? codigoPedido : Request.GetIntParam("Pedido");
            List<int> codigosCanalEntregas = Request.GetListParam<int>("CanalEntrega");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            double codigoRemetente = Request.GetDoubleParam("Remetente");
            double codigoDestinatario = Request.GetDoubleParam("Destinatario");
            int codigoUsuario = Request.GetIntParam("CodigoUsuario");
            string estadoOrigem = Request.GetStringParam("EstadoOrigem");
            string estadoDestino = Request.GetStringParam("EstadoDestino");

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosFilialVenda = Request.GetListParam<int>("FilialVenda");
            List<int> codigosPedido = Request.GetListParam<int>("Pedidos");
            List<int> codigosCanalEntrega = Request.GetListParam<int>("CodigosCanalEntrega");
            List<int> codigosTipoDeCarga = Request.GetListParam<int>("CodigosTipoDeCarga");
            List<int> codigosRotaFrete = Request.GetListParam<int>("CodigosRotaFrete");
            List<int> codigosUsuario = Request.GetListParam<int>("CodigosUsuario");

            double tomador = Request.GetDoubleParam("Tomador");
            List<double> codigosTomadores = new List<double>();
            if (tomador > 0)
                codigosTomadores.Add(tomador);

            List<double> codigosRemetente = Request.GetListParam<double>("Remetente");
            List<double> codigosDestinatario = Request.GetListParam<double>("Destinatario");

            List<string> estadosOrigem = Request.GetListParam<string>("EstadosOrigem");
            List<string> estadosDestino = Request.GetListParam<string>("EstadosDestino");

            if ((codigoFilial > 0) && (codigosFilial.Count == 0))
                codigosFilial.Add(codigoFilial);

            if ((codigoPedido > 0) && (codigosPedido.Count == 0))
                codigosPedido.Add(codigoPedido);

            if (codigosCanalEntregas.Count > 0 && codigosCanalEntrega.Count == 0)
            {
                foreach (int codigoCanal in codigosCanalEntregas)
                    codigosCanalEntrega.Add(codigoCanal);
            }

            if ((codigoRemetente > 0) && (codigosRemetente.Count == 0))
                codigosRemetente.Add(codigoRemetente);

            if ((codigoDestinatario > 0) && (codigosDestinatario.Count == 0))
                codigosDestinatario.Add(codigoDestinatario);

            if ((codigoUsuario > 0) && (codigosUsuario.Count == 0))
                codigosUsuario.Add(codigoUsuario);

            if ((!string.IsNullOrEmpty(estadoOrigem) && estadoOrigem != "0") && (estadosOrigem.Count == 0))
                estadosOrigem.Add(estadoOrigem);

            if ((!string.IsNullOrEmpty(estadoDestino) && estadoDestino != "0") && (estadosDestino.Count == 0))
                estadosDestino.Add(estadoDestino);

            bool usarTipoTomadorPedido = Request.GetBoolParam("UsarTipoTomadorPedido");
            Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.NaoInformado;
            if (usarTipoTomadorPedido)
                tipoTomador = Request.GetEnumParam("TipoTomador", Dominio.Enumeradores.TipoTomador.NaoInformado);

            List<Dominio.Enumeradores.TipoTomador> tiposTomador = Request.GetListEnumParam<Dominio.Enumeradores.TipoTomador>("TiposTomador");

            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = null;
            if (codigosFilial.Count == 0 || codigosFilialVenda.Count == 0 || codigosTipoDeCarga.Count == 0)
                operadorLogistica = ObterOperadorLogistica(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
            {
                CodigoPedidoViagemNavio = Request.GetIntParam("PedidoViagemNavio"),
                CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(operadorLogistica) : codigosFilial,
                CodigosFilialVenda = codigosFilialVenda.Count == 0 ? ObterListaCodigoFilialVendaPermitidasOperadorLogistica(operadorLogistica) : codigosFilialVenda,
                CodigosPedido = codigosPedido,
                CodigosTipoCarga = codigosTipoDeCarga.Count == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(operadorLogistica) : codigosTipoDeCarga,
                Remetentes = codigosRemetente,
                Destinatarios = codigosDestinatario,
                DataInclusaoPCPInicial = Request.GetNullableDateTimeParam("DataInclusaoPCPInicial"),
                DataInclusaoPCPLimite = Request.GetNullableDateTimeParam("DataInclusaoPCPLimite"),
                DataInclusaoBookingInicial = Request.GetNullableDateTimeParam("DataInclusaoBookingInicial"),
                DataInclusaoBookingLimite = Request.GetNullableDateTimeParam("DataInclusaoBookingLimite"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                DeliveryTerm = Request.GetStringParam("DeliveryTerm"),
                CodigosDestino = Request.GetListParam<int>("Destino"),
                DisponivelColeta = Request.GetBoolParam("GerarCargasDeColeta"),
                //EstadoDestino = Request.GetStringParam("EstadoDestino"),
                //EstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                EstadosOrigem = estadosOrigem,
                EstadosDestino = estadosDestino,
                ExibirPedidosExpedidor = ConfiguracaoEmbarcador.NaoGerarCarregamentoRedespacho,
                FiltrarPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                GrupoPessoa = Request.GetIntParam("GrupoPessoaRemetente"),
                IdAutorizacao = Request.GetStringParam("IdAutorizacao"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroCarga = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroCarregamento = Request.GetStringParam("NumeroCarregamento"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroPedido = Request.GetIntParam("NumeroPedido"),
                NumeroPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador"),
                NumeroPedidoEmbarcadorDe = Request.GetStringParam("CodigoPedidoEmbarcadorDe"),
                NumeroPedidoEmbarcadorAte = Request.GetStringParam("CodigoPedidoEmbarcadorAte"),
                Ordem = Request.GetStringParam("Ordem"),
                OrdernarPorPrioridade = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS),
                Origem = Request.GetIntParam("Origem"),
                PaisDestino = Request.GetIntParam("PaisDestino"),
                PedidoSemCarregamento = ConfiguracaoEmbarcador.FiltrarPorPedidoSemCarregamentoNaMontagemCarga,
                PedidoSemCarga = semCarga,
                OcultarPedidosRetiradaProdutos = true,
                PedidosRedespacho = Request.GetBoolParam("GerarCargasDeRedespacho"),
                PedidosOrigemRecebedor = Request.GetBoolParam("PedidosOrigemRecebedor"),
                PedidosSelecionados = Request.GetListParam<int>("Selecionados"),
                PortoSaida = Request.GetStringParam("PortoSaida"),
                Expedidor = Request.GetDoubleParam("Expedidor"),
                Recebedor = Request.GetDoubleParam("Recebedor"),
                Rota = Request.GetIntParam("Rota"),
                Reserva = Request.GetStringParam("Reserva"),
                Situacao = SituacaoPedido.Aberto,
                SomenteComReserva = Request.GetBoolParam("SomenteComReserva"),
                TipoEmbarque = Request.GetStringParam("TipoEmbarque"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                Transbordo = false,
                CodigosCanalEntrega = codigosCanalEntrega,
                GrupoPessoaDestinatario = Request.GetIntParam("GrupoPessoaDestinatario"),
                CategoriaPessoa = Request.GetIntParam("CategoriaPessoa"),
                ProgramaComSessaoRoteirizador = Request.GetBoolParam("ComSessaoRoteirizador"),
                OpcaoSessaoRoteirizador = Request.GetEnumParam("OpcaoSessaoRoteirizador", OpcaoSessaoRoteirizador.NENHUM),
                CodigoSessaoRoteirizador = Request.GetIntParam("SessaoRoteirizador"),
                SituacaoSessaoRoteirizador = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador>("SituacaoSessaoRoteirizador", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Todas),
                TipoFiltroData = ConfiguracaoEmbarcador.TipoFiltroDataMontagemCarga,
                PesoDe = Request.GetDecimalParam("PesoDe"),
                PesoAte = Request.GetDecimalParam("PesoAte"),
                PalletDe = Request.GetDecimalParam("PalletDe"),
                PalletAte = Request.GetDecimalParam("PalletAte"),
                VolumeDe = Request.GetDecimalParam("VolumeDe"),
                VolumeAte = Request.GetDecimalParam("VolumeAte"),
                ValorDe = Request.GetDecimalParam("ValorDe"),
                ValorAte = Request.GetDecimalParam("ValorAte"),
                TipoCarga = codigoTipoCarga,
                TiposDeCargas = Request.GetListParam<int>("TipoDeCarga"),
                NaoRecebeCargaCompartilhada = Request.GetBoolParam("NaoRecebeCargaCompartilhada"),
                SomentePedidosComNota = Request.GetBoolParam("SomentePedidosComNota"),
                SomentePedidosSemNota = Request.GetBoolParam("SomentePedidosSemNota"),
                CodigosVeiculo = codigoVeiculo > 0 ? new List<int>() { codigoVeiculo } : null,
                DataCriacaoPedidoInicio = Request.GetNullableDateTimeParam("DataCriacaoPedidoInicio"),
                DataCriacaoPedidoLimite = Request.GetNullableDateTimeParam("DataCriacaoPedidoLimite"),
                Deposito = Request.GetIntParam("Deposito"),
                NumeroTransporte = Request.GetStringParam("NumeroTransporte"),
                CodigosLinhaSeparacao = Request.GetListParam<int>("CodigosLinhaSeparacao"),
                CodigosRestricoesEntrega = Request.GetListParam<int>("CodigosRestricoesEntrega"),
                CodigosProdutos = Request.GetListParam<int>("CodigosProdutos"),
                CodigosGrupoProdutos = Request.GetListParam<int>("CodigosGrupoProdutos"),
                CodigosCategoriaClientes = Request.GetListParam<int>("CodigosCategoriaClientes"),
                UsarTipoTomadorPedido = usarTipoTomadorPedido,
                TipoTomador = tipoTomador,
                TiposTomador = tiposTomador,
                CodigosRotaFrete = codigosRotaFrete,
                CodigosUsuario = codigosUsuario,
                UsuarioRemessa = Request.GetStringParam("UsuarioRemessa"),
                CodigosRegiaoDestino = Request.GetListParam<int>("RegiaoDestino"),
                CodigoProcessamentoEspecial = Request.GetIntParam("CodigoProcessamentoEspecial"),
                CodigoHorarioEntrega = Request.GetIntParam("CodigoHorarioEntrega"),
                RestricaoDiasEntrega = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>("RestricaoDiasEntrega"),
                NumeroCarregamentoPedido = Request.GetStringParam("NumeroCarregamentoPedido"),
                Tomadores = codigosTomadores,
                CodigoZonaTransporte = Request.GetIntParam("CodigoZonaTransporte"),
                CodigoDetalheEntrega = Request.GetIntParam("CodigoDetalheEntrega"),
                PedidoSemCargaPedido = ConfiguracaoEmbarcador.AtualizarProdutosCarregamentoPorNota, // Ppc, problema pedidos retornando disponível
                PedidosBloqueados = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao>("PedidosBloqueados", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Todos),
                PedidosRestricaoData = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao>("PedidosRestricaoData", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Todos),
                PedidosRestricaoPercentual = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao>("PedidosRestricaoPercentual", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Todos),
                DataAgendamentoInicial = Request.GetNullableDateTimeParam("DataAgendamentoInicial"),
                DataAgendamentoFinal = Request.GetNullableDateTimeParam("DataAgendamentoFinal"),
                PedidoParaReentrega = Request.GetBoolParam("PedidosParaReentrega"),
                PedidosSessao = Request.GetIntParam("PedidosSessao"),
                TipoControleSaldoPedido = configuracaoMontagemCarga.TipoControleSaldoPedido,
                ChaveNotaFiscalEletronica = configuracaoMontagemCarga.AtivarMontagemCargaPorNFe ? Request.GetStringParam("ChaveNotaFiscalEletronica") : string.Empty,
                CodigosVendedor = Request.GetListParam<int>("Vendedor"),
                CodigosSupervisor = Request.GetListParam<int>("Supervisor"),
                CodigosGerente = Request.GetListParam<int>("Gerente"),
                NotaFiscal = Request.GetListParam<int>("NotaFiscal"),
                BloquearSituacaoComercialPedido = true,
                Ordenacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrdenacaoFiltroPesquisaPedido>("Ordenacao", OrdenacaoFiltroPesquisaPedido.Padrao),
                HabilitarCadastroArmazem = ConfiguracaoGeral.HabilitarCadastroArmazem,
                CodigosAgrupadores = configuracaoMontagemCarga.ManterPedidosComMesmoAgrupadorNaMesmaCarga ? Request.GetListParam<string>("CodigosAgrupadores") : new List<string>(),
                PedidosSemSessao = Request.GetListParam<int>("PedidosSemSessao"),
                CodigosRegiao = Request.GetListParam<int>("Regiao"),
                CodigosMesorregiao = Request.GetListParam<int>("Mesorregiao"),
                TendenciaEntrega = Request.GetEnumParam<TendenciaEntrega>("TendenciaEntrega"),
                FiltrarPedidosVinculadoOutrasCarga = configuracaoMontagemCarga.FiltrarPedidosVinculadoOutrasCarga,
            };

            List<int> codigosTipoOperacao = Request.GetListParam<int>("TipoOperacao");
            int codigoEmpresa = Request.GetIntParam("Empresa");

            filtrosPesquisa.CodigosTipoOperacao = codigosTipoOperacao.Count > 0 ? codigosTipoOperacao : ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);

            List<int> codigosEmpresa = ObterListaCodigoTransportadorPermitidosOperadorLogistica(unitOfWork);
            if (codigosEmpresa.Count > 0)
                filtrosPesquisa.CodigosTransportador = codigoEmpresa > 0 ? new List<int>() { codigoEmpresa } : codigosEmpresa;
            else
                filtrosPesquisa.Transportador = codigoEmpresa;

            if (filtrosPesquisa.Recebedor == 0)
                filtrosPesquisa.ListaRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);

            if (filtrosPesquisa.Expedidor == 0)
                filtrosPesquisa.ListaExpedidor = ObterListaCnpjCpfExpedidorPermitidosOperadorLogistica(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (Empresa.Matriz.Any())
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Matriz.FirstOrDefault().Codigo;
                else
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Codigo;
            }

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && configuracaoMontagemCarga.FiltrarPedidosOndeRecebedorTransportadorNoPortalDoTransportador)
            {
                filtrosPesquisa.Recebedor = Usuario.Empresa.CNPJ.ToDouble();
                filtrosPesquisa.FiltrarPedidosOndeRecebedorTransportadorNoPortalDoTransportador = configuracaoMontagemCarga.FiltrarPedidosOndeRecebedorTransportadorNoPortalDoTransportador;
            }

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPedidoProduto()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoPedido", false);
            grid.AdicionarCabecalho("CodigoPedidoProduto", false);
            grid.AdicionarCabecalho("PalletFechado", false);
            grid.AdicionarCabecalho("Cliente", false); //, "Cliente", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Produto", "Produto", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Categoria", "Categoria", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("LinhaSeparacao", "LinhaSeparacao", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Pedido", "Pedido", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Qtde", false);
            grid.AdicionarCabecalho("Qtde", "QtdeCarregar", 10, Models.Grid.Align.center, false).Editable(new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9));
            grid.AdicionarCabecalho("Peso", false);
            grid.AdicionarCabecalho("Peso", "PesoCarregar", 10, Models.Grid.Align.center, false); //.Editable(new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9));
            grid.AdicionarCabecalho("Pallet", false);
            grid.AdicionarCabecalho("Pallet", "PalletCarregar", 10, Models.Grid.Align.center, false).Editable(new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9));
            grid.AdicionarCabecalho("Metro", false);
            grid.AdicionarCabecalho("Metro", "MetroCarregar", 10, Models.Grid.Align.center, false);
            //grid.AdicionarCabecalho("Saldo", false);
            return grid;
        }

        private List<int> PedidosCarregamentoTipoOperacaoPedidoColetaEntrega(List<int> codigosPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            List<int> pedidosColetaEntregaCarregamentoTipoOperacao = new List<int>();
            if (codigosPedidos?.Count > 0)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                pedidosColetaEntregaCarregamentoTipoOperacao = carregamentoCarregamentoPedido.PedidosCarregamentoTipoOperacaoPedidoColetaEntrega(codigosPedidos);
            }
            return pedidosColetaEntregaCarregamentoTipoOperacao;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> ObterNotasFiscaisPorPedidos(List<int> codigosPedidos, bool considerarApenasVinculoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> listaNotas = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal>();
            if (codigosPedidos?.Count > 0)
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                listaNotas = repPedido.BuscarNumeroNotasFiscaisPorPedidos(codigosPedidos, considerarApenasVinculoPedido);
            }

            return listaNotas;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais> ObterDetalhesAdicionaisPedido(List<int> codigosPedido, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais> pedidoDetalhesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais>();

            if (codigosPedido?.Count > 0)
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                List<(int CodigoPedido, DateTime? DataDigitalizacaoCanhoto, DateTime? DataEntregaNotaCanhoto)> pedidosCanhoto = repositorioPedido.BuscarDataDigitalizacaoCanhotoAvulsoPorPedidos(codigosPedido);
                List<(int CodigoPedido, string CodigoCargaEmbarcador)> pedidosCarga = repositorioPedido.BuscarNumeroPrimieraCargaPorPedidos(codigosPedido);

                foreach (int codigoPedido in codigosPedido)
                {
                    (int CodigoPedido, DateTime? DataDigitalizacaoCanhoto, DateTime? DataEntregaNotaCanhoto) pedidoCanhoto = pedidosCanhoto.FirstOrDefault(x => x.CodigoPedido == codigoPedido);
                    (int CodigoPedido, string CodigoCargaEmbarcador) pedidoCarga = pedidosCarga.FirstOrDefault(x => x.CodigoPedido == codigoPedido);

                    pedidoDetalhesAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais()
                    {
                        CodigoPedido = codigoPedido,
                        DataDigitalizacaoCanhoto = pedidoCanhoto.DataDigitalizacaoCanhoto,
                        DataEntregaNotaCanhoto = pedidoCanhoto.DataEntregaNotaCanhoto,
                        CodigoCargaEmbarcador = pedidoCarga.CodigoCargaEmbarcador
                    });
                }
            }

            return pedidoDetalhesAdicionais;
        }

        #endregion Métodos Privados
    }
}
