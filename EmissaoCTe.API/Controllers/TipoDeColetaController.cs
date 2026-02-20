using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class TipoDeColetaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("tiposdecoletas.aspx") select obj).FirstOrDefault();
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

                Repositorio.TipoColeta repTipoColeta = new Repositorio.TipoColeta(unitOfWork);

                List<Dominio.Entidades.TipoColeta> listaTipoCustoFixo = repTipoColeta.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countTipoCustoFixo = repTipoColeta.ContarConsulta(this.EmpresaUsuario.Codigo, status, descricao);

                var retorno = from obj in listaTipoCustoFixo select new { obj.Codigo, obj.Status, obj.Descricao, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Descrição|60", "Status|20" }, countTipoCustoFixo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os tipos de coletas.");
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
                string status = Request.Params["Status"];
                string descricao = Request.Params["Descricao"];

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");

                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                Repositorio.TipoColeta repTipoColeta = new Repositorio.TipoColeta(unitOfWork);
                Dominio.Entidades.TipoColeta tipoColeta;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    tipoColeta = repTipoColeta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    tipoColeta = new Dominio.Entidades.TipoColeta();
                    tipoColeta.Status = "A";
                }

                tipoColeta.Descricao = descricao;
                tipoColeta.Empresa = this.EmpresaUsuario;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    tipoColeta.Status = status;

                if (codigo > 0)
                    repTipoColeta.Atualizar(tipoColeta);
                else
                    repTipoColeta.Inserir(tipoColeta);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o tipo de coleta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
