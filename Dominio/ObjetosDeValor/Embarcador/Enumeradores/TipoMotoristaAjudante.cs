namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMotoristaAjudante
    {
        Motorista = 0,
        Ajudante = 1,
        Todos = 2
    }

    public static class TipoMotoristaAjudanteHelper
    {
        public static string ObterDescricao(this TipoMotoristaAjudante tipo)
        {
            switch (tipo)
            {
                case TipoMotoristaAjudante.Motorista: return "Motorista";
                case TipoMotoristaAjudante.Ajudante: return "Ajudante";
                default: return string.Empty;
            }
        }

        public static TipoMotoristaAjudante ObterTipoMotoristaAjudante(string descricaoMotorista)
        {
            if (string.IsNullOrWhiteSpace(descricaoMotorista))
                return TipoMotoristaAjudante.Motorista;

            switch (descricaoMotorista)
            {
                case "Motorista": return TipoMotoristaAjudante.Motorista;
                case "0": return TipoMotoristaAjudante.Motorista;
                case "Ajudante": return TipoMotoristaAjudante.Ajudante;
                case "1": return TipoMotoristaAjudante.Ajudante;
                default: return TipoMotoristaAjudante.Motorista;
            }
        }
    }
}
