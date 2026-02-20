using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class AliquotaDeICMSController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("aliquotasdeicms.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                decimal aliquota = 0m;
                decimal.TryParse(Request.Params["Aliquota"], out aliquota);

                string status = Request.Params["Status"];

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.AliquotaDeICMS repAliquota = new Repositorio.AliquotaDeICMS(unitOfWork);

                List<Dominio.Entidades.AliquotaDeICMS> listaAliquotas = repAliquota.Consultar(this.EmpresaUsuario.Codigo, aliquota, status, inicioRegistros, 50);
                int countAliquotas = repAliquota.ContarConsulta(this.EmpresaUsuario.Codigo, aliquota, status);

                var retorno = from obj in listaAliquotas select new { obj.Codigo, obj.Status, Aliquota = obj.Aliquota.ToString("n2"), obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Codigo", "Status", "Aliquota|60", "Status|30" }, countAliquotas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as alíquotas de ICMS.");
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

                decimal aliquota = 0m;
                decimal.TryParse(Request.Params["Aliquota"], out aliquota);

                if (aliquota < 0m || aliquota > 100m)
                    return Json<bool>(false, false, "Alíquota inválida!");

                string status = Request.Params["Status"];

                Repositorio.AliquotaDeICMS repAliquota = new Repositorio.AliquotaDeICMS(unitOfWork);

                Dominio.Entidades.AliquotaDeICMS aliquotaDeICMS = repAliquota.BuscarPorAliquota(this.EmpresaUsuario.Codigo, aliquota);
                if (aliquotaDeICMS != null && aliquotaDeICMS.Codigo != codigo)
                    return Json<bool>(false, false, "Alíquota de ICMS já cadastrada!");

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    aliquotaDeICMS = repAliquota.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");

                    aliquotaDeICMS = new Dominio.Entidades.AliquotaDeICMS();
                }

                aliquotaDeICMS.Aliquota = aliquota;
                aliquotaDeICMS.Empresa = this.EmpresaUsuario;
                aliquotaDeICMS.Status = status;

                if (codigo > 0)
                    repAliquota.Atualizar(aliquotaDeICMS);
                else
                    repAliquota.Inserir(aliquotaDeICMS);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a aliquota de ICMS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterAliquotasDaEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.AliquotaDeICMS repAliquotaICMS = new Repositorio.AliquotaDeICMS(unitOfWork);
                List<Dominio.Entidades.AliquotaDeICMS> listaAliquotas = repAliquotaICMS.BuscarPorEmpresa(this.EmpresaUsuario.Codigo, "A");

                var result = from obj in listaAliquotas select new { obj.Codigo, obj.Aliquota };

                return Json(result, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as alíquotas do ICMS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
