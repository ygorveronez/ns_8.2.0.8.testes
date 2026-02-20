namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum Escolaridade
    {
        SemInstrucaoFormal = 0,
        EnsinoFundamental = 1,
        EnsinoMedio = 2,
        EnsinoSuperior = 3,
        PosGraduacao = 4,
        Mestrado = 5,
        Doutorado = 6
    }

    public static class EscolaridadeHelper
    {
        public static string ObterDescricao(this Escolaridade situacao)
        {
            switch (situacao)
            {
                case Escolaridade.EnsinoFundamental: return "Ensino Fundamental";
                case Escolaridade.EnsinoMedio: return "Ensino Médio";
                case Escolaridade.EnsinoSuperior: return "Ensino Superior";
                case Escolaridade.PosGraduacao: return "Pós Graduação";
                case Escolaridade.Mestrado: return "Mestrado";
                case Escolaridade.Doutorado: return "Doutorado";
                default: return string.Empty;
            }
        }
    }
}
