namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDiarioBordoSemanal
    {
        Todas = 0,
        Aberto = 1,
        EntregueParcial = 2,
        EntregueCompleto = 3,
        Cancelado = 4
    }

    public static class SituacaoDiarioBordoSemanalHelper
    {
        public static string ObterDescricao(this SituacaoDiarioBordoSemanal situacao)
        {
            switch (situacao)
            {
                case SituacaoDiarioBordoSemanal.Aberto: return "Aberto";
                case SituacaoDiarioBordoSemanal.EntregueParcial: return "Entregue Parcialmente";
                case SituacaoDiarioBordoSemanal.EntregueCompleto: return "Entregue Completo";
                case SituacaoDiarioBordoSemanal.Cancelado: return "Cancelado";
                default: return "Todas";
            }
        }
    }
}
