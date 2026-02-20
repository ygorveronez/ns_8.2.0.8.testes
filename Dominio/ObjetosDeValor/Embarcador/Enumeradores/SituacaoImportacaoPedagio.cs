namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoImportacaoPedagio
    {
        Todas = 0,
        Pendente = 1,
        Processando = 2,
        Sucesso = 3,
        Erro = 4,
        Cancelado = 5
    }

    public static class SituacaoImportacaoPedagioHelper
    {
        public static string ObterDescricao(this SituacaoImportacaoPedagio situacao)
        {
            switch (situacao)
            {
                case SituacaoImportacaoPedagio.Todas:
                    return "Todas";
                case SituacaoImportacaoPedagio.Pendente:
                    return "Pendente";
                case SituacaoImportacaoPedagio.Processando:
                    return "Processando";
                case SituacaoImportacaoPedagio.Sucesso:
                    return "Sucesso";
                case SituacaoImportacaoPedagio.Erro:
                    return "Erro";
                case SituacaoImportacaoPedagio.Cancelado:
                    return "Cancelado";
                default:
                    return string.Empty;
            }
        }
    }
}
