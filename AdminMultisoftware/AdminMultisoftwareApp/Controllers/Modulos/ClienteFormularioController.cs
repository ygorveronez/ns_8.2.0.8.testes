using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Modulos
{
    [CustomAuthorize("Modulos/ClienteFormulario")]
    public class ClienteFormularioController : BaseController
    {
        #region Métodos Globais

        public ActionResult Pesquisa()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCliente = int.Parse(Request.Params["Cliente"]);
                int codigoFormulario = int.Parse(Request.Params["Formulario"]);
                string descricao = Request.Params["Descricao"];

                bool usuarioEmbarcador = false;
                bool usuarioTMS = false;
                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"] != null)
                    grupos = Session["GrupoRedmine"] as List<String>;

                if (grupos.Contains("Torre Embarcador"))
                    usuarioEmbarcador = true;

                if (grupos.Contains("Torre Transportador"))
                    usuarioTMS = true;

                bool? exclusivo = null;
                bool exclusivoAux;
                if (bool.TryParse(Request.Params["FormularioExclusivo"], out exclusivoAux))
                    exclusivo = exclusivoAux;

                bool? bloqueado = null;
                bool bloqueadoAux;
                if (bool.TryParse(Request.Params["FormularioBloqueado"], out bloqueadoAux))
                    bloqueado = bloqueadoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Cliente", "Cliente", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Formulário", "Formulario", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Serviço", "TiposServicosMultisoftware", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Exclusivo", "FormularioExclusivo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Bloqueado", "FormularioBloqueado", 10, Models.Grid.Align.left, true);

                AdminMultisoftware.Repositorio.Modulos.ClienteFormulario repClienteFormulario = new AdminMultisoftware.Repositorio.Modulos.ClienteFormulario(unitOfWork);
                List<AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario> listaClienteFormulario = repClienteFormulario.Consultar(codigoCliente, usuarioEmbarcador, usuarioTMS, codigoFormulario, descricao, exclusivo, bloqueado, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repClienteFormulario.ContarConsulta(codigoCliente, usuarioEmbarcador, usuarioTMS, codigoFormulario, descricao, exclusivo, bloqueado));
                var lista = (from p in listaClienteFormulario
                             select new
                             {
                                 p.Codigo,
                                 Cliente = p.Cliente.RazaoSocial,
                                 Formulario = p.Formulario.Descricao,
                                 p.Formulario.TiposServicosMultisoftware,
                                 p.Descricao,
                                 FormularioExclusivo = p.FormularioExclusivo ? "Sim" : "Não",
                                 FormularioBloqueado = p.FormularioBloqueado ? "Sim" : "Não"
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
                int codigoFormulario;
                int.TryParse(Request.Params["Formulario"], out codigoFormulario);

                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"] != null)
                    grupos = Session["GrupoRedmine"] as List<String>;

                unitOfWork.Start();
                AdminMultisoftware.Repositorio.Modulos.ClienteFormulario repClienteFormulario = new AdminMultisoftware.Repositorio.Modulos.ClienteFormulario(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario clienteFormulario = new AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario();

                clienteFormulario.Cliente = codigoCliente > 0 ? new AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente() { Codigo = codigoCliente } : null;
                clienteFormulario.Formulario = codigoFormulario > 0 ? new AdminMultisoftware.Dominio.Entidades.Modulos.Formulario() { Codigo = codigoFormulario } : null;
                clienteFormulario.Descricao = Request.Params["Descricao"];
                clienteFormulario.FormularioExclusivo = bool.Parse(Request.Params["FormularioExclusivo"]);
                clienteFormulario.FormularioBloqueado = bool.Parse(Request.Params["FormularioBloqueado"]);

                if (VerificarFormularioExclusivo(codigoFormulario, repClienteFormulario, clienteFormulario, grupos))
                    return new JsonpResult(false, true, "Este formulário já está sendo utilizado por outro cliente, não é possível adicioná-lo como exclusivo.");

                repClienteFormulario.Inserir(clienteFormulario, Auditado);

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

                AdminMultisoftware.Repositorio.Modulos.ClienteFormulario repClienteFormulario = new AdminMultisoftware.Repositorio.Modulos.ClienteFormulario(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario clienteFormulario = repClienteFormulario.BuscarPorCodigo(int.Parse(Request.Params["Codigo"]));

                int codigoCliente;
                int.TryParse(Request.Params["Cliente"], out codigoCliente);
                int codigoFormulario;
                int.TryParse(Request.Params["Formulario"], out codigoFormulario);

                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"] != null)
                    grupos = Session["GrupoRedmine"] as List<String>;

                clienteFormulario.Initialize();
                clienteFormulario.Cliente = codigoCliente > 0 ? new AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente() { Codigo = codigoCliente } : null;
                clienteFormulario.Formulario = codigoFormulario > 0 ? new AdminMultisoftware.Dominio.Entidades.Modulos.Formulario() { Codigo = codigoFormulario } : null;
                clienteFormulario.Descricao = Request.Params["Descricao"];
                clienteFormulario.FormularioExclusivo = bool.Parse(Request.Params["FormularioExclusivo"]);
                clienteFormulario.FormularioBloqueado = bool.Parse(Request.Params["FormularioBloqueado"]);

                if (VerificarFormularioExclusivo(codigoFormulario, repClienteFormulario, clienteFormulario, grupos))
                    return new JsonpResult(false, true, "Não é possível adicionar este módulo como exclusivo, pois já está em uso por outros clientes.");

                repClienteFormulario.Atualizar(clienteFormulario, Auditado);

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

                AdminMultisoftware.Repositorio.Modulos.ClienteFormulario repClienteFormulario = new AdminMultisoftware.Repositorio.Modulos.ClienteFormulario(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario clienteFormulario = repClienteFormulario.BuscarPorCodigo(codigo);
                var dynClienteFormulario = new
                {
                    clienteFormulario.Codigo,
                    Cliente = new { clienteFormulario.Cliente.Codigo, Descricao = clienteFormulario.Cliente.RazaoSocial },
                    Formulario = new { clienteFormulario.Formulario.Codigo, clienteFormulario.Formulario.Descricao },
                    clienteFormulario.Descricao,
                    clienteFormulario.FormularioExclusivo,
                    clienteFormulario.FormularioBloqueado
                };
                return new JsonpResult(dynClienteFormulario);
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
                AdminMultisoftware.Repositorio.Modulos.ClienteFormulario repClienteFormulario = new AdminMultisoftware.Repositorio.Modulos.ClienteFormulario(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario clienteFormulario = repClienteFormulario.BuscarPorCodigo(codigo);
                repClienteFormulario.Deletar(clienteFormulario, Auditado);
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

        private static bool VerificarFormularioExclusivo(int codigoFormulario, AdminMultisoftware.Repositorio.Modulos.ClienteFormulario repositorioClienteFormulario, AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario clienteFormulario, List<string> grupos)
        {
            bool formularioExclusivo = repositorioClienteFormulario.VerificaFormularioExclusivo(codigoFormulario, clienteFormulario.Codigo);
            bool grupoInvalido = !grupos.Contains("Head") && !grupos.Contains("Dev");

            if (clienteFormulario.FormularioExclusivo && !formularioExclusivo && grupoInvalido)
                return true;

            return false;
        }

        #endregion
    }
}