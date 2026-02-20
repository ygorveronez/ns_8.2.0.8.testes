using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioVeiculosController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("relatorioveiculos.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão de acesso negada!");


                string tipoPropriedade = Request.Params["TipoPropriedade"];
                string tipoVeiculo = Request.Params["TipoVeiculo"];
                string tipoRodado = Request.Params["TipoRodado"];
                string tipoCarroceria = Request.Params["TipoCarroceria"];
                string status = Request.Params["Status"];
                string tipoArquivo = Request.Params["TipoArquivo"];
                string tipoRelatorio = Request.Params["TipoRelatorio"];

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.ObterRelatorio(this.EmpresaUsuario.Codigo, tipoPropriedade, tipoVeiculo, tipoRodado, tipoCarroceria, status, tipoRelatorio == "RelatorioVeiculosCompleto" ? true : false);

                List<Microsoft.Reporting.WebForms.ReportParameter> parametros = new List<Microsoft.Reporting.WebForms.ReportParameter>();
                parametros.Add(new Microsoft.Reporting.WebForms.ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));

                List<Microsoft.Reporting.WebForms.ReportDataSource> dataSources = new List<Microsoft.Reporting.WebForms.ReportDataSource>();
                dataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Veiculos", veiculos));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/" + tipoRelatorio + ".rdlc", tipoArquivo, parametros, dataSources, (object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e) =>
                {
                    if (e.ReportPath.Contains("RelatorioVeiculosCompleto_Vinculos"))
                    {
                        IList<Dominio.Entidades.Veiculo> listaVeiculos = (from obj in veiculos where obj.Codigo == int.Parse(e.Parameters["CodigoVeiculo"].Values[0]) select obj.VeiculosVinculados).FirstOrDefault();
                        e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("VeiculosVinculados", listaVeiculos));
                    }
                });

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioVeiculos." + arquivo.FileNameExtension);
                return Json<bool>(false, false, "entrou.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório de veículos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
