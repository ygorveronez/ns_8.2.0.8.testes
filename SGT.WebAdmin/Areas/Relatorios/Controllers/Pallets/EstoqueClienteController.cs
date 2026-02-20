using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Entidades.Embarcador.Relatorios;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pallets
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pallets/EstoqueCliente")]
    public class EstoqueClienteController : BaseController
    {
		#region Construtores

		public EstoqueClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R164_Pallets_Estoque_Cliente;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                var codigoRelatorio = Request.GetIntParam("Codigo");
                var serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                var relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Estoque de Pallets por Cliente", "Pallets", "EstoqueCliente.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                var gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

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

                var cliente = Request.GetDoubleParam("Cliente");
                var dataInicio = Request.GetNullableDateTimeParam("DataInicio");
                var dataFim = Request.GetNullableDateTimeParam("DataFim");
                var codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");

                var parametrosConsulta = grid.ObterParametrosConsulta();
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeOrdenar);
                var repositorioEstoque = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);
                var movimentacaoEstoquePalletsCliente = repositorioEstoque.ConsultarCliente(cliente, dataInicio, dataFim, codigoGrupoPessoas, parametrosConsulta);

                grid.setarQuantidadeTotal(repositorioEstoque.ContarConsultaCliente(cliente, dataInicio, dataFim, codigoGrupoPessoas));

                grid.AdicionaRows((
                    from movimentacao in movimentacaoEstoquePalletsCliente
                    select new
                    {
                        movimentacao.Codigo,
                        Data = movimentacao.Data.ToString("dd/MM/yyyy HH:mm"),
                        Cliente = movimentacao.Cliente?.Nome,
                        ClienteCpfCnpj = movimentacao.Cliente?.CPF_CNPJ_Formatado,
                        ClienteCodigoIntegracao = movimentacao.Cliente?.CodigoIntegracao,
                        TipoLancamento = movimentacao.ObterTipoLancamento(),
                        movimentacao.Observacao,
                        Entrada = movimentacao.ObterQuantidadeEntrada(),
                        Saida = movimentacao.ObterQuantidadeSaida(),
                        Descarte = (movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada ? movimentacao.QuantidadeDescartada : movimentacao.QuantidadeDescartada * -1),
                        movimentacao.SaldoTotal,
                        GrupoPessoas = movimentacao.GrupoPessoas?.Descricao ?? ""
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
                var cpfCnpjCliente = Request.GetDoubleParam("Cliente");
                var dataInicio = Request.GetNullableDateTimeParam("DataInicio");
                var dataFim = Request.GetNullableDateTimeParam("DataFim");
                var codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                var dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                var relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                var relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                var relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                var mdlRelatorio = new Models.Grid.Relatorio();
                var grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                var propOrdena = relatorioTemp.PropriedadeOrdena;
                var stringConexao = _conexao.StringConexao;
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                _ = Task.Factory.StartNew(() => GerarRelatorioEstoqueCliente(agrupamentos, cpfCnpjCliente, dataInicio, dataFim, codigoGrupoPessoas, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "Data", 12, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Cliente", "Cliente", 20, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ", "ClienteCpfCnpj", 13, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Integração", "ClienteCodigoIntegracao", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Lançamento", "TipoLancamento", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Entrada", "Entrada", 6, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Saída", "Saida", 6, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Descarte", "Descarte", 6, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Saldo Total", "SaldoTotal", 6, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", 12, Models.Grid.Align.center, false, false, false, false, false);

            return grid;
        }

        private async Task GerarRelatorioEstoqueCliente(List<PropriedadeAgrupamento> agrupamentos, double cpfCnpjCliente, DateTime? dataInicio, DateTime? dataFim, int codigoGrupoPessoas, RelatorioControleGeracao relatorioControleGeracao, Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                var repositorioEstoque = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);
                var parametrosConsulta = relatorioTemp.ObterParametrosConsulta();
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeOrdenar);
                var movimentacaoEstoquePalletsCliente = repositorioEstoque.ConsultarCliente(cpfCnpjCliente, dataInicio, dataFim, codigoGrupoPessoas, parametrosConsulta);

                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.EstoqueCliente> dynEstoque = (
                    from movimentacao in movimentacaoEstoquePalletsCliente
                    select new Dominio.Relatorios.Embarcador.DataSource.Pallets.EstoqueCliente()
                    {
                        Codigo = movimentacao.Codigo,
                        Data = movimentacao.Data,
                        Cliente = movimentacao.Cliente?.Nome,
                        ClienteCodigoIntegracao = movimentacao.Cliente?.CodigoIntegracao,
                        ClienteCpfCnpj = movimentacao.Cliente?.CPF_CNPJ_Formatado,
                        TipoLancamento = movimentacao.ObterTipoLancamento(),
                        Observacao = movimentacao.Observacao,
                        Entrada = movimentacao.ObterQuantidadeEntrada(),
                        Saida = movimentacao.ObterQuantidadeSaida(),
                        Descarte = (movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada ? movimentacao.QuantidadeDescartada : movimentacao.QuantidadeDescartada * -1),
                        SaldoTotal = movimentacao.SaldoTotal,
                        GrupoPessoas = movimentacao.GrupoPessoas?.Descricao ?? ""
                    }
                ).ToList();

                var parametros = new List<Parametro>();

                if (cpfCnpjCliente > 0d)
                {
                    var repositorioCliente = new Repositorio.Cliente(unitOfWork);
                    var cliente = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

                    parametros.Add(new Parametro("Cliente", cliente.Nome, true));
                }
                else
                    parametros.Add(new Parametro("Cliente", false));

                if (dataInicio.HasValue || dataFim.HasValue)
                {
                    string data = "";
                    data += dataInicio.HasValue ? dataInicio.Value.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFim.HasValue ? "até " + dataFim.Value.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Parametro("Periodo", data, true));
                }
                else
                    parametros.Add(new Parametro("Periodo", false));
                if (codigoGrupoPessoas > 0)
                {
                    var repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                    var grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);

                    parametros.Add(new Parametro("GrupoPessoas", grupoPessoas?.Descricao ?? "", true));
                }
                else
                    parametros.Add(new Parametro("GrupoPessoas", false));

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/EstoqueCliente",parametros,relatorioControleGeracao, relatorioTemp, dynEstoque, unitOfWork, null, null, true, TipoServicoMultisoftware);
            }
            catch (Exception ex)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string nomePropriedadeOrdenar)
        {
            if (nomePropriedadeOrdenar == "Cliente")
                return "Cliente.Nome";

            return nomePropriedadeOrdenar;
        }

        #endregion
    }
}
