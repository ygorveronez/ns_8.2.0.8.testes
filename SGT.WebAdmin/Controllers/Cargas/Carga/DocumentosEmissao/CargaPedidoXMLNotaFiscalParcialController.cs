using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento", "CargaPedidoXMLNotaFiscalParcial/Pesquisa")]
    public class CargaPedidoXMLNotaFiscalParcialController : BaseController
    {
		#region Construtores

		public CargaPedidoXMLNotaFiscalParcialController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int cargaPedido = int.Parse(Request.Params("CodigoCargaPedido"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Chave", "Chave", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número DT", "Pedido", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Fatura", "NumeroFatura", 20, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> cargaPedidoXMLNotaFiscalParcial = repCargaPedidoXMLNotaFiscalParcial.Consultar(cargaPedido, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                
                grid.setarQuantidadeTotal(repCargaPedidoXMLNotaFiscalParcial.ContarConsulta(cargaPedido));
                var dynXmlNotaFiscal = (from obj in cargaPedidoXMLNotaFiscalParcial
                                        select new
                                        {
                                            obj.Codigo,
                                            Numero = obj.Numero > 0 ? obj.Numero.ToString() : "",
                                            obj.Chave,
                                            obj.Pedido,
                                            obj.NumeroFatura
                                        }).ToList();

                grid.AdicionaRows(dynXmlNotaFiscal);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repositorio = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial notaParcial = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (notaParcial == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(notaParcial, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VincularNotasFiscaisParciaisNovamente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (!ConfiguracaoEmbarcador.VincularNotasParciaisPedidoPorProcesso)
                    return new JsonpResult(false, true, "O processo para vinculo de notas fiscais automaticamente não está habilitado.");

                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    return new JsonpResult(false, true, "A situação da carga não permite que o vínculo das notas fiscais seja realizado novamente.");

                if (carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga já está com os documentos fiscais sendo processados, não sendo possível realizar o vínculo dos mesmos.");

                unitOfWork.Start();

                repCargaPedidoXMLNotaFiscalParcial.ReprocessarNotasParciaisPorCarga(carga.Codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Reenviou as notas fiscais parciais para processo de vínculo automático.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as notas fiscais parciais da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
