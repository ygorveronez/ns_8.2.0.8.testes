namespace Dominio.ObjetosDeValor.Embarcador.Imposto
{
    public class ParametroCalculoISS
    {
        public int IDProposta { get; set; }
        public int IBGEOrigem { get; set; }
        public int IBGEDestino { get; set; }
        public string CodigoServicoOrigem { get; set; }
        public string CodigoServicoDestino { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
