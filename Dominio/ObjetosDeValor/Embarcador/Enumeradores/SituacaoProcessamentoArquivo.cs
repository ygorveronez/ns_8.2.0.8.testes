namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoProcessamentoArquivo
    {
        Todos = 0,
        AguardandoProcessamento = 1,
        ErroNoArquivo = 2,
        ErroNoProcessamento = 3,
        Processado = 4,
        Cancelado = 5
    }

    public static class SituacaoProcessamentoArquivoHelper
    {
        public static string ObterDescricao(this SituacaoProcessamentoArquivo tipo)
        {
            switch (tipo)
            {
                case SituacaoProcessamentoArquivo.AguardandoProcessamento:
                    return "Aguardando Processamento";
                case SituacaoProcessamentoArquivo.ErroNoArquivo:
                    return "Erro no Arquivo";
                case SituacaoProcessamentoArquivo.ErroNoProcessamento:
                    return "Erro no Processamento";
                case SituacaoProcessamentoArquivo.Processado:
                    return "Processado";     
                case SituacaoProcessamentoArquivo.Cancelado:
                    return "Cancelado";
                default:
                    return "";
            }
        }
        public static string ObterCorLinha(this SituacaoProcessamentoArquivo situacao)
        {
            switch (situacao)
            {
                case SituacaoProcessamentoArquivo.AguardandoProcessamento: return CorGrid.Azul;
                case SituacaoProcessamentoArquivo.Processado: return CorGrid.Verde;
                case SituacaoProcessamentoArquivo.ErroNoProcessamento:
                case SituacaoProcessamentoArquivo.ErroNoArquivo:
                case SituacaoProcessamentoArquivo.Cancelado:
                    return CorGrid.Vermelho;
                default: return string.Empty;
            }
        }
    }
}
