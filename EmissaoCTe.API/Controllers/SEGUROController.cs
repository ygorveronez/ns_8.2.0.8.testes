using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class SEGUROController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("seguro.aspx") select obj).FirstOrDefault();
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

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]);

                int codigoLayout = 0;
                int.TryParse(Request.Params["Versao"], out codigoLayout);

                Repositorio.LayoutEDI repLayout = new Repositorio.LayoutEDI(unitOfWork);
                Dominio.Entidades.LayoutEDI layout = repLayout.BuscarPorCodigoETipo(codigoLayout, Dominio.Enumeradores.TipoLayoutEDI.SEGURO);

                if (layout == null)
                    return Json<bool>(false, false, "Layout do arquivo não encontrado.");

                System.IO.MemoryStream arquivo = this.GerarSEGURO(cpfCnpjRemetente, dataInicial, dataFinal, layout, unitOfWork);

                return Arquivo(arquivo, "text/plain", string.Concat("SEGURO_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o arquivo SEGURO.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private System.IO.MemoryStream GerarSEGURO(string cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.LayoutEDI layout, Repositorio.UnitOfWork unitOfWork, int codigoVeiculo = 0)
        {
            Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unitOfWork, layout, this.EmpresaUsuario, cpfCnpjRemetente, string.Empty, dataInicial, dataFinal, codigoVeiculo, false, null, 0, null, null);

            return svcEDI.GerarArquivo();
        }

        #endregion
    }
}
