using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class MotivoChamadoArvore : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore>
    {
        public MotivoChamadoArvore(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MotivoChamadoArvore(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore> BuscarPorCodigoMotivoChamado(int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore>();

            if (codigoMotivoChamado > 0)
                query = query.Where(motivo => motivo.MotivoChamado.Codigo == codigoMotivoChamado);

            return query.ToList();

        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore>> BuscarPorCodigoMotivoChamadoAsync(int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore>();

            if (codigoMotivoChamado > 0)
                query = query.Where(motivo => motivo.MotivoChamado.Codigo == codigoMotivoChamado);

            return query.ToListAsync(CancellationToken);
        }
    }
}
