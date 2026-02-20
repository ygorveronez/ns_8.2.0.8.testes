using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Modulos
{
    [CustomAuthorize("Modulos/PermissaoPersonalizada")]
    public class PermissaoPersonalizadaController : BaseController
    {
        #region Métodos Globais    
        public ActionResult Pesquisa()
        {

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string descricao = Request.Params["Descricao"];
                int formulario = int.Parse(Request.Params["Formulario"]);
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoPermissao", false);
                grid.AdicionarCabecalho("Formulário", "Formulario", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 15, Models.Grid.Align.left, false);


                AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada repPermissaoPersonalizada = new AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada(unitOfWork);
                List<AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada> listaPermissaoPersonalizada = repPermissaoPersonalizada.Consultar(descricao, formulario, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPermissaoPersonalizada.ContarConsulta(descricao, formulario));
                var lista = (from p in listaPermissaoPersonalizada
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.CodigoPermissao,
                                 Formulario = p.Formulario.Descricao,
                                 p.DescricaoAtivo,
                             }).ToList();
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

        public ActionResult Adicionar()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada repPermissaoPersonalizada = new AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada permissaoPersonalizada = new AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada();
                permissaoPersonalizada.Ativo = bool.Parse(Request.Params["Ativo"]);
                permissaoPersonalizada.Descricao = Request.Params["Descricao"];
                permissaoPersonalizada.Formulario = new AdminMultisoftware.Dominio.Entidades.Modulos.Formulario() { Codigo = int.Parse(Request.Params["Formulario"]) };
                permissaoPersonalizada.CodigoPermissao = (AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada)int.Parse(Request.Params["CodigoPermissao"]);
                permissaoPersonalizada.TranslationResourcePath = Request.Params["TranslationResourcePath"];

                if (repPermissaoPersonalizada.BuscarPorPermissaoECodigoFormulario(permissaoPersonalizada.CodigoPermissao, permissaoPersonalizada.Formulario.Codigo) == null)
                {
                    repPermissaoPersonalizada.Inserir(permissaoPersonalizada, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe essa Permissão Personalizada cadastrada para esse Formulário");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult Atualizar()
        {

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {

                unitOfWork.Start();

                AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada repPermissaoPersonalizada = new AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada permissaoPersonalizada = repPermissaoPersonalizada.BuscarPorCodigo(int.Parse(Request.Params["Codigo"]));
                permissaoPersonalizada.Initialize();
                permissaoPersonalizada.Ativo = bool.Parse(Request.Params["Ativo"]);
                permissaoPersonalizada.Descricao = Request.Params["Descricao"];
                permissaoPersonalizada.Formulario = new AdminMultisoftware.Dominio.Entidades.Modulos.Formulario() { Codigo = int.Parse(Request.Params["Formulario"]) };
                permissaoPersonalizada.CodigoPermissao = (AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada)int.Parse(Request.Params["CodigoPermissao"]);
                permissaoPersonalizada.TranslationResourcePath = Request.Params["TranslationResourcePath"];

                AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada permissaoPersonalizadaExiste = repPermissaoPersonalizada.BuscarPorPermissaoECodigoFormulario(permissaoPersonalizada.CodigoPermissao, permissaoPersonalizada.Formulario.Codigo);
                if(permissaoPersonalizadaExiste == null || permissaoPersonalizadaExiste.Codigo == permissaoPersonalizada.Codigo)
                {
                    repPermissaoPersonalizada.Atualizar(permissaoPersonalizada, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe essa Permissão Personalizada cadastrada para esse Formulário");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult BuscarPorCodigo()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params["Codigo"]);

                AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada repPermissaoPersonalizada = new AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada permissaoPersonalizada = repPermissaoPersonalizada.BuscarPorCodigo(codigo);
                var dynPermissaoPersonalizada = new
                {
                    permissaoPersonalizada.Codigo,
                    permissaoPersonalizada.CodigoPermissao,
                    permissaoPersonalizada.Descricao,
                    Formulario = new { permissaoPersonalizada.Formulario.Codigo, permissaoPersonalizada.Formulario.Descricao },
                    permissaoPersonalizada.Ativo,
                    permissaoPersonalizada.TranslationResourcePath
            };
                return new JsonpResult(dynPermissaoPersonalizada);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public ActionResult ExcluirPorCodigo()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params["codigo"]);
                AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada repPermissaoPersonalizada = new AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada permissaoPersonalizada = repPermissaoPersonalizada.BuscarPorCodigo(codigo);
                repPermissaoPersonalizada.Deletar(permissaoPersonalizada, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vinculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}