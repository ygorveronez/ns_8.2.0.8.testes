using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.Conciliacao
{
    [CustomAuthorize("Financeiros/DocumentosConciliacao")]
    public class DocumentosConciliacaoController : BaseController
    {
		#region Construtores

		public DocumentosConciliacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterResultadoPesquisa(unitOfWork));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = ObterResultadoPesquisa(unitOfWork);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterResultadoPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentosConciliacao filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = ObterGrid();

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "DocumentosConciliacao/Pesquisa", "grid-documentos-conciliacao");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

            if (parametrosConsulta.PropriedadeOrdenar == "DescricaoDataConsulta")
                parametrosConsulta.PropriedadeOrdenar = "DataConsulta";

            int totalRegistros = repCTe.ContarDocumentosConciliacao(filtrosPesquisa);
            IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoConciliacao> lista = totalRegistros > 0 ? repCTe.ConsultarDocumentosConciliacao(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoConciliacao>();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentosConciliacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentosConciliacao()
            {
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa.Codigo : Request.GetIntParam("Empresa"),
                StatusTitulo = Request.GetEnumParam<StatusTitulo>("StatusTitulo"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroFatura = Request.GetIntParam("NumeroFatura"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CnpjCpfRemetente = Request.GetDoubleParam("Remetente"),
                TipoServicoMultisoftware = TipoServicoMultisoftware
            };
        }

        private Models.Grid.Grid ObterGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número CT-e", "Numero", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Série CT-e", "Serie", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação Carga", "StatusCarga", 8, Models.Grid.Align.center, false, false, false, false, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportador_Formatado", 15, Models.Grid.Align.left, false, false, false, false, true);
            }
            grid.AdicionarCabecalho("Filial", "Filial", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data de Emissão", "DescricaoDataEmissao", 15, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data de Vencimento", "DescricaoDataVencimento", 15, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data do Pagamento", "DescricaoDataLiquidacao", 15, Models.Grid.Align.left, true, false, false, false, true);
            //grid.AdicionarCabecalho("Situação Carga", "SituacaoCarga", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo Documento", "DescricaoTipoDocumento", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor Original", "ValorOriginalFormatado", 10, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Liquidação", "ValorLiquidacaoFormatado", 10, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor a Receber", "ValorAReceberFormatado", 10, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 20, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº Fatura", "NumeroFatura", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Acréscimos", "Acrescimos", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Decréscimos", "Decrescimos", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Consulta", "DescricaoDataConsulta", 15, Models.Grid.Align.left, true, false, false, false, true);

            return grid;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoDataEmissao")
                return "DataEmissao";
            if (propriedadeOrdenar == "DescricaoSituacao")
                return "StatusTitulo";
            if (propriedadeOrdenar == "DescricaoDataLiquidacao")
                return "DataLiquidacao";
            if (propriedadeOrdenar == "DescricaoDataVencimento")
                return "DataVencimento";

            return propriedadeOrdenar;
        }

        #endregion

    }
}
