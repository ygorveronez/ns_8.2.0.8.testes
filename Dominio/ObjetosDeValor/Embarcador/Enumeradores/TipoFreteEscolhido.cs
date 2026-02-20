namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFreteEscolhido
    {
        todos = 0,
        Tabela = 1,
        Operador = 2,
        Leilao = 3,
        Embarcador = 4,
        Cliente = 5
    }

    public static class TipoFreteEscolhidoHelper
    {
        public static string ObterDescricao(this TipoFreteEscolhido tipoFreteEscolhido)
        {
            switch (tipoFreteEscolhido)
            {
                case TipoFreteEscolhido.Tabela: return "Tabela";
                case TipoFreteEscolhido.Operador: return "Operador";
                case TipoFreteEscolhido.Leilao: return "Leilão";
                case TipoFreteEscolhido.Embarcador: return "Embarcador";
                case TipoFreteEscolhido.Cliente: return "Cliente";
                default: return string.Empty;
            }
        }
    }
}