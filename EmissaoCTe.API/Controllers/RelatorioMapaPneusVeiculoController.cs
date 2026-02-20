using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioMapaPneusVeiculoController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoVeiculo, codigoPneu = 0;
                int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["Pneu"], out codigoPneu);

                string tipoArquivo = Request.Params["TipoArquivo"];

                Repositorio.Pneu repPneu = new Repositorio.Pneu(unidadeDeTrabalho);
                Dominio.Entidades.Pneu pneu = repPneu.BuscarPorCodigo(codigoPneu, this.EmpresaUsuario.Codigo);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);

                if (veiculo == null && pneu != null)
                    veiculo = pneu.Veiculo;

                Repositorio.HistoricoPneu repHistoricoPneu = new Repositorio.HistoricoPneu(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosPneus> listaHistoricoPneu = repHistoricoPneu.Relatorio(this.EmpresaUsuario.Codigo, veiculo != null ? veiculo.Codigo : 0);
                List<Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosPneusMapa> listaMapaPneu = repPneu.RelatorioMapa(this.EmpresaUsuario.Codigo, veiculo != null ? veiculo.Codigo : 0);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Pneu", pneu != null ? string.Concat(pneu.Serie, " - ", pneu.ModeloPneu.Descricao) : "Todos"));
                parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : "Todos"));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Historicos", listaHistoricoPneu));
                dataSources.Add(new ReportDataSource("Mapas", listaMapaPneu));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeDeTrabalho);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioMapaPneusVeiculo.rdlc", tipoArquivo, parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioMapaPneusVeiculo." + arquivo.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório mapa de pneus.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
