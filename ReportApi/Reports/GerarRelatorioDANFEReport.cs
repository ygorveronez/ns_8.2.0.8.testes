using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;
using Zen.Barcode;

namespace ReportApi.Reports;

[UseReportType(ReportType.GerarRelatorioDANFE)]
public class GerarRelatorioDANFEReport : ReportBase
{
    public GerarRelatorioDANFEReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(_unitOfWork);
        Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal = repNotaFiscal.BuscarPorCodigo(extraData.GetValue<int>("notaFiscal"));

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = extraData.GetValue<string>("propriedades").FromJson<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento>>();

        int codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");

        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio relatorioTemp = extraData.GetValue<string>("relatorioTemp").FromJson<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>();

        Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela repNotaFiscalParcela = new Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela(_unitOfWork);

        IList<Dominio.Relatorios.Embarcador.DataSource.NFe.DANFE> listaNotaFiscal = repNotaFiscal.BuscarDANFE(notaFiscal, propriedades, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, true);
        IList<Dominio.Relatorios.Embarcador.DataSource.NFe.DANFEParcela> listaParcelasNotaFiscal = repNotaFiscalParcela.BuscarParcelasDANFE(notaFiscal, propriedades, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, true);

        relatorioTemp.Colunas = null;
        CrystalDecisions.CrystalReports.Engine.ReportDocument report = _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaNotaFiscal, _unitOfWork, null,
            new List<KeyValuePair<string, dynamic>>()
            {
                new KeyValuePair<string, dynamic>("DANFE_Parcelas.rpt", listaParcelasNotaFiscal)
            }, false);
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Chave", !string.IsNullOrWhiteSpace(notaFiscal.Chave) ? notaFiscal.Chave : "", false));
        if (notaFiscal.Status == Dominio.Enumeradores.StatusNFe.Autorizado)
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "4", false));
        else if (notaFiscal.Status == Dominio.Enumeradores.StatusNFe.Cancelado)
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "3", false));
        else
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "1", false));
        if (listaParcelasNotaFiscal.Count() > 0)
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContemParcela", "S", false));
        else
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContemParcela", "N", false));

        Dominio.Entidades.Empresa empresa = BuscarEmpresa(notaFiscal.Empresa.Codigo);
        string caminhoLogo = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"], empresa.CNPJ + ".png");

        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoLogo))
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoLogoEmpresa", caminhoLogo, false));
        else
        {
            caminhoLogo = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"], "LogoBranca.png");
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoLogoEmpresa", caminhoLogo, false));
        }

        if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
        {
            byte[] codigoBarras = Utilidades.Barcode.Gerar(notaFiscal.Chave, ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Bmp);
            string caminhoCodigoBarras = @"C:\\XML NF-e\\" + notaFiscal.Chave + ".png";

            using (var fs = new System.IO.BinaryWriter(Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoCodigoBarras)))
                fs.Write(codigoBarras);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoImagemCodigoBarras", caminhoCodigoBarras, false));
        }
        else
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoImagemCodigoBarras", "SEM CAMINHO", false));

        _servicoRelatorioReportService.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "NotasFiscais/NotaFiscalEletronica", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }
}