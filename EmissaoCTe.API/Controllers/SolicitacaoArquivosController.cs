using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class SolicitacaoArquivosController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("solicitacaoarquivos.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST", "GET")]
        public ActionResult Solicitar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para a solicitação de Arquivos.");

                int tipoArquivo = 0;
                int.TryParse(Request.Params["Tipo"], out tipoArquivo);

                string emails = Request.Params["Emails"];
                string mes = Utilidades.String.OnlyNumbers(Request.Params["MesAno"]).Substring(0,2);
                string ano = Utilidades.String.OnlyNumbers(Request.Params["MesAno"]).Substring(2,4);

                Servicos.SolicitarArquivos svcSolicitadao = new Servicos.SolicitarArquivos(unitOfWork);
                if (svcSolicitadao.Solicitar(this.EmpresaUsuario.CNPJ, int.Parse(mes), int.Parse(ano), tipoArquivo, emails, unitOfWork))
                    return Json<bool>(true, true, "Arquivo solicitado com sucesso.");
                else
                    return Json<bool>(false, false, "Não foi possível solicitar envio de arquivos.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao solicitar arquivos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
