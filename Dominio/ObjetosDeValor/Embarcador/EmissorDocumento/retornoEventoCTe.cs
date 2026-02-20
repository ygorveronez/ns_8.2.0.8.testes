namespace Dominio.ObjetosDeValor.Embarcador.EmissorDocumento
{
    public class RetornoEventoCTe
    {
        public Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cteOracle { get; set; }
        public Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle { get; set; }
        public dynamic objeto { get; set; }
    }
}
