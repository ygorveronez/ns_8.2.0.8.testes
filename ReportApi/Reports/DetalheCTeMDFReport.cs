using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System.Collections.Generic;
using System.Linq;

namespace ReportApi.Reports;

[UseReportType(ReportType.DetalheCTeMDF)]
public class DetalheCTeMDFReport : ReportBase
{
    public DetalheCTeMDFReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("CodigoCarga");

        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
        Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repositorioCargaIntegracaoValePedagio.BuscarPorUnicaCarga(codigoCarga);
        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotaFiscal = repNotaFiscal.BuscarPorCarga(codigoCarga);
        List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = repCargaLocaisPrestacao.BuscarPorCarga(codigoCarga);
        List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> cargaLocaisPrestacaoPassagens = repCargaLocaisPrestacaoPassagens.BuscarPorLocaisPrestacao((from obj in cargaLocaisPrestacao select obj.Codigo).ToList());

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.MDFe> listaMDFe = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.MDFe>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CTe> listaCte = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CTe>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.NotaFiscal> listaNFe = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.NotaFiscal>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Passagem> listaPassagem = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Passagem>();

        foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe mdf in carga.CargaMDFes)
        {
            if (mdf.MDFe?.Status != StatusMDFe.Encerrado && mdf.MDFe?.Status != StatusMDFe.Autorizado)
                continue;

            listaMDFe.Add(new Dominio.Relatorios.Embarcador.DataSource.Cargas.MDFe
            {
                NumeroMDFe = mdf?.MDFe?.Numero ?? 0,
                ChaveMDFe = mdf?.MDFe?.Chave ?? string.Empty,
                SerieMDFe = mdf?.MDFe?.Serie?.Numero ?? 0,
                RemessaMDFe = mdf?.MDFe?.NumeroPedido ?? string.Empty,
            });
        }

        listaNFe.AddRange(repNotaFiscal.BuscarInfosNotaFiscal(codigoCarga));
        listaCte.AddRange(repNotaFiscal.BuscarCTesRelatorioDetalheCTeMDFePorCarga(codigoCarga));

        foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens localPassagem in cargaLocaisPrestacaoPassagens)
        {
            listaPassagem.Add(new Dominio.Relatorios.Embarcador.DataSource.Cargas.Passagem
            {
                Posicao = localPassagem.Posicao,
                Sigla = localPassagem.EstadoDePassagem?.Nome ?? string.Empty,
            });
        }

        Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalheCTeMDF DetalheCTeMDFe = new Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalheCTeMDF();

        listaPassagem = listaPassagem.Distinct().ToList();
        decimal valorMercadoria = 0;
        string destino = string.Empty;

        if (listaNotaFiscal?.Count > 0)
        {
            if ((carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ClassificacaoNFeRemessaVenda ?? false) && (carga?.Pedidos?.Any(o => o.IndicadorRemessaVenda) ?? false))
            {
                if (carga?.TipoOperacao.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeRemessa ?? false)
                    valorMercadoria = listaNotaFiscal.Where(notaFiscal => notaFiscal.ClassificacaoNFe != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe.Remessa).Sum(notaFiscal => notaFiscal.Valor);
                else if (carga?.TipoOperacao.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeVenda ?? false)
                    valorMercadoria = listaNotaFiscal.Where(notaFiscal => notaFiscal.ClassificacaoNFe != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe.Venda).Sum(notaFiscal => notaFiscal.Valor);
            }
            else
                valorMercadoria = listaNotaFiscal.Sum(notaFiscal => notaFiscal.Valor);
        }

        if (carga?.DadosSumarizados != null)
        {
            if (!string.IsNullOrWhiteSpace(carga.DadosSumarizados.Recebedores))
                destino = "(Recebedor: " + carga.DadosSumarizados.Recebedores + ")";

            destino += carga.DadosSumarizados.Destinos;
        }

        DetalheCTeMDFe.Filial = carga?.Filial?.Descricao ?? string.Empty;
        DetalheCTeMDFe.NumeroCarga = carga?.Numero ?? string.Empty;
        DetalheCTeMDFe.Transportadora = carga?.Empresa?.RazaoSocial ?? string.Empty;
        DetalheCTeMDFe.Placa = carga?.DadosSumarizados?.Veiculos ?? string.Empty;
        DetalheCTeMDFe.TotalCaixas = carga?.DadosSumarizados?.QuantidadeVolumesNF ?? 0;
        DetalheCTeMDFe.PesoBruto = carga?.DadosSumarizados?.PesoTotal ?? 0;
        DetalheCTeMDFe.ValePedagio = cargaIntegracaoValePedagio?.NumeroValePedagio ?? string.Empty;
        DetalheCTeMDFe.ValorPedagio = cargaIntegracaoValePedagio?.ValorValePedagio ?? 0;
        DetalheCTeMDFe.QuantidadePaletes = listaNotaFiscal.Count > 0 ? listaNotaFiscal.Where(x => x.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet).Sum(x => x.QuantidadePallets) : carga?.Pedidos?.Sum(obj => obj.Pedido?.NumeroPaletes ?? 0) ?? 0;
        DetalheCTeMDFe.Motorista = carga?.Motoristas?.FirstOrDefault()?.DescricaoTelefone ?? string.Empty;
        DetalheCTeMDFe.Destino = destino;
        DetalheCTeMDFe.ValorFrete = carga?.ValorFreteAPagar ?? 0;
        DetalheCTeMDFe.ValorNF = valorMercadoria;
        DetalheCTeMDFe.QuantidadeVolumes = carga?.DadosSumarizados?.VolumesTotal ?? 0;

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet subReportMDFe = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "MDFe",
            DataSet = listaMDFe
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet subReportCTe = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "CTe",
            DataSet = listaCte
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet subReportNFe = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "NFe",
            DataSet = listaNFe
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet subReportPassagens = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "Passagens",
            DataSet = listaPassagem
        };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>() { subReportPassagens };

        subReports.Add(subReportCTe);

        subReports.Add(subReportMDFe);

        subReports.Add(subReportNFe);

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalheCTeMDF> dsDetalheCTeMDF = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalheCTeMDF>() { DetalheCTeMDFe };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dsDetalheCTeMDF,
            SubReports = subReports
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Cargas\DetalheCTeMDF.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}