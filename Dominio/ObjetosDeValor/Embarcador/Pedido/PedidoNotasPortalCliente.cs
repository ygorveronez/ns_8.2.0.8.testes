using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class PedidoNotasPortalCliente
    {
        public int Codigo { get; set; }

        public SituacaoPedido? Status { get; set; }

        public int? CodigoStatus
        {
            set
            {
                if (value.HasValue)
                    Status = (SituacaoPedido)value.Value;
                else
                    Status = null;
            }
        }

        public SituacaoDigitalizacaoCanhoto? StatusCanhoto { get; set; }

        public int? CodigoStatusCanhoto
        {
            set
            {
                if (value.HasValue)
                    StatusCanhoto = (SituacaoDigitalizacaoCanhoto)value.Value;
                else
                    StatusCanhoto = null;
            }
        }

        public int Nota { get; set; }

    }
}
