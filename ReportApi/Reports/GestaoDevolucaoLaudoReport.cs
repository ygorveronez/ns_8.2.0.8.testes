using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System;

namespace ReportApi.Reports;

[UseReportType(ReportType.GestaoDevolucaoLaudo)]
public class GestaoDevolucaoLaudoReport : ReportBase
{
    public GestaoDevolucaoLaudoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoGestaoDevolucaoLaudo = extraData.GetValue<int>("CodigoGestaoDevolucaoLaudo");

        Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo repositorioGestaoDevolucaoLaudo = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo(_unitOfWork);
        Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(_unitOfWork);
        Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

        Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo gestaoDevolucaoLaudo = repositorioGestaoDevolucaoLaudo.BuscarPorCodigo(codigoGestaoDevolucaoLaudo);

        if (gestaoDevolucaoLaudo == null)
            throw new ServicoException("Não foi possível encontrar o registro.");

        Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorGestaoDevolucaoLaudo(gestaoDevolucaoLaudo.Codigo);
        Dominio.Entidades.Embarcador.Cargas.Carga carga = gestaoDevolucao?.CargaOrigem ?? null;

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDevolucaoLaudo.GestaoDevolucaoLaudoProduto> listaProdutos = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDevolucaoLaudo.GestaoDevolucaoLaudoProduto>();
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();

        Dominio.Entidades.Cliente clienteFilial = (carga?.Filial != null) ? repositorioCliente.BuscarPorCPFCNPJ(carga.Filial.CNPJ.ToDouble()) : null;

        BuscarListaProdutos(gestaoDevolucaoLaudo.Codigo, listaProdutos);

        Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDevolucaoLaudo.GestaoDevolucaoLaudo gestaoDevolucaoLaudoDados = new Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDevolucaoLaudo.GestaoDevolucaoLaudo()
        {
            CodigoLaudo = gestaoDevolucaoLaudo.Codigo,
            FilialCargaOrigem = carga?.Filial?.Descricao ?? string.Empty,
            FilialCidadeEstado = clienteFilial?.EnderecoCompletoCidadeeEstado ?? string.Empty,
            FilialCNPJ = clienteFilial?.CPF_CNPJ_Formatado ?? string.Empty,
            FilialCEP = clienteFilial?.CEPFormatado ?? string.Empty,
            FilialIE = clienteFilial?.IE_RG ?? string.Empty,
            TransportadorCarga = gestaoDevolucaoLaudo.Transportador?.RazaoSocial ?? string.Empty,
            NumeroDTCarga = carga?.CodigoCargaEmbarcador ?? string.Empty,
            NumeroLaudoCarga = (int)gestaoDevolucaoLaudo.Codigo,
            PlacaVeiculoCarga = gestaoDevolucaoLaudo.Veiculo?.Placa_Formatada ?? string.Empty,
            ResponsavelCarga = gestaoDevolucaoLaudo.Responsavel?.Nome ?? string.Empty,
            UsuarioCarga = gestaoDevolucaoLaudo.Responsavel?.Nome ?? string.Empty,
            DataCriacao = gestaoDevolucaoLaudo.DataCriacao.ToString("dd/MM/yyyy") ?? string.Empty,
            HoraCriacao = gestaoDevolucaoLaudo.DataCriacao.ToString("HH:mm:ss") ?? string.Empty,
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "GestaoDevolucaoLaudoProduto",
            DataSet = listaProdutos
        };

        subReports.Add(ds1);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDevolucaoLaudo.GestaoDevolucaoLaudo> { gestaoDevolucaoLaudoDados },
            SubReports = subReports
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Cargas\GestaoDevolucaoLaudo.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, possuiLogo: true);

        return PrepareReportResult(FileType.PDF, pdf);
    }

    private void BuscarListaProdutos(long codigoGestaoDevolucaoLaudo, List<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDevolucaoLaudo.GestaoDevolucaoLaudoProduto> listaProdutos)
    {
        decimal totalQuantidadeOrigem = 0;
        decimal totalQuantidadeAvariada = 0;
        decimal totalQuantidadeDevolvida = 0;
        decimal totalQuantidadeFalta = 0;
        decimal totalValorAvariado = 0;
        decimal totalValorFalta = 0;
        decimal totalValor = 0;
        decimal totalQuantidadeSemCondicao = 0;
        decimal totalValorSemCondicao = 0;

        Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto repositorioGestaoDevolucaoLaudoProduto = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto(_unitOfWork);
        List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto> gestaoDevolucaoLaudoProdutos = repositorioGestaoDevolucaoLaudoProduto.BuscarPorGestaoDevolucaoLaudo(codigoGestaoDevolucaoLaudo);

        foreach (Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto gestaoDevolucaoLaudoProduto in gestaoDevolucaoLaudoProdutos)
        {
            totalQuantidadeOrigem += gestaoDevolucaoLaudoProduto.QuantidadeOrigem;
            totalQuantidadeAvariada += gestaoDevolucaoLaudoProduto.QuantidadeAvariada;
            totalQuantidadeDevolvida += gestaoDevolucaoLaudoProduto.QuantidadeDevolvida;
            totalQuantidadeFalta += gestaoDevolucaoLaudoProduto.QuantidadeFalta;
            totalValorAvariado += gestaoDevolucaoLaudoProduto.ValorAvariado;
            totalValorFalta += gestaoDevolucaoLaudoProduto.ValorFalta;
            totalValor += gestaoDevolucaoLaudoProduto.ValorTotal;
            totalQuantidadeSemCondicao += gestaoDevolucaoLaudoProduto.QuantidadeSemCondicao;
            totalValorSemCondicao += gestaoDevolucaoLaudoProduto.ValorSemCondicao;

            listaProdutos.Add(new Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDevolucaoLaudo.GestaoDevolucaoLaudoProduto()
            {
                CodigoLaudo = gestaoDevolucaoLaudoProduto.Laudo.Codigo,
                CodigoIntegracao = gestaoDevolucaoLaudoProduto.Produto?.CodigoProdutoEmbarcador ?? "-",
                Descricao = gestaoDevolucaoLaudoProduto.Produto?.Descricao ?? "-",
                QuantidadeOrigem = gestaoDevolucaoLaudoProduto.QuantidadeOrigem,
                TotalQuantidadeOrigem = totalQuantidadeOrigem,
                QuantidadeAvariada = gestaoDevolucaoLaudoProduto.QuantidadeAvariada,
                TotalQuantidadeAvariada = totalQuantidadeAvariada,
                QuantidadeDevolvida = gestaoDevolucaoLaudoProduto.QuantidadeDevolvida,
                TotalQuantidadeDevolvida = totalQuantidadeDevolvida,
                QuantidadeFalta = gestaoDevolucaoLaudoProduto.QuantidadeFalta,
                TotalQuantidadeFalta = totalQuantidadeFalta,
                ValorAvariado = gestaoDevolucaoLaudoProduto.ValorAvariado,
                TotalValorAvariado = totalValorAvariado,
                ValorFalta = gestaoDevolucaoLaudoProduto.ValorFalta,
                TotalValorFalta = totalValorFalta,
                ValorTotal = gestaoDevolucaoLaudoProduto.ValorTotal,
                TotalValor = totalValor,
                ValorSemCondicao = gestaoDevolucaoLaudoProduto.ValorSemCondicao,
                TotalValorSemCondicao = totalValorSemCondicao,
                QuantidadeSemCondicao = gestaoDevolucaoLaudoProduto.QuantidadeSemCondicao,
                TotalQuantidadeSemCondicao = totalQuantidadeSemCondicao,
            });
        }
    }
}