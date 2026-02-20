using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class CFOPController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("cfop.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Dominio.Enumeradores.TipoCFOP tipo;
                Enum.TryParse(Request.Params["Tipo"], out tipo);

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

                var listaCFOPs = repCFOP.BuscarTodos(tipo);

                var result = from obj in listaCFOPs select new { obj.Codigo, Numero = obj.CodigoCFOP, Descricao = string.IsNullOrWhiteSpace(obj.Descricao) ? string.Empty : obj.Descricao };

                return Json(result, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as CFOPs. Tente novamente!");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorNaturezaDaOperacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int idNaturezaOperacao = 0;
                int.TryParse(Request.Params["IdNaturezaOperacao"], out idNaturezaOperacao);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                var listaCFOPs = repCFOP.BuscarPorNaturezaDaOperacao(idNaturezaOperacao);
                return Json(from obj in listaCFOPs select new { obj.Codigo, obj.CodigoCFOP }, true, null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as CFOPs. Tente novamente!");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorNaturezaDaOperacaoETipo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int idNaturezaOperacao = 0;
                int.TryParse(Request.Params["IdNaturezaOperacao"], out idNaturezaOperacao);

                Dominio.Enumeradores.TipoCFOP tipo;
                Enum.TryParse(Request.Params["Tipo"], out tipo);

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

                var listaCFOPs = repCFOP.BuscarPorNaturezaDaOperacao(idNaturezaOperacao, tipo);

                return Json(from obj in listaCFOPs select new { obj.Codigo, obj.CodigoCFOP }, true, null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as CFOPs. Tente novamente!");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo, naturezaDaOperacao, cfop;
                int.TryParse(Request.Params["Codigo"], out codigo);
                if (!int.TryParse(Request.Params["NaturezaDaOperacao"], out naturezaDaOperacao))
                    return Json<bool>(false, false, "Natureza da Operação inválida.");
                if (!int.TryParse(Request.Params["CFOP"], out cfop))
                    return Json<bool>(false, false, "CFOP inválida.");

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.CFOP objCFOP;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de CFOP negada!");
                    objCFOP = repCFOP.BuscarPorId(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de CFOP negada!");
                    objCFOP = new Dominio.Entidades.CFOP();
                }

                objCFOP.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(naturezaDaOperacao);
                objCFOP.CodigoCFOP = cfop;
                objCFOP.Tipo = Dominio.Enumeradores.TipoCFOP.Saida;
                objCFOP.Status = "A";

                if (codigo > 0)
                    repCFOP.Atualizar(objCFOP);
                else
                    repCFOP.Inserir(objCFOP);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar as CFOPs.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros, cfop = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["CFOP"], out cfop);
                string natureza = Request.Params["NaturezaDaOperacao"];

                Dominio.Enumeradores.TipoCFOP? tipo = null;
                if (Enum.TryParse(Request.Params["Tipo"], out Dominio.Enumeradores.TipoCFOP tipoAux))
                    tipo = tipoAux;

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                var listaCFOPs = repCFOP.Consultar(cfop, natureza, tipo, "CodigoCFOP", "asc", inicioRegistros, 50);
                var countCFOPs = repCFOP.ContarConsulta(cfop, natureza, tipo);
                var retorno = from obj in listaCFOPs
                              select new
                              {
                                  CodigoNaturezaDaOperacao = obj.NaturezaDaOperacao?.Codigo ?? 0,
                                  obj.Codigo,
                                  CFOP = obj.CodigoCFOP,
                                  DescricaoNaturezaDaOperacao = obj.NaturezaDaOperacao?.Descricao ?? string.Empty
                              };
                return Json(retorno, true, null, new String[] { "Codigo da Natureza da Operacao", "Código|10", "CFOP|10", "Natureza da Operação|70" }, countCFOPs);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as CFOPs.");
            }
        }
    }
}
