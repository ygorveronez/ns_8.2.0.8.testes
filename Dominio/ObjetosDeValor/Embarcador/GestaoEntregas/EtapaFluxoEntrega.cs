namespace Dominio.ObjetosDeValor.Embarcador.GestaoEntregas
{
    public class EtapaFluxoEntrega
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio Etapa { get; set; }

        public bool EtapaLiberada { get; set; }

        public dynamic Pedido  { get; set; }
    }
}
