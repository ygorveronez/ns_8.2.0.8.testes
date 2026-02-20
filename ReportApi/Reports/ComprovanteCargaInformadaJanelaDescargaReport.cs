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

[UseReportType(ReportType.ComprovanteCargaInformadaJanelaDescarga)]
public class ComprovanteCargaInformadaJanelaDescargaReport : ReportBase
{
    public ComprovanteCargaInformadaJanelaDescargaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigo = extraData.GetValue<int>("codigo");
        bool descarga = extraData.GetValue<bool>("descarga");
        string senhaAgendamento = extraData.GetValue<string>("senhaAgendamento");
        int codigoJanelaDescarregamento = extraData.GetValue<int>("codigoJanelaDescarregamento");

        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido =
            new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido =
            repositorioCargaPedido.BuscarPorCarga(codigo);
        Dominio.Entidades.Embarcador.Cargas.Carga carga = listaCargaPedido.FirstOrDefault()?.Carga;
        Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento =
            new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento =
            repositorioCargaJanelaDescarregamento.BuscarPorCodigo(codigoJanelaDescarregamento, auditavel: false);

        if (carga == null)
            throw new ServicoException(Localization.Resources.GestaoPatio.FluxoPatio.CargaNaoEncontrada);

        string numeroDoca = string.Empty;
        string rota = carga.CodigoCargaEmbarcador;

        if (!string.IsNullOrWhiteSpace(carga.NumeroDoca) && !string.IsNullOrWhiteSpace(carga.NumeroDocaEncosta) &&
            carga.NumeroDoca != carga.NumeroDocaEncosta)
            numeroDoca = string.Concat(carga.NumeroDoca, " / ", carga.NumeroDocaEncosta);
        else if (!string.IsNullOrWhiteSpace(carga.NumeroDoca) && !string.IsNullOrWhiteSpace(carga.NumeroDocaEncosta) &&
                 carga.NumeroDoca == carga.NumeroDocaEncosta)
            numeroDoca = carga.NumeroDoca;
        else if (string.IsNullOrWhiteSpace(carga.NumeroDoca) && !string.IsNullOrWhiteSpace(carga.NumeroDocaEncosta))
            numeroDoca = carga.NumeroDocaEncosta;
        else
            numeroDoca = carga.NumeroDoca;

        if (carga.CargaAgrupada)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupamento =
                repositorioCarga.BuscarCargasOriginais(carga.Codigo);

            rota += $", {string.Join(", ", (from o in cargasAgrupamento select o.CodigoCargaEmbarcador))}";
        }

        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();
        Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupo =
            (from o in cargaPedido?.Pedido.Produtos
             where o?.Produto?.GrupoProduto != null
             select o.Produto.GrupoProduto).FirstOrDefault();

        Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteCargaInformada
            dataSetComprovanteCargaInformada =
                new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteCargaInformada()
                {
                    Cavalo = carga.Veiculo?.Placa,
                    CodigoConsincoMotorista = carga.Motoristas.FirstOrDefault()?.CodigoIntegracao ?? "",
                    CodigoConsincoTransportador = carga.Motoristas.FirstOrDefault()?.Empresa?.CodigoIntegracao ?? "",
                    CodigoIntegracaoFilial = descarga ? senhaAgendamento : carga.Filial?.CodigoFilialEmbarcador,
                    Contato = carga.Motoristas.FirstOrDefault()?.Telefone ?? "",
                    Data = cargaJanelaDescarregamento?.InicioDescarregamento.ToString("dd/MM/yyyy"),
                    Hora = cargaJanelaDescarregamento?.InicioDescarregamento.ToString("HH:mm:ss"),
                    Doca = numeroDoca,
                    Lacre = carga.Lacres.FirstOrDefault()?.Numero,
                    NomeMotorista = carga.Motoristas.FirstOrDefault()?.Nome ?? "",
                    NomeTransportador = carga.Motoristas.FirstOrDefault()?.Empresa?.Descricao ?? "",
                    Placa = carga.VeiculosVinculados != null
                        ? String.Join(", ", (from o in carga.VeiculosVinculados select o.Placa))
                        : "",
                    Rota = rota,
                    Tipo = carga.Veiculo?.ModeloVeicularCarga?.Descricao,
                    SubTitulo = descarga ? string.Empty : "CENTRAL DE DISTRIBUIÇÃO",
                    Titulo = descarga ? "Senha para Descarregamento" : "Senha para Carregamento",
                    RGMotorista = carga.Motoristas.FirstOrDefault()?.RG ?? string.Empty,
                    GrupoProduto = grupo != null ? grupo.Descricao : string.Empty,
                    Fornecedor = carga.Pedidos.FirstOrDefault().Pedido.Remetente.Nome,
                    DataConfirmacaoChegada = cargaJanelaDescarregamento?.DataConfirmacao?.ToString("dd/MM/yyyy") ??
                                             string.Empty,
                    HoraConfirmacaoChegada =
                        cargaJanelaDescarregamento?.DataConfirmacao?.ToString("HH:mm:ss") ?? string.Empty,
                };

        List<Dominio.Entidades.Cliente> lojas =
            (from o in listaCargaPedido where o.Pedido.Destinatario != null select o.Pedido.Destinatario).Distinct()
            .ToList();

        if (lojas.Count >= 1)
        {
            Dominio.Entidades.Cliente loja = lojas[0];

            if (loja != null)
            {
                dataSetComprovanteCargaInformada.LojaUmCodigoSendas = loja.CodigoCompanhia;
                dataSetComprovanteCargaInformada.LojaUmNumero = loja.CodigoIntegracao;

                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga =
                    loja.ClienteDescargas?.FirstOrDefault();

                if (clienteDescarga != null)
                {
                    dataSetComprovanteCargaInformada.LojaUmHoraInicioDescarga = clienteDescarga.HoraInicioDescarga;
                    dataSetComprovanteCargaInformada.LojaUmHoraLimiteDescarga = clienteDescarga.HoraLimiteDescarga;
                }
            }
        }

        string caminhoLogo =
            Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"],
                "crystal.png");
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoImagem", caminhoLogo, true)
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteCargaInformada>()
                    { dataSetComprovanteCargaInformada },
            };


        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Logistica\ComprovanteCargaInformadaJanelaDescarga.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, possuiLogo: true);

        if (pdfContent == null)
            throw new ServicoException(Localization.Resources.Gerais.Geral.NaoFoiPossivelGerarDocumento);

        return PrepareReportResult(FileType.PDF, pdfContent);

    }
}