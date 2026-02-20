using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EstadoController : ApiController
    {

        #region Propriedades

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("contingenciaestado.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                List<Dominio.Entidades.Estado> listaEstados = repEstado.BuscarTodos();
                var retorno = from obj in listaEstados select new { obj.Sigla, obj.Nome };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados dos Estados!");
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

                string nome = Request.Params["Nome"];

                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                List<Dominio.Entidades.Estado> listaEstados = repEstado.Consultar(nome, 0, "Sigla", "asc", inicioRegistros, 50, string.Empty);
                int countEstados = repEstado.ContarConsulta(nome, 0, string.Empty);

                var result = from obj in listaEstados select new { obj.Sigla, obj.Nome, obj.DescricaoTipoEmissao };

                return Json(result, true, null, new string[] { "Sigla", "Nome|60", "Contingencia|30" }, countEstados);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar estados.");
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
                string sigla = Request.Params["Sigla"];

                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(sigla);

                if (estado == null)
                    return Json<bool>(false, false, "Estado não encontrado.");

                var retorno = new
                {
                    estado.Sigla,
                    estado.Nome,
                    estado.TipoEmissao,
                    HabilitarContigenciaEpecAutomaticamente = estado.HabilitarContingenciaEPECAutomaticamente,
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do estado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarContingencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string sigla = Request.Params["Sigla"];
                string tipoEmissao = Request.Params["TipoEmissao"];
                bool habilitarContigenciaEpecAutomaticamente = Request.Params["HabilitarContigenciaEpecAutomaticamente"].ToBool();

                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                Dominio.Entidades.Estado estado = null;

                if (!string.IsNullOrWhiteSpace(sigla))
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    estado = repEstado.BuscarPorSigla(sigla);
                }
                else
                {
                    return Json<bool>(false, false, "Não é possível criar um estado, favor editar.");
                }

                estado.TipoEmissao = tipoEmissao;
                estado.HabilitarContingenciaEPECAutomaticamente = habilitarContigenciaEpecAutomaticamente;
                repEstado.Atualizar(estado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, estado, null, "Alterou contingência para " + tipoEmissao.ToString(), unitOfWork);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o estado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
