using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/AutorizacaoPesagem")]
    public class AutorizacaoPesagemController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem,
        Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem,
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita
    >
    {
		#region Construtores

		public AutorizacaoPesagemController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Reprocessar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                int codigoGuarita = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorCodigo(codigoGuarita, auditavel: false);

                if (guarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (guarita.SituacaoPesagemCarga != SituacaoPesagemCarga.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();

                Servicos.Embarcador.Carga.CargaAprovacaoPesagem servicoCargaAprovacaoPesagem = new Servicos.Embarcador.Carga.CargaAprovacaoPesagem(unitOfWork);

                servicoCargaAprovacaoPesagem.CriarAprovacao(guarita, TipoServicoMultisoftware);
                repositorioGuarita.Atualizar(guarita);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = guarita.SituacaoPesagemCarga != SituacaoPesagemCarga.SemRegraAprovacao });
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
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                List<int> codigosGuarita = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> listaGuarita = repositorioGuarita.BuscarSemRegraAprovacaoPorCodigos(codigosGuarita);
                Servicos.Embarcador.Carga.CargaAprovacaoPesagem servicoCargaAprovacaoPesagem = new Servicos.Embarcador.Carga.CargaAprovacaoPesagem(unitOfWork);
                int totalRegrasReprocessadas = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita in listaGuarita)
                {
                    servicoCargaAprovacaoPesagem.CriarAprovacao(guarita, TipoServicoMultisoftware);

                    if (guarita.SituacaoPesagemCarga != SituacaoPesagemCarga.SemRegraAprovacao)
                    {
                        repositorioGuarita.Atualizar(guarita);
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
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as cargas.");
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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorio.BuscarPorCodigo(codigo);

                if (guarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                decimal pesoCarga = (guarita.Carga.DadosSumarizados?.PesoTotal ?? 0m);
                decimal pesoBruto = (guarita.PesagemInicial + pesoCarga);
                decimal diferencaPeso = guarita.PesagemFinal - pesoBruto;
                decimal percentualDiferencaPeso = (pesoCarga > 0m) ? (diferencaPeso / pesoCarga * 100) : 0m;

                return new JsonpResult(new
                {
                    guarita.Codigo,
                    guarita.Carga.CodigoCargaEmbarcador,
                    Filial = guarita.Carga.Filial?.Descricao ?? string.Empty,
                    guarita.Carga.TipoFreteEscolhido,
                    ModeloVeicularCarga = guarita.Carga.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    Motoristas = guarita.Carga.NomeMotoristas,
                    Operador = guarita.Carga.Operador.Descricao,
                    PesoCarga = pesoCarga.ToString("n2"),
                    PesagemInicial = guarita.PesagemInicial.ToString("n2"),
                    PesagemFinal = guarita.PesagemFinal.ToString("n2"),
                    PesoBruto = pesoBruto.ToString("n2"),
                    DiferencaPeso = diferencaPeso.ToString("n2"),
                    PercentualDiferencaPeso = percentualDiferencaPeso.ToString("n2"),
                    Placas = guarita.Carga.PlacasVeiculos,
                    Rota = guarita.Carga.Rota?.Descricao,
                    Situacao = guarita.Carga.SituacaoCarga.ObterDescricao(),
                    guarita.Carga.SituacaoAlteracaoFreteCarga,
                    guarita.Carga.SituacaoAutorizacaoIntegracaoCTe,
                    TipoCarga = guarita.Carga.TipoDeCarga?.Descricao,
                    TipoOperacao = guarita.Carga.TipoOperacao?.Descricao,
                    Transportador = guarita.Carga.Empresa?.Descricao,
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

        #endregion Métodos Globais Sobrescritos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaPesagemAprovacao ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaPesagemAprovacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaPesagemAprovacao()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                SituacaoPesagemCarga = Request.GetNullableEnumParam<SituacaoPesagemCarga>("SituacaoPesagem"),
                CodigosModeloVeicular = Request.GetListParam<int>("ModeloVeicularCarga")
            };

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosTipoOperacao = Request.GetListParam<int>("TipoOperacao");
            List<int> codigosTipoCarga = Request.GetListParam<int>("TipoDeCarga");
            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosTipoOperacao = codigosTipoOperacao.Count == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : codigosTipoOperacao;
            filtrosPesquisa.CodigosTipoCarga = codigosTipoCarga.Count == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : codigosTipoOperacao;

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

            return propriedadeOrdenar;
        }

        private dynamic ObterRetornoGrid(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita)
        {
            decimal pesoCarga = (guarita.Carga.DadosSumarizados?.PesoTotal ?? 0m);
            decimal pesoBruto = (guarita.PesagemInicial + pesoCarga);
            decimal diferencaPeso = guarita.PesagemFinal - pesoBruto;
            decimal percentualDiferencaPeso = (pesoCarga > 0m) ? (diferencaPeso / pesoCarga * 100) : 0m;

            return new
            {
                Codigo = guarita.Codigo,
                CodigoCargaEmbarcador = guarita.Carga.CodigoCargaEmbarcador,
                Filial = guarita.Carga.Filial?.Descricao,
                ModeloVeicular = guarita.Carga.ModeloVeicularCarga?.Descricao,
                TipoCarga = guarita.Carga.TipoDeCarga?.Descricao,
                TipoOperacao = guarita.Carga.TipoOperacao?.Descricao,
                SituacaoPesagemCarga = guarita.SituacaoPesagemCarga?.ObterDescricao(),
                Situacao = guarita.Carga.SituacaoCarga.ObterDescricao(),
                PesoCarga = pesoCarga.ToString("n2"),
                PesagemInicial = guarita.PesagemInicial.ToString("n2"),
                PesagemFinal = guarita.PesagemFinal.ToString("n2"),
                PesoBruto = pesoBruto.ToString("n2"),
                DiferencaPeso = diferencaPeso.ToString("n2"),
                PercentualDiferencaPeso = percentualDiferencaPeso.ToString("n2")
            };
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita origem)
        {
            return origem.SituacaoPesagemCarga == SituacaoPesagemCarga.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> guaritas;
            bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                Repositorio.Embarcador.GestaoPatio.AprovacaoAlcadaToleranciaPesagem repositorioAprovacaoAlcadaCarga = new Repositorio.Embarcador.GestaoPatio.AprovacaoAlcadaToleranciaPesagem(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaPesagemAprovacao filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" };
                guaritas = repositorioAprovacaoAlcadaCarga.Consultar(filtrosPesquisa, parametrosConsulta);

                List<int> codigosItensNaoSelecionados = Request.GetListParam<int>("ItensNaoSelecionados");

                foreach (var itemNaoSelecionado in codigosItensNaoSelecionados)
                    guaritas.RemoveAll(guarita => codigosItensNaoSelecionados.Contains(guarita.Codigo));
            }
            else
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                List<int> codigosItensSelecionados = Request.GetListParam<int>("ItensSelecionados");

                guaritas = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCodigos(codigosItensSelecionados);
            }

            return guaritas.Select(guarita => guarita.Codigo).ToList();
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
                grid.AdicionarCabecalho(descricao: "Nº da Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tipo de Carga", propriedade: "TipoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tipo de Operação", propriedade: "TipoOperacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Modelo Veícular", propriedade: "ModeloVeicular", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Situação da Pesagem", propriedade: "SituacaoPesagemCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Peso da carga", propriedade: "PesoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Pesagem Inicial", propriedade: "PesagemInicial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Pesagem Final", propriedade: "PesagemFinal", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Peso Bruto", propriedade: "PesoBruto", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Diferença do Peso (KG)", propriedade: "DiferencaPeso", tamanho: 10, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Diferença do Peso (%)", propriedade: "PercentualDiferencaPeso", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "AutorizacaoPesagem/Pesquisa", "grid-pesquisa-pesagem-carga");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaPesagemAprovacao filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GestaoPatio.AprovacaoAlcadaToleranciaPesagem repositorio = new Repositorio.Embarcador.GestaoPatio.AprovacaoAlcadaToleranciaPesagem(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> guaritas = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>();

                var lista = (
                    from guarita in guaritas
                    select ObterRetornoGrid(guarita)
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Carga.SituacaoCarga == SituacaoCarga.Cancelada || origem.Carga.SituacaoCarga == SituacaoCarga.Anulada)
                return;

            if (origem.SituacaoPesagemCarga != SituacaoPesagemCarga.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
            Servicos.Embarcador.Carga.CargaAprovacaoPesagem servicoCargaAprovacaoPesagem = new Servicos.Embarcador.Carga.CargaAprovacaoPesagem(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (!servicoCargaAprovacaoPesagem.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                origem.SituacaoPesagemCarga = SituacaoPesagemCarga.Aprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem.FluxoGestaoPatio, "Pesagem final aprovada", unitOfWork);

                Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio servicoMensagemAlerta = new Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio(unitOfWork);

                servicoMensagemAlerta.Confirmar(origem.FluxoGestaoPatio, TipoMensagemAlerta.CargaSemRegraAutorizacaoTolerenciaPesagem);
                servicoMensagemAlerta.Confirmar(origem.FluxoGestaoPatio, TipoMensagemAlerta.CargaAguardandoAprovacaoPesagem);
            }
            else
            {
                origem.SituacaoPesagemCarga = SituacaoPesagemCarga.Reprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem.FluxoGestaoPatio, "Pesagem final reprovada", unitOfWork);
            }

            repositorioCargaJanelaCarregamentoGuarita.Atualizar(origem);
        }

        #endregion Métodos Protegidos Sobrescritos
    }
}