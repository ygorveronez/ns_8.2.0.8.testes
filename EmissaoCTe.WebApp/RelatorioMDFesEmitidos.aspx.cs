using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace EmissaoCTe.WebApp
{
    public partial class RelatorioMDFesEmitidos : PaginaBaseSegura
    {
        #region Manipuladores de Eventos

        new protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PreencherEstados();
            }
        }
        
        #endregion

        #region Métodos Privados

        private void PreencherEstados()
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

            List<Dominio.Entidades.Estado> estados = repEstado.BuscarTodos();

            this.selUFCarregamento.Items.AddRange((from obj in estados select new ListItem { Text = obj.Sigla + " - " + obj.Nome, Value = obj.Sigla }).ToArray());
            this.selUFDescarregamento.Items.AddRange((from obj in estados select new ListItem { Text = obj.Sigla + " - " + obj.Nome, Value = obj.Sigla }).ToArray());
        }

        #endregion
    }
}