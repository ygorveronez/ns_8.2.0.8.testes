using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

[UseReportType(ReportType.CapaViagem)]
public class CapaViagemReport : ReportBase
{
    public CapaViagemReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoFluxoPatio = extraData.GetValue<int>("codigoFluxoPatio");
        Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio =
            new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
        Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio =
            repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoPatio, false);

        if (fluxoGestaoPatio == null)
            throw new ServicoException("Carga não encontrada.");

        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
        Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal =
            new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
        Repositorio.Embarcador.Cargas.FaixaTemperatura repositorioFaixaTemperatura =
            new Repositorio.Embarcador.Cargas.FaixaTemperatura(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento =
            new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento =
            new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
        Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio =
            new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.Carga carga = fluxoGestaoPatio.Carga;
        Dominio.Entidades.Cliente remetente = carga.Pedidos.FirstOrDefault()?.Pedido?.Remetente;

        List<Dominio.ObjetosDeValor.WebService.Carga.Carga> cargasAgrupadas =
            repositorioCarga.BuscarNumeroCargasAgrupadas(carga.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas =
            repositorioCargaEntrega.BuscarPorCarga(carga.Codigo);
        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais =
            repositorioNotaFiscal.BuscarPorCarga(carga.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> faixasTemperatura = remetente != null
            ? repositorioFaixaTemperatura.BuscarPorRemetente(remetente.CPF_CNPJ)
            : new List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento =
            repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento =
            repositorioCargaJanelaDescarregamento.BuscarPorCarga(carga.Codigo);
        Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio =
            repositorioConfiguracaoGestaoPatio.BuscarConfiguracao();

        Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura menorFaixaTemperatura =
            faixasTemperatura.OrderBy(obj => obj.FaixaInicial).FirstOrDefault();

        List<DateTime> datasEntregasOrdenacaoCrescente = entregas.Where(obj => obj.DataPrevista.HasValue)
            .Select(obj => obj.DataPrevista.Value).OrderBy(data => data).ToList();

        cargasAgrupadas.Add(new Dominio.ObjetosDeValor.WebService.Carga.Carga()
        {
            NumeroCarga = carga.CodigoCargaEmbarcador
        });

        string numerosCarga = string.Join(", ", cargasAgrupadas.Select(obj => obj.NumeroCarga));

        Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CapaViagem dataSource =
            new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CapaViagem()
            {
                Viagem = numerosCarga,
                DataPrimeiraEntrega = datasEntregasOrdenacaoCrescente.FirstOrDefault().ToString("dd/MM/yyyy HH:mm"),
                DataHoraEntrada = carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm"),
                DataLiberacaoChave = fluxoGestaoPatio?.DataLiberacaoChave?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataViagem = carga.DataInicioViagem?.ToString("dd/MM/yyyy"),
                Observacao = string.Join(", ", notasFiscais.Select(obj => obj.Numero)),
                Roteiro = carga.Rota?.Descricao,
                Motorista = carga.DadosSumarizados?.Motoristas,
                PlacaVeiculo = carga.RetornarPlacas,
                Transportadora = carga.Empresa?.Descricao,
                Temperatura = menorFaixaTemperatura != null
                    ? $"{menorFaixaTemperatura.FaixaInicial} até {menorFaixaTemperatura.FaixaFinal}"
                    : string.Empty,
                ValorTotalNFs = notasFiscais.Sum(nf => nf.Valor).ToString("n2"),
                RoteiroDestino = $"{carga.DadosSumarizados?.Recebedores}",
                TotalPesoBruto = carga.DadosSumarizados?.PesoTotal.ToString("n2"),
                TotalPesoLiquido = carga.DadosSumarizados?.PesoLiquidoTotal.ToString("n2"),
                Descricao = cargaJanelaCarregamento?.ObservacaoFluxoPatio ??
                            cargaJanelaDescarregamento?.ObservacaoFluxoPatio ?? string.Empty,
                Mensagem = configuracaoGestaoPatio.FaturamentoMensagemCapaViagem
            };

        List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CapaViagemEntrega> dataSetSubReport =
            new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CapaViagemEntrega>();

        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega in entregas)
        {
            dataSetSubReport.Add(new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CapaViagemEntrega()
            {
                DataEntrega = entrega.DataPrevista?.ToString("dd/MM/yyyy HH:mm")
            });
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSetDados =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "CapaViagemEntrega.rpt",
                DataSet = dataSetSubReport
            };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>() { dataSetDados };
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
        Repositorio.Embarcador.GestaoPatio.TravamentoChave repTravamentoChave =
            new Repositorio.Embarcador.GestaoPatio.TravamentoChave(_unitOfWork);
        Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave =
            repTravamentoChave.BuscarTravamentoChaveMotoristaPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);
        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro assinaturaMotorista =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();

        string caminhoAssinaturaMotorista = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "GestaoPatio", "TravamentoChave", "AssinaturaMotorista" });
        assinaturaMotorista.NomeParametro = "CaminhoAssinaturaMotorista";
        assinaturaMotorista.ValorParametro = travamentoChave.TravamentoChaveAssinaturaMotorista != null
            ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoAssinaturaMotorista,
                travamentoChave.TravamentoChaveAssinaturaMotorista.GuidArquivo + "-miniatura" +
                Path.GetExtension(travamentoChave.TravamentoChaveAssinaturaMotorista.NomeArquivo))
            : string.Empty;
        parametros.Add(assinaturaMotorista);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CapaViagem>() { dataSource },
                Parameters = parametros,
                SubReports = subReports
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\GestaoPatio\CapaViagem.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}