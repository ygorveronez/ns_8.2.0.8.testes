
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusNfe
    {
        SemStatus = 0,
        Autorizado = 100,
        Cancelado = 101
    }

    public static class StatusNfeHelper
    {
        public static string ObterDescricao(this StatusNfe status)
        {
            switch (status)
            {
                case StatusNfe.SemStatus: return "Sem Status";
                case StatusNfe.Autorizado: return "Autorizado";
                case StatusNfe.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
