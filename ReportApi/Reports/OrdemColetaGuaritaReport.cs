using System.Collections.Generic;
using System.Linq;
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

[UseReportType(ReportType.OrdemColetaGuarita)]
public class OrdemColetaGuaritaReport : ReportBase
{
    public OrdemColetaGuaritaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("CodigoCarga");

        Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
        Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

        if (carga == null)
            return null;

        double cnpjFilial;
        double.TryParse(Utilidades.String.OnlyNumbers(carga.Filial.CNPJ), out cnpjFilial);
        bool OcultarQuantidadeValoresOrdemColeta = carga.TipoOperacao?.ConfiguracaoImpressao?.OcultarQuantidadeValoresOrdemColeta ?? false;

        Dominio.Entidades.Cliente filial = repCliente.BuscarPorCPFCNPJ(cnpjFilial);

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Carga> dsCarga = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Carga>()
        {
        new Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Carga()
        {
                DataCarregamento = cargaJanelaCarregamento?.InicioCarregamento ?? carga.DataCarregamentoCarga.Value,
                EmpresaCEP = filial?.CEP,
                EmpresaCidade = carga.Filial.Localidade.Descricao,
                EmpresaCNPJ = carga.Filial.CNPJ_Formatado,
                EmpresaEmail = filial?.Email,
                EmpresaEndereco = filial?.Endereco + " - " + filial?.Numero + ", " + filial?.Complemento + ", " + filial?.Bairro,
                EmpresaEstado = carga.Filial.Localidade.Estado.Sigla,
                EmpresaIE = filial?.IE_RG,
                EmpresaNome = filial?.Nome,
                EmpresaTelefone = filial?.Telefone1,
                ModeloVeiculo = carga.ModeloVeicularCarga?.Descricao,
                Motoristas = carga.RetornarDescricaoMotoristas,
                Numero = carga.CodigoCargaEmbarcador,
                Observacao = cargaJanelaCarregamento?.ObservacaoTransportador ?? string.Empty,
                Peso = carga.Pedidos.Sum(o => o.Peso),
                TipoCarga = carga.TipoDeCarga?.Descricao,
                TransportadorCNPJ = carga.Empresa?.CNPJ_Formatado,
                TransportadorNome = carga.Empresa?.RazaoSocial,
                Veiculos = carga.RetornarPlacas,
                AgruparPorDestinatario = configuracaoGeral.AgruparRelatorioOrdemColetaGuaritaPorDestinatario,
                TipoOperacao = carga.TipoOperacao?.Descricao,
                CodigoDeBarrasNumeroCarga = Utilidades.Barcode.Gerar(carga.Numero,  ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png),
                LicencasVeiculos = carga.RetornarVeiculoLicencas,
        }
        };

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Pedido> dsPedidos = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Pedido>();

        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
        {
            dsPedidos.AddRange((
                from obj in cargaPedido.Pedido.Produtos
                select new Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Pedido()
                {
                    DestinatarioCEP = cargaPedido?.Pedido?.Destinatario?.CEP,
                    DestinatarioCidade = cargaPedido?.Pedido?.Destinatario?.Localidade?.Descricao,
                    DestinatarioCPFCNPJ = cargaPedido?.Pedido?.Destinatario?.CPF_CNPJ_Formatado,
                    DestinatarioEmail = cargaPedido?.Pedido?.Destinatario?.Email,
                    DestinatarioEndereco = (cargaPedido?.Pedido?.Destinatario == null) ? "" : cargaPedido.Pedido.Destinatario.Endereco + " - " + cargaPedido.Pedido.Destinatario.Numero + ", " + cargaPedido.Pedido.Destinatario.Complemento + ", " + cargaPedido.Pedido.Destinatario.Bairro,
                    DestinatarioEstado = cargaPedido?.Pedido?.Destinatario?.Localidade?.Estado.Sigla,
                    DestinatarioIE = cargaPedido?.Pedido?.Destinatario?.IE_RG,
                    DestinatarioNome = cargaPedido?.Pedido?.Destinatario?.Nome,
                    DestinatarioTelefone = cargaPedido?.Pedido?.Destinatario?.Telefone1,
                    Numero = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    NumeroPallets = cargaPedido.Pedido.NumeroPaletes,
                    Peso = cargaPedido.Pedido.PesoTotal,
                    PesoPallets = cargaPedido.Pedido.PesoTotalPaletes,
                    Codigo = obj.Produto.CodigoProdutoEmbarcador,
                    CodigoPedido = obj.Pedido.Codigo,
                    Descricao = obj.Produto.Descricao,
                    PesoTotal = obj.PesoTotal,
                    PesoTotalEmbalagem = obj.PesoTotalEmbalagem,
                    Quantidade = obj.Quantidade,
                    QuantidadePlanejada = obj.QuantidadePlanejada,
                    QuantidadeEmbalagem = obj.QuantidadeEmbalagem,
                    ValorUnitario = obj.ValorProduto,
                    PesoUnitario = obj.PesoUnitario,
                    ValorTotal = obj.ValorProduto * obj.Quantidade,
                    CodigoIntegracao = cargaPedido.Pedido?.Destinatario?.CodigoIntegracao ?? string.Empty,
                    OcultarQuantidadeValoresOrdemColeta = OcultarQuantidadeValoresOrdemColeta
                }
            ).ToList());
        }

        if (carga.Pedidos.Count() > 0 && dsPedidos.Count == 0)
        {
            dsPedidos.AddRange((
                from cargaPedido in carga.Pedidos
                select new Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Pedido()
                {
                    DestinatarioCEP = cargaPedido?.Pedido?.Destinatario.CEP,
                    DestinatarioCidade = cargaPedido?.Pedido?.Destinatario.Localidade.Descricao,
                    DestinatarioCPFCNPJ = cargaPedido?.Pedido?.Destinatario.CPF_CNPJ_Formatado,
                    DestinatarioEmail = cargaPedido?.Pedido?.Destinatario.Email,
                    DestinatarioEndereco = cargaPedido?.Pedido?.Destinatario.Endereco + " - " + cargaPedido?.Pedido?.Destinatario.Numero + ", " + cargaPedido?.Pedido?.Destinatario.Complemento + ", " + cargaPedido?.Pedido?.Destinatario.Bairro,
                    DestinatarioEstado = cargaPedido?.Pedido?.Destinatario.Localidade.Estado.Sigla,
                    DestinatarioIE = cargaPedido?.Pedido?.Destinatario.IE_RG,
                    DestinatarioNome = cargaPedido?.Pedido?.Destinatario.Nome,
                    DestinatarioTelefone = cargaPedido?.Pedido?.Destinatario.Telefone1,
                    Numero = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    NumeroPallets = cargaPedido.Pedido.NumeroPaletes,
                    Peso = cargaPedido.Pedido.PesoTotal,
                    PesoPallets = cargaPedido.Pedido.PesoTotalPaletes,
                    CodigoPedido = cargaPedido.Codigo,
                }
            ).ToList());
        }

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.PedidoDestinatarioLicenca> dsPedidosDestinatariosLicencas = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.PedidoDestinatarioLicenca>();
        if (carga.Pedidos != null)
        {
            foreach (var pedido in carga.Pedidos
                .Where(cargaPedido => cargaPedido.Pedido != null && cargaPedido.Pedido.Destinatario != null && cargaPedido.Pedido.Destinatario.Licencas != null)
                .Select(cargaPedido => cargaPedido.Pedido))
            {
                foreach (var destinatarioLicenca in pedido.Destinatario.Licencas)
                {
                    var licencaAdicionar = new Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.PedidoDestinatarioLicenca()
                    {
                        CodigoPedido = pedido.Codigo,
                        DestinatarioCPFCNPJ = destinatarioLicenca.Pessoa?.CPF_CNPJ_Formatado ?? "",
                        Numero = destinatarioLicenca.Numero,
                        DescricaoLicenca = destinatarioLicenca.Licenca?.Descricao ?? "",
                        DataVencimento = destinatarioLicenca.DataVencimento?.ToString("dd/MM/yyyy") ?? ""
                    };

                    if (VerificarLicencaDestinatarioExiste(licencaAdicionar, dsPedidosDestinatariosLicencas))
                        continue;

                    dsPedidosDestinatariosLicencas.Add(licencaAdicionar);
                }
            }

            if (dsCarga?.Count > 0)
                dsCarga.First().ExibirLicencasDestinatario = dsPedidosDestinatariosLicencas.Count > 0;
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dsCarga,
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                         Key = "OrdemColetaGuarita_Pedido.rpt",
                         DataSet = dsPedidos
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "OrdemColetaGuaritaDestinatario_Pedido.rpt",
                        DataSet = dsPedidos
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "OrdemColetaGuarita_Pedido_Licenca.rpt",
                        DataSet = dsPedidosDestinatariosLicencas
                    }
                }
        };

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
                @"Areas\Relatorios\Reports\Default\Logistica\OrdemColetaGuarita.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }

    private static bool VerificarLicencaDestinatarioExiste(Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.PedidoDestinatarioLicenca pedidoAdicionar, List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.PedidoDestinatarioLicenca> listaVerificar)
    {
        return listaVerificar.Exists(itemLista => itemLista.DestinatarioCPFCNPJ.Equals(pedidoAdicionar.DestinatarioCPFCNPJ)
            && itemLista.Numero.Equals(pedidoAdicionar.Numero)
            && itemLista.DescricaoLicenca.Equals(pedidoAdicionar.DescricaoLicenca)
            && itemLista.DataVencimento.Equals(pedidoAdicionar.DataVencimento));
    }
}