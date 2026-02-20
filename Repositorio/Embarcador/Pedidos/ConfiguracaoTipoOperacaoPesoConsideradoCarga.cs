using System.Linq;
namespace Repositorio.Embarcador.Pedidos
{
    public class ConfiguracaoTipoOperacaoPesoConsideradoCarga: RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga>
    {
        public ConfiguracaoTipoOperacaoPesoConsideradoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga BuscarPorTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga>();

            query = query.Where(o => o.TipoOperacao == tipoOperacao);

            return query.FirstOrDefault();
        }

    }
}
