namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum VerificarStatusViagem
    {
        Todos = 0,
        NaoVerificar = 1,
        EstarComStatusViagem = 2,
        NaoEstarComStatusViagem = 3,
        HaverPeloMenosUmStatusViagem = 4,
        HaverTodosStatusViagem = 5
    }

    public static class VerificarStatusViagemHelper
    {
        public static string ObterDescricao(this VerificarStatusViagem verificarStatusViagem)
        {
            switch (verificarStatusViagem)
            {
                case VerificarStatusViagem.Todos: return "Todos";
                case VerificarStatusViagem.NaoVerificar: return "Não verificar";
                case VerificarStatusViagem.EstarComStatusViagem: return "Estar com o status de viagem";
                case VerificarStatusViagem.NaoEstarComStatusViagem: return "Não estar com o status de viagem";
                case VerificarStatusViagem.HaverPeloMenosUmStatusViagem: return "Haver pelo menos um dos status de viagem";
                case VerificarStatusViagem.HaverTodosStatusViagem: return "Haver todos os status de viagem";
                default: return "";
            }
        }
    }
}
