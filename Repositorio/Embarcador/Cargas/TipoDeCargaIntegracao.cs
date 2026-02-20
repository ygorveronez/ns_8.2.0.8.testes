using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoDeCargaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaIntegracao>
    {
        public TipoDeCargaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoDeCargaIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaIntegracao> BuscarPorTipoCarga(int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaIntegracao>()
                .Where(o => o.TipoDeCarga.Codigo == codigoTipoCarga);

            return query.ToList();
        }
    }
}
