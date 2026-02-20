using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class TipoDeCustoFixoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("tiposdecustosfixos.aspx") select obj).FirstOrDefault();
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

                Repositorio.TipoDeCustoFixo repTipoCustoFixo = new Repositorio.TipoDeCustoFixo(unitOfWork);
                var listaTipoCustoFixo = repTipoCustoFixo.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countTipoCustoFixo = repTipoCustoFixo.ContarConsulta(this.EmpresaUsuario.Codigo, status, descricao);

                var retorno = from obj in listaTipoCustoFixo select new { obj.Codigo, obj.Status, obj.Descricao, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Descrição|60", "Status|20" }, countTipoCustoFixo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os tipos de custos fixos.");
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

                Repositorio.TipoDeCustoFixo repTipoCustoFixo = new Repositorio.TipoDeCustoFixo(unitOfWork);
                Dominio.Entidades.TipoDeCustoFixo tipoCustoFixo;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    tipoCustoFixo = repTipoCustoFixo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo); 
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    tipoCustoFixo = new Dominio.Entidades.TipoDeCustoFixo();
                    tipoCustoFixo.Status = "A";
                }

                tipoCustoFixo.Descricao = descricao;
                tipoCustoFixo.Empresa = this.EmpresaUsuario;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    tipoCustoFixo.Status = status;

                if (codigo > 0)
                    repTipoCustoFixo.Atualizar(tipoCustoFixo);
                else
                    repTipoCustoFixo.Inserir(tipoCustoFixo);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o tipo de custo fixo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
