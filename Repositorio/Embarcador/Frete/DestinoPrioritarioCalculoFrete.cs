using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class DestinoPrioritarioCalculoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete>
    {

        public DestinoPrioritarioCalculoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(o => o.Ativo == (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, status);

            return result.Count();
        }
    }
}
