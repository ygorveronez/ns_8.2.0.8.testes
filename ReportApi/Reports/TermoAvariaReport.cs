using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.TermoAvaria)]
public class TermoAvariaReport : ReportBase
{
    public TermoAvariaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria =
            new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

        Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria =
            repSolicitacaoAvaria.BuscarPorCodigo(extraData.GetValue<int>("CodigoTermoAvaria"));

        if (avaria == null)
            throw new ServicoException("Não foi possível encontrar o registro.");

        string nomeEmpresa = "Danone";
        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctes = repCargaCTe.BuscarPreCTePorCarga(avaria.Carga.Codigo);

        List<Dominio.Relatorios.Embarcador.DataSource.Avarias.TermoAvaria> dsInformacoes =
            new List<Dominio.Relatorios.Embarcador.DataSource.Avarias.TermoAvaria>()
            {
                new Dominio.Relatorios.Embarcador.DataSource.Avarias.TermoAvaria()
                {
                    NomeEmpresa = nomeEmpresa,
                    NumeroAvaria = avaria.NumeroAvaria,
                    Transportador = avaria.Transportador?.RazaoSocial ?? string.Empty,
                    Filial = avaria.Carga.Filial?.Descricao ?? string.Empty,
                    TipoTransporte = avaria.Carga.TipoOperacao?.Descricao ?? string.Empty,
                    Origem = avaria.Carga.DadosSumarizados?.Origens ?? string.Empty,
                    Destino = avaria.Carga.DadosSumarizados?.Destinos ?? string.Empty,
                    MotivoAvaria = avaria.MotivoAvaria.Descricao,
                    Motorista = avaria.Motorista ?? string.Empty,
                    RGMotorista = avaria.RGMotorista ?? string.Empty,
                    CPFMotorista = avaria.CPFMotorista ?? string.Empty,
                    Placa = avaria.Carga.Veiculo?.Placa ?? string.Empty,
                    CTe = avaria.Carga.CargaCTes.Count() > 0
                        ? String.Join(", ", (from o in ctes where o.CTe != null select o.CTe.Numero).ToArray())
                        : string.Empty,
                    DataEntrega = avaria.Carga.DataPrevisaoTerminoCarga.HasValue
                        ? avaria.Carga.DataPrevisaoTerminoCarga.Value
                        : DateTime.Now,
                    NotasFiscais = avaria.ProdutosAvariados.Count() > 0
                        ? String.Join(", ",
                            (from c in avaria.ProdutosAvariados
                                where c.ProdutoAvariado && !string.IsNullOrWhiteSpace(c.NotaFiscal)
                                select c.NotaFiscal).ToArray())
                        : string.Empty,
                    Viagens = avaria.Carga.CodigoCargaEmbarcador,
                    ValorAvaria = avaria.ValorAvaria,
                    ValorAvariaExtenso = Utilidades.Conversor.DecimalToWords(avaria.ValorAvaria)
                }
            };

        List<Dominio.Relatorios.Embarcador.DataSource.Avarias.TermoAvariaProduto> dsItens;
        dsItens = (from obj in avaria.ProdutosAvariados
            where obj.ProdutoAvariado
            select new Dominio.Relatorios.Embarcador.DataSource.Avarias.TermoAvariaProduto()
            {
                Codigo = obj.ProdutoEmbarcador.CodigoProdutoEmbarcador,
                Descricao = obj.ProdutoEmbarcador.Descricao,
                Caixas = obj.CaixasAvariadas,
                Unidades = obj.UnidadesAvariadas,
                ValorAvaria = obj.ValorAvaria
            }).ToList();


        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "TermoAvariaProduto.rpt",
                DataSet = dsItens
            };
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsInformacoes,
                SubReports = subReports
            };
        
        string caminhoLogo =
            Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"],
                "crystal.png");
        
        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Avarias\TermoAvaria.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}