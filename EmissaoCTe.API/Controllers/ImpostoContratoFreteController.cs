using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ImpostoContratoFreteController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("impostosparacontratodefrete.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        public ActionResult ObterImpostosDaEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unitOfWork);

                Dominio.Entidades.ImpostoContratoFrete imposto = repImpostoContratoFrete.BuscarPorEmpresaVigencia(this.EmpresaUsuario.Codigo, DateTime.Now);

                if (imposto == null)
                    return Json<bool>(true, true);

                Repositorio.INSSImpostoContratoFrete repINSS = new Repositorio.INSSImpostoContratoFrete(unitOfWork);
                Repositorio.IRImpostoContratoFrete repIR = new Repositorio.IRImpostoContratoFrete(unitOfWork);

                List<Dominio.Entidades.INSSImpostoContratoFrete> lINSS = repINSS.BuscarPorImposto(imposto.Codigo);
                List<Dominio.Entidades.IRImpostoContratoFrete> lIR = repIR.BuscarPorImposto(imposto.Codigo);

                var retorno = new
                {                    
                    imposto.AliquotaSENAT,
                    imposto.AliquotaSEST,
                    imposto.AliquotaINCRA,
                    imposto.AliquotaSalarioEducacao,
                    imposto.ValorPorDependenteDescontoIRRF,
                    imposto.PercentualBCINSS,
                    imposto.PercentualBCIR,
                    imposto.ValorTetoRetencaoINSS,
                    INSS = from obj in lINSS
                           select new
                           {
                               obj.Codigo,
                               obj.PercentualAplicar,
                               obj.PercentualAplicarContratante,
                               obj.ValorFinal,
                               obj.ValorInicial,
                               Excluir = false
                           },
                    IR = from obj in lIR
                         select new
                         {
                             obj.Codigo,
                             obj.PercentualAplicar,
                             obj.ValorDeduzir,
                             obj.ValorFinal,
                             obj.ValorInicial,
                             Excluir = false
                         }
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os impostos da empresa. Atualize a página e tente novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.ImpostoContratoFrete repImposto = new Repositorio.ImpostoContratoFrete(unidadeDeTrabalho);

                Dominio.Entidades.ImpostoContratoFrete imposto = repImposto.BuscarPorEmpresa(this.EmpresaUsuario.Codigo);

                if (imposto == null)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de imposto negada.");

                    imposto = new Dominio.Entidades.ImpostoContratoFrete();
                    imposto.Empresa = this.EmpresaUsuario;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de imposto negada.");
                }

                unidadeDeTrabalho.Start();

                decimal aliquotaSEST, aliquotaSENAT, baseCalculoINSS, baseCalculoIR, retencaoINSS, aliquotaINCRA, aliquotaSalarioEducacao, valorPorDependenteDescontoIRRF;
                decimal.TryParse(Request.Params["AliquotaSEST"], out aliquotaSEST);
                decimal.TryParse(Request.Params["AliquotaSENAT"], out aliquotaSENAT);
                decimal.TryParse(Request.Params["AliquotaINCRA"], out aliquotaINCRA);
                decimal.TryParse(Request.Params["AliquotaSalarioEducacao"], out aliquotaSalarioEducacao);
                decimal.TryParse(Request.Params["BaseCalculoINSS"], out baseCalculoINSS);
                decimal.TryParse(Request.Params["BaseCalculoIR"], out baseCalculoIR);
                decimal.TryParse(Request.Params["RetencaoINSS"], out retencaoINSS);
                decimal.TryParse(Request.Params["ValorPorDependenteDescontoIRRF"], out valorPorDependenteDescontoIRRF);                

                imposto.AliquotaSENAT = aliquotaSENAT;
                imposto.AliquotaSEST = aliquotaSEST;
                imposto.AliquotaINCRA = aliquotaINCRA;
                imposto.AliquotaSalarioEducacao = aliquotaSalarioEducacao;
                imposto.ValorPorDependenteDescontoIRRF = valorPorDependenteDescontoIRRF;
                imposto.PercentualBCINSS = baseCalculoINSS;
                imposto.PercentualBCIR = baseCalculoIR;
                imposto.ValorTetoRetencaoINSS = retencaoINSS;

                if (imposto.Codigo > 0)
                    repImposto.Atualizar(imposto);
                else
                    repImposto.Inserir(imposto);

                this.SalvarINSS(imposto, unidadeDeTrabalho);
                this.SalvarIR(imposto, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os impostos.");
            }
        }

        private void SalvarIR(Dominio.Entidades.ImpostoContratoFrete imposto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.IRImpostoContratoFrete repIR = new Repositorio.IRImpostoContratoFrete(unidadeDeTrabalho);
            
            dynamic impostos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params["IR"]);

            foreach (var impostoEditar in impostos)
            {
                Dominio.Entidades.IRImpostoContratoFrete ir = (int)impostoEditar.Codigo > 0 ? repIR.BuscarPorCodigo((int)impostoEditar.Codigo) : null;

                if (!(bool)impostoEditar.Excluir)
                {
                    if (ir == null)
                    {
                        ir = new Dominio.Entidades.IRImpostoContratoFrete();
                        ir.Imposto = imposto;
                    }

                    ir.PercentualAplicar = (decimal)impostoEditar.PercentualAplicar;
                    ir.ValorDeduzir = (decimal)impostoEditar.ValorDeduzir;
                    ir.ValorFinal = (decimal)impostoEditar.ValorFinal;
                    ir.ValorInicial = (decimal)impostoEditar.ValorInicial;
                    
                    if (ir.Codigo > 0)
                        repIR.Atualizar(ir);
                    else
                        repIR.Inserir(ir);
                }
                else if (ir != null && ir.Codigo > 0)
                {
                    repIR.Deletar(ir);
                }
            }
        }

        private void SalvarINSS(Dominio.Entidades.ImpostoContratoFrete imposto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.INSSImpostoContratoFrete repINSS = new Repositorio.INSSImpostoContratoFrete(unidadeDeTrabalho);

            dynamic impostos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params["INSS"]);

            foreach (var impostoEditar in impostos)
            {
                Dominio.Entidades.INSSImpostoContratoFrete inss = (int)impostoEditar.Codigo > 0 ? repINSS.BuscarPorCodigo((int)impostoEditar.Codigo) : null;

                if (!(bool)impostoEditar.Excluir)
                {
                    if (inss == null)
                    {
                        inss = new Dominio.Entidades.INSSImpostoContratoFrete();
                        inss.Imposto = imposto;
                    }

                    inss.PercentualAplicar = (decimal)impostoEditar.PercentualAplicar;
                    inss.PercentualAplicarContratante = (decimal)impostoEditar.PercentualAplicarContratante;
                    inss.ValorFinal = (decimal)impostoEditar.ValorFinal;
                    inss.ValorInicial = (decimal)impostoEditar.ValorInicial;                    

                    if (inss.Codigo > 0)
                        repINSS.Atualizar(inss);
                    else
                        repINSS.Inserir(inss);
                }
                else if (inss != null && inss.Codigo > 0)
                {
                    repINSS.Deletar(inss);
                }
            }
        }

        #endregion
    }
}
