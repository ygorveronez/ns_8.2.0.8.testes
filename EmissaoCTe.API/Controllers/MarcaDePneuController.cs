using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class MarcaDePneuController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("marcasdepneus.aspx") select obj).FirstOrDefault();
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

                Repositorio.MarcaPneu repMarcaPneu = new Repositorio.MarcaPneu(unitOfWork);
                var listaMarcaPneu = repMarcaPneu.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countMarcaPneu = repMarcaPneu.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, status);

                var retorno = from obj in listaMarcaPneu select new { obj.Codigo, obj.Status, obj.Descricao, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Descrição|60", "Status|20" }, countMarcaPneu);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as marcas.");
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
                Repositorio.MarcaPneu repMarcaPneu = new Repositorio.MarcaPneu(unitOfWork);
                Dominio.Entidades.MarcaPneu marcaPneu;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    marcaPneu = new Dominio.Entidades.MarcaPneu();
                    marcaPneu.Status = "A";
                    marcaPneu.Data = DateTime.Now;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    marcaPneu = repMarcaPneu.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }
                marcaPneu.Descricao = descricao;
                marcaPneu.Empresa = this.EmpresaUsuario;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    marcaPneu.Status = status;

                if (codigo > 0)
                    repMarcaPneu.Atualizar(marcaPneu);
                else
                    repMarcaPneu.Inserir(marcaPneu);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a marca.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
