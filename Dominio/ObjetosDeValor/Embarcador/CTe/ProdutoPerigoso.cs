namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ProdutoPerigoso
    {

        public string ClasseRisco { get; set; }
        public int NumeroONU { get; set; }
        public string NomeApropriado { get; set; }
        public string Quantidade { get; set; }
        public string Volumes { get; set; }
        public string Grupo { get; set; }
        public string PontoFulgor { get; set; }
        public decimal? QuantidadeTotal { get; set; }
        public Enumeradores.UnidadeDeMedidaCTeAereo? UnidadeDeMedida { get; set; }
    }
}
