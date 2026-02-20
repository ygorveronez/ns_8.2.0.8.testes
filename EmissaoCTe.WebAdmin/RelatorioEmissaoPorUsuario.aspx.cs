using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;

namespace EmissaoCTe.WebAdmin
{
    public partial class RelatorioEmissaoPorUsuario : PaginaBaseSegura
    {
        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            int.TryParse(this.hddCodigoUsuario.Value, out int codigoUsuario);

            bool.TryParse(this.selRealtorioGrafico.Text, out bool relatorioGrafico);

            DateTime.TryParseExact(this.txtDataInicial.Text, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(this.txtDataFinal.Text, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);

            IList<Dominio.ObjetosDeValor.Relatorios.RelatorioEmissaoPorUsuario> listaObjetos = repCTe.RelatorioEmissaoPorUsuario(this.EmpresaUsuario.Codigo, codigoUsuario, dataInicial, dataFinal, !relatorioGrafico);

            if (relatorioGrafico)
            {
                this.rvwRelatorioEmissaoPorUsuarioGrafico.LocalReport.SetParameters(new ReportParameter("Usuario", usuario != null ? usuario.Nome : "Todos"));
                this.rvwRelatorioEmissaoPorUsuarioGrafico.LocalReport.SetParameters(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy HH:mm"), " até ", dataFinal.ToString("dd/MM/yyyy HH:mm"))));

                this.rvwRelatorioEmissaoPorUsuarioGrafico.LocalReport.DataSources.Clear();
                this.rvwRelatorioEmissaoPorUsuarioGrafico.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("RelatorioPorUsuario", listaObjetos));

                this.rvwRelatorioEmissaoPorUsuarioGrafico.DataBind();

                this.rvwRelatorioEmissaoPorUsuarioGrafico.Style.Remove("display");
                this.rvwRelatorioEmissaoPorUsuarioDetalhado.Style.Add("display", "none");
            }
            else
            {
                this.rvwRelatorioEmissaoPorUsuarioDetalhado.LocalReport.SetParameters(new ReportParameter("Usuario", usuario != null ? usuario.Nome : "Todos"));
                this.rvwRelatorioEmissaoPorUsuarioDetalhado.LocalReport.SetParameters(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy HH:mm"), " até ", dataFinal.ToString("dd/MM/yyyy HH:mm"))));

                this.rvwRelatorioEmissaoPorUsuarioDetalhado.LocalReport.DataSources.Clear();
                this.rvwRelatorioEmissaoPorUsuarioDetalhado.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("RelatorioPorUsuario", listaObjetos));

                this.rvwRelatorioEmissaoPorUsuarioDetalhado.DataBind();

                this.rvwRelatorioEmissaoPorUsuarioDetalhado.Style.Remove("display");
                this.rvwRelatorioEmissaoPorUsuarioGrafico.Style.Add("display", "none");
            }
        }
    }
}