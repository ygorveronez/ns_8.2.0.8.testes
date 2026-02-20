using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EFPHController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("efph.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST", "GET")]
        public ActionResult Gerar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar este recurso!");

                DateTime dataInicial = DateTime.MinValue;
                DateTime dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Servicos.EFPH svcEFPH = new Servicos.EFPH(unitOfWork, this.EmpresaUsuario, dataInicial, dataFinal);
                System.IO.MemoryStream arquivo = svcEFPH.Gerar();

                return Arquivo(arquivo, "text/plain", string.Concat("EFPH_", dataInicial.ToString("ddMMyy"), "-", dataFinal.ToString("ddMMyy"), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o arquivo EFPH.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
