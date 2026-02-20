using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Storage;
using Servicos.Embarcador.Relatorios;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;

namespace ReportApi.Reports
{
    [UseReportType(ReportType.CCe)]
    public sealed class CCeReport : ReportBase
    {
        public CCeReport(Repositorio.UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
        {            
        }

        public override ReportResult InternalProcess(Dictionary<string, string> extraData)
        {
            int codigoCCe = extraData.GetValue<int>("codigoCCe");

            try
            {
                Repositorio.CartaDeCorrecaoEletronica repositorioCartaDeCorrecaoEletronica = new Repositorio.CartaDeCorrecaoEletronica(_unitOfWork);
                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repositorioCartaDeCorrecaoEletronica.BuscarPorCodigo(codigoCCe);
                ReportApi.ReportService.CCe relatorioCCe = new ReportApi.ReportService.CCe();
                byte[] pdf = relatorioCCe.ObterRelatorio(cce, _unitOfWork);

                return PrepareReportResult(FileType.PDF, pdf);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                throw new ServerException($"{excecao.Message} - {excecao.StackTrace}");
            }
        }
    }
}
