namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class IntegracaoNotificacao
    {
        public Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public Enumeradores.TipoNotificacaoApp TipoNotificacaoApp { get; set; }

        public Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe GestaoDadosColetaDadosNFe { get; set; }

        public Entidades.Embarcador.Configuracoes.Motivo MotivoRejeicao { get; set; }

        public Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp IntegracaoMonitoramentoNotificacoesApp { get; set; }

        public Entidades.Embarcador.Chamados.Chamado Chamado { get; set; }
    }
}
