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

[UseReportType(ReportType.ValeAvulso)]
public class ValeAvulsoReport : ReportBase
{
    public ValeAvulsoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Financeiro.ValeAvulso repValeAvulso = new Repositorio.Embarcador.Financeiro.ValeAvulso(_unitOfWork);

        Dominio.Entidades.Embarcador.Financeiro.ValeAvulso valeAvulso = repValeAvulso.BuscarPorCodigo(extraData.GetValue<int>("CoidgoValeAvulso"));

        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ValeAvulso> dsValeAvulso = new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ValeAvulso>()
        {
            new Dominio.Relatorios.Embarcador.DataSource.Financeiros.ValeAvulso()
            {
                NumeroVale = valeAvulso.Numero,
                EmpresaNome = valeAvulso.Empresa.RazaoSocial,
                EmpresaLocalidade = valeAvulso.Empresa.LocalidadeUF,
                Valor = valeAvulso.Valor,
                DataDia = valeAvulso.Data.ToString("dd"),
                DataMes = valeAvulso.Data.ToString("MMMM"),
                DataAno = valeAvulso.Data.ToString("yyyy"),
                PessoaNome = valeAvulso.Pessoa.Nome,
                PessoaCPFCNPJ = valeAvulso.Pessoa.CPF_CNPJ_Formatado,
                Correspondente = valeAvulso.Correspondente ?? "",
                TipoDocumento = valeAvulso.TipoDocumento
            }
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dsValeAvulso
        };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Financeiros\ValeAvulso.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}