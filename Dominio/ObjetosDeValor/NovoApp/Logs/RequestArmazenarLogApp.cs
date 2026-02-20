namespace Dominio.ObjetosDeValor.NovoApp.Logs
{
    public class RequestArmazenarLogApp
    {
        public int? codigoMotorista { get; set; }
        public int? clienteMultisoftware { get; set; }
        public long dataRegistroApp { get; set; }
        public string versaoApp { get; set; }
        public string versaoSistemaOperacional { get; set; }
        public string marcaAparelho { get; set; }
        public string modeloAparelho { get; set; }
        public string mensagem { get; set; }
        public string extra { get; set; }
        public bool erro { get; set; }
    }
}
