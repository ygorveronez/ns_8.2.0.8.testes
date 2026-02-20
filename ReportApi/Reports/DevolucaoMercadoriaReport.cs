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

[UseReportType(ReportType.DevolucaoMercadoria)]
public class DevolucaoMercadoriaReport : ReportBase
{
    public DevolucaoMercadoriaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
        Dominio.Entidades.Embarcador.Chamados.Chamado chamado =
            repChamado.BuscarPorCodigo(extraData.GetValue<int>("CodigoChamado"));

        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido =
            new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.Carga carga = chamado.Carga;

        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();

        List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.NotaDevolucao> notas =
            new List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.NotaDevolucao>();

        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal nf in cargaPedido.NotasFiscais)
        {
            Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.NotaDevolucao notaDevolucao =
                new Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.NotaDevolucao()
                {
                    CodigoFilial = cargaPedido.Carga.Filial?.CodigoFilialEmbarcador ?? "",
                    CodigoFornecedor = cargaPedido.Pedido.Remetente.CodigoIntegracao,
                    Fornecedor = cargaPedido.Pedido.Remetente.Descricao,
                    NFNota = nf.XMLNotaFiscal.Numero.ToString(),
                    OC = carga.CodigoCargaEmbarcador,
                    Ocorrencia = chamado.Numero.ToString(),
                    Pallets = chamado.NumeroPallet,
                    Pedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    Produto = "9999 - TODOS",
                    Quantidade = nf.XMLNotaFiscal.Peso,
                    ValorNF = nf.XMLNotaFiscal.ValorTotalProdutos
                };
            notas.Add(notaDevolucao);
        }


        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro DataDevolucao =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        DataDevolucao.NomeParametro = "DataDevolucao";
        DataDevolucao.ValorParametro = chamado.DataCriacao.ToString("dd/MM/yyyy");
        parametros.Add(DataDevolucao);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro MotivoChamado =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        MotivoChamado.NomeParametro = "MotivoChamado";
        MotivoChamado.ValorParametro = chamado.MotivoChamado.Descricao;
        parametros.Add(MotivoChamado);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro Placa =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        Placa.NomeParametro = "Placa";
        Placa.ValorParametro = carga.PlacasVeiculos;
        parametros.Add(Placa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro Motorista =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        Motorista.NomeParametro = "Motorista";
        Motorista.ValorParametro = carga.NomeMotoristas;
        parametros.Add(Motorista);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro NumeroCarga =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        NumeroCarga.NomeParametro = "NumeroCarga";
        NumeroCarga.ValorParametro = carga.CodigoCargaEmbarcador;
        parametros.Add(NumeroCarga);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro Responsavel =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        Responsavel.NomeParametro = "Responsavel";
        Responsavel.ValorParametro = chamado.Responsavel?.Nome ?? "";
        parametros.Add(Responsavel);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro Filial =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        Filial.NomeParametro = "Filial";
        Filial.ValorParametro = carga.Filial?.Descricao ?? "";
        parametros.Add(Filial);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro Transportador =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        Transportador.NomeParametro = "Transportador";
        Transportador.ValorParametro = carga.EmpresaFilialEmissora?.Descricao ?? "";
        parametros.Add(Transportador);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = notas,
                Parameters = parametros
            };

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Chamados\DevolucaoMercadoria.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}