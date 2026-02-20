using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioDocumentosTransporteController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int codigoVeiculo, codigoMotorista, numeroDT, numeroCTe, numeroNFSe, numeroNFe;
                int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["Motorista"], out codigoMotorista);
                int.TryParse(Request.Params["NumeroDT"], out numeroDT);
                int.TryParse(Request.Params["NumeroCTe"], out numeroCTe);
                int.TryParse(Request.Params["NumeroNFSe"], out numeroNFSe);
                int.TryParse(Request.Params["NumeroNFe"], out numeroNFe);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.NotaFiscalDocumentoTransporteNatura repNotas = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
                Repositorio.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.DocumentoTransporteNatura(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo) : null;
                Dominio.Entidades.Usuario motorista = codigoMotorista > 0 ? repUsuario.BuscarPorCodigo(codigoMotorista) : null;

                List<Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosTransporte> documentos = repDocumentoTransporteNatura.Relatorio(this.EmpresaUsuario.Codigo, codigoVeiculo, codigoMotorista, dataInicial, dataFinal, numeroDT, numeroCTe, numeroNFSe, numeroNFe);
                
                //List<Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosTransporte> documentos = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosTransporte>();

                //for(var i = 0; i < dts.Count; i++)
                //{
                //    Dominio.Entidades.DocumentoTransporteNatura dt = dts[i];
                //    string notas = repNotas.BuscarNotasPorDocumentoTransporte(dt.Codigo);

                //    for (var j = 0; j < dt.NotasFiscais.Count; j++)
                //    {
                //        Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal = dt.NotasFiscais[j];
                //        documentos.Add(new Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosTransporte()
                //        {
                //            Numero = (int)dt.NumeroDT,
                //            Data = dt.DataEmissao,
                //            Valor = dt.ValorFrete,
                //            Veiculo = dt.Veiculo?.Placa ?? string.Empty,
                //            Motorista = dt.Motorista?.Nome ?? string.Empty,
                //            Status = dt.DescricaoStatus,
                //            Notas = notas,

                //            TipoDocumento = notaFiscal.CTe != null ? "CT-e" : notaFiscal.NFSe != null ? "NFs-e" : string.Empty,
                //            DocumentoNumero = notaFiscal.CTe != null ? notaFiscal.CTe.Numero : notaFiscal.NFSe != null ? notaFiscal.NFSe.Numero : 0,
                //            DocumentoSerie = notaFiscal.CTe != null ? notaFiscal.CTe.Serie.Numero.ToString() : notaFiscal.NFSe != null ? notaFiscal.NFSe.Serie.Numero.ToString() : string.Empty,
                //            DocumentoDataEmissao = notaFiscal.CTe != null ? notaFiscal.CTe.DataEmissao : notaFiscal.NFSe != null ? notaFiscal.NFSe.DataEmissao : DateTime.Today,
                //            DocumentoValor = notaFiscal.CTe != null ? notaFiscal.CTe.ValorAReceber : notaFiscal.NFSe != null ? notaFiscal.NFSe.ValorServicos : 0,
                //        });
                //    }
                //}

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Periodo", this.MontaPeriodo(dataInicial, dataFinal)));
                parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : "Todos"));
                parametros.Add(new ReportParameter("Motorista", motorista != null ? motorista.Nome : "Todos"));
                parametros.Add(new ReportParameter("NumeroDT", numeroDT > 0 ? numeroDT.ToString() : "--"));
                parametros.Add(new ReportParameter("NumeroCTe", numeroCTe > 0 ? numeroCTe.ToString() : "--"));
                parametros.Add(new ReportParameter("NumeroNFSe", numeroNFSe > 0 ? numeroNFSe.ToString() : "--"));
                parametros.Add(new ReportParameter("NumeroNFe", numeroNFe > 0 ? numeroNFe.ToString() : "--"));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Documentos", documentos));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo;

                arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioDocumentosTransporte.rdlc", "PDF", parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioDocumentosTransporte." + arquivo.FileNameExtension);
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
