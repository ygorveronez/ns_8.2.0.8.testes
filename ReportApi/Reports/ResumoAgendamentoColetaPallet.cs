using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System.Collections.Generic;

namespace ReportApi.Reports;

[UseReportType(ReportType.ResumoAgendamentoColetaPallet)]
public class ResumoAgendamentoColetaPalletReport : ReportBase
{
    public ResumoAgendamentoColetaPalletReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet repAgendamentoColeta = new Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet(_unitOfWork);

        Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet agendamento = repAgendamentoColeta.BuscarPorCodigo(extraData.GetValue<int>("CodigoAgendamento"), false);

        Dominio.Relatorios.Embarcador.DataSource.GestaoPallet.ResumoAgendamentoColetaPallet DSresumo =
            new Dominio.Relatorios.Embarcador.DataSource.GestaoPallet.ResumoAgendamentoColetaPallet
            {
                Carga = agendamento.Carga.Descricao,
                Cliente = agendamento.Cliente.Descricao,
                DataOrdem = agendamento.DataOrdem,
                Filial = agendamento.Carga.Filial?.Descricao ?? string.Empty,
                Motorista = agendamento.Motorista.Descricao,
                NumeroOrdem = agendamento.NumeroOrdem,
                Placa = agendamento.Veiculo.Descricao,
                QuantidadePallet = agendamento.QuantidadePallets,
                Solicitante = agendamento.Usuario.Descricao,
                Transportador = agendamento.Transportador.Descricao
            };


        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPallet.ResumoAgendamentoColetaPallet>()
                    { DSresumo },
            };

        Dominio.Enumeradores.TipoArquivoRelatorio tipo = Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
        string arquivoRPT = @"Areas\Relatorios\Reports\Default\GestaoPallet\ResumoAgendamentoColetaPallet.rpt";

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(arquivoRPT, tipo, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}