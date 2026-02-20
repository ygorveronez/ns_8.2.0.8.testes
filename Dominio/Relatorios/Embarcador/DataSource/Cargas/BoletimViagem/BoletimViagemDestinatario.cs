namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem
{
    public class BoletimViagemDestinatario
    {
        public int CodigoCarga { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Destinatario { get; set; }
        public virtual double CPF_CNPJ { get; set; }
        public string Endereco { get; set; }
        public int Numero { get; set; }
        public string Serie { get; set; }
        public decimal Valor { get; set; }
        public string HorariosDescarga { get; set; }
    }
}
