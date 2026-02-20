using NHibernate.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.WebService
{
    public class IntegradoraIntegracao : RepositorioBase<Dominio.Entidades.WebService.IntegradoraIntegracao>
    {
        #region Construtores

        public IntegradoraIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegradoraIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Task<List<Dominio.Entidades.WebService.IntegradoraIntegracao>> BuscarPorIntegradoraAsync(int codigo)
        {
            IQueryable<Dominio.Entidades.WebService.IntegradoraIntegracao> consultaIntegradoraIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.IntegradoraIntegracao>()
                .Where(integracao => integracao.Integradora.Codigo == codigo);

            return consultaIntegradoraIntegracao.ToListAsync(CancellationToken);
        }

        #endregion Métodos Públicos
    }
}
