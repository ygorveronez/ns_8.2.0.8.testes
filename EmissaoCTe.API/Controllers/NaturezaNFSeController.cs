using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class NaturezaNFSeController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("naturezasnfse.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult ObterNaturezasDaEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.NaturezaNFSe repNaturezaNFSe = new Repositorio.NaturezaNFSe(unitOfWork);
                List<Dominio.Entidades.NaturezaNFSe> listaNaturezas = repNaturezaNFSe.BuscarPorEmpresa(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Codigo : 1, "A");

                var result = from obj in listaNaturezas select new { obj.Codigo, obj.Descricao };

                return Json(result, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as naturezas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];

                Repositorio.NaturezaNFSe repNaturezaNFSe = new Repositorio.NaturezaNFSe(unitOfWork);
                List<Dominio.Entidades.NaturezaNFSe> listaNaturezas = repNaturezaNFSe.Consultar(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai.Codigo, descricao, status, inicioRegistros, 50);
                int countNaturezas = repNaturezaNFSe.ContarConsulta(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai.Codigo, descricao, status);

                var retorno = from obj in listaNaturezas
                              select new
                              {
                                  obj.Codigo,
                                  obj.Numero,
                                  obj.Descricao
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Número|15", "Descrição|70" }, countNaturezas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as naturezas.");
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
                int codigo;
                int.TryParse(Request.Params["CodigoNatureza"], out codigo);

                Repositorio.NaturezaNFSe repNatureza = new Repositorio.NaturezaNFSe(unitOfWork);

                Dominio.Entidades.NaturezaNFSe natureza = repNatureza.BuscarPorCodigo(codigo);

                if (natureza == null)
                    return Json<bool>(false, false, "Natureza não encontrada. Atualize a página e tente novamente.");

                var retorno = new
                {
                    natureza.Codigo,
                    natureza.Descricao,
                    natureza.Numero,
                    natureza.Status
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da natureza.");
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
                int codigo, numero;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Numero"], out numero);

                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");

                Repositorio.NaturezaNFSe repNatureza = new Repositorio.NaturezaNFSe(unitOfWork);

                Dominio.Entidades.NaturezaNFSe natureza = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    natureza = repNatureza.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");

                    natureza = new Dominio.Entidades.NaturezaNFSe();
                }

                natureza.Descricao = descricao;
                natureza.Empresa = this.EmpresaUsuario;
                natureza.Numero = numero;
                natureza.Status = status;

                if (natureza.Codigo > 0)
                    repNatureza.Atualizar(natureza);
                else
                    repNatureza.Inserir(natureza);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a natureza.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
