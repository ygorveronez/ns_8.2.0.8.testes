namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Carga
{
    public sealed class ModeloVeicular
    {
        public string CodigoIntegracao { get; set; }

        public string Descricao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga TipoModeloVeicular { get; set; }
    }
}
