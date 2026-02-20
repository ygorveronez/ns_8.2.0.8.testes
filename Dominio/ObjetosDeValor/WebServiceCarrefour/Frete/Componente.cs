namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Frete
{
    public sealed class Componente
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }

        public string Descricao { get; set; }

        public string CodigoIntegracao { get; set; }

        public int Codigo { get; set; }
    }
}
