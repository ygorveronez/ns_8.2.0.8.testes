//using Microsoft.Reporting.WebForms;
//using SGTAdmin.Controllers;
//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Mvc;

//namespace SGT.WebAdmin.Controllers.Relatorios
//{
//    [CustomAuthorize("RelatoriosRV/CTesEmitidosEmbarcador")]
//    public class CTesEmitidosEmbarcadorController : BaseController
//    {
//        public async Task<IActionResult> DownloadRelatorio()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

//            try
//            {
//                int codigoTransportador = 0;
//                int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador);

//                double cpfCnpjEmbarcador;
//                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("CodigoEmbarcador")), out cpfCnpjEmbarcador);

//                DateTime dataAutorizacaoInicial, dataAutorizacaoFinal, dataEmissaoInicial, dataEmissaoFinal;
//                DateTime.TryParseExact(Request.Params("DataAutorizacaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoInicial);
//                DateTime.TryParseExact(Request.Params("DataAutorizacaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoFinal);
//                DateTime.TryParseExact(Request.Params("DataEmissaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
//                DateTime.TryParseExact(Request.Params("DataEmissaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

//                bool todosCNPJRaiz;
//                bool.TryParse(Request.Params("TodosRaizCNPJEmbarcador"), out todosCNPJRaiz);

//                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
//                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

//                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);
//                Dominio.Entidades.Cliente cliente = cpfCnpjEmbarcador > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpjEmbarcador) : null;

//                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

//                List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador> listaCTes = repCTe.RelatorioCTesEmitidosPorEmbarcador(this.Empresa.Codigo, codigoTransportador, cliente != null ? cliente.CPF_CNPJ_SemFormato : null, dataAutorizacaoInicial, dataAutorizacaoFinal, dataEmissaoInicial, dataEmissaoFinal, todosCNPJRaiz);

//                List<ReportDataSource> dataSources = new List<ReportDataSource>();
//                dataSources.Add(new ReportDataSource("CTes", listaCTes));

//                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(_conexao.StringConexao);

//                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCTesEmitidosPorEmbarcador.rdlc", "PDF", null, dataSources);

//                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioCTesEmitidosPorEmbarcador." + arquivo.FileNameExtension.ToLower());
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);

//                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }
//    }
//}
