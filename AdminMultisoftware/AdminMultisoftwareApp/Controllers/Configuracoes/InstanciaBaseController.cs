using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/InstanciaBase")]
    public class InstanciaBaseController : BaseController
    {
        #region Métodos Globais     

        public ActionResult Pesquisa()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string servidor = Request.Params["Servidor"];
                string usuario = Request.Params["Usuario"];

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Servidor", "Servidor", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Usuário", "Usuario", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Porta", "Porta", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descricao", false);

                AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase repInstanciaBase = new AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase(unitOfWork);
                List<AdminMultisoftware.Dominio.Entidades.Configuracoes.InstanciaBase> listaInstanciaBase = repInstanciaBase.Consultar(servidor, usuario, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repInstanciaBase.ContarConsulta(servidor, usuario));
                var lista = (from p in listaInstanciaBase
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.Servidor,
                                 p.Usuario,
                                 p.Porta,
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
                AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase repInstanciaBase = new AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Configuracoes.InstanciaBase instanciaBase = new AdminMultisoftware.Dominio.Entidades.Configuracoes.InstanciaBase();

                instanciaBase.Servidor = Request.Params["Servidor"];
                instanciaBase.Usuario = Request.Params["Usuario"];
                instanciaBase.Senha = Request.Params["Senha"];
                instanciaBase.Porta = int.Parse(Request.Params["Porta"]);

                repInstanciaBase.Inserir(instanciaBase, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

                AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase repInstanciaBase = new AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Configuracoes.InstanciaBase instanciaBase = repInstanciaBase.BuscarPorCodigo(int.Parse(Request.Params["Codigo"]));

                instanciaBase.Initialize();
                instanciaBase.Servidor = Request.Params["Servidor"];
                instanciaBase.Usuario = Request.Params["Usuario"];
                instanciaBase.Porta = int.Parse(Request.Params["Porta"]);

                repInstanciaBase.Atualizar(instanciaBase, Auditado);

                if (!string.IsNullOrWhiteSpace(Request.Params["Senha"]))
                    instanciaBase.Senha = Request.Params["Senha"];

                repInstanciaBase.Atualizar(instanciaBase);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

                AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase repInstanciaBase = new AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Configuracoes.InstanciaBase instanciaBase = repInstanciaBase.BuscarPorCodigo(codigo);
                var dynInstanciaBase = new
                {
                    instanciaBase.Codigo,
                    instanciaBase.Servidor,
                    instanciaBase.Usuario,
                    instanciaBase.Porta
                };
                return new JsonpResult(dynInstanciaBase);
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
                int codigo = int.Parse(Request.Params["Codigo"]);
                AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase repInstanciaBase = new AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Configuracoes.InstanciaBase instanciaBase = repInstanciaBase.BuscarPorCodigo(codigo);
                repInstanciaBase.Deletar(instanciaBase, Auditado);
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