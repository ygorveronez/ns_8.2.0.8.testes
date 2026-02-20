using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Seguros
{
    public class FechamentoAverbacao : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao>
    {
        public FechamentoAverbacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao>();
            var result = from obj in query select obj;
            int ultimoNumero = result.Count() > 0 ? result.Select(o => o.Numero).Max() : 0;
            return ++ultimoNumero;
        }
        private IQueryable<Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao> _Consultar(int numeroInicial, int numeroFinal, int transportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao>();

            var result = from obj in query select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (transportador > 0)
                result = result.Where(o => o.Transportador.Codigo == transportador);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao> Consultar(int numeroInicial, int numeroFinal, int transportador, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(numeroInicial, numeroFinal, transportador);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int numeroInicial, int numeroFinal, int transportador)
        {
            var result = _Consultar(numeroInicial, numeroFinal, transportador);

            return result.Count();
        }
    }
}
