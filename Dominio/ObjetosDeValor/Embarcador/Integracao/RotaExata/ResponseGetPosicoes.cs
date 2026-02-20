using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RotaExata
{
    public class ResponseGetPosicoes
    {
        public List<Datum> data { get; set; }
    }

    public class Datum
    {
        public Posicao posicao { get; set; }
    }

    public class Adesao
    {
        public int id { get; set; }
        public int? empresa_id { get; set; }
        public int? central_id { get; set; }
        public string rastreador_id { get; set; }
        public string vei_placa { get; set; }
        public string dt_instalacao { get; set; }
        public string vei_descricao { get; set; }
        public string fusohorario { get; set; }
        public string horarioverao { get; set; }
        public string ico_cor { get; set; }
        public string ico_tipo { get; set; }
        public string vei_ano { get; set; }
        public string vei_renavam { get; set; }
        public string vei_chassi { get; set; }
        public string adesao_id_locada { get; set; }
        public string adesao_id_locadora { get; set; }
        public string dt_fim_locacao { get; set; }
        public string dt_inicio_locacao { get; set; }
        public string tipo_veiculo { get; set; }
        public string usuario_id_app { get; set; }
        public string qrcode_codigo { get; set; }
        public int? ativar_buzzer { get; set; }
        public string fim_vinculo { get; set; }
        public string horario_rastreamento { get; set; }
        public string uf { get; set; }
        public int? operacao_id { get; set; }
        //public Categoria categoria { get; set; }
        //public Marca marca { get; set; }
        //public Modelo modelo { get; set; }
        //public Entradas entradas { get; set; }
        //public Saidas saidas { get; set; }
        public string migracao { get; set; }
        public string identificador_motorista { get; set; }
    }

    public class Categoria
    {
        public int id { get; set; }
        public string categoria { get; set; }
        public string categoria_id { get; set; }
    }

    public class Entradas
    {
        public string entrada1 { get; set; }
        public string entrada2 { get; set; }
        public string entrada3 { get; set; }
    }

    public class Geojson
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Marca
    {
        public int id { get; set; }
        public string marca { get; set; }
    }

    public class Modelo
    {
        public int id { get; set; }
        public string modelo { get; set; }
    }

    public class Posicao
    {
        public string motorista_id { get; set; }
        public long packetID { get; set; }
        public int? header { get; set; }
        public DateTime dt_posicao { get; set; }
        public int? sequence { get; set; }
        public long rastreador_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int? velocidade { get; set; }
        public long odometro_gps { get; set; }
        public long odometro_original { get; set; }
        public int? horimetro { get; set; }
        public int? bateria_conectada { get; set; }
        public double tensao_bateria_veiculo { get; set; }
        public string tipo_rastreador { get; set; }
        public string gateway_entrada { get; set; }
        public int? satelites { get; set; }
        public int? ignicao { get; set; }
        public int? entrada1 { get; set; }
        public int? entrada2 { get; set; }
        public int? entrada3 { get; set; }
        public int? bt_panico { get; set; }
        public int? bloqueio { get; set; }
        public string motorista_key { get; set; }
        public Geojson geojson { get; set; }
        public DateTime dt_transferencia { get; set; }
        public DateTime dt_padrao { get; set; }
        public int? adesao_id { get; set; }
        public string _id { get; set; }
        public string versao_gateway { get; set; }
        public long timestamp { get; set; }
        public string fila { get; set; }
        public int? empresa_id { get; set; }
        public Adesao adesao { get; set; }
        public DateTime dt_apagar { get; set; }
        public string fuso_horario { get; set; }
        public DateTime dt_ativo { get; set; }
        public int? parado_ligado { get; set; }
    }

    public class Saidas
    {
        public string saida1 { get; set; }
        public string saida2 { get; set; }
        public string saida3 { get; set; }
    }
}
