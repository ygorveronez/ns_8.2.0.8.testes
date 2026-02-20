using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Filiais
{
    public class ConfiguracaoGestaoPatio : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio>
    {

        public ConfiguracaoGestaoPatio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfiguracaoGestaoPatio(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio BuscarConfiguracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio>();
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }
    }
}
