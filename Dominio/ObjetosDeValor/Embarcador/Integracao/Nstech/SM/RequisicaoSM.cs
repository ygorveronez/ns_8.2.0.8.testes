using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM
{
    public class RequisicaoSM
    {
        public string solicitante_id { get; set; }
        public string solicitante_token { get; set; }
        public string cliente_documento { get; set; }
        public string fornecedor_id { get; set; }
        //public string gr_id { get; set; }
        public string usuario { get; set; }
        public string senha { get; set; }
        public string token { get; set; }
        public TagsLogin tagsLogin { get; set; }
        public string sm_numero_viagem { get; set; }
        public string sm_tipo_operacao { get; set; }
        public string sm_tipo_manutencao { get; set; }
        public Transportador transportador { get; set; }
        public Embarcador embarcador { get; set; }
        public List<Condutor> condutor { get; set; }
        public Tracionador tracionador { get; set; }
        public List<Reboque> reboque { get; set; }
        public Origem origem { get; set; }
        public Destino destino { get; set; }
        public string data_previsao_inicio { get; set; }
        public string data_previsao_fim { get; set; }
        public List<PontoColeta> ponto_coleta { get; set; }
        public List<PontoEntrega> ponto_entrega { get; set; }
        public List<Temperatura> temperatura { get; set; }
        public Rota rota { get; set; }
        public string sm_observacoes { get; set; }

    }

    public class Condutor
    {
        public string documento_identificador { get; set; }
        public string nome { get; set; }
        public string ddd_telefone { get; set; }
        public string numero_telefone { get; set; }
        public string tipo_contato { get; set; }
        public int uf_rg { get; set; }
        public int pais_origem { get; set; }
    }

    public class Destino
    {
        public string uf { get; set; }
        public string cidade { get; set; }
        public string bairro { get; set; }
        public string rua { get; set; }
        public string numero { get; set; }
        public string codigo { get; set; }
        public string nome { get; set; }
    }

    public class DocumentoColeta
    {
        public string numero_documento { get; set; }
        public string serie_documento { get; set; }
        public string produto_coletado { get; set; }
        public decimal valor_coletado { get; set; }
        public double peso_coletado { get; set; }
        public int quantidade_coletada { get; set; }
        public int tipo_documento { get; set; }
        public string volume_produto { get; set; }
    }

    public class DocumentoEntrega
    {
        public int tipo_documento { get; set; }
        public string numero_documento { get; set; }
        public string serie_documento { get; set; }
        public string centro_custo { get; set; }
        public string produto_entregue { get; set; }
        public decimal valor_entregue { get; set; }
        public decimal valor_documento { get; set; }
        public double peso_entregue { get; set; }
        public int quantidade_entregue { get; set; }

        public DocumentoEntrega Clonar()
        {
            DocumentoEntrega documentoEntregaClonado = (DocumentoEntrega)this.MemberwiseClone();

            return documentoEntregaClonado;
        }
    }

    public class Embarcador
    {
        public string codigo { get; set; }
        public string documento_identificador { get; set; }
        public int pais_origem { get; set; }
    }

    public class Origem
    {
        public string uf { get; set; }
        public string cidade { get; set; }
        public string bairro { get; set; }
        public string rua { get; set; }
        public string numero { get; set; }
        public string codigo { get; set; }
        public string nome { get; set; }
        public int pais { get; set; }
    }

    public class PontoColeta
    {
        public string sequencia { get; set; }
        public string documento_identificador { get; set; }
        public string nome { get; set; }
        public string endereco_uf { get; set; }
        public int endereco_codigo_cidade_ibge { get; set; }
        public string endereco_cep { get; set; }
        public string nome_rua { get; set; }
        public int numero { get; set; }
        public string previsao_chegada { get; set; }
        public string previsao_saida { get; set; }
        public string codigo_destino { get; set; }

        public List<DocumentoColeta> documento_coleta { get; set; }
    }

    public class PontoEntrega
    {
        public string sequencia { get; set; }
        public string documento_identificador { get; set; }
        public string nome_cliente { get; set; }
        public string remetente_descricao { get; set; }
        public int tipo { get; set; }
        public string nome { get; set; }
        public string endereco_uf { get; set; }
        public int endereco_codigo_cidade_ibge { get; set; }
        public string endereco_cep { get; set; }
        public string nome_rua { get; set; }
        public string nome_bairro { get; set; }
        public string nome_complemento { get; set; }
        public string numero { get; set; }
        public int pais { get; set; }
        public string previsao_chegada { get; set; }
        public string previsao_saida { get; set; }
        public int latitude { get; set; }
        public int longitude { get; set; }
        public string codigo_destino { get; set; }
        public List<DocumentoEntrega> documento_entrega { get; set; }
    }

    public class Reboque
    {
        public string identificador { get; set; }
    }

    public class Rota
    {
        public string codigo_gr { get; set; }
        public string codigo_cliente { get; set; }
        public string nome { get; set; }
        public string polyline { get; set; }
    }

    public class TagsLogin
    {
        public string dominio { get; set; }
        public string ipporta_dns { get; set; }
    }

    public class Temperatura
    {
        public string posicao_sensor { get; set; }
        public string tipo_sensor { get; set; }
        public string minimo_ideal { get; set; }
        public string minimo_tolerado { get; set; }
        public string maximo_ideal { get; set; }
        public string maximo_tolerado { get; set; }
    }

    public class Tracionador
    {
        public string identificador { get; set; }
        public int pais_origem { get; set; }
        public int empresa_device_rastreamento { get; set; }
        public int tipo_contato { get; set; }
    }

    public class Transportador
    {
        public string codigo { get; set; }
        public string documento_identificador { get; set; }
        public int pais_origem { get; set; }
    }
}
