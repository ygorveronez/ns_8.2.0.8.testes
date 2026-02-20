using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class VistoriaCheckList : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList>
    {
        public VistoriaCheckList(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaVistoriaCheckList filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaVistoriaCheckList filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaVistoriaCheckList filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.CheckList>0)
                result = result.Where(obj => obj.Checklist.Codigo == filtrosPesquisa.CheckList);

            result = result.Where(o => o.Status == filtrosPesquisa.Status);


            if (filtrosPesquisa.ModeloVeicular > 0)
               result = result.Where(o => o.ModelosVeiculares.Any( m => m.Codigo ==  filtrosPesquisa.ModeloVeicular));

            return result;
        }

        #endregion
    }
}
