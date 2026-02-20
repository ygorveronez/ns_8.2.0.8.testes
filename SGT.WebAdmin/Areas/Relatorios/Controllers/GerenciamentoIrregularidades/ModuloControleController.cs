using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.ModuloControle
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/GerenciamentoIrregularidades/ModuloControle")]
    public class ModuloControleController : BaseController
    {
		#region Construtores

		public ModuloControleController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R327_ModuloControle;
        private decimal TamanhoColunasValores = (decimal)1.75;
        private decimal TamanhoColunasParticipantes = (decimal)5.50;
        private decimal TamanhoColunasLocalidades = 3;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório do Módulo de Controle", "GerenciamentoIrregularidades", "ModuloControle.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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


                //Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                //grid.scrollHorizontal = IsLayoutClienteAtivo();
                //Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                //Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                //Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                //List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFes.NFes servicoRelatorioNFes = new Servicos.Embarcador.Relatorios.NFes.NFes(unitOfWork, TipoServicoMultisoftware, Cliente);

                //servicoRelatorioNFes.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.NFes> listaNFes, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                //return new JsonpResult(grid);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                var listaModuloControle = new List<Dominio.Relatorios.Embarcador.DataSource.GerenciamentoIrregularidades.RelatorioModuloControle>();


                grid.setarQuantidadeTotal(listaModuloControle.Count);
                grid.AdicionaRows(listaModuloControle);

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
        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {


            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Atendimento", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status de Atendimento", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Irregularidade Atendimento", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Prazo Termo de Quitação", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status Termo de Quitação", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo Recepção Documento", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data e Hora Recepção Documento", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Empresa", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Cód Centro Unilever", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Descrição Centro Unilever", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Transportadora ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Razão Social Transportadora ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Em Tratativa (área que está parada a pendência) ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Última Tratativa Realizada", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo de Documento", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("DT/Ocorrência", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("DT de referência", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Origem Carga/Ocorrência", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Zona Tarifária Origem", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Destino Carga/Ocorrência", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Sistema de Entrega", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo de carga", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo de Equipamento", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Processo de Expedição", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nome Portfólio", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nome da Irregularidade Módulo de Controle", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status Módulo de Controle", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Qntd Etapas", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Chave", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Número Documento", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status Documento", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Etapa ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Documento (Bruto)", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Documento (Líquido)", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Emissor ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Chaves de Acesso NF-es Transportadas", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Número de NF-e Transportadas", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Peso Utilizado para o cálculo", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Peso das notas", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Chave Original", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("MIRO Original", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Remetente", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Razão Social Remetente", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Destinatário", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Razão Social Destinatário", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Expedidor", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Razão Social Expedidor", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Recebedor ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Razão Social Recebedor  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Informação Adicional (observações)  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status FRS  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("FRS  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor FRS (Bruto)  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor FRS (Líquido)  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status do POD ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("GNRE  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status GNRE  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Área 1  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Usuário 1   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Ação 1   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data 1   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Dias parado para tratativa área 1   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Comentário 1   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Área 2   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Usuário 2  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Ação 2    ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data 2  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Dias parado para tratativa área 2   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Comentário 2  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Área 3  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Usuário 3  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Ação 3  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data 3   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Dias parado para tratativa área 3  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Comentários 3   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data da MIRO  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("MIRO (número)  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor MIRO (Bruto)   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor MIRO (Líquido)   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status MIRO  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Issue MIRO   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Detalhes (retorno MIRO SAP mensagem)  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data prevista pagamento  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Pagamento  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Pago (Bruto)   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Pago (Líquido)  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Total Etapas (DT/Ocorrência) - Bruto   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Total Etapas (DT/Ocorrência) - Líquido  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Total Etapas (Documentos) - Bruto", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Total Etapas (Documentos) - Líquido", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Etapas (FRS menos Documento) - Bruto", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Percentual diferença dos valores totais das etapas", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IVA", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IVA - Ação", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("ICMS MIRO", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Base ICMS MIRO", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Alíquota ICMS MIRO   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("PIS MIRO", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Base PIS MIRO", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Alíquota PIS MIRO", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("COFINS MIRO", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Base COFINS MIRO", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Alíquota COFINS MIRO", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("ICMS MIRO - Desconto", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Alíquota ICMS MIRO - Desconto   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("PIS MIRO - Desconto   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Base PIS MIRO - Desconto   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Alíquota PIS MIRO - Desconto   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("COFINS MIRO - Desconto   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Base COFINS MIRO - Desconto   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Alíquota COFINS MIRO - Desconto   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("ISS MIRO  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Base ISS MIRO  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Alíquota ISS MIRO  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Local da Prestação do Serviço - Municipio   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("RETENÇÃO ISS - DEVIDA AO PRESTADOR OU TOMADOR   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Municipio da Transportadora   ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status CT-e  ", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);




            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioModuloControle ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioModuloControle filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioModuloControle()
            {
                DataInicialEmissao = Request.GetNullableDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetNullableDateTimeParam("DataFinalEmissao"),
                DataInicialIrregularidade = Request.GetNullableDateTimeParam("DataInicialIrregularidade"),
                DataFinalIrregularidade = Request.GetNullableDateTimeParam("DataFinalIrregularidade"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                SerieCTe = Request.GetIntParam("SerieCTe"),
                Situacao = Request.GetNullableEnumParam<SituacaoControleDocumento>("Situacao"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoSetor = Request.GetIntParam("Setor"),
                CodigoIrregularidade = Request.GetIntParam("Irregularidade"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoPortifolio = Request.GetIntParam("Portifolio"),

            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
