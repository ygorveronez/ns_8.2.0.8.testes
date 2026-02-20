namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusIntegracaoSIC
    {
        AguardandoIntegracao = 0,
        ArmazenamentoSolicitado = 1,
        Validando = 2,
        IntegrandoSap = 3,
        PersistindoRepositorioSic = 4,
        ErroProcessamento = 5,
        IntegracaoConcluida = 6,
    }

    public static class StatusIntegracaoSICHelper
    {
        public static string ObterDescricao(this StatusIntegracaoSIC situacao)
        {
            switch (situacao)
            {
                case StatusIntegracaoSIC.AguardandoIntegracao:return "Aguardando Integração";
                case StatusIntegracaoSIC.ArmazenamentoSolicitado: return "Armazenamento Solicitado";
                case StatusIntegracaoSIC.Validando: return "Validando";
                case StatusIntegracaoSIC.IntegrandoSap: return "Integrando Sap";
                case StatusIntegracaoSIC.PersistindoRepositorioSic: return "PersistindoRepositorio Sic";
                case StatusIntegracaoSIC.ErroProcessamento: return "Erro Processamento";
                case StatusIntegracaoSIC.IntegracaoConcluida: return "Integração Concluida";
                default: return string.Empty;
            }
        }
    }
}
