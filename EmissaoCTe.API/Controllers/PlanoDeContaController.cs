using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PlanoDeContaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("planosdecontas.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string conta = Request.Params["Conta"];
                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];
                string tipo = Request.Params["Tipo"];
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.PlanoDeConta repPlanoDeConta = new Repositorio.PlanoDeConta(unitOfWork);

                List<Dominio.Entidades.PlanoDeConta> listaPlanoDeConta = repPlanoDeConta.Consultar(this.EmpresaUsuario.Codigo, status, tipo, descricao, conta, inicioRegistros, 50);
                int countPlanoDeConta = repPlanoDeConta.ContarConsulta(this.EmpresaUsuario.Codigo, status, tipo, descricao, conta);

                var retorno = from obj in listaPlanoDeConta select new { obj.Codigo, obj.Tipo, obj.TipoDeConta, obj.Status, obj.Conta, obj.Descricao, obj.DescricaoTipo, obj.DescricaoTipoDeConta, obj.ContaContabil, obj.NaoExibirDRE };

                return Json(retorno, true, null, new string[] { "Codigo", "Tipo", "TipoDeConta", "Status", "Conta|20", "Descrição|32", "Tipo|10", "Tipo Conta|10", "Conta Contábil|13", "Exibir DRE|5" }, countPlanoDeConta);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os planos de contas.");
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
                string conta = Request.Params["Conta"];
                string contaContabil = Request.Params["ContaContabil"];
                string tipoDeConta = Request.Params["TipoDeConta"];
                bool.TryParse(Request.Params["NaoExibirDRE"], out bool naoExibirDRE);

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");
                if (string.IsNullOrWhiteSpace(tipo))
                    return Json<bool>(false, false, "Tipo inválido.");
                if (string.IsNullOrWhiteSpace(tipoDeConta))
                    return Json<bool>(false, false, "Tipo de conta inválido.");

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^([0-9]+\.){1,15}$");
                if (!regex.IsMatch(string.Concat(conta, ".")))
                    return Json<bool>(false, false, "Conta com formato inválido. Formato aceito: 1.1.1.1.");

                Repositorio.PlanoDeConta repPlanoConta = new Repositorio.PlanoDeConta(unitOfWork);

                if (conta.Contains("."))
                {
                    string contaPai = conta.Substring(0, conta.LastIndexOf("."));
                    if (!string.IsNullOrWhiteSpace(contaPai))
                    {
                        Dominio.Entidades.PlanoDeConta planoPai = repPlanoConta.BuscarPorConta(this.EmpresaUsuario.Codigo, contaPai);
                        if (planoPai == null)
                            return Json<bool>(false, false, "Conta pai inexistente.");
                        else if (planoPai.Tipo != "S")
                            return Json<bool>(false, false, "A conta pai deve ser sintética.");
                        else if (planoPai.TipoDeConta != tipoDeConta)
                            return Json<bool>(false, false, string.Concat("O tipo de conta deve ser o mesmo do plano pai (", planoPai.DescricaoTipoDeConta, ")."));
                        else
                            planoPai = null;
                    }
                }

                Dominio.Entidades.PlanoDeConta plano = repPlanoConta.BuscarPorConta(this.EmpresaUsuario.Codigo, conta);
                if (plano != null)
                {
                    if (plano.Codigo != codigo)
                        return Json<bool>(false, false, "Conta já existente.");
                    else
                        plano = null;
                }

                Dominio.Entidades.PlanoDeConta planoConta = null;

                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    planoConta = new Dominio.Entidades.PlanoDeConta();
                    planoConta.Status = "A";
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    planoConta = repPlanoConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }

                planoConta.Descricao = descricao;
                planoConta.Empresa = this.EmpresaUsuario;
                planoConta.Conta = conta;
                planoConta.ContaContabil = contaContabil;
                planoConta.Empresa = this.EmpresaUsuario;
                planoConta.Tipo = tipo;
                planoConta.TipoDeConta = tipoDeConta;
                planoConta.NaoExibirDRE = naoExibirDRE;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    planoConta.Status = status;

                if (codigo > 0)
                    repPlanoConta.Atualizar(planoConta);
                else
                    repPlanoConta.Inserir(planoConta);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o plano de conta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

    }
}
