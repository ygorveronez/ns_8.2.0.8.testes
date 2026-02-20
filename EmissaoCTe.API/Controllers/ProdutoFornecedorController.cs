using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ProdutoFornecedorController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("produtofornecedor.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string numero = Request.Params["Numero"];
                string produto = Request.Params["Produto"];

                double cnpjFornecedor;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Fornecedor"]), out cnpjFornecedor);

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unitOfWork);

                List<Dominio.Entidades.ProdutoFornecedor> produtos = repProdutoFornecedor.Consultar(this.EmpresaUsuario.Codigo, numero, produto, cnpjFornecedor, inicioRegistros, 50);

                int count = repProdutoFornecedor.ContarConsulta(this.EmpresaUsuario.Codigo, numero, produto, cnpjFornecedor);

                var retorno = from obj in produtos
                              select new
                              {
                                  obj.Codigo,
                                  FornecedorCodigo = obj.Fornecedor.Codigo,
                                  ProdutoCodigo = obj.Produto.Codigo,

                                  NumeroFornecedor = obj.CodigoProduto,
                                  ProdutoDescricao = obj.Produto.Descricao,
                                  FornecedorDescricao = string.Concat(obj.Fornecedor.CPF_CNPJ_Formatado, " - ", obj.Fornecedor.Nome),
                              };

                return Json(retorno, true, null, new String[] { "Codigo", "FornecedorCodigo", "ProdutoCodigo", "Número|22", "Produto|25", "Fornecedor|43" }, count);
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
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoProduto = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Produto"], out codigoProduto);

                double cnpjFornecedor;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Fornecedor"]), out cnpjFornecedor);

                string numeroFornecedor = Request.Params["NumeroFornecedor"];

                Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeDeTrabalho);
                Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);
                Repositorio.Cliente repFornecedor = new Repositorio.Cliente(unidadeDeTrabalho);

                Dominio.Entidades.ProdutoFornecedor produtoFornecedor;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada!");

                    produtoFornecedor = repProdutoFornecedor.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada!");

                    produtoFornecedor = new Dominio.Entidades.ProdutoFornecedor();
                }

                // Valida dados de entrada
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoProduto);
                if (produto == null)
                    return Json<bool>(false, false, "Produto não encontrado!");

                Dominio.Entidades.Cliente fornecedor = repFornecedor.BuscarPorCPFCNPJ(cnpjFornecedor);
                if (fornecedor == null)
                    return Json<bool>(false, false, "Fornecedor não encontrado!");
                
                if (string.IsNullOrWhiteSpace(numeroFornecedor))
                    return Json<bool>(false, false, "Nenhum número do fornecedor informado!");

                // Regras
                // Codigo do fornecedor precisa ser unico pra cada fornecedor
                if (repProdutoFornecedor.BuscarPorProdutoEFornecedor(numeroFornecedor, cnpjFornecedor) != null)
                    return Json<bool>(false, false, "Já existe esse número para esse fornecedor!");

                produtoFornecedor.Produto = produto;
                produtoFornecedor.Fornecedor = fornecedor;
                produtoFornecedor.CodigoProduto = numeroFornecedor;

                if (produtoFornecedor.Codigo > 0)
                    repProdutoFornecedor.Atualizar(produtoFornecedor);
                else
                    repProdutoFornecedor.Inserir(produtoFornecedor);
                
                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os dados.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Excluir()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                
                Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeDeTrabalho);

                Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo); ;

                if (this.Permissao() == null || this.Permissao().PermissaoDeDelecao != "A")
                    return Json<bool>(false, false, "Permissão para exclusão negada!");

                if (produtoFornecedor == null)
                    return Json<bool>(false, false, "Produto não encontrado!");

                repProdutoFornecedor.Deletar(produtoFornecedor);

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao excluir os dados.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
 }