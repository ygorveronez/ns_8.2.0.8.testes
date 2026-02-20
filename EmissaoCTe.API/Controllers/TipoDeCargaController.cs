using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class TipoDeCargaController : ApiController
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

                Repositorio.TipoCarga repNaturezaDaOperacao = new Repositorio.TipoCarga(unitOfWork);
                var listaTipoCarga = repNaturezaDaOperacao.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countTipoCarga = repNaturezaDaOperacao.ContarConsulta(this.EmpresaUsuario.Codigo, status, descricao);

                var retorno = from obj in listaTipoCarga select new { obj.Codigo, obj.Status, obj.Descricao, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Descrição|60", "Status|20" }, countTipoCarga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os tipos de cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarTipoCargaEmbarcador()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string descricao = Request.Params["Descricao"];

                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);

                var listaTipoCarga = repTipoCarga.Consultar(descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, 0, null, null, null, 0, "Descricao", "asc", inicioRegistros, 50);
                int countTipoCarga = repTipoCarga.ContarConsulta(descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, 0, null, null, null, 0);

                var retorno = from obj in listaTipoCarga select new { obj.Codigo, obj.Descricao };

                return Json(retorno, true, null, new string[] { "Código", "Descrição|80" }, countTipoCarga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os tipos de cargas.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
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
                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");
                Repositorio.TipoCarga repTipoCarga = new Repositorio.TipoCarga(unitOfWork);
                Dominio.Entidades.TipoCarga tipoCarga;
                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    tipoCarga = repTipoCarga.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    tipoCarga = new Dominio.Entidades.TipoCarga();
                    tipoCarga.Status = "A";
                }
                tipoCarga.Descricao = descricao;
                tipoCarga.Empresa = this.EmpresaUsuario;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    tipoCarga.Status = status;

                if (codigo > 0)
                    repTipoCarga.Atualizar(tipoCarga);
                else
                    repTipoCarga.Inserir(tipoCarga);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o tipo de carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
