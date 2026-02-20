using NHibernate.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoIntegracaoAutenticacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao>
    {
        public TipoIntegracaoAutenticacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao>();

            var result = from obj in query where obj.Tipo == tipo select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao> BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao>();

            var result = from obj in query where obj.Tipo == tipo select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
