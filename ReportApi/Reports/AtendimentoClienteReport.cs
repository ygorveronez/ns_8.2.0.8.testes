using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.AtendimentoCliente)]
public class AtendimentoClienteReport : ReportBase
{
    public AtendimentoClienteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        // Instancia repositorios
        Repositorio.Embarcador.CTe.CTeDocumentoOriginario repCTeDocumentoOriginario =
            new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(_unitOfWork);

        // Parametros
        int codigo = extraData.GetValue<int>("codigo");

        // Valida
        if (codigo == 0)
            throw new ServicoException("Não foi possível encontrar o registro.");

        IList<Dominio.Relatorios.Embarcador.DataSource.SAC.Documento> dsDocumentos =
            repCTeDocumentoOriginario.DadosConhecimento(codigo);
        IList<Dominio.Relatorios.Embarcador.DataSource.SAC.ComponentesDocumento> dsComponentes =
            repCTeDocumentoOriginario.ComponentesConhecimento(codigo);
        IList<Dominio.Relatorios.Embarcador.DataSource.SAC.PesoDocumento> dsPesos =
            repCTeDocumentoOriginario.PesosConhecimento(codigo);
        IList<Dominio.Relatorios.Embarcador.DataSource.SAC.NotasFiscaisDocumento> dsNotas =
            repCTeDocumentoOriginario.NotasFiscaisConhecimento(codigo);
        IList<Dominio.Relatorios.Embarcador.DataSource.SAC.FaturamentoDocumento> dsFaturas =
            repCTeDocumentoOriginario.FaturasConhecimento(codigo);
        IList<Dominio.Relatorios.Embarcador.DataSource.SAC.OcorrenciasDocumento> dsOcorrencias =
            repCTeDocumentoOriginario.OcorrenciasConhecimento(codigo);

        if (dsComponentes.Count == 0)
        {
            dsComponentes.Add(new Dominio.Relatorios.Embarcador.DataSource.SAC.ComponentesDocumento
                { Codigo = 0, CodigoCTe = codigo });
            dsDocumentos[0].ContemComponentes = false;
        }
        else
            dsDocumentos[0].ContemComponentes = true;

        if (dsPesos.Count == 0)
        {
            dsPesos.Add(new Dominio.Relatorios.Embarcador.DataSource.SAC.PesoDocumento
                { Codigo = 0, CodigoCTe = codigo });
            dsDocumentos[0].ContemDocumentos = false;
        }
        else
            dsDocumentos[0].ContemDocumentos = true;

        if (dsNotas.Count == 0)
        {
            dsNotas.Add(new Dominio.Relatorios.Embarcador.DataSource.SAC.NotasFiscaisDocumento
                { Codigo = 0, CodigoCTe = codigo });
            dsDocumentos[0].ContemNotas = false;
        }
        else
            dsDocumentos[0].ContemNotas = true;

        if (dsFaturas.Count == 0)
        {
            dsFaturas.Add(new Dominio.Relatorios.Embarcador.DataSource.SAC.FaturamentoDocumento
                { Codigo = 0, CodigoCTe = codigo });
            dsDocumentos[0].ContemFaturas = false;
        }
        else
            dsDocumentos[0].ContemFaturas = true;

        if (dsOcorrencias.Count == 0)
        {
            dsOcorrencias.Add(new Dominio.Relatorios.Embarcador.DataSource.SAC.OcorrenciasDocumento
                { Codigo = 0, CodigoCTe = codigo });
            dsDocumentos[0].ContemOcorrencias = false;
        }
        else
            dsDocumentos[0].ContemOcorrencias = true;


        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "AtendimentoClienteComponentes.rpt",
                DataSet = dsComponentes
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds2 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "AtendimentoClientePesos.rpt",
                DataSet = dsPesos
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds3 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "AtendimentoClienteNotas.rpt",
                DataSet = dsNotas
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds4 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "AtendimentoClienteFaturas.rpt",
                DataSet = dsFaturas
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds5 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "AtendimentoClienteOcorrencias.rpt",
                DataSet = dsOcorrencias
            };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);
        subReports.Add(ds2);
        subReports.Add(ds3);
        subReports.Add(ds4);
        subReports.Add(ds5);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsDocumentos,
                SubReports = subReports
            };

        // Gera pdf
        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\SAC\AtendimentoCliente.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        // Retorna o arquivo
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}