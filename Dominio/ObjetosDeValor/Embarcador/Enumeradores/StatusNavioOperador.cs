using Dominio.Excecoes.Embarcador;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusNavioOperador
    {
        Inativo = 0,
        Ativo = 1,
        ParcialmenteAtivo = 2
    }

    public static class StatusNavioOperadorHelper
    {
        public static string ObterDescricao(this StatusNavioOperador StatusNavioOperador)
        {
            switch (StatusNavioOperador)
            {
                case StatusNavioOperador.Inativo: return "Inativo";
                case StatusNavioOperador.Ativo: return "Ativo";
                case StatusNavioOperador.ParcialmenteAtivo: return "Parcialmente Ativo";
                default: return string.Empty;
            }
        }


        public static StatusNavioOperador ConverterDoIngles(string status) => status?.ToUpper() switch
        {
            "INACTIVE" => StatusNavioOperador.Inativo,
            "ACTIVE" => StatusNavioOperador.Ativo,
            "PARTIALLY ACTIVE" => StatusNavioOperador.ParcialmenteAtivo,
            _ => throw new ServicoException($"Status de navio operador inválido: {status}")
        };
    }
}