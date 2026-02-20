using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pedidos
{
    public class ConfiguracaoFTPImportacaoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoFTPImportacaoPedido>
    {
        public ConfiguracaoFTPImportacaoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoFTPImportacaoPedido BuscarConfiguracaoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoFTPImportacaoPedido>();

            var result = from obj in query  select obj;

            return result.FirstOrDefault();
        }
    }
}
