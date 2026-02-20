using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using System.Linq;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.CargaAgrupadaDivisao)]
public class CargaAgrupadaDivisaoReport : ReportBase
{
    public CargaAgrupadaDivisaoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Relatorios.Relatorio repRelatorio =
            new Repositorio.Embarcador.Relatorios.Relatorio(_unitOfWork);

        int codigo = extraData.GetValue<int>("Codigo");
        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

        if (carga == null)
            throw new ServicoException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

        List<Dominio.Entidades.Embarcador.Cargas.Carga> CargasOriginais =
            new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

        if (carga?.TipoOperacao?.ManterUnicaCargaNoAgrupamento ?? false)
            CargasOriginais = repositorioCarga.BuscarCargasOriginaisVinculadas(carga.Codigo);
        else
            CargasOriginais = repositorioCarga.BuscarCargasOriginais(carga.Codigo);

        if (CargasOriginais?.Count == 0)
            throw new ServicoException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);
        
        List<int> CodigoDivisoesDistintas = CargasOriginais
            .Select(o => o.DadosSumarizados.ModeloVeicularCargaDivisaoCapacidade.Codigo).Distinct().OrderBy(o => o)
            .ToList();

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.CargaDivisaoCapacidadeModeloVeicular>
            DataSourceDivisaoCapacidade1 =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.
                    CargaDivisaoCapacidadeModeloVeicular>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.CargaDivisaoCapacidadeModeloVeicular>
            DataSourceDivisaoCapacidade2 =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.
                    CargaDivisaoCapacidadeModeloVeicular>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.CargaDivisaoCapacidadeModeloVeicular>
            DataSourceDivisaoCapacidade3 =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.
                    CargaDivisaoCapacidadeModeloVeicular>();

        if (CodigoDivisoesDistintas.Count > 0)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasCapacidade1 =
                new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            cargasCapacidade1.AddRange(CargasOriginais
                .Where(x => x.DadosSumarizados.ModeloVeicularCargaDivisaoCapacidade.Codigo ==
                            CodigoDivisoesDistintas[0]).OrderBy(x => x.DadosSumarizados.OrdemAgrupamentoDivisao)
                .ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga1 in cargasCapacidade1)
                DataSourceDivisaoCapacidade1.Add(ObterDataSourceDivisaoCapacidadeCarga(carga1, _unitOfWork));
        }

        if (CodigoDivisoesDistintas.Count > 1)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasCapacidade2 =
                new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            cargasCapacidade2.AddRange(CargasOriginais
                .Where(x => x.DadosSumarizados.ModeloVeicularCargaDivisaoCapacidade.Codigo ==
                            CodigoDivisoesDistintas[1]).OrderBy(x => x.DadosSumarizados.OrdemAgrupamentoDivisao)
                .ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga2 in cargasCapacidade2)
                DataSourceDivisaoCapacidade2.Add(ObterDataSourceDivisaoCapacidadeCarga(carga2, _unitOfWork));
        }

        if (CodigoDivisoesDistintas.Count > 2)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasCapacidade3 =
                new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            cargasCapacidade3.AddRange(CargasOriginais
                .Where(x => x.DadosSumarizados.ModeloVeicularCargaDivisaoCapacidade.Codigo ==
                            CodigoDivisoesDistintas[2]).OrderBy(x => x.DadosSumarizados.OrdemAgrupamentoDivisao)
                .ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga3 in cargasCapacidade3)
                DataSourceDivisaoCapacidade3.Add(ObterDataSourceDivisaoCapacidadeCarga(carga3, _unitOfWork));
        }

        Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.DadosCargaAgrupada dataSourceCargaAgrupada
            = new Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.DadosCargaAgrupada()
            {
                CodigoCarga = carga.Codigo,
                CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador,
                Transportador = carga.Empresa.Descricao,
                PlacaVeiculo = carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0
                    ? carga.VeiculosVinculados.ToList()[0].Placa
                    : carga.Veiculo?.Placa ?? "",
                Peso = carga.DadosSumarizados.PesoTotal,
                Volumes = carga.DadosSumarizados.VolumesTotal,
                ValorTotal = carga.DadosSumarizados.ValorTotalProdutos,
                DataCarregamento = carga.DataCarregamentoCarga.HasValue
                    ? carga.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy")
                    : "",
            };

        byte[] pdf =
            this.GerarRelatorioCargaAgrupadaSeparacaoDivisoes(
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.DadosCargaAgrupada>
                    { dataSourceCargaAgrupada }, DataSourceDivisaoCapacidade1, DataSourceDivisaoCapacidade2,
                DataSourceDivisaoCapacidade3);
        
        
        return PrepareReportResult(FileType.PDF, pdf);
    }

    public byte[] GerarRelatorioCargaAgrupadaSeparacaoDivisoes(
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.DadosCargaAgrupada> listaReportCarga,
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.CargaDivisaoCapacidadeModeloVeicular>
            listacargasDivisoes1,
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.CargaDivisaoCapacidadeModeloVeicular>
            listacargasDivisoes2,
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.CargaDivisaoCapacidadeModeloVeicular>
            listacargasDivisoes3)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();

        if (listacargasDivisoes1 != null && listacargasDivisoes1.Count > 0)
        {
            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "CargaAgrupadaDivisaoCapacidadeDetalhes.rpt",
                    DataSet = listacargasDivisoes1
                };
            subReports.Add(ds);
        }
        else
        {
            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "CargaAgrupadaDivisaoCapacidadeDetalhes.rpt",
                    DataSet =
                        new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.
                            CargaDivisaoCapacidadeModeloVeicular>()
                };
            subReports.Add(ds);
        }

        if (listacargasDivisoes2 != null && listacargasDivisoes2.Count > 0)
        {
            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "CargaAgrupadaDivisaoCapacidadeDetalhes.rpt - 01",
                    DataSet = listacargasDivisoes2
                };
            subReports.Add(ds);
        }
        else
        {
            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "CargaAgrupadaDivisaoCapacidadeDetalhes.rpt - 01",
                    DataSet =
                        new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.
                            CargaDivisaoCapacidadeModeloVeicular>()
                };
            subReports.Add(ds);
        }


        if (listacargasDivisoes3 != null && listacargasDivisoes3.Count > 0)
        {
            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "CargaAgrupadaDivisaoCapacidadeDetalhes.rpt - 02",
                    DataSet = listacargasDivisoes3
                };
            subReports.Add(ds);
        }
        else
        {
            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds =
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "CargaAgrupadaDivisaoCapacidadeDetalhes.rpt - 02",
                    DataSet =
                        new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.
                            CargaDivisaoCapacidadeModeloVeicular>()
                };
            subReports.Add(ds);
        }


        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = listaReportCarga,
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>(),
                SubReports = subReports
            };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\CargaAgrupadaDivisaoCapacidade.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, possuiLogo: true);

        return pdf;
    }


    private Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.CargaDivisaoCapacidadeModeloVeicular
        ObterDataSourceDivisaoCapacidadeCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga,
            Repositorio.UnitOfWork unitOfWork)
    {
        Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.CargaDivisaoCapacidadeModeloVeicular divisao =
            new Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada.CargaDivisaoCapacidadeModeloVeicular()
            {
                CodigoDivisao = carga.DadosSumarizados.ModeloVeicularCargaDivisaoCapacidade.Codigo,
                NomeDivisaoModelo = carga.DadosSumarizados.ModeloVeicularCargaDivisaoCapacidade.Descricao,
                QuantidadeDivisaoModelo = carga.DadosSumarizados.ModeloVeicularCargaDivisaoCapacidade.Quantidade,
                CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador,
                Peso = carga.DadosSumarizados.PesoTotal,
                Volumes = carga.DadosSumarizados.VolumesTotal,
                ZonaTransporte = ObterZonaTransporteCarga(carga, unitOfWork)
            };

        return divisao;
    }

    private string ObterZonaTransporteCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga,
        Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Cargas.CargaZonaTransporte repcargaZonaTransporte =
            new Repositorio.Embarcador.Cargas.CargaZonaTransporte(unitOfWork);

        List<Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte> cargaZonaTransporte =
            repcargaZonaTransporte.Consultar(carga.Codigo, null);

        if (cargaZonaTransporte != null && cargaZonaTransporte.Count > 0)
            return cargaZonaTransporte.FirstOrDefault().ZonaTransporte.Descricao;
        else
            return "";
    }
}