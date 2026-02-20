using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pessoas
{
    public class ConfiguracaoFTPImportacaoVendedorCliente : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedorCliente>
    {
        public ConfiguracaoFTPImportacaoVendedorCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedorCliente BuscarConfiguracaoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedorCliente>();

            var result = from obj in query  select obj;

            return result.FirstOrDefault();
        }
    }
}
