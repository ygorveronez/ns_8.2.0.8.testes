using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EBSController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("ebs.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

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

                int codigoLayout, codigoSerie;
                int.TryParse(Request.Params["Versao"], out codigoLayout);
                int.TryParse(Request.Params["Serie"], out codigoSerie);

                Repositorio.LayoutEDI repLayout = new Repositorio.LayoutEDI(unitOfWork);
                Dominio.Entidades.LayoutEDI layout = repLayout.BuscarPorCodigoETipo(codigoLayout, Dominio.Enumeradores.TipoLayoutEDI.EBS);

                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Dominio.Entidades.EmpresaSerie serie = repSerie.BuscarPorCodigo(codigoSerie);

                if (layout == null)
                    return Json<bool>(false, false, "Layout do arquivo não encontrado.");

                System.IO.MemoryStream arquivo = this.GerarEBS(dataInicial, dataFinal, layout, serie, unitOfWork);

                return Arquivo(arquivo, "text/plain", string.Concat("EBS_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o arquivo EBS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private System.IO.MemoryStream GerarEBS(DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.LayoutEDI layout, Dominio.Entidades.EmpresaSerie serie, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.GeracaoEDI svcEDI = null;

            svcEDI = new Servicos.GeracaoEDI(unitOfWork, layout, this.EmpresaUsuario, "", "", dataInicial, dataFinal, 0, false, null, 0, new string[] { "A", "C" }, serie != null ? new int[] { serie.Codigo } : null);

            return svcEDI.GerarArquivo();
        }

        #endregion
    }
}
