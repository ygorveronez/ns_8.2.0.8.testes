using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Patrimonio
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Patrimonio/MapaDepreciacao")]
    public class MapaDepreciacaoController : BaseController
    {
		#region Construtores

		public MapaDepreciacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R169_MapaDepreciacao;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo").Nome("Código").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("Descricao").Nome("Descrição").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("NumeroSerie").Nome("Número Série").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("GrupoProduto").Nome("Grupo Produto").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("CentroResultado").Nome("Centro Resultado").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Almoxarifado").Nome("Almoxarifado").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);

            grid.Prop("ValorBem").Nome("Valor Patrimônio").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("PercentualDepreciacao").Nome("% Depreciação").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("MesAno").Nome("Mês e ano").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false); ;
            grid.Prop("ValorDepreciacao").Nome("Valor Depreciação Mes/Ano").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum).Visibilidade(false); ;

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Mapa de Depreciação", "Patrimonio", "MapaDepreciacao.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Descricao", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

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

                List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.MapaDepreciacao> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (from o in listaReport
                             select new
                             {
                                 o.Codigo,
                                 o.Descricao,
                                 o.NumeroSerie,
                                 o.GrupoProduto,
                                 o.CentroResultado,
                                 o.Almoxarifado,
                                 o.ValorBem,
                                 o.PercentualDepreciacao,
                                 o.MesAno,
                                 o.ValorDepreciacao,
                             }).ToList();

                grid.setarQuantidadeTotal(quantidade);
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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                GerarRelatorioAsync(agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, cancellationToken);

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

        private async Task GerarRelatorioAsync(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
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

                List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.MapaDepreciacao> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

               serRelatorio.GerarRelatorioDinamico("Relatorios/Patrimonio/MapaDepreciacao",parametros,relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.MapaDepreciacao> reportResult, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);

            DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
            DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

            int.TryParse(Request.Params("Bem"), out int codigoBem);
            int.TryParse(Request.Params("GrupoProduto"), out int codigoGrupoProduto);
            int.TryParse(Request.Params("Almoxarifado"), out int codigoAlmoxarifado);
            int.TryParse(Request.Params("CentroResultado"), out int codigoCentroResultado);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            #region Parametros
            if (parametros != null)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProdutoTMS = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);
                Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

                if (dataInicial != DateTime.MinValue || dataFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataInicial != DateTime.MinValue ? dataInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFinal != DateTime.MinValue ? "até " + dataFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", false));

                if (codigoBem > 0)
                {
                    Dominio.Entidades.Embarcador.Patrimonio.Bem _bem = repBem.BuscarPorCodigo(codigoBem);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Bem", _bem.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Bem", false));

                if (codigoGrupoProduto > 0)
                {
                    Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS _grupoProduto = repGrupoProdutoTMS.BuscarPorCodigo(codigoGrupoProduto);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", _grupoProduto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", false));

                if (codigoAlmoxarifado > 0)
                {
                    Dominio.Entidades.Embarcador.Frota.Almoxarifado _almoxarifado = repAlmoxarifado.BuscarPorCodigo(codigoAlmoxarifado);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Almoxarifado", _almoxarifado.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Almoxarifado", false));

                if (codigoEmpresa > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

                if (!string.IsNullOrWhiteSpace(propAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", propAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));
            }
            #endregion
            // TODO: ToList cast
            reportResult = repBem.ConsultarRelatorioMapaDepreciacao(codigoEmpresa, dataInicial, dataFinal, codigoBem, codigoGrupoProduto, codigoAlmoxarifado, codigoCentroResultado, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite).ToList();
            quantidade = repBem.ContarConsultaRelatorioMapaDepreciacao(codigoEmpresa, dataInicial, dataFinal, codigoBem, codigoGrupoProduto, codigoAlmoxarifado, codigoCentroResultado, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena);
        }
    }
}
