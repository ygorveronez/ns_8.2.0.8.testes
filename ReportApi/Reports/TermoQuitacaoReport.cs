using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.TermoQuitacao)]
public class TermoQuitacaoReport : ReportBase
{
    public TermoQuitacaoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Financeiro.TermoQuitacao repTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(_unitOfWork);

        var termoQuitacao = repTermoQuitacao.BuscarPorCodigo(extraData.GetValue<int>("CodigoTermoQuitacao"));
        
        string caminhoArquivo = string.Empty;

        if (termoQuitacao == null)
            throw new ServicoException("O transportador selecionado não possui um termo de quitação!");

        if (termoQuitacao.Transportador == null)
            throw new ServicoException("Não existe um transportador selecionado!");

        if (termoQuitacao.Transportador.TipoTermo == TipoTermo.Bilateral)
            caminhoArquivo = "TermoQuitacaoBilateral.rpt";
        else if (termoQuitacao.Transportador.TipoTermo == TipoTermo.Unilateral)
            caminhoArquivo = "TermoQuitacaoUnilateral.rpt";
        else
            throw new ServicoException("O transportador selecionado não possui um tipo de termo cadastrado!");

        Dominio.Relatorios.Embarcador.DataSource.Financeiros.TermoQuitacao dsTermoQuitacao = ObterDadosTermoQuitacao(termoQuitacao);
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TermoQuitacao>() { dsTermoQuitacao },
        };

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio($@"Areas\Relatorios\Reports\Default\Financeiros\{caminhoArquivo}", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
    
    private Dominio.Relatorios.Embarcador.DataSource.Financeiros.TermoQuitacao ObterDadosTermoQuitacao(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacao)
    {
        Dominio.Relatorios.Embarcador.DataSource.Financeiros.TermoQuitacao dsComprovanteEntrega = new Dominio.Relatorios.Embarcador.DataSource.Financeiros.TermoQuitacao()
        {
            BairroTransportador = termoQuitacao.Transportador.Bairro ?? string.Empty,
            CidadeTransportador = termoQuitacao.Transportador.Localidade?.Descricao ?? string.Empty,
            EnderecoTransportador = termoQuitacao.Transportador.Endereco ?? string.Empty,
            NumeroEnderecoTransportador = termoQuitacao.Transportador.Numero ?? string.Empty,
            UFTransportador = termoQuitacao.Transportador.Localidade?.Estado?.Sigla ?? string.Empty,
            EstadoTransportador = termoQuitacao.Transportador.Localidade?.Estado?.Nome ?? string.Empty,
            CNPJFiliais = string.Join(",", termoQuitacao.Transportador.Filiais?.Select(x => x.CNPJ_Formatado ?? string.Empty)),
            CNPJTransportador = termoQuitacao.Transportador.CNPJ_Formatado ?? string.Empty,
            DataCriacaoTermo = termoQuitacao.DataCriacao.Value.ToString("dd/MM/yyyy") ?? string.Empty,
            DataFinalTermo = termoQuitacao.DataFinal.Value.ToString("dd/MM/yyyy") ?? string.Empty,
            InscricaoEstadualTransportador = termoQuitacao.Transportador.InscricaoEstadual ?? string.Empty,
            RazaoSocialTransportador = termoQuitacao.Transportador.RazaoSocial ?? string.Empty,
            ComplementoTransportador = !string.IsNullOrEmpty(termoQuitacao.Transportador.Complemento) ? termoQuitacao.Transportador.Complemento : "S/C",
        };

        return dsComprovanteEntrega;
    }
}