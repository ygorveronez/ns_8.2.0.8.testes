using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/TransportadoresSemContrato")]
    public class TransportadoresSemContratoController : BaseController
    {
		#region Construtores

		public TransportadoresSemContratoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R106_TransportadoresSemContrato;

        private Models.Grid.Grid GridPadrao()
        {
            decimal TamanhoColunasMedia = 3.50m;
            decimal TamanhoColunasDescritivos = 5.50m;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Número Último Contrato", "NumeroUltimoContrato", TamanhoColunasMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Vigência Último Contrato", "Vigencia", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Último Contrato", "TipoUltimoContrato", TamanhoColunasMedia, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Razao Social", "RazaoSocial", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nome Fantasia", "NomeFantasia", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("CNPJ", "CNPJ", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", TamanhoColunasMedia, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Inscrição Estadual", "InscricaoEstadual", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Situação Transportador", "Situacao", TamanhoColunasMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação Contrato", "DescricaoSituacaoContrato", TamanhoColunasMedia, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Transportadores Sem Contrato de Frete", "Fretes", "TransportadoresSemContrato.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "RazaoSocial", "desc", "", "", codigoRelatorio, unitOfWork, false, true);
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
                IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores> reportResult = null;
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);
                DateTime.TryParse(Request.Params("DataInicio"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFim"), out DateTime dataFinal);
                Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao);
                bool.TryParse(Request.Params("SomenteContrato"), out bool somenteContrato);
                bool.TryParse(Request.Params("SomenteSemContrato"), out bool somenteSemContrato);
                reportResult = await ExecutarBusca(propriedades, dataInicial, dataFinal, situacao, somenteContrato, somenteSemContrato, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork, cancellationToken);
                grid.setarQuantidadeTotal(reportResult.Count);
                grid.AdicionaRows(reportResult);
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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioAsync(agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private async Task GerarRelatorioAsync(
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp,
            string stringConexao,
            CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                List<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork, cancellationToken);

                DateTime.TryParse(Request.Params("DataInicio"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFim"), out DateTime dataFinal);

                Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao);

                bool.TryParse(Request.Params("SomenteContrato"), out bool somenteContrato);
                bool.TryParse(Request.Params("SomenteSemContrato"), out bool somenteSemContrato);

                SetarPropriedadeOrdenacao(ref propOrdena);
                SetarPropriedadeOrdenacao(ref propAgrupa);

                var reportResult = await repContratoFreteTransportador.ConsultarRelatorioTransportadoresSemContratoAsync(dataInicial, dataFinal, situacao, somenteContrato, somenteSemContrato, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Fretes/TransportadoresSemContrato", parametros,relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
            }
            catch (Exception ex)
            {
                await serRelatorio.RegistrarFalhaGeracaoRelatorioAsync(relatorioControleGeracao, unitOfWork, ex, cancellationToken);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private void SetarPropriedadeOrdenacao(ref string propOrdena)
        {
            if (propOrdena == "DescricaoSituacao")
                propOrdena = "Situacao";
            else if (propOrdena == "Vigencia")
                propOrdena = "DataFinal";
        }

        private async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores>> ExecutarBusca(
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
            DateTime dataInicial,
            DateTime dataFinal,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao,
            bool somenteContrato,
            bool somenteSemContrato,
            string propAgrupa,
            string dirAgrupa,
            string propOrdena,
            string dirOrdena,
            int inicio,
            int limite,
            Repositorio.UnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork, cancellationToken);

            SetarPropriedadeOrdenacao(ref propOrdena);
            SetarPropriedadeOrdenacao(ref propAgrupa);

            return await repContratoFreteTransportador.ConsultarRelatorioTransportadoresSemContratoAsync( dataInicial, dataFinal, situacao, somenteContrato, somenteSemContrato, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
        }

    }
}
