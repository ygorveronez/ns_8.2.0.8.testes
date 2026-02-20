using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.FalhaNumeracaoCTe)]
public class FalhaNumeracaoCTeReport : ReportBase
{
    public FalhaNumeracaoCTeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        DateTime dataInicial = extraData.GetValue<DateTime>("DataInicial");
        DateTime dataFinal = extraData.GetValue<DateTime>("DataFinal");

        Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);
        Repositorio.ConhecimentoDeTransporteEletronico repCTe =
            new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

        List<Dominio.Entidades.Empresa> empresas = repCTe.BuscarEmpresasEmissao(dataInicial, dataFinal);
        List<Dominio.Entidades.EmpresaSerie> series = repCTe.BuscarSeriesEmissao(dataInicial, dataFinal);
        Dominio.Entidades.ModeloDocumentoFiscal modeloCTe = repModeloDocumentoFiscal.BuscarPorModelo("57");

        List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FalhaNumeracaoCTe> numeracoes =
            new List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FalhaNumeracaoCTe>();

        for (int i = 0; i < empresas.Count(); i++)
        {
            List<Dominio.Entidades.EmpresaSerie> seriesEmpresa =
                series.Where(o => o.Empresa.Codigo == empresas[i].Codigo).Select(obj => obj).ToList();
            for (int a = 0; a < seriesEmpresa.Count(); a++)
            {
                string numeracaoEmpresaSerie = "Número inicial: " +
                                               repCTe.BuscarPrimeiraNumeracao(dataInicial, dataFinal,
                                                   seriesEmpresa[a].Codigo).ToString();
                numeracaoEmpresaSerie += " até " +
                                         repCTe.BuscarUltimaNumeracao(dataInicial, dataFinal, seriesEmpresa[a].Codigo)
                                             .ToString();
                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FalhaNumeracaoCTe> numeracoesAux =
                    repCTe.ConsultarRelatorioFalhaNumeracaoCTe(dataInicial, dataFinal, seriesEmpresa[a].Empresa.Codigo,
                        modeloCTe.Codigo, seriesEmpresa[a].Codigo, numeracaoEmpresaSerie);
                for (int l = 0; l < numeracoesAux.Count(); l++)
                {
                    Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FalhaNumeracaoCTe numeroFalha =
                        new Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FalhaNumeracaoCTe();
                    numeroFalha.CNPJEmpresa = numeracoesAux[l].CNPJEmpresa;
                    numeroFalha.NumeracaoInicialFinal = numeracoesAux[l].NumeracaoInicialFinal;
                    numeroFalha.NumeroCTe = numeracoesAux[l].NumeroCTe;
                    numeroFalha.RazaoEmpresa = numeracoesAux[l].RazaoEmpresa;
                    numeroFalha.SerieCTe = numeracoesAux[l].SerieCTe;

                    numeracoes.Add(numeroFalha);
                }
            }
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataset =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = numeracoes
            };

        byte[] relatorio = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\CTe\FalhaNumeracaoCTe.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataset);

        return PrepareReportResult(FileType.PDF, relatorio);
    }
}