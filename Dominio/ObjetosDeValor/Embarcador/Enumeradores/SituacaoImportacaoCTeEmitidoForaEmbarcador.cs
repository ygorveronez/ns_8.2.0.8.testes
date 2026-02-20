namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoImportacaoCTeEmitidoForaEmbarcador
    {
        Todas = 0,
        Pendente = 1,
        Processando = 2,
        Sucesso = 3,
        Erro = 4,
        Cancelado = 5
    }

    public static class SituacaoImportacaoCTeEmitidoForaEmbarcadorHelper
    {
        public static string ObterDescricao(this SituacaoImportacaoCTeEmitidoForaEmbarcador situacao)
        {
            switch (situacao)
            {
                case SituacaoImportacaoCTeEmitidoForaEmbarcador.Todas:
                    return "Todas";
                case SituacaoImportacaoCTeEmitidoForaEmbarcador.Pendente:
                    return "Pendente";
                case SituacaoImportacaoCTeEmitidoForaEmbarcador.Processando:
                    return "Processando";
                case SituacaoImportacaoCTeEmitidoForaEmbarcador.Sucesso:
                    return "Sucesso";
                case SituacaoImportacaoCTeEmitidoForaEmbarcador.Erro:
                    return "Erro";
                case SituacaoImportacaoCTeEmitidoForaEmbarcador.Cancelado:
                    return "Cancelado";
                default:
                    return string.Empty;
            }
        }
    }
}