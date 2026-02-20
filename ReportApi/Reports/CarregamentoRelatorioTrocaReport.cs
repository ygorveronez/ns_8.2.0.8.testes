using System;
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

namespace ReportApi.Reports;

[UseReportType(ReportType.CarregamentoRelatorioTroca)]
public class CarregamentoRelatorioTrocaReport : ReportBase
{
    public CarregamentoRelatorioTrocaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoCarga = extraData.GetValue<int>("CodigoCarga");
        var relatorioTroca = extraData.GetValue<bool>("relatorioTroca");    
        
        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
        
        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS =
            new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS =
            repConfiguracaoTMS.BuscarConfiguracaoPadrao();
        
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = null;
        if (configuracaoTMS.NaoImprimirNotasBoletosComRecebedor)
            cargaPedidos = (from obj in carga.Pedidos.Where(o => o.Recebedor is null) select obj).ToList();
        else
            cargaPedidos = (from obj in carga.Pedidos select obj).ToList();

        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosOrdenado =
            new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

        Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Carga DSHead =
            new Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Carga
            {
                Transporte = carga.CodigoCargaEmbarcador,
                Motorista = carga.Motoristas.FirstOrDefault()?.Nome ?? "",
                Placa = carga.Veiculo?.Placa ?? ""
            };


        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Pedido> DSPedido =
            new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Pedido>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Produto> DSProdutos =
            new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Produto>()
            {
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Produto()
                {
                    Pai = 0
                }
            };
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Pagamento> DSPagamentos =
            new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Pagamento>()
            {
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Pagamento()
                {
                    Pai = 0
                }
            };
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Recolhimento> DSRecolhimentos =
            new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Recolhimento>()
            {
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Recolhimento()
                {
                    Pai = 0
                }
            };

        if (relatorioTroca)
            cargaPedidos = (from o in cargaPedidos where o.Pedido.PedidosRecolhimentoTroca.Count > 0 select o).ToList();
        
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.PedidoAgrupado> PedidosAgrupados = null;

        PedidosAgrupados = cargaPedidos.OrderBy(obj => obj.OrdemEntrega).GroupBy(x => new
        {
            CNPJCPF = x.Pedido.Destinatario.CPF_CNPJ_Formatado,
            Cliente = x.Pedido.Destinatario.CodigoIntegracao + " - " + x.Pedido.Destinatario.Nome,
            Fantasia = x.Pedido.Destinatario.NomeFantasia,
            Fone = x.Pedido.Destinatario.Telefone1,
            Numero = x.Pedido.Destinatario.Numero,
            Endereco = x.Pedido.Destinatario.Endereco,
            Bairro = x.Pedido.Destinatario.Bairro,
            Cidade = x.Pedido.Destinatario.Localidade.Descricao,
            UF = x.Pedido.Destinatario.Localidade.Estado.Sigla,
            CEP = x.Pedido.Destinatario.CEP,
            Complemento = x.Pedido.Destinatario.Complemento,
        }).Select(g => new Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.PedidoAgrupado()
        {
            CNPJCPF = g.Key.CNPJCPF,
            Cliente = g.Key.Cliente,
            Fantasia = g.Key.Fantasia,
            Fone = g.Key.Fone,
            Numero = g.Key.Numero,
            Endereco = g.Key.Endereco,
            Bairro = g.Key.Bairro,
            Cidade = g.Key.Cidade,
            UF = g.Key.UF,
            CEP = g.Key.CEP,
            Complemento = g.Key.Complemento,
            CargasPedidos = g.ToList()
        }).ToList();

        for (int i = 0, s = PedidosAgrupados.Count; i < s; i++)
        {
            Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.PedidoAgrupado PedidoAgrupado = PedidosAgrupados[i];
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> Produtos =
                new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoRecolhimentoTroca> PedidoRecolhimentoTroca =
                new List<Dominio.Entidades.Embarcador.Pedidos.PedidoRecolhimentoTroca>();

            string notasFiscais = string.Empty;
            string restricoes = string.Empty;
            string observacao = string.Empty;
            string blocos = string.Empty;

            int Codigo = PedidoAgrupado.CargasPedidos.Select(x => x.Codigo).FirstOrDefault();

            foreach (var cargaPedido in PedidoAgrupado.CargasPedidos)
            {
                notasFiscais += String.Join(", ", (from n in cargaPedido.NotasFiscais select n.XMLNotaFiscal.Numero)) +
                                " ";
                restricoes += string.Join(", ",
                    (from o in cargaPedido.Pedido.Destinatario.ClienteDescargas
                        select string.Join(", ", from r in o.RestricoesDescarga select r.Descricao))) + " ";
                observacao += cargaPedido.Pedido.Observacao + "  ";

                blocos = string.Join(", ", (from blocoCarregamento in cargaPedido.Pedido.Blocos
                                                where cargaPedido.Carga.Carregamento?.Codigo == blocoCarregamento.Carregamento.Codigo
                                              select blocoCarregamento.Bloco).Distinct());

                Produtos.AddRange(cargaPedido.Produtos);
                PedidoRecolhimentoTroca.AddRange(cargaPedido.Pedido.PedidosRecolhimentoTroca);
            }

            DSPedido.Add(new Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Pedido
            {
                Codigo = Codigo,
                Sequencia = (i + 1).ToString(),
                Operacao = PedidoAgrupado.CargasPedidos.Select(x => x.Pedido.CanalEntrega?.Descricao ?? "")
                    .FirstOrDefault(),
                VendaNFe = notasFiscais,
                Cliente = PedidoAgrupado.Cliente,
                Fantasia = PedidoAgrupado.Fantasia,
                CNPJCPF = PedidoAgrupado.CNPJCPF,
                Fone = PedidoAgrupado.Fone,
                Numero = PedidoAgrupado.Numero,
                Endereco = PedidoAgrupado.Endereco,
                Bairro = PedidoAgrupado.Bairro,
                Cidade = PedidoAgrupado.Cidade,
                UF = PedidoAgrupado.UF,
                CEP = PedidoAgrupado.CEP,
                Referencia = PedidoAgrupado.Complemento,
                Restricoes = restricoes,
                DataEntrega = DateTime.Now,
                Observacao = observacao,
                Blocos = blocos,
                QuantidadeTotal = Produtos.Sum(o => o.Quantidade),
            });

            if (!relatorioTroca && Produtos != null && Produtos.Count > 0)
            {
                DSProdutos.AddRange((from p in Produtos
                    select new Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Produto()
                    {
                        Pai = Codigo,
                        Item =
                            !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(p.Produto.CodigoProdutoEmbarcador))
                                ? int.Parse(Utilidades.String.OnlyNumbers(p.Produto.CodigoProdutoEmbarcador)).ToString()
                                : p.Produto.CodigoProdutoEmbarcador,
                        Descricao = p.Produto.Descricao,
                        Quantidade = p.Quantidade,
                        Devolucao = "",
                    }).ToList());
            }

            if (!relatorioTroca)
            {
                DSPagamentos.AddRange(new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Pagamento>()
                    {
                    }
                );
            }

            if (PedidoRecolhimentoTroca != null && PedidoRecolhimentoTroca.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoRecolhimentoTroca pedidoRecolhimento in
                         PedidoRecolhimentoTroca)
                {
                    DSRecolhimentos.AddRange((from o in pedidoRecolhimento.RecolhimentoTroca.Produtos
                        select new Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca.Recolhimento
                        {
                            Pai = Codigo,
                            Item = int.Parse(Utilidades.String.OnlyNumbers(o.Produto.CodigoProdutoEmbarcador))
                                .ToString(),
                            Descricao = o.Produto.Descricao,
                            Quantidade = o.Quantidade,
                            QuantidadePlanejada = o.QuantidadePlanejada,
                            Valor = o.ValorProduto * o.Quantidade
                        }).ToList());
                }
            }
        }


        decimal Totalizador = DSPedido.Sum(o => o.QuantidadeTotal);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = DSPedido,
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", DSHead.Motorista ?? "",
                        true),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Placa", DSHead.Placa ?? "", true),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transporte", DSHead.Transporte ?? "",
                        true),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo",
                        relatorioTroca ? "Relatório Troca" : "Relatório Entrega"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RelatorioTroca",
                        relatorioTroca ? "true" : "false"),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Totalizador", Totalizador),
                },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        DataSet = DSProdutos,
                        Key = "Produtos"
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        DataSet = DSPagamentos,
                        Key = "Pagamentos"
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        DataSet = DSRecolhimentos,
                        Key = "Recolhimentos"
                    }
                }
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\Troca.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, false);
        
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}