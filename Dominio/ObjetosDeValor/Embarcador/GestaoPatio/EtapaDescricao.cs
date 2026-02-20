namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class EtapaDescricao
    {
        public Enumeradores.EtapaFluxoGestaoPatio Enumerador { get; set; }

        public string Descricao { get; set; }

        public string DataInformada { get; set; }

        public int CodigoFilial { get; set; }

        public Enumeradores.TipoFluxoGestaoPatio Tipo { get; set; }
    }
}
