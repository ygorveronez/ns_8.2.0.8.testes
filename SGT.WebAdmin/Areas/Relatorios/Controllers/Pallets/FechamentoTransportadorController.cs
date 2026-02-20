using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Entidades.Embarcador.Relatorios;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pallets
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pallets/FechamentoTransportador")]
    public class FechamentoTransportadorController : BaseController
    {
		#region Construtores

		public FechamentoTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R059_FechamentoPalletTransportador;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Fechamento de Pallets por Transportador", "Pallets", "FechamentoTransportador.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Transportador", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                List<int> codigosTransportador = Request.GetListParam<int>("Empresa");
                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                Repositorio.Embarcador.Pallets.EstoquePallet repEstoqueTransportador = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;


                IList<Dominio.Relatorios.Embarcador.DataSource.Pallets.FechamentoTransportador> estoqueTransportador = repEstoqueTransportador.ConsultarRelatario(codigosTransportador, dataInicio, dataFim, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repEstoqueTransportador.ContarConsultaRelatorio(codigosTransportador, dataInicio, dataFim));

                grid.AdicionaRows((
                    from obj in estoqueTransportador
                    select new
                    {
                        obj.CNPJFormatado,
                        obj.Transportador,
                        obj.TransportadorCodigoIntegracao,
                        obj.TotalEntradas,
                        obj.TotalSaidas,
                        obj.DiasDeRotatividade,
                        obj.TotalEmRotatividade,
                        obj.SaldoTotal,
                        obj.SaldoPendente,
                        ValorTotal = obj.ValorTotal.ToString("n2"),
                        Acrescimo = obj.Acrescimo.ToString("n2"),
                        TotalFinal = obj.TotalFinal.ToString("n2"),
                    }
                ).ToList());

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
                List<int> codigosTransportador = Request.GetListParam<int>("Empresa");
                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

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

                _ = Task.Factory.StartNew(() => GerarRelatorioEstoqueTransportador(codigosTransportador, dataInicio, dataFim, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("CNPJ", "CNPJFormatado", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Integração", "TransportadorCodigoIntegracao", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Total de Entradas", "TotalEntradas", 15, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Total de Saídas", "TotalSaidas", 15, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Dias de Rotatividade", "DiasDeRotatividade", 15, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Em Rotatividade", "TotalEmRotatividade", 15, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Saldo Total", "SaldoTotal", 15, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Saldo Pendente", "SaldoPendente", 15, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Total", "ValorTotal", 15, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor de Acrescimo", "Acrescimo", 15, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Final", "TotalFinal", 15, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);


            return grid;
        }

        private async Task GerarRelatorioEstoqueTransportador(List<int> codigosTransportador, DateTime dataInicio, DateTime dataFim, RelatorioControleGeracao relatorioControleGeracao, Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pallets.EstoquePallet repEstoqueTransportador = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Pallets.FechamentoTransportador> dynEstoque = repEstoqueTransportador.ConsultarRelatario(codigosTransportador, dataInicio, dataFim, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao);


                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (codigosTransportador?.Count > 0)
                {
                    if (codigosTransportador.Count == 1)
                    {
                        Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(codigosTransportador.First());
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportador.RazaoSocial, true));
                    }
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", "Múltiplos Registros Selecionados", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                if (dataInicio != DateTime.MinValue || dataFim != DateTime.MinValue)
                {
                    string data = "";
                    data += dataInicio != DateTime.MinValue ? dataInicio.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFim != DateTime.MinValue ? "até " + dataFim.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/FechamentoTransportador",parametros,relatorioControleGeracao, relatorioTemp, dynEstoque, unitOfWork, null, null, true, TipoServicoMultisoftware);
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

        #endregion
    }
}
