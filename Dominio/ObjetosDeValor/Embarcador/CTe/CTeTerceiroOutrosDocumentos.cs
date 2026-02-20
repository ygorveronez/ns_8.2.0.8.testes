namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class CTeTerceiroOutrosDocumentos
    {
        public CTeTerceiro CTeTerceiro { get; set; }
        public Enumeradores.TipoOutroDocumento Tipo { get; set; }
        public string Numero { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string NCM { get; set; }
    }
}
