using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pessoas
{
    public class ConfiguracaoFTPImportacaoSupervisor : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoSupervisor>
    {
        public ConfiguracaoFTPImportacaoSupervisor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoSupervisor BuscarConfiguracaoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoSupervisor>();

            var result = from obj in query  select obj;

            return result.FirstOrDefault();
        }
    }
}
