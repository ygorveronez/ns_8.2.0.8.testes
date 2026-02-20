namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoContaBanco
    {
        Nenhum = 0,
        Corrente = 1,
        Poupança = 2,
        Salario = 3
    }

    public static class TipoContaBancoHelper
    {
        public static string ObterDescricao(this TipoContaBanco tipo)
        {
            switch (tipo)
            {
                case TipoContaBanco.Salario: return "Conta Salário";
                case TipoContaBanco.Corrente: return "Conta Corrente";
                case TipoContaBanco.Poupança: return "Conta Poupança";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoAbreviada(this TipoContaBanco? tipo)
        {
            switch (tipo)
            {
                case TipoContaBanco.Salario: return "Salário";
                case TipoContaBanco.Corrente: return "Corrente";
                case TipoContaBanco.Poupança: return "Poupança";
                default: return string.Empty;
            }
        }
    }
}
