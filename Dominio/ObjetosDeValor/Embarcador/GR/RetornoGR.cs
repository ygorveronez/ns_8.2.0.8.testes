namespace Dominio.ObjetosDeValor.Embarcador.GR
{
    public class RetornoGR
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string Protocolo { get; set; }
        public Enumeradores.TipoIntegracao? TipoIntegracao { get; set; }
    }
}

