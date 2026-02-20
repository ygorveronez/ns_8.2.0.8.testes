namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMotivo
    {
        RejeicaoDadosNFeColeta = 1
    }

    public static class TipoMotivoHelper
    {
        public static string ObterDescricao(this TipoMotivo tipoMotivo)
        {
            switch (tipoMotivo)
            {
                case TipoMotivo.RejeicaoDadosNFeColeta: return "Rejeição de dados da NF-e da coleta";
                default: return string.Empty;
            }
        }
    }
}
