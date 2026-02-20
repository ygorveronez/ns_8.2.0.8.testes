namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoNota
    {
        Todas = 0,
        NFe = 1,
        NFSe = 2
    }

    public static class TipoNotaHelper
    {
        public static string ObterDescricao(this TipoNota tipoNota)
        {
            switch (tipoNota)
            {
                case TipoNota.NFe: return "NF-e";
                case TipoNota.NFSe: return "NFS-e";
                default: return string.Empty;
            }
        }
    }
}
