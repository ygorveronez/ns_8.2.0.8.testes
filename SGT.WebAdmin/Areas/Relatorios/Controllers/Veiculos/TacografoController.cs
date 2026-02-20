using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Veiculos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Veiculos/Tacografo")]
    public class TacografoController : BaseController
    {
		#region Construtores

		public TacografoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R281_Tacografo;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, Localization.Resources.Relatorios.Veiculos.Veiculo.RelatorioTacógrafo, "Veiculos", "Tacografo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
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
                Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork, cancellationToken);
                IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Tacografo> listaReport = null;

                string propOrdena = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioTacografo filtrosPesquisa = ObterFiltrosPesquisa();

                var parametros = GetParametros(unitOfWork, ObterFiltrosPesquisa(), propAgrupa);
                listaReport = await repControleTacografo.ConsultarRelatorioTacografoAsync(filtrosPesquisa, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(listaReport.Count);
                grid.AdicionaRows(listaReport);

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
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                relatorioTemp.PropriedadeOrdena = ObterPropriedadeOrdenar(relatorioTemp.PropriedadeOrdena);
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

                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaAoGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> GetParametros(
            Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioTacografo filtrosPesquisa,
                        string propAgrupa)
        {
            var parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                if (filtrosPesquisa.CodigosVeiculos != null && filtrosPesquisa.CodigosVeiculos.Count > 0)
                {
                    List<Dominio.Entidades.Veiculo> veiculos = filtrosPesquisa.CodigosVeiculos.Count > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosVeiculos.ToArray()) : new List<Dominio.Entidades.Veiculo>();
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", string.Join(", ", from obj in veiculos select obj.Placa_Formatada), true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

                if (filtrosPesquisa.CodigosMotoristas != null && filtrosPesquisa.CodigosMotoristas.Count > 0)
                {
                    List<Dominio.Entidades.Usuario> funcionarios = filtrosPesquisa.CodigosMotoristas.Count > 0 ? repUsuario.BuscarUsuariosPorCodigos(filtrosPesquisa.CodigosMotoristas.ToArray(), null) : new List<Dominio.Entidades.Usuario>();
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", string.Join(", ", from obj in funcionarios select obj.Nome), true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

                if (!string.IsNullOrWhiteSpace(propAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", propAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                return parametros;
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
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Empresa empresaRelatorio = await repEmpresa.BuscarPorCodigoAsync(Empresa.Codigo);

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Tacografo> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                parametros = GetParametros(unitOfWork, ObterFiltrosPesquisa(), propAgrupa);
                Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioTacografo filtrosPesquisa = ObterFiltrosPesquisa();

                listaReport = await repControleTacografo.ConsultarRelatorioTacografoAsync(filtrosPesquisa, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Veiculos/Tacografo", parametros,relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
                
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

        private Models.Grid.Grid GridPadrao()
        {
            decimal TamanhoColunasMedia = 5;
            decimal TamanhoColunasDescritivos = 10;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };
            
            grid.Prop("Codigo").Nome(Localization.Resources.Relatorios.Veiculos.Veiculo.NumeroDisco).Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("Placa").Nome(Localization.Resources.Relatorios.Veiculos.Veiculo.Placa).Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Ord(true);
            grid.Prop("Motorista").Nome(Localization.Resources.Gerais.Geral.Motorista).Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("SituacaoFormatada").Nome(Localization.Resources.Gerais.Geral.Situacao).Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Ord(true);            
            grid.Prop("DataRepasseFormatada").Nome(Localization.Resources.Relatorios.Veiculos.Veiculo.DataRepasse).Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("DataRetornoFormatada").Nome(Localization.Resources.Relatorios.Veiculos.Veiculo.DataRetorno).Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("HouveExcessoVelocidadeFormatada").Nome(Localization.Resources.Relatorios.Veiculos.Veiculo.HouveExcessoVelocidade).Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("Observacao").Nome(Localization.Resources.Gerais.Geral.Observacao).Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioTacografo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioTacografo()
            {
                CodigosMotoristas = Request.GetListParam<int>("Motorista"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                Situacoes = Request.GetListParam<int>("Situacao"),
                DataInicialRepasse = Request.GetDateTimeParam("DataInicialRepasse"),
                DataFinalRepasse = Request.GetDateTimeParam("DataFinalRepasse"),
                DataInicialRetorno = Request.GetDateTimeParam("DataInicialRetorno"),
                DataFinalRetorno = Request.GetDateTimeParam("DataFinalRetorno"),
                ExcessoVelocidade = Request.GetEnumParam<SituacaoAtivoPesquisa>("ExcessoVelocidade")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Contains("Formatada"))
                return propriedadeOrdenar.Replace("Formatada", "");

            return propriedadeOrdenar;
        }

        #endregion
    }
}
