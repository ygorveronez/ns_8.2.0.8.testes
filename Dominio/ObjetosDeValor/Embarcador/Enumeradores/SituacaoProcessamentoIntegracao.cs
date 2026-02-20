
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoProcessamentoIntegracao
    {
        AguardandoProcessamento = 0,
        Processado = 1,
        ErroProcessamento = 2
    }

    public static class SituacaoProcessamentoIntegracaoHelper
    {
        public static string ObterDescricao(this SituacaoProcessamentoIntegracao situacao)
        {
            switch (situacao)
            {
                case SituacaoProcessamentoIntegracao.AguardandoProcessamento:
                    return "Aguardando Processamento";
                case SituacaoProcessamentoIntegracao.Processado:
                    return "Processado";
                case SituacaoProcessamentoIntegracao.ErroProcessamento:
                    return "Erro Processado";
                default:
                    return "";
            }
        }

        public static string ObterCorLinha(this SituacaoProcessamentoIntegracao situacao)
        {
            switch (situacao)
            {
                case SituacaoProcessamentoIntegracao.AguardandoProcessamento: return CorGrid.Amarelo;
                case SituacaoProcessamentoIntegracao.Processado: return CorGrid.Verde;
                case SituacaoProcessamentoIntegracao.ErroProcessamento: return CorGrid.Vermelho;
                default: return string.Empty;
            }
        }

        public static string ObterCorFonte(this SituacaoProcessamentoIntegracao situacao)
        {
            switch (situacao)
            {
                case SituacaoProcessamentoIntegracao.ErroProcessamento: return CorGrid.Branco;
                default: return string.Empty;
            }
        }
    }
}
