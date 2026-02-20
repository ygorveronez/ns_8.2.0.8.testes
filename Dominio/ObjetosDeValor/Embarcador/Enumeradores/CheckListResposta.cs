namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CheckListResposta
    {
        Aprovada = 1,
        Reprovada = 2
    }

    public static class CheckListRespostaHelper
    {
        public static string ObterDescricao(this CheckListResposta resposta)
        {
            switch (resposta)
            {
                case CheckListResposta.Aprovada: return "Aprovada";
                case CheckListResposta.Reprovada: return "Reprovada";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoSimNao(this CheckListResposta resposta)
        {
            switch (resposta)
            {
                case CheckListResposta.Aprovada: return "Sim";
                case CheckListResposta.Reprovada: return "Não";
                default: return string.Empty;
            }
        }
    }
}
