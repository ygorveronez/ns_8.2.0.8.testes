using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.InternalWebAdmin
{
    public partial class RelatorioCTesEmitidos : PaginaBaseSegura
    {
        #region Manipuladores de Eventos

        new protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoEmpresa = 0;
                int.TryParse(this.hddCodigoEmpresa.Value, out codigoEmpresa);

                DateTime dataInicial, dataFinal, dataAutorizacaoInicial, dataAutorizacaoFinal;
                DateTime.TryParseExact(this.txtDataInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(this.txtDataFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);
                DateTime.TryParseExact(this.txtDataAutorizacaoInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoInicial);
                DateTime.TryParseExact(this.txtDataAutorizacaoFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoFinal);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupado> listaCTesAgrupados = repCTe.RelatorioCTesAgrupadosEmpresasPai(this.EmpresaUsuario.Codigo, codigoEmpresa, dataInicial, dataFinal, dataAutorizacaoInicial, dataAutorizacaoFinal);

                this.rvwRelatorioCTesEmitidos.LocalReport.SetParameters(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                this.rvwRelatorioCTesEmitidos.LocalReport.SetParameters(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
                this.rvwRelatorioCTesEmitidos.LocalReport.SetParameters(new ReportParameter("EmpresaPai", empresa != null ? empresa.RazaoSocial : string.Empty));

                this.rvwRelatorioCTesEmitidos.LocalReport.DataSources.Clear();
                this.rvwRelatorioCTesEmitidos.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CTesAgrupados", listaCTesAgrupados));

                this.rvwRelatorioCTesEmitidos.DataBind();
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}