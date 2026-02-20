using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaPedidoXMLNotaFiscal : IEquatable<CargaPedidoXMLNotaFiscal>
    {
        public bool Ativa { get; set; }

        public int Codigo { get; set; }

        public decimal PesoCubado { get; set; }

        public decimal Peso { get; set; }

        public decimal Valor { get; set; }

        public bool TipoFatura { get; set; }

        public Enumeradores.ClassificacaoNFe? ClassificacaoNFe { get; set; }

        public bool Equals(CargaPedidoXMLNotaFiscal other)
        {
            return (other.Codigo == Codigo);
        }

        public override int GetHashCode()
        {
            return Codigo.GetHashCode();
        }
    }
}
