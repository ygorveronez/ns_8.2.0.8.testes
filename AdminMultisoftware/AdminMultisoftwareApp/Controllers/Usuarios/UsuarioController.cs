using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Usuarios
{
    [CustomAuthorize("Usuarios/Usuario")]
    public class UsuarioController : BaseController
    {
        #region Métodos Globais       
        public ActionResult Pesquisa()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string nome = Request.Params["Nome"];
                string login = Request.Params["Login"];

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nome", "Nome", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Usuário", "Login", 30, Models.Grid.Align.left, true);

                AdminMultisoftware.Repositorio.Pessoas.Usuario repUsuario = new AdminMultisoftware.Repositorio.Pessoas.Usuario(unitOfWork);
                List<AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario> listaUsuario = repUsuario.Consultar(nome, login, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repUsuario.ContarConsulta(nome, login));
                var lista = (from p in listaUsuario
                             select new
                             {
                                 p.Codigo,
                                 p.Nome,
                                 p.Login,
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
                AdminMultisoftware.Repositorio.Pessoas.Usuario repUsuario = new AdminMultisoftware.Repositorio.Pessoas.Usuario(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario usuario = new AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario();

                if (Request.Params["Senha"] == "")
                {
                    return new JsonpResult(true, "Por favor informe uma senha para o cadastro!");
                }
                else
                {

                    usuario.Nome = Request.Params["Nome"];
                    usuario.Login = Request.Params["Login"];
                    usuario.Senha = Criptografar(Request.Params["Senha"]);

                    repUsuario.Inserir(usuario, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

                AdminMultisoftware.Repositorio.Pessoas.Usuario repUsuario = new AdminMultisoftware.Repositorio.Pessoas.Usuario(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario usuario = repUsuario.BuscarPorCodigo(codigo);
                var dynFormulario = new
                {
                    usuario.Codigo,
                    usuario.Login,
                    usuario.Nome,
                };
                return new JsonpResult(dynFormulario);
            }
            catch (Exception)
            {
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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

                AdminMultisoftware.Repositorio.Pessoas.Usuario repUsuario = new AdminMultisoftware.Repositorio.Pessoas.Usuario(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario usuario = repUsuario.BuscarPorCodigo(int.Parse(Request.Params["Codigo"]));
                usuario.Initialize();
                usuario.Nome = Request.Params["Nome"];
                usuario.Login = Request.Params["Login"];

                if (Request.Params["Senha"] != "")
                {
                    usuario.Senha = Criptografar(Request.Params["Senha"]);
                }

                repUsuario.Atualizar(usuario, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                int codigo = int.Parse(Request.Params["Codigo"]);
                AdminMultisoftware.Repositorio.Pessoas.Usuario repUsuario = new AdminMultisoftware.Repositorio.Pessoas.Usuario(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario usuario = repUsuario.BuscarPorCodigo(codigo);
                repUsuario.Deletar(usuario, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vinculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string Criptografar(string senha)
        {
            senha = Servicos.Criptografia.GerarHashMD5(senha);

            return senha;
        }

        #endregion
    }
}