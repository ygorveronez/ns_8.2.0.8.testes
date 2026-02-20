using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Modulos
{
    [CustomAuthorize("Modulos/ClienteModulo")]
    public class ClienteModuloController : BaseController
    {
        #region Métodos Globais

        public ActionResult Pesquisa()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCliente = int.Parse(Request.Params["Cliente"]);
                int codigoModulo = int.Parse(Request.Params["Modulo"]);
                string descricao = Request.Params["Descricao"];

                bool? exclusivo = null;
                bool exclusivoAux;
                if (bool.TryParse(Request.Params["ModuloExclusivo"], out exclusivoAux))
                    exclusivo = exclusivoAux;

                bool? bloqueado = null;
                bool bloqueadoAux;
                if (bool.TryParse(Request.Params["ModuloBloqueado"], out bloqueadoAux))
                    bloqueado = bloqueadoAux;

                bool usuarioEmbarcador = false;
                bool usuarioTMS = false;

                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"] != null)
                    grupos = Session["GrupoRedmine"] as List<String>;

                if (grupos.Contains("Torre Embarcador"))
                    usuarioEmbarcador = true;

                if (grupos.Contains("Torre Transportador"))
                    usuarioTMS = true;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Cliente", "Cliente", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Módulo", "Modulo", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Serviço", "TiposServicosMultisoftware", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Exclusivo", "ModuloExclusivo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Bloqueado", "ModuloBloqueado", 10, Models.Grid.Align.left, true);

                AdminMultisoftware.Repositorio.Modulos.ClienteModulo repClienteModulo = new AdminMultisoftware.Repositorio.Modulos.ClienteModulo(unitOfWork);
                List<AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo> listaClienteModulo = repClienteModulo.Consultar(codigoCliente, usuarioEmbarcador, usuarioTMS, codigoModulo, descricao, exclusivo, bloqueado, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repClienteModulo.ContarConsulta(codigoCliente, usuarioEmbarcador, usuarioTMS, codigoModulo, descricao, exclusivo, bloqueado));
                var lista = (from p in listaClienteModulo
                             select new
                             {
                                 p.Codigo,
                                 Cliente = p.Cliente.RazaoSocial,
                                 Modulo = p.Modulo.Descricao,
                                 p.Modulo.TiposServicosMultisoftware,
                                 p.Descricao,
                                 ModuloExclusivo = p.ModuloExclusivo ? "Sim" : "Não",
                                 ModuloBloqueado = p.ModuloBloqueado ? "Sim" : "Não"
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
                int codigoCliente;
                int.TryParse(Request.Params["Cliente"], out codigoCliente);
                int codigoModulo;
                int.TryParse(Request.Params["Modulo"], out codigoModulo);
                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"] != null)
                    grupos = Session["GrupoRedmine"] as List<String>;

                unitOfWork.Start();
                AdminMultisoftware.Repositorio.Modulos.ClienteModulo repClienteModulo = new AdminMultisoftware.Repositorio.Modulos.ClienteModulo(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo clienteModulo = new AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo();

                clienteModulo.Initialize();
                clienteModulo.Cliente = codigoCliente > 0 ? new AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente() { Codigo = codigoCliente } : null;
                clienteModulo.Modulo = codigoModulo > 0 ? new AdminMultisoftware.Dominio.Entidades.Modulos.Modulo() { Codigo = codigoModulo } : null;
                clienteModulo.Descricao = Request.Params["Descricao"];
                clienteModulo.ModuloExclusivo = bool.Parse(Request.Params["ModuloExclusivo"]);
                clienteModulo.ModuloBloqueado = bool.Parse(Request.Params["ModuloBloqueado"]);

                if (VerificaModuloExclusivo(codigoModulo, repClienteModulo, clienteModulo, grupos))
                    return new JsonpResult(false, true, "Não é possível adicionar este módulo como exclusivo, pois já está em uso por outros clientes.");

                repClienteModulo.Inserir(clienteModulo, Auditado);

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

                AdminMultisoftware.Repositorio.Modulos.ClienteModulo repClienteModulo = new AdminMultisoftware.Repositorio.Modulos.ClienteModulo(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo clienteModulo = repClienteModulo.BuscarPorCodigo(int.Parse(Request.Params["Codigo"]));

                int codigoCliente;
                int.TryParse(Request.Params["Cliente"], out codigoCliente);
                int codigoModulo;
                int.TryParse(Request.Params["Modulo"], out codigoModulo);

                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"] != null)
                    grupos = Session["GrupoRedmine"] as List<String>;

                clienteModulo.Cliente = codigoCliente > 0 ? new AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente() { Codigo = codigoCliente } : null;
                clienteModulo.Modulo = codigoModulo > 0 ? new AdminMultisoftware.Dominio.Entidades.Modulos.Modulo() { Codigo = codigoModulo } : null;
                clienteModulo.Descricao = Request.Params["Descricao"];
                clienteModulo.ModuloExclusivo = bool.Parse(Request.Params["ModuloExclusivo"]);
                clienteModulo.ModuloBloqueado = bool.Parse(Request.Params["ModuloBloqueado"]);

                if (VerificaModuloExclusivo(codigoModulo, repClienteModulo, clienteModulo, grupos))
                    return new JsonpResult(false, true, "Não é possível adicionar este módulo como exclusivo, pois já está em uso por outros clientes.");

                repClienteModulo.Atualizar(clienteModulo, Auditado);

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

                AdminMultisoftware.Repositorio.Modulos.ClienteModulo repClienteModulo = new AdminMultisoftware.Repositorio.Modulos.ClienteModulo(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo clienteModulo = repClienteModulo.BuscarPorCodigo(codigo);
                var dynClienteModulo = new
                {
                    clienteModulo.Codigo,
                    Cliente = new { clienteModulo.Cliente.Codigo, Descricao = clienteModulo.Cliente.RazaoSocial },
                    Modulo = new { clienteModulo.Modulo.Codigo, clienteModulo.Modulo.Descricao },
                    clienteModulo.Descricao,
                    clienteModulo.ModuloExclusivo,
                    clienteModulo.ModuloBloqueado
                };
                return new JsonpResult(dynClienteModulo);
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
                AdminMultisoftware.Repositorio.Modulos.ClienteModulo repClienteModulo = new AdminMultisoftware.Repositorio.Modulos.ClienteModulo(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo clienteModulo = repClienteModulo.BuscarPorCodigo(codigo);
                repClienteModulo.Deletar(clienteModulo, Auditado);
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

        #region Métodos Privados

        private static bool VerificaModuloExclusivo(int codigoModulo, AdminMultisoftware.Repositorio.Modulos.ClienteModulo repositorioClienteModulo, AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo clienteModulo, List<string> grupos)
        {
            bool moduloExclusivo = repositorioClienteModulo.VerificaModuloExclusivo(codigoModulo, clienteModulo.Codigo);
            bool grupoInvalido = !grupos.Contains("Head") && !grupos.Contains("Dev");

            if (clienteModulo.ModuloExclusivo && !moduloExclusivo && grupoInvalido)
                return true;

            return false;
        }

        #endregion
    }
}