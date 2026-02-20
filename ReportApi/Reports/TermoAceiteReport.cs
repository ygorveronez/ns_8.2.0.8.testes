using System;
using System.Collections.Generic;
using System.Linq;
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

[UseReportType(ReportType.TermoAceite)]
public class TermoAceiteReport : ReportBase
{
    public TermoAceiteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(_unitOfWork);
        
        Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(extraData.GetValue<int>("CodigoLote"));
        
        if (lote == null)
            throw new ServicoException("Não foi possível encontrar o registro.");
        
        string nomeEmpresa = "Danone";
        string numeroAvarias = String.Join(", ", (from o in lote.Avarias select o.NumeroAvaria).ToArray());

        List<Dominio.Relatorios.Embarcador.DataSource.Avarias.TermoAceite> dsInformacoes =
            new List<Dominio.Relatorios.Embarcador.DataSource.Avarias.TermoAceite>()
            {
                new Dominio.Relatorios.Embarcador.DataSource.Avarias.TermoAceite()
                {
                    NomeEmpresa = nomeEmpresa,
                    NumeroLote = lote.Numero,
                    ValorLote = lote.ValorLote,
                    ValorLoteExtenso = Utilidades.Conversor.DecimalToWords(lote.ValorLote),
                    Transportador = lote.Transportador?.RazaoSocial ?? string.Empty,
                    NumeroAvarias = numeroAvarias,
                    DataGeracao = lote.DataGeracao,
                }
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet() { DataSet = dsInformacoes };
        
        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Avarias\TermoAceite.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}