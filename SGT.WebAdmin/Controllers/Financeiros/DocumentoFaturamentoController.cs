using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/DocumentoFaturamento")]
    public class DocumentoFaturamentoController : BaseController
    {
		#region Construtores

		public DocumentoFaturamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumentosEmAberto()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Documento", "Documento", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 22, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 22, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Faturar", "ValorAFaturar", 12, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repDocumentoFaturamento.ConsultarDocumentosParaFatura(filtros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int countDocumentosFaturamento = repDocumentoFaturamento.ContarConsultaDocumentosParaFatura(filtros);

                if (propOrdenar == "Documento")
                    propOrdenar = "CTe.Numero";
                else if (propOrdenar == "Valor")
                    propOrdenar = "ValorDocumento";

                grid.setarQuantidadeTotal(countDocumentosFaturamento);

                var retorno = (from o in documentosFaturamento
                               select new
                               {
                                   o.Codigo,
                                   Documento = o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe ? o.CTe.Numero + "-" + o.CTe.Serie.Numero : o.Carga.CodigoCargaEmbarcador,
                                   Tipo = o.DescricaoTipoDocumento,
                                   Origem = o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe ? o.CTe.LocalidadeInicioPrestacao.DescricaoCidadeEstado : o.Carga.DadosSumarizados.Origens,
                                   Destino = o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe ? o.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado : o.Carga.DadosSumarizados.Destinos,
                                   Valor = o.ValorDocumento.ToString("n2"),
                                   ValorAFaturar = o.ValorAFaturar.ToString("n2")
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar os documentos para faturamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFinanceiraFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(unidadeTrabalho);

                Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCodigo(codigo);

                if (documentoFaturamento == null)
                    return new JsonpResult(false, true, "Documento para faturamento não encontrado.");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura = repConfiguracaoFinanceiraFatura.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoUso = servicoTitulo.ObterTipoMovimentoConfiguracaoFinanceiraFatura(documentoFaturamento.CTe, documentoFaturamento.Carga, configuracaoFinanceiraFatura);

                DateTime dataEmissao = DateTime.MinValue;
                string numeroDocumento = string.Empty;

                if (documentoFaturamento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga)
                {
                    dataEmissao = documentoFaturamento.Carga.CargaCTes.Select(o => o.CTe.DataEmissao.Value).FirstOrDefault();
                    numeroDocumento = documentoFaturamento.Carga.CodigoCargaEmbarcador;
                }
                else
                {
                    dataEmissao = documentoFaturamento.CTe.DataEmissao.Value;
                    numeroDocumento = documentoFaturamento.CTe.Numero.ToString() + "-" + documentoFaturamento.CTe.Serie.Numero.ToString();
                }

                return new JsonpResult(new
                {
                    documentoFaturamento.Codigo,
                    Tipo = documentoFaturamento.DescricaoTipoDocumento,
                    Documento = numeroDocumento,
                    DataEmissao = dataEmissao.ToString("dd/MM/yyyy"),
                    ValorDocumento = documentoFaturamento.ValorDocumento.ToString("n2"),
                    ValorAcrescimo = documentoFaturamento.ValorAcrescimo.ToString("n2"),
                    ValorAFaturar = documentoFaturamento.ValorAFaturar.ToString("n2"),
                    ValorDesconto = documentoFaturamento.ValorDesconto.ToString("n2"),
                    ValorEmFatura = documentoFaturamento.ValorEmFatura.ToString("n2"),
                    ValorPago = documentoFaturamento.ValorPago.ToString("n2"),
                    GrupoPessoas = new
                    {
                        Codigo = documentoFaturamento.GrupoPessoas?.Codigo ?? 0,
                        Descricao = documentoFaturamento.GrupoPessoas?.Descricao ?? string.Empty
                    },
                    Tomador = new
                    {
                        Codigo = documentoFaturamento.Tomador?.CPF_CNPJ_SemFormato ?? "0",
                        Descricao = documentoFaturamento.Tomador != null ? (documentoFaturamento.Tomador.Nome + " (" + documentoFaturamento.Tomador.CPF_CNPJ_Formatado + ")") : string.Empty
                    },
                    TipoMovimentoUso = new
                    {
                        Codigo = tipoMovimentoUso?.Codigo ?? 0,
                        Descricao = tipoMovimentoUso?.Descricao
                    }
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao obter os detalhes do documento para faturamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
            {
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CPFCNPJTomador = Request.GetDoubleParam("Tomador"),
                TiposPropostasMultimodal = Usuario?.PerfilAcesso?.TiposPropostasMultimodal?.ToList(),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal")
            };
        }

        #endregion
    }
}
