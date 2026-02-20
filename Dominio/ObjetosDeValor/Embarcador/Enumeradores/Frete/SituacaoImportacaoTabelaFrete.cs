namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete
{
    public enum SituacaoImportacaoTabelaFrete
    {
        Todas = 6,
        Pendente = 0,
        Processando = 1,
        Sucesso = 2,
        Erro = 3,
        Cancelado = 4
    }

    public static class SituacaoImportacaoTabelaFreteHelper
    {
        public static string ObterDescricao(this SituacaoImportacaoTabelaFrete situacao)
        {
            switch (situacao)
            {
                case SituacaoImportacaoTabelaFrete.Pendente:
                    return "Pendente";
                case SituacaoImportacaoTabelaFrete.Processando:
                    return "Processando";
                case SituacaoImportacaoTabelaFrete.Sucesso:
                    return "Sucesso";
                case SituacaoImportacaoTabelaFrete.Erro:
                    return "Erro";
                case SituacaoImportacaoTabelaFrete.Cancelado:
                    return "Cancelado";
                case SituacaoImportacaoTabelaFrete.Todas:
                    return "Todas";
                default:
                    return string.Empty;
            }
        }
    }
}
