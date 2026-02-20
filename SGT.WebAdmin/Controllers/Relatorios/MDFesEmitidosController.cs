//using Microsoft.Reporting.WebForms;
//using SGTAdmin.Controllers;
//using Microsoft.AspNetCore.Mvc;

//namespace SGT.WebAdmin.Controllers.Relatorios
//{
//    [CustomAuthorize("RelatoriosRV/MDFesEmitidos")]
//    public class MDFesEmitidosController : BaseController
//    {
//        public async Task<IActionResult> DownloadRelatorio()
//        {
//            try
//            {
//                int codigoTransportador = 0;
//                int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador);

//                DateTime dataAutorizacaoInicial, dataAutorizacaoFinal, dataEmissaoInicial, dataEmissaoFinal;

//                DateTime.TryParseExact(Request.Params("DataAutorizacaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoInicial);
//                DateTime.TryParseExact(Request.Params("DataAutorizacaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoFinal);
//                DateTime.TryParseExact(Request.Params("DataEmissaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
//                DateTime.TryParseExact(Request.Params("DataEmissaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

//                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_conexao.StringConexao);
//                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

//                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_conexao.StringConexao);
//                List<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFeAgrupado> listaMDFesAgrupados = repMDFe.RelatorioMDFesAgrupados(this.Empresa.Codigo, codigoTransportador, dataEmissaoInicial, dataEmissaoFinal, dataAutorizacaoInicial, dataAutorizacaoFinal);
                
//                List<ReportParameter> parametros = new List<ReportParameter>();
//                parametros.Add(new ReportParameter("Empresa", this.Empresa.RazaoSocial));
//                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataEmissaoInicial.ToString("dd/MM/yyyy"), " até ", dataEmissaoFinal.ToString("dd/MM/yyyy"))));
//                parametros.Add(new ReportParameter("EmpresaEmissora", empresa != null ? empresa.RazaoSocial : "Todos"));

//                List<ReportDataSource> dataSources = new List<ReportDataSource>();
//                dataSources.Add(new ReportDataSource("MDFesAgrupados", listaMDFesAgrupados));

//                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(_conexao.StringConexao);

//                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioMDFesEmitidos.rdlc", "PDF", parametros, dataSources);

//                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioMDFesEmitidos." + arquivo.FileNameExtension.ToLower());
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);

//                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
//            }
//        }
//    }
//}
