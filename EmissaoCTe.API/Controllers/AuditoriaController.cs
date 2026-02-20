using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class AuditoriaController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
#if DEBUG
#else
                if (this.UsuarioAdministrativo == null)
                    return Json<bool>(false, false, "Sem permissão para auditar.");
#endif
                Repositorio.Auditoria.HistoricoObjeto repHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);

                string entidade = Request.Params["Entidade"];
                long.TryParse(Request.Params["Codigo"], out long codigo);

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int codigoEmpresa = 0;

                List<Dominio.Entidades.Auditoria.HistoricoObjeto> historico = repHistoricoObjeto.Consultar(codigoEmpresa, codigo, entidade, "Data", "desc", inicioRegistros, 50);
                int count = repHistoricoObjeto.ContarConsulta(codigoEmpresa, codigo, entidade);

                var retorno = (from p in historico
                             select new
                             {
                                 p.Codigo,
                                 Data = p.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                 Auditado = p.Auditado,
                                 p.Descricao,
                                 Acao = p.DescricaoAcao
                             }).ToList();
                
                return Json(retorno, true, null, new string[] { "Codigo", "Data|12", "Auditado|20", "Descrição|25", "Ação|35" }, count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult PesquisaComposAlterados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
#if DEBUG
#else
                if (this.UsuarioAdministrativo == null)
                    return Json<bool>(false, false, "Sem permissão para auditar.");
#endif
                Repositorio.Auditoria.HistoricoObjeto repHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);

                long.TryParse(Request.Params["Codigo"], out long codigo);
                dynamic dicionario = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params["Dicionario"]);

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repHistoricoObjeto.BuscarPorCodigo(codigo);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> propriedade = historico.Propriedades.ToList();
                propriedade = (from o in propriedade
                               where 
                                (!string.IsNullOrWhiteSpace(o.De) && !string.IsNullOrWhiteSpace(o.Para)) ||
                                (!string.IsNullOrWhiteSpace(o.De) || !string.IsNullOrWhiteSpace(o.Para))
                               select o).ToList();

                int count = propriedade.Count;
                var retorno = (from p in propriedade.OrderBy(obj => obj.Propriedade).ToList()
                               select new
                               {
                                   p.Codigo,
                                   Propriedade = ReplaceDicionario(p.Propriedade, dicionario),
                                   De = p.De,
                                   Para = p.Para
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Propriedade|20", "De|30", "Para|35" }, count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ReplaceDicionario(string prop, dynamic dicionario)
        {
            string propCorreta = "";

            try
            {
                propCorreta = (string)dicionario[prop];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao acessar propriedade do dicionário de auditoria - API: {ex.ToString()}", "CatchNoAction");
            }
            if (string.IsNullOrWhiteSpace(propCorreta))
                propCorreta = prop;

            return propCorreta;
        }
    }
}

