using Newtonsoft.Json;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/ComponenteFreteCTe")]
    public class ComponenteFreteCTeController : BaseController
    {
		#region Construtores

		public ComponenteFreteCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R137_ComponenteFreteCTe;

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Componentes de Frete do CT-e", "CTe", "ComponenteFreteCTe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "ComponenteFrete", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

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
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                string propriedadeAgrupar = grid.group.enable ? grid.group.propAgrupa : "";
                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propriedadeOrdenar, string.Empty);
                propriedadeAgrupar = ObterPropriedadeOrdenarOuAgrupar(propriedadeAgrupar);
                propriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(propriedadeOrdenar);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork, cancellationToken);

                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ComponenteFreteCTe.ComponenteFreteCTe> listaPedido =
                   await repCargaCTeComponenteFrete.ConsultarRelatorioComponenteFreteCTeAsync(filtrosPesquisa, propriedades, propriedadeAgrupar, grid.group.dirOrdena, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = await repCargaCTeComponenteFrete.ContarConsultaRelatorioComponenteFreteCTeAsync(filtrosPesquisa, propriedades, propriedadeAgrupar, grid.group.dirOrdena, propriedadeOrdenar, grid.dirOrdena);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaPedido);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                string propriedadeOrdenar = relatorioTemporario.PropriedadeOrdena;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propriedadeOrdenar, relatorioTemporario.PropriedadeAgrupa);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioComponenteFreteCTeAsync(filtrosPesquisa, agrupamentos, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioComponenteFreteCTeAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe filtrosPesquisa,
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                string propriedadeAgrupar = ObterPropriedadeOrdenarOuAgrupar(relatorioTemporario.PropriedadeAgrupa);
                string propriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(relatorioTemporario.PropriedadeOrdena);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = await ObterParametrosAsync(filtrosPesquisa, unitOfWork, cancellationToken);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork, cancellationToken);

                Dominio.Entidades.Empresa empresaRelatorio = await repEmpresa.BuscarPorCodigoAsync(Empresa.Codigo);

                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ComponenteFreteCTe.ComponenteFreteCTe> listaRelatorio = await repCargaCTeComponenteFrete.ConsultarRelatorioComponenteFreteCTeAsync(filtrosPesquisa, propriedades, propriedadeAgrupar, relatorioTemporario.OrdemAgrupamento, propriedadeOrdenar, relatorioTemporario.OrdemOrdenacao, inicio: 0, limite: 0);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/CTe/ComponenteFreteCTe", parametros, relatorioControleGeracao, relatorioTemporario, listaRelatorio, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe()
            {
                Carga = Request.GetIntParam("Carga"),
                ComponenteFrete = JsonConvert.DeserializeObject<List<int>>(Request.Params("ComponenteFrete")),
                CTe = Request.GetIntParam("CTe"),
                DataFinalAutorizacao = Request.GetDateTimeParam("DataFinalAutorizacao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                DataInicialAutorizacao = Request.GetDateTimeParam("DataInicialAutorizacao"),
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                Empresa = Request.GetIntParam("Empresa"),
                GrupoPessoas = JsonConvert.DeserializeObject<List<int>>(Request.Params("GrupoPessoas")),
                ModeloDocumento = JsonConvert.DeserializeObject<List<int>>(Request.Params("ModeloDocumento")),
                StatusCTe = JsonConvert.DeserializeObject<List<string>>(Request.Params("Situacao") ?? "")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            decimal TamanhoColunasValores = 3;
            decimal TamanhoColunasDescritivo = (decimal)4.50;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Componente de Frete", "ComponenteFrete", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Empresa/Filial", "Empresa", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Modelo de Documento", "ModeloDocumento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e", "NumeroCTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Série do CT-e", "SerieCTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor do Componente", "ValorComponenteFrete", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Modelo Veicular Carga", "ModeloVeicularCarga", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Frotas", "NumeroFrotasVeiculos", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso do CT-e", "Peso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Emissão do CT-e", "DataEmissaoCTeFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Destinatário do CT-e", "DestinatarioCTe", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        private async Task<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>> ObterParametrosAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe filtrosPesquisa,
            Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork, cancellationToken);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork, cancellationToken);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork, cancellationToken);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.Carga > 0 ? await repCarga.BuscarPorCodigoAsync(filtrosPesquisa.Carga) : null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = filtrosPesquisa.CTe > 0 ? await repCTe.BuscarPorCodigoAsync(filtrosPesquisa.CTe, false) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.Empresa > 0 ? await repEmpresa.BuscarPorCodigoAsync(filtrosPesquisa.Empresa) : null;

            List<string> grupoPessoas = filtrosPesquisa.GrupoPessoas != null && filtrosPesquisa.GrupoPessoas.Count > 0 ? await repGrupoPessoas.BuscarDescricaoPorCodigoAsync(filtrosPesquisa.GrupoPessoas) : null;
            IList<string> modeloDocumento = filtrosPesquisa.ModeloDocumento != null && filtrosPesquisa.ModeloDocumento.Count > 0 ? await repModeloDocumentoFiscal.BuscarDescricaoPorCodigoAsync(filtrosPesquisa.ModeloDocumento) : null;
            List<string> situacoesCTe = filtrosPesquisa.StatusCTe != null && filtrosPesquisa.StatusCTe.Count > 0 ? (from obj in filtrosPesquisa.StatusCTe select Servicos.Embarcador.CTe.CTe.ObterDescricaoSituacao(obj)).ToList() : null;
            List<string> componenteFrete = filtrosPesquisa.ComponenteFrete != null && filtrosPesquisa.ComponenteFrete.Count > 0 ? await repComponenteFrete.BuscarDescricaoPorCodigoAsync(filtrosPesquisa.ComponenteFrete.ToArray()) : null;

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialAutorizacao", filtrosPesquisa.DataInicialAutorizacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalAutorizacao", filtrosPesquisa.DataFinalAutorizacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialEmissao", filtrosPesquisa.DataInicialEmissao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalEmissao", filtrosPesquisa.DataFinalEmissao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.RazaoSocial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", carga?.CodigoCargaEmbarcador));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", cte?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloDocumento", modeloDocumento));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ComponenteFrete", componenteFrete));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCTe", situacoesCTe));

            return parametros;
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.EndsWith("Formatado"))
                return propriedadeOrdenar.Replace("Formatado", "");
            else if (propriedadeOrdenar.EndsWith("Formatada"))
                return propriedadeOrdenar.Replace("Formatada", "");

            return propriedadeOrdenar;
        }

        #endregion
    }
}
