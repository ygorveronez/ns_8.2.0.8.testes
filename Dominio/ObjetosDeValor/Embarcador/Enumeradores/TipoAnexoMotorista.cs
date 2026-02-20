namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAnexoMotorista
    {
        Outros = 0,
        Cnh = 1
    }
    public static class TipoAnexoMotoristaHelper
    {
        public static string ObterDescricao(this TipoAnexoMotorista tipo)
        {
            switch (tipo)
            {
                case TipoAnexoMotorista.Outros: return "Outros";
                case TipoAnexoMotorista.Cnh: return "CNH";
                default: return string.Empty;
            }
        }
    }
}
