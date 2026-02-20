using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Terceiros
{
    public class AutorizacaoPagamento
    {
        public int ProtocoloContratoFrete { get; set; }
        public TipoAutorizacaoPagamentoCIOTParcela TipoAutorizacao { get; set; }
    }
}
