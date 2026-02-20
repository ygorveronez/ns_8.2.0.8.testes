using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.WMS
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/WMS/RastreabilidadeVolumes")]
    public class RastreabilidadeVolumesController : BaseController
    {
		#region Construtores

		public RastreabilidadeVolumesController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R246_RastreabilidadeVolumes;

        #endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Rastreamento de Volumes", "WMS", "RastreamentoVolumes.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "ProdutoEmbarcador", "desc", "", "", Codigo, unitOfWork, false, true);
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


        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioRastreabilidadeVolumes filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.WMS.RastreabilidadeVolumes svcRastreabilidadeVolumes = new Servicos.Embarcador.Relatorios.WMS.RastreabilidadeVolumes(unitOfWork, TipoServicoMultisoftware, Cliente);

                svcRastreabilidadeVolumes.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.WMS.RastreamentoVolumes> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta); ;

                grid.setarQuantidadeTotal(totalRegistros);
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

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioRastreabilidadeVolumes filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware); ;
                await unitOfWork.CommitChangesAsync(cancellationToken);

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


        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Número Pedido Embarcador", "NumeroPedidoEmbarcador", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Data Criação Pedido", "DataPedidoFormatada", 10, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Recebimento", "DataRecebimentoFormatada", 10, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Usuário Recebimento", "UsuarioRecebimento", 10, Models.Grid.Align.left, false, false, true, true, false);
            grid.AdicionarCabecalho("Número Nota", "NumeroNF", 8, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Série", "Serie", 8, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 8, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Peso Líquido", "PesoLiquido", 8, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Peso Bruto", "PesoBruto", 8, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor Mercadoria", "ValorMercadoria", 8, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo NF", "TipoNFDescricao", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Qtd Itens Recebidos", "QtdItensRecebidos", 8, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Qtd Itens Faltantes", "QtdItensFaltantes", 8, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Ocorrencias", "Ocorrencias", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Armazen", "DataArmazenFormatada", 10, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Operador Armazen", "OperadorArmazen", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Produto Embarcador", "ProdutoEmbarcador", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Depósito", "Deposito", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Bloco", "Bloco", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Rua", "Rua", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Posição", "Posicao", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Local", "Local", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Cód. Barras/QR Cód. Uni.", "CodigoBarras", 12, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Cód. Etiqueta Master Pallet", "EtiquetaMasterPallet", 12, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimentoFormatada", 10, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo Mercadoria", "TipoMercadoria", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UN", "UN", 5, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. Lote", "QtdLote", 8, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Registro de Ocorrência", "RegistroOcorrencia", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Qtd Devolvida", "QtdDevolvida", 8, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. Disponível", "QtdDisponivel", 8, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Dt. Expedição", "DataExpedicaoFormatada", 10, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Usuário Expedição", "UsuarioExpedicao", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Cod. Etiqueta Expedição", "EtiquetaExpedicao", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Volumes", "Volumes", 8, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Embarcados", "Embarcados", 8, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Falta(m)", "Falta", 8, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("UF - remetnete", "UfRemetente", 5, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Destinatario", "Destinatario", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("UF - Destinatario", "UfDestinatario", 5, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Expedidor", "Expedidor", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("UF - expedidor", "UfExpedidor", 5, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Carga Transbordo", "CargaTransbordo", 8, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Conferência", "DataConferenciaFormatada", 10, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Embarque", "DataEmbarqueFormatada", 10, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("MDF-e", "MDFe", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 8, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Placa Veículo", "PlacaVeiculo", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chegada Filial", "ChegadaFilialFormatada", 10, Models.Grid.Align.center, false, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioRastreabilidadeVolumes ObterFiltrosPesquisa()
        {

            return new Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioRastreabilidadeVolumes()
            {
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                Carga = Request.GetIntParam("Carga"),
                ProdutoEmbarcador = Request.GetIntParam("ProdutoEmbarcador"),
                DataPedidoInicial = Request.GetDateTimeParam("DataPedidoInicial"),
                DataPedidoFinal = Request.GetDateTimeParam("DataPedidoFinal"),
                DataRecebimentoInicial = Request.GetDateTimeParam("DataRecebimentoInicial"),
                DataRecebimentoFinal = Request.GetDateTimeParam("DataRecebimentoFinal"),
            };
        }

        #endregion
    }
}

