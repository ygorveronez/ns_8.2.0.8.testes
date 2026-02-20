using System;
using System.Collections.Generic;

namespace EmissaoCTe.WebApp
{
    public partial class CartaDeCorrecaoEletronica : PaginaBaseSegura
    {
        #region Manipuladores de Eventos

        new protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            this.GerarRelatorio();
        }

        #endregion

        #region Métodos Privados

        private void GerarRelatorio()
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            this.rvwRelatorioCCe.LocalReport.DataSources.Clear();

            int codigoCCe = 0;
            int.TryParse(this.hddCodigoCCe.Value, out codigoCCe);

            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);
            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unitOfWork);

            Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigoCCe);
            List<Dominio.Entidades.ItemCCe> itensCCe = repItemCCe.BuscarPorCCe(codigoCCe);

            this.rvwRelatorioCCe.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CCe", new Dominio.Entidades.CartaDeCorrecaoEletronica[] { cce }));
            this.rvwRelatorioCCe.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("ItensCCe", itensCCe));

            this.rvwRelatorioCCe.DataBind();

            this.hddCodigoCCe.Value = "0";
        }

        #endregion
    }
}