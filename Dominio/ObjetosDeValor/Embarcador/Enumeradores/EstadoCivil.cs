namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EstadoCivil
    {
        Outros = 0,
        Solteiro = 1,
        Casado = 2,
        Divorciado = 3,
        Desquitado = 4,
        Viuvo = 5
    }

    public static class EstadoCivilHelper
    {
        public static string ObterDescricao(this EstadoCivil situacao)
        {
            switch (situacao)
            {
                case EstadoCivil.Solteiro: return "Solteiro";
                case EstadoCivil.Casado: return "Casado";
                case EstadoCivil.Divorciado: return "Divorciado";
                case EstadoCivil.Desquitado: return "Desquitado";
                case EstadoCivil.Viuvo: return "Viúvo";
                default: return string.Empty;
            }
        }
    }
}
