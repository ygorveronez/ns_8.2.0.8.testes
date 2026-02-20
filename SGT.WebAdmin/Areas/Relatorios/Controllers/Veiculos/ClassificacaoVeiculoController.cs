using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Veiculos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Veiculos/ClassificacaoVeiculo")]
    public class ClassificacaoVeiculoController : BaseController
    {
		#region Construtores

		public ClassificacaoVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R032_ClassificacaoVeiculo;

        private decimal TamanhoColumaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #region Métodos Globais

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Placa, "Placa", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.RENAVAM, "RENAVAM", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.CapacidadeQuilos, "CapacidadeKG", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.MetrosCubicosAbr, "CapacidadeM3", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Tara, "Tara", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Tipo, "DescricaoTipoVeiculo", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.TipoRodado, "DescricaoTipoRodado", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.TipoCarroceria, "DescricaoTipoCarroceria", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.ModeloVeicular, "ModeloVeiculo", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.ModeloCarroceria, "ModeloCarroceria", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.PorcentagemAdicionarFrete, "PercentualAdicionalFrete", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Propriedade, "DescricaoTipoPropriedade", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Estado, "Estado", TamanhoColumaExtraPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.CNPJTransportador, "CNPJTransportador", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Transportador, "Transportador", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, Localization.Resources.Relatorios.Veiculos.Veiculo.RelatorioClassifcacaoVeiculos, "Veiculos", "ClassificacaoVeiculo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "ModeloCarroceria", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarOsDadosDoRelatorio);
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

                int codigoTransportador, codigoModeloVeiculo, codigoModeloCarroceria;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("ModeloVeiculo"), out codigoModeloVeiculo);
                int.TryParse(Request.Params("ModeloCarroceria"), out codigoModeloCarroceria);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao;
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                SetarPropriedadeOrdenacao(ref propOrdena);

                List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ClassificacaoVeiculo> listaClassificacaoVeiculo = await repVeiculo.ConsultarClassificacaoVeiculoAsync(codigoTransportador, codigoModeloVeiculo, codigoModeloCarroceria, situacao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(listaClassificacaoVeiculo.Count);

                grid.AdicionaRows(listaClassificacaoVeiculo);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaAoConsultar);
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
                int codigoTransportador, codigoModeloVeiculo, codigoModeloCarroceria;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("ModeloVeiculo"), out codigoModeloVeiculo);
                int.TryParse(Request.Params("ModeloCarroceria"), out codigoModeloCarroceria);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao;
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
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

                _ = Task.Factory.StartNew(() => GerarRelatorioClassificacaoVeiculo(agrupamentos, codigoTransportador, codigoModeloVeiculo, codigoModeloCarroceria, situacao, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaAoGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioClassificacaoVeiculo(
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
            int codigoTransportador,
            int codigoModeloVeiculo,
            int codigoModeloCarroceria,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp,
            string stringConexao,
            CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Veiculos.ModeloCarroceria repModeloCarroceria = new Repositorio.Embarcador.Veiculos.ModeloCarroceria(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork, cancellationToken);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                SetarPropriedadeOrdenacao(ref propOrdena);

                List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ClassificacaoVeiculo> listaClassificacaoVeiculo = await repVeiculo.ConsultarClassificacaoVeiculoAsync(codigoTransportador, codigoModeloVeiculo, codigoModeloCarroceria, situacao, propOrdena, relatorioTemp.OrdemOrdenacao, 0, 0);

                Dominio.Entidades.Empresa empresaRelatorio = await repEmpresa.BuscarPorCodigoAsync(Empresa.Codigo);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (codigoTransportador > 0)
                {
                    Dominio.Entidades.Empresa empresa = await repEmpresa.BuscarPorCodigoAsync(codigoTransportador);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                if (codigoModeloCarroceria > 0)
                {
                    Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria modeloCarroceria = await repModeloCarroceria.BuscarPorCodigoAsync(codigoModeloCarroceria);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloCarroceria", modeloCarroceria.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloCarroceria", false));

                if (codigoModeloVeiculo > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculo = await repModeloVeiculo.BuscarPorCodigoAsync(codigoModeloVeiculo, cancellationToken);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modeloVeiculo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", false));

                if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? "Ativo" : "Inativo", true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Veiculos/ClassificacaoVeiculo",parametros,relatorioControleGeracao, relatorioTemp, listaClassificacaoVeiculo, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private void SetarPropriedadeOrdenacao(ref string propOrdena)
        {
            if (propOrdena == "RENAVAM")
                propOrdena = "Renavam";
            else if (propOrdena == "DescricaoTipoVeiculo")
                propOrdena = "TipoVeiculo";
            else if (propOrdena == "DescricaoTipoRodado")
                propOrdena = "TipoRodado";
            else if (propOrdena == "DescricaoTipoCarroceria")
                propOrdena = "TipoCarroceria";
            else if (propOrdena == "ModeloVeiculo")
                propOrdena = "ModeloVeicularCarga.Descricao";
            else if (propOrdena == "ModeloCarroceria")
                propOrdena = "ModeloCarroceria.Descricao";
            else if (propOrdena == "PercentualAdicionalFrete")
                propOrdena = "ModeloCarroceria.PercentualAdicionalFrete";
            else if (propOrdena == "DescricaoTipoPropriedade")
                propOrdena = "TipoPropriedade";
            else if (propOrdena == "Estado")
                propOrdena = "Estado.Sigla";
            else if (propOrdena == "CNPJTransportador")
                propOrdena = "Empresa.CNPJ";
            else if (propOrdena == "Transportador")
                propOrdena = "Empresa.RazaoSocial";
            else if (propOrdena == "DescricaoAtivo")
                propOrdena = "Ativo";
        }

        #endregion
    }
}
