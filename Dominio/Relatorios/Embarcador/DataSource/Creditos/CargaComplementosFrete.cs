using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Creditos
{
    public class CargaComplementosFrete
    {
        public int Codigo { get; set; }
        public string Solicitante { get; set; }
        public string Solicitado { get; set; }
        public string Carga { get; set; }
        public DateTime Data { get; set; }
        public string Destinatario { get; set; }
        public string Remetente { get; set; }
        public string Filial { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Transportador { get; set; }
        public string Veiculo { get; set; }
        public decimal ValorSolicitado { get; set; }
        public decimal ValorLiberado { get; set; }
        public string Situacao { get; set; }
        public string Motivo { get; set; }
        public string RetornoSolicitacao { get; set; }
        public string MotivoAdicionalFrete { get; set; }

    }
}
