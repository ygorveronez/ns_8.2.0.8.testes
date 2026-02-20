using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class DimensaoDePneuController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("dimensoesdepneus.aspx") select obj).FirstOrDefault();
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

                Repositorio.DimensaoPneu repDimensaoPneu = new Repositorio.DimensaoPneu(unitOfWork);
                var listaDimensaoPneu = repDimensaoPneu.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countDimensaoPneu = repDimensaoPneu.ContarConsulta(this.EmpresaUsuario.Codigo, status, descricao);

                var retorno = from obj in listaDimensaoPneu select new { obj.Codigo, obj.Status, obj.Descricao, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Descrição|60", "Status|20" }, countDimensaoPneu);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as dimensões.");
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
                Repositorio.DimensaoPneu repDimensaoPneu = new Repositorio.DimensaoPneu(unitOfWork);
                Dominio.Entidades.DimensaoPneu dimensaoPneu;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    dimensaoPneu = new Dominio.Entidades.DimensaoPneu();
                    dimensaoPneu.Status = "A";
                    dimensaoPneu.Data = DateTime.Now;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    dimensaoPneu = repDimensaoPneu.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }
                dimensaoPneu.Descricao = descricao;
                dimensaoPneu.Empresa = this.EmpresaUsuario;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    dimensaoPneu.Status = status;

                if (codigo > 0)
                    repDimensaoPneu.Atualizar(dimensaoPneu);
                else
                    repDimensaoPneu.Inserir(dimensaoPneu);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a dimensão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
