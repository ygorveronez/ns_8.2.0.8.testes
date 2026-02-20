using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class TipoDeDespesaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("tiposdecargas.aspx") select obj).FirstOrDefault();
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

                Repositorio.TipoDespesa repNaturezaDaOperacao = new Repositorio.TipoDespesa(unitOfWork);
                var listaTipoDespesa = repNaturezaDaOperacao.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countTipoDespesa = repNaturezaDaOperacao.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, status);

                var retorno = from obj in listaTipoDespesa select new { obj.Codigo, CodigoContaContabil = (obj.PlanoDeConta != null ? obj.PlanoDeConta.Codigo : 0), obj.Status, obj.Descricao, ContaContabil = (obj.PlanoDeConta != null ? (obj.PlanoDeConta.Conta + " - " + obj.PlanoDeConta.Descricao) : string.Empty), obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "CodigoContaContabil", "Status", "Descrição|40", "Conta Contábil|25", "Status|15" }, countTipoDespesa);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os tipos de despesas.");
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
                int codigo, codigoPlanoConta;

                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoContaContabil"], out codigoPlanoConta);


                string status = Request.Params["Status"];
                string descricao = Request.Params["Descricao"];

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");

                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                Repositorio.TipoDespesa repTipoDespesa = new Repositorio.TipoDespesa(unitOfWork);

                Dominio.Entidades.TipoDespesa tipoDespesa;

                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    tipoDespesa = new Dominio.Entidades.TipoDespesa();
                    tipoDespesa.Status = "A";
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    tipoDespesa = repTipoDespesa.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }

                Repositorio.PlanoDeConta repPlanoConta = new Repositorio.PlanoDeConta(unitOfWork);

                tipoDespesa.PlanoDeConta = repPlanoConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPlanoConta);
                tipoDespesa.Descricao = descricao;
                tipoDespesa.Empresa = this.EmpresaUsuario;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    tipoDespesa.Status = status;

                if (codigo > 0)
                    repTipoDespesa.Atualizar(tipoDespesa);
                else
                    repTipoDespesa.Inserir(tipoDespesa);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o tipo de despesa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
