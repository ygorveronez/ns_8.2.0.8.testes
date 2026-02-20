using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class SINTEGRAController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("sintegra.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public ActionResult Gerar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para a geração do SINTEGRA.");

                DateTime dataInicial = DateTime.MinValue;
                DateTime dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string codigoEstruturaArquivo = Request.Params["CodigoEstruturaArquivo"];
                string codigoNaturezaOperacoes = Request.Params["CodigoNaturezaOperacoes"];
                string codigoFinalidadeArquivo = Request.Params["CodigoFinalidadeArquivo"];

                Servicos.SINTEGRA svcSintegra = new Servicos.SINTEGRA(this.EmpresaUsuario, dataInicial, dataFinal, codigoEstruturaArquivo, codigoNaturezaOperacoes, codigoFinalidadeArquivo, unitOfWork);
                return Arquivo(svcSintegra.Gerar(), "text/plain", string.Concat("SINTEGRA_", dataInicial.ToString("ddMMyy"), "-", dataFinal.ToString("ddMMyy"), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o SINTEGRA.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
