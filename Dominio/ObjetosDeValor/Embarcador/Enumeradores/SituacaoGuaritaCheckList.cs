namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoGuaritaCheckList
    {
        Todos = 0,
        Aberto = 1,
        Cancelado = 2,
        Finalizado = 3
    }

    public static class SituacaoGuaritaCheckListHelper
    {
        public static string ObterDescricao(this SituacaoGuaritaCheckList situacao)
        {
            switch (situacao)
            {
                case SituacaoGuaritaCheckList.Aberto: return "Aberto";
                case SituacaoGuaritaCheckList.Finalizado: return "Finalizado";
                case SituacaoGuaritaCheckList.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
