using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Transportadores
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Transportadores/AceiteContrato")]
    public class AceiteContratoController : BaseController
    {
		#region Construtores

		public AceiteContratoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R196_AceiteContrato;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Aceite de Contrato", "Transportadores", "AceiteContrato.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
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

                IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.AceiteContrato> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);
                SetarPropriedadeOrdenacao(ref propOrdena);

                Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioAceiteContrato filtrosPesquisa = ObterFiltrosPesquisa();

                if (parametros != null)
                    parametros = await ObterParametrosRelatorio(unitOfWork, filtrosPesquisa, propAgrupa, cancellationToken);

                listaReport = await repEmpresaContrato.ConsultarRelatorioAceiteContratoAsync(filtrosPesquisa, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(listaReport.Count);
                grid.AdicionaRows(listaReport);

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
                SetarPropriedadeOrdenacao(ref propOrdena);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioAsync(agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
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

                IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.AceiteContrato> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioAceiteContrato filtrosPesquisa = ObterFiltrosPesquisa();

                if (parametros != null)
                    parametros = await ObterParametrosRelatorio(unitOfWork, filtrosPesquisa, propAgrupa, cancellationToken);

                listaReport = await repEmpresaContrato.ConsultarRelatorioAceiteContratoAsync(filtrosPesquisa, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Transportadores/AceiteContrato", parametros,relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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
            decimal TamanhoColunasGrande = 20;
            decimal TamanhoColunasPequeno = 3;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("CPFCNPJFormatado").Nome("CNPJ/CPF").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Ord(false);
            grid.Prop("Razao").Nome("Razão").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Cidade").Nome("Cidade").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("DescricaoAceite").Nome("Situação").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).Ord(false);
            grid.Prop("DataAceiteFormatada").Nome("Data Aceite").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("LogAceite").Nome("Log").Tamanho(TamanhoColunasGrande).Align(Models.Grid.Align.left);
            grid.Prop("NomeDoContrato").Nome("Nome do Contrato").Tamanho(TamanhoColunasGrande).Align(Models.Grid.Align.left);


            return grid;
        }

        private async Task<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>> ObterParametrosRelatorio(
            Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioAceiteContrato filtrosPesquisa,
            string propAgrupa,
            CancellationToken cancellationToken)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
            Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork, cancellationToken);

            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoTransportador > 0 ? await repEmpresa.BuscarPorCodigoAsync(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.EmpresaContrato contratoNotaFiscal = filtrosPesquisa.CodigoContratoNotaFiscal > 0 ? await repEmpresaContrato.BuscarPorCodigoAsync(filtrosPesquisa.CodigoContratoNotaFiscal) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa?.RazaoSocial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", propAgrupa));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContratoNotaFiscal", contratoNotaFiscal?.ContratoFormatado));

            return parametros;
        }

        private Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioAceiteContrato ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioAceiteContrato()
            {
                CodigoTransportador = Request.GetIntParam("Transportador"),
                Situacao = Request.GetEnumParam<SituacaoAceiteContrato>("Situacao"),
                CodigoContratoNotaFiscal = Request.GetIntParam("ContratoNotaFiscal")
            };
        }

        private void SetarPropriedadeOrdenacao(ref string propOrdena)
        {
            if (propOrdena.Contains("Formatada"))
                propOrdena = propOrdena.Replace("Formatada", "");
            else if (propOrdena.Contains("Formatado"))
                propOrdena = propOrdena.Replace("Formatado", "");
        }

        #endregion
    }
}
