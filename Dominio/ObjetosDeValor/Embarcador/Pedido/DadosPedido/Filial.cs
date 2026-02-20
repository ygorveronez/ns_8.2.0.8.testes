namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class Filial
    {
        public int Codigo { get; set; }

        public string Cnpj { get; set; }

        public string Descricao { get; set; }

        public bool ExigirPreCargaMontagemCarga { get; set; }
    }
}
