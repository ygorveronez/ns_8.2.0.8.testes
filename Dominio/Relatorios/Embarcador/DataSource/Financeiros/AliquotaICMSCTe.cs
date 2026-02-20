namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class AliquotaICMSCTe
    {
        public int Codigo { get; set; }
        public string EstadoEmpresa { get; set; }
        public string EstadoOrigem { get; set; }
        public string EstadoDestino { get; set; }
        public string CST { get; set; }
        public decimal Aliquota { get; set; }
        public string Atividade { get; set; }
        public string AtividadeDestinatario { get; set; }
        public int CFOP { get; set; }

        public int CFOPCompra
        {
            get
            {
                return CFOP - 4000;
            }
        }
    }
}
