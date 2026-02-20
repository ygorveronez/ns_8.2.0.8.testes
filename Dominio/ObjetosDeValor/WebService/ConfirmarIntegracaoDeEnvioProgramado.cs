namespace Dominio.ObjetosDeValor.WebService
{
    public class ConfirmarIntegracaoDeEnvioProgramado
    {
        public int ProtocoloCTe { get; set; }

        public int MessageCode { get; set; }

        public string Message { get; set; }
        public string DtCargoNumber { get; set; }

        public string Step { get; set; }
    }
}
