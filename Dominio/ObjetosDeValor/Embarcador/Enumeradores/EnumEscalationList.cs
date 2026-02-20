namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum Aposentadoria
    {
        NaoInformado = 0,
        Aposentado = 1,
        NaoAposentado = 2,
        Todos = 99
    }

    public static class AposentadoriaHelper
    {
        public static string ObterDescricao(this Aposentadoria situacao)
        {
            switch (situacao)
            {
                case Aposentadoria.Aposentado: return "Aposentado";
                case Aposentadoria.NaoAposentado: return "Não Aposentado";
                case Aposentadoria.NaoInformado: return "Não Informado";
                default: return string.Empty;
            }
        }

    }
}
