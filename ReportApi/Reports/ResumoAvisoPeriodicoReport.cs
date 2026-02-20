using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.ResumoAvisoPeriodico)]
public class ResumoAvisoPeriodicoReport : ReportBase
{
    public ResumoAvisoPeriodicoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao repAvisoPeriodicoQuitacao = new Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao(_unitOfWork);

        var avisoPeriodico = repAvisoPeriodicoQuitacao.BuscarPorCodigo(extraData.GetValue<int>("CodigoAvisoPeriodico"), false);
        
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = ObterDataSetResumo(avisoPeriodico);

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Financeiros\ResumoAvisoPeriodico.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.XLS, dataSet);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
    
    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetResumo(Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao avisoPeriodico)
        {
            var totalTransportadorVencido = avisoPeriodico?.TotalPendenciasVencidoTransportador ?? 0m;
            var totalTransportadorAVencer = avisoPeriodico?.TotalPendenciasAVencerTransportador ?? 0m;
            var totalDesbloqueadoVencido = avisoPeriodico?.TotalPendenciasVencidoDesbloqueado ?? 0m;
            var totalDesbloqueadoAVencer = avisoPeriodico?.TotalPendenciasVencidoDesbloqueado ?? 0m;
            var totalUnileverVencido = avisoPeriodico?.TotalPendenciasVencidoUnilever ?? 0m;
            var totalUnileverAVencer = avisoPeriodico?.TotalPendenciasAVencerUnilever ?? 0m;
            var totalBloqueioPODVencido = avisoPeriodico?.TotalPendenciasVencidoBloqueioPOD ?? 0m;
            var totalBloqueioPODAVencer = avisoPeriodico?.TotalPendenciasAVencerBloqueioPOD ?? 0m;

            var totalPendentes = totalTransportadorVencido + totalTransportadorAVencer + totalDesbloqueadoVencido + totalDesbloqueadoAVencer + totalUnileverVencido + totalUnileverAVencer + totalBloqueioPODVencido + totalBloqueioPODAVencer;

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoAvisoPeriodico>()
                {
                    new Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoAvisoPeriodico()
                    {
                        NumeroAviso = avisoPeriodico?.Codigo ?? 0,
                        DataInicial = avisoPeriodico?.DataInicial?.ToDateString() ?? string.Empty,
                        DataFinal = avisoPeriodico?.DataFinal?.ToDateString() ?? string.Empty,
                        NomeTransportador = avisoPeriodico?.Transportador.Descricao,
                        CreditoEmConta = avisoPeriodico?.TotalPagamentoEDescontosEmConta ?? 0m,
                        NotasCompensadasContraAdiantamento = avisoPeriodico?.TotalNotasCompensadasAdiantamento ?? 0m,
                        PagamentoEDescontoViaConfirming = avisoPeriodico?.TotalPagamentoEDescontosViaConfirming ?? 0m,
                        PagamentoEDescontoViaCreditoEmConta = avisoPeriodico?.TotalPagamentoEDescontosViaCreditoConta ?? 0m,
                        SaldoDeAdiantamentoEmAberto = avisoPeriodico?.TotalSaldoAdiantamentoEmAberto ?? 0m,
                        TotalDeAdiantamento = avisoPeriodico?.TotalAdiantamento ?? 0m,
                        TotalGeralDosPag = avisoPeriodico?.TotalGeralPagamentos ?? 0m,
                        TotalTransportadorVencido = avisoPeriodico?.TotalPendenciasVencidoTransportador ?? 0m,
                        TotalTransportadorAVencer = avisoPeriodico?.TotalPendenciasAVencerTransportador ?? 0m,
                        TotalDesbloqueadoVencido = avisoPeriodico?.TotalPendenciasVencidoDesbloqueado ?? 0m,
                        TotalDesbloqueadoAVencer = avisoPeriodico?.TotalPendenciasVencidoDesbloqueado ?? 0m,
                        TotalUnileverVencido = avisoPeriodico?.TotalPendenciasVencidoUnilever ?? 0m,
                        TotalUnileverAVencer = avisoPeriodico?.TotalPendenciasAVencerUnilever ?? 0m,
                        TotalBloqueioPODVencido = avisoPeriodico?.TotalPendenciasVencidoBloqueioPOD ?? 0m,
                        TotalBloqueioPODAVencer = avisoPeriodico?.TotalPendenciasAVencerBloqueioPOD ?? 0m,
                        TotalPendentes = totalPendentes,
                        AvariasEmAberto = avisoPeriodico.TotalAvariasEmAberto,
                        DebitosBaixaResultado = avisoPeriodico.TotalDebitoBaixa,
                        ProjecaoRecebimento = avisoPeriodico.TotalProjecoesRecebimento,
                    }
                },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ResumoAvisoPeriodicoFiliais.rpt",
                        DataSet = ObterDataSetsResumoFiliais(avisoPeriodico)
                    }
                }
            };

            return dataSet;
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoAvisoPeriodicoFiliais> ObterDataSetsResumoFiliais(Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao avisoPeriodico)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoAvisoPeriodicoFiliais> dataSetFiliais = new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoAvisoPeriodicoFiliais>();

            if (avisoPeriodico?.Transportador.Filiais != null && avisoPeriodico?.Transportador.Filiais.Count > 0)
            {
                foreach (var filial in avisoPeriodico.Transportador.Filiais)
                {
                    dataSetFiliais.Add(new Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoAvisoPeriodicoFiliais
                    {
                        CodigoIntegracao = filial?.CodigoIntegracao ?? string.Empty,
                        CNPJ = filial.CNPJ_Formatado,
                        Cidade = filial.Localidade.Descricao ?? string.Empty,
                        UF = filial.LocalidadeUF ?? string.Empty
                    });
                }
            }

            return dataSetFiliais;
        }
}