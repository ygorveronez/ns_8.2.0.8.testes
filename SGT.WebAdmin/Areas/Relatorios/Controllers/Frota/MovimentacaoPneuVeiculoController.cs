using Dominio.Excecoes.Embarcador;
using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frota
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frota/MovimentacaoPneuVeiculo")]
    public class MovimentacaoPneuVeiculoController : BaseController
    {
		#region Construtores

		public MovimentacaoPneuVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R157_MovimentacaoPneuVeiculo;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Movimentação de Pneus do Veículo", "Frota", "MovimentacaoPneuVeiculo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

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
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                Dominio.Entidades.Veiculo veiculo = await ObterVeiculo(codigoVeiculo, unitOfWork, cancellationToken);
                List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculoDados> listaMovimentacaoPneuVeiculoDados = ObterListaMovimentacaoPneuVeiculoDados(veiculo);

                grid.setarQuantidadeTotal(listaMovimentacaoPneuVeiculoDados.Count);
                grid.AdicionaRows(listaMovimentacaoPneuVeiculoDados);

                return new JsonpResult(grid);
            }
            catch (ControllerException excecao)
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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                bool ocultarDados = Request.GetBoolParam("OcultarDados");
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                string stringConexao = _conexao.StringConexao;

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                _ = Task.Factory.StartNew(() => GerarRelatorio(codigoVeiculo, ocultarDados, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));                

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados


        private async Task GerarRelatorio(int codigoVeiculo, bool ocultarDados, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            try
            {
                Dominio.Entidades.Veiculo veiculo = await ObterVeiculo(codigoVeiculo, unitOfWork, cancellationToken);
                List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculoDados> dataSourceMovimentacaoPneuVeiculoDados = ocultarDados ? new List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculoDados>() : ObterListaMovimentacaoPneuVeiculoDados(veiculo);

                var result = ReportRequest.WithType(ReportType.MovimentacaoPneuVeiculo)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("codigoVeiculo", codigoVeiculo)
                    .AddExtraData("ocultarDados", ocultarDados)
                    .AddExtraData("caminhimagemPneuDesabilitado", Path.GetFullPath("wwwroot/img/Eixos/PneuDesabilitado.png"))
                    .AddExtraData("caminhoimagemEixoDuplo", Path.GetFullPath("wwwroot/img/Eixos/EixoDuploSemPneu.png"))
                    .AddExtraData("caminhoimagemEixoSimples", Path.GetFullPath("wwwroot/img/Eixos/EixoSimplesSemPneu.png"))
                    .AddExtraData("caminhoimagemPneu", Path.GetFullPath("wwwroot/img/Eixos/Pneu.png"))
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("relatorioTemporario", serRelatorio.ObterConfiguracaoRelatorio(relatorioTemporario).ToJson())
                    .AddExtraData("dataSourceMovimentacaoPneuVeiculoDados", dataSourceMovimentacaoPneuVeiculoDados.ToJson())
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result?.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await serRelatorio.RegistrarFalhaGeracaoRelatorioAsync(relatorioControleGeracao, unitOfWork, ex, cancellationToken);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private string ObterCaminhoImagemVeiculo(Repositorio.UnitOfWork unitOfWork)
        {
            string pastaImagemVeiculos = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), "ImagemVeiculos");
            string caminhoImagemVeiculo = Utilidades.IO.FileStorageService.Storage.Combine(pastaImagemVeiculos, $"veiculo_{this.Usuario.Codigo}.png");

            return caminhoImagemVeiculo;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Posição", "Posicao", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Nº de Fogo", "NumeroFogo", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Dimensão", "Dimensao", 12, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Marca", "Marca", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Modelo", "Modelo", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Banda de Rodagem", "BandaRodagem", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Km Rodado", "KmAtualRodado", 12, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Sulco", "Sulco", 12, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Aquisição", "ValorAquisicao", 12, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Vl. Custo", "ValorCustoAtualizado", 12, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Vl. Custo por Km", "ValorCustoKmAtualizado", 12, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Vida Atual", "VidaAtual", 15, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculoDados> ObterListaMovimentacaoPneuVeiculoDados(Dominio.Entidades.Veiculo veiculo)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculoDados> listaMovimentacaoPneuVeiculo = new List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculoDados>();

            listaMovimentacaoPneuVeiculo.AddRange(
                (
                    from veiculoPneu in veiculo.Pneus.OrderBy(o => o.EixoPneu.Eixo.Numero).ThenBy(o => o.EixoPneu.Posicao)
                    select ObterMovimentacaoPneuVeiculoDados(veiculoPneu.Pneu, $"Eixo {veiculoPneu.EixoPneu.Eixo.Numero} {veiculoPneu.EixoPneu.Posicao.ObterDescricao().ToLower()}", veiculoPneu.DataMovimentacao)
                ).ToList()
            );

            listaMovimentacaoPneuVeiculo.AddRange(
                (
                    from veiculoEstepe in veiculo.Estepes.OrderBy(o => o.Estepe.Numero)
                    select ObterMovimentacaoPneuVeiculoDados(veiculoEstepe.Pneu, $"Estepe {veiculoEstepe.Estepe.Numero}", veiculoEstepe.DataMovimentacao)
                ).ToList()
            );

            return listaMovimentacaoPneuVeiculo;
        }

        private Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculoDados ObterMovimentacaoPneuVeiculoDados(Dominio.Entidades.Embarcador.Frota.Pneu pneu, string posicao, DateTime? dataMovimentacao)
        {
            return new Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculoDados()
            {
                Codigo = pneu.Codigo,
                BandaRodagem = pneu.BandaRodagem.Descricao,
                Dimensao = pneu.Modelo.Dimensao.Aplicacao,
                KmAtualRodado = pneu.KmAtualRodado,
                Marca = pneu.Modelo.Marca.Descricao,
                Modelo = pneu.Modelo.Descricao,
                NumeroFogo = pneu.NumeroFogo,
                Posicao = posicao,
                Sulco = pneu.Sulco,
                ValorAquisicao = pneu.ValorAquisicao,
                ValorCustoAtualizado = pneu.ValorCustoAtualizado,
                ValorCustoKmAtualizado = pneu.ValorCustoKmAtualizado,
                VidaAtual = pneu.VidaAtual.ObterDescricao(),
                DataMovimentacao = dataMovimentacao.HasValue && dataMovimentacao.Value > DateTime.MinValue ? dataMovimentacao.Value.ToString("dd/MM/yyyy") : ""
            };
        }

        private async Task<Dominio.Entidades.Veiculo> ObterVeiculo(int codigoVeiculo, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Veiculo repositorio = new Repositorio.Veiculo(unitOfWork, cancellationToken);
            Dominio.Entidades.Veiculo veiculo = await repositorio.BuscarPorCodigoAsync(codigoVeiculo);

            if (veiculo == null)
                throw new ControllerException("Não foi possível encontrar o registro.");

            return veiculo;
        }

        #endregion
    }
}
