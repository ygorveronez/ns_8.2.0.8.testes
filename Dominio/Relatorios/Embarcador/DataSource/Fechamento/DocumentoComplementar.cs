namespace Dominio.Relatorios.Embarcador.DataSource.Fechamento
{
    public class DocumentoComplementar
    {
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string Tomador { get; set; }
        public string Destinatario { get; set; }
        public string Destino { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal Aliquota { get; set; }

    }
}
