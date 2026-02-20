using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Utilidades.Extensions;

namespace ReportApi.Reports
{
    [UseReportType(ReportType.EtiquetaDeposito)]
    public class DepositoReport : ReportBase
    {

        public DepositoReport(UnitOfWork unitOfWork, Servicos.Embarcador.Relatorios.RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
        {
        }

        public override ReportResult InternalProcess(Dictionary<string, string> extraData)
        {
            var nomeCliente = extraData.GetValue<string>("NomeCliente"); 
            var nomeEtiqueta = extraData.GetValue<string>("NomeEtiqueta"); 
            var dadosEtiquetaString = extraData.GetValue<string>("DadosEtiqueta"); 
            var dadosEtiqueta = dadosEtiquetaString.FromJson<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito>();

            var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

            var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
            empresa.NomeParametro = "Empresa";
            empresa.ValorParametro = nomeCliente;
            parametros.Add(empresa);

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosEtiqueta,
                Parameters = parametros
            };
            CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\WMS\" + nomeEtiqueta, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
            
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "WMS/Deposito", _unitOfWork);
            
            return PrepareReportResult(FileType.PDF);  
            
        }
    }
}