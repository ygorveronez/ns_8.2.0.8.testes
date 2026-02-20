using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioCTesRedespachoController : ApiController
    {
        #region Métodos Públicos

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Relatorio servicoRelatorio = new Servicos.Relatorio(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeRedespacho filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesRedespacho> dados = repositorioConhecimentoDeTransporteEletronico.RelatorioCTesRedespacho(filtrosPesquisa);

                string tipoArquivo = Request.Params["Arquivo"];
                List<ReportParameter> parametros = new List<ReportParameter>();
                List<ReportDataSource> dataSources = new List<ReportDataSource> { new ReportDataSource("CTes", dados) };

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = servicoRelatorio.GerarWeb("Relatorios/RelatorioCTesRedespacho" + ".rdlc", tipoArquivo, parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, string.Concat("RelatorioCTesRedespacho", '.', arquivo.FileNameExtension.ToLower()));
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

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeRedespacho ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            int codigoEmpresa = 0;
            int.TryParse(Request.Params["Empresa"], out codigoEmpresa);
            string cnpjEmbarcadorUsuario = this.Usuario.CNPJEmbarcador ?? string.Empty;

            DateTime dataInicial, dataFinal;
            DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
            DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

            double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CnpjEmbarcador"]), out double cnpjEmbarcador);

            string serieEmissao = Request.Params["SerieEmissaoCte"];

            Dominio.Entidades.Cliente cliente = cnpjEmbarcador > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cnpjEmbarcador) : null;

            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeRedespacho
            {
                CodigoEmpresa = codigoEmpresa,
                CodigoEmpresaPai = this.EmpresaUsuario.Codigo,
                TipoAmbiente = this.EmpresaUsuario.TipoAmbiente,
                CpfCnpjEmbarcador = cliente?.CPF_CNPJ_SemFormato ?? string.Empty,
                CpfCnpjEmbarcadorUsuario = cnpjEmbarcadorUsuario,
                SerieEmissao = serieEmissao,
                DataEmissaoFinal = dataFinal,
                DataEmissaoInicial = dataInicial
            };
        }

        #endregion Métodos Privados
    }
}
