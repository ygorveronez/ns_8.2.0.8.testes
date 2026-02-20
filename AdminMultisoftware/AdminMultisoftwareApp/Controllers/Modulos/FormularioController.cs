using AdminMultisoftware.Dominio.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Formularios
{
    [CustomAuthorize("Modulos/Formulario")]
    public class FormularioController : BaseController
    {
        #region Métodos Globais   
        public ActionResult Pesquisa()
        {

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string descricao = Request.Params["Descricao"];
                int modulo = int.Parse(Request.Params["Modulo"]);
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
                grid.AdicionarCabecalho("Sequencia", "Sequencia", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Codigo Formulário", "CodigoFormulario", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modulo", "Modulo", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Serviço", "TiposServicosMultisoftware", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 15, Models.Grid.Align.left, false);


                AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWork);
                List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> listaFormulario = repFormulario.Consultar(descricao, usuarioEmbarcador, usuarioTMS, modulo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFormulario.ContarConsulta(descricao, modulo, usuarioEmbarcador, usuarioTMS));
                var lista = (from p in listaFormulario
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.Sequencia,
                                 p.CodigoFormulario,
                                 Modulo = p.Modulo.Descricao,
                                 p.DescricaoAtivo,
                                 p.TiposServicosMultisoftware,
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
                List<AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware> tiposServico = JsonConvert.DeserializeObject<List<AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware>>(Request.Params["TipoServico"]);

                unitOfWork.Start();
                AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWork);

                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"] != null)
                    grupos = Session["GrupoRedmine"] as List<String>;

                AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario = new AdminMultisoftware.Dominio.Entidades.Modulos.Formulario();
                formulario.Ativo = bool.Parse(Request.Params["Ativo"]);
                formulario.Descricao = Request.Params["Descricao"];
                formulario.CaminhoPagina = Request.Params["CaminhoPagina"];
                formulario.Modulo = new AdminMultisoftware.Dominio.Entidades.Modulos.Modulo() { Codigo = int.Parse(Request.Params["Modulo"]) };
                formulario.Sequencia = int.Parse(Request.Params["Sequencia"]);
                formulario.EmHomologacao = bool.Parse(Request.Params["EmHomologacao"]);
                formulario.CodigoFormulario = repFormulario.BuscarProximoCodigo();
                formulario.TiposServicosMultisoftware = tiposServico;
                formulario.TranslationResourcePath = Request.Params["TranslationResourcePath"];

                if (grupos != null && grupos.Contains("Torre Embarcador") && tiposServico.Any() && !tiposServico.Contains(TipoServicoMultisoftware.MultiEmbarcador) && !tiposServico.Contains(TipoServicoMultisoftware.MultiCTe))
                    return new JsonpResult(false, true, "Tipo de Serviço selecionado não pertence a Torre Embarcador, utilize um dos seguintes: MultiEmbarcador, MultiCTe ou então Todos.");

                if (grupos != null && grupos.Contains("Torre Transportador") && tiposServico.Any() && !tiposServico.Contains(TipoServicoMultisoftware.MultiTMS))
                    return new JsonpResult(false, true, "Tipo de Serviço selecionado não pertence a Torre Transportador, utilize um dos seguintes: MultiTMS ou então Todos.");

                if (repFormulario.BuscarPorCodigoFormulario(formulario.CodigoFormulario) == null)
                {
                    repFormulario.Inserir(formulario, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe uma formulario cadastrada para esse código");
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
                List<AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware> tiposServico = JsonConvert.DeserializeObject<List<AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware>>(Request.Params["TipoServico"]);

                unitOfWork.Start();

                AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario = repFormulario.BuscarPorCodigo(int.Parse(Request.Params["Codigo"]));

                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"] != null)
                    grupos = Session["GrupoRedmine"] as List<String>;

                formulario.Initialize();
                formulario.Ativo = bool.Parse(Request.Params["Ativo"]);
                formulario.Descricao = Request.Params["Descricao"];
                formulario.CaminhoPagina = Request.Params["CaminhoPagina"];
                formulario.Modulo = new AdminMultisoftware.Dominio.Entidades.Modulos.Modulo() { Codigo = int.Parse(Request.Params["Modulo"]) };
                formulario.Sequencia = int.Parse(Request.Params["Sequencia"]);
                formulario.EmHomologacao = bool.Parse(Request.Params["EmHomologacao"]);
                formulario.TiposServicosMultisoftware = tiposServico;
                formulario.TranslationResourcePath = Request.Params["TranslationResourcePath"];


                if (grupos != null && grupos.Contains("Torre Embarcador") && tiposServico.Any() && !tiposServico.Contains(TipoServicoMultisoftware.MultiEmbarcador) && !tiposServico.Contains(TipoServicoMultisoftware.MultiCTe))
                    return new JsonpResult(false, true, "Tipo de Serviço selecionado não pertence a Torre Embarcador, utilize um dos seguintes: MultiEmbarcador, MultiCTe ou então Todos.");

                if (grupos != null && grupos.Contains("Torre Transportador") && tiposServico.Any() && !tiposServico.Contains(TipoServicoMultisoftware.MultiTMS))
                    return new JsonpResult(false, true, "Tipo de Serviço selecionado não pertence a Torre Transportador, utilize um dos seguintes: MultiTMS ou então Todos.");

                repFormulario.Atualizar(formulario, Auditado);

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

                AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario = repFormulario.BuscarPorCodigo(codigo);
                var dynFormulario = new
                {
                    formulario.Codigo,
                    formulario.CodigoFormulario,
                    formulario.Descricao,
                    formulario.CaminhoPagina,
                    Modulo = new { formulario.Modulo.Codigo, formulario.Modulo.Descricao },
                    formulario.Ativo,
                    formulario.EmHomologacao,
                    formulario.Sequencia,
                    TipoServico = formulario.TiposServicosMultisoftware.ToList(),
                    formulario.TranslationResourcePath
                };
                return new JsonpResult(dynFormulario);
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
                AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario = repFormulario.BuscarPorCodigo(codigo);
                repFormulario.Deletar(formulario, Auditado);
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