using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Repositorio.Embarcador.Frota;
using Servicos.Embarcador.Relatorios;
using Zen.Barcode;

namespace ReportApi.Reports;

[UseReportType(ReportType.OrdemServico)]
public class OrdemServicoReport : ReportBase
{
    public OrdemServicoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork);

        int codigoOrdemServico = extraData.GetValue<int>("CodigoOrdemServico");

        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork).BuscarConfiguracaoPadrao();
        Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServicoFrota.BuscarPorCodigo(codigoOrdemServico);
        List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> manutencoes = new OrdemServicoFrotaServicoVeiculo(_unitOfWork).BuscarPorOrdemServico(codigoOrdemServico);
        List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao> manutencoesTempoExecucao = new OrdemServicoFrotaServicoVeiculoTempoExecucao(_unitOfWork).BuscarPorOrdemServico(codigoOrdemServico);
        Dominio.Entidades.Empresa empresa = new Repositorio.Empresa(_unitOfWork).BuscarPrincipalEmissoraTMS();
        Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento orcamentoOrdemServico = new OrdemServicoFrotaOrcamento(_unitOfWork).BuscarPorOrdemServico(codigoOrdemServico);
        List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico> orcamentoOrdemServicoServico = new OrdemServicoFrotaOrcamentoServico(_unitOfWork).BuscarPorOrdemServico(codigoOrdemServico);
        List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto> orcamentoOrdemServicoProduto = new OrdemServicoFrotaOrcamentoServicoProduto(_unitOfWork).BuscarPorOrdemServico(codigoOrdemServico);
        List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto> fechamentoOrdemServico = new OrdemServicoFrotaFechamentoProduto(_unitOfWork).BuscarPorOrdemServico(codigoOrdemServico);
        
        if (manutencoes.Count == 0) //Para visualizar mesmo sem serviços
            manutencoes.Add(new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo());

        List<Dominio.Relatorios.Embarcador.DataSource.Frota.OrdemServico> dsOrdemServico =
            (from obj in manutencoes
                select new Dominio.Relatorios.Embarcador.DataSource.Frota.OrdemServico()
                {
                    NumeroOS = ordemServico.Numero,
                    DataProgramada = ordemServico.DataProgramada.ToString("dd/MM/yyyy"),
                    LocalManutencao = ordemServico.LocalManutencao != null
                        ? ordemServico.LocalManutencao.Nome + " (" +
                          ordemServico.LocalManutencao.Localidade.DescricaoCidadeEstado + ")"
                        : string.Empty,
                    Motorista = ordemServico.Motorista?.Nome ?? string.Empty,
                    TelefoneMotorista = ordemServico.Motorista?.Telefone ?? string.Empty,
                    Observacao = ordemServico.Observacao,
                    Veiculo = ordemServico.Veiculo != null
                        ? ordemServico.Veiculo.Placa + (ordemServico.Veiculo.ModeloVeicularCarga != null
                            ? " (" + ordemServico.Veiculo.ModeloVeicularCarga.Descricao + ")"
                            : string.Empty)
                        : "",
                    NumeroFrota = ordemServico.Veiculo?.NumeroFrota ?? string.Empty,
                    ModeloVeiculo = ordemServico.Veiculo?.Modelo?.Descricao ?? string.Empty,
                    MarcaVeiculo = ordemServico.Veiculo?.Marca?.Descricao ?? string.Empty,
                    Quilometragem = ordemServico.QuilometragemVeiculo.ToString("n0"),
                    Equipamento = ordemServico.Equipamento?.Descricao ?? string.Empty,
                    ModeloEquipamento = ordemServico.Equipamento?.ModeloEquipamento?.Descricao ?? string.Empty,
                    MarcaEquipamento = ordemServico.Equipamento?.MarcaEquipamento?.Descricao ?? string.Empty,
                    Horimetro = ordemServico.Horimetro.ToString("n0"),
                    Tipo = ordemServico.TipoOrdemServico?.Descricao ?? string.Empty,
                    Operador = ordemServico.Operador.Nome,
                    DataFechamento = ordemServico.DataFechamento?.ToString("dd/MM/yyyy") ?? string.Empty,
                    CNPJMatriz = configuracaoGeral.ImprimeOrdemServiçoCNPJMatriz
                        ? $"{(empresa?.CNPJ_Formatado ?? string.Empty)} - {(empresa?.RazaoSocial.ToString() ?? string.Empty)} - {(empresa?.Endereco.ToString() ?? string.Empty)} , {(empresa?.Localidade.DescricaoCidadeEstado.ToString() ?? string.Empty)}"
                        : string.Empty,
                    CodigoBarrasImagem = Utilidades.Barcode.Gerar((ordemServico.Numero.ToString()),
                        ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30),
                        System.Drawing.Imaging.ImageFormat.Png),

                    //Dados Serviço
                    CodigoServico = obj.Servico?.Codigo ?? 0,
                    ObservacaoServico = obj.Observacao,
                    Servico = obj.Servico?.Descricao ?? string.Empty,
                    TipoServico = obj.Servico != null ? obj.DescricaoTipoManutencao : string.Empty,
                    UltimaExecucaoServico = obj.UltimaManutencao != null
                        ? "Aos " + obj.UltimaManutencao.OrdemServico.QuilometragemVeiculo + "km em " +
                          obj.UltimaManutencao.OrdemServico.DataProgramada.ToString("dd/MM/yyyy")
                        : string.Empty,
                    TempoEstimado = obj.Servico != null ? obj.TempoEstimado.ToString("n0") + " (min)" : string.Empty,
                    CustoEstimado = obj.Servico != null ? obj.CustoEstimado : decimal.Zero,
                    CustoMedio = obj.Servico != null ? obj.CustoMedio : decimal.Zero
                }).ToList();

        List<Dominio.Relatorios.Embarcador.DataSource.Frota.OrdemServicoManutencao> dsManutencoesTemposervico =
            (from obj in manutencoesTempoExecucao
                select new Dominio.Relatorios.Embarcador.DataSource.Frota.OrdemServicoManutencao()
                {
                    CodigoServico = obj.Servico.Codigo,
                    Mecanico = obj.Mecanico.Nome,
                    Data = obj.Data.ToString("dd/MM/yyyy"),
                    HoraInicio = obj.HoraInicio.HasValue ? obj.HoraInicio.Value.ToString() : string.Empty,
                    HoraFim = obj.HoraFim.HasValue ? obj.HoraFim.Value.ToString() : string.Empty,
                    TempoExecutado = obj.TempoExecutado.ToString("n0") + " (min)"
                }).ToList();

        List<Dominio.Relatorios.Embarcador.DataSource.Frota.OrdemServicoOrcamentoServico>
            dsOrdemServicoOrcamentoServico =
                (from obj in orcamentoOrdemServicoServico
                    select new Dominio.Relatorios.Embarcador.DataSource.Frota.OrdemServicoOrcamentoServico()
                    {
                        CodigoServico = obj.Manutencao.Servico.Codigo,
                        NomeServico = obj.Manutencao.Servico.Descricao,
                        Observacao = obj.Observacao,
                        Parcelas = orcamentoOrdemServico.Parcelas,
                        ValorTotalMaoObra = orcamentoOrdemServico.ValorTotalMaoObra,
                        ValorTotalOrcado = orcamentoOrdemServico.ValorTotalOrcado,
                        ValorTotalPreAprovado = orcamentoOrdemServico.ValorTotalPreAprovado,
                        ValorTotalProdutos = orcamentoOrdemServico.ValorTotalProdutos,

                        CodigoOrdemServicoOrcamentoServico = obj.Codigo,
                        Manutencao = obj.Manutencao.Codigo,
                        ObservacaoServico = obj.Observacao,
                        OrcadoPor = obj.OrcadoPor,
                        OrdemServicoFrotaOrcamento = obj.Orcamento.Codigo,
                        ValorMaoObra = obj.ValorMaoObra,
                        ValorProdutos = obj.ValorProdutos
                    }
                ).ToList();

        List<Dominio.Relatorios.Embarcador.DataSource.Frota.OrdemServicoOrcamentoProduto>
            dsOrdemServicoOrcamentoProduto =
                (from obj in orcamentoOrdemServicoProduto
                    select new Dominio.Relatorios.Embarcador.DataSource.Frota.OrdemServicoOrcamentoProduto()
                    {
                        CodigoServico = obj.OrcamentoServico.Manutencao.Servico.Codigo,
                        Autorizado = obj.Autorizado,
                        Garantia = obj.Garantia,
                        Produto = obj.Produto?.Descricao ?? string.Empty,
                        QuantidadeProduto = obj.OrcamentoServico != null ? obj.Quantidade : decimal.Zero,
                        ValorProduto = obj.OrcamentoServico != null ? obj.Valor : decimal.Zero
                    }
                ).ToList();

        List<Dominio.Relatorios.Embarcador.DataSource.Frota.OrdemServicoFechamento>
            dsOrdemServicoFechamento =
                (from obj in fechamentoOrdemServico
                 select new Dominio.Relatorios.Embarcador.DataSource.Frota.OrdemServicoFechamento()
                 {
                     CodigoServico = obj.Codigo,
                     Autorizado = obj.Autorizado,
                     Garantia = obj.Garantia,
                     Produto = obj.Produto?.Descricao ?? string.Empty,
                     QuantidadeProduto = obj.QuantidadeDocumento,
                     ValorProduto = obj.ValorUnitario
                 }
                ).ToList();


        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsOrdemServico,
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ManutencaoTempoExecucao",
                        DataSet = dsManutencoesTemposervico
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ServicoOrcamentoServico",
                        DataSet = dsOrdemServicoOrcamentoServico
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ServicoOrcamentoProduto",
                        DataSet = dsOrdemServicoOrcamentoProduto
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ServicoFechamento",
                        DataSet = dsOrdemServicoFechamento
                    }
                }
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Frota\OrdemServico.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, true);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}