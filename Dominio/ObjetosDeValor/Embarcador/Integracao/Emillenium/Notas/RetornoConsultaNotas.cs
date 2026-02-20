using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas
{
    public class RetornoConsultaNotas
    {
        public string informacao { get; set; }
        public int ultimaTransId { get; set; }
        public List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao> ListapedidoAguardandoIntegracaoNotaretorno { get; set; }


    }
}
