namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoTrajetoCarga
    {
        AguardandoInicio = 0,
        EmTransporte = 1,
        Finalizada = 2
    }

    public static class SituacaoTrajetoCargaHelper
    {
        public static string ObterDescricao(this SituacaoTrajetoCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoTrajetoCarga.AguardandoInicio: return "Aguardando Início";
                case SituacaoTrajetoCarga.EmTransporte: return "Em Trânsito";
                case SituacaoTrajetoCarga.Finalizada: return "Finalizada";
                default: return string.Empty;
            }
        }
    }
}
