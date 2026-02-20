using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk
{
    public class viagens
    {
        public List<viagem> viagem { get; set; }
    }
    public class viagem
    {
        public int viag_codigo_externo { get; set; }
        public int viag_ttra_codigo { get; set; }
        public string viag_carregado { get; set; }
        public string documento_transportador { get; set; }
        public string cnpj_emba { get; set; }
        public string cnpj_gr { get; set; }
        public string viag_numero_manifesto { get; set; }
        public decimal viag_valor_merc_especifica { get; set; }
        public decimal viag_valor_merc_nao_especifica { get; set; }
        public string viag_previsao_inicio { get; set; }
        public string viag_previsao_fim { get; set; }
        public int viag_pgpg_codigo { get; set; }
        public decimal viag_valor_carga { get; set; }
        public decimal viag_valor_ddr { get; set; }
        public decimal viag_peso_total { get; set; }
        public decimal viag_distancia { get; set; }
        public int viag_codigo_pai { get; set; }
        public string viag_descricao_carga { get; set; }
        public string viag_observacao { get; set; }
        public string viag_justificativa_liberacao { get; set; }
        public List<veiculo> veiculos { get; set; }
        public List<motorista> motoristas { get; set; }
        public List<contato> contatos { get; set; }
        public List<ajudante> ajudantes { get; set; }
        public List<terminal> terminais { get; set; }
        public List<tipo_produto> tipos_produtos { get; set; }
        public int rota_codigo { get; set; }
        public string coordenadas { get; set; }
        public string rota_descricao { get; set; }
        public local origem { get; set; }
        public List<local> locais { get; set; }
        public local destino { get; set; }
    }

    public class atualizarStatusViagem
    {
        public status_viagem status_viagem { get; set; }
    }

    public class status_viagem
    {
        public int id_novo_status { get; set; }
    }

    public class veiculo
    {
        public string placa { get; set; }
        public int nro_liberacao { get; set; }
        public string escolta { get; set; }
        public int quantidade_embarcada { get; set; }
    }

    public class motorista
    {
        public string cpf_moto { get; set; }
        public int nro_liberacao { get; set; }
    }

    public class contato
    {
        public string email { get; set; }
        public string fone1 { get; set; }
        public string radio { get; set; }
        public DateTime? previsao_chegada { get; set; }
        public DateTime? previsao_inicio_espera { get; set; }
        public DateTime? previsao_fim_espera { get; set; }
        public DateTime? previsao_inicio_carregamento { get; set; }
        public DateTime? previsao_fim_carregamento { get; set; }
        public DateTime? previsao_inicio_descarregamento { get; set; }
        public DateTime? previsao_fim_descarregamento { get; set; }
        public DateTime? previsao_saida { get; set; }
    }

    public class ajudante
    {
        public string cpf_ajudante { get; set; }
        public int nro_liberacao { get; set; }
        public List<contato> contatos { get; set; }
    }

    public class terminal
    {
        public string term_numero_terminal { get; set; }
        public string tecn_tecnologia { get; set; }
        public string tope_descricao { get; set; }
    }

    public class tipo_produto
    {
        public string produto { get; set; }
        public decimal valor { get; set; }
        public int viag_pgpg_codigo { get; set; }
        public temperatura temperatura { get; set; }
    }

    public class temperatura
    {
        public string descricao { get; set; }
        public int de { get; set; }
        public int ate { get; set; }
        public int sensores { get; set; }
        public string sensor_01 { get; set; }
        public string sensor_02 { get; set; }
        public string sensor_03 { get; set; }
        public string sensor_04 { get; set; }
        public string sensor_05 { get; set; }
        public string sensor_06 { get; set; }
        public string sensor_07 { get; set; }
        public string sensor_08 { get; set; }
        public string sensor_09 { get; set; }
        public string sensor_10 { get; set; }
    }

    public class conhecimento
    {
        public string vlco_numero { get; set; }
        public string vlco_cpf_cnpj { get; set; }
        public decimal vlco_valor { get; set; }
        public List<nota_fiscal> notas_fiscais { get; set; }
    }

    public class nota_fiscal
    {
        public string vnfi_numero { get; set; }
        public string vnfi_pedido { get; set; }
        public decimal vnfi_valor { get; set; }
        public DateTime vnfi_data_fat { get; set; }
        public string vnfi_observacao { get; set; }
        public List<produto> produtos { get; set; }
    }

    public class produto
    {
        public string vpro_descricao { get; set; }
        public string vpro_tipo { get; set; }
        public int vpro_quantidade { get; set; }
        public decimal vpro_valor { get; set; }
    }

    public class local
    {
        public string vloc_descricao { get; set; }
        public string logradouro { get; set; }
        public string complemento { get; set; }
        public string cep { get; set; }
        public string numero { get; set; }
        public string bairro { get; set; }
        public string cida_descricao_ibge { get; set; }
        public string sigla_estado { get; set; }
        public string pais { get; set; }
        public decimal refe_latitude { get; set; }
        public decimal refe_longitude { get; set; }
        public decimal refe_raio { get; set; }
        public string refe_km { get; set; }
        public string refe_bandeira { get; set; }
        public string associar_transportador { get; set; }
        public List<contato> contatos { get; set; }
        public List<conhecimento> conhecimentos { get; set; }
    }

}
