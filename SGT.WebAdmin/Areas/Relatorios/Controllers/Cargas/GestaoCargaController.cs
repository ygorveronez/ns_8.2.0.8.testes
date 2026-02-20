using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Repositorio;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Cargas/GestaoCarga")]
    public class GestaoCargaController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga>
    {
		#region Construtores

		public GestaoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Atributos

		private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R280_GestaoCarga;
        private readonly int _numeroMaximoComplementos = 60;
        private readonly decimal _tamanhoColunaExtraPequena = 1m;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, Localization.Resources.Relatorios.Cargas.GestaoCarga.RelatorioDeGestaoDeCarga, "Cargas", "GestaoCarga.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Filial", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Relatorios.Cargas.GestaoCarga.OcorreuUmaFalhaAoBuscarDadosRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Carga.GestaoCarga srvRelatorioCarga = new Servicos.Embarcador.Relatorios.Carga.GestaoCarga(unitOfWork, TipoServicoMultisoftware, Cliente);

                srvRelatorioCarga.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioGestaoCarga> listaCargas, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaCargas);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Relatorios.Cargas.GestaoCarga.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.Cargas.GestaoCarga.OcorreuUmaFalhaAoGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorioGestaoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga filtros = ObterFiltrosPesquisa(unitOfWork);

                // TODO (ct-reports): Repassar CT
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioGestaoCarga> listaReport = repCarga.ConsultarRelatorioGestaoCargaDetalhado(filtros);

                if (listaReport.Count > 0)
                {
                    List<Parametro> listaParametros = await ObterParametrosGerarRelatorioGestaoAsync(filtros, unitOfWork, cancellationToken);

                    var report = await ReportRequest.WithType(ReportType.GerarRelatorioGestao)
                        .WithExecutionType(ExecutionType.Sync)
                        .AddExtraData("ListaParametros", listaParametros.ToJson())
                        .AddExtraData("FiltrosPesquisa", filtros.ToJson())
                        .CallReportAsync(cancellationToken: cancellationToken);

                    byte[] pdf = report.GetContentFile();

                    if (pdf == null)
                        throw new Exception();

                    return Arquivo(pdf, "application/pdf", "Relatório de Gestão de Carga.pdf");
                }
                else
                {
                    return new JsonpResult(false, false, Localization.Resources.Relatorios.Cargas.GestaoCarga.NenhumRegistroEncontradoParaGerarRelatorio);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Relatorios.Cargas.GestaoCarga.OcorreuUmaFalhaAoGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga()
            {
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                CodigosGrupoPessoa = Request.GetListParam<int>("GrupoPessoa"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CNPJsTomador = Request.GetListParam<double>("Tomador"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                StatusGestaoCarga = Request.GetEnumParam<StatusGestaoCarga>("StatusGestaoCarga"),
                IncluirOperacoesDeslocamentoVazio = Request.GetBoolParam("IncluirOperacoesDeslocamentoVazio"),
                NumeroNF = Request.GetIntParam("NumeroNF"),
                NumeroCTe = Request.GetIntParam("NumeroCTe")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                if (this.Usuario.Cliente != null)
                {
                    if (this.Usuario.Cliente.GrupoPessoas != null)
                        filtrosPesquisa.CodigosGrupoPessoa.Add(this.Usuario.Cliente.GrupoPessoas.Codigo);
                    else
                        filtrosPesquisa.CNPJsTomador.Add(this.Usuario.Cliente.CPF_CNPJ);
                }
                if (this.Usuario.ClienteFornecedor != null)
                {
                    if (this.Usuario.ClienteFornecedor.GrupoPessoas != null)
                        filtrosPesquisa.CodigosGrupoPessoa.Add(this.Usuario.ClienteFornecedor.GrupoPessoas.Codigo);
                    else
                        filtrosPesquisa.CNPJsTomador.Add(this.Usuario.ClienteFornecedor.CPF_CNPJ);
                }
                if (this.Usuario.ClienteTerceiro != null)
                {
                    if (this.Usuario.ClienteTerceiro.GrupoPessoas != null)
                        filtrosPesquisa.CodigosGrupoPessoa.Add(this.Usuario.ClienteTerceiro.GrupoPessoas.Codigo);
                    else
                        filtrosPesquisa.CNPJsTomador.Add(this.Usuario.ClienteTerceiro.CPF_CNPJ);
                }
            }

            return filtrosPesquisa;
        }

        protected override Task<FiltroPesquisaGestaoCarga> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Tipo, "Tipo", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "Status", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.GrupoPessoas, "Grupo", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.Filial, "Filial", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.ProgVei, "ProgVei", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Remetente, "Remetente", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Origem, "Origem", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.PaisOrigem, "PaisOrigem", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.FronteiraOrigem, "FronteiraOrigem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.FronteiraDestino, "FronteiraDestino", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.Cavalo, "Cavalo", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.Carretas, "Carretas", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.ChegadaNaOrigem, "ChegadaOrigemFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.SaidaDaOrigem, "SaidaOrigemFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.TempoNaOrigem, "TempoOrigemFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.ChegadaFronteiraOrigem, "ChegadaFronteiraOrigemFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.SaidaFronteiraOrigem, "SaidaFronteiraOrigemFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.TempoFronteiraOrigem, "TempoFronteiraOrigemFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.ChegadaFronteiraOrigem, "ChegadaFronteiraDestinoFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.SaidaFronteiraOrigem, "SaidaFronteiraDestinoFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.TempoFronteiraOrigem, "TempoFronteiraDestinoFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.ChegadaNoDestino, "ChegadaDestinoFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.SaidaDoDestino, "SaidaDestinoFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.TempoNoDestino, "TempoDestinoFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.TempoDeViagem, "TempoViagemFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);

            grid.AdicionarCabecalho("VlrDiaria", false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.NPedidoEmbarcador, "NumeroPedidoEmbarcador", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.NPedido, "NumeroPedido", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.Destinatario, "Destinatario", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.Destino, "Destino", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.PaisDestino, "PaisDestino", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.PrevisaodeEmbarque, "PrevisaoEmbarqueFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.PrevisaodeEmbarque, "ValorFrete", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.Produto, "Produto", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.Temperatura, "Temperatura", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.Motoristas, "Motoristas", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.UltimaPosicao, "DataUltimaPosicaoFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.LocalUltimaPosicao, "LocalUltimaPosicao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.DescricaoUltimaPosicao, "DescricaoLocalUltimaPosicao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);

            grid.AdicionarCabecalho("DataUltimaOcorrenciaFormatado", false);
            grid.AdicionarCabecalho("DescricaoUltimaOcorrencia", false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.NumeroNF, "NumeroNF", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.GestaoCarga.NumeroCTe, "NumeroCTe", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private async Task<List<Parametro>> ObterParametrosGerarRelatorioGestaoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga filtrosPesquisa, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> centrosResultado = filtrosPesquisa.CodigosCentroResultado.Count > 0 ? await repCentroResultado.BuscarPorCodigosAsync(filtrosPesquisa.CodigosCentroResultado) : new List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoas = filtrosPesquisa.CodigosGrupoPessoa.Count > 0 ? await repGrupoPessoas.BuscarPorCodigosAsync(filtrosPesquisa.CodigosGrupoPessoa) : new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = filtrosPesquisa.CodigosTipoOperacao.Count > 0 ? await repTipoOperacao.BuscarPorCodigosAsync(filtrosPesquisa.CodigosTipoOperacao) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            List<Dominio.Entidades.Cliente> tomadores = filtrosPesquisa.CNPJsTomador.Count > 0 ? await repCliente.BuscarPorCPFCNPJsAsync(filtrosPesquisa.CNPJsTomador) : new List<Dominio.Entidades.Cliente>();

            parametros.Add(new Parametro("CentroResultado", string.Join(", ", centrosResultado?.Select(o => o.Descricao))));
            parametros.Add(new Parametro("GrupoPessoa", string.Join(", ", gruposPessoas?.Select(o => o.Descricao))));
            parametros.Add(new Parametro("TipoOperacao", string.Join(", ", tiposOperacao?.Select(o => o.Descricao))));
            parametros.Add(new Parametro("Tomador", string.Join(", ", tiposOperacao?.Select(o => o.Descricao))));
            parametros.Add(new Parametro("DataInicial", filtrosPesquisa.DataInicial > System.DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") : ""));
            parametros.Add(new Parametro("DataFinal", filtrosPesquisa.DataFinal > System.DateTime.MinValue ? filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : ""));
            parametros.Add(new Parametro("NumeroNF", filtrosPesquisa.NumeroNF));
            parametros.Add(new Parametro("NumeroCTe", filtrosPesquisa.NumeroCTe));

            return parametros;
        }



        #endregion
    }
}
