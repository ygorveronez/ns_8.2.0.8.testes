using System.Collections.Generic;
using System.IO;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.GerarRelatorioCCeNFe)]
public class GerarRelatorioCCeNFeReport : ReportBase
{
    public GerarRelatorioCCeNFeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repositorioRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork);
        Repositorio.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao repositorioNotaFiscalCartaCorrecao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao(_unitOfWork);

        int codigoEmpresa = extraData.GetValue<int>("codigoEmpresa");
        Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao carta = repositorioNotaFiscalCartaCorrecao.BuscarPorCodigo(extraData.GetValue<int>("carta"));
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = extraData.GetValue<string>("propriedades").FromJson<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento>>();
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = repositorioRelatorioControleGeracao.BuscarPorCodigo(extraData.GetValue<int>("relatorioControleGeracao"));
        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = extraData.GetValue<string>("relatorioTemp").FromJson<Dominio.Entidades.Embarcador.Relatorios.Relatorio>();

        IList<Dominio.Relatorios.Embarcador.DataSource.NFe.CCeNFe> listaCCeNFe = repositorioNotaFiscalCartaCorrecao.BuscarCCeNFe(carta, propriedades, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, true);
        relatorioTemp.Colunas = null;
        CrystalDecisions.CrystalReports.Engine.ReportDocument report = _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaCCeNFe, _unitOfWork);
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Entidades.Empresa empresa = BuscarEmpresa(codigoEmpresa);
        string caminhoLogo = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"], empresa.CNPJ + ".png");
        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoLogo))
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoLogoEmpresa", caminhoLogo, false));
        else
        {
            caminhoLogo = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"], "LogoBranca.png");
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoLogoEmpresa", caminhoLogo, false));
        }

        _servicoRelatorioReportService.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "NotasFiscais/NotaFiscalEletronica", _unitOfWork);
        return PrepareReportResult(FileType.PDF);
    }
}