using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Cargas/AutorizacaoCarregamento")]
    public class AutorizacaoCarregamentoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento,
        Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento,
        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao
    >
    {
        #region Construtores

        public AutorizacaoCarregamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ReprocessarCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                int codigoCarregamentoSolicitacao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao = repositorioCarregamentoSolicitacao.BuscarPorCodigo(codigoCarregamentoSolicitacao, auditavel: false);

                if (carregamentoSolicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (carregamentoSolicitacao.Situacao != SituacaoCarregamentoSolicitacao.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Servicos.Embarcador.Carga.MontagemCarga.CarregamentoAprovacao servicoCarregamentoAprovacao = new Servicos.Embarcador.Carga.MontagemCarga.CarregamentoAprovacao(unitOfWork);

                servicoCarregamentoAprovacao.CriarAprovacao(carregamentoSolicitacao, TipoServicoMultisoftware);
                repositorioCarregamento.Atualizar(carregamentoSolicitacao.Carregamento);
                repositorioCarregamentoSolicitacao.Atualizar(carregamentoSolicitacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = carregamentoSolicitacao.Situacao != SituacaoCarregamentoSolicitacao.SemRegraAprovacao });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar o carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplosCarregamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                List<int> codigosCarregamentoSolicitacao = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao> listaCarregamentoSolicitacao = repositorioCarregamentoSolicitacao.BuscarSemRegraAprovacaoPorCodigos(codigosCarregamentoSolicitacao);
                Servicos.Embarcador.Carga.MontagemCarga.CarregamentoAprovacao servicoCarregamentoAprovacao = new Servicos.Embarcador.Carga.MontagemCarga.CarregamentoAprovacao(unitOfWork);
                int totalRegrasReprocessadas = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao in listaCarregamentoSolicitacao)
                {
                    servicoCarregamentoAprovacao.CriarAprovacao(carregamentoSolicitacao, TipoServicoMultisoftware);

                    if (carregamentoSolicitacao.Situacao != SituacaoCarregamentoSolicitacao.SemRegraAprovacao)
                    {
                        repositorioCarregamento.Atualizar(carregamentoSolicitacao.Carregamento);
                        repositorioCarregamentoSolicitacao.Atualizar(carregamentoSolicitacao);
                        totalRegrasReprocessadas++;
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegrasReprocessadas = totalRegrasReprocessadas });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar os carregamentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao = repositorioCarregamentoSolicitacao.BuscarPorCodigo(codigo, auditavel: false);

                if (carregamentoSolicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = carregamentoSolicitacao.Carregamento;
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                bool exibirHoraDataCarregamentoEDescarregamento = configuracaoEmbarcador.InformaApoliceSeguroMontagemCarga || configuracaoEmbarcador.InformaHorarioCarregamentoMontagemCarga;
                Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoDadosPesagem dadosPesagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).ObterDadosPesagem(carregamentoSolicitacao.Carregamento);

                return new JsonpResult(new
                {
                    carregamentoSolicitacao.Codigo,
                    carregamentoSolicitacao.Situacao,
                    carregamento.NumeroCarregamento,
                    DataCarregamento = carregamento.DataCarregamentoCarga?.ToString($"dd/MM/yyyy{(exibirHoraDataCarregamentoEDescarregamento ? " HH:mm" : "")}") ?? "",
                    ModeloVeicularCarga = carregamento.ModeloVeicularCarga?.Descricao ?? "",
                    TipoCarga = carregamento.TipoDeCarga?.Descricao ?? "",
                    Cubagem = dadosPesagem.Cubagem.ToString("n2"),
                    Pallets = dadosPesagem.Pallet.ToString("n2"),
                    Peso = dadosPesagem.Peso.ToString("n4"),
                    CapacidadeCubagem = dadosPesagem.CapacidadeCubagem.ToString("n2"),
                    CapacidadePallets = dadosPesagem.CapacidadePallet.ToString("n2"),
                    CapacidadePeso = dadosPesagem.CapacidadePeso.ToString("n4"),
                    CorCubagem = dadosPesagem.SituacaoCubagem.ObterCorLinha(),
                    CorPallets = dadosPesagem.SituacaoPallet.ObterCorLinha(),
                    CorPeso = dadosPesagem.SituacaoPeso.ObterCorLinha(),
                    LotacaoCubagem = dadosPesagem.PercentualOcupacaoCubagem.ToString("n2"),
                    LotacaoPallets = dadosPesagem.PercentualOcupacaoPallet.ToString("n2"),
                    LotacaoPeso = dadosPesagem.PercentualOcupacaoPeso.ToString("n4"),
                    dadosPesagem.PossuiCubagem,
                    dadosPesagem.PossuiPallet
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamentoAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamentoAprovacao()
            {
                CodigosFilial = Request.GetListParam<int>("Filial"),
                CodigosModeloVeicularCarga = Request.GetListParam<int>("ModeloVeicularCarga"),
                CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                NumeroCarregamento = Request.GetStringParam("NumeroCarregamento"),
                SituacaoCarregamentoSolicitacao = Request.GetNullableEnumParam<SituacaoCarregamentoSolicitacao>("Situacao")
            };
        }

        private string ObterFilialCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial> carregamentoFiliais)
        {
            if (carregamentoSolicitacao.Carregamento.Filial != null)
                return carregamentoSolicitacao.Carregamento.Filial.Descricao;

            return carregamentoFiliais.Find(o => o.Carregamento.Codigo == carregamentoSolicitacao.Carregamento.Codigo)?.Filial?.Descricao ?? "";
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataCarregamentoCarga")
                return "Carregamento.DataCarregamentoCarga";

            if (propriedadeOrdenar == "NumeroCarregamento")
                return "Carregamento.NumeroCarregamento";

            if (propriedadeOrdenar == "ModeloVeicularCarga")
                return "Carregamento.ModeloVeicularCarga.Descricao";

            if (propriedadeOrdenar == "TipoCarga")
                return "Carregamento.TipoDeCarga.Descricao";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao origem)
        {
            return origem.Situacao == SituacaoCarregamentoSolicitacao.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao> carregamentosSolicitacao;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamentoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento repositorioAprovacaoAlcadaCarregamento = new Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento(unitOfWork);

                carregamentosSolicitacao = repositorioAprovacaoAlcadaCarregamento.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    carregamentosSolicitacao.Remove(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                carregamentosSolicitacao = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    carregamentosSolicitacao.Add(repositorioCarregamentoSolicitacao.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return (from o in carregamentosSolicitacao select o.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número da Solicitação", propriedade: "Numero", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Número do Carregamento", propriedade: "NumeroCarregamento", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data do Carregamento", propriedade: "DataCarregamentoCarga", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tipo de Carga", propriedade: "TipoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Modelo Veicular de Carga", propriedade: "ModeloVeicularCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "% de Ocupação", propriedade: "Ocupacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Expedidor", propriedade: "Expedidor", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Capacidade do Veículo", propriedade: "CapacidadeVeiculo", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Peso dos Pedidos (soma)", propriedade: "PesoPedidos", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Origem", propriedade: "Origem", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Destino", propriedade: "Destino", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                bool exibirHoraDataCarregamentoEDescarregamento = configuracaoEmbarcador.InformaApoliceSeguroMontagemCarga || configuracaoEmbarcador.InformaHorarioCarregamentoMontagemCarga;
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamentoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento repositorio = new Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao> carregamentosSolicitacao = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao>();
                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga montagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial repositorioCarregamentoFiliais = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial> carregamentoFiliais = repositorioCarregamentoFiliais.BuscarPorCarregamentos(carregamentosSolicitacao.Select(o => o.Carregamento.Codigo).ToList());

                dynamic lista = new List<dynamic>() { };

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao in carregamentosSolicitacao)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoDadosPesagem dadosPesagem = montagemCarga.ObterDadosPesagem(carregamentoSolicitacao.Carregamento);

                    lista.Add(new
                    {
                        carregamentoSolicitacao.Codigo,
                        carregamentoSolicitacao.Numero,
                        carregamentoSolicitacao.Carregamento.NumeroCarregamento,
                        DataCarregamentoCarga = carregamentoSolicitacao.Carregamento.DataCarregamentoCarga?.ToString($"dd/MM/yyyy{(exibirHoraDataCarregamentoEDescarregamento ? " HH:mm" : "")}"),
                        ModeloVeicularCarga = carregamentoSolicitacao.Carregamento.ModeloVeicularCarga?.Descricao,
                        TipoCarga = carregamentoSolicitacao.Carregamento.TipoDeCarga?.Descricao,
                        Situacao = carregamentoSolicitacao.Situacao.ObterDescricao(),
                        Filial = ObterFilialCarregamento(carregamentoSolicitacao, carregamentoFiliais),
                        Ocupacao = dadosPesagem?.PercentualOcupacaoPeso.ToString("n4"),
                        Expedidor = carregamentoSolicitacao.Carregamento.Expedidor?.Descricao ?? "",
                        CapacidadeVeiculo = dadosPesagem?.CapacidadePeso.ToString("n4"),
                        PesoPedidos = carregamentoSolicitacao.Carregamento.Pedidos?.Select(pedido => pedido.Pedido.PesoTotal).Sum().ToString("n4"),
                        Origem = string.Join(", ", carregamentoSolicitacao.Carregamento.Pedidos.Select(pedido => pedido.Pedido.Remetente?.Localidade?.DescricaoCidadeEstado)),
                        Destino = string.Join(", ", carregamentoSolicitacao.Carregamento.Pedidos.Select(pedido => pedido.Pedido.Destino?.DescricaoCidadeEstado))
                    });
                }

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao != SituacaoCarregamentoSolicitacao.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.MontagemCarga.CarregamentoAprovacao servicoCarregamentoAprovacao = new Servicos.Embarcador.Carga.MontagemCarga.CarregamentoAprovacao(unitOfWork);
            Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork, configuracaoEmbarcador);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (!servicoCarregamentoAprovacao.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                origem.Situacao = SituacaoCarregamentoSolicitacao.Aprovada;
                origem.Carregamento.SituacaoCarregamento = SituacaoCarregamento.EmMontagem;

                repositorioCarregamento.Atualizar(origem.Carregamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem.Carregamento, (configuracaoEmbarcador.OcultaGerarCarregamentosMontagemCarga ? "Agendamento de carga aprovado" : "Geração de carga aprovada"), unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
                {
                    MontagemCargaPedidoProduto = Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto.Validar,
                    Usuario = Usuario
                };

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(origem.Carregamento.Codigo);
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = (from obj in carregamentoPedidos select obj.Pedido.Filial).Distinct().ToList();

                servicoMontagemCarga.GerarCarga(origem.Carregamento, filiais, carregamentoPedidos, TipoServicoMultisoftware, Cliente, Auditado, propriedades, ClienteAcesso.URLAcesso);
            }
            else
            {
                origem.Situacao = SituacaoCarregamentoSolicitacao.Reprovada;
                origem.Carregamento.SituacaoCarregamento = SituacaoCarregamento.SolicitacaoReprovada;

                repositorioCarregamento.Atualizar(origem.Carregamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem.Carregamento, (configuracaoEmbarcador.OcultaGerarCarregamentosMontagemCarga ? "Agendamento de carga reprovado" : "Geração de carga reprovada"), unitOfWork);
            }

            repositorioCarregamentoSolicitacao.Atualizar(origem);
        }

        #endregion
    }
}