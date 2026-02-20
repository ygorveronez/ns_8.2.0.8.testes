using System;
using System.Collections.Generic;
using System.Globalization;
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

[UseReportType(ReportType.DiarioBordoSemanal)]
public class DiarioBordoSemanalReport : ReportBase
{
    public DiarioBordoSemanalReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.RH.DiarioBordoSemanal repDiarioBordoSemanal =
            new Repositorio.Embarcador.RH.DiarioBordoSemanal(_unitOfWork);
        int codigo = extraData.GetValue<int>("codigo");
        // Busca informacoes
        Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal diarioBordo = repDiarioBordoSemanal.BuscarPorCodigo(codigo);

        // Valida
        if (diarioBordo == null)
            throw new ServicoException("Não foi possível encontrar o registro.");

        DateTime dataInicio = diarioBordo.DataInicio.Date;
        DateTime dataFim = diarioBordo.DataFim.Date;

        List<Dominio.Relatorios.Embarcador.DataSource.RH.DiarioBordoSemanal> dias =
            new List<Dominio.Relatorios.Embarcador.DataSource.RH.DiarioBordoSemanal>();
        Dominio.Relatorios.Embarcador.DataSource.RH.DiarioBordoSemanal diario =
            new Dominio.Relatorios.Embarcador.DataSource.RH.DiarioBordoSemanal()
            {
                Carga = diarioBordo.Carga?.CodigoCargaEmbarcador ?? "",
                DataFim = diarioBordo.DataFim.ToString("dd/MM/yyyy"),
                DataInicio = diarioBordo.DataInicio.ToString("dd/MM/yyyy"),
                Motorista = diarioBordo.Motorista?.Descricao ?? "",
                Numero = diarioBordo.Numero,
                Veiculo = diarioBordo.Veiculo?.Placa ?? ""
            };
        int count = 0;
        while (count <= 6)
        {
            if (count == 0)
            {
                diario.DataUmDiarioBordo = dataInicio.ToString("dd/MM/yyyy");
                diario.DiaUmSemana = dataInicio.ToString("dddd", new CultureInfo("pt-BR"));
            }
            else if (count == 1)
            {
                diario.DataDoisDiarioBordo = dataInicio.ToString("dd/MM/yyyy");
                diario.DiaDoisSemana = dataInicio.ToString("dddd", new CultureInfo("pt-BR"));
            }
            else if (count == 2)
            {
                diario.DataTresDiarioBordo = dataInicio.ToString("dd/MM/yyyy");
                diario.DiaTresSemana = dataInicio.ToString("dddd", new CultureInfo("pt-BR"));
            }
            else if (count == 3)
            {
                diario.DataQuatroDiarioBordo = dataInicio.ToString("dd/MM/yyyy");
                diario.DiaQuatroSemana = dataInicio.ToString("dddd", new CultureInfo("pt-BR"));
            }
            else if (count == 4)
            {
                diario.DataCincoDiarioBordo = dataInicio.ToString("dd/MM/yyyy");
                diario.DiaCincoSemana = dataInicio.ToString("dddd", new CultureInfo("pt-BR"));
            }
            else if (count == 5)
            {
                diario.DataSeisDiarioBordo = dataInicio.ToString("dd/MM/yyyy");
                diario.DiaSeisSemana = dataInicio.ToString("dddd", new CultureInfo("pt-BR"));
            }
            else if (count == 6)
            {
                diario.DataSeteDiarioBordo = dataInicio.ToString("dd/MM/yyyy");
                diario.DiaSeteSemana = dataInicio.ToString("dddd", new CultureInfo("pt-BR"));
            }

            count++;
            dataInicio = dataInicio.AddDays(1);
        }

        dias.Add(diario);
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dias
            };

        byte[] arquivo = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\RH\DiarioBordoSemanal.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return PrepareReportResult(FileType.PDF, arquivo);
    }
}