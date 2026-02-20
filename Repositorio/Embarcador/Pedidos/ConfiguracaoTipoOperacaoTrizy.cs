using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ConfiguracaoTipoOperacaoTrizy : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy>
    {
        public ConfiguracaoTipoOperacaoTrizy(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            query = query.Where(obj => obj.Codigo == codigoTipoOperacao);
            return query.Select(obj => obj.ConfiguracaoTrizy).FirstOrDefault();
        }
    }
}
