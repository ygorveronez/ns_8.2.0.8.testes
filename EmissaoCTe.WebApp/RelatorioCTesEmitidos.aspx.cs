using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;


namespace EmissaoCTe.WebApp
{
    public partial class RelatorioCTesEmitidos : PaginaBaseSegura
    {
        #region Manipuladores de Eventos

        new protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PreencherDDLSerie();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherDDLSerie()
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
            List<Dominio.Entidades.EmpresaSerie> series = repSerie.BuscarTodosPorEmpresa(this.EmpresaUsuario.Codigo, Dominio.Enumeradores.TipoSerie.CTe);

            foreach (Dominio.Entidades.EmpresaSerie serie in series)
            {
                if (serie.Status == "A")
                    this.ddlSerie.Items.Add(new ListItem(serie.Numero.ToString(), serie.Codigo.ToString()));
            }
        }

        #endregion
    }
}