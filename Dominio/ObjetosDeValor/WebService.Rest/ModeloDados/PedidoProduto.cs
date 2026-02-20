namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class PedidoProduto
    {
        public string CodigoIntegracao {  get; set; }

        public string Descricao { get; set; }

        public decimal Quantidade { get; set; }

        public decimal PesoUnitario { get; set; }

        public decimal PesoTotal { get; set; }

        public decimal ValorUnitario { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
