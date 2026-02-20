namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoInicioHigienizacao
    {
        AguardandoInicioHigienizacao = 1,
        HigienizacaoIniciada = 2
    }

    public static class SituacaoInicioHigienizacaoHelper
    {
        public static string ObterDescricao(this SituacaoInicioHigienizacao situacao)
        {
            switch (situacao)
            {
                case SituacaoInicioHigienizacao.AguardandoInicioHigienizacao: return "Aguardando Início da Higienização";
                case SituacaoInicioHigienizacao.HigienizacaoIniciada: return "Higienização Iniciada";
                default: return string.Empty;
            }
        }
    }
}
