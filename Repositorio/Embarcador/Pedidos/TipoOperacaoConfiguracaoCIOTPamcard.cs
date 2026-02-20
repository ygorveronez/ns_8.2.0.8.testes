using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoConfiguracaoCIOTPamcard : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard>
    {
        public TipoOperacaoConfiguracaoCIOTPamcard(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard BuscarPorTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard>();

            query = query.Where(o => o.TipoOperacao == tipoOperacao);

            return query.FirstOrDefault();
        }
    }
}
