namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RelacionamentoCarga
    {
        Relacionada = 1,
        NaoRelacionada = 2,
    }

    public static class RelacionamentoCargaHelper
    {
        public static string ObterDescricao(this RelacionamentoCarga situacao)
        {
            switch (situacao)
            {
                case RelacionamentoCarga.Relacionada: return "Relacionada";
                case RelacionamentoCarga.NaoRelacionada: return "Não Relacionada";
                default: return string.Empty;
            }
        }
    }
}
