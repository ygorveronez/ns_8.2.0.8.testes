namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoObrigacaoUsoTerminal
    {
        Nenhum = 1,
        OrigemDestino = 2,
        Origem = 3,
        Destino = 4
    }

    public static class TipoObrigacaoUsoTerminalHelper
    {
        public static string ObterDescricao(this TipoObrigacaoUsoTerminal tipo)
        {
            switch (tipo)
            {
                case TipoObrigacaoUsoTerminal.Nenhum: return "Nenhum";
                case TipoObrigacaoUsoTerminal.OrigemDestino: return "Origem e Destino";
                case TipoObrigacaoUsoTerminal.Origem: return "Origem";
                case TipoObrigacaoUsoTerminal.Destino: return "Destino";                
                default: return string.Empty;
            }
        }
    }
}
