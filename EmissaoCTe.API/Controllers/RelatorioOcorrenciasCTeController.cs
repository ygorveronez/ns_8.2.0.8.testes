using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioOcorrenciasCTeController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("relatorioocorrenciascte.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão para download do relatório negada!");

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["Remetente"]);
                string cpfCnpjExpedidor = Utilidades.String.OnlyNumbers(Request.Params["Expedidor"]);
                string cpfCnpjRecebedor = Utilidades.String.OnlyNumbers(Request.Params["Recebedor"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]);
                string cpfCnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Tomador"]);
                string numeroNF = Request.Params["NumeroNF"];
                string tipoArquivo = Request.Params["TipoArquivo"];
                string tipoRelatorio = Request.Params["TipoRelatorio"];
                string tipoOcorrencia = Request.Params["TipoOcorrencia"];

                int codigoLocalidadeInicioPrestacao, codigoLocalidadeTerminoPrestacao, codigoOcorrencia, numeroInicial, numeroFinal;
                int.TryParse(Request.Params["CodigoLocalidadeInicio"], out codigoLocalidadeInicioPrestacao);
                int.TryParse(Request.Params["CodigoLocalidadeFim"], out codigoLocalidadeTerminoPrestacao);
                int.TryParse(Request.Params["CodigoOcorrencia"], out codigoOcorrencia);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                Repositorio.OcorrenciaDeCTe repOcorrenciaCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);

                List<Dominio.ObjetosDeValor.Relatorios.RelatorioOcorrenciasCTe> ocorrencias = repOcorrenciaCTe.ObterRelatorio(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, codigoLocalidadeInicioPrestacao, codigoLocalidadeTerminoPrestacao, codigoOcorrencia, tipoOcorrencia, numeroInicial, numeroFinal, numeroNF);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial != DateTime.MinValue ? dataInicial.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataFinal != DateTime.MinValue ? dataFinal.ToString("dd/MM/yyyy") : "99/99/9999")));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Ocorrencias", ocorrencias));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/" + tipoRelatorio + ".rdlc", tipoArquivo, parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioOcorrenciasCTe." + arquivo.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar relatório!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
