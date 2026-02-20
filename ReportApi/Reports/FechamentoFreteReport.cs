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

[UseReportType(ReportType.FechamentoFrete)]
public class FechamentoFreteReport : ReportBase
{
    public FechamentoFreteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(_unitOfWork);

        var fechamentofrete = repFechamentoFrete.BuscarPorCodigo(extraData.GetValue<int>("CodigoFechamentoFrete"));

        bool tipoPDF = extraData.GetValue<bool>("TipoPDF");

        Repositorio.Embarcador.Fechamento.FechamentoFreteOcorrencia repositorioFechamentoFreteOcorrencia =
            new Repositorio.Embarcador.Fechamento.FechamentoFreteOcorrencia(_unitOfWork);
        List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias =
            repositorioFechamentoFreteOcorrencia.BuscarPorFechamento(fechamentofrete.Codigo);
        Dominio.Relatorios.Embarcador.DataSource.Fechamento.Fechamento dataSetFechamento =
            repFechamentoFrete.ConsultarRelatorio(fechamentofrete.Codigo);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoComplemento> listaFechamentoComplemento =
            repFechamentoFrete.ConsultarRelatorioComplemento(fechamentofrete.Codigo);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoDestinatario> listaFechamentoDestinatario =
            repFechamentoFrete.ConsultarRelatorioDestinatario(fechamentofrete.Codigo);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoDestinatarioDetalhado>
            listaFechamentoDestinatarioDetalhado = repFechamentoFrete.ConsultarRelatorioDestinatarioDetalhado(fechamentofrete.Codigo, fechamentofrete.ObterComponenteFreteValorContrato() != null);

        List<Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoOcorrencia> listaFechamentoOcorrencias = (
            from ocorrencia in ocorrencias
            select new Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoOcorrencia()
            {
                Data = ocorrencia.DescricaoDataOcorrencia,
                Numero = ocorrencia.NumeroOcorrencia,
                Tipo = ocorrencia.TipoOcorrencia.Descricao,
                Valor = ocorrencia.ValorOcorrencia
            }
        ).ToList();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSetComplementos =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "Complemento",
                DataSet = listaFechamentoComplemento
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSetDestinatarios =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "Destinatario",
                DataSet = listaFechamentoDestinatario
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSetDestinatariosDetalhado =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "DestinatarioDetalhado",
                DataSet = listaFechamentoDestinatarioDetalhado
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSetOcorrencias =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "Ocorrencia",
                DataSet = listaFechamentoOcorrencias
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Fechamento.Fechamento>()
                    { dataSetFechamento },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    dataSetDestinatarios, dataSetDestinatariosDetalhado, dataSetOcorrencias, dataSetComplementos
                }
            };

        Dominio.Enumeradores.TipoArquivoRelatorio tipo = tipoPDF
            ? Dominio.Enumeradores.TipoArquivoRelatorio.PDF
            : Dominio.Enumeradores.TipoArquivoRelatorio.XLS;

        byte[] arquivo = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Fechamento\FechamentoFrete.rpt", tipo, dataSet, false);

        return PrepareReportResult(tipoPDF ? FileType.PDF : FileType.EXCEL, arquivo);
    }
}