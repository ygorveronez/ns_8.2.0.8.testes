using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Relatorios.Embarcador.DataSource.Logistica;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Logistica/JanelaDisponivelAgendamento")]
    public class JanelaDisponivelAgendamentoController : BaseController
    {
		#region Construtores

		public JanelaDisponivelAgendamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R240_JanelaDisponivelAgendamento;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao); ;
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Janelas Disponíveis do Agendamento", "Logistica", "JanelaDisponivelAgendamento.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                FiltroPesquisaRelatorioJanelaDisponivelAgendamento filtrosPesquisa = ObterFiltrosPesquisa();
                List<JanelaDisponivelAgendamento> listaJanelas = ObterResultados(filtrosPesquisa, unitOfWork);

                grid.AdicionaRows(listaJanelas);
                grid.setarQuantidadeTotal(listaJanelas.Count);

                return new JsonpResult(grid);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                FiltroPesquisaRelatorioJanelaDisponivelAgendamento filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);


                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemp.ObterParametrosConsulta();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemp.PropriedadeAgrupa);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioJanelaDisponivelAgendamento(propriedades, filtrosPesquisa, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioJanelaDisponivelAgendamento(List<PropriedadeAgrupamento> propriedades, FiltroPesquisaRelatorioJanelaDisponivelAgendamento filtrosPesquisa, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = relatorioTemp.OrdemOrdenacao,
                    PropriedadeOrdenar = relatorioTemp.PropriedadeOrdena
                };

                List<Parametro> parametros = ObterParametros(filtrosPesquisa, unitOfWork);
                List<JanelaDisponivelAgendamento> listaJanelas = ObterResultados(filtrosPesquisa, unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Logistica/JanelaDisponivelAgendamento",parametros,relatorioControleGeracao, relatorioTemp, listaJanelas, unitOfWork);

                await unitOfWork.DisposeAsync();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
        }

        private List<Parametro> ObterParametros(FiltroPesquisaRelatorioJanelaDisponivelAgendamento filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            List<Parametro> parametros = new List<Parametro>
            {
                new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal),
                new Parametro("CentroDescarregamento", repositorioCentroDescarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroDescarregamento).Descricao),
                new Parametro("ModeloVeicular", filtrosPesquisa.CodigoModeloVeicular > 0 ? repositorioModeloVeicular.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeicular).Descricao : null)
            };

            return parametros;
        }

        private FiltroPesquisaRelatorioJanelaDisponivelAgendamento ObterFiltrosPesquisa()
        {
            FiltroPesquisaRelatorioJanelaDisponivelAgendamento filtroPesquisa = new FiltroPesquisaRelatorioJanelaDisponivelAgendamento()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento"),
                CodigoModeloVeicular = Request.GetIntParam("ModeloVeicular")
            };

            if (filtroPesquisa.DataInicial == DateTime.MinValue)
                throw new ControllerException("A data inicial deve ser informada");

            if (filtroPesquisa.DataFinal == DateTime.MinValue)
                throw new ControllerException("A data final deve ser informada");

            if (filtroPesquisa.DataInicial > filtroPesquisa.DataFinal)
                throw new ControllerException("A data inicial não pode ser superior a final");

            if ((filtroPesquisa.DataFinal - filtroPesquisa.DataInicial).TotalDays >= 30)
                throw new ControllerException("Não é possível consultar um período superior a 30 dias");

            return filtroPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoDeCarga", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Quantidade de Itens", "QuantidadeItens", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Hora", "Hora", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Quantidade de Janelas Disponíveis", "QuantidadeJanelasDisponiveis", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Quantidade de Janelas Ocupadas", "QuantidadeJanelasOcupadas", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Quantidade de Janelas Cadastradas", "QuantidadeJanelasCadastradas", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Janela Exclusiva", "JanelaExclusiva", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo da Agenda", "AgendaExtra", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Chegada Planejada", "DataChegadaPlanejada", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Dia", "Dia", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Mês", "Mes", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Grupo de Produto", "GrupoProduto", 15, Models.Grid.Align.center, false, false, false, false, true);

            return grid;
        }

        private List<JanelaDisponivelAgendamento> ObterResultados(FiltroPesquisaRelatorioJanelaDisponivelAgendamento filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroDescarregamento);

            if (centroDescarregamento == null)
                return new List<JanelaDisponivelAgendamento>();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga = ObterTiposDeCargaPorCentroDescarregamento(filtrosPesquisa, unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento = ObterPeriodosDescarregamento(filtrosPesquisa, unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasExistentes = ObterJanelasExistentes(filtrosPesquisa, unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga> listaPeriodoDescarregamentoTipoDeCarga = ObterPeriodoDescarregamentoTipoDeCarga(tiposDeCarga.Select(x => x.Codigo).ToList(), filtrosPesquisa.CodigoCentroDescarregamento, unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> listaPeriodosGrupoPessoas = ObterGruposPessoaPeriodos(periodosDescarregamento, unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente> listaPeriodosRemetente = ObterRemetentesPeriodos(periodosDescarregamento, unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> listaPeriodoDescarregamentoGrupoProduto = ObterPeriodoDescarregamentoGrupoProduto(periodosDescarregamento, unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> listaPeriodoDescarregamentoCanalVenda = ObterPeriodoDescarregamentoCanalVenda(periodosDescarregamento, unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> listaAgendamentosColetas = ObterAgendas(janelasExistentes.Select(obj => obj.Carga).ToList(), unitOfWork);
            bool filtrarPorCanaisVenda = listaPeriodoDescarregamentoCanalVenda.Count > 0;
            List<(int CodigoGrupoPessoas, string RaizCnpj)> dadosGruposPessoas = ObterDadosGruposPessoas(listaPeriodosGrupoPessoas, unitOfWork);
            List<(int CodigoCarga, int Sku)> dadosSku = ObterDadosSku(janelasExistentes, unitOfWork);
            IList<(int CodigoCarga, int CodigoGrupoProduto)> dadosGruposProdutosDominantes = ObterDadosGruposProdutosDominantes(janelasExistentes, unitOfWork);
            IList<(int CodigoCarga, int CodigoCanalVenda)> dadosCanaisVendas = ObterDadosCanaisVendas(janelasExistentes, centroDescarregamento, unitOfWork);
            List<JanelaDisponivelAgendamento> listaResultados = new List<JanelaDisponivelAgendamento>();

            for (DateTime inicio = filtrosPesquisa.DataInicial.Date; inicio <= filtrosPesquisa.DataFinal.Date; inicio = inicio.Date.AddDays(1))
            {
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamentoPorDia;

                if (centroDescarregamento.CapacidadeDescaregamentoPorDia)
                    periodosDescarregamentoPorDia = periodosDescarregamento.Where(o => (o.DiaDoMes == inicio.Day) && (o.Mes == inicio.Month)).ToList();
                else
                    periodosDescarregamentoPorDia = periodosDescarregamento.Where(o => o.Dia == DiaSemanaHelper.ObterDiaSemana(inicio)).ToList();

                List<TimeSpan> horariosInicioDescarregamentoPorPeriodo = periodosDescarregamentoPorDia.Select(periodo => periodo.HoraInicio).Distinct().ToList();
                List<int> codigosCargasComPeriodo = new List<int>();

                foreach (TimeSpan horarioInicioDescarregamento in horariosInicioDescarregamentoPorPeriodo)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamentoPorDiaEHorario = periodosDescarregamentoPorDia.Where(o => o.HoraInicio == horarioInicioDescarregamento).ToList();

                    foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo in periodosDescarregamentoPorDiaEHorario)
                    {
                        List<int> codigosTiposCargaDoPeriodo = listaPeriodoDescarregamentoTipoDeCarga.Where(tipoCargaPeriodo => tipoCargaPeriodo.PeriodoDescarregamento.Codigo == periodo.Codigo).Select(tipoCargaPeriodo => tipoCargaPeriodo.TipoDeCarga.Codigo).Distinct().ToList();
                        List<int> codigosGruposPessoasPeriodo = listaPeriodosGrupoPessoas.Where(grupoPessoaPeriodo => grupoPessoaPeriodo.PeriodoDescarregamento.Codigo == periodo.Codigo).Select(grupoPessoaPeriodo => grupoPessoaPeriodo.GrupoPessoas.Codigo).Distinct().ToList();
                        List<int> codigosGruposProdutosPeriodo = listaPeriodoDescarregamentoGrupoProduto.Where(grupoProdutoPeriodo => grupoProdutoPeriodo.PeriodoDescarregamento.Codigo == periodo.Codigo).Select(grupoProdutoPeriodo => grupoProdutoPeriodo.GrupoProduto.Codigo).Distinct().ToList();
                        List<int> codigosCanaisVendasPeriodo = listaPeriodoDescarregamentoCanalVenda.Where(canalVendaPeriodo => canalVendaPeriodo.PeriodoDescarregamento.Codigo == periodo.Codigo).Select(canalVendaPeriodo => canalVendaPeriodo.CanalVenda.Codigo).Distinct().ToList();
                        List<string> raizesCnpjGruposPessoas = dadosGruposPessoas.Where(dadosGrupoPessoas => codigosGruposPessoasPeriodo.Contains(dadosGrupoPessoas.CodigoGrupoPessoas)).Select(dadosGrupoPessoas => dadosGrupoPessoas.RaizCnpj).ToList();
                        List<double> cpfCnpjRemetentesPeriodo = listaPeriodosRemetente.Where(remetentePeriodo => remetentePeriodo.PeriodoDescarregamento.Codigo == periodo.Codigo).Select(remetentePeriodo => remetentePeriodo.Remetente.CPF_CNPJ).Distinct().ToList();
                        List<string> descricoesTiposCargaPeriodo = listaPeriodoDescarregamentoTipoDeCarga.Where(tipoCargaPeriodo => tipoCargaPeriodo.PeriodoDescarregamento.Codigo == periodo.Codigo).Select(tipoCargaPeriodo => tipoCargaPeriodo.TipoDeCarga.Descricao).Distinct().ToList();
                        List<string> descricoesGruposProdutosPeriodo = listaPeriodoDescarregamentoGrupoProduto.Where(grupoProdutoPeriodo => grupoProdutoPeriodo.PeriodoDescarregamento.Codigo == periodo.Codigo).Select(grupoProdutoPeriodo => grupoProdutoPeriodo.GrupoProduto.Descricao).Distinct().ToList();
                        List<int> codigosCargasPossiveis = janelasExistentes.Where(janela => !janela.Carga.AgendaExtra).Select(janela => janela.Carga.Codigo).ToList();

                        if (configuracaoJanelaCarregamento.UtilizarPeriodoDescarregamentoExclusivo)
                        {
                            if ((codigosGruposPessoasPeriodo.Count > 0) || (cpfCnpjRemetentesPeriodo.Count > 0))
                            {
                                List<int> codigosCargas = janelasExistentes.Where(janela => janela.PeriodoDescarregamentoExclusivo).Select(janela => janela.Carga.Codigo).ToList();
                                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> agendamentosColetas = listaAgendamentosColetas.Where(agendamentoColeta => codigosCargas.Contains(agendamentoColeta.Carga.Codigo) && (agendamentoColeta.Remetente != null)).ToList();

                                if (codigosGruposPessoasPeriodo.Count > 0)
                                    agendamentosColetas = agendamentosColetas.Where(agendamentoColeta => raizesCnpjGruposPessoas.Contains(agendamentoColeta.Remetente.RaizCnpjSemFormato)).ToList();

                                if (cpfCnpjRemetentesPeriodo.Count > 0)
                                    agendamentosColetas = agendamentosColetas.Where(agendamentoColeta => cpfCnpjRemetentesPeriodo.Contains(agendamentoColeta.Remetente.CPF_CNPJ)).ToList();

                                List<int> codigosCargaPorAgendamentoColeta = agendamentosColetas.Select(agendamentoColeta => agendamentoColeta.Carga.Codigo).ToList();

                                codigosCargasPossiveis = codigosCargasPossiveis.Where(codigoCarga => codigosCargaPorAgendamentoColeta.Contains(codigoCarga)).ToList();
                            }
                            else
                            {
                                List<int> codigosCargasSemPeriodoExclusivo = janelasExistentes.Where(janela => !janela.PeriodoDescarregamentoExclusivo).Select(janela => janela.Carga.Codigo).ToList();

                                codigosCargasPossiveis = codigosCargasPossiveis.Where(codigoCarga => codigosCargasSemPeriodoExclusivo.Contains(codigoCarga)).ToList();
                            }
                        }
                        else if ((codigosGruposPessoasPeriodo.Count > 0) || (cpfCnpjRemetentesPeriodo.Count > 0))
                        {
                            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> agendamentosColetas = listaAgendamentosColetas.Where(obj => janelasExistentes.Select(x => x.Carga.Codigo).Contains(obj.Carga.Codigo)).ToList();

                            if (codigosGruposPessoasPeriodo.Count > 0)
                                agendamentosColetas = agendamentosColetas.Where(agendamentoColeta => (agendamentoColeta.Remetente?.GrupoPessoas == null) || codigosGruposPessoasPeriodo.Contains(agendamentoColeta.Remetente.GrupoPessoas.Codigo)).ToList();

                            if (cpfCnpjRemetentesPeriodo.Count > 0)
                                agendamentosColetas = agendamentosColetas.Where(agendamentoColeta => (agendamentoColeta.Remetente == null) || cpfCnpjRemetentesPeriodo.Contains(agendamentoColeta.Remetente.CPF_CNPJ)).ToList();

                            List<int> codigosCargaPorAgendamentoColeta = agendamentosColetas.Select(agendamentoColeta => agendamentoColeta.Carga.Codigo).ToList();

                            codigosCargasPossiveis = codigosCargasPossiveis.Where(codigoCarga => codigosCargaPorAgendamentoColeta.Contains(codigoCarga)).ToList();
                        }

                        if (codigosTiposCargaDoPeriodo.Count > 0)
                        {
                            List<int> codigosCargasPorTipoCarga = janelasExistentes.Where(janela => (janela.Carga.TipoDeCarga != null) && codigosTiposCargaDoPeriodo.Contains(janela.Carga.TipoDeCarga.Codigo)).Select(janela => janela.Carga.Codigo).ToList();

                            codigosCargasPossiveis = codigosCargasPossiveis.Where(codigoCarga => codigosCargasPorTipoCarga.Contains(codigoCarga)).ToList();
                        }

                        if (codigosGruposProdutosPeriodo.Count > 0)
                        {
                            List<int> codigosCargasPorGrupoProduto = dadosGruposProdutosDominantes.Where(grupoProdutoDominante => (grupoProdutoDominante.CodigoGrupoProduto == 0) || codigosGruposProdutosPeriodo.Contains(grupoProdutoDominante.CodigoGrupoProduto)).Select(grupoProdutoDominante => grupoProdutoDominante.CodigoCarga).ToList();

                            codigosCargasPossiveis = codigosCargasPossiveis.Where(codigoCarga => codigosCargasPorGrupoProduto.Contains(codigoCarga)).ToList();
                        }

                        if (filtrarPorCanaisVenda)
                        {
                            List<int> codigosCargasPorCanalVenda;

                            if (codigosCanaisVendasPeriodo.Count > 0)
                                codigosCargasPorCanalVenda = dadosCanaisVendas.Where(canalVenda => codigosCanaisVendasPeriodo.Contains(canalVenda.CodigoCanalVenda)).Select(canalVenda => canalVenda.CodigoCarga).ToList();
                            else
                                codigosCargasPorCanalVenda = dadosCanaisVendas.Where(canalVenda => canalVenda.CodigoCanalVenda == 0).Select(canalVenda => canalVenda.CodigoCarga).ToList();

                            codigosCargasPossiveis = codigosCargasPossiveis.Where(codigoCarga => codigosCargasPorCanalVenda.Contains(codigoCarga)).ToList();
                        }

                        if (periodo.SkuDe.HasValue || periodo.SkuAte.HasValue)
                        {
                            List<int> codigosCargasPorSku;

                            if (periodo.SkuDe.HasValue && periodo.SkuAte.HasValue)
                                codigosCargasPorSku = dadosSku.Where(skuPorCarga => skuPorCarga.Sku >= periodo.SkuDe.Value && skuPorCarga.Sku <= periodo.SkuAte.Value).Select(skuPorCarga => skuPorCarga.CodigoCarga).ToList();
                            else if (periodo.SkuDe.HasValue)
                                codigosCargasPorSku = dadosSku.Where(skuPorCarga => skuPorCarga.Sku >= periodo.SkuDe.Value).Select(skuPorCarga => skuPorCarga.CodigoCarga).ToList();
                            else
                                codigosCargasPorSku = dadosSku.Where(skuPorCarga => skuPorCarga.Sku <= periodo.SkuAte.Value).Select(skuPorCarga => skuPorCarga.CodigoCarga).ToList();

                            codigosCargasPossiveis = codigosCargasPossiveis.Where(codigoCarga => codigosCargasPorSku.Contains(codigoCarga)).ToList();
                        }

                        List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasExistentesPorPeriodo = ObterJanelasExistentesNoPeriodo(inicio, periodo, janelasExistentes, codigosCargasPossiveis);
                        int quantidadeJanelasUsadasNoPeriodo = janelasExistentesPorPeriodo.Count;
                        int quantidadeDisponivel = periodo.CapacidadeDescarregamentoSimultaneoTotal - quantidadeJanelasUsadasNoPeriodo;

                        JanelaDisponivelAgendamento janelaDisponivel = new JanelaDisponivelAgendamento()
                        {
                            Data = inicio.ToString("dd/MM/yyyy"),
                            Hora = periodo.HoraInicio.ToString(@"hh\:mm"),
                            InicioDescarregamento = inicio.Add(periodo.HoraInicio),
                            QuantidadeJanelasDisponiveis = quantidadeDisponivel < 0 ? 0 : quantidadeDisponivel,
                            QuantidadeJanelasOcupadas = quantidadeJanelasUsadasNoPeriodo,
                            QuantidadeJanelasCadastradas = periodo.CapacidadeDescarregamentoSimultaneoTotal,
                            QuantidadeItens = $"De {periodo.SkuDe} Até {periodo.SkuAte}",
                            TipoDeCarga = string.Join(", ", descricoesTiposCargaPeriodo),
                            Destino = centroDescarregamento.Descricao,
                            JanelaExclusiva = cpfCnpjRemetentesPeriodo.Count > 0 || codigosGruposPessoasPeriodo.Count > 0 ? "Sim" : "Não",
                            AgendaExtra = "Normal",
                            DataChegadaPlanejada = string.Join(", ", from obj in janelasExistentesPorPeriodo where obj.Carga.DataFimViagemReprogramada.HasValue select obj.Carga.DataFimViagemReprogramada?.ToDateTimeString() ?? string.Empty),
                            Dia = centroDescarregamento.CapacidadeDescaregamentoPorDia ? periodo.DiaDoMes.ToString() : string.Empty,
                            Mes = centroDescarregamento.CapacidadeDescaregamentoPorDia ? periodo.Mes.ToString() : string.Empty,
                            GrupoProduto = string.Join(", ", descricoesGruposProdutosPeriodo)
                        };

                        listaResultados.Add(janelaDisponivel);
                        codigosCargasComPeriodo.AddRange(janelasExistentesPorPeriodo.Select(janela => janela.Carga.Codigo).ToList());
                    }
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasAgendaExtraExistentesPorDia = janelasExistentes.Where(janelaDescarga => (janelaDescarga.InicioDescarregamento.Date == inicio) && janelaDescarga.Carga.AgendaExtra).ToList();
                List<TimeSpan> horariosInicioDescarregamentoAgendaExtra = janelasAgendaExtraExistentesPorDia.Select(janela => janela.InicioDescarregamento.TimeOfDay).Distinct().ToList();

                foreach (TimeSpan horarioInicioDescarregamento in horariosInicioDescarregamentoAgendaExtra)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasAgendaExtraExistentesPorDiaEHorario = janelasAgendaExtraExistentesPorDia.Where(janela => janela.InicioDescarregamento.TimeOfDay == horarioInicioDescarregamento).Distinct().ToList();

                    JanelaDisponivelAgendamento janelaAgendaExtra = new JanelaDisponivelAgendamento()
                    {
                        Data = inicio.ToString("dd/MM/yyyy"),
                        Hora = horarioInicioDescarregamento.ToString(@"hh\:mm"),
                        InicioDescarregamento = inicio.Add(horarioInicioDescarregamento),
                        QuantidadeJanelasDisponiveis = 0,
                        QuantidadeJanelasOcupadas = janelasAgendaExtraExistentesPorDiaEHorario.Count,
                        QuantidadeJanelasCadastradas = 0,
                        QuantidadeItens = string.Empty,
                        TipoDeCarga = string.Empty,
                        Destino = centroDescarregamento.Descricao,
                        JanelaExclusiva = "Não",
                        AgendaExtra = "Extra",
                        DataChegadaPlanejada = string.Join(", ", from obj in janelasAgendaExtraExistentesPorDiaEHorario where obj.Carga.DataFimViagemReprogramada.HasValue select obj.Carga.DataFimViagemReprogramada?.ToDateTimeString() ?? string.Empty),
                        Dia = centroDescarregamento.CapacidadeDescaregamentoPorDia ? inicio.Day.ToString() : string.Empty,
                        Mes = centroDescarregamento.CapacidadeDescaregamentoPorDia ? inicio.Month.ToString() : string.Empty,
                        GrupoProduto = string.Empty
                    };

                    listaResultados.Add(janelaAgendaExtra);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasSemPeriodoExistentesPorDia = janelasExistentes.Where(janelaDescarga => (janelaDescarga.InicioDescarregamento.Date == inicio) && !janelaDescarga.Carga.AgendaExtra && !codigosCargasComPeriodo.Contains(janelaDescarga.Carga.Codigo)).ToList();
                List<TimeSpan> horariosInicioDescarregamentoSemPeriodo = janelasSemPeriodoExistentesPorDia.Select(janela => janela.InicioDescarregamento.TimeOfDay).Distinct().ToList();

                foreach (TimeSpan horarioInicioDescarregamento in horariosInicioDescarregamentoSemPeriodo)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasSemPeriodoExistentesPorDiaEHorario = janelasSemPeriodoExistentesPorDia.Where(janela => janela.InicioDescarregamento.TimeOfDay == horarioInicioDescarregamento).Distinct().ToList();

                    JanelaDisponivelAgendamento janelaAgendaExtra = new JanelaDisponivelAgendamento()
                    {
                        Data = inicio.ToString("dd/MM/yyyy"),
                        Hora = horarioInicioDescarregamento.ToString(@"hh\:mm"),
                        InicioDescarregamento = inicio.Add(horarioInicioDescarregamento),
                        QuantidadeJanelasDisponiveis = 0,
                        QuantidadeJanelasOcupadas = janelasSemPeriodoExistentesPorDiaEHorario.Count,
                        QuantidadeJanelasCadastradas = 0,
                        QuantidadeItens = string.Empty,
                        TipoDeCarga = string.Empty,
                        Destino = centroDescarregamento.Descricao,
                        JanelaExclusiva = "Não",
                        AgendaExtra = "Sem Período Configurado",
                        DataChegadaPlanejada = string.Join(", ", from obj in janelasSemPeriodoExistentesPorDiaEHorario where obj.Carga.DataFimViagemReprogramada.HasValue select obj.Carga.DataFimViagemReprogramada?.ToDateTimeString() ?? string.Empty),
                        Dia = centroDescarregamento.CapacidadeDescaregamentoPorDia ? inicio.Day.ToString() : string.Empty,
                        Mes = centroDescarregamento.CapacidadeDescaregamentoPorDia ? inicio.Month.ToString() : string.Empty,
                        GrupoProduto = string.Empty
                    };

                    listaResultados.Add(janelaAgendaExtra);
                }
            }

            return listaResultados.OrderBy(janela => janela.InicioDescarregamento).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> ObterAgendas(List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorio = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            return repositorio.BuscarPorCargas(listaCargas.Select(obj => obj.Codigo).ToList());
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> ObterGruposPessoaPeriodos(List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa repositorio = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa(unitOfWork);
            return repositorio.BuscarPorPeriodosDescarregamento(periodosDescarregamento.Select(obj => obj.Codigo).ToList());
        }

        private List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> ObterTiposDeCargaPorCentroDescarregamento(FiltroPesquisaRelatorioJanelaDisponivelAgendamento filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);

            return repositorioPeriodoDescarregamento.BuscarTiposDeCargaPorPeriodoCentroDescarregamento(filtrosPesquisa);
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> ObterPeriodosDescarregamento(FiltroPesquisaRelatorioJanelaDisponivelAgendamento filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);

            return repositorioPeriodoDescarregamento.BuscarPorCentroDescarregamento(filtrosPesquisa.CodigoCentroDescarregamento).OrderBy(obj => obj.HoraInicio).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> ObterJanelasExistentes(FiltroPesquisaRelatorioJanelaDisponivelAgendamento filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);

            return repositorioCargaJanelaDescarregamento.BuscarPorPeriodo(filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, filtrosPesquisa.CodigoCentroDescarregamento);
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente> ObterRemetentesPeriodos(List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente repositorio = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente(unitOfWork);
            return repositorio.BuscarPorPeriodosDescarregamento(periodosDescarregamento.Select(obj => obj.Codigo).ToList());
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> ObterJanelasExistentesNoPeriodo(DateTime data, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasExistentes, List<int> codigosCargasPossiveis)
        {
            DateTime dataInicioDescarregamento = data.Add(periodo.HoraInicio);
            DateTime dataTerminoDescarregamento = data.Add(periodo.HoraTermino);
            DateTime dataTerminoDescarregamentoComTolerancia = dataTerminoDescarregamento.AddMinutes(periodo.ToleranciaExcessoTempo);

            IEnumerable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaJanelaExistenteNoPeriodo = janelasExistentes
                .Where(janelaDescarga =>
                    codigosCargasPossiveis.Contains(janelaDescarga.Carga.Codigo) &&
                    (
                        (janelaDescarga.InicioDescarregamento >= dataInicioDescarregamento && janelaDescarga.InicioDescarregamento < dataTerminoDescarregamento) ||
                        (janelaDescarga.TerminoDescarregamento > dataInicioDescarregamento && janelaDescarga.TerminoDescarregamento <= dataTerminoDescarregamentoComTolerancia)
                    )
                );

            return listaJanelaExistenteNoPeriodo.ToList();
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga> ObterPeriodoDescarregamentoTipoDeCarga(List<int> codigosTipoDeCarga, int codigoCentroDescarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga repositorioPeriodoDescarregamentoTipoDeCarga = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga(unitOfWork);

            return repositorioPeriodoDescarregamentoTipoDeCarga.BuscarPorTiposDeCargaCentroDescarregamento(codigosTipoDeCarga, codigoCentroDescarregamento);
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> ObterPeriodoDescarregamentoGrupoProduto(List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodoDescarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto repositorioPeriodoDescarregamentoGrupoProduto = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto(unitOfWork);

            return repositorioPeriodoDescarregamentoGrupoProduto.BuscarPorPeriodosDescarregamento(periodoDescarregamento.Select(obj => obj.Codigo).ToList());
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> ObterPeriodoDescarregamentoCanalVenda(List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodoDescarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda repositorioPeriodoDescarregamentoCanalVenda = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda(unitOfWork);

            return repositorioPeriodoDescarregamentoCanalVenda.BuscarPorPeriodosDescarregamento(periodoDescarregamento.Select(obj => obj.Codigo).ToList());
        }

        private List<(int CodigoGrupoPessoas, string RaizCnpj)> ObterDadosGruposPessoas(List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> listaPeriodosGrupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ repositorioGrupoPessoasRaizCNPJ = new Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ(unitOfWork);

            return repositorioGrupoPessoasRaizCNPJ.BuscarDadosPorGruposPessoas(listaPeriodosGrupoPessoas.Select(grupoPessoas => grupoPessoas.GrupoPessoas.Codigo).ToList());
        }

        private List<(int CodigoCarga, int Sku)> ObterDadosSku(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasExistentes, Repositorio.UnitOfWork unitOfWork)
        {
            List<(int CodigoCarga, int Sku)> cargasSku = new List<(int Carga, int Sku)>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento janela in janelasExistentes)
            {
                if (janela.Carga.DadosSumarizados != null && janela.Carga.DadosSumarizados.QuantidadeSKU.HasValue)
                    cargasSku.Add((janela.Carga.Codigo, janela.Carga.DadosSumarizados.QuantidadeSKU.Value));
            }

            janelasExistentes = janelasExistentes.Where(obj => !cargasSku.Select(x => x.CodigoCarga).ToList().Contains(obj.Carga.Codigo)).ToList();

            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            return janelasExistentes.Any(obj => obj.CentroDescarregamento?.Destinatario != null) ? repositorioAgendamentoColetaPedido.BuscarSKUPorCargas(janelasExistentes.Select(obj => obj.Carga.Codigo).ToList(), janelasExistentes.Where(obj => obj.CentroDescarregamento?.Destinatario != null).FirstOrDefault().CentroDescarregamento.Destinatario.CPF_CNPJ) : new List<(int CodigoCarga, int Sku)>();
        }

        private IList<(int CodigoCarga, int CodigoGrupoProduto)> ObterDadosGruposProdutosDominantes(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasExistentes, Repositorio.UnitOfWork unitOfWork)
        {
            List<int> codigosCargas = janelasExistentes.Select(janela => janela.Carga.Codigo).ToList();

            if (codigosCargas.Count == 0)
                return new List<(int CodigoCarga, int CodigoGrupoProduto)>();

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            return repositorioCarga.BuscarDadosGruposProdutosDominantesPorCargas(codigosCargas);
        }

        private IList<(int CodigoCarga, int CodigoCanalVenda)> ObterDadosCanaisVendas(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasExistentes, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            List<int> codigosCargas = janelasExistentes.Select(janela => janela.Carga.Codigo).ToList();

            if (codigosCargas.Count == 0)
                return new List<(int CodigoCarga, int CodigoCanalVenda)>();

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            return repositorioCargaPedido.BuscarDadosCanalVendaPorDestinatarioDasCargas(codigosCargas, centroDescarregamento.Destinatario.CPF_CNPJ);
        }

        #endregion
    }
}
