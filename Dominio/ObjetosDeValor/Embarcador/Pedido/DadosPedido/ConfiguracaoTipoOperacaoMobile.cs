using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class ConfiguracaoTipoOperacaoMobile
    {
        public bool NecessarioConfirmacaoMotorista { get; set; }

        public TimeSpan TempoLimiteConfirmacaoMotorista { get; set; }
    }
}
