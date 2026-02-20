namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class RetornoProcessamentoDocumentosFiscais
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public dynamic Carga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete RetornoDadosFrete { get; set; }
    }
}
