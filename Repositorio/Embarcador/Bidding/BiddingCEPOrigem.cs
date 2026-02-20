using System;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingCEPOrigem : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPOrigem>
    {
        public BiddingCEPOrigem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public BiddingCEPOrigem(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public async Task DeletarTodosPorCodigoAsync(int codigoRota)
        {
            await UnitOfWork.Sessao
                .CreateQuery("DELETE FROM BiddingOfertaRotaCEPOrigem c WHERE c.BiddingOfertaRota.Codigo = :codigo")
                .SetInt32("codigo", codigoRota)
                .ExecuteUpdateAsync(CancellationToken);
        }
    }
}
