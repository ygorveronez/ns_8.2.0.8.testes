using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Mondelez
{
    public class ParametrosGeracaoIntegracaoDrivin
    {
        public GatilhoIntegracaoMondelezDrivin Gatilho { get; set; }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        public EventoColetaEntrega EventoColetaEntrega { get; set; }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        public Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente ConfiguracaoPortalCliente { get; set; }

        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware { get; set; }
    }
}