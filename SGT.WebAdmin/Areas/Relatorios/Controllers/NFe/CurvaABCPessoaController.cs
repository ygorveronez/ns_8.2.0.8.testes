using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/NFe/CurvaABCPessoa")]
    public class CurvaABCPessoaController : BaseController
    {
		#region Construtores

		public CurvaABCPessoaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R078_CurvaABCPessoa;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Posição", "Posicao", 2, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Fone", "Telefone", 4, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Qtd. Notas", "Quantidade", 3, Models.Grid.Align.right, false, false, false, true, true);
            grid.AdicionarCabecalho("Vlr. Tot. Notas", "Valor", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("% Contribuição", "Contribuicao", 4, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("% Acumulado", "Acumulado", 4, Models.Grid.Align.right, false, true);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Curva ABC por Pessoas", "NFe", "CurvaABCPessoa.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Posicao", "asc", "", "", Codigo, unitOfWork, true, true);
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

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCompraVenda tipoMovimento;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrdenacaoCurvaABC ordemCurva;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);
                Enum.TryParse(Request.Params("Ordenar"), out ordemCurva);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = grid.header[grid.indiceColunaOrdena].data;

                IList<Dominio.Relatorios.Embarcador.DataSource.NFe.CurvaABCPessoa> listaCurvaABC = repNotaFiscal.RelatorioCurvaABCPessoa(codigoEmpresa, tipoMovimento, ordemCurva, dataInicial, dataFinal, tipoAmbiente, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNotaFiscal.ContarRelatorioCurvaABCPessoa(codigoEmpresa, tipoMovimento, ordemCurva, dataInicial, dataFinal, tipoAmbiente));

                int posicao = 0;
                decimal acumulador = 0;
                for (int i = 0; i < listaCurvaABC.Count; i++)
                {
                    posicao = posicao + 1;
                    listaCurvaABC[i].Posicao = posicao;
                    acumulador = acumulador + listaCurvaABC[i].Contribuicao;
                    listaCurvaABC[i].Acumulado = acumulador;
                }

                var lista = (from obj in listaCurvaABC
                             select new
                             {
                                 obj.Posicao,
                                 obj.Pessoa,
                                 obj.Telefone,
                                 obj.Quantidade,
                                 obj.Valor,
                                 obj.Contribuicao,
                                 obj.Acumulado
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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCompraVenda tipoMovimento;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrdenacaoCurvaABC ordemCurva;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);
                Enum.TryParse(Request.Params("Ordenar"), out ordemCurva);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

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
                _ = Task.Factory.StartNew(() => GerarRelatorioCurvaABCPessoa(codigoEmpresa, tipoAmbiente, tipoMovimento, ordemCurva, dataInicial, dataFinal, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioCurvaABCPessoa(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCompraVenda tipoMovimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrdenacaoCurvaABC ordenar, DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.NFe.CurvaABCPessoa> listaCurvaABC = repNotaFiscal.RelatorioCurvaABCPessoa(codigoEmpresa, tipoMovimento, ordenar, dataInicial, dataFinal, tipoAmbiente, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                int posicao = 0;
                decimal acumulador = 0;
                for (int i = 0; i < listaCurvaABC.Count; i++)
                {
                    posicao = posicao + 1;
                    listaCurvaABC[i].Posicao = posicao;
                    acumulador = acumulador + listaCurvaABC[i].Contribuicao;
                    listaCurvaABC[i].Acumulado = acumulador;
                }

                //var lista = (from obj in listaCurvaABC
                //             select new
                //             {
                //                 obj.Posicao,
                //                 obj.Pessoa,
                //                 obj.Telefone,
                //                 obj.Quantidade,
                //                 obj.Valor,
                //                 obj.Contribuicao,
                //                 obj.Acumulado
                //             }).ToList();

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if ((int)tipoMovimento > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMovimento", tipoMovimento.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMovimento", false));

                if ((int)ordenar > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ordenacao", ordenar.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ordenacao", false));

                if (dataInicial != DateTime.MinValue || dataFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataInicial != DateTime.MinValue ? dataInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFinal != DateTime.MinValue ? "até " + dataFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", false));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/NFe/CurvaABCPessoa",parametros,relatorioControleGeracao, relatorioTemp, listaCurvaABC, unitOfWork, identificacaoCamposRPT);
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
    }
}
