namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class CargaSituacao
    {
        public int ProtocoloCarga { get; set; }
        public string NumeroCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCarga { get; set; }
        public string Mensagem { get; set; } 
    }
}
