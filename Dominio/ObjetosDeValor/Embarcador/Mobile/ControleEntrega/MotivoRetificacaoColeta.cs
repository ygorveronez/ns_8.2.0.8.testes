namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class MotivoRetificacaoColeta
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoOperacao TipoOperacao { get; set; }
    }
}
