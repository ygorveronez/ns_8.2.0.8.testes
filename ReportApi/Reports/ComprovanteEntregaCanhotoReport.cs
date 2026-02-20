using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Zen.Barcode;

namespace ReportApi.Reports;

[UseReportType(ReportType.ComprovanteEntregaCanhoto)]
public class ComprovanteEntregaCanhotoReport : ReportBase
{
    public ComprovanteEntregaCanhotoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        string caminhoLogo = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"], "crystal.png");
        //TODO : Validar se Usaremos AppSettings ou a tabela de Configuracao de arquivo

        Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(extraData.GetValue<int>("Codigo"));

        if (canhoto == null)
            throw new ServicoException(Localization.Resources.Canhotos.Canhoto.NaoExisteUmCanhotoEnviadoParaEsteDocumento);

        Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
        Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCNPJ(canhoto.Pedido?.Remetente?.CPF_CNPJ.ToString() ?? string.Empty);

        Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.ComprovanteEntrega dsComprovanteEntrega = ObterDadosComprovanteEntrega(canhoto, filial);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        {
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoImagem", caminhoLogo, true)
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.ComprovanteEntrega>() { dsComprovanteEntrega },
            Parameters = parametros
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Canhotos\ComprovanteEntrega.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdf);
    }
    
    private Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.ComprovanteEntrega ObterDadosComprovanteEntrega(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.ComprovanteEntrega dsComprovanteEntrega = new Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.ComprovanteEntrega()
            {
                RazaoSocialRemetente = canhoto.Pedido?.Remetente?.Nome ?? string.Empty,
                EnderecoRemetente = canhoto.Pedido?.Remetente?.EnderecoCompletoCidadeeEstado ?? string.Empty,

                RazaoSocialTransportador = canhoto.Carga?.Empresa?.RazaoSocial ?? string.Empty,
                FretePorConta = canhoto.Pedido?.TipoTomador.ObterDescricao() ?? string.Empty,
                CNPJTransportador = canhoto.Carga?.Empresa?.CNPJ_Formatado ?? string.Empty,
                EnderecoTransportador = canhoto.Carga?.Empresa?.Endereco ?? string.Empty,
                UFTransportador = canhoto.Carga?.Empresa?.Localidade?.Estado?.Sigla ?? string.Empty,
                CidadeTransportador = canhoto.Carga?.Empresa?.Localidade?.Descricao ?? string.Empty,
                PlacaVeiculo = canhoto.Carga?.Veiculo?.Placa ?? string.Empty,
                ANTT = canhoto.Carga?.Empresa?.RegistroANTT ?? string.Empty,


                RazaoSocialDestinatario = canhoto.Pedido?.Destinatario.Nome ?? string.Empty,
                EnderecoDestinatario = canhoto.Pedido?.Destinatario?.EnderecoCompleto ?? string.Empty,
                CNPJDestinatario = canhoto.Pedido?.Destinatario?.CPF_CNPJ_Formatado ?? string.Empty,
                CidadeDestinatario = canhoto.Pedido?.Destinatario?.Localidade?.Descricao ?? string.Empty,
                UFDestinatario = canhoto.Pedido?.Destinatario?.Localidade?.Estado?.Sigla ?? string.Empty,
                IEDestinatario = canhoto.XMLNotaFiscal?.IEDestinatario ?? string.Empty,

                Centro = filial?.CodigoFilialEmbarcador ?? string.Empty,
                ValorTotalNF = canhoto.XMLNotaFiscal?.Valor.ToString("n2") ?? string.Empty,
                ValorDesconto = canhoto.XMLNotaFiscal?.ValorDesconto.ToString("n2") ?? string.Empty,
                ValorCobranca = canhoto.XMLNotaFiscal?.ValorLiquido.ToString("n2") ?? string.Empty,
                ValorParaSeguro = canhoto.XMLNotaFiscal?.ValorSeguro.ToString("n2") ?? string.Empty,
                ValorOutros = canhoto.XMLNotaFiscal?.ValorOutros.ToString("n2") ?? string.Empty,
                DocTransportes = canhoto.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                Fatura = canhoto.XMLNotaFiscal?.NumeroDaFatura.ToString() ?? string.Empty,
                Remessa = canhoto.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                DocExternoTMS = canhoto.Carga?.ExternalID1 ?? string.Empty,
                Quantidade = canhoto.XMLNotaFiscal?.Volumes.ToString() ?? string.Empty,
                NumeroNF = canhoto.XMLNotaFiscal?.Numero.ToString() ?? string.Empty,
                SerieNF = canhoto.XMLNotaFiscal?.Serie ?? string.Empty,
                ChaveAcessoNF = canhoto.XMLNotaFiscal?.Chave ?? string.Empty,
                DataEmissao = canhoto.XMLNotaFiscal?.DataEmissao.ToString("dd/MM/yyyy") ?? string.Empty,
                EspecieCX = canhoto.XMLNotaFiscal?.Especie ?? string.Empty,
                PesoBruto = canhoto.XMLNotaFiscal?.Peso.ToString("n2") ?? string.Empty,
                CodigoDeBarras = !string.IsNullOrWhiteSpace(canhoto.XMLNotaFiscal?.Chave) ? Utilidades.Barcode.Gerar(canhoto.XMLNotaFiscal.Chave, ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Bmp) : null
            };

            return dsComprovanteEntrega;
        }
}