using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52
{
    public class Viagem
    {
        public string cd_viagem_cliente { get; set; }
        public List<Carga> cargas { get; set; }
        public string nr_pedido { get; set; }
        public string nr_carga { get; set; }
        public string cd_veiculo { get; set; }
        public string cd_motorista1 { get; set; }
        public string cd_motorista2 { get; set; }
        public string cd_carreta1 { get; set; }
        public string cd_carreta2 { get; set; }
        public string cd_carreta3 { get; set; }
        public string dt_prev_chegada { get; set; }
        public string dt_ini_prev { get; set; }
        public string dt_fim_prev { get; set; }
        public string temperatura_minima { get; set; }
        public string temperatura_maxima { get; set; }
        public decimal vl_frete { get; set; }
        public string uf_origem { get; set; }
        public string cidade_origem_ibge { get; set; }
        public string uf_destino { get; set; }
        public string cidade_destino_ibge { get; set; }
        public string vl_latitude_origem { get; set; }
        public string vl_longitude_origem { get; set; }
        public string cd_embarcador { get; set; }
        public string cd_tipo { get; set; }
        public string cd_gestor { get; set; }
        public string km_percurso { get; set; }
        public string cd_rota { get; set; }
        public ViagemRota rota { get; set; }
        public List<ViagemEntrega> entregas { get; set; }
        public string cd_tipo_operacao { get; set; }
    }
}
