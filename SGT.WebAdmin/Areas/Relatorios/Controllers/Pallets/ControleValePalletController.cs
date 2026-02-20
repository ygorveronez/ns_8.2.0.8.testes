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
	[CustomAuthorize("Relatorios/Pallets/ControleValePallet")]
    public class ControleValePalletController : BaseController
    {
		#region Construtores

		public ControleValePalletController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R140_ControleValePallet;

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
                var relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Controle de Vale Pallets", "Pallets", "ControleValePallet.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
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

                var filtrosPesquisa = ObterFiltrosPesquisa();
                var repositorio = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);
                var propriedadeAgrupar = grid.group.enable ? grid.group.propAgrupa : "";
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var listaValePallet = repositorio.ConsultarRelatorio(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repositorio.ContarConsultaRelatorio(filtrosPesquisa));

                grid.AdicionaRows((
                    from valePallet in listaValePallet
                    select new
                    {
                        Cidade = "",
                        Cliente = valePallet.Devolucao.XMLNotaFiscal?.Destinatario?.Descricao ?? "",
                        valePallet.Codigo,
                        Data = valePallet.DataLancamento,
                        Filial = valePallet.Devolucao.Filial?.Descricao ?? "",
                        FilialCnpj = valePallet.Devolucao.Filial?.CNPJ_Formatado ?? "",
                        FilialCodigoIntegracao = valePallet.Devolucao.Filial?.CodigoFilialEmbarcador ?? "",
                        Motorista = "",
                        valePallet.Numero,
                        NumeroNfe = valePallet.Devolucao.XMLNotaFiscal?.Numero,
                        valePallet.Quantidade,
                        Representante = valePallet.Representante?.Descricao ?? "",
                        Situacao = valePallet.Situacao.ObterDescricao(),
                        Transportador = valePallet.Devolucao.Transportador.Descricao,
                        TransportadorCnpj = valePallet.Devolucao.Transportador.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = valePallet.Devolucao.Transportador.CodigoIntegracao
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
                var filtrosPesquisa = ObterFiltrosPesquisa();
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

                _ = Task.Factory.StartNew(() => GerarRelatorioControleValePallet(agrupamentos, filtrosPesquisa, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioControleValePallet(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleValePallet filtrosPesquisa, RelatorioControleGeracao relatorioControleGeracao, Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                var repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemporario.PropriedadeOrdena);
                var listaValePallet = repositorioValePallet.ConsultarRelatorio(filtrosPesquisa, propriedadeOrdenar, relatorioTemporario.OrdemOrdenacao, 0, 0);

                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleValePallet> dataSourceValePallet = (
                    from valePallet in listaValePallet
                    select new Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleValePallet()
                    {
                        Cidade = "",
                        Cliente = valePallet.Devolucao.XMLNotaFiscal?.Destinatario?.Descricao ?? "",
                        Codigo = valePallet.Codigo,
                        Data = valePallet.DataLancamento,
                        Filial = valePallet.Devolucao.Filial?.Descricao ?? "",
                        FilialCnpj = valePallet.Devolucao.Filial?.CNPJ_Formatado ?? "",
                        FilialCodigoIntegracao = valePallet.Devolucao.Filial?.CodigoFilialEmbarcador ?? "",
                        Motorista = "",
                        Numero = valePallet.Numero,
                        NumeroNfe = valePallet.Devolucao.XMLNotaFiscal?.Numero ?? 0,
                        Quantidade = valePallet.Quantidade,
                        Representante = valePallet.Representante?.Descricao ?? "",
                        Situacao = valePallet.Situacao.ObterDescricao(),
                        Transportador = valePallet.Devolucao.Transportador.Descricao,
                        TransportadorCnpj = valePallet.Devolucao.Transportador.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = valePallet.Devolucao.Transportador.CodigoIntegracao
                    }
                ).ToList();

                var parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/ControleValePallet",parametros,relatorioControleGeracao, relatorioTemporario, dataSourceValePallet, unitOfWork, null, null, true, TipoServicoMultisoftware);

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
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("NF-e", "NumeroNfe", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Filial", "FilialCnpj", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Integração Filial", "FilialCodigoIntegracao", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CNPJ Transportador", "TransportadorCnpj", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Integração Transportador", "TransportadorCodigoIntegracao", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Cliente", "Cliente", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 8, Models.Grid.Align.center, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Cidade", "Cidade", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Representante", "Representante", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.left, true, false, false, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleValePallet ObterFiltrosPesquisa()
        {
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleValePallet()
            {
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                ListaCodigoFilial = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Filial")),
                ListaCpfCnpjCliente = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Cliente")),
                NumeroNfe = Request.GetIntParam("Nfe"),
                Situacao = Request.GetNullableEnumParam<SituacaoValePallet>("Situacao")
            };

            return filtrosPesquisa;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleValePallet filtrosPesquisa)
        {
            var parametros = new List<Parametro>();

            if (filtrosPesquisa.DataInicial.HasValue || filtrosPesquisa.DataLimite.HasValue)
            {
                var periodo = $"{(filtrosPesquisa.DataInicial.HasValue ? $"{filtrosPesquisa.DataInicial.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataLimite.HasValue ? $"até {filtrosPesquisa.DataLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
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

            if (filtrosPesquisa.NumeroNfe > 0)
                parametros.Add(new Parametro("Nfe", filtrosPesquisa.NumeroNfe.ToString(), true));
            else
                parametros.Add(new Parametro("Nfe", false));

            if (filtrosPesquisa.Situacao.HasValue)
                parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao.Value.ObterDescricao(), true));
            else
                parametros.Add(new Parametro("Situacao", false));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string nomePropriedadeOrdenar)
        {
            if (nomePropriedadeOrdenar == "Data")
                return "DataLancamento";

            if (nomePropriedadeOrdenar == "Filial")
                return "Devolucao.Filial.Descricao";

            if (nomePropriedadeOrdenar == "Transportador")
                return "Devolucao.Transportador.RazaoSocial";

            return nomePropriedadeOrdenar;
        }

        #endregion
    }
}
