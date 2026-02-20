using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasIntegracaoFTP : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP>
    {
        public GrupoPessoasIntegracaoFTP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public GrupoPessoasIntegracaoFTP(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Task<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP> BuscarPorGrupoPessoasAsync(int codigoGrupoPessoas, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP>();
            query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
