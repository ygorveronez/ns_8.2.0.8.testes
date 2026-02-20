namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class FinalidadeProduto
    {
        public string CodigoIntegracao
        {
            get; set;
        }
        public string Descricao
        {
            get; set;
        }
        public Dominio.ObjetosDeValor.Embarcador.Financeiro.PlanoDeConta PlanoContaCredito { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Financeiro.PlanoDeConta PlanoContaDebito { get; set; }
    }
}
