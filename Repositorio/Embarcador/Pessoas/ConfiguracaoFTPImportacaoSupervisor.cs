using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pessoas
{
    public class ConfiguracaoFTPImportacaoVendedor : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedor>
    {
        public ConfiguracaoFTPImportacaoVendedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedor BuscarConfiguracaoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedor>();

            var result = from obj in query  select obj;

            return result.FirstOrDefault();
        }
    }
}
