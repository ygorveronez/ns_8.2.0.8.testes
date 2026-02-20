using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.AcertoViagem
{
    [Area("Relatorios")]
	[CustomAuthorize("Acertos/AcertoViagem")]
    public class AcertoFechamentoRelatorioController : BaseController
    {
		#region Construtores

		public AcertoFechamentoRelatorioController(Conexao conexao) : base(conexao) { }

		#endregion


        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R006_FechamentoAcertoViagem;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Nº Frota", "NumeroFrota", 10, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Marca", "Marca", 20, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Modelo", "Modelo", 30, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Renavam", "Renavam", 10, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("KM Inicial", "KMInicial", 8, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("KM Final", "KMFinal", 8, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("KM Total", "KMTotalAjustado", 8, Models.Grid.Align.center, false, true);

            return grid;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao); ;
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Acerto de Viagem", "AcertoViagem", "AcertoViagem.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, false, true);
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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {            
            try
            {         
                int codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                var relatorio = Request.Params("Relatorio");

                await ReportRequest.WithType(ReportType.AcertoFechamento)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("Codigo", codigoAcerto)
                    .AddExtraData("Relatorio", relatorio)
                    .AddExtraData("CodigoUsuario", Usuario.Codigo)
                    .CallReportAsync(cancellationToken: cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }
    }
}
