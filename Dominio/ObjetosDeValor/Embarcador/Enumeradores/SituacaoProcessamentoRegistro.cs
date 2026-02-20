
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoProcessamentoRegistro
    {
        Pendente = 0,
        Sucesso = 1,
        Falha = 2,
        Liberado = 3,
        FalhaLiberacao = 4,
    }

    public static class SituacaoProcessamentoRegistroHelper
    {
        public static string ObterDescricao(this SituacaoProcessamentoRegistro situacao)
        {
            switch (situacao)
            {
                case SituacaoProcessamentoRegistro.Pendente:
                    return "Pendente";
                case SituacaoProcessamentoRegistro.Sucesso:
                    return "Sucesso";
                case SituacaoProcessamentoRegistro.Falha:
                    return "Falha";
                case SituacaoProcessamentoRegistro.Liberado:
                    return "Liberado";
                case SituacaoProcessamentoRegistro.FalhaLiberacao:
                    return "Falha Liberação";
                default:
                    return "";
            }
        }

        public static string ObterCorFonte(this SituacaoProcessamentoRegistro situacao)
        {
            switch (situacao)
            {
                case SituacaoProcessamentoRegistro.Falha: return CorGrid.Branco;
                default: return string.Empty;
            }
        }

        public static string ObterCorLinha(this SituacaoProcessamentoRegistro situacao)
        {
            switch (situacao)
            {
                case SituacaoProcessamentoRegistro.Liberado: return CorGrid.Amarelo;
                case SituacaoProcessamentoRegistro.Pendente: return CorGrid.Azul;
                case SituacaoProcessamentoRegistro.Sucesso: return CorGrid.Verde;
                case SituacaoProcessamentoRegistro.Falha: return CorGrid.Vermelho;
                case SituacaoProcessamentoRegistro.FalhaLiberacao: return CorGrid.Laranja;
                default: return string.Empty;
            }
        }
    }
}
