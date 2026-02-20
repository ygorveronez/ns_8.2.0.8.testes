using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace EmissaoCTe.WebApp
{
    public partial class ImportarCTes : PaginaBaseSegura
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

        #region Metodos

        private void PreencherDDLSerie()
        {
            List<Dominio.Entidades.EmpresaSerie> listaSeries = new List<Dominio.Entidades.EmpresaSerie>();

            if (this.Usuario.Series.Count > 0)
                listaSeries = this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe && o.Status == "A").ToList();
            else
            {
                using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
                Repositorio.EmpresaSerie repositorioSerie = new Repositorio.EmpresaSerie(unitOfWork);

                listaSeries = repositorioSerie.BuscarTodosPorEmpresa(this.EmpresaUsuario.Codigo, Dominio.Enumeradores.TipoSerie.CTe, "A");
            }

            foreach (Dominio.Entidades.EmpresaSerie serie in listaSeries)
            {
                this.ddlSerieFiltro.Items.Add(new ListItem(serie.Numero.ToString(), serie.Codigo.ToString()));
            }
        }

        #endregion
    }
}