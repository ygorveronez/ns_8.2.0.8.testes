using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class UnidadeMedidaGeralController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("unidademedida.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.UnidadeMedidaGeral repUnidadeMedida = new Repositorio.UnidadeMedidaGeral(unitOfWork);

                List<Dominio.Entidades.UnidadeMedidaGeral> unidadesDeMedida = repUnidadeMedida.BuscarTodos(this.EmpresaUsuario.Codigo);

                var retorno = from obj in unidadesDeMedida
                              select new
                              {
                                  obj.Codigo,
                                  obj.Sigla,
                                  obj.Descricao
                              };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as unidades de medida.");
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
                string descricao = Request.Params["Descricao"];
                string sigla = Request.Params["Sigla"];
                string status = Request.Params["Status"];

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.UnidadeMedidaGeral repUnidadeMedida = new Repositorio.UnidadeMedidaGeral(unitOfWork);

                List<Dominio.Entidades.UnidadeMedidaGeral> unidadesDeMedida = repUnidadeMedida.Consultar(this.EmpresaUsuario.Codigo, descricao, sigla, status, inicioRegistros, 50);

                int countUnidadesMedida = repUnidadeMedida.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, sigla, status);

                var retorno = from obj in unidadesDeMedida
                              select new
                              {
                                  obj.Codigo,
                                  obj.Status,
                                  obj.Sigla,
                                  obj.Descricao,
                                  obj.DescricaoStatus
                              };

                return Json(retorno, true, null, new String[] { "Codigo", "Status", "Sigla|15", "Descrição|50", "Status|15" }, countUnidadesMedida);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as unidades de medida.");
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
                string sigla = Request.Params["Sigla"];
                string status = Request.Params["Status"];

                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "A descrição é obrigatória.");

                if (string.IsNullOrWhiteSpace(sigla))
                    return Json<bool>(false, false, "A sigla é obrigatória.");

                Repositorio.UnidadeMedidaGeral repUnidadeMedida = new Repositorio.UnidadeMedidaGeral(unitOfWork);

                Dominio.Entidades.UnidadeMedidaGeral unidadeMedida = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de Unidade de Medida negada!");

                    unidadeMedida = repUnidadeMedida.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de Unidade de Medida negada!");

                    unidadeMedida = new Dominio.Entidades.UnidadeMedidaGeral();

                    unidadeMedida.Empresa = this.EmpresaUsuario;
                }

                unidadeMedida.Descricao = descricao;
                unidadeMedida.Status = status;
                unidadeMedida.Sigla = sigla;

                if (unidadeMedida.Codigo > 0)
                    repUnidadeMedida.Atualizar(unidadeMedida);
                else
                    repUnidadeMedida.Inserir(unidadeMedida);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a unidade de medida.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
