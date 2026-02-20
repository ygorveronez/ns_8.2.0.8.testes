using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ViaLink
{
    public class ResponsePosicoes
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<TendenciaEntregaData> data { get; set; }
    }

    public class TendenciaEntregaData
    {
        public string ras_ras_id { get; set; }
        public string ras_prd_id { get; set; }
        public string ras_ras_id_aparelho { get; set; }
        public string ras_cli_desc { get; set; }
        public string ras_cli_id { get; set; }
        public string ras_eve_latitude { get; set; }
        public string ras_eve_longitude { get; set; }
        public string ras_eve_voltagem_backup { get; set; }
        public string ras_eve_porc_bat_backup { get; set; }
        public string ras_eve_data_gps { get; set; }
        public string ras_eve_data_enviado { get; set; }
        public string ras_ras_data_ult_comunicacao { get; set; }
        public string ras_ras_sinal_gps { get; set; }
        public string ras_eve_nivel_sinal_gprs { get; set; }
        public string ras_mot_id { get; set; }
        public string ras_mot_nome { get; set; }
        public string ras_eve_direcao { get; set; }
        public string ras_eve_gps_status { get; set; }
        public string ras_eve_hodometro { get; set; }
        public string ras_eve_horimetro { get; set; }
        public string ras_eve_ignicao { get; set; }
        public string ras_eve_satelites { get; set; }
        public string ras_eve_velocidade { get; set; }
        public string ras_eve_voltagem { get; set; }
        public string ras_vei_placa { get; set; }
        public string ras_vei_tipo { get; set; }
        public string ras_vei_veiculo { get; set; }
        public string ras_vei_id { get; set; }
        public string ras_vei_tag_identificacao { get; set; }
        public string campos_extras { get; set; }
        public string total_combustivel { get; set; }
        public string ibutton { get; set; }
        //public List<string> sensor_combustivel { get; set; }
        //public List<string> sensor_temperatura { get; set; }
        //public List<string> ras_eve_output { get; set; }
        //public List<string> ras_eve_input { get; set; }
    }
}
