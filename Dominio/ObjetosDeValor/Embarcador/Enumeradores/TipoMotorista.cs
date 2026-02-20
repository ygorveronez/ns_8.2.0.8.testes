namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMotorista
    {
        Todos = 0,
        Proprio = 1,
        Terceiro = 2
    }

    public static class TipoMotoristaHelper
    {
        public static string ObterDescricao(this TipoMotorista tipo)
        {
            switch (tipo)
            {
                case TipoMotorista.Proprio: return "Próprio";
                case TipoMotorista.Terceiro: return "Terceiro";
                default: return string.Empty;
            }
        }

        public static TipoMotorista ObterTipoMotorista(string descricaoMotorista)
        {
            if (string.IsNullOrWhiteSpace(descricaoMotorista))
                return TipoMotorista.Proprio;

            switch (descricaoMotorista)
            {
                case "Proprio": return TipoMotorista.Proprio;
                case "1": return TipoMotorista.Proprio;
                case "Terceiro": return TipoMotorista.Terceiro;
                case "2": return TipoMotorista.Terceiro;
                default: return TipoMotorista.Proprio;
            }
        }

    }
}
