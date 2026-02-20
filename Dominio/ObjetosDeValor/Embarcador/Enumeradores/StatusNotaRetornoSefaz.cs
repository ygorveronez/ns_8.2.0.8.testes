namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusNotaRetornoSefaz
    {
        Autorizado = 138,
        DocumentoNaoLocalidado = 137,
        Cancelado = 653,
        NaoDefinido = 0
    }

    public static class StatusNotaRetornoSefazHelper
    {
        public static StatusNotaRetornoSefaz ObterStatusSefas(string retornoStatus)
        {
            switch (retornoStatus)
            {
                case "653":
                    return StatusNotaRetornoSefaz.Cancelado;
                case "137":
                    return StatusNotaRetornoSefaz.DocumentoNaoLocalidado;
                case "138":
                    return StatusNotaRetornoSefaz.Autorizado;
                default:
                    return StatusNotaRetornoSefaz.NaoDefinido;
            }
        }
    }

}
