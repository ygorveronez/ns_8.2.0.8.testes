using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BaixaTituloReceberNovo")]
    public class BaixaTituloReceberNovoAgrupadoDocumentoController : BaseController
    {
		#region Construtores

		public BaixaTituloReceberNovoAgrupadoDocumentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloBaixaAgrupadoDocumento()
                {
                    CodigoTituloBaixa = Request.GetIntParam("TituloBaixa"),
                    NumeroCTe = Request.GetIntParam("NumeroCTe"),
                    NumeroTitulo = Request.GetIntParam("NumeroTitulo"),
                    NumeroCarga = Request.GetStringParam("NumeroCarga"),
                    CodigoTomador = Request.GetDoubleParam("Tomador"),
                    CodigoDocumento = Request.GetIntParam("DocumentoCTe"),
                };

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(filtrosPesquisa.CodigoTituloBaixa);

                Models.Grid.EditableCell editValorPago = null;

                if (tituloBaixa != null && (tituloBaixa.SituacaoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada || tituloBaixa.SituacaoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao))
                    editValorPago = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 11);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = tituloBaixa.MoedaCotacaoBancoCentral ?? MoedaCotacaoBancoCentral.Real;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCTe", false);
                grid.AdicionarCabecalho("TipoDocumento", false);
                grid.AdicionarCabecalho("SituacaoBaixaTitulo", false);
                grid.AdicionarCabecalho("Título", "Titulo", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Documento", "NumeroDocumento", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Doc. Orig.", "DocumentoVinculado", 8, Models.Grid.Align.left, false);

                if (moeda != MoedaCotacaoBancoCentral.Real)
                {
                    string prefixoMoeda = moeda.ObterSigla();

                    grid.AdicionarCabecalho("Tomador", "Tomador", 12, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho($"{prefixoMoeda} Vl. em Título", "ValorTotalMoeda", 8, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho($"{prefixoMoeda} Vl. Acréscimo", "ValorAcrescimoMoeda", 8, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho($"{prefixoMoeda} Vl. Desconto", "ValorDescontoMoeda", 8, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho($"{prefixoMoeda} Vl. Total A Pagar", "ValorTotalAPagarMoeda", 8, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho($"{prefixoMoeda} Vl. Pago", "ValorPagoMoeda", 8, Models.Grid.Align.left, true, true, false, false, true, editValorPago);
                }
                else
                {
                    grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Data Base", "DataBase", 8, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Data do Pagamento", "DataBaixa", 8, Models.Grid.Align.left, true);
                }

                grid.AdicionarCabecalho("Vl. Avaria", "ValorAvaria", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. em Título", "ValorTotal", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Acréscimo", "ValorAcrescimo", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Desconto", "ValorDesconto", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Total A Pagar", "ValorTotalAPagar", 8, Models.Grid.Align.left, true);

                if (moeda != MoedaCotacaoBancoCentral.Real)
                    grid.AdicionarCabecalho("Vl. Pago", "ValorPago", 8, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("Vl. Pago", "ValorPago", 8, Models.Grid.Align.left, true, true, false, false, true, editValorPago);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "DataBase" || propOrdena == "Titulo" || propOrdena == "DataBaixa" || propOrdena == "DataBaixa")
                    propOrdena = "TituloBaixaAgrupado." + propOrdena;
                else if (propOrdena == "NumeroDocumento" || propOrdena == "ValorTotal")
                    propOrdena = "TituloDocumento." + propOrdena;

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> lTituloBaixaAgrupadoDocumento = repTituloBaixaAgrupadoDocumento.ConsultarPorTituloBaixa(filtrosPesquisa, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                int count = repTituloBaixaAgrupadoDocumento.ContarConsultaPorTituloBaixa(filtrosPesquisa);

                grid.setarQuantidadeTotal(count);

                var lista = (from p in lTituloBaixaAgrupadoDocumento
                             select ObterDadosGridTituloBaixaAgrupadoDocumento(p)).ToList();

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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarValorPago()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoTituloBaixaDocumento);

                decimal.TryParse(Request.Params("ValorPago"), out decimal valorPago);
                decimal.TryParse(Request.Params("ValorPagoMoeda"), out decimal valorPagoMoeda);

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = repTituloBaixaAgrupadoDocumento.BuscarPorCodigo(codigoTituloBaixaDocumento);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa;
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.MoedaCotacaoBancoCentral ?? MoedaCotacaoBancoCentral.Real;

                if (tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao &&
                    tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "A situação da baixa não permite que o valor pago seja alterado.");

                unitOfWork.Start();

                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AjustarValorPagoDocumento(tituloBaixaAgrupadoDocumento, moeda != MoedaCotacaoBancoCentral.Real, ref valorPago, ref valorPagoMoeda, unitOfWork);

                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixaAgrupado(ref tituloBaixaAgrupado, unitOfWork);
                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixa(ref tituloBaixa, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixaAgrupadoDocumento, null, "Alterou o valor pago.", unitOfWork);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    Negociacao = Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesNegociacaoBaixa(tituloBaixa, this.Usuario, ConfiguracaoEmbarcador, unitOfWork),
                    Row = ObterDadosGridTituloBaixaAgrupadoDocumento(tituloBaixaAgrupadoDocumento)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterDadosGridTituloBaixaAgrupadoDocumento(Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento)
        {
            return new
            {
                tituloBaixaAgrupadoDocumento.Codigo,
                CodigoCTe = tituloBaixaAgrupadoDocumento.TituloDocumento.CTe?.Codigo ?? 0,
                TipoDocumento = tituloBaixaAgrupadoDocumento.TituloDocumento.TipoDocumento,
                tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.SituacaoBaixaTitulo,
                Titulo = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.Titulo.Codigo,
                NumeroDocumento = tituloBaixaAgrupadoDocumento.TituloDocumento.NumeroDocumento,
                Tomador = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.Titulo.DescricaoTomador,
                DataBase = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.DataBase.ToString("dd/MM/yyyy"),
                DataBaixa = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.DataBaixa.ToString("dd/MM/yyyy"),
                ValorTotal = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorTotal.ToString("n2"),
                ValorPago = tituloBaixaAgrupadoDocumento.ValorPago.ToString("n2"),
                ValorAcrescimo = tituloBaixaAgrupadoDocumento.ValorAcrescimo.ToString("n2"),
                ValorDesconto = tituloBaixaAgrupadoDocumento.ValorDesconto.ToString("n2"),
                ValorTotalAPagar = tituloBaixaAgrupadoDocumento.ValorTotalAPagar.ToString("n2"),
                ValorTotalMoeda = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorTotalMoeda.ToString("n2"),
                ValorPagoMoeda = tituloBaixaAgrupadoDocumento.ValorPagoMoeda.ToString("n2"),
                ValorAcrescimoMoeda = tituloBaixaAgrupadoDocumento.ValorAcrescimoMoeda.ToString("n2"),
                ValorDescontoMoeda = tituloBaixaAgrupadoDocumento.ValorDescontoMoeda.ToString("n2"),
                ValorTotalAPagarMoeda = tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda.ToString("n2"),
                ValorAvaria = tituloBaixaAgrupadoDocumento.ValorAvaria.ToString("n2"),
                DocumentoVinculado = tituloBaixaAgrupadoDocumento.TituloDocumento.CTe?.NumeroDocumentosOriginarios ?? ""
            };
        }

        #endregion
    }
}
