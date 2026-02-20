using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class CampoDaCartaDeCorrecaoEletronicaController : ApiController
    {

        #region Propriedades

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("camposdacartadecorrecaoeletronica.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string nomeCampo = Request.Params["NomeCampo"];
                string descricaoCampo = Request.Params["Descricao"];
                string grupoCampo = Request.Params["GrupoCampo"];

                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);

                List<Dominio.Entidades.CampoCCe> listaCamposCCe = repCampoCCe.Consultar(descricaoCampo, nomeCampo, grupoCampo, "", "Descricao", "asc", inicioRegistros, 50);
                int countCamposCCe = repCampoCCe.ContarConsulta(descricaoCampo, nomeCampo, grupoCampo, "");

                var result = from obj in listaCamposCCe select new { obj.Codigo, obj.GrupoCampo, obj.NomeCampo, obj.Descricao };

                return Json(result, true, null, new string[] { "Codigo", "Grupo|30", "Nome|30", "Descrição|30" }, countCamposCCe);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os campos da CC-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["CodigoCampoCCe"], out codigo);

                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);
                Dominio.Entidades.CampoCCe campoCCe = repCampoCCe.BuscarPorCodigo(codigo);

                if (campoCCe == null)
                    return Json<bool>(false, false, "Campo da CC-e não encontrado.");

                var retorno = new
                {
                    campoCCe.Codigo,
                    campoCCe.Descricao,
                    campoCCe.GrupoCampo,
                    campoCCe.IndicadorRepeticao,
                    campoCCe.NomeCampo,
                    campoCCe.QuantidadeCaracteres,
                    campoCCe.QuantidadeDecimais,
                    campoCCe.QuantidadeInteiros,
                    campoCCe.TipoCampo,
                    campoCCe.Status
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do campo da CC-e.");
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
                int codigo, quantidadeCaracteres, quantidadeDecimais, quantidadeInteiros = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["QuantidadeCaracteres"], out quantidadeCaracteres);
                int.TryParse(Request.Params["QuantidadeDecimais"], out quantidadeDecimais);
                int.TryParse(Request.Params["QuantidadeInteiros"], out quantidadeInteiros);

                Dominio.Enumeradores.TipoCampoCCe tipoCampo;
                Enum.TryParse<Dominio.Enumeradores.TipoCampoCCe>(Request.Params["TipoCampo"], out tipoCampo);

                bool indicadorRepeticao = false;
                bool.TryParse(Request.Params["IndicadorRepeticao"], out indicadorRepeticao);

                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];
                string nomeCampo = Request.Params["NomeCampo"];
                string grupoCampo = Request.Params["GrupoCampo"];

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");

                if (string.IsNullOrWhiteSpace(nomeCampo))
                    return Json<bool>(false, false, "Nome do campo inválido.");

                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);

                Dominio.Entidades.CampoCCe campoCCe = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    campoCCe = repCampoCCe.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    campoCCe = new Dominio.Entidades.CampoCCe();
                }

                campoCCe.Descricao = descricao;
                campoCCe.GrupoCampo = grupoCampo;
                campoCCe.IndicadorRepeticao = indicadorRepeticao;
                campoCCe.NomeCampo = nomeCampo;
                campoCCe.QuantidadeCaracteres = quantidadeCaracteres;
                campoCCe.QuantidadeDecimais = quantidadeDecimais;
                campoCCe.QuantidadeInteiros = quantidadeInteiros;
                campoCCe.TipoCampo = tipoCampo;
                campoCCe.Status = status;

                if (campoCCe.Codigo > 0)
                    repCampoCCe.Atualizar(campoCCe);
                else
                    repCampoCCe.Inserir(campoCCe);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o campo da CC-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

    }
}
