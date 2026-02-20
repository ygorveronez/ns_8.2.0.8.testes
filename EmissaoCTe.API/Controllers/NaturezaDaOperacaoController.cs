using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class NaturezaDaOperacaoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("naturezasdasoperacoes.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                List<Dominio.Entidades.NaturezaDaOperacao> listaNaturezas = repNaturezaDaOperacao.BuscarTodos();
                var retorno = from obj in listaNaturezas select new { obj.Codigo, obj.Descricao };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as naturezas das operações.");
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
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);
                string descricao = Request.Params["Descricao"];
                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida!");

                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.NaturezaDaOperacao natureza;
                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de natureza da operação negada!");
                    natureza = repNaturezaDaOperacao.BuscarPorId(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de natureza da operação negada!");
                    natureza = new Dominio.Entidades.NaturezaDaOperacao();
                }

                natureza.Descricao = descricao;
                natureza.Status = "A";
                if (codigo > 0)
                    repNaturezaDaOperacao.Atualizar(natureza);
                else
                    repNaturezaDaOperacao.Inserir(natureza);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a natureza da operação.");
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
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string descricao = Request.Params["Descricao"];

                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                var listaNaturezas = repNaturezaDaOperacao.Consultar(descricao, inicioRegistros, 50);
                int countNaturezas = repNaturezaDaOperacao.ContarConsulta(descricao);

                var retorno = from obj in listaNaturezas select new { obj.Codigo, obj.Descricao };

                return Json(retorno, true, null, new string[] { "Código|10", "Descrição|70" }, countNaturezas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as naturezas das operações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion
    }
}
