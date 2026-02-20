namespace System
{
    public static class BoolExtensions
    {
        public static string ObterDescricao(this bool valor)
        {
            return valor ? "Sim" : "Não";
        }

        public static string ObterDescricaoAtivo(this bool valor)
        {
            return valor ? "Ativo" : "Inativo";
        }

        public static string ObterDescricaoAtiva(this bool valor)
        {
            return valor ? "Ativa" : "Inativa";
        }
    }
}
