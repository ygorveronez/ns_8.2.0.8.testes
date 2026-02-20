using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/TabelaFreteRota")]
    public class TabelaFreteRotaController : BaseController
    {
		#region Construtores

		public TabelaFreteRotaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R054_TabelaFreteRota;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            bool ordena = true;
            bool agrupa = true;

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, ordena, false, false, false, true);
            grid.AdicionarCabecalho("Codigo Frete", "CodigoEmbarcador", 7, Models.Grid.Align.right, ordena, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", 20, Models.Grid.Align.left, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Pedágio", "Pedagio", 7, Models.Grid.Align.right, ordena, false, false, false, true);
            grid.AdicionarCabecalho("Km", "Km", 5, Models.Grid.Align.right, ordena, false, false, false, true);
            grid.AdicionarCabecalho("Imposto", "Imposto", 7, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Frete Líquido", "ValorFrete", 7, Models.Grid.Align.right, ordena, false, false, false, true);
            grid.AdicionarCabecalho("Frete Bruto", "FreteBruto", 7, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Valor Por Km Líquido", "ValorPorKmLiquido", 10, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Valor Por Km Bruto", "ValorPorKmBruto", 10, Models.Grid.Align.right, false, false, false, false, true);

            return grid;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Tabela de Frete por Rota", "Fretes", "TabelaFreteRota.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", Codigo, unitOfWork, true, false);
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

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                List<Dominio.Relatorios.Embarcador.DataSource.Fretes.TabelaFreteRota> listaGrid = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                SetarPropriedadeOrdenacao(ref propOrdena);

                ExecutarBusca(ref listaGrid, ref quantidade, ref parametros, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(listaGrid);

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

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioAsync(relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        #region Métodos Privados
        private async Task GerarRelatorioAsync(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            try
            {
                List<Dominio.Relatorios.Embarcador.DataSource.Fretes.TabelaFreteRota> listaGrid = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>(); ;
                int quantidade = 0;

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                ExecutarBusca(ref listaGrid, ref quantidade, ref parametros, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Fretes/TabelaFreteRota",parametros,relatorioControleGeracao, relatorioTemp, listaGrid, unitOfWork);

                await unitOfWork.DisposeAsync();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
        }

        private void SetarPropriedadeOrdenacao(ref string propOrdena)
        {
        }

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Fretes.TabelaFreteRota> listaGrid, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            int codOrigem;
            int.TryParse(Request.Params("Origem"), out codOrigem);

            int codDestino;
            int.TryParse(Request.Params("Destino"), out codDestino);

            int codTipoCarga;
            int.TryParse(Request.Params("TipoCarga"), out codTipoCarga);

            int codModeloVeicularCarga;
            int.TryParse(Request.Params("ModeloVeicularCarga"), out codModeloVeicularCarga);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            #region Parametros
            if (parametros != null)
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                Dominio.Entidades.Localidade origem = repLocalidade.BuscarPorCodigo(codOrigem);
                if (origem != null && codOrigem > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", origem.Descricao, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", false));

                Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorCodigo(codDestino);
                if (destino != null && codDestino > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", destino.Descricao, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", false));

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoDeCarga.BuscarPorCodigo(codTipoCarga);
                if (tipoCarga != null)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", tipoCarga.Descricao, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", false));

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculo = repModeloVeicularCarga.BuscarPorCodigo(codModeloVeicularCarga);
                if (modeloVeiculo != null)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", modeloVeiculo.Descricao, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));
            }
            #endregion

            Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga repTabelaFreteRotaTipoCargaModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga(unitOfWork);


            listaGrid = repTabelaFreteRotaTipoCargaModeloVeicularCarga.ConsultarRelatorio(codOrigem, codDestino, codTipoCarga, codModeloVeicularCarga, ativo, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            quantidade = repTabelaFreteRotaTipoCargaModeloVeicularCarga.ContarConsultaRelatorio(codOrigem, codDestino, codTipoCarga, codModeloVeicularCarga, ativo);

        }
        #endregion
    }
}
