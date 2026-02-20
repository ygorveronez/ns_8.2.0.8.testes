using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/AlteracaoProduto")]
    public class AlteracaoProdutoController : BaseController
    {
		#region Construtores

		public AlteracaoProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais        

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigoGrupoImposto = 0;
                int.TryParse(Request.Params("GrupoImposto"), out codigoGrupoImposto);
                string codigoNCM = Request.Params("CodigoNCM");
                string descricao = Request.Params("Descricao");
                string codigoBarrasEAN = Request.Params("CodigoBarrasEAN");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.EditableCell editableValorLiquido = null;
                Models.Grid.EditableCell editableValorString = null;
                editableValorLiquido = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9);
                editableValorString = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, 500);
                Models.Grid.EditableCell editableValorNCM = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, 8);
                Models.Grid.EditableCell editableValorCEST = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, 7);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("CodigoProduto", false);
                grid.AdicionarCabecalho("CodigoGrupoImposto", false);
                grid.AdicionarCabecalho("Codigo", "IdProduto", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cod. Produto", "Codigo", 10, Models.Grid.Align.center, true, false, false, false, true, editableValorString);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true, false, false, false, true, editableValorString);
                grid.AdicionarCabecalho("NCM", "NCM", 10, Models.Grid.Align.left, true, false, false, false, true, editableValorNCM);
                grid.AdicionarCabecalho("CEST", "CEST", 10, Models.Grid.Align.left, true, false, false, false, true, editableValorCEST);
                grid.AdicionarCabecalho("Cód. Barras EAN", "CodigoBarrasEAN", 10, Models.Grid.Align.left, true, false, false, false, true, editableValorString);
                grid.AdicionarCabecalho("Valor Venda", "Valor", 12, Models.Grid.Align.right, false, false, false, false, true, editableValorLiquido);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "IdProduto")
                    propOrdenar = "Codigo";
                else if (propOrdenar == "Codigo")
                    propOrdenar = "CodigoProduto";
                else if (propOrdenar == "NCM")
                    propOrdenar = "CodigoNCM";
                else if (propOrdenar == "CEST")
                    propOrdenar = "CodigoCEST";
                else if (propOrdenar == "CodigoBarrasEAN")
                    propOrdenar = "CodigoEAN";

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                List<Dominio.Entidades.Produto> listaProduto = repProduto.Consulta(codigoGrupoImposto, codigoBarrasEAN, 0, "", descricao, codigoNCM, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, null, codigoEmpresa, "", 0, false, false, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repProduto.ContaConsulta(codigoGrupoImposto, codigoBarrasEAN, 0, "", descricao, codigoNCM, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, null, codigoEmpresa, "", 0, false, false));
                var lista = (from p in listaProduto
                             select new
                             {
                                 CodigoProduto = p.Codigo,
                                 CodigoGrupoImposto = p.GrupoImposto != null ? p.GrupoImposto.Codigo : 0,
                                 IdProduto = p.Codigo,
                                 Codigo = !string.IsNullOrWhiteSpace(p.CodigoProduto) ? p.CodigoProduto : string.Empty,
                                 p.Descricao,
                                 NCM = !string.IsNullOrWhiteSpace(p.CodigoProduto) ? p.CodigoProduto : string.Empty,
                                 CEST = !string.IsNullOrWhiteSpace(p.CodigoProduto) ? p.CodigoProduto : string.Empty,
                                 CodigoBarrasEAN = !string.IsNullOrWhiteSpace(p.CodigoProduto) ? p.CodigoProduto : string.Empty,
                                 Valor = p.ValorVenda.ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os produtos ativos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);

                int codigoGrupoImpostoAlterar = 0;
                int.TryParse(Request.Params("GrupoImpostoAlterar"), out codigoGrupoImpostoAlterar);

                if (SalvarAlteracoesProdutos(unitOfWork, codigoGrupoImpostoAlterar))
                {
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true, "Sucesso");
                }
                return new JsonpResult(false, "Favor verifique os campos CEST, NCM e Descrição dos produtos selecionados.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os produtos selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarGrupoImposto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoGrupoImposto = 0;
                int.TryParse(Request.Params("GrupoImposto"), out codigoGrupoImposto);
                string codigoNCM = Request.Params("CodigoNCM");
                string descricao = Request.Params("Descricao");
                string codigoBarrasEAN = Request.Params("CodigoBarrasEAN");

                int codigoGrupoImpostoAlterar = 0;
                int.TryParse(Request.Params("GrupoImpostoAlterar"), out codigoGrupoImpostoAlterar);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

                if (codigoGrupoImpostoAlterar > 0)
                {
                    List<Dominio.Entidades.Produto> listaProduto = repProduto.Consulta(codigoGrupoImposto, codigoBarrasEAN, 0, "", descricao, codigoNCM, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, null, codigoEmpresa, "", 0, false, false, "", "", 0, 0);

                    foreach (var produto in listaProduto)
                    {
                        Dominio.Entidades.Produto prod = repProduto.BuscarPorCodigo(produto.Codigo, true);
                        prod.GrupoImposto = repGrupoImposto.BuscarPorCodigo(codigoGrupoImpostoAlterar);
                        repProduto.Atualizar(prod, Auditado);
                    }
                }
                else
                    return new JsonpResult(false, "Grupo de Imposto não selecionado.");

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os produtos selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        private bool SalvarAlteracoesProdutos(Repositorio.UnitOfWork unidadeDeTrabalho, int codigoGrupoImpostoAlterar)
        {
            Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unidadeDeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);

            if (!string.IsNullOrWhiteSpace(Request.Params("ListaProdutos")))
            {
                dynamic listaProdutos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaProdutos"));
                if (listaProdutos != null)
                {
                    foreach (var produto in listaProdutos)
                    {
                        Dominio.Entidades.Produto prod = repProduto.BuscarPorCodigo(int.Parse((string)produto.IdProduto), true);
                        prod.ValorVenda = decimal.Parse((string)produto.Valor);
                        prod.CodigoEAN = (string)produto.CodigoBarrasEAN;
                        prod.CodigoCEST = (string)produto.CEST;
                        prod.CodigoNCM = (string)produto.NCM;
                        prod.Descricao = (string)produto.Descricao;
                        prod.CodigoProduto = (string)produto.Codigo;
                        if (codigoGrupoImpostoAlterar > 0)
                            prod.GrupoImposto = repGrupoImposto.BuscarPorCodigo(codigoGrupoImpostoAlterar);

                        if (!string.IsNullOrWhiteSpace(prod.CodigoCEST) && prod.CodigoCEST.Length != 7)
                            return false;

                        if (string.IsNullOrWhiteSpace(prod.CodigoNCM) || prod.CodigoNCM.Length != 8)
                            return false;

                        if (string.IsNullOrWhiteSpace(prod.Descricao))
                            return false;

                        repProduto.Atualizar(prod, Auditado);
                    }
                }
            }

            return true;
        }
    }
}
