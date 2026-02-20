using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ConfiguracaoTipoOperacaoEmissaoDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissaoDocumento>
    {
        public ConfiguracaoTipoOperacaoEmissaoDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public bool ExisteConfiguracaoNaoAvancarCargaSemValePedagio()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissaoDocumento>();
            query = query.Where(t => t.NaoPermitirLiberarSemValePedagio);
            return query.FirstOrDefault() != null;
        }
    }
}
