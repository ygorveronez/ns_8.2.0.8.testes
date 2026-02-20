using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.Bordero
{
    [CustomAuthorize("Financeiros/Bordero")]
    public class BorderoTituloDocumentoController : BaseController
    {
		#region Construtores

		public BorderoTituloDocumentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoBorderoTitulo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoDocumento", false);
                grid.AdicionarCabecalho("CodigoCTe", false);
                grid.AdicionarCabecalho("Modelo", false);
                grid.AdicionarCabecalho("Documento", "Documento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Origem", "Origem", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", "Destino", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Vl. a Cobrar", "ValorACobrar", 10, Models.Grid.Align.left, true, false, false, false, true, new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9));
                grid.AdicionarCabecalho("Vl. Acréscimo", "ValorTotalAcrescimo", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Desconto", "ValorTotalDesconto", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Total", "ValorTotalACobrar", 10, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento> listaBorderoTituloDocumentos = repBorderoTituloDocumento.Consultar(codigoBorderoTitulo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                int countTitulosBordero = repBorderoTituloDocumento.ContarConsulta(codigoBorderoTitulo);

                var lista = (from p in listaBorderoTituloDocumentos select ObterDetalhesGrid(p)).ToList();

                grid.setarQuantidadeTotal(countTitulosBordero);
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

        public async Task<IActionResult> AlterarDadosDocumento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoBorderoTituloDocumento);
                decimal.TryParse(Request.Params("ValorACobrar"), out decimal valorACobrar);

                Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento borderoTituloDocumento = repBorderoTituloDocumento.BuscarPorCodigo(codigoBorderoTituloDocumento);

                if (borderoTituloDocumento.BorderoTitulo.Bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento)
                    return new JsonpResult(false, true, "Não é possível alterar os dados do documento na situação atual do borderô.");

                unidadeTrabalho.Start();

                if (valorACobrar > borderoTituloDocumento.TituloDocumento.ValorPendente)
                    valorACobrar = borderoTituloDocumento.TituloDocumento.ValorPendente;

                borderoTituloDocumento.ValorACobrar = valorACobrar;
                borderoTituloDocumento.ValorTotalACobrar = borderoTituloDocumento.ValorACobrar - borderoTituloDocumento.ValorTotalDesconto + borderoTituloDocumento.ValorTotalAcrescimo;

                if (borderoTituloDocumento.ValorTotalACobrar < 0)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não é possível alterar para este valor, pois o total ficará negativo.");
                }

                repBorderoTituloDocumento.Atualizar(borderoTituloDocumento);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = borderoTituloDocumento.BorderoTitulo.Bordero;
                Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo borderoTitulo = borderoTituloDocumento.BorderoTitulo;

                Servicos.Embarcador.Financeiro.Bordero.AtualizarTotaisBorderoTitulo(ref borderoTitulo, unidadeTrabalho);
                Servicos.Embarcador.Financeiro.Bordero.AtualizarTotaisBordero(ref bordero, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, borderoTituloDocumento, null, "Alterou Título.", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, bordero, null, "Alterou o Título" + borderoTituloDocumento.Descricao + ".", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new
                {
                    Bordero = Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero),
                    BorderoTituloDocumento = ObterDetalhesGrid(borderoTituloDocumento)
                });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar os documentos na fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privada

        private dynamic ObterDetalhesGrid(Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento borderoTituloDocumento)
        {
            return new
            {
                borderoTituloDocumento.Codigo,
                CodigoCTe = borderoTituloDocumento.TituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe ? borderoTituloDocumento.TituloDocumento.CTe.Codigo : 0,
                borderoTituloDocumento.TituloDocumento.TipoDocumento,
                Modelo = borderoTituloDocumento.TituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe ? borderoTituloDocumento.TituloDocumento.CTe.ModeloDocumentoFiscal?.Numero ?? string.Empty : string.Empty,
                Documento = borderoTituloDocumento.TituloDocumento.NumeroDocumento,
                Origem = borderoTituloDocumento.TituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe ? borderoTituloDocumento.TituloDocumento.CTe.LocalidadeInicioPrestacao.DescricaoCidadeEstado : borderoTituloDocumento.TituloDocumento.Carga.DadosSumarizados.Origens,
                Destino = borderoTituloDocumento.TituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe ? borderoTituloDocumento.TituloDocumento.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado : borderoTituloDocumento.TituloDocumento.Carga.DadosSumarizados.Destinos,
                ValorACobrar = borderoTituloDocumento.ValorACobrar.ToString("n2"),
                ValorTotalAcrescimo = borderoTituloDocumento.ValorTotalAcrescimo.ToString("n2"),
                ValorTotalDesconto = borderoTituloDocumento.ValorTotalDesconto.ToString("n2"),
                ValorTotalACobrar = borderoTituloDocumento.ValorTotalACobrar.ToString("n2")
            };
        }

        #endregion
    }
}
