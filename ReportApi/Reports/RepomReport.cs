using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System.Collections.Generic;
using System.Linq;

namespace ReportApi.Reports;

[UseReportType(ReportType.Repom)]
public class RepomReport : ReportBase
{
    public RepomReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCargaValePedagio = extraData.GetValue<int>("codigoCargaValePedagio");

        Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaIntegracaoValePedagio.BuscarPorCodigo(codigoCargaValePedagio);
        Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca repositorioDadosCompraPraca = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca(_unitOfWork);
        List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca> pracas = repositorioDadosCompraPraca.BuscarPorCarga(cargaValePedagio.Carga.Codigo);

        string caminhoLogo = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Integracao", "LogoRepom");

        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca primeiraPraca = pracas.FirstOrDefault();
        List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoRecibo> listaInformacoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoRecibo>()
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoRecibo()
                {
                    Data = primeiraPraca?.CargaValePedagioDadosCompra.DataEmissao.ToString("dd/MM/yyyy HH:mm") ??
                           string.Empty,
                    Cliente = primeiraPraca?.CargaValePedagioDadosCompra.CodigoFilialCliente,
                    Viagem = primeiraPraca?.CargaValePedagioDadosCompra?.CodigoViagem ?? 0,
                    Motorista = primeiraPraca?.CargaValePedagioDadosCompra.Carga.DadosSumarizados?.Motoristas ??
                                string.Empty,
                    CPF = primeiraPraca?.CargaValePedagioDadosCompra.Carga.Motoristas.FirstOrDefault().CPF_Formatado ??
                          string.Empty,
                    CNH = primeiraPraca?.CargaValePedagioDadosCompra.Carga.Motoristas.FirstOrDefault().CNPJEmbarcador ??
                          string.Empty,
                    Transportador = primeiraPraca?.CargaValePedagioDadosCompra.Carga.Empresa?.Descricao ?? string.Empty,
                    PlacaVeiculo = primeiraPraca?.CargaValePedagioDadosCompra.Carga.Veiculo?.Placa_Formatada ??
                                   string.Empty,
                    PlacaCarreta =
                        primeiraPraca?.CargaValePedagioDadosCompra.Carga.VeiculosVinculados?.FirstOrDefault()
                            ?.Placa_Formatada ?? string.Empty,
                    TotalEixos = primeiraPraca?.CargaValePedagioDadosCompra.Carga.ModeloVeicularCarga?.Eixos?.Count() ??
                                 0,
                    CTes = string.Join(", ",
                        primeiraPraca?.CargaValePedagioDadosCompra.Carga.CargaCTes?.Select(x => x.CTe?.Numero)),
                    Percurso =
                        $"{primeiraPraca?.CargaValePedagioDadosCompra.Carga.Percursos?.FirstOrDefault()?.Origem?.Descricao} até {cargaValePedagio.Carga.Percursos?.LastOrDefault()?.Destino?.Descricao}.",
                    Roteiro = primeiraPraca?.CargaValePedagioDadosCompra.Carga.Rota?.Descricao ?? string.Empty,
                    ValorTotalPracas = pracas.Select(x => x.Valor).Sum().ToString("n2")
                }
            };

        List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoReciboPraca> pracasDataSet = (from p in pracas
                                                                                                       select new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ImpressaoReciboPraca
                                                                                                       {
                                                                                                           Praca = p.NomePraca,
                                                                                                           DataEmissao = p.CargaValePedagioDadosCompra.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                                                                                           NumeroEixos = p.NumeroEixos,
                                                                                                           Valor = p.Valor.ToString("n2")
                                                                                                       }).ToList();

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
            @"Areas\Relatorios\Reports\Default\ValePedagio\Repom.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, false);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}