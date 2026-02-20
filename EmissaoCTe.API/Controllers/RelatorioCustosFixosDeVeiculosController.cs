using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioCustosFixosDeVeiculosController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int codigoVeiculo, codigoTipoCustoFixo = 0;
                int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["TipoCustoFixo"], out codigoTipoCustoFixo);

                string tipoArquivo = Request.Params["TipoArquivo"];

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.TipoDeCustoFixo repTipoCustoFixo = new Repositorio.TipoDeCustoFixo(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
                Dominio.Entidades.TipoDeCustoFixo tipoCustoFixo = repTipoCustoFixo.BuscarPorCodigo(codigoTipoCustoFixo, this.EmpresaUsuario.Codigo);

                Repositorio.CustoFixo repCustoFixo = new Repositorio.CustoFixo(unitOfWork);
                List<Dominio.Entidades.CustoFixo> listaObjetos = repCustoFixo.Relatorio(this.EmpresaUsuario.Codigo, codigoVeiculo, codigoTipoCustoFixo, dataInicial, dataFinal);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
                parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : string.Empty));
                parametros.Add(new ReportParameter("TipoCustoFixo", tipoCustoFixo != null ? tipoCustoFixo.Descricao : string.Empty));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("CustosFixos", listaObjetos));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCustosFixos.rdlc", tipoArquivo, parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioCustosFixosVeiculos." + arquivo.FileNameExtension);
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
