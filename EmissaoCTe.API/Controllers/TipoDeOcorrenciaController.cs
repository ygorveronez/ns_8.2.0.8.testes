using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class TipoDeOcorrenciaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("tiposdeocorrencias.aspx") select obj).FirstOrDefault();
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

                Repositorio.TipoDeOcorrencia repTipoOcorrencia = new Repositorio.TipoDeOcorrencia(unitOfWork);
                var listaTipoOcorrencia = repTipoOcorrencia.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countTipoOcorrencia = repTipoOcorrencia.ContarConsulta(this.EmpresaUsuario.Codigo, status, descricao);

                var retorno = from obj in listaTipoOcorrencia select new { obj.Codigo, obj.Status, obj.Tipo, obj.Descricao, obj.DescricaoTipo, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Tipo", "Descrição|60", "Tipo|15", "Status|15" }, countTipoOcorrencia);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os tipos de ocorrências.");
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
                string tipo = Request.Params["Tipo"];
                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");
                if (string.IsNullOrWhiteSpace(tipo))
                    return Json<bool>(false, false, "Tipo inválido.");
                Repositorio.TipoDeOcorrencia repTipoOcorrencia = new Repositorio.TipoDeOcorrencia(unitOfWork);
                Dominio.Entidades.TipoDeOcorrencia tipoOcorrencia;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    tipoOcorrencia = new Dominio.Entidades.TipoDeOcorrencia();
                    tipoOcorrencia.Status = "A";
                    tipoOcorrencia.DataCadastro = DateTime.Now;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }

                tipoOcorrencia.Descricao = descricao;
                tipoOcorrencia.Empresa = this.EmpresaUsuario;
                tipoOcorrencia.Tipo = tipo;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    tipoOcorrencia.Status = status;

                if (codigo > 0)
                    repTipoOcorrencia.Atualizar(tipoOcorrencia);
                else
                    repTipoOcorrencia.Inserir(tipoOcorrencia);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o tipo de ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
