using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/MensagemAviso")]
    public class MensagemAvisoController : BaseController
    {
        public ActionResult Pesquisa()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string titulo = Request.Params["Titulo"];

                AdminMultisoftware.Dominio.Enumeradores.SituacaoAtivoPesquisa situacao = !string.IsNullOrEmpty(Request.Params["Ativo"]) ? (AdminMultisoftware.Dominio.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params["Ativo"]) : AdminMultisoftware.Dominio.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data Inicial", "DataInicio", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Final", "DataFim", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Título", "Titulo", 60, Models.Grid.Align.left, true);

                if (situacao == AdminMultisoftware.Dominio.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso repMensagemAviso = new AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso(unitOfWork);

                List<AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAviso> listaMensagemAviso = repMensagemAviso.Consultar(dataInicial, dataFinal, titulo, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repMensagemAviso.ContarConsulta(dataInicial, dataFinal, titulo));

                grid.AdicionaRows((from obj in listaMensagemAviso
                                   select new
                                   {
                                       obj.Codigo,
                                       DataInicio = obj.DataInicio.ToString("dd/MM/yyyy"),
                                       DataFim = obj.DataFim.ToString("dd/MM/yyyy"),
                                       obj.Titulo,
                                       obj.DescricaoAtivo,

                                   }).ToList());

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
                bool ativo;
                bool.TryParse(Request.Params["Ativo"], out ativo);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;
                Enum.TryParse(Request.Params["TipoServicoMultisoftware"], out tipoServicoMultisoftware);

                string mensagem = Request.Params["Mensagem"];
                string titulo = Request.Params["Titulo"];
                string observacao = Request.Params["Observacao"];

                unitOfWork.Start();
                AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso repMensagemAviso = new AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAviso mensagemAviso = new AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAviso
                {
                    Ativo = ativo,
                    DataFim = dataFinal,
                    DataInicio = dataInicial,
                    Descricao = mensagem,
                    Status = ativo ? "A" : "I",
                    Titulo = titulo,
                    TipoServicoMultisoftware = tipoServicoMultisoftware,
                    Observacao = observacao
                };

                repMensagemAviso.Inserir(mensagemAviso, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    mensagemAviso.Codigo
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

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
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                bool ativo;
                bool.TryParse(Request.Params["Ativo"], out ativo);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;
                Enum.TryParse(Request.Params["TipoServicoMultisoftware"], out tipoServicoMultisoftware);

                string mensagem = Request.Params["Mensagem"];
                string titulo = Request.Params["Titulo"];
                string observacao = Request.Params["Observacao"];

                AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso repMensagemAviso = new AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso(unitOfWork);

                unitOfWork.Start();

                AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAviso mensagemAviso = repMensagemAviso.BuscarPorCodigo(codigo);

                mensagemAviso.Ativo = ativo;
                mensagemAviso.DataFim = dataFinal;
                mensagemAviso.DataInicio = dataInicial;
                mensagemAviso.Descricao = mensagem;
                mensagemAviso.Status = ativo ? "A" : "I";
                mensagemAviso.Titulo = titulo;
                mensagemAviso.TipoServicoMultisoftware = tipoServicoMultisoftware;
                mensagemAviso.Observacao = observacao;


                repMensagemAviso.Atualizar(mensagemAviso, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

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
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso repMensagemAviso = new AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAviso mensagemAviso = repMensagemAviso.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    mensagemAviso.Ativo,
                    mensagemAviso.Codigo,
                    Mensagem = mensagemAviso.Descricao,
                    DataFinal = mensagemAviso.DataFim.ToString("dd/MM/yyyy"),
                    DataInicial = mensagemAviso.DataInicio.ToString("dd/MM/yyyy"),
                    mensagemAviso.Titulo,
                    mensagemAviso.Observacao,
                    mensagemAviso.TipoServicoMultisoftware,
                    Anexos = (from anexo in mensagemAviso.Anexos
                              select new
                              {
                                  anexo.Codigo,
                                  anexo.Descricao,
                                  anexo.NomeArquivo,
                              }).ToList(),
                };

                return new JsonpResult(retorno);
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
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso repMensagemAviso = new AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAviso mensagemAviso = repMensagemAviso.BuscarPorCodigo(codigo);

                repMensagemAviso.Deletar(mensagemAviso);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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
    }
}