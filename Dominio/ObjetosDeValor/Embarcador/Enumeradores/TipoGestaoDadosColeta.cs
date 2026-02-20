namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGestaoDadosColeta
    {
        DadosNfe = 0,
        DadosTransporte = 1
    }

    public static class TipoGestaoDadosColetaHelper
    {
        public static string ObterDescricao(this TipoGestaoDadosColeta origem)
        {
            switch (origem)
            {
                case TipoGestaoDadosColeta.DadosNfe: return "Dados da NF-e";
                case TipoGestaoDadosColeta.DadosTransporte: return "Dados de Transporte";
                default: return string.Empty;
            }
        }
    }
}
