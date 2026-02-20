using System;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingCEPDestino : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPDestino>
    {
        public BiddingCEPDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public BiddingCEPDestino(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public async Task DeletarTodosPorCodigoAsync(int codigoRota)
        {
            await UnitOfWork.Sessao
                .CreateQuery("DELETE FROM BiddingOfertaRotaCEPDestino c WHERE c.BiddingOfertaRota.Codigo = :codigo")
                .SetInt32("codigo", codigoRota)
                .ExecuteUpdateAsync(CancellationToken);
            
        }
    }
}
