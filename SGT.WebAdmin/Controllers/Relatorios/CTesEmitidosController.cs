//using Microsoft.Reporting.WebForms;
//using SGTAdmin.Controllers;
//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Mvc;

//namespace SGT.WebAdmin.Controllers.Relatorios
//{
//    [CustomAuthorize("RelatoriosRV/CTesEmitidos")]
//    public class CTesEmitidosController : BaseController
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

//                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_conexao.StringConexao);

//                List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupado> listaCTesAgrupados = repCTe.RelatorioCTesAgrupados(this.Empresa.Codigo, codigoTransportador, dataEmissaoInicial, dataEmissaoFinal, dataAutorizacaoInicial, dataAutorizacaoFinal, null);

//                List<ReportParameter> parametros = new List<ReportParameter>();
//                parametros.Add(new ReportParameter("Empresa", this.Empresa.RazaoSocial));
//                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataEmissaoInicial.ToString("dd/MM/yyyy"), " até ", dataEmissaoFinal.ToString("dd/MM/yyyy"))));
//                parametros.Add(new ReportParameter("EmpresaEmissora", empresa != null ? empresa.RazaoSocial : "Todos"));

//                List<ReportDataSource> dataSources = new List<ReportDataSource>();
//                dataSources.Add(new ReportDataSource("CTesAgrupados", listaCTesAgrupados));

//                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(_conexao.StringConexao);

//                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCTesEmitidos.rdlc", "PDF", parametros, dataSources);

//                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioCTesEmitidos." + arquivo.FileNameExtension.ToLower());
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);

//                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
//            }
//        }
//    }
//}
