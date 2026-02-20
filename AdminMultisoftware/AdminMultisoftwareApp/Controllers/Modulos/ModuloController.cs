using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Integracao.Redmine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Modulos
{
    [CustomAuthorize("Modulos/Modulo")]
    public class ModuloController : BaseController
    {
        #region Métodos Globais      
        public ActionResult Pesquisa()
        {

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string descricao = Request.Params["Descricao"];
                bool usuarioEmbarcador = false;
                bool usuarioTMS = false;

                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"]!=null)
                    grupos = Session["GrupoRedmine"] as List<String>;
                
                if (grupos.Contains("Torre Embarcador"))
                    usuarioEmbarcador = true;

                if (grupos.Contains("Torre Transportador"))
                    usuarioTMS = true;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Sequencia", "Sequencia", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Codigo Módulo", "CodigoModulo", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modulo Pai", "ModuloPai", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Serviço", "TiposServicosMultisoftware", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 15, Models.Grid.Align.left, false);


                AdminMultisoftware.Repositorio.Modulos.Modulo repModulo = new AdminMultisoftware.Repositorio.Modulos.Modulo(unitOfWork);
                List<AdminMultisoftware.Dominio.Entidades.Modulos.Modulo> listaModulo = repModulo.Consultar(descricao, usuarioEmbarcador, usuarioTMS, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repModulo.ContarConsulta(descricao, usuarioEmbarcador, usuarioTMS));
                var lista = (from p in listaModulo
                             select new
                             {
                                 p.Codigo,
                                 p.Sequencia,
                                 p.Descricao,
                                 p.Icone,
                                 p.IconeNovo,
                                 p.CodigoModulo,
                                 ModuloPai = p.ModuloPai != null ? p.ModuloPai.Descricao : "",
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
                int codigoModuloPai;
                int.TryParse(Request.Params["ModuloPai"], out codigoModuloPai);
                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"] != null)
                    grupos = Session["GrupoRedmine"] as List<String>;

                List<AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware> tiposServico = JsonConvert.DeserializeObject<List<AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware>>(Request.Params["TipoServico"]);

                unitOfWork.Start();

                AdminMultisoftware.Repositorio.Modulos.Modulo repModulo = new AdminMultisoftware.Repositorio.Modulos.Modulo(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo = new AdminMultisoftware.Dominio.Entidades.Modulos.Modulo();
                modulo.Ativo = bool.Parse(Request.Params["Ativo"]);
                modulo.Descricao = Request.Params["Descricao"];
                modulo.Icone = Request.Params["Icone"];
                modulo.IconeNovo = Request.Params["IconeNovo"];
                modulo.ModuloPai = codigoModuloPai > 0 ? new AdminMultisoftware.Dominio.Entidades.Modulos.Modulo() { Codigo = codigoModuloPai } : null;
                modulo.Sequencia = int.Parse(Request.Params["Sequencia"]);
                modulo.EmHomologacao = bool.Parse(Request.Params["EmHomologacao"]);
                modulo.CodigoModulo = repModulo.BuscarProximoCodigo();
                modulo.TiposServicosMultisoftware = tiposServico;
                modulo.TranslationResourcePath = Request.Params["TranslationResourcePath"];


                if (grupos != null && grupos.Contains("Torre Embarcador") && tiposServico.Any() && !tiposServico.Contains(TipoServicoMultisoftware.MultiEmbarcador) && !tiposServico.Contains(TipoServicoMultisoftware.MultiCTe))
                    return new JsonpResult(false, true, "Tipo de Serviço selecionado não pertence a Torre Embarcador, utilize um dos seguintes: MultiEmbarcador, MultiCTe ou então Todos.");

                if (grupos != null && grupos.Contains("Torre Transportador") && tiposServico.Any() && !tiposServico.Contains(TipoServicoMultisoftware.MultiTMS))
                    return new JsonpResult(false, true, "Tipo de Serviço selecionado não pertence a Torre Transportador, utilize um dos seguintes: MultiTMS ou então Todos.");

                if (repModulo.BuscarPorCodigoModulo(modulo.CodigoModulo) == null)
                {
                    repModulo.Inserir(modulo, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe um modulo cadastrado para esse código");
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
                int codigoModuloPai;
                int.TryParse(Request.Params["ModuloPai"], out codigoModuloPai);

                List<AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware> tiposServico = JsonConvert.DeserializeObject<List<AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware>>(Request.Params["TipoServico"]);

                unitOfWork.Start();

                AdminMultisoftware.Repositorio.Modulos.Modulo repModulo = new AdminMultisoftware.Repositorio.Modulos.Modulo(unitOfWork);

                List<String> grupos = new List<string>();

                if (Session["GrupoRedmine"] != null)
                    grupos = Session["GrupoRedmine"] as List<String>;

                AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo = repModulo.BuscarPorCodigo(int.Parse(Request.Params["Codigo"]));
                modulo.Initialize();
                modulo.Ativo = bool.Parse(Request.Params["Ativo"]);
                modulo.Descricao = Request.Params["Descricao"];
                modulo.ModuloPai = codigoModuloPai > 0 ? new AdminMultisoftware.Dominio.Entidades.Modulos.Modulo() { Codigo = codigoModuloPai } : null;
                modulo.Icone = Request.Params["Icone"];
                modulo.IconeNovo = Request.Params["IconeNovo"];
                modulo.Sequencia = int.Parse(Request.Params["Sequencia"]);
                modulo.EmHomologacao = bool.Parse(Request.Params["EmHomologacao"]);
                modulo.TiposServicosMultisoftware = tiposServico;
                modulo.TranslationResourcePath = Request.Params["TranslationResourcePath"];


                if (grupos != null && grupos.Contains("Torre Embarcador") && tiposServico.Any() && !tiposServico.Contains(TipoServicoMultisoftware.MultiEmbarcador) && !tiposServico.Contains(TipoServicoMultisoftware.MultiCTe))
                    return new JsonpResult(false, true, "Tipo de Serviço selecionado não pertence a Torre Embarcador, utilize um dos seguintes: MultiEmbarcador, MultiCTe ou então Todos.");

                if (grupos != null && grupos.Contains("Torre Transportador") && tiposServico.Any() && !tiposServico.Contains(TipoServicoMultisoftware.MultiTMS))
                    return new JsonpResult(false, true, "Tipo de Serviço selecionado não pertence a Torre Transportador, utilize um dos seguintes: MultiTMS ou então Todos.");

                repModulo.Atualizar(modulo, Auditado);

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

                AdminMultisoftware.Repositorio.Modulos.Modulo repModulo = new AdminMultisoftware.Repositorio.Modulos.Modulo(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo = repModulo.BuscarPorCodigo(codigo);

                var dynModulo = new
                {
                    modulo.Codigo,
                    modulo.CodigoModulo,
                    modulo.Descricao,
                    modulo.Icone,
                    modulo.IconeNovo,
                    modulo.EmHomologacao,
                    ModuloPai = new
                    {
                        Codigo = modulo.ModuloPai?.Codigo ?? 0,
                        Descricao = modulo.ModuloPai?.Descricao ?? string.Empty
                    },
                    modulo.Ativo,
                    modulo.Sequencia,
                    TipoServico = modulo.TiposServicosMultisoftware.ToList(),
                    modulo.TranslationResourcePath
                };

                return new JsonpResult(dynModulo);
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
                AdminMultisoftware.Repositorio.Modulos.Modulo repModulo = new AdminMultisoftware.Repositorio.Modulos.Modulo(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo = repModulo.BuscarPorCodigo(codigo);
                repModulo.Deletar(modulo, Auditado);
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