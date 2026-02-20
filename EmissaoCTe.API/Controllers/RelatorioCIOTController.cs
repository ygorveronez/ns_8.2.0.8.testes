using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioCIOTController : ApiController
    {

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                double.TryParse(Request.Params["Transportador"], out double cpfCnpjTransp);

                int.TryParse(Request.Params["Motorista"], out int codMotorista);
                int.TryParse(Request.Params["Veiculo"], out int codVeiculo);

                Dominio.Entidades.Cliente transportador = cpfCnpjTransp > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTransp) : null;
                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codMotorista);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codVeiculo);

                List<Dominio.ObjetosDeValor.Relatorios.RelatorioCIOT> ciots = repCIOT.RelatorioCIOT(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, cpfCnpjTransp, codMotorista, codVeiculo);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.CNPJ_Formatado + " - " + this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("PeriodoEmissao", FormataPeriodoData(dataInicial, dataFinal, "Todos")));
                parametros.Add(new ReportParameter("Transportador", transportador?.Nome ?? "Todos"));
                parametros.Add(new ReportParameter("Motorista", motorista?.Nome ?? "Todos"));
                parametros.Add(new ReportParameter("Veiculo", veiculo?.Placa ?? "Todos"));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("CIOTS", ciots));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCIOTS.rdlc", "PDF", parametros, dataSources);
                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioCIOT." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string FormataPeriodoData(DateTime inicio, DateTime fim, string dft)
        {
            string _formato = "dd/MM/yyyy";
            string periodo = string.Empty;

            if (inicio != DateTime.MinValue)
                periodo += " De " + inicio.ToString(_formato);

            if (fim != DateTime.MinValue)
                periodo += " Até " + fim.ToString(_formato);

            periodo = periodo.Trim();
            return string.IsNullOrWhiteSpace(periodo) ? dft : periodo;
        }
    }
}