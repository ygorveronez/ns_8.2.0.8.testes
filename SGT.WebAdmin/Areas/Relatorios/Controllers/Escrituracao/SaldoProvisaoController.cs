using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Escrituracao
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Escrituracao/SaldoProvisao")]
    public class SaldoProvisaoController : BaseController
    {
		#region Construtores

		public SaldoProvisaoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R170_SaldoProvisao;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Companhia").Nome("Companhia").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("Transportadora").Nome("Transportadora").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("TipoTransporte").Nome("Tipo Transporte").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("Origem").Nome("Cidade Origem").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("Destino").Nome("Cidade Destino").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("Carga").Nome("Viagem").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("DataEmissao").Nome("Data Emissão").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center);
            grid.Prop("Emitente").Nome("Emitente").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("NotaFiscal").Nome("Nota Fiscal").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("CTe").Nome("Conhecimento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("Serie").Nome("Série").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("MeioTransporte").Nome("Meio Transporte").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("CentroCusto").Nome("Centro de Custo").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("DataLancamento").Nome("Data Lançamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center);
            grid.Prop("ContaContabil").Nome("Conta Contábil").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("TipoContabilizacao").Nome("Tipo Contabilização").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("TipoLancamento").Nome("Tipo Lançamento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("ValorLancamento").Nome("Valor Lançamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("Credito").Nome("Crédito").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("Debito").Nome("Débito").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("Aliquota").Nome("Alíquota").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("ValorICMS").Nome("Valor ICMS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("ICMSRetido").Nome("ICMS Retido").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("MotivoCancelamento").Nome("Motivo Cancelamento").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("MesComp").Nome("Mês Comp").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("Filial").Nome("Filial").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("MesCont").Nome("Mês Cont").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("Ano").Nome("Ano").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Saldo Provisão", "Escrituracao", "SaldoProvisao.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Tomador", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
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

                List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.SaldoProvisao> reportResult = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                ExecutarBusca(ref reportResult, ref quantidade, ref parametros, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.setarQuantidadeTotal(quantidade);
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

                List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.SaldoProvisao> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);

                //serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

                //serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Escrituracao/SaldoProvisao", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Escrituracao/SaldoProvisao", parametros, relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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
            if (propOrdena == "Companhia") propOrdena = propOrdena = "NomeCompanhia";
            else if (propOrdena == "Emitente") propOrdena = propOrdena = "NomeEmitente";
            else if (propOrdena == "Transportadora") propOrdena = propOrdena = "NomeTransportadora";
            //else if (propOrdena == "Destinatario") propOrdena = propOrdena = "NomeDestinatario";
            //else if (propOrdena == "NumeroOcorrencia") propOrdena = propOrdena = "Ocorrencia";
            //else if (propOrdena == "TipoContabilizacao") propOrdena = propOrdena = "_TipoContabilizacao";
        }

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.SaldoProvisao> reportResult, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioSaldoProvisao filtro = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioSaldoProvisao()
            {
                //Filial = Request.GetIntParam("Filial"),
                //Transportador = Request.GetIntParam("Transportador"),
                //DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                //DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                //Tomador = Request.GetDoubleParam("Tomador"),
                //CentroResultado = Request.GetListParam<int>("CentroResultado"),
                //DataLancamentoInicial = Request.GetDateTimeParam("DataLancamentoInicial"),
                //DataLancamentoFinal = Request.GetDateTimeParam("DataLancamentoFinal"),
                //ContaContabil = Request.GetListParam<int>("ContaContabil"),
            };

            #region Parametros
            //if (parametros != null)
            //{
            //    if (filtro.DataEmissaoInicial != DateTime.MinValue)
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoInicial", filtro.DataEmissaoInicial.ToString("dd/MM/yyyy"), true));
            //    else
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoInicial", false));

            //    if (filtro.DataEmissaoFinal != DateTime.MinValue)
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFinal", filtro.DataEmissaoFinal.ToString("dd/MM/yyyy"), true));
            //    else
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFinal", false));

            //    if (filtro.DataLancamentoInicial != DateTime.MinValue)
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoInicial", filtro.DataLancamentoInicial.ToString("dd/MM/yyyy"), true));
            //    else
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoInicial", false));

            //    if (filtro.DataLancamentoFinal != DateTime.MinValue)
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoFinal", filtro.DataLancamentoFinal.ToString("dd/MM/yyyy"), true));
            //    else
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoFinal", false));

            //    if (filtro.Filial > 0)
            //    {
            //        var _filial = repFilial.BuscarPorCodigo(filtro.Filial);
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", _filial.Descricao, true));
            //    }
            //    else
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", false));

            //    if (filtro.Transportador > 0)
            //    {
            //        var _transportador = repEmpresa.BuscarPorCodigo(filtro.Transportador);
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", _transportador.Descricao, true));
            //    }
            //    else
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

            //    if (filtro.Tomador > 0)
            //    {
            //        var _tomador = repCliente.BuscarPorCPFCNPJ(filtro.Tomador);
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", _tomador.Descricao, true));
            //    }
            //    else
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", false));

            //    if (filtro.CentroResultado.Count > 0)
            //    {
            //        var _centros = repCentroResultado.BuscarPorCodigos(filtro.CentroResultado);
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", String.Join("; ", from o in _centros select o.BuscarDescricao), true));
            //    }
            //    else
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", false));

            //    if (filtro.ContaContabil.Count > 0)
            //    {
            //        var _contas = repPlanoConta.BuscarPorCodigos(filtro.ContaContabil);
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContaContabil", String.Join("; ", from o in _contas select o.BuscarDescricao), true));
            //    }
            //    else
            //        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContaContabil", false));
            //}
            #endregion

            SetarPropriedadeOrdenacao(ref propOrdena);
            SetarPropriedadeOrdenacao(ref propAgrupa);
            // TODO: ToList cast
            reportResult = repDocumentoEscrituracao.ConsultarRelatorioSaldoProvisao(filtro, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite).ToList();
            quantidade = repDocumentoEscrituracao.ContarConsultaRelatorioSaldoProvisao(filtro, propriedades, propAgrupa, dirAgrupa, propOrdena);
        }
    }
}
