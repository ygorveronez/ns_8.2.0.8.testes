using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo>
    {
        public TipoOperacaoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TipoOperacaoAnexo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo> BuscarPorCodigoTipoOperacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo>();
            var result = from obj in query where obj.TipoOperacao.Codigo == codigo select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo>> BuscarPorCodigoTipoOperacaoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo>();
            var result = from obj in query where obj.TipoOperacao.Codigo == codigo select obj;
            return result.ToListAsync(CancellationToken);
        }
    }
}
