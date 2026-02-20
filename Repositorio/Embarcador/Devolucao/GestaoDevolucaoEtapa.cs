using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucaoEtapa : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa>
    {
        public GestaoDevolucaoEtapa(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public GestaoDevolucaoEtapa(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }


        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa BuscarEtapaPorEtapaECodigoGestao(long codigoGestao, EtapaGestaoDevolucao etapaGestaoDevolucao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa>();
            query = query.Where(o => o.GestaoDevolucao.Codigo == codigoGestao && o.Etapa == etapaGestaoDevolucao);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa BuscarEtapaPorCodigoGestaoEOrdem(long codigo, int ordem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa>();
            query = query.Where(o => o.GestaoDevolucao.Codigo == codigo && o.Ordem == ordem);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa> BuscarEtapaPorCodigoGestao(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa>();
            query = query.Where(o => o.GestaoDevolucao.Codigo == codigo);
            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa>> BuscarEtapaPorCodigoGestaoAsync(List<long> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa>();
            query = query.Where(o => codigos.Contains(o.GestaoDevolucao.Codigo));
            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa> BuscarEtapasAnteriores(long codigoGestao, Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa etapaGestaoDevolucao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa>();
            query = query.Where(o => o.GestaoDevolucao.Codigo == codigoGestao && o.Ordem < etapaGestaoDevolucao.Ordem);
            return query.ToList();
        }
    }
}
