using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.WebAdmin
{
    public partial class RelatorioCTesCancelados : PaginaBaseSegura
    {
        #region Manipuladores de Eventos

        new protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            int codigoEmpresa = 0;
            DateTime dataInicial, dataFinal;
            int.TryParse(this.hddCodigoEmpresa.Value, out codigoEmpresa);
            DateTime.TryParseExact(this.txtDataInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
            DateTime.TryParseExact(this.txtDataFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

            int numeroSerie = 0;
            int.TryParse(this.txtSerie.Text, out numeroSerie);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesCancelados> listaCTesCancelados = repCTe.RelatorioCTesCancelados(this.EmpresaUsuario.Codigo, codigoEmpresa, dataInicial, dataFinal, numeroSerie);

            this.rvwRelatorioCTesCancelados.LocalReport.SetParameters(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
            this.rvwRelatorioCTesCancelados.LocalReport.SetParameters(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
            this.rvwRelatorioCTesCancelados.LocalReport.SetParameters(new ReportParameter("EmpresaEmissora", empresa != null ? empresa.RazaoSocial : string.Empty));
            this.rvwRelatorioCTesCancelados.LocalReport.SetParameters(new ReportParameter("Serie", numeroSerie > 0 ? numeroSerie.ToString() : "Toddas"));

            this.rvwRelatorioCTesCancelados.LocalReport.DataSources.Clear();
            this.rvwRelatorioCTesCancelados.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CTesCancelados", listaCTesCancelados));

            this.rvwRelatorioCTesCancelados.DataBind();
        }

        #endregion

        #region Métodos Privados


        #endregion
    }
}