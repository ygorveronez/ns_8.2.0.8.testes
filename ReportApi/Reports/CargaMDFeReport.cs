using ReportApi.Attributes;
using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using ReportApi.DTO;
using System.Runtime.Remoting;

namespace ReportApi.Reports
{
    [UseReportType(Dominio.ObjetosDeValor.Relatorios.ReportType.CargaMDFe)]
    public sealed class CargaMDFeReport : ReportBase
    {
        public CargaMDFeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
        {
        }

        public override ReportResult InternalProcess(Dictionary<string, string> extraData)
        {
            int codigoMDFe = extraData.GetValue<int>("codigoMDFe");
            bool contingencia = extraData.GetValue<bool>("contingencia");
            
            try
            {
                var relatorioDAMDFE = new ReportApi.ReportService.DAMDFE();

                var resultByte = relatorioDAMDFE.Gerar(codigoMDFe, _unitOfWork, contingencia);

                return PrepareReportResult(FileType.PDF, resultByte);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw new ServerException($"{ex.Message} - {ex.StackTrace}");
            }
        }          
    }
}