namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class CTeTerceiroQuantidade
    {
        public CTeTerceiro CTeTerceiro { get; set; }
        public Dominio.Enumeradores.UnidadeMedida Unidade { get; set; }
        public string TipoMedida { get; set; }
        public decimal Quantidade { get; set; }
    }
}
