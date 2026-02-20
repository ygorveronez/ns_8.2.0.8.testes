using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.WebAdmin
{
    public partial class RelatorioCTesEmitidos : PaginaBaseSegura
    {
        #region Manipuladores de Eventos

        new protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);


            int.TryParse(this.hddCodigoEmpresa.Value, out int codigoEmpresa);
            int.TryParse(this.txtSerie.Text, out int numeroSerie);
            int.TryParse(this.selTipoEmissao.Text, out int tipoEmissao);

            Dominio.Enumeradores.EmissaoPor? emissaoPor = null;
            if (Enum.TryParse(this.selEmissaoPor.Text, out Dominio.Enumeradores.EmissaoPor emissaoPorAux))
                emissaoPor = emissaoPorAux;

            bool.TryParse(this.selRealtorioGrafico.Text, out bool realtorioGrafico);

            DateTime.TryParseExact(this.txtDataInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(this.txtDataFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);
            DateTime.TryParseExact(this.txtDataAutorizacaoInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataAutorizacaoInicial);
            DateTime.TryParseExact(this.txtDataAutorizacaoFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataAutorizacaoFinal);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            string parametroSerieTipoEmissao = "";
            parametroSerieTipoEmissao = numeroSerie > 0 ? numeroSerie.ToString() : "Todas";
            parametroSerieTipoEmissao = string.Concat(parametroSerieTipoEmissao, " - Tipo Emissao: ", this.selTipoEmissao.SelectedItem.Text);

            if (!realtorioGrafico)
            {
                IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupado> listaCTesAgrupados = repCTe.RelatorioCTesAgrupados(this.EmpresaUsuario.Codigo, codigoEmpresa, dataInicial, dataFinal, dataAutorizacaoInicial, dataAutorizacaoFinal, emissaoPor, numeroSerie, tipoEmissao);


                this.rvwRelatorioCTesEmitidos.LocalReport.SetParameters(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                this.rvwRelatorioCTesEmitidos.LocalReport.SetParameters(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
                this.rvwRelatorioCTesEmitidos.LocalReport.SetParameters(new ReportParameter("EmpresaEmissora", empresa != null ? empresa.RazaoSocial : string.Empty));
                this.rvwRelatorioCTesEmitidos.LocalReport.SetParameters(new ReportParameter("Serie", parametroSerieTipoEmissao));

                this.rvwRelatorioCTesEmitidos.LocalReport.DataSources.Clear();
                this.rvwRelatorioCTesEmitidos.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CTesAgrupados", listaCTesAgrupados));

                this.rvwRelatorioCTesEmitidos.DataBind();

                this.rvwRelatorioCTesEmitidos.Style.Remove("display");
                this.rvwRelatorioCTesEmitidosGrafico.Style.Add("display", "none");
            }
            else
            {
                List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupadoGrafico> listaCTesAgrupados = repCTe.GraficosCTesAgrupados(this.EmpresaUsuario.Codigo, codigoEmpresa, dataInicial, dataFinal, dataAutorizacaoInicial, dataAutorizacaoFinal, emissaoPor, numeroSerie, tipoEmissao);

                this.rvwRelatorioCTesEmitidosGrafico.LocalReport.SetParameters(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                this.rvwRelatorioCTesEmitidosGrafico.LocalReport.SetParameters(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
                this.rvwRelatorioCTesEmitidosGrafico.LocalReport.SetParameters(new ReportParameter("EmpresaEmissora", empresa != null ? empresa.RazaoSocial : string.Empty));
                this.rvwRelatorioCTesEmitidosGrafico.LocalReport.SetParameters(new ReportParameter("Serie", parametroSerieTipoEmissao));

                this.rvwRelatorioCTesEmitidosGrafico.LocalReport.DataSources.Clear();
                this.rvwRelatorioCTesEmitidosGrafico.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CTesAgrupados", listaCTesAgrupados));

                this.rvwRelatorioCTesEmitidosGrafico.DataBind();

                this.rvwRelatorioCTesEmitidosGrafico.Style.Remove("display");
                this.rvwRelatorioCTesEmitidos.Style.Add("display", "none");
            }
        }

        #endregion
    }
}