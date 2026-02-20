using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52
{
    public class ViagemEntrega
    {
        public string cd_cliente_destino { get; set; }
        public string cd_cliente_origem { get; set; }
        public string cd_cliente_expedidor { get; set; }
        public string cd_cliente_recebedor { get; set; }
        public string dt_prev_chegada { get; set; }
        public int nr_cte { get; set; }
        public string serie_cte { get; set; }
        public decimal peso_cte { get; set; }
        public decimal m3_cte { get; set; }
        public string cd_tipo_carga { get; set; }
        public List<ViagemEntregaNotaFiscal> notasfiscais { get; set; }
        public List<ViagemEntregaProduto> produtos { get; set; }
    }
}
