using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EmpresaLayoutEDIController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult ObterLayoutsPorEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.EmpresaAdministradora == null)
                    return Json<bool>(false, false, "Permissão para acesso negada.");

                int codigoEmpresa = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (empresa == null)
                    return Json<bool>(false, false, "Empresa não encontrada.");

                var retorno = (from obj in empresa.LayoutsEDI where obj.Status == "A" select new { obj.Codigo, obj.Descricao, obj.DescricaoTipo }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os layouts de EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult DeletarLayoutEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.EmpresaAdministradora == null)
                    return Json<bool>(false, false, "Permissão para acesso negada.");

                int codigoEmpresa, codigoLayout = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);
                int.TryParse(Request.Params["CodigoLayout"], out codigoLayout);
                
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (empresa == null)
                    return Json<bool>(false, false, "Empresa não encontrada.");

                empresa.LayoutsEDI.Remove((from obj in empresa.LayoutsEDI where obj.Codigo == codigoLayout select obj).FirstOrDefault());

                repEmpresa.Atualizar(empresa);

                return Json(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao remover o layout de EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult AdicionarLayoutEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.EmpresaAdministradora == null)
                    return Json<bool>(false, false, "Permissão para acesso negada.");

                int codigoEmpresa, codigoLayout = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);
                int.TryParse(Request.Params["CodigoLayout"], out codigoLayout);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (empresa == null)
                    return Json<bool>(false, false, "Empresa não encontrada.");

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.Buscar(codigoLayout);

                if (layoutEDI == null)
                    return Json<bool>(false, false, "Layout EDI não encontrado.");

                if ((from obj in empresa.LayoutsEDI where obj.Codigo == layoutEDI.Codigo select obj).Any())
                    return Json<bool>(false, false, "Este layout já está configurado para esta empresa.");

                empresa.LayoutsEDI.Add(layoutEDI);

                repEmpresa.Atualizar(empresa);

                return Json(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao adicionar o layout de EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}