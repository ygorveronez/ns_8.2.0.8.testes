using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ModeloDePneuController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("modelosdepneus.aspx") select obj).FirstOrDefault();
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
                string status = Request.Params["Status"];

                Repositorio.ModeloPneu repModeloPneu = new Repositorio.ModeloPneu(unitOfWork);
                var listaModeloPneu = repModeloPneu.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countModeloPneu = repModeloPneu.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, status);

                var retorno = from obj in listaModeloPneu select new { obj.Codigo, obj.Status, obj.Descricao, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Descrição|60", "Status|20" }, countModeloPneu);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os modelos.");
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
                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");
                Repositorio.ModeloPneu repModeloPneu = new Repositorio.ModeloPneu(unitOfWork);
                Dominio.Entidades.ModeloPneu modeloPneu;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    modeloPneu = new Dominio.Entidades.ModeloPneu();
                    modeloPneu.Status = "A";
                    modeloPneu.Data = DateTime.Now;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    modeloPneu = repModeloPneu.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }
                modeloPneu.Descricao = descricao;
                modeloPneu.Empresa = this.EmpresaUsuario;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    modeloPneu.Status = status;

                if (codigo > 0)
                    repModeloPneu.Atualizar(modeloPneu);
                else
                    repModeloPneu.Inserir(modeloPneu);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o modelo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
