using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CTe
{
    public class ObservacaoContribuinte : RepositorioBase<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte>
    {
        public ObservacaoContribuinte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte> BuscarAtivos()
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte>();

            query = query.Where(o => o.Ativo == true && o.Carga == null);
            
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);

            return query
                .Fetch(obj => obj.Carga)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte> _Consultar(string texto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte>();

            var result = from obj in query where obj.Carga == null select obj;
            
            // Filtros
            if (!string.IsNullOrWhiteSpace(texto))
                result = result.Where(o => o.Texto.Contains(texto));


            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo == true);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => o.Ativo == false);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte> Consultar(string texto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(texto, status);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string texto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(texto, status);

            return result.Count();
        }

    }
}
