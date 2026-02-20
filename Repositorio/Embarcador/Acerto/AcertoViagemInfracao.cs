using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Acerto
{
    public class AcertoViagemInfracao : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao>
    {
        public AcertoViagemInfracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao> BuscarPorAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao BuscarPorAcertoInfracao(int codigo, int infracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo && obj.Codigo == infracao select obj;
            return result.FirstOrDefault();
        }

        public bool ContemInfracaoEmAcerto(int codigoInfracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao>();
            query = query.Where(c => c.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado && c.Infracao.Codigo == codigoInfracao);
            return query.Any();
        }

    }
}
