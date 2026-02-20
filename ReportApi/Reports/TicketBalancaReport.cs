using System;
using System.Collections.Generic;
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

[UseReportType(ReportType.TicketBalanca)]
public class TicketBalancaReport : ReportBase
{
    public TicketBalancaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoGuarita = extraData.GetValue<int>("codigoGuarita");
        int codigoFluxoGestaoPatio = extraData.GetValue<int>("codigoFluxoGestaoPatio");
        Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita =
            new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = null;

        if (codigoGuarita > 0)
            cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);
        else if (codigoFluxoGestaoPatio > 0)
            cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

        if (cargaGuarita == null)
            throw new ServicoException("Carga não encontrada.");

        if (cargaGuarita.FluxoGestaoPatio == null)
            throw new ServicoException("Carga não encontrada.");

        Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa =
            new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
        Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio =
            servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(cargaGuarita.FluxoGestaoPatio);

        Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.TicketBalanca dsTicketBalanca =
            new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.TicketBalanca()
            {
                Remetente = cargaGuarita.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.Remetente?.Nome ?? string.Empty,
                CNPJRemetente = cargaGuarita.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.Remetente?.CPF_CNPJ_Formatado ??
                                string.Empty,
                InscricaoEstadual = cargaGuarita.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.Remetente?.IE_RG ??
                                    string.Empty,
                Endereco = cargaGuarita.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.Remetente?.EnderecoCompleto ?? "",
                CEP = cargaGuarita.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.Remetente?.CEP ??
                      string.Empty ?? string.Empty,
                Telefone =
                    string.IsNullOrWhiteSpace(cargaGuarita?.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.Remetente
                        ?.Telefone1)
                        ? cargaGuarita?.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.Remetente?.Telefone2 ?? string.Empty
                        : cargaGuarita?.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.Remetente?.Telefone1,
                Email = cargaGuarita.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.Remetente?.Email ?? string.Empty,
                RazaoSocialTransportador = cargaGuarita.Carga?.Empresa?.RazaoSocial ?? string.Empty,
                TipoCarga = cargaGuarita.Carga?.TipoDeCarga?.Descricao ?? string.Empty,
                NomeMotorista = cargaGuarita.Carga.Motoristas?.FirstOrDefault()?.Nome ?? string.Empty,
                ModeloVeicular = cargaGuarita.Carga?.Veiculo?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                Emissor = this.BuscarUsuario(extraData.GetValue<int>("CodigoUsuario")).Nome,
                NumeroCarga = cargaGuarita.Carga?.CodigoCargaEmbarcador,
                CodigoFilial = sequenciaGestaoPatio?.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                TipoOperacao = cargaGuarita.Carga.TipoOperacao?.Descricao ?? string.Empty,
                CPFMotorista = cargaGuarita.Carga?.Motoristas?.FirstOrDefault()?.CPF_Formatado ?? string.Empty,
                PlacaCavalo = cargaGuarita.Carga?.Veiculo?.Placa_Formatada ?? string.Empty,
                PlacaCarreta =
                    cargaGuarita.Carga?.VeiculosVinculados?.FirstOrDefault()?.Placa_Formatada ?? string.Empty,
                PesoEntrada = string.Concat(cargaGuarita?.PesagemInicial.ToString("n2") ?? "0,00", " kg"),
                PesoSaida = string.Concat(cargaGuarita?.PesagemFinal.ToString("n2") ?? "0,00", " kg"),
                PesoLiquido =
                    string.Concat(
                        ((cargaGuarita?.PesagemFinal ?? 0) - (cargaGuarita?.PesagemInicial ?? 0)).ToString("n2") ??
                        "0,00", " kg"),
                DataGeracao = DateTime.Now.ToString("d"),
                HoraGeracao = DateTime.Now.ToString("t"),
                DataHoraEntrada = cargaGuarita?.DataEntregaGuarita?.ToString() ?? string.Empty,
                DataHoraSaida = cargaGuarita?.DataSaidaGuarita?.ToString() ?? string.Empty,
            };

        string caminhoLogo =
            Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"],
                "Comprovante" + ".png");
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoImagem", caminhoLogo, true)
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.TicketBalanca>()
                    { dsTicketBalanca },
                Parameters = parametros
            };

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\GestaoPatio\TicketBalanca.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        if (pdfContent == null)
            throw new ServicoException( "Não foi possível gerar o comprovante de saída.");

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}