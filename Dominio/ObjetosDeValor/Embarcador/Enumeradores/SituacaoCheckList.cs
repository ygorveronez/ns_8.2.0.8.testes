namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCheckList
    {
        Aberto = 1,
        Finalizado = 2,
        Rejeitado = 3
    }

    public static class SituacaoCheckListHelper
    {
        public static bool IsPermiteEdicao(this SituacaoCheckList situacao)
        {
            return (situacao == SituacaoCheckList.Aberto);
        }

        public static string ObterDescricao(this SituacaoCheckList situacao)
        {
            switch (situacao)
            {
                case SituacaoCheckList.Aberto: return "Aberto";
                case SituacaoCheckList.Finalizado: return "Finalizado";
                case SituacaoCheckList.Rejeitado: return "Rejeitado";
                default: return string.Empty;
            }
        }
    }
}
