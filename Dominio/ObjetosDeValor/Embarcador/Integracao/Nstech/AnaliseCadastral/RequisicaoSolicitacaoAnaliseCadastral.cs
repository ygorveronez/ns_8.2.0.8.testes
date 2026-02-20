using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral
{
    public class RequisicaoSolicitacaoAnaliseCadastral
    {
        public string solicitante_id { get; set; }
        public string solicitante_token { get; set; }
        public int cliente_id { get; set; }
        public string cliente_cnpj { get; set; }

        [JsonProperty(PropertyName = "cnpj_embarcador", Required = Required.AllowNull)]
        public string cnpj_embarcador { get; set; }
        public int fornecedor_id { get; set; }
        public string usuario { get; set; }
        public string senha { get; set; }
        public string token { get; set; }
        public TagsLogin tagsLogin { get; set; }
        public int solicitacao_pesquisa_numero { get; set; }
        public string modo_pesquisa { get; set; }
        public string tipo_pesquisa { get; set; }
        public string codigo_conjunto { get; set; }
        public string codigo_de_relacao { get; set; }
        public List<Veiculo> veiculos { get; set; }
        public Profissional profissional { get; set; }
        public Viagem viagem { get; set; }
    }

    public class Cnh
    {
        public string numero_registro { get; set; }
        public string categoria { get; set; }
        public string sigla_estado { get; set; }
        public string data_vencimento { get; set; }
        public string data_emissao_cnh { get; set; }
        public string data_primeira_emissao_cnh { get; set; }
        public string numero_cnh { get; set; }
        public string codigo_seguranca { get; set; }
        public int cidade { get; set; }
        public string foto_documento { get; set; }
    }

    public class Contato
    {
        public int tipo_contato { get; set; }
        public string nome_contato { get; set; }
        public string ddd_contato { get; set; }
        public string telefone_contato { get; set; }
        public string descricao { get; set; }
    }

    public class DadosComplementares
    {
        public string vitima_roubo { get; set; }
        public string acidentes_sofridos { get; set; }
        public int viagens_realizadas_unidade_medida { get; set; }
        public int viagens_realizadas_quantidade { get; set; }
    }

    public class Endereco
    {
        public string uf { get; set; }
        public int codigo_cidade_ibge { get; set; }
        public string cep { get; set; }
        public string nome_bairro { get; set; }
        public string nome_rua { get; set; }
        public string numero_casa { get; set; }
    }

    public class Imobilizador
    {
        public string cod_empresa { get; set; }
        public string dsId_empresa { get; set; }
    }

    public class Profissional
    {
        public int pais_origem { get; set; }
        public string rg { get; set; }
        public string orgao_rg { get; set; }
        public string data_emissao_RG { get; set; }
        public string cpf { get; set; }
        public string nome { get; set; }
        public string genero { get; set; }
        public string nome_mae { get; set; }
        public string nome_pai { get; set; }
        public string estado_civil { get; set; }
        public string data_nascimento { get; set; }
        public int codigo_cidade_nascimento_ibge { get; set; }
        public string nome_rua { get; set; }
        public string numero_casa { get; set; }
        public int codigo_profissao { get; set; }
        public string vinculo { get; set; }
        public string ddd { get; set; }
        public string telefone { get; set; }
        public string uf_rg { get; set; }
        public Endereco endereco { get; set; }
        public List<Contato> contatos { get; set; }
        public Cnh cnh { get; set; }
        public DadosComplementares dados_complementares { get; set; }
    }

    public class Proprietario
    {
        public string documento { get; set; }
        public string numero_rg { get; set; }
        public string uf_rg { get; set; }
        public string nome { get; set; }
        public object nome_mae { get; set; }
        public string nome_pai { get; set; }
        public string nome_rua { get; set; }
        public string numero_rua { get; set; }
        public string nome_bairro { get; set; }
        public string rntrc { get; set; }
        public string ddd { get; set; }
        public string telefone { get; set; }
        public string tipo_proprietario { get; set; }
        public int pais_origem { get; set; }
        public int estado_origem { get; set; }
        public int cidade_nascimento_ibge { get; set; }
        public object data_nascimento { get; set; }
        public int endereco_codigo_cidade_ibge { get; set; }
        public string endereco_cep { get; set; }
    }

    public class Rastreador
    {
        public string empresa_rastreador { get; set; }
        public string nome_rastreador { get; set; }
        public Imobilizador imobilizador { get; set; }
    }

    public class Veiculo
    {
        public int pais_origem { get; set; }
        public string placa { get; set; }
        public string sigla_estado { get; set; }
        public string sigla_cidade { get; set; }
        public string tipo_veiculo { get; set; }
        public string tipo_carroceria { get; set; }
        public string numero_chassi { get; set; }
        public string fabricante_veiculo { get; set; }
        public string modelo_veiculo { get; set; }
        public string cor_veiculo { get; set; }
        public string tipo_combustivel { get; set; }
        public int ano_fabricacao { get; set; }
        public int ano_modelo { get; set; }
        public string codigo_antt { get; set; }
        public string data_rntrc { get; set; }
        public string vinculo { get; set; }
        public string numero_registro { get; set; }
        public string data_emissao_registro { get; set; }
        public string numero_renavam { get; set; }
        //public string Codigo_Seguranca_Crlv { get; set; }
        //public string Crlv_Imagem { get; set; }
        public string acessorio { get; set; }
        public List<Rastreador> rastreador { get; set; }
        public Proprietario proprietario { get; set; }
    }

    public class Viagem
    {
        public string produto { get; set; }
        public string cnpj_embarcador { get; set; }
        public string cnpj_transportador { get; set; }
        public string carga_tipo { get; set; }
        public int carga_valor { get; set; }
        public int pais_origem { get; set; }
        public int uf_origem { get; set; }
        public int cidade_origem { get; set; }
        public int pais_destino { get; set; }
        public int uf_destino { get; set; }
        public int cidade_destino { get; set; }
        public string carga_nome { get; set; }
    }
}
