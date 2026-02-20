namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class EnderecoProduto
    {
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public int NivelPrioridade { get; set; }
        public Filial.Filial Filial { get; set; }
    }
}
