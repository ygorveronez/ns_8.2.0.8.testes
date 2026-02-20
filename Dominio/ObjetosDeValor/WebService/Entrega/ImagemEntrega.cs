namespace Dominio.ObjetosDeValor.WebService.Entrega
{
    public class ImagemEntrega
    {
        public int ProtocoloImagem { get; set; }
        public string Imagem { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega Entrega { get; set; }
    }
}
