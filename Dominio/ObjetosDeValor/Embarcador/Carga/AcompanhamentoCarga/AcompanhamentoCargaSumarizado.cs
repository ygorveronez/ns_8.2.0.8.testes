namespace Dominio.ObjetosDeValor.Embarcador.Carga.AcompanhamentoCarga
{
    public class AcompanhamentoCargaSumarizado
    {
        public int ColetasEmTempo { get; set; }
        public int ColetasAtraso1 { get; set; }
        public int ColetasAtraso2 { get; set; }
        public int ColetasAtraso3 { get; set; }

        public int EmtransitoOK { get; set; }
        public int EmtransitoAtraso1 { get; set; }
        public int EmtransitoAtraso2 { get; set; }
        public int EmtransitoAtraso3 { get; set; }
        
        public int DestinoEmTempo { get; set; }
        public int DestinoAtraso1 { get; set; }
        public int DestinoAtraso2 { get; set; }
        public int DestinoAtraso3 { get; set; }

    }
}
