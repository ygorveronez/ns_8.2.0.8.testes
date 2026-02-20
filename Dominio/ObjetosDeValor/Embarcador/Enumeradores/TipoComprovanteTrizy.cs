namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoComprovanteTrizy
    {
        ComprovanteEntrega = 1,
        CTe = 2,
        MDFe = 3
    }

    public static class TipoComprovanteTrizyHelper
    {
        public static string ObterDescricao(this TipoComprovanteTrizy status)
        {
            switch (status)
            {
                case TipoComprovanteTrizy.ComprovanteEntrega: return "Comprovante de Coleta";
                case TipoComprovanteTrizy.CTe: return "CT-e";
                case TipoComprovanteTrizy.MDFe: return "MDF-e";
                default: return string.Empty;
            }
        }

        public static string ObterNumero(this TipoComprovanteTrizy status)
        {
            switch (status)
            {
                case TipoComprovanteTrizy.ComprovanteEntrega: return "3";
                case TipoComprovanteTrizy.CTe: return "2";
                case TipoComprovanteTrizy.MDFe: return "4";
                default: return string.Empty;
            }
        }


        public static string ObterNomeArquivo(this TipoComprovanteTrizy status)
        {
            switch (status)
            {
                case TipoComprovanteTrizy.ComprovanteEntrega: return "ComprovanteDeColeta.pdf";
                case TipoComprovanteTrizy.CTe: return "CTe.pdf";
                case TipoComprovanteTrizy.MDFe: return "MDFe.pdf";
                default: return string.Empty;
            }
        }
    }
}
