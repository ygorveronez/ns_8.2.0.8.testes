using System;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingAceitamentoRota : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>
    {
        public BiddingAceitamentoRota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void DeletarPorRotaTransportador(int rotaCodigo, int transportadorCodigo)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE FROM BiddingTransportadorRota c WHERE c.Rota.Codigo = :codigo and c.Transportador.Codigo = :codigoTransportador")
                .SetInt32("codigoTransportador", transportadorCodigo)
                .SetInt32("codigo", rotaCodigo)
                .ExecuteUpdate();
           
        }
    }
}
