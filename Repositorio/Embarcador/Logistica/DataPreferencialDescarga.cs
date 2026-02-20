using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class DataPreferencialDescarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga>
    {
        public DataPreferencialDescarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga BuscarPorCentroDescarregamentoDiasBloqueioDiaPreferencial(double destinatarioCD, int diasBloqueio, int diaPreferencial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga>()
                .Where(obj => obj.CentroDescarregamento.Destinatario.CPF_CNPJ == destinatarioCD && obj.DiasBloqueio == diasBloqueio && obj.DiaPreferencial == diaPreferencial);

            return query
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDataPreferencialDescarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga> consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }
        
        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDataPreferencialDescarga filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            consulta = consulta
                .Fetch(obj => obj.CentroDescarregamento);
            
            return consulta.Count();
        }

        #region Métodos Privados
        
        private IQueryable<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDataPreferencialDescarga filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescarga>();

            if (filtrosPesquisa.CodigoCentroDescarregamento > 0)
                query = query.Where(obj => obj.CentroDescarregamento.Codigo == filtrosPesquisa.CodigoCentroDescarregamento);

            if (filtrosPesquisa.DiaPreferencial > 0)
                query = query.Where(obj => obj.DiaPreferencial == filtrosPesquisa.DiaPreferencial);

            return query;
        }

        #endregion
    }
}
