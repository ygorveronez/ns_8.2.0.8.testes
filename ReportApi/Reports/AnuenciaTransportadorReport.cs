using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.AnuenciaTransportador)]
public class AnuenciaTransportadorReport : ReportBase
{
    public AnuenciaTransportadorReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var empresa = BuscarEmpresa(extraData.GetValue<int>("EmpresaCodigo"));
        var conciliacao =
            new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(_unitOfWork).BuscarPorCodigo(
                extraData.GetValue<int>("CodigoConciliacao"), false);
        string saldoTotal = extraData.GetValue<string>("SaldoTotal");

        X509Certificate2 certificado = new X509Certificate2(empresa.NomeCertificado, empresa.SenhaCertificado);

        Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador =
            new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(_unitOfWork);
        Dominio.Entidades.Empresa transportadorParaAssinar =
            repConciliacaoTransportador.ObterTransportadorParaAssinatura(conciliacao);

        string dataAssinatura = conciliacao.DataAssinaturaAnuencia.Value.ToDateTimeString(true);
        string validadeCertificado = DateTime.Parse(certificado.GetEffectiveDateString()).ToDateString() + " até " +
                                     DateTime.Parse(certificado.GetExpirationDateString()).ToDateString();

        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AnuenciaTransportador> dsProtocolo =
            new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AnuenciaTransportador>()
            {
                new Dominio.Relatorios.Embarcador.DataSource.Financeiros.AnuenciaTransportador()
                {
                    NomeEmpresa = transportadorParaAssinar.RazaoSocial,
                    CNPJ = transportadorParaAssinar.CNPJ_Formatado,
                    Endereco = transportadorParaAssinar.Endereco,
                    Bairro = transportadorParaAssinar.Bairro,
                    Numero = transportadorParaAssinar.Numero,
                    Cidade = transportadorParaAssinar.Localidade.Descricao,
                    Estado = transportadorParaAssinar.Localidade.Estado.Sigla,
                    DataInicial = conciliacao.DataInicial.ToDateString(),
                    DataFinal = conciliacao.DataFinal.ToDateString(),
                    ValorConciliacao = saldoTotal,

                    CertificadoEmitidoPor = certificado.GetNameInfo(X509NameType.SimpleName, true),
                    ValidadeCertificado = validadeCertificado,
                    ImpressaoDigitalCertificado = certificado.Thumbprint,
                    DataAssinatura = dataAssinatura
                }
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsProtocolo,
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>() { }
            };

        byte[] pdfData = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\ConciliacaoTransportador\AnuenciaTransportador.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoArquivos"], "AnuenciaTransportador", conciliacao.Codigo + ".pdf"), pdfData);

        return PrepareReportResult(FileType.PDF, pdfData);
    }
}