using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class MensagemDeAvisoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("mensagensdeaviso.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string titulo = Request.Params["Titulo"];

                DateTime dataInicial, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.MensagemAviso repMensagemAviso = new Repositorio.MensagemAviso(unitOfWork);

                List<Dominio.Entidades.MensagemAviso> listaMensagens = repMensagemAviso.Consultar(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, titulo, inicioRegistros, 50);
                int countMensagens = repMensagemAviso.ContarConsulta(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, titulo);

                var retorno = from obj in listaMensagens select new { obj.Codigo, obj.Status, obj.Titulo, obj.Descricao, DataInicial = obj.DataInicio.ToString("dd/MM/yyyy"), DataFinal = obj.DataFim.ToString("dd/MM/yyyy") };

                return Json(retorno, true, null, new string[] { "Codigo", "Status", "Titulo|26", "Descricao|40", "Data Inicial|12", "Data Final|12" }, countMensagens);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as mensagens de aviso.");
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

                string mensagem = System.Uri.UnescapeDataString(Request.Params["Mensagem"]);
                string titulo = Request.Params["Titulo"];
                string status = Request.Params["Status"];

                DateTime dataInicial, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                if (dataInicial == DateTime.MinValue)
                    return Json<bool>(false, false, "Data incial inválida.");

                if (dataFinal == DateTime.MinValue)
                    return Json<bool>(false, false, "Data final inválida.");

                if (string.IsNullOrWhiteSpace(titulo))
                    return Json<bool>(false, false, "Título inválido.");

                if (string.IsNullOrWhiteSpace(mensagem))
                    return Json<bool>(false, false, "Mensagem inválida.");

                Repositorio.MensagemAviso repMensagemAviso = new Repositorio.MensagemAviso(unitOfWork);

                Dominio.Entidades.MensagemAviso mensagemAviso = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    mensagemAviso = repMensagemAviso.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");

                    mensagemAviso = new Dominio.Entidades.MensagemAviso();
                }

                mensagemAviso.DataFim = dataFinal;
                mensagemAviso.DataInicio = dataInicial;
                mensagemAviso.Descricao = mensagem;
                mensagemAviso.Status = status;
                mensagemAviso.Ativo = status == "A" ? true : false;
                mensagemAviso.Titulo = titulo;
                mensagemAviso.Empresa = this.EmpresaUsuario;
                mensagemAviso.TipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe;

                if (codigo > 0)
                    repMensagemAviso.Atualizar(mensagemAviso);
                else
                    repMensagemAviso.Inserir(mensagemAviso);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a mensagem de aviso.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterMensagensParaExibicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.EmpresaPai == null)
                    return Json<bool>(false, false, "Empresa inválida.");

                Repositorio.MensagemAviso repMensagemAviso = new Repositorio.MensagemAviso(unitOfWork);
                List<Dominio.Entidades.MensagemAviso> listaMensagens = repMensagemAviso.BuscarParaExibicao(this.EmpresaUsuario.EmpresaPai.Codigo);

                var result = from obj in listaMensagens select new { obj.Titulo, obj.Descricao };

                return Json(result, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as mensagens de aviso.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
