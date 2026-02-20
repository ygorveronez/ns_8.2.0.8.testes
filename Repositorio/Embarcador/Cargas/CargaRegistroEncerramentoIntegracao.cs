using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaRegistroEncerramentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao>
    {
        public CargaRegistroEncerramentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao> BuscarPorCargaEncerramento(int codigoCargaEncerramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao>();

            query = query.Where(o => o.CargaRegistroEncerramento.Codigo == codigoCargaEncerramento);

            return query.Fetch(o => o.TipoIntegracao).ToList();
        }



        public bool ExisteIntegracao(int tipoIntegracao, int codigoCargaRegistroEncerramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao>();

            query = query.Where(o => o.CargaRegistroEncerramento.Codigo == codigoCargaRegistroEncerramento && o.TipoIntegracao.Codigo == tipoIntegracao);

            return query.Any();
        }



    }
}
