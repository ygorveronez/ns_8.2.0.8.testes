using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Repositorio.Embarcador.Compras;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.OrdemCompra)]
public class OrdemCompraReport : ReportBase
{
    public OrdemCompraReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoOrdemCompra = extraData.GetValue<int>("CodigoOrdemCompra");

        OrdemCompra repOrdemCompra = new OrdemCompra(_unitOfWork);
        var ordemCompra = repOrdemCompra.BuscarPorCodigo(codigoOrdemCompra);

        Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(_unitOfWork);
        Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(_unitOfWork);

        byte[] pdfContent = null;
        if (ordemCompra == null)
            return null;

        List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> mercadorias = repOrdemCompraMercadoria.BuscarPorOrdem(ordemCompra.Codigo);
        List<Dominio.Entidades.Usuario> aprovadores = repAprovacaoAlcadaOrdemCompra.BuscarAprovadores(ordemCompra.Codigo);
        
        if ((bool)ordemCompra.MotivoCompra?.GerarImpressaoOC)
        { // Requisicao de Exames

            Dominio.Relatorios.Embarcador.DataSource.Compras.OrdemCompra DSOrdemCompra = new Dominio.Relatorios.Embarcador.DataSource.Compras.OrdemCompra()
            {
                Numero = ordemCompra.Numero,
                DataPrevisao = ordemCompra.DataPrevisaoRetorno,
                Operador = ordemCompra.Usuario?.Nome ?? "",
                Observacao = ordemCompra.Observacao,

                Motorista = ordemCompra.Motorista?.Nome ?? "",
                MotoristaCFP = ordemCompra.Motorista?.CPF ?? "",
                MotoristaFuncao = ordemCompra.Motorista?.Cargo ?? "",
                MotoristaSetor = ordemCompra.Motorista?.Setor?.Descricao ?? "",
                MotoristaNascimento = ordemCompra.Motorista?.DataNascimento?.ToString("dd/MM/yyyy") ?? "",
                MotoristaRG = ordemCompra.Motorista?.RG ?? "",

                Fornecedor = ordemCompra.Fornecedor?.Nome ?? "",
                FornecedorCNPJ = ordemCompra.Fornecedor?.CPF_CNPJ.ToString() ?? "",
                FornecedorEndereco = ordemCompra.Fornecedor?.Endereco ?? "",
                LocalidadeFornecedor = ordemCompra.Fornecedor?.Localidade.DescricaoCidadeEstado ?? "",
                FornecedorBairro = ordemCompra.Fornecedor?.Bairro ?? "",
                FornecedorFone = ordemCompra.Fornecedor?.Telefone1 ?? ""

            };

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet DSMercadorias = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "_RequisicaoExamesItens_.rpt",
                DataSet = (from obj in mercadorias
                           select new Dominio.Relatorios.Embarcador.DataSource.Compras.OrdemCompraMercadoria
                           {
                               Numero = obj.Produto.CodigoProduto,
                               Produto = obj.Produto.Descricao
                           }).ToList()
            };

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Compras.OrdemCompra>() { DSOrdemCompra },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>() { DSMercadorias }
            };

            pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Compras\Exames\RequisicaoExames.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        }
        else
        {

            Dominio.Relatorios.Embarcador.DataSource.Compras.OrdemCompra DSOrdemCompra = new Dominio.Relatorios.Embarcador.DataSource.Compras.OrdemCompra()
            {
                Numero = ordemCompra.Numero,
                Data = ordemCompra.Data,
                DataPrevisao = ordemCompra.DataPrevisaoRetorno,
                Situacao = ordemCompra.DescricaoSituacao,
                Fornecedor = ordemCompra.Fornecedor.Nome,
                LocalidadeFornecedor = ordemCompra.Fornecedor.Localidade.DescricaoCidadeEstado,
                Operador = ordemCompra.Usuario?.Nome ?? "",
                Transportador = ordemCompra.Transportador?.Nome ?? string.Empty,
                Observacao = ordemCompra.Observacao,
                Placa = ordemCompra.Veiculo?.Placa ?? string.Empty,
                Aprovador = string.Join(", ", aprovadores.Select(o => o.Nome)),
                Motorista = ordemCompra.Motorista?.Nome ?? "",
                MotivoOrdemCompra = ordemCompra.MotivoCompra.Descricao ?? "",
                CondicaoPagamento = ordemCompra.CondicaoPagamento ?? ""
            };

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet DSMercadorias = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "_OrdemCompraMercadoria_.rpt",
                DataSet = (from obj in mercadorias
                           select new Dominio.Relatorios.Embarcador.DataSource.Compras.OrdemCompraMercadoria
                           {
                               Numero = obj.Produto.CodigoProduto,
                               Produto = obj.Produto.Descricao,
                               VeiculoMercadoria = obj.VeiculoMercadoria?.Descricao ?? string.Empty,
                               Quantidade = obj.Quantidade,
                               ValorUnitario = obj.ValorUnitario,
                               ValorTotal = obj.ValorTotal
                           }).ToList()
            };

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Compras.OrdemCompra>() { DSOrdemCompra },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>() { DSMercadorias }
            };

            pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Compras\OrdemCompra.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        }

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}