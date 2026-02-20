using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class PedidoPortalCliente
    {
        public int Codigo { get; set; }

        public SituacaoAcompanhamentoPedido? Status { get; set; }

        public int? CodigoStatus
        {
            set
            {
                if (value.HasValue)
                    Status = (SituacaoAcompanhamentoPedido)value.Value;
                else
                    Status = null;
            }
        }

        public string CTes { get; set; }

        public int QuantidadeNotas { get; set; }

        public int QuantidadeNotasEntregues { get; set; }

        public string Cliente { get; set; }

        public string Destino { get; set; }

        public decimal Peso { get; set; }

        public DateTime? Emissao { get; set; }

        public DateTime? Entrega { get; set; }

        public DateTime? PrevisaoEntrega { get; set; }

        public string NumeroPedido { get; set; }

        public string NotasFiscais { get; set; }
    }
}
