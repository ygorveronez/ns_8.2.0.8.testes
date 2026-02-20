using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Havan
{
    public class Protocolo
    {
        public int protocoloIntegracaoCarga { get; set; }
        public int protocoloTipoOperacao { get; set; }
        public string tipoOperacao { get; set; }
        public string codigoIntegracaoTipoOperacao { get; set; }
        public List<Veiculo> Veiculos { get; set; }
    }
}
