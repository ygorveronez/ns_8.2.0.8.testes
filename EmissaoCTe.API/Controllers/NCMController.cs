using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class NCMController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string numero = Request.Params["Numero"];
                string descricao = Request.Params["Descricao"];

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.NCM repNCM = new Repositorio.NCM(unitOfWork);

                List<Dominio.Entidades.NCM> ncms = repNCM.Consultar(numero, descricao, inicioRegistros, 50);

                int countNCMs = repNCM.ContarConsulta(numero, descricao);

                var retorno = from obj in ncms
                              select new
                              {
                                   obj.Codigo,
                                   obj.Numero,
                                   obj.Descricao
                              };

                return Json(retorno, true, null, new string[] { "Código", "Código|15", "Descrição|65" }, countNCMs);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as NCM's. Atualize a página e tente novamente.");
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

                string descricao = Request.Params["Descricao"];
                string numero = Request.Params["Numero"];

                // Validacoes
                if (string.IsNullOrWhiteSpace(numero))
                    return Json<bool>(false, false, "Número do NCM é inválido.");

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descricão é obrigatório.");

                Repositorio.NCM repNCM = new Repositorio.NCM(unitOfWork);

                if (repNCM.BuscarPorNumero(numero) != null)
                    return Json<bool>(false, false, "O número do NCM já está cadastrado.");

                Dominio.Entidades.NCM ncm = repNCM.BuscarPorCodigo(codigo);

                if (ncm == null)
                    ncm = new Dominio.Entidades.NCM();

                ncm.Descricao = descricao;
                ncm.Numero = numero;

                if (ncm.Codigo > 0)
                    repNCM.Atualizar(ncm);
                else
                    repNCM.Inserir(ncm);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o NCM.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.NCM repNCM = new Repositorio.NCM(unitOfWork);
                Dominio.Entidades.NCM ncm = repNCM.BuscarPorCodigo(codigo);

                // Validacoes
                if (ncm == null)
                    return Json<bool>(false, false, "O NCM não foi encontrado.");
                
                repNCM.Deletar(ncm);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao excluir o NCM.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
