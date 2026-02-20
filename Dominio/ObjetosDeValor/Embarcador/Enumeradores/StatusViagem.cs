namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusViagem
    {
        Todas,
        SemViagem,
        EmViagem
    }

    public static class StatusViagemHelper
    {
        public static string ObterDescricao(this StatusViagem statusViagem)
        {
            switch (statusViagem)
            {
                case StatusViagem.SemViagem: return "Sem Viagem";
                case StatusViagem.EmViagem: return "Em Viagem";
                default: return string.Empty;
            }
        }
    }
}
