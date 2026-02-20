namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoImportacaoProgramacaoColeta
    {
        Todos = 0,
        EmCriacao = 1,
        EmAndamento = 2,
        Finalizado = 3,
        FalhaNaGeracao = 4
    }

    public static class SituacaoImportacaoProgramacaoColetaHelper
    {
        public static string ObterDescricao(this SituacaoImportacaoProgramacaoColeta situacao)
        {
            switch (situacao)
            {
                case SituacaoImportacaoProgramacaoColeta.EmCriacao: return "Em Criação";
                case SituacaoImportacaoProgramacaoColeta.EmAndamento: return "Em Andamento";
                case SituacaoImportacaoProgramacaoColeta.Finalizado: return "Finalizado";
                case SituacaoImportacaoProgramacaoColeta.FalhaNaGeracao: return "Falha na Geração";
                default: return string.Empty;
            }
        }
    }
}