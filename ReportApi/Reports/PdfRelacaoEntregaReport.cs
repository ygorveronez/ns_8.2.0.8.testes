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

[UseReportType(ReportType.PdfRelacaoEntrega)]
public class PdfRelacaoEntregaReport : ReportBase
{
    public PdfRelacaoEntregaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("CodigoCarga");
        string nomeCliente = extraData.GetValue<string>("NomeCliente");

        Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe =
            new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(_unitOfWork);
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Embarcador.Logistica.RotaFreteCEP repBuscarPorCEP =
            new Repositorio.Embarcador.Logistica.RotaFreteCEP(_unitOfWork);

        IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista>
            dadosRelacaoEntregaMotorista =
                repCargaPedidoDocumentoCTe.RelatorioRelacaoEntregaMotoristaCarga(codigoCarga);
        IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque>
            dadosRelacaoEntregaReboque = repCargaPedidoDocumentoCTe.RelatorioRelacaoEntregaReboqueCarga(codigoCarga);
        IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega> dadosRelacaoEntrega =
            repCargaPedidoDocumentoCTe.RelatorioRelacaoEntregaCarga(codigoCarga);
        IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento>
            dadosRelacaoEntregaDocumento =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento>();

        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

        if (carga.Empresa?.EmpresaPropria ?? false)
            dadosRelacaoEntregaDocumento =
                repCargaPedidoDocumentoCTe.RelatorioRelacaoEntregaDocumentoPedidoCarga(codigoCarga);
        else
            dadosRelacaoEntregaDocumento =
                repCargaPedidoDocumentoCTe.RelatorioRelacaoEntregaDocumentoCarga(codigoCarga);

        string rotasDestinos = string.Empty;
        string rotaDest = string.Empty;
        int cepDest = 0;
        List<int> codigosRotas = new List<int>();
        Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP rotaFreteCEP = null;
        for (int k = 0; k < dadosRelacaoEntregaDocumento.Count; k++)
        {
            int.TryParse(Utilidades.String.OnlyNumbers(dadosRelacaoEntregaDocumento[k].CEPDestinatario), out cepDest);
            if (cepDest > 0)
            {
                rotaFreteCEP = repBuscarPorCEP.BuscarPorCEP(cepDest);
                if (rotaFreteCEP != null && !codigosRotas.Contains(rotaFreteCEP.RotaFrete.Codigo))
                {
                    dadosRelacaoEntregaDocumento[k].EmpresaFilial = rotaFreteCEP.RotaFrete.FilialDistribuidora;
                    rotaDest = rotaFreteCEP.RotaFrete.Descricao;
                    codigosRotas.Add(rotaFreteCEP.RotaFrete.Codigo);
                    if (!string.IsNullOrWhiteSpace(rotasDestinos))
                        rotasDestinos = ", " + rotaDest;
                    else
                        rotasDestinos = rotaDest;
                }
            }

            cepDest = 0;
            rotaFreteCEP = null;
        }

        if (dadosRelacaoEntrega.Count > 0)
            dadosRelacaoEntrega[0].Rota = rotasDestinos;

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeCliente;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "RelacaoEntregaDocumentoCarga.rpt",
                DataSet = dadosRelacaoEntregaDocumento
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds2 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "RelacaoEntregaMotoristaCarga.rpt",
                DataSet = dadosRelacaoEntregaMotorista
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds3 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "RelacaoEntregaReboqueCarga.rpt",
                DataSet = dadosRelacaoEntregaReboque
            };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);
        subReports.Add(ds2);
        subReports.Add(ds3);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosRelacaoEntrega,
                Parameters = parametros,
                SubReports = subReports
            };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Pedidos\RelacaoEntregaCarga.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}