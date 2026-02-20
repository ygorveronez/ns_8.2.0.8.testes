using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/GestaoNotasFiscais")]
    public class GestaoNotasFiscaisController : BaseController
    {
		#region Construtores

		public GestaoNotasFiscaisController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

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
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroGestaoNotasFiscais filtros = ObterFiltros();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº CTe", "DescricaoNumeroCTe", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor CT-e", "DescricaoValorCTe", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação CT-e", "DescricaoSituacaoCTe", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº NF-e", "NumeroNFe", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Série NF-e", "Serie", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Chave NF-e", "ChaveNFe", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CNPJ Emitente", "CPFCNPJEmitenteFormatado", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nome Emitente", "NomeEmitente", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Cliente Origem", "ClienteOrigem", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Loc. Origem", "CidadeOrigem", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("UF Origem", "UFOrigem", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Cliente Destino", "ClienteDestino", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Loc. Destino", "CidadeDestino", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("UF Destino", "UFDestino", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Produto", "Produto", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Peso", "Peso", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor NF-e", "Valor", 5, Models.Grid.Align.left, false);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                int totalRegistros = repXMLNotaFiscal.ContarConsultaGestaoNotasFiscais(filtros);

                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.GestaoNotasFiscais> notasFiscais = totalRegistros > 0 ? repXMLNotaFiscal.ConsultarGestaoNotasFiscais(filtros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.ObjetosDeValor.Embarcador.Documentos.GestaoNotasFiscais>();
                
                grid.AdicionaRows(notasFiscais);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroGestaoNotasFiscais ObterFiltros()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroGestaoNotasFiscais()
            {
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoCTe = Request.GetIntParam("CTe"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CPFCNPJEmitente = Request.GetDoubleParam("Emitente"),
                DataEmissaoCargaFinal = Request.GetNullableDateTimeParam("DataEmissaoCargaFinal"),
                DataEmissaoCargaInicial = Request.GetNullableDateTimeParam("DataEmissaoCargaInicial"),
                DataEmissaoCTeFinal = Request.GetNullableDateTimeParam("DataEmissaoCTeFinal"),
                DataEmissaoCTeInicial = Request.GetNullableDateTimeParam("DataEmissaoCTeInicial"),
                DataEmissaoNotaFiscalFinal = Request.GetNullableDateTimeParam("DataEmissaoNotaFiscalFinal"),
                DataEmissaoNotaFiscalInicial = Request.GetNullableDateTimeParam("DataEmissaoNotaFiscalInicial"),
                Numero = Request.GetIntParam("Numero"),
                Produto = Request.GetStringParam("Produto"),
                Serie = Request.GetIntParam("Serie"),
                Veiculo = Request.GetStringParam("Veiculo"),
                PossuiCTe = Request.GetNullableBoolParam("PossuiCTe")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
