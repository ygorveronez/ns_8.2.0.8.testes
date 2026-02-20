using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Repositorio.Embarcador.Cargas.ValePedagio;
using Servicos.Embarcador.Relatorios;
using System.Collections.Generic;
using System.Linq;

namespace ReportApi.Reports;

[UseReportType(ReportType.ValePedagioPagBem)]
public class ValePedagioPagBemReport : ReportBase
{
    public ValePedagioPagBemReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var info = extraData.GetInfo();
        var codigoCargaValePedagio = extraData.GetValue<int>("CodigoCargaValePedagio");
        CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new CargaIntegracaoValePedagio(_unitOfWork);
        var cargaValePedagio = repCargaIntegracaoValePedagio.BuscarPorCodigo(codigoCargaValePedagio);

        string caminhoLogo =
            Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos,
                "Integracao", "LogoPagBem");

        int eixosTotal = 0;
        if (info.TipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
        {
            eixosTotal = cargaValePedagio.Carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos.Value ?? 0;
            if (cargaValePedagio.Carga.VeiculosVinculados != null &&
                cargaValePedagio.Carga.VeiculosVinculados.Count > 0)
            {
                foreach (var veinculoVinculado in cargaValePedagio.Carga.VeiculosVinculados)
                    eixosTotal += veinculoVinculado?.ModeloVeicularCarga?.NumeroEixos.Value ?? 0;
            }
        }

        List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoRecibo> listaInformacoes =
            new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoRecibo>()
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoRecibo()
                {
                    Data = cargaValePedagio.DataIntegracao.ToString("dd/MM/yyyy") ?? string.Empty,
                    Cliente = cargaValePedagio.Carga.Empresa.Descricao,
                    Viagem = cargaValePedagio.Codigo,
                    Motorista = cargaValePedagio.Carga.DadosSumarizados?.Motoristas ?? string.Empty,
                    CPF = cargaValePedagio.Carga.Motoristas.FirstOrDefault().CPF_Formatado ?? string.Empty,
                    CNH = cargaValePedagio.Carga.Motoristas.FirstOrDefault().CNPJEmbarcador ?? string.Empty,
                    Transportador = cargaValePedagio.Carga.Empresa?.Descricao ?? string.Empty,
                    PlacaVeiculo = cargaValePedagio.Carga.Veiculo?.Placa_Formatada ?? string.Empty,
                    PlacaCarreta = cargaValePedagio.Carga.VeiculosVinculados?.FirstOrDefault()?.Placa_Formatada ??
                                   string.Empty,
                    TotalEixos = eixosTotal > 0
                        ? eixosTotal
                        : cargaValePedagio.Carga.ModeloVeicularCarga?.Eixos?.Count() ?? 0,
                    CTes = string.Join(", ", cargaValePedagio.Carga.CargaCTes?.Select(x => x.CTe?.Numero)),
                    Percurso =
                        $"{cargaValePedagio.Carga.Percursos?.FirstOrDefault()?.Origem?.Descricao} até {cargaValePedagio.Carga.Percursos?.LastOrDefault()?.Destino?.Descricao}.",
                    Roteiro = cargaValePedagio.Carga.Rota?.Descricao ?? string.Empty,
                    ValorTotalPracas = cargaValePedagio.ValorValePedagio.ToString("n2")
                }
            };

        List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoReciboPraca> pracasDataSet =
            new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoReciboPraca>();
        pracasDataSet.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoReciboPraca()
        {
            DataEmissao = cargaValePedagio.DataIntegracao.ToString("dd/MM/yyyy") ?? string.Empty,
            NumeroEixos = eixosTotal > 0 ? eixosTotal : cargaValePedagio.Carga.ModeloVeicularCarga?.Eixos?.Count() ?? 0,
            Praca = "",
            Valor = cargaValePedagio.ValorValePedagio.ToString("n2")
        });

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = listaInformacoes,
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoLogoValePedagio", caminhoLogo,
                        true)
                },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "RepomPracas",
                        DataSet = pracasDataSet
                    }
                }
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\ValePedagio\PagBem.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, false);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}