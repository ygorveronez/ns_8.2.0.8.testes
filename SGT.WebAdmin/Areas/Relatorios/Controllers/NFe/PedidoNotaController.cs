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
	[CustomAuthorize("Relatorios/NFe/PedidoNota")]
    public class PedidoNotaController : BaseController
    {
		#region Construtores

		public PedidoNotaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R086_PedidoNota;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Nº Nota", "NumeroNota", 3, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Série", "Serie", 2, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 4, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Data Nota", "DataNota", 4, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 10, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("CNPJ Pessoa", "CNPJPessoa", 4, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("Nº Pedidos", "NumerosPedido", 3, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Data Pedidos", "DatasPedido", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tipo Pedidos", "TiposPedido", 4, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Funcionário Pedidos", "FuncionariosPedido", 8, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Vlr. Nota", "ValorNota", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Vlr. Pedidos", "ValorPedidos", 4, Models.Grid.Align.right, true, true);

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pedido x Nota", "NFe", "PedidoNota.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroNota", "asc", "", "", Codigo, unitOfWork, true, true);
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

                int codigoProduto, codigoNota, codigoPedido;
                int.TryParse(Request.Params("Produto"), out codigoProduto);
                int.TryParse(Request.Params("Nota"), out codigoNota);
                int.TryParse(Request.Params("Pedido"), out codigoPedido);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                DateTime dataNotaInicial, dataNotaFinal, dataPedidoInicial, dataPedidoFinal;
                DateTime.TryParseExact(Request.Params("DataNotaInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataNotaInicial);
                DateTime.TryParseExact(Request.Params("DataNotaFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataNotaFinal);
                DateTime.TryParseExact(Request.Params("DataPedidoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPedidoInicial);
                DateTime.TryParseExact(Request.Params("DataPedidoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPedidoFinal);

                Dominio.Enumeradores.StatusNFe statusNota;
                Enum.TryParse(Request.Params("StatusNota"), out statusNota);

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = grid.header[grid.indiceColunaOrdena].data;

                IList<Dominio.Relatorios.Embarcador.DataSource.NFe.PedidoNota> listaPedidoNota = repNotaFiscal.RelatorioPedidoNota(codigoEmpresa, codigoProduto, codigoNota, codigoPedido, cnpjPessoa, statusNota, dataNotaInicial, dataNotaFinal, dataPedidoInicial, dataPedidoFinal, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNotaFiscal.ContarRelatorioPedidoNota(codigoEmpresa, codigoProduto, codigoNota, codigoPedido, cnpjPessoa, statusNota, dataNotaInicial, dataNotaFinal, dataPedidoInicial, dataPedidoFinal));

                var lista = (from obj in listaPedidoNota
                             select new
                             {
                                 obj.NumeroNota,
                                 obj.Serie,
                                 obj.DescricaoStatus,
                                 DataNota = obj.DataNota.ToString("dd/MM/yyyy"),
                                 obj.Pessoa,
                                 obj.CNPJPessoa,
                                 obj.NumerosPedido,
                                 obj.DatasPedido,
                                 obj.TiposPedido,
                                 obj.FuncionariosPedido,
                                 obj.ValorNota,
                                 obj.ValorPedidos
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

                int codigoProduto, codigoNota, codigoPedido;
                int.TryParse(Request.Params("Produto"), out codigoProduto);
                int.TryParse(Request.Params("Nota"), out codigoNota);
                int.TryParse(Request.Params("Pedido"), out codigoPedido);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                DateTime dataNotaInicial, dataNotaFinal, dataPedidoInicial, dataPedidoFinal;
                DateTime.TryParseExact(Request.Params("DataNotaInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataNotaInicial);
                DateTime.TryParseExact(Request.Params("DataNotaFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataNotaFinal);
                DateTime.TryParseExact(Request.Params("DataPedidoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPedidoInicial);
                DateTime.TryParseExact(Request.Params("DataPedidoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPedidoFinal);

                Dominio.Enumeradores.StatusNFe statusNota;
                Enum.TryParse(Request.Params("StatusNota"), out statusNota);

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
                _ = Task.Factory.StartNew(() => GerarRelatorioPedidoNota(codigoEmpresa, codigoProduto, codigoNota, codigoPedido, cnpjPessoa, statusNota, dataNotaInicial, dataNotaFinal, dataPedidoInicial, dataPedidoFinal, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioPedidoNota(int codigoEmpresa, int codigoProduto, int codigoNota, int codigoPedido, double cnpjPessoa, Dominio.Enumeradores.StatusNFe statusNota, DateTime dataNotaInicial, DateTime dataNotaFinal, DateTime dataPedidoInicial, DateTime dataPedidoFinal, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.NFe.PedidoNota> listaPedidoNota = repNotaFiscal.RelatorioPedidoNota(codigoEmpresa, codigoProduto, codigoNota, codigoPedido, cnpjPessoa, statusNota, dataNotaInicial, dataNotaFinal, dataPedidoInicial, dataPedidoFinal, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                //var lista = (from obj in listaPedidoNota
                //             select new
                //             {
                //                 obj.NumeroNota,
                //                 obj.Serie,
                //                 obj.DescricaoStatus,
                //                 DataNota = obj.DataNota.ToString("dd/MM/yyyy"),
                //                 obj.Pessoa,
                //                 obj.CNPJPessoa,
                //                 obj.NumerosPedido,
                //                 obj.DatasPedido,
                //                 obj.TiposPedido,
                //                 obj.FuncionariosPedido,
                //                 obj.ValorNota,
                //                 obj.ValorPedidos
                //             }).ToList();

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedido = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                if (codigoProduto > 0)
                {
                    Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(codigoProduto);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", "(" + produto.Codigo.ToString() + ") " + produto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

                if (codigoNota > 0)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nota = repNotaFiscal.BuscarPorCodigo(codigoNota);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Nota", "(" + nota.Codigo.ToString() + ") " + nota.Numero, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Nota", false));

                if (codigoPedido > 0)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda = repPedido.BuscarPorCodigo(codigoPedido);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", "(" + pedidoVenda.Codigo.ToString() + ") " + pedidoVenda.Numero, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", false));

                if (cnpjPessoa > 0)
                {
                    Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", "(" + pessoa.Codigo.ToString() + ") " + pessoa.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if (dataNotaInicial != DateTime.MinValue || dataNotaFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataNotaInicial != DateTime.MinValue ? dataNotaInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataNotaFinal != DateTime.MinValue ? "até " + dataNotaFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataNota", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataNota", false));

                if (dataPedidoInicial != DateTime.MinValue || dataPedidoFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataPedidoInicial != DateTime.MinValue ? dataPedidoInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataPedidoFinal != DateTime.MinValue ? "até " + dataPedidoFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPedido", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPedido", false));

                if ((int)statusNota > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusNota", statusNota.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusNota", false));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/NFe/PedidoNota",parametros,relatorioControleGeracao, relatorioTemp, listaPedidoNota, unitOfWork, identificacaoCamposRPT);
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
