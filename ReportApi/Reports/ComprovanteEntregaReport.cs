using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Dominio.Excecoes.Embarcador;
using Servicos.Embarcador.Relatorios;


namespace ReportApi.Reports;

[UseReportType(ReportType.ComprovanteEntrega)]
public class ComprovanteEntregaReport : ReportBase
{
    public ComprovanteEntregaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoDevolucao = extraData.GetValue<int>("CodigoDevolucao");
        var repositorioDevolucaoPallet = new Repositorio.Embarcador.Pallets.DevolucaoPallet(_unitOfWork);
        var devolucao = repositorioDevolucaoPallet.BuscarPorCodigo(codigoDevolucao);

        if (devolucao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Entregue)
            throw new ServicoException("A situação da devolução de pallets não permite a geração do comprovante de entrega.");

        var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
        var repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
        var matriz = repositorioFilial.BuscarMatriz();
        var empresaMatriz = repositorioEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(matriz.CNPJ));

#if DEBUG
        empresaMatriz = repositorioEmpresa.BuscarPorCodigo(1);
#endif

        var dsComprovantes = new List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ComprovanteEntrega>()
        {
            new Dominio.Relatorios.Embarcador.DataSource.Pallets.ComprovanteEntrega()
            {
                NumeroDevolucao = devolucao.NumeroDevolucao,
                CEPEmpresa = empresaMatriz.CEP,
                CidadeEmpresa = empresaMatriz.Localidade.Descricao,
                CNPJEmpresa = empresaMatriz.CNPJ_Formatado,
                EnderecoEmpresa = empresaMatriz.Endereco + ", " + empresaMatriz.Numero + " - " + empresaMatriz.Bairro,
                EstadoEmpresa = empresaMatriz.Localidade.Estado.Nome,
                IEEmpresa = empresaMatriz.InscricaoEstadual,
                NomeEmpresa = empresaMatriz.RazaoSocial,
                TelefoneEmpresa = empresaMatriz.Telefone,
                Filial = devolucao.Filial?.Descricao ?? "",
                DataDevolucao = devolucao.DataDevolucao.Value,
                NumeroNotaFiscal = devolucao.XMLNotaFiscal?.Numero ?? 0,
                Placa = devolucao.CargaPedido?.Carga.RetornarPlacas ?? string.Empty,
                QuantidadePallets = devolucao.QuantidadePallets,
                Transportador = devolucao.Transportador.CNPJ_Formatado + " - " + devolucao.Transportador.RazaoSocial,
                ValorTotal = devolucao.Situacoes.Where(o => o.AcresceSaldo).Sum(o => o.ValorTotal)
            }
        };

        var dsItens = (
            from obj in devolucao.Situacoes
            orderby obj.ValorUnitario
            select new Dominio.Relatorios.Embarcador.DataSource.Pallets.ComprovanteEntregaItem()
            {
                Descricao = obj.Situacao.Descricao,
                Quantidade = obj.Quantidade
            }
        ).ToList();

        var ds1 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ComprovanteEntrega_Itens.rpt",
            DataSet = dsItens
        };

        var ds2 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ComprovanteEntrega_Itens.rpt - 01",
            DataSet = dsItens
        };

        var ds3 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ComprovanteEntrega_Itens.rpt - 02",
            DataSet = dsItens
        };

        var subReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();

        subReports.Add(ds1);
        subReports.Add(ds2);
        subReports.Add(ds3);

        var dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dsComprovantes,
            SubReports = subReports
        };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Pallets\ComprovanteEntrega.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);
        
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}