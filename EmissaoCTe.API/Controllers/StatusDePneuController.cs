using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class StatusDePneuController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("statusdepneus.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string descricao = Request.Params["Descricao"];
                string tipo = Request.Params["Tipo"];
                string status = Request.Params["Status"];

                Repositorio.StatusPneu repStatusPneu = new Repositorio.StatusPneu(unitOfWork);
                var listaStatusPneu = repStatusPneu.Consultar(this.EmpresaUsuario.Codigo, descricao, tipo, status, inicioRegistros, 50);
                int countStatusPneu = repStatusPneu.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, tipo, status);

                var retorno = from obj in listaStatusPneu select new { obj.Codigo, obj.Status, obj.Tipo, obj.Descricao, obj.DescricaoTipo, obj.DescricaoStatus, };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Tipo", "Descrição|50", "Tipo|20", "Status|20" }, countStatusPneu);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os status.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterUltimoRegistroPorTipo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string tipo = Request.Params["Tipo"];
                Repositorio.StatusPneu repStatusPneu = new Repositorio.StatusPneu(unitOfWork);
                Dominio.Entidades.StatusPneu statusPneu;

                var lista = repStatusPneu.Consultar(this.EmpresaUsuario.Codigo, string.Empty, tipo, "A", 0, 1);
                statusPneu = lista.FirstOrDefault();

                var retorno = statusPneu != null ? new { statusPneu.Codigo, statusPneu.Status, statusPneu.Tipo, statusPneu.Descricao, statusPneu.DescricaoTipo, statusPneu.DescricaoStatus } : null;

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar o status.");
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
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                string status = Request.Params["Status"];
                string descricao = Request.Params["Descricao"];
                string tipo = Request.Params["Tipo"];
                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");
                if (string.IsNullOrWhiteSpace(tipo))
                    return Json<bool>(false, false, "Tipo inválido.");
                Repositorio.StatusPneu repStatusPneu = new Repositorio.StatusPneu(unitOfWork);
                Dominio.Entidades.StatusPneu statusPneu;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    statusPneu = new Dominio.Entidades.StatusPneu();
                    statusPneu.Status = "A";
                    statusPneu.Data = DateTime.Now;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    statusPneu = repStatusPneu.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                    Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);
                    if (!tipo.Equals(statusPneu.Tipo))
                        if (repPneu.ContarPorStatus(statusPneu.Codigo, this.EmpresaUsuario.Codigo) > 0)
                            return Json<bool>(false, false, "Não é possível alterar o tipo do status pois ele está vinculado a um ou mais pneus.");
                }
                statusPneu.Descricao = descricao;
                statusPneu.Empresa = this.EmpresaUsuario;
                statusPneu.Tipo = tipo;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    statusPneu.Status = status;

                if (codigo > 0)
                    repStatusPneu.Atualizar(statusPneu);
                else
                    repStatusPneu.Inserir(statusPneu);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o status.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
