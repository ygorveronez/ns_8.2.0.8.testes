using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatoriosGeraisController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult RelatorioContadores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                int.TryParse(Request.Params["Empresa"], out int codigoEmpresa);
                string tipoArquivo = Request.Params["Arquivo"];

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                IList<Dominio.ObjetosDeValor.Relatorios.RelatorioContadores> listaContadores = repEmpresa.RelatorioContadores(codigoEmpresa);
                
                List<ReportDataSource> dataSources = new List<ReportDataSource>
                {
                    new ReportDataSource("Contadores", listaContadores)
                };

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioContadores.rdlc", tipoArquivo, null, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioContadores." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
