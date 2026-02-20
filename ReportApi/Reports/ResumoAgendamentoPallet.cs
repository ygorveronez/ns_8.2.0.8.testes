using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System.Collections.Generic;

namespace ReportApi.Reports;

[UseReportType(ReportType.ResumoAgendamentoPallet)]
public class ResumoAgendamentoPallet : ReportBase
{
    #region Construtores

    public ResumoAgendamentoPallet(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    #endregion Construtores

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoAgendamento = extraData.GetValue<int>("CodigoAgendamento");

        Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);

        Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamento = repositorioAgendamentoPallet.BuscarPorCodigo(codigoAgendamento, false);

        Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCarga(agendamento.Carga?.Codigo ?? 0);

        Dominio.Relatorios.Embarcador.DataSource.GestaoPallet.ResumoAgendamentoPallet DSresumo = new Dominio.Relatorios.Embarcador.DataSource.GestaoPallet.ResumoAgendamentoPallet
        {
            Situacao = cargaJanelaDescarregamento?.Situacao.ObterDescricao() ?? string.Empty,
            SenhaAgendamento = agendamento.Senha,
            DataAgendamento = cargaJanelaDescarregamento?.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
            Observacao = agendamento.Observacao,
            TipoDeCarga = agendamento.TipoCarga?.Descricao ?? string.Empty,
            Carga = agendamento.Carga.Descricao,
            Filial = agendamento.Carga.Filial?.Descricao ?? string.Empty,
            Motorista = agendamento.MotoristaSelecionado?.Descricao ?? string.Empty,
            Placa = agendamento.VeiculoSelecionado?.Descricao ?? string.Empty,
            QuantidadePallet = agendamento.QuantidadePallets,
            Solicitante = agendamento.Solicitante.Descricao,
            Transportador = agendamento.Transportador?.Descricao ?? string.Empty
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPallet.ResumoAgendamentoPallet>()
            {
                DSresumo
            }
        };

        Dominio.Enumeradores.TipoArquivoRelatorio tipo = Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
        string arquivoRPT = @"Areas\Relatorios\Reports\Default\GestaoPallet\ResumoAgendamentoPallet.rpt";

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(arquivoRPT, tipo, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}