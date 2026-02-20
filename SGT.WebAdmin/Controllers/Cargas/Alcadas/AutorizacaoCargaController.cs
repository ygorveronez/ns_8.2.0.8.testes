using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Cargas/AutorizacaoCarga")]
    public class AutorizacaoCargaController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga,
        Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga,
        Dominio.Entidades.Embarcador.Cargas.Carga
    >
    {
        #region Construtores

        public AutorizacaoCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repositorio = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorio.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                bool exibirDetalhesFreteCarga = false;
                string informacaoTipoFreteEscolhido = "";
                string valorFrete = carga.ValorFreteAPagar.ToString("n2");

                if (carga.ValorFreteAPagar > 0)
                {
                    exibirDetalhesFreteCarga = true;

                    if ((TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && ((carga.SituacaoCarga == SituacaoCarga.Nova) || (carga.SituacaoCarga == SituacaoCarga.AgNFe)))
                        informacaoTipoFreteEscolhido = "(Prévia do Frete)";
                    else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                        informacaoTipoFreteEscolhido = "(Valor informado pelo Operador)";
                    else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador)
                        informacaoTipoFreteEscolhido = "(Valor informado pelo embarcador)";
                }
                else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Cliente)
                {
                    informacaoTipoFreteEscolhido = "Frete por Conta do Cliente";
                    valorFrete = "";
                }
                else
                    valorFrete = "Pendente";

                return new JsonpResult(new
                {
                    carga.Codigo,
                    carga.CodigoCargaEmbarcador,
                    ExibirDetalhesFreteCarga = exibirDetalhesFreteCarga,
                    Filial = carga.Filial?.Descricao,
                    InformacaoTipoFreteEscolhido = informacaoTipoFreteEscolhido,
                    carga.TipoFreteEscolhido,
                    ModeloVeicularCarga = carga.ModeloVeicularCarga?.Descricao,
                    Motoristas = carga.NomeMotoristas,
                    Operador = carga.Operador?.Descricao,
                    Peso = carga.DadosSumarizados?.PesoTotal.ToString("n2") ?? "",
                    Placas = carga.PlacasVeiculos,
                    Rota = carga.Rota?.Descricao,
                    Situacao = carga.SituacaoCarga.ObterDescricao(),
                    carga.SituacaoAlteracaoFreteCarga,
                    carga.SituacaoAutorizacaoIntegracaoCTe,
                    TipoCarga = carga.TipoDeCarga?.Descricao,
                    TipoOperacao = carga.TipoOperacao?.Descricao,
                    Transportador = carga.Empresa?.Descricao,
                    ValorFrete = valorFrete,
                    MotivoSolicitacaoFrete = carga.MotivoSolicitacaoFrete?.Descricao ?? string.Empty,
                    PortoOrigem = carga.PortoOrigem?.Descricao,
                    PortoDestino = carga.PortoDestino?.Descricao,
                    Tomador = carga.DadosSumarizados?.Tomadores ?? "",
                    SolicitacaoFrete = new
                    {
                        Motivo = carga.MotivoSolicitacaoFrete?.Descricao ?? string.Empty,
                        Observacao = carga.ObservacaoSolicitacaoFrete ?? string.Empty
                    }
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

        public async Task<IActionResult> SalvarCustoExra()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int justificativaCustoExtra = Request.GetIntParam("JustificativaCustoExtra");
                int setorResponsavel = Request.GetIntParam("SetorResponsavel");
                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga repJustificativa = new Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Setor setor = repSetor.BuscarPorCodigo(setorResponsavel);
                Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga justificativa = repJustificativa.BuscarPorCodigo(justificativaCustoExtra);

                unitOfWork.Start();

                carga.JustificativaAutorizacaoCarga = justificativa;
                carga.Setor = setor;
                carga.ObservacaoJustificativaAprovacaoCarga = observacao;
                carga.UsuarioAutorizouAlteracaoFrete = Usuario;

                repCarga.Atualizar(carga);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarCustoExraMultiplasCargas()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigosCargas = ObterCodigosOrigensSelecionadas(unitOfWork);

                int justificativaCustoExtra = Request.GetIntParam("JustificativaCustoExtra");
                int setorResponsavel = Request.GetIntParam("SetorResponsavel");
                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga repJustificativa = new Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarCargasPorCodigos(codigosCargas);

                if (cargas == null && cargas.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar registros.");

                Dominio.Entidades.Setor setor = repSetor.BuscarPorCodigo(setorResponsavel);
                Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga justificativa = repJustificativa.BuscarPorCodigo(justificativaCustoExtra);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    carga.JustificativaAutorizacaoCarga = justificativa;
                    carga.Setor = setor;
                    carga.ObservacaoJustificativaAprovacaoCarga = observacao;
                    repCarga.Atualizar(carga);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaAprovacao ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaAprovacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaAprovacao()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigosMotivoSolicitacaoFrete = Request.GetListParam<int>("MotivoSolicitacaoFrete"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                SituacaoAlteracaoFrete = Request.GetNullableEnumParam<SituacaoAlteracaoFreteCarga>("SituacaoAlteracaoFrete"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem")
            };

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosTipoOperacao = Request.GetListParam<int>("TipoOperacao");

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoOperacao = codigosTipoOperacao.Count == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : codigosTipoOperacao;

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (propriedadeOrdenar == "ModeloVeicular")
                return "ModeloVeicularCarga.Descricao";

            if (propriedadeOrdenar == "TipoCarga")
                return "TipoDeCarga.Descricao";

            if (propriedadeOrdenar == "PortoOrigem")
                return "PortoOrigem.Descricao";

            if (propriedadeOrdenar == "PortoDestino")
                return "PortoDestino.Descricao";

            if (propriedadeOrdenar == "Transportador")
                return "Empresa.RazaoSocial";

            return propriedadeOrdenar;
        }

        private dynamic ObterRetornoGrid(Dominio.Entidades.Embarcador.Cargas.Carga carga, Servicos.Embarcador.Carga.Frete serFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> listaCargasComponentesFrete)
        {
            (decimal Percentual, decimal ValorTabela, decimal PercentualInverso) calculo = serFrete.CalcularPercentualFrete(carga, ConfiguracaoEmbarcador, TipoServicoMultisoftware);
            decimal percentual = calculo.Percentual;
            decimal valorTabela = calculo.ValorTabela;
            decimal percentualInverso = calculo.PercentualInverso;
            decimal percentualFreteSobreNotaPedido = serFrete.CalcularPercentualFreteSobreNotaPedido(carga);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = listaCargasComponentesFrete.FindAll(o => o.Carga.Codigo == carga.Codigo);

            return new
            {
                carga.Codigo,
                carga.CodigoCargaEmbarcador,
                Filial = carga.Filial?.Descricao,
                ModeloVeicular = carga.ModeloVeicularCarga?.Descricao,
                Motorista = carga.NomeMotoristas,
                NumeroCTes = carga.NumerosCTes,
                SituacaoAlteracaoFreteCarga = carga.SituacaoAlteracaoFreteCarga.ObterDescricao(),
                Situacao = carga.SituacaoCarga.ObterDescricao(),
                TipoCarga = carga.TipoDeCarga?.Descricao,
                Transportador = carga.Empresa?.RazaoSocial,
                Veiculo = carga.PlacasVeiculos,
                Operador = carga.Operador?.Descricao ?? "",
                JustificativaAutorizacaoCarga = carga.JustificativaAutorizacaoCarga?.Descricao ?? "",
                PortoOrigem = carga.PortoOrigem?.Descricao ?? "",
                PortoDestino = carga.PortoDestino?.Descricao ?? "",
                carga.TiposTomador,
                Remetentes = carga.DadosSumarizados?.Remetentes ?? "",
                Destinatarios = carga.DadosSumarizados?.Destinatarios ?? "",
                carga.Containeres,
                DataCriacaoCarga = carga.DataCriacaoCarga.ToString("dd/MM/yyyy"),
                carga.ModaisCarga,
                ValorFrete = carga.ValorFrete.ToString("n2"),
                ValorTabela = valorTabela,
                Percentual = percentual,
                PercentualInverso = percentualInverso,
                PercentualFreteSobreNotaPedido = percentualFreteSobreNotaPedido,
                DescricaoComponente = string.Join(", ", cargaComponentesFrete.Select(o => o.ComponenteFrete?.Descricao))
            };
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Cargas.Carga origem)
        {
            return origem.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaAprovacao filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorioAprovacaoAlcadaCarga = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(unitOfWork);

                cargas = repositorioAprovacaoAlcadaCarga.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    cargas.Remove(new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    cargas.Add(repositorioCarga.BuscarPorCodigo((int)itemSelecionado.Codigo));
            }

            return (from carga in cargas select carga.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                if (!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                    grid.AdicionarCabecalho(descricao: "Nº da Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Tipo de Carga", propriedade: "TipoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Modelo Veícular", propriedade: "ModeloVeicular", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Operador", propriedade: "Operador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Justificativa Custo Extra", propriedade: "JustificativaAutorizacaoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        grid.AdicionarCabecalho(descricao: "CT-e", propriedade: "NumeroCTes", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                        grid.AdicionarCabecalho(descricao: "Empresa/Filial", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    }
                    else
                    {
                        grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                        grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    }

                    grid.AdicionarCabecalho(descricao: "Veículo", propriedade: "Veiculo", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Motorista", propriedade: "Motorista", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Situação Alteração Frete", propriedade: "SituacaoAlteracaoFreteCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Valor Calculado pela Tabela", propriedade: "ValorTabela", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Percentual em relação a tabela", propriedade: "Percentual", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Percentual em relação ao valor do frete", propriedade: "PercentualInverso", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                    if (ConfiguracaoEmbarcador.ObrigarMotivoSolicitacaoFrete)
                        grid.AdicionarCabecalho(descricao: "Valor do Frete", propriedade: "ValorFrete", tamanho: 10, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false);
                    else
                        grid.AdicionarCabecalho(descricao: "Valor do Frete", propriedade: "ValorFrete", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                    grid.AdicionarCabecalho(descricao: "Descrição Componente", propriedade: "DescricaoComponente", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Percentual frete sobre nota/pedido", propriedade: "PercentualFreteSobreNotaPedido", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                }
                else
                {
                    grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                    grid.AdicionarCabecalho(descricao: "Nº da Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(propriedade: "TipoCarga", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "ModeloVeicular", visivel: false);
                    grid.AdicionarCabecalho(descricao: "Operador", propriedade: "Operador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Justificativa Custo Extra", propriedade: "JustificativaAutorizacaoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(propriedade: "NumeroCTes", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "Transportador", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "Filial", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "Veiculo", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "Motorista", visivel: false);
                    grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(propriedade: "SituacaoAlteracaoFreteCarga", visivel: false);
                    grid.AdicionarCabecalho(descricao: "Porto de Origem", propriedade: "PortoOrigem", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Porto de Destino", propriedade: "PortoDestino", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Tipo Tomador", propriedade: "TiposTomador", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Remetente(s)", propriedade: "Remetentes", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Destinatário(s)", propriedade: "Destinatarios", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Container(es)", propriedade: "Containeres", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Data Carga", propriedade: "DataCriacaoCarga", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Modal da Operação", propriedade: "ModaisCarga", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                }

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "AutorizacaoCarga/Pesquisa", "grid-pesquisa-autorizacao-carga");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorio = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repositorioCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);

                Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaAprovacao filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> listaCargaComponentesFretes = repositorioCargaComponentesFrete.BuscarTodosPorCargas(cargas.Select(o => o.Codigo).ToList());


                var lista = (
                    from carga in cargas
                    select ObterRetornoGrid(carga, serFrete, listaCargaComponentesFretes)
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Cargas.Carga origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.SituacaoCarga == SituacaoCarga.Cancelada || origem.SituacaoCarga == SituacaoCarga.Anulada)
                return;

            if (origem.SituacaoAlteracaoFreteCarga != SituacaoAlteracaoFreteCarga.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork);
            Servicos.Embarcador.Frete.ContratoFreteCliente.ContratoFreteCliente servicoContratoFreteCliente = new Servicos.Embarcador.Frete.ContratoFreteCliente.ContratoFreteCliente(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (!servicoCargaAprovacaoFrete.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                bool existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao = repositorioConfiguracaoGeralCarga.ExisteConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao() && (origem.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? false);

                if (!origem.ExigeNotaFiscalParaCalcularFrete && !existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao)
                    origem.SituacaoCarga = SituacaoCarga.AgTransportador;

                origem.SituacaoAlteracaoFreteCarga = SituacaoAlteracaoFreteCarga.Aprovada;

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargaJanelaCarregamentoTransportadors = repCargaJanelaCarregamentoTransportador.BuscarCargasComTabelaPorCarga(origem.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamento in cargaJanelaCarregamentoTransportadors)
                {
                    cargaJanelaCarregamento.PossuiFreteCalculado = false;
                    cargaJanelaCarregamento.FreteCalculadoComProblemas = false;
                    repCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamento);
                }

                if (origem.TipoOperacao != null && origem.TipoOperacao.EmiteCTeFilialEmissora && origem.Filial != null && origem.Filial.EmpresaEmissora != null)
                {
                    Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref origem, false, TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador);
                }

                servicoContratoFreteCliente.ConsultarSaldoEConsome(origem, true);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Alteração do valor de frete aprovada", unitOfWork);
            }
            else
            {
                origem.SituacaoAlteracaoFreteCarga = SituacaoAlteracaoFreteCarga.Reprovada;
                origem.CalculandoFrete = true;
                origem.DataInicioCalculoFrete = DateTime.Now;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Alteração do valor de frete reprovada", unitOfWork);
            }

            repositorioCarga.Atualizar(origem);
            servicoCargaAprovacaoFrete.NotificarSituacaoAprovacaoAoOperadorCarga(origem, TipoServicoMultisoftware);
        }

        protected override void PreencherDadosRejeicaoAprovacao(Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga aprovacao, Repositorio.UnitOfWork unitOfWork)
        {
            base.PreencherDadosRejeicaoAprovacao(aprovacao, unitOfWork);

            Servicos.Embarcador.Frete.ContratoFreteCliente.ContratoFreteCliente servicoContratoFreteCliente = new Servicos.Embarcador.Frete.ContratoFreteCliente.ContratoFreteCliente(unitOfWork);

            servicoContratoFreteCliente.InverteTomador(aprovacao.OrigemAprovacao);
        }

        #endregion
    }
}