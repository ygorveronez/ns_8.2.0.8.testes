using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioHistoricosPneusController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            int codigoVeiculo, codigoPneu = 0;
            int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
            int.TryParse(Request.Params["Pneu"], out codigoPneu);

            string tipoArquivo = Request.Params["TipoArquivo"];

            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            Dominio.Entidades.Pneu pneu = repPneu.BuscarPorCodigo(codigoPneu, this.EmpresaUsuario.Codigo);            
            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
            
            Repositorio.HistoricoPneu repHistoricoPneu = new Repositorio.HistoricoPneu(unitOfWork);
            List<Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosPneus> listaHistoricoPneu = repHistoricoPneu.RelatorioHistoricos(this.EmpresaUsuario.Codigo, codigoVeiculo, codigoPneu);

            List<ReportParameter> parametros = new List<ReportParameter>();
            parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
            parametros.Add(new ReportParameter("Pneu", pneu != null ? string.Concat(pneu.Serie, " - ", pneu.ModeloPneu.Descricao) : "Todos"));
            parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : "Todos"));

            List<ReportDataSource> dataSources = new List<ReportDataSource>();
            dataSources.Add(new ReportDataSource("Historicos", listaHistoricoPneu));

            Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

            Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioHistoricosPneus.rdlc", tipoArquivo, parametros, dataSources);

            return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioHistoricosPneus." + arquivo.FileNameExtension.ToLower());
        }
    }
}
