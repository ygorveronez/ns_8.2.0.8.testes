namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EixosSuspenso
    {
        Ida = 1,
        Volta = 2, //Padrão..        
        Nenhum = 3
    }

    public static class EixosSuspensoHelper
    {
        public static string ObterDescricao(this EixosSuspenso tipo)
        {
            switch (tipo)
            {
                case EixosSuspenso.Nenhum: return "Nenhum";
                case EixosSuspenso.Ida: return "Ida";
                case EixosSuspenso.Volta: return "Volta";
                default: return string.Empty;
            }
        }
    }
}
