using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class AjusteTabelaFreteSimulacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacao>
    {
        public AjusteTabelaFreteSimulacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacao>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacao BuscarPorAjuste(int codigoAjuste)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacao>();

            query = query.Where(o => o.Ajuste.Codigo == codigoAjuste);

            return query.FirstOrDefault();
        }
    }
}
