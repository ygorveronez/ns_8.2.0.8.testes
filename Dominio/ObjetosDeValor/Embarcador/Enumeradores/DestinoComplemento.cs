namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DestinoComplemento
    {
        Subcontratada = 0,
        FilialEmissora = 1

    }

    public static class DestinoComplementoHelper
    {
        public static string ObterDescricao(this DestinoComplemento destinoComplemento)
        {
            switch (destinoComplemento)
            {
                case DestinoComplemento.Subcontratada: return "Subcontratada";
                case DestinoComplemento.FilialEmissora: return "Filial Emissora";
                default: return "";
            }
        }
    }
}
