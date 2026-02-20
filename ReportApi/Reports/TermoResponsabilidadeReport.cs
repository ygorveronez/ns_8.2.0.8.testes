using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;


namespace ReportApi.Reports;

[UseReportType(ReportType.TermoResponsabilidade)]
public class TermoResponsabilidadeReport : ReportBase
{
    private readonly BemReportService _bemReportService;
    
    public TermoResponsabilidadeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage, BemReportService bemReportService) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
        _bemReportService = bemReportService;
    }


    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var information = extraData.GetInfo(); 
        
        var empresa = BuscarEmpresa(extraData.GetValue<int>("CodigoEmpresa"));

        int codigoTransferencia = extraData.GetValue<int>("CodigoTransferencia");
        var dadosBem = extraData.GetValue<string>("DadosBem").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoResponsabilidade>>();
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        GerarRelatorioTermoResponsabilidade(empresa, codigoTransferencia, dadosBem, _unitOfWork.StringConexao, relatorioControleGeracao);
        
        return PrepareReportResult(FileType.PDF);
    }
    
    private void GerarRelatorioTermoResponsabilidade(Dominio.Entidades.Empresa empresa, int codigoTransferencia, List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoResponsabilidade> dadosBem, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
    {
        try
        {
            string mensagem = "";

            CrystalDecisions.CrystalReports.Engine.ReportDocument report = _bemReportService.GerarRelatorioTermoResponsabilidade(_unitOfWork, dadosBem, empresa.CaminhoLogoDacte, out mensagem);
            if (string.IsNullOrWhiteSpace(mensagem))
            {
                if (codigoTransferencia > 0)
                    _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Patrimonio/TransferenciaBem", _unitOfWork);
                else
                    _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Patrimonio/Bem", _unitOfWork);
            }
            else
            {
                _servicoRelatorioReportService.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, _unitOfWork, mensagem);
            }
        }
        catch (Exception ex)
        {
            _servicoRelatorioReportService.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, _unitOfWork, ex);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }

   
}