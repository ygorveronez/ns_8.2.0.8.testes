using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ConfiguracaoTipoOperacaoControleEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega>
    {
        public ConfiguracaoTipoOperacaoControleEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            query = query.Where(obj => obj.Codigo == codigoTipoOperacao);
            return query.Select(obj => obj.ConfiguracaoControleEntrega).FirstOrDefault();
        }
    }
}