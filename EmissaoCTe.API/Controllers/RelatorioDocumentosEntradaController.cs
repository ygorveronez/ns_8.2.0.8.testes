using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioDocumentosEntradaController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal, dataEntradaInicial, dataEntradaFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);
                DateTime.TryParseExact(Request.Params["DataEntradaInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntradaInicial);
                DateTime.TryParseExact(Request.Params["DataEntradaFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntradaFinal);

                int codigoVeiculo, numeroInicial, numeroFinal;
                int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                double cpfCnpjFornecedor;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Fornecedor"]), out cpfCnpjFornecedor);

                Dominio.Enumeradores.StatusDocumentoEntrada? status = null;
                Dominio.Enumeradores.StatusDocumentoEntrada statusAux;

                if (Enum.TryParse<Dominio.Enumeradores.StatusDocumentoEntrada>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                string tipoArquivo = Request.Params["TipoArquivo"];
                string relatorio = Request.Params["Relatorio"];
                string ordenacao = Request.Params["Ordenacao"];
                if (string.IsNullOrWhiteSpace(ordenacao))
                    ordenacao = "DataEntrada";

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.DocumentoEntrada repDocumentoEntrada = new Repositorio.DocumentoEntrada(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo) : null;
                Dominio.Entidades.Cliente cliente = cpfCnpjFornecedor > 0f ? repCliente.BuscarPorCPFCNPJ(cpfCnpjFornecedor) : null;

                List<Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosEntrada> documentos = repDocumentoEntrada.Relatorio(this.EmpresaUsuario.Codigo, codigoVeiculo, cpfCnpjFornecedor, dataInicial, dataFinal, dataEntradaInicial, dataEntradaFinal, numeroInicial, numeroFinal, status, ordenacao);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Periodo", this.MontaPeriodo(dataInicial, dataFinal)));
                parametros.Add(new ReportParameter("PeriodoEntrada", this.MontaPeriodo(dataEntradaInicial, dataEntradaFinal)));
                parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : "Todos"));
                parametros.Add(new ReportParameter("Fornecedor", cliente != null ? cliente.CPF_CNPJ_Formatado + " - " + cliente.Nome : "Todos"));
                parametros.Add(new ReportParameter("Status", status != null ? status.Value.ToString("G") : "Todos"));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Documentos", documentos));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo;

                if (relatorio.ToLower().Contains("cfop"))
                {
                    arquivo = svcRelatorio.GerarWeb("Relatorios/" + relatorio + ".rdlc", tipoArquivo, parametros, dataSources, (object sender, SubreportProcessingEventArgs e) =>
                    {
                        List<Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosEntradaCFOP> listaCFOP = repDocumentoEntrada.RelatorioCFOP(this.EmpresaUsuario.Codigo, int.Parse(e.Parameters["CodigoDocumentoEntrada"].Values[0]));
                        e.DataSources.Add(new ReportDataSource("ListaCFOP", listaCFOP));
                    });
                }
                else
                {
                    arquivo = svcRelatorio.GerarWeb("Relatorios/" + relatorio + ".rdlc", tipoArquivo, parametros, dataSources);
                }

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioDocumentosEntrada." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        private string MontaPeriodo(DateTime inicio, DateTime final)
        {
            string periodo = "Nenhum período";

            if (inicio != DateTime.MinValue && final != DateTime.MinValue)
                periodo = string.Concat("De ", inicio.ToString("dd/MM/yyyy"), " até ", final.ToString("dd/MM/yyyy"));

            else if (inicio != DateTime.MinValue)
                periodo = string.Concat("De ", inicio.ToString("dd/MM/yyyy"));

            else if (final != DateTime.MinValue)
                periodo = string.Concat("Até ", final.ToString("dd/MM/yyyy"));

            return periodo;
        }
    }
}
