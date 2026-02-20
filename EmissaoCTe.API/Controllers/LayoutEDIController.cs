using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class LayoutEDIController : ApiController
    {
        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                List<Dominio.Entidades.LayoutEDI> layouts = repLayoutEDI.Buscar("A");

                var retorno = (from obj in layouts select new { obj.Codigo, obj.Descricao, obj.DescricaoTipo }).ToList();

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
        public ActionResult BuscarTodosPorTipo()
        {
            try
            {
                Dominio.Enumeradores.TipoLayoutEDI tipoLayout;
                Enum.TryParse<Dominio.Enumeradores.TipoLayoutEDI>(Request.Params["TipoLayout"], out tipoLayout);

                var retorno = (from obj in this.EmpresaUsuario.LayoutsEDI where obj.Tipo == tipoLayout && obj.Status == "A" select new { obj.Codigo, obj.Descricao }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os layouts de EDI.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consulta()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["InicioRegistros"], out inicioRegistros);

                Dominio.Enumeradores.TipoLayoutEDI tipoLayout;
                Enum.TryParse<Dominio.Enumeradores.TipoLayoutEDI>(Request.Params["TipoLayout"], out tipoLayout);

                string descricao = Request.Params["Descricao"];

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);
                List<Dominio.Entidades.LayoutEDI> listaLayouts = repLayoutEDI.ConsultaPorEmpresaETipo(this.EmpresaUsuario.Codigo, tipoLayout, descricao, inicioRegistros, 50);
                int countLayouts = repLayoutEDI.ContarConsultaPorEmpresaETipo(this.EmpresaUsuario.Codigo, tipoLayout, descricao);

                var retorno = (from obj in listaLayouts
                               select new   
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   Tipo = obj.Tipo.ToString("G"),
                                   Status = obj.Status.Equals("A") ? "Ativo" : "Inativo"
                               }).ToList();

                return Json(retorno, true, null, new string[] {"Código", "Descricao|40", "Tipo|20", "Status|20" }, countLayouts);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os layouts de EDI.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarTipoImportacaoEDI()
        {
            try
            {
                var importacaoEDI = EmpresaUsuario.Configuracao != null ? EmpresaUsuario.Configuracao.UtilizaNovaImportacaoEDI : false;

                return Json(importacaoEDI, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os layouts de EDI.");
            }
        }


        #endregion
    }
}
