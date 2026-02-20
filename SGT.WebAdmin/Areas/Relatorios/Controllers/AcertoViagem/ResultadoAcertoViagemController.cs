using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.AcertoViagem
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/AcertoViagem/ResultadoAcertoViagem")]
    public class ResultadoAcertoViagemController : BaseController
    {
		#region Construtores

		public ResultadoAcertoViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R009_ResultadoAcertoViagem;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Resultado do Acerto de Viagem", "AcertoViagem", "ResultadoAcertoViagem.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);
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

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAcertoViagem filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.AcertoViagem.ResultadoAcertoViagem servicoRelatorioResultadoAcertoViagem = new Servicos.Embarcador.Relatorios.AcertoViagem.ResultadoAcertoViagem(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioResultadoAcertoViagem.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ResultadoAcertoViagem> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAcertoViagem filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Cavalo", "PlacaTracao", 4, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Veículo", "ModeloTracao", 4, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Segmento", "Segmento", 4, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Pessoa", "GrupoPessoa", 4, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Modelo Veícular", "ModeloVeicular", 4, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Marca Veículo", "MarcaVeiculo", 4, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Ano", "AnoTracao", 4, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Nº Frota", "FrotaTracao", 4, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Nº Viagem", "NumeroAcertoViagem", 4, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Dias", "QuantidadeDias", 4, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Data Final", "DataFinal", 4, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Motorista", "NomeMotorista", 4, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Fat. Bruto", "FaturamentoBruto", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Res. Liquido", "ResultadoLiquido", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("ICMS", "ValorICMS", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Comb. Cavalo", "CombustivelTracao", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Comb. Cavalo/KM", "CombustivelTracaoKm", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Comb. TK", "CombustivelEquipamentos", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Comb. TK/KM", "CombustivelEquipamentosKm", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("KM", "KMTotal", 4, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Val. KM S/ ICMS", "ValorKMSemICMS", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Val. KM C/ ICMS", "ValorKMComICMS", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Tot. Mês C/ ICMS", "TotalMesComICMS", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Diesel / KM", "DieselKM", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Parametro Média", "ParametroMedia", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Média", "Media", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Obs", "Observacao", 4, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Ocorrências", "Ocorrencias", 4, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("LT Cavalo", "LitrosTracao", 4, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("LT TK", "LitrosReboque", 4, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Média LT Cavalo", "MediaLitroCavalo", 4, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Média LT TK", "MediaLitroReboque", 4, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Pedágio Pago", "PedagioPago", 4, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Pedágio Recebido", "PedagioRecebido", 4, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Cód Integração Motorista", "CodigoIntegracaoMotorista", 4, Models.Grid.Align.right, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAcertoViagem ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioResultadoAcertoViagem()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Motorista = Request.GetIntParam("Motorista"),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                ModeloVeiculo = Request.GetIntParam("ModeloVeiculo"),
                SegmentoVeiculo = Request.GetIntParam("SegmentoVeiculo"),
                VeiculoTracao = Request.GetIntParam("VeiculoTracao"),
                VeiculoReboque = Request.GetIntParam("VeiculoReboque"),
            };
        }

        #endregion
    }
}
