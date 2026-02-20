using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ProdutoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("produtos.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {                
                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];
                string codigoProduto = Request.Params["CodigoProduto"];

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

                List<Dominio.Entidades.Produto> produtos = repProduto.Consultar(this.EmpresaUsuario.Codigo, descricao, status, codigoProduto, inicioRegistros, 50);

                int countProdutos = repProduto.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, status, codigoProduto);

                var retorno = from obj in produtos
                              select new
                              {
                                  obj.Codigo,
                                  obj.Descricao,
                                  CodigoProduto = obj.CodigoProduto,
                                  obj.DescricaoStatus
                              };

                return Json(retorno, true, null, new String[] { "Codigo", "Descrição|40", "Código Produto|20", "Status|20" }, countProdutos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os produtos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];
                string codigoProduto = Request.Params["CodigoProduto"];

                int codigoNCM, codigoUnidadeMedida, codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["NCM"], out codigoNCM);
                int.TryParse(Request.Params["UnidadeMedida"], out codigoUnidadeMedida);

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "A descrição é obrigatória.");

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

                Dominio.Entidades.Produto produto = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de Produto negada!");

                    produto = repProduto.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de Produto negada!");

                    produto = new Dominio.Entidades.Produto();

                    produto.Empresa = this.EmpresaUsuario;
                }

                Repositorio.NCM repNCM = new Repositorio.NCM(unitOfWork);
                Repositorio.UnidadeMedidaGeral repUnidadeMedida = new Repositorio.UnidadeMedidaGeral(unitOfWork);

                produto.Descricao = descricao;
                produto.Status = status;
                produto.UnidadeMedida = repUnidadeMedida.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoUnidadeMedida);
                produto.NCM = repNCM.BuscarPorCodigo(codigoNCM);
                produto.CodigoProduto = codigoProduto;

                if (produto.UnidadeMedida == null)
                    return Json<bool>(false, false, "Unidade de medida é obrigatório.");

                if (produto.NCM == null)
                    return Json<bool>(false, false, "NCM é obrigatório.");

                if (produto.Codigo > 0)
                    repProduto.Atualizar(produto);
                else
                    repProduto.Inserir(produto);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o produto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoProduto = 0;
                int.TryParse(Request.Params["CodigoProduto"], out codigoProduto);

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoProduto);

                if (produto == null)
                    return Json<bool>(false, false, "Produto não encontrado.");

                var retorno = new
                {
                    produto.Codigo,
                    produto.Descricao,
                    CodigoNCM = produto.NCM.Codigo,
                    DescricaoNCM = produto.NCM.Numero + " - " + produto.NCM.Descricao,
                    produto.Status,
                    CodigoUnidadeMedida = produto.UnidadeMedida.Codigo,
                    DescricaoUnidadeMedida = produto.UnidadeMedida.Sigla + " - " + produto.UnidadeMedida.Descricao,
                    produto.CodigoProduto
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do produto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
