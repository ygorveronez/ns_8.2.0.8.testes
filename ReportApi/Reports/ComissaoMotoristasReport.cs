using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System.Collections.Generic;

namespace ReportApi.Reports;
[UseReportType(ReportType.ComissaoMotoristas)]
public class ComissaoMotoristasReport : ReportBase
{
    public ComissaoMotoristasReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        #region Parametros

        int codigoComissao = extraData.GetValue<int>("codigoComissao");
        string nomeCliente = extraData.GetValue<string>("nomeCliente");
        Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(_unitOfWork);
        Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(_unitOfWork);
        IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotorista> comissoesFuncionarioMotorista = serComissaoFuncionario.BuscarListaDataSetComissao(codigoComissao, _unitOfWork);
        bool utilizarComissaoPorCargo = extraData.GetValue<bool>("utilizarComissaoPorCargo");
        IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento> comissaoFuncionarioMotoristaAbastecimento = serComissaoFuncionario.BuscarListaDataSetComissaoAbastecimento(codigoComissao, _unitOfWork);
        if (comissaoFuncionarioMotoristaAbastecimento == null || comissaoFuncionarioMotoristaAbastecimento.Count == 0)
        {
            comissaoFuncionarioMotoristaAbastecimento = new List<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento>();
            comissaoFuncionarioMotoristaAbastecimento.Add(new Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento { CodigoComissaoFuncionarioMotorista = -1 });
        }

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);
        #endregion

        #region MetodosInternos
        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioComissoes(codigoComissao, nomeCliente, comissoesFuncionarioMotorista, utilizarComissaoPorCargo, comissaoFuncionarioMotoristaAbastecimento);
        #endregion

        if (relatorioControleGeracao != null)
        {
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "RH/ComissaoFuncionario", _unitOfWork);
            return PrepareReportResult(FileType.PDF);
        }
        return PrepareReportResult(FileType.PDF, RelatorioSemPadraoReportService.ObterBufferReport(report, TipoArquivoRelatorio.PDF));
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioComissoes(int codigoComissaoFuncionario, string nomeEmpresa, IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotorista> comissoesFuncionarioMotorista, bool comissaoGeracaPorCargo, IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento> comissaoFuncionarioMotoristaAbastecimento)
    {

        Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(_unitOfWork);
        Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(codigoComissaoFuncionario);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroMensagemBC = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametroMensagemBC.NomeParametro = "MesagemBaseCalculoComissao";
        parametroMensagemBC.ValorParametro = comissaoFuncionario.MesagemBaseCalculoComissao;
        parametros.Add(parametroMensagemBC);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroDataInicio = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametroDataInicio.NomeParametro = "DataInicio";
        parametroDataInicio.ValorParametro = comissaoFuncionario.DataInicio.ToString("dd/MM/yyyy");
        parametros.Add(parametroDataInicio);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroDataFim = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametroDataFim.NomeParametro = "DataFim";
        parametroDataFim.ValorParametro = comissaoFuncionario.DataFim.ToString("dd/MM/yyyy");
        parametros.Add(parametroDataFim);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroLocalidade = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametroLocalidade.NomeParametro = "Localidade";
        parametroLocalidade.ValorParametro = comissaoFuncionario.Localidade != null ? comissaoFuncionario.Localidade.Descricao + " (" + comissaoFuncionario.Localidade.Estado.Sigla + ")" : "";
        parametros.Add(parametroLocalidade);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroMotorista = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametroMotorista.NomeParametro = "Motorista";
        parametroMotorista.ValorParametro = comissaoFuncionario.Motorista != null ? comissaoFuncionario.Motorista.Nome + " (" + comissaoFuncionario.Motorista.CPF_Formatado + ")" : "Todos";
        parametros.Add(parametroMotorista);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroComissaoPorCargo = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametroComissaoPorCargo.NomeParametro = "ComissaoPorCargo";
        parametroComissaoPorCargo.ValorParametro = comissaoGeracaPorCargo ? "SIM" : "NAO";
        parametros.Add(parametroComissaoPorCargo);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = comissoesFuncionarioMotorista,
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                         Key = "ComissaoMotoristas_Abastecimento",
                         DataSet = comissaoFuncionarioMotoristaAbastecimento
                    }
                },
            Parameters = parametros
        };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\RH\ComissaoMotoristas.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return report;
    }
}