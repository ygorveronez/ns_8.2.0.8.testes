using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;

namespace EmissaoCTe.WebAdmin
{
    public partial class RelatorioMDFesEmitidos : PaginaBaseSegura
    {
        #region Manipuladores de Eventos

        new protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            int codigoEmpresa = 0;
            int.TryParse(this.hddCodigoEmpresa.Value, out codigoEmpresa);
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            DateTime dataInicial, dataFinal, dataAutorizacaoInicial, dataAutorizacaoFinal;
            DateTime.TryParseExact(this.txtDataInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
            DateTime.TryParseExact(this.txtDataFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);
            DateTime.TryParseExact(this.txtDataAutorizacaoInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoInicial);
            DateTime.TryParseExact(this.txtDataAutorizacaoFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoFinal);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            IList<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFeAgrupado> listaMDFesAgrupados = repMDFe.RelatorioMDFesAgrupados(this.EmpresaUsuario.Codigo, codigoEmpresa, dataInicial, dataFinal, dataAutorizacaoInicial, dataAutorizacaoFinal);

            this.rvwRelatorioMDFesEmitidos.LocalReport.SetParameters(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
            this.rvwRelatorioMDFesEmitidos.LocalReport.SetParameters(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
            this.rvwRelatorioMDFesEmitidos.LocalReport.SetParameters(new ReportParameter("EmpresaEmissora", empresa != null ? empresa.RazaoSocial : string.Empty));

            this.rvwRelatorioMDFesEmitidos.LocalReport.DataSources.Clear();
            this.rvwRelatorioMDFesEmitidos.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("MDFesAgrupados", listaMDFesAgrupados));

            this.rvwRelatorioMDFesEmitidos.DataBind();
        }

        #endregion
    }
}