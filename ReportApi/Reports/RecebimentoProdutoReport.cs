using System.Collections.Generic;
using System.IO;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.RecebimentoProduto)]
public class RecebimentoProdutoReport : ReportBase
{
    public RecebimentoProdutoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);

        int codigoCargaEntrega = extraData.GetValue<int>("CodigoCargaEntrega");

        var cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel
            repCargaEntregaAssinaturaResponsavel =
                new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repositorioCargaEntregaFoto =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel cargaEntregaAssinatura =
            repCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntrega(codigoCargaEntrega);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos =
            repositorioCargaEntregaProduto.BuscarPorCargaEntrega(codigoCargaEntrega);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto> listFotosCargaEntrega =
            repositorioCargaEntregaFoto.BuscarPorCargaEntrega(codigoCargaEntrega);
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RecibimentoProduto> listaProduto =
            new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RecibimentoProduto>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RecibimentoProdutoFoto>
            listaFotosRecibimentoProduto =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RecibimentoProdutoFoto>();
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();

        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto in
                 cargaEntregaProdutos)
            listaProduto.Add(new Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RecibimentoProduto()
            {
                Produto = cargaEntregaProduto.Produto.CodigoProdutoEmbarcador ?? string.Empty,
                NumeroNf = cargaEntregaProduto.XMLNotaFiscal?.Numero ?? 0,
                QtePecas = cargaEntregaProduto.QuantidadeConferencia,
                Condicao = cargaEntregaProduto.ObservacaoProdutoDevolucao ?? string.Empty,
                QteTotal = cargaEntregaProduto.Quantidade,
                Divergencia = string.IsNullOrEmpty(cargaEntregaProduto.ObservacaoProdutoDevolucao)
                    ? "Não Houve"
                    : "Houve"
            });

        Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RecebimentoProdutoDados
            recebimentoProdutoDados =
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RecebimentoProdutoDados()
                {
                    Data = cargaEntrega?.DataConfirmacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                    Transportadora = cargaEntrega?.Carga?.Empresa?.RazaoSocial ?? string.Empty
                };

        //Assinatura
        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro assinatura =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        string caminhoAssinatura = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "Assinatura" });
        assinatura.NomeParametro = "Assinatura";
        assinatura.ValorParametro = cargaEntregaAssinatura != null
            ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoAssinatura,
                cargaEntregaAssinatura.GuidArquivo + "-miniatura" +
                Path.GetExtension(cargaEntregaAssinatura.NomeArquivo))
            : string.Empty;
        parametros.Add(assinatura);

        //Fotos
        string caminhoFotos = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto fotoCargaEntrega in
                 listFotosCargaEntrega)
            listaFotosRecibimentoProduto.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RecibimentoProdutoFoto()
                {
                    PossuiFoto = fotoCargaEntrega != null ? true : false,
                    CaminhoFoto = fotoCargaEntrega != null
                        ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoFotos,
                            fotoCargaEntrega.GuidArquivo + "-miniatura" +
                            Path.GetExtension(fotoCargaEntrega.NomeArquivo))
                        : string.Empty
                });

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "RecebimentoProduto",
                DataSet = listaProduto
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds2 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "RecebimentoProdutoFoto",
                DataSet = listaFotosRecibimentoProduto
            };

        subReports.Add(ds1);
        subReports.Add(ds2);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet =
                    new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RecebimentoProdutoDados>()
                        { recebimentoProdutoDados },
                SubReports = subReports,
                Parameters = parametros
            };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\RecebimentoProduto.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}