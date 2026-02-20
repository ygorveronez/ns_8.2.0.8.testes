using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GestaoEntregas
{
    public class PortalClientePerguntaAvaliacao : RepositorioBase<Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao>
    {

        public PortalClientePerguntaAvaliacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ProximaOrdem()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao>();

            var ordem = query.Max(o => (int?)o.Ordem);

            return (ordem ?? 0) + 1;
        }

        public List<Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao> BuscarTodasPerguntas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao>();
            
            var result = from obj in query orderby obj.Ordem ascending select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao> BuscarPerguntas(int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao>();
            var result = from obj in query select obj;

            result = result.OrderBy(o => o.Ordem);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);
            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarBuscaPerguntas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao>();
            var result = from obj in query select obj;

            return result.Count();
        }
    }
}
