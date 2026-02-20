using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.AcertoViagem
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/AcertoViagem/DespesaAcertoViagem")]
    public class DespesaAcertoViagemController : BaseController
    {
		#region Construtores

		public DespesaAcertoViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R053_DespesaAcertoViagem;

        #region Metódos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Despesas do Acerto de Viagem", "AcertoViagem", "DespesaAcertoViagem.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Data", "desc", "", "", Codigo, unitOfWork, true, true);
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

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDespesaAcertoViagem filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = grid.header[grid.indiceColunaOrdena].data;

                var listaDespesaAcertoViagem = await repAcertoViagem.RelatorioDespesaAcertoViagem(filtrosPesquisa, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite, cancellationToken: cancellationToken);
                grid.setarQuantidadeTotal(await repAcertoViagem.ContarDespesaAcertoViagem(filtrosPesquisa));

                var lista = (from obj in listaDespesaAcertoViagem
                             select new
                             {
                                 Data = obj.Data != null && obj.Data > DateTime.MinValue ? obj.Data.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.Fornecedor,
                                 obj.NumeroAcerto,
                                 obj.Observacao,
                                 obj.Quantidade,
                                 obj.Valor,
                                 obj.Placa,
                                 DataAcerto = obj.DataAcerto != null && obj.DataAcerto > DateTime.MinValue ? obj.DataAcerto.ToString("dd/MM/yyyy") : string.Empty,
                                 DataInicialAcerto = obj.DataInicialAcerto != null && obj.DataInicialAcerto > DateTime.MinValue ? obj.DataInicialAcerto.ToString("dd/MM/yyyy") : string.Empty,
                                 DataFinalAcerto = obj.DataFinalAcerto != null && obj.DataFinalAcerto > DateTime.MinValue ? obj.DataFinalAcerto.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.Situacao,
                                 obj.Motorista,
                                 obj.ModeloVeiculo,
                                 obj.Justificativa,
                                 Moeda = !obj.MoedaCotacaoBancoCentral.HasValue ? "Reais" : ((Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral)obj.MoedaCotacaoBancoCentral.Value).ObterDescricao(),
                                 obj.ValorMoedaCotacao,
                                 obj.ValorOriginalMoedaEstrangeira,
                                 obj.PaisFornecedor
                             }).ToList();

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
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);


                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorio, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDespesaAcertoViagem filtrosPesquisa = ObterFiltrosPesquisa();

                _ = Task.Factory.StartNew(() => GerarRelatorioDespesaAcertoViagem(unitOfWork.StringConexao, relatorioControleGeracao, filtrosPesquisa, CancellationToken.None), CancellationToken.None);
                return new JsonpResult(true);

            }
            catch (Exception ex)
            {

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }

        }

        #endregion

        #region Metódos Privados

        private async Task GerarRelatorioDespesaAcertoViagem(string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDespesaAcertoViagem filtrosPesquisa, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                ReportRequest.WithType(ReportType.DespesaAcertoViagem)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("FiltrosPesquisa", filtrosPesquisa.ToJson())
                    .AddExtraData("Relatorio", Request.Params("Relatorio"))
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport()
                    .GetContentFile();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Número Acerto", "NumeroAcerto", 6, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Obs/Produto", "Observacao", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Placa", "Placa", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Acerto", "DataAcerto", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Data Inicial", "DataInicialAcerto", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Data Final", "DataFinalAcerto", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Situação Acerto", "Situacao", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Mod. Veículo", "ModeloVeiculo", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Justificativa", "Justificativa", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Moeda", "Moeda", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Cotação", "ValorMoedaCotacao", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor Original Moeda", "ValorOriginalMoedaEstrangeira", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("País", "PaisFornecedor", 8, Models.Grid.Align.left, true, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDespesaAcertoViagem ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDespesaAcertoViagem filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDespesaAcertoViagem()
            {
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoModeloVeicular = Request.GetIntParam("ModeloVeiculo"),
                CodigoVeiculoTracao = Request.GetIntParam("VeiculoTracao"),
                CodigoVeiculoReboque = Request.GetIntParam("VeiculoReboque"),
                CodigoAcertoViagem = Request.GetIntParam("AcertoViagem"),
                CodigoProduto = Request.GetIntParam("Produto"),
                CodigoJustificativa = Request.GetIntParam("Justificativa"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Situacao = Request.GetEnumParam<SituacaoAcertoViagem>("Situacao"),
                CodigoPais = Request.GetListParam<int>("Pais")
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
