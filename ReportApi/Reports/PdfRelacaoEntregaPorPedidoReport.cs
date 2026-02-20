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

[UseReportType(ReportType.PdfRelacaoEntregaPorPedido)]
public class PdfRelacaoEntregaPorPedidoReport : ReportBase
{
    public PdfRelacaoEntregaPorPedidoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("CodigoCarga");
        string nomeCliente = extraData.GetValue<string>("NomeCliente");
        
        Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe =
            new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(_unitOfWork);
        Repositorio.Embarcador.Logistica.RotaFreteCEP repBuscarPorCEP =
            new Repositorio.Embarcador.Logistica.RotaFreteCEP(_unitOfWork);

        IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaCargaPedido>
            dadosRelacaoEntrega = repCargaPedidoDocumentoCTe.RelatorioRelacaoEntregaCargaPedido(codigoCarga);

        string rotasDestinos = string.Empty;
        List<int> codigosRotas = new List<int>();
        for (int k = 0; k < dadosRelacaoEntrega.Count; k++)
        {
            int.TryParse(Utilidades.String.OnlyNumbers(dadosRelacaoEntrega[k].CEPDestinatario), out int cepDest);
            if (cepDest > 0)
            {
                Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP
                    rotaFreteCEP = repBuscarPorCEP.BuscarPorCEP(cepDest);
                if (rotaFreteCEP != null && !codigosRotas.Contains(rotaFreteCEP.RotaFrete.Codigo))
                {
                    dadosRelacaoEntrega[k].EmpresaFilial = rotaFreteCEP.RotaFrete.FilialDistribuidora;
                    string rotaDest = rotaFreteCEP.RotaFrete.Descricao;
                    codigosRotas.Add(rotaFreteCEP.RotaFrete.Codigo);
                    if (!string.IsNullOrWhiteSpace(rotasDestinos))
                        rotasDestinos = ", " + rotaDest;
                    else
                        rotasDestinos = rotaDest;
                }
            }
        }

        if (dadosRelacaoEntrega.Count > 0)
            dadosRelacaoEntrega[0].Rota = rotasDestinos;

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosRelacaoEntrega
            };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Pedidos\RelacaoEntregaCargaPedido.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdf);;
    }
}