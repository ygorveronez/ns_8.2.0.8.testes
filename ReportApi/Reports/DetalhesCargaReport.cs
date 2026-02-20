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

[UseReportType(ReportType.DetalhesCarga)]
public class DetalhesCargaReport : ReportBase
{
    public DetalhesCargaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("CodigoCarga");

        Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento =
            new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento =
            repCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

        if (carga == null)
            return null;

        double cnpjFilial;
        double.TryParse(Utilidades.String.OnlyNumbers(carga.Filial.CNPJ), out cnpjFilial);

        Dominio.Entidades.Cliente filial = repCliente.BuscarPorCPFCNPJ(cnpjFilial);

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Carga> dsCarga =
            new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Carga>()
            {
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Carga()
                {
                    DataCarregamento = cargaJanelaCarregamento?.InicioCarregamento ?? carga.DataCarregamentoCarga.Value,
                    EmpresaCEP = filial?.CEP,
                    EmpresaCidade = carga.Filial.Localidade.Descricao,
                    EmpresaCNPJ = carga.Filial.CNPJ_Formatado,
                    EmpresaEmail = filial?.Email,
                    EmpresaEndereco = filial?.Endereco + " - " + filial?.Numero + ", " + filial?.Complemento + ", " +
                                      filial?.Bairro,
                    EmpresaEstado = carga.Filial.Localidade.Estado.Sigla,
                    EmpresaIE = filial?.IE_RG,
                    EmpresaNome = filial?.Nome,
                    EmpresaTelefone = filial?.Telefone1,
                    ModeloVeiculo = carga.ModeloVeicularCarga?.Descricao,
                    Motoristas = carga.RetornarDescricaoMotoristas,
                    Numero = carga.CodigoCargaEmbarcador,
                    Observacao = cargaJanelaCarregamento.ObservacaoTransportador,
                    Peso = carga.Pedidos.Sum(o => o.Peso),
                    TipoCarga = carga.TipoDeCarga?.Descricao,
                    TransportadorCNPJ = carga.Empresa?.CNPJ_Formatado,
                    TransportadorNome = carga.Empresa?.RazaoSocial,
                    Veiculos = carga.RetornarPlacas,
                    CodigoDeBarrasNumeroCarga = Utilidades.Barcode.Gerar(carga.Numero, ZXing.BarcodeFormat.CODE_128,
                        new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png)
                }
            };

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Pedido> dsPedidos =
            new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DetalhesCarga.Pedido>();

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
                    DestinatarioEndereco = (cargaPedido?.Pedido?.Destinatario == null)
                        ? ""
                        : cargaPedido.Pedido.Destinatario.Endereco + " - " + cargaPedido.Pedido.Destinatario.Numero +
                          ", " + cargaPedido.Pedido.Destinatario.Complemento + ", " +
                          cargaPedido.Pedido.Destinatario.Bairro,
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
                    PesoUnitario = obj.PesoUnitario,
                    Quantidade = obj.Quantidade,
                    QuantidadePlanejada = obj.QuantidadePlanejada,
                    QuantidadeEmbalagem = obj.QuantidadeEmbalagem,
                    ValorTotal = obj.ValorProduto * obj.Quantidade,
                    ValorUnitario = obj.ValorProduto
                }
            ).ToList());
        }

        // Quando não ha produtos no pedido, os detalhes do pedido saem branco
        // Nesse caso, devemos preencher apenas com os dados do pedido sem o produto
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
                    DestinatarioEndereco = cargaPedido?.Pedido?.Destinatario.Endereco + " - " +
                                           cargaPedido?.Pedido?.Destinatario.Numero + ", " +
                                           cargaPedido?.Pedido?.Destinatario.Complemento + ", " +
                                           cargaPedido?.Pedido?.Destinatario.Bairro,
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

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsCarga,
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "DetalheCarga_Pedido.rpt",
                        DataSet = dsPedidos
                    }
                }
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\DetalheCarga.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}