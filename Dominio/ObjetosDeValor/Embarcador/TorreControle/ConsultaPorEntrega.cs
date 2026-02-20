using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class ConsultaPorEntrega
    {
        public string Carga { get; set; }
        public string Pedidos { get; set; }
        public string Notas { get; set; }
        public string Status { get; set; }
        public string Ocorrencia { get; set; }
        public string CidadeOrigem{ get; set; }
        public string Cliente { get; set; }
        public string CidadeDestino { get; set; }
        public DateTime DataCriacaoCarga { get; set; }
        public DateTime DataCarregamento { get; set; }
        public DateTime DataPrevisaoEntrega { get; set; }
        public DateTime DataEntregaReprogramada { get; set; }
        public string Operacao { get; set; }
        public string Veiculo { get; set; }
        public string Motorista { get; set; }
        public string Transportador { get; set; }
    }
}
