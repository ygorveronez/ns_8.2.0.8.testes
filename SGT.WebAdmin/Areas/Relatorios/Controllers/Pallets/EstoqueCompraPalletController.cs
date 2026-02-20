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
	[CustomAuthorize("Relatorios/Pallets/EstoqueCompraPallet")]
    public class EstoqueCompraPalletController : BaseController
    {
		#region Construtores

		public EstoqueCompraPalletController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R143_EstoqueCompraPallet;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                var codigoRelatorio = Request.GetIntParam("Codigo");
                var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                var relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Estoque de Compra de Pallets", "Pallets", "EstoqueCompraPallet.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                var gridRelatorio = new Models.Grid.Relatorio();
                var dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaEstoqueCompraPallet()
                {
                    DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                    ListaCodigoFilial = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Filial")),
                    ListaCodigoTransportador = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Transportador")),
                    ListaCpfCnpjFornecedor = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Fornecedor")),
                    NumeroNfe = Request.GetIntParam("Nfe")
                };

                var repositorio = new Repositorio.Embarcador.Pallets.CompraPallets(unitOfWork, cancellationToken);
                var propriedadeAgrupar = grid.group.enable ? grid.group.propAgrupa : "";
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var listaCompraPallet = repositorio.ConsultarRelatorio(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(listaCompraPallet.Count);

                grid.AdicionaRows((
                    from compra in listaCompraPallet
                    select new
                    {
                        compra.Codigo,
                        Data = compra.DataCriacao.ToString("dd/MM/yyyy"),
                        Filial = compra.Filial?.Descricao,
                        FilialCnpj = compra.Filial?.CNPJ_Formatado,
                        FilialCodigoIntegracao = compra.Filial?.CodigoFilialEmbarcador,
                        Fornecedor = compra.Fornecedor.Nome,
                        FornecedorCpfCnpj = compra.Fornecedor.CPF_CNPJ_Formatado,
                        FornecedorCodigoIntegracao = compra.Fornecedor.CodigoIntegracao,
                        Transportador = compra.Transportador?.Descricao,
                        TransportadorCnpj = compra.Transportador?.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = compra.Transportador?.CodigoIntegracao,
                        NumeroNfe = compra.Numero,
                        compra.Quantidade,
                        ValorUnitario = compra.Valor,
                        ValorTotal = (compra.Valor * compra.Quantidade)
                    }
                ).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaEstoqueCompraPallet()
                {
                    DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                    ListaCodigoFilial = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Filial")),
                    ListaCodigoTransportador = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Transportador")),
                    ListaCpfCnpjFornecedor = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Fornecedor")),
                    NumeroNfe = Request.GetIntParam("Nfe")
                };

                var repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                var gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                var dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                var relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                var relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                var relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                var mdlRelatorio = new Models.Grid.Relatorio();
                var grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemporario.PropriedadeOrdena);
                var stringConexao = _conexao.StringConexao;
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propriedadeOrdenar, relatorioTemporario.PropriedadeAgrupa);

                _ = Task.Factory.StartNew(() => GerarRelatorioEstoqueCompraPallet(agrupamentos, filtrosPesquisa, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioEstoqueCompraPallet(
            List<PropriedadeAgrupamento> agrupamentos,
            Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaEstoqueCompraPallet filtrosPesquisa,
            RelatorioControleGeracao relatorioControleGeracao,
            Relatorio relatorioTemporario,
            string stringConexao,
            CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                var repositorioCompraPallet = new Repositorio.Embarcador.Pallets.CompraPallets(unitOfWork);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemporario.PropriedadeOrdena);
                var listaCompraPallet = repositorioCompraPallet.ConsultarRelatorio(filtrosPesquisa, propriedadeOrdenar, relatorioTemporario.OrdemOrdenacao, 0, 0);

                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.EstoqueCompraPallet> dataSourceCompraPallet = (
                    from compra in listaCompraPallet
                    select new Dominio.Relatorios.Embarcador.DataSource.Pallets.EstoqueCompraPallet()
                    {
                        Codigo = compra.Codigo,
                        Data = compra.DataCriacao,
                        Filial = compra.Filial?.Descricao,
                        FilialCnpj = compra.Filial?.CNPJ_Formatado,
                        FilialCodigoIntegracao = compra.Filial?.CodigoFilialEmbarcador,
                        Fornecedor = compra.Fornecedor.Nome,
                        FornecedorCpfCnpj = compra.Fornecedor.CPF_CNPJ_Formatado,
                        FornecedorCodigoIntegracao = compra.Fornecedor.CodigoIntegracao,
                        Transportador = compra.Transportador?.Descricao,
                        TransportadorCnpj = compra.Transportador?.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = compra.Transportador?.CodigoIntegracao,
                        NumeroNfe = compra.Numero,
                        Quantidade = compra.Quantidade,
                        ValorTotal = (compra.Valor * compra.Quantidade),
                        ValorUnitario = compra.Valor
                    }
                ).ToList();

                var parametros = await ObterParametrosRelatorio(unitOfWork, filtrosPesquisa, cancellationToken);
                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/EstoqueCompraPallet",parametros,relatorioControleGeracao, relatorioTemporario, dataSourceCompraPallet, unitOfWork, null, null, true, TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                await servicoRelatorio.RegistrarFalhaGeracaoRelatorioAsync(relatorioControleGeracao, unitOfWork, excecao, cancellationToken);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("NF-e", "NumeroNfe", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 20, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("CPF/CNPJ Fornecedor", "FornecedorCpfCnpj", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Código Integração Fornecedor", "FornecedorCodigoIntegracao", 10, Models.Grid.Align.left, false, false, false, false, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("Empresa/Filial", "Transportador", 20, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ Empresa/Filial", "TransportadorCnpj", 10, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Código Integração Empresa/Filial", "TransportadorCodigoIntegracao", 10, Models.Grid.Align.left, false, false, false, false, true);
            }
            else
            {
                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ Filial", "FilialCnpj", 10, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Código Integração Filial", "FilialCodigoIntegracao", 10, Models.Grid.Align.left, false, false, false, false, true);
            }

            grid.AdicionarCabecalho("Quantidade", "Quantidade", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Unitário", "ValorUnitario", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Total", "ValorTotal", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);

            return grid;
        }

        private async Task<List<Parametro>> ObterParametrosRelatorio(
            Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaEstoqueCompraPallet filtrosPesquisa,
            CancellationToken cancellationToken)
        {
            var parametros = new List<Parametro>();

            if (filtrosPesquisa.DataInicio.HasValue || filtrosPesquisa.DataLimite.HasValue)
            {
                var periodo = $"{(filtrosPesquisa.DataInicio.HasValue ? $"{filtrosPesquisa.DataInicio.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataLimite.HasValue ? $"até {filtrosPesquisa.DataLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Parametro("Periodo", periodo, true));
            }
            else
                parametros.Add(new Parametro("Periodo", false));

            if (filtrosPesquisa.ListaCodigoFilial?.Count > 0)
            {
                if (filtrosPesquisa.ListaCodigoFilial.Count == 1)
                {
                    var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
                    var filial = await repositorioFilial.BuscarPorCodigoAsync(filtrosPesquisa.ListaCodigoFilial.First());

                    parametros.Add(new Parametro("Filial", filial.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Filial", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Filial", false));

            if (filtrosPesquisa.ListaCodigoTransportador?.Count > 0)
            {
                if (filtrosPesquisa.ListaCodigoTransportador.Count == 1)
                {
                    Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork, cancellationToken);
                    Dominio.Entidades.Empresa transportador = await repositorioTransportador.BuscarPorCodigoAsync(filtrosPesquisa.ListaCodigoTransportador.First());

                    parametros.Add(new Parametro("Transportador", transportador.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Transportador", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Transportador", false));

            if (filtrosPesquisa.ListaCpfCnpjFornecedor?.Count > 0)
            {
                if (filtrosPesquisa.ListaCpfCnpjFornecedor.Count == 1)
                {
                    var repositorioCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
                    var fornecedor = await repositorioCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.ListaCpfCnpjFornecedor.First());

                    parametros.Add(new Parametro("Fornecedor", fornecedor.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Fornecedor", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Fornecedor", false));

            if (filtrosPesquisa.NumeroNfe > 0)
                parametros.Add(new Parametro("Nfe", filtrosPesquisa.NumeroNfe.ToString(), true));
            else
                parametros.Add(new Parametro("Nfe", false));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string nomePropriedadeOrdenar)
        {
            if (nomePropriedadeOrdenar == "Data")
                return "DataCriacao";

            if (nomePropriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (nomePropriedadeOrdenar == "Transportador")
                return "Transportador.RazaoSocial";

            if (nomePropriedadeOrdenar == "NumeroNfe")
                return "Numero";

            return nomePropriedadeOrdenar;
        }

        #endregion
    }
}
