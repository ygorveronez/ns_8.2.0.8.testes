namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemGestaoDadosColeta
    {
        Embarcador = 0,
        Transportador = 1,
        Motorista = 2
    }

    public static class OrigemGestaoDadosColetaHelper
    {
        public static string ObterDescricao(this OrigemGestaoDadosColeta origem)
        {
            switch (origem)
            {
                case OrigemGestaoDadosColeta.Embarcador: return "Embarcador";
                case OrigemGestaoDadosColeta.Transportador: return "Transportador";
                case OrigemGestaoDadosColeta.Motorista: return "Motorista";
                default: return string.Empty;
            }
        }
    }
}
