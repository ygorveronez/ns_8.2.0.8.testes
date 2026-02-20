using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus
{

    public class Error
    {
        public string cod { get; set; }
        public object valor { get; set; }
        public string campo { get; set; }
        public string mensagem { get; set; }
    }

    public class RetornoSucesso
    {
        public int cod_status { get; set; }
        public string status { get; set; }
        public string cod_viagem { get; set; }
    }


    public class IntegarcaoRetorno
    {
        public List<RetornoSucesso> sucesso { get; set; }
        public List<object> success { get; set; }
        public List<Error> error { get; set; }
    }

    public class Veiculo
    {
        public string placa { get; set; }
        public int tipo_veiculo { get; set; }
    }

    public class Transportador
    {
        public string documento_transportador { get; set; }
        public string nome { get; set; }
    }


    public class Motorista
    {
        public string cpf_moto { get; set; }
        public string cpf_motorista { get; set; }
        public string nome { get; set; }
    }

    public class Terminai
    {
        public string term_numero_terminal { get; set; }
        public string tecn_tecnologia { get; set; }
    }

    public class Origem
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
        public string refe_raio { get; set; }
        public string associar_transportador { get; set; }
        public string cnpj { get; set; }
    }

    public class NotasFiscais
    {
        public string vnfi_numero { get; set; }
        public string vnfi_pedido { get; set; }
        public string vnfi_valor { get; set; }
        public string vnfi_data_fat { get; set; }
        public string vnfi_observacao { get; set; }
    }

    public class Conhecimento
    {
        public string vlco_numero { get; set; }
        public double vlco_valor { get; set; }
        public string vlco_cpf_cnpj { get; set; }
        public List<NotasFiscais> notas_fiscais { get; set; }
    }

    public class Locais
    {
        public int vloc_sequencia { get; set; }
        public string vloc_descricao { get; set; }
        public int tipo_parada { get; set; }
        public int tipo_local { get; set; }
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
        public string refe_raio { get; set; }
        public string associar_transportador { get; set; }
        public string cnpj { get; set; }
        public List<Conhecimento> conhecimentos { get; set; }
    }

    public class NotasFiscai2
    {
        public string vnfi_numero { get; set; }
        public string vnfi_pedido { get; set; }
        public string vnfi_valor { get; set; }
        public string vnfi_data_fat { get; set; }
        public string vnfi_observacao { get; set; }
    }


    public class Destino
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
        public string refe_raio { get; set; }
        public string associar_transportador { get; set; }
        public string cnpj { get; set; }
        public List<Conhecimento> conhecimentos { get; set; }
    }

    public class CadastroMotorista
    {
        public List<Motorista> motorista { get; set; }
    }

    public class CadastroVeiculo
    {
        public List<Veiculo> veiculo { get; set; }
    }

    public class CadastroTransportador
    {
        public List<Transportador> transportador { get; set; }
    }

    public class Viagem
    {
        public string viag_codigo_externo { get; set; }
        public string documento_transportador { get; set; }
        public string cnpj_emba { get; set; }        
        public string cnpj_gr { get; set; }
        public string viag_ttra_codigo { get; set; }
        public List<Veiculo> veiculos { get; set; }
        public List<Motorista> motoristas { get; set; }
        public List<Terminai> terminais { get; set; }
        public string viag_previsao_inicio { get; set; }
        public string viag_previsao_fim { get; set; }
        public string alterar_pgr_edicao_sm { get; set; }
        public int viag_pgpg_codigo { get; set; }
        public double viag_valor_carga { get; set; }
        public double viag_peso_total { get; set; }
        public string viag_numero_manifesto { get; set; }
        public string viag_descricao_carga { get; set; }
        public int rota_codigo { get; set; }
        public string coordenadas { get; set;}
        public double viag_distancia { get; set; }
        public string rota_descricao { get; set;}
        public Origem origem { get; set; }
        public List<Locais> locais { get; set; }
        public Destino destino { get; set; }

        public Temperatura temperatura { get; set; }
    }

    public class Temperatura
    {
        public string descricao { get; set; }
        public int de { get; set; }
        public int ate { get; set; }
    }

    public class StatusViagem
    {
        public int id_novo_status { get; set; }
    }

    public class IntegracaoStatusViagem
    {
        public StatusViagem status_viagem { get; set; }
    }

    public class IntegracaoViagem
    {
        public bool vincularEmpresaLocal { get; set; }
        public List<Viagem> viagem { get; set; }
    }
    
}
