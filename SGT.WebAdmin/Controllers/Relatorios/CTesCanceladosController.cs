//using Microsoft.Reporting.WebForms;
//using SGTAdmin.Controllers;
//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Mvc;

//namespace SGT.WebAdmin.Controllers.Relatorios
//{
//    [CustomAuthorize("RelatoriosRV/CTesCancelados")]
//    public class CTesCanceladosController : BaseController
//    {
//        public async Task<IActionResult> DownloadRelatorio()
//        {
//            try
//            {
//                int codigoTransportador = 0;
//                int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador);

//                DateTime dataInicial, dataFinal;
                
//                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
//                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

//                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_conexao.StringConexao);
//                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

//                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_conexao.StringConexao);

//                List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesCancelados> listaCTesCancelados = repCTe.RelatorioCTesCancelados(this.Empresa.Codigo, codigoTransportador, dataInicial, dataFinal);

//                List<ReportParameter> parametros = new List<ReportParameter>();
//                parametros.Add(new ReportParameter("Empresa", this.Empresa.RazaoSocial));
//                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
//                parametros.Add(new ReportParameter("EmpresaEmissora", empresa != null ? empresa.RazaoSocial : "Todos"));

//                List<ReportDataSource> dataSources = new List<ReportDataSource>();
//                dataSources.Add(new ReportDataSource("CTesCancelados", listaCTesCancelados));

//                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(_conexao.StringConexao);

//                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCTesCancelados.rdlc", "PDF", parametros, dataSources);

//                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioCTesCancelados." + arquivo.FileNameExtension.ToLower());
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);

//                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
//            }
//        }
//    }
//}
