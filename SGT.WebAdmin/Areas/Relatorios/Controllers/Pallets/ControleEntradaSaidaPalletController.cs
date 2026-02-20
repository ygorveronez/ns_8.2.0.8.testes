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
	[CustomAuthorize("Relatorios/Pallets/ControleEntradaSaidaPallet")]
    public class ControleEntradaSaidaPalletController : BaseController
    {
		#region Construtores

		public ControleEntradaSaidaPalletController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R141_ControleEntradaSaidaPallet;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                var codigoRelatorio = Request.GetIntParam("Codigo");
                var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                var relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Controle de Entrada e Saída de Pallets", "Pallets", "ControleEntradaSaidaPallet.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                var gridRelatorio = new Models.Grid.Relatorio();
                var dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

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

                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleEntradaSaidaPallet()
                {
                    DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                    ListaCodigoFilial = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Filial")),
                    ListaCodigoTransportador = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Transportador")),
                    ListaCpfCnpjCliente = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Cliente")),
                    NaturezaMovimentacao = Request.GetNullableEnumParam<NaturezaMovimentacaoEstoquePallet>("NaturezaMovimentacao")
                };

                var repositorio = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);
                var propriedadeAgrupar = grid.group.enable ? grid.group.propAgrupa : "";
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var listaMovimentacaoEstoquePallet = repositorio.ConsultarRelatorioControleEntradaSaida(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repositorio.ContarConsultaRelatorioControleEntradaSaida(filtrosPesquisa));

                grid.AdicionaRows((
                    from movimentacaoEstoque in listaMovimentacaoEstoquePallet
                    select new
                    {
                        Cliente = movimentacaoEstoque.Cliente?.Descricao,
                        movimentacaoEstoque.Codigo,
                        Data = movimentacaoEstoque.Data.ToString("dd/MM/yyyy"),
                        Filial = movimentacaoEstoque.Filial?.Descricao,
                        FilialCnpj = movimentacaoEstoque.Filial?.CNPJ_Formatado,
                        FilialCodigoIntegracao = movimentacaoEstoque.Filial?.CodigoFilialEmbarcador,
                        NaturezaMovimentacao = movimentacaoEstoque.NaturezaMovimentacao.ObterDescricao(),
                        movimentacaoEstoque.Observacao,
                        QuantidadeEntrada = movimentacaoEstoque.ObterQuantidadeEntrada(),
                        QuantidadeSaida = movimentacaoEstoque.ObterQuantidadeSaida(),
                        movimentacaoEstoque.SaldoTotal,
                        Setor = movimentacaoEstoque.Setor?.Descricao,
                        TipoLancamento = movimentacaoEstoque.ObterTipoLancamento(),
                        Transportador = movimentacaoEstoque.Transportador?.Descricao,
                        TransportadorCnpj = movimentacaoEstoque.Transportador?.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = movimentacaoEstoque.Transportador?.CodigoIntegracao
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
                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleEntradaSaidaPallet()
                {
                    DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                    ListaCodigoFilial = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Filial")),
                    ListaCodigoTransportador = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Transportador")),
                    ListaCpfCnpjCliente = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Cliente")),
                    NaturezaMovimentacao = Request.GetNullableEnumParam<NaturezaMovimentacaoEstoquePallet>("NaturezaMovimentacao")
                };

                var repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                var gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                var dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                var relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                var relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                var relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                var mdlRelatorio = new Models.Grid.Relatorio();
                var grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemporario.PropriedadeOrdena);
                var stringConexao = _conexao.StringConexao;
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propriedadeOrdenar, relatorioTemporario.PropriedadeAgrupa);

                _ = Task.Factory.StartNew(() => GerarRelatorioControleEntradaSaidaPallet(agrupamentos, filtrosPesquisa, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioControleEntradaSaidaPallet(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleEntradaSaidaPallet filtrosPesquisa, RelatorioControleGeracao relatorioControleGeracao, Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                var repositorioEstoquePallet = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemporario.PropriedadeOrdena);
                var listaMovimentacaoPallet = repositorioEstoquePallet.ConsultarRelatorioControleEntradaSaida(filtrosPesquisa, propriedadeOrdenar, relatorioTemporario.OrdemOrdenacao, 0, 0);

                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleEntradaSaidaPallet> dataSourceEntradaSaidaPallet = (
                    from movimentacaoEstoque in listaMovimentacaoPallet
                    select new Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleEntradaSaidaPallet()
                    {
                        Cliente = movimentacaoEstoque.Cliente?.Descricao,
                        Codigo = movimentacaoEstoque.Codigo,
                        Data = movimentacaoEstoque.Data,
                        Filial = movimentacaoEstoque.Filial?.Descricao,
                        FilialCnpj = movimentacaoEstoque.Filial?.CNPJ_Formatado,
                        FilialCodigoIntegracao = movimentacaoEstoque.Filial?.CodigoFilialEmbarcador,
                        NaturezaMovimentacao = movimentacaoEstoque.NaturezaMovimentacao.ObterDescricao(),
                        Observacao = movimentacaoEstoque.Observacao,
                        QuantidadeEntrada = movimentacaoEstoque.ObterQuantidadeEntrada(),
                        QuantidadeSaida = movimentacaoEstoque.ObterQuantidadeSaida(),
                        SaldoTotal = movimentacaoEstoque.SaldoTotal,
                        Setor = movimentacaoEstoque.Setor?.Descricao,
                        TipoLancamento = movimentacaoEstoque.ObterTipoLancamento(),
                        Transportador = movimentacaoEstoque.Transportador?.Descricao,
                        TransportadorCnpj = movimentacaoEstoque.Transportador?.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = movimentacaoEstoque.Transportador?.CodigoIntegracao
                    }
                ).ToList();

                var parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/ControleEntradaSaidaPallet",parametros,relatorioControleGeracao, relatorioTemporario, dataSourceEntradaSaidaPallet, unitOfWork, null, null, true, TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true, false, false, true, true);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ", "FilialCnpj", 10, Models.Grid.Align.center, false, false, false, false, false);
            }

            grid.AdicionarCabecalho("Código Integração", "FilialCodigoIntegracao", 15, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Setor", "Setor", 20, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("Empresa/Filial", "Transportador", 20, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ Empresa/Filial", "TransportadorCnpj", 10, Models.Grid.Align.center, false, false, false, false, false);
            }
            else
            {
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ Transportador", "TransportadorCnpj", 10, Models.Grid.Align.center, false, false, false, false, false);
            }

            grid.AdicionarCabecalho("Código Integração Transportador", "TransportadorCodigoIntegracao", 15, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Cliente", "Cliente", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Natureza Movimentação", "NaturezaMovimentacao", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Lançamento", "TipoLancamento", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade Entrada", "QuantidadeEntrada", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Quantidade Saída", "QuantidadeSaida", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Saldo Total", "SaldoTotal", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);

            return grid;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleEntradaSaidaPallet filtrosPesquisa)
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
                    var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                    var filial = repositorioFilial.BuscarPorCodigo(filtrosPesquisa.ListaCodigoFilial.First());

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
                    var repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                    var transportador = repositorioTransportador.BuscarPorCodigo(filtrosPesquisa.ListaCodigoTransportador.First());

                    parametros.Add(new Parametro("Transportador", transportador.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Transportador", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Transportador", false));

            if (filtrosPesquisa.ListaCpfCnpjCliente?.Count > 0)
            {
                if (filtrosPesquisa.ListaCpfCnpjCliente.Count == 1)
                {
                    var repositorioCliente = new Repositorio.Cliente(unitOfWork);
                    var cliente = repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.ListaCpfCnpjCliente.First());

                    parametros.Add(new Parametro("Cliente", cliente.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Cliente", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Cliente", false));

            if (filtrosPesquisa.NaturezaMovimentacao.HasValue)
                parametros.Add(new Parametro("NaturezaMovimentacao", filtrosPesquisa.NaturezaMovimentacao.Value.ObterDescricao(), true));
            else
                parametros.Add(new Parametro("NaturezaMovimentacao", false));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string nomePropriedadeOrdenar)
        {
            if (nomePropriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (nomePropriedadeOrdenar == "Transportador")
                return "Transportador.RazaoSocial";

            return nomePropriedadeOrdenar;
        }

        #endregion
    }
}
