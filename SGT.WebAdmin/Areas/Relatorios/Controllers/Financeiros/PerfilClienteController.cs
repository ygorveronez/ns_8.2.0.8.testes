using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/PerfilCliente")]
    public class PerfilClienteController : BaseController
    {
		#region Construtores

		public PerfilClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R087_PerfilCliente;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("CNPJ Pessoa", "CNPJPessoa", 4, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 10, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Telefone", "Telefone", 4, Models.Grid.Align.left, true, false);

            grid.AdicionarCabecalho("Média Compras", "MediaCompras", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Maior Compra", "MaiorCompra", 4, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Última Compra", "UltimaCompra", 4, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Data Última Compra", "DataUltimaCompra", 4, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Vencimento Última Compra", "VencimentoUltimaCompra", 4, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Próximo Vencimento", "ProximoVencimento", 4, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Vlr. Próximo Vencimento", "ValorProximoVencimento", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Data Primeira Compra", "DataPrimeiraCompra", 4, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Total Pago", "TotalPago", 4, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Total Geral", "TotalGeral", 4, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Total Pendente", "TotalPendente", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Total a Vencer", "TotalVencer", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Total Vencido", "TotalVencido", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Qtd. Títulos", "QuantidadeTitulos", 4, Models.Grid.Align.right, true, false);

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Perfil de Cliente", "Financeiros", "PerfilCliente.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Pessoa", "asc", "", "", Codigo, unitOfWork, true, true);
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

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                string estado = Request.Params("Estado");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = grid.header[grid.indiceColunaOrdena].data;

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PerfilCliente> listaPerfilClientes = repTitulo.RelatorioPerfilCliente(codigoEmpresa, cnpjPessoa, estado, tipoAmbiente, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarRelatorioPerfilCliente(codigoEmpresa, cnpjPessoa, estado, tipoAmbiente));

                var lista = (from obj in listaPerfilClientes
                             select new
                             {
                                 obj.CNPJPessoa,
                                 obj.Pessoa,
                                 obj.Telefone,
                                 obj.MediaCompras,
                                 obj.MaiorCompra,
                                 obj.UltimaCompra,
                                 DataUltimaCompra = obj.DataUltimaCompra.ToString("dd/MM/yyyy"),
                                 VencimentoUltimaCompra = obj.VencimentoUltimaCompra.ToString("dd/MM/yyyy"),
                                 ProximoVencimento = obj.ProximoVencimento != DateTime.MinValue ? obj.ProximoVencimento.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.ValorProximoVencimento,
                                 DataPrimeiraCompra = obj.DataPrimeiraCompra.ToString("dd/MM/yyyy"),
                                 obj.TotalPago,
                                 obj.TotalGeral,
                                 obj.TotalPendente,
                                 obj.TotalVencer,
                                 obj.TotalVencido,
                                 obj.QuantidadeTitulos
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

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                string estado = Request.Params("Estado");

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

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
                _ = Task.Factory.StartNew(() => GerarRelatorioPerfilCliente(codigoEmpresa, cnpjPessoa, estado, tipoAmbiente, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioPerfilCliente(int codigoEmpresa, double cnpjPessoa, string estado, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PerfilCliente> listaPerfilClientes = repTitulo.RelatorioPerfilCliente(codigoEmpresa, cnpjPessoa, estado, tipoAmbiente, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                //var lista = (from obj in listaPerfilClientes
                //             select new
                //             {
                //                 obj.CNPJPessoa,
                //                 obj.Pessoa,
                //                 obj.Telefone,
                //                 obj.MediaCompras,
                //                 obj.MaiorCompra,
                //                 obj.UltimaCompra,
                //                 DataUltimaCompra = obj.DataUltimaCompra.ToString("dd/MM/yyyy"),
                //                 VencimentoUltimaCompra = obj.VencimentoUltimaCompra.ToString("dd/MM/yyyy"),
                //                 ProximoVencimento = obj.ProximoVencimento != DateTime.MinValue ? obj.ProximoVencimento.ToString("dd/MM/yyyy") : string.Empty,
                //                 obj.ValorProximoVencimento,
                //                 DataPrimeiraCompra = obj.DataPrimeiraCompra.ToString("dd/MM/yyyy"),
                //                 obj.TotalPago,
                //                 obj.TotalGeral,
                //                 obj.TotalPendente,
                //                 obj.TotalVencer,
                //                 obj.TotalVencido,
                //                 obj.QuantidadeTitulos
                //             }).ToList();

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                if (cnpjPessoa > 0)
                {
                    Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", "(" + pessoa.Codigo.ToString() + ") " + pessoa.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if (!string.IsNullOrWhiteSpace(estado))
                {
                    Dominio.Entidades.Estado uf = repEstado.BuscarPorSigla(estado);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Estado", "(" + uf.Sigla.ToString() + ") " + uf.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Estado", false));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Financeiros/PerfilCliente", parametros,relatorioControleGeracao, relatorioTemp, listaPerfilClientes, unitOfWork, identificacaoCamposRPT);
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
