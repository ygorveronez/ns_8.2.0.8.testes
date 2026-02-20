using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioPagamentoMotoristaMDFeController : ApiController
    {

        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("relatoriopagamentomotoristamdfe.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão para download do relatório negada!");

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int codigoMotorista = 0;
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);

                string status = Request.Params["Status"];
                string tipoArquivo = Request.Params["TipoArquivo"];

                Repositorio.PagamentoMotoristaMDFe repPagamentoMotorista = new Repositorio.PagamentoMotoristaMDFe(unitOfWork);
                List<Dominio.Entidades.PagamentoMotoristaMDFe> listaPagamentos = repPagamentoMotorista.Relatorio(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, codigoMotorista, status);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Status", DescricaoStatus(status)));
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial != DateTime.MinValue ? dataInicial.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataFinal != DateTime.MinValue ? dataFinal.ToString("dd/MM/yyyy") : "99/99/9999")));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Pagamentos", listaPagamentos));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioPagamentoMotoristaMDFe.rdlc", tipoArquivo, parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioPagamentoMotoristas." + arquivo.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar relatório!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        private string DescricaoStatus(string status)
        {
            Dominio.Entidades.PagamentoMotoristaMDFe aux = new Dominio.Entidades.PagamentoMotoristaMDFe();
            aux.Status = status;

            return string.IsNullOrWhiteSpace(status) ? "Todos" : aux.DescricaoStatus;
        }
    }
}
