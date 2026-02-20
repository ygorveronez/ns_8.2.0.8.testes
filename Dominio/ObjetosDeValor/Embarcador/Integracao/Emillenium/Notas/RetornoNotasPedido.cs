using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas
{
    public partial class RetornoNotasPedido
    {
        [JsonProperty("value")]
        public List<Pedido> Value { get; set; }
    }

    public partial class EnderecoEntrega
    {
        public string logradouro { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public string cep { get; set; }
        public string ddd { get; set; }
        public string fone { get; set; }
        public object ramal { get; set; }
        public object fax { get; set; }
        public string contato { get; set; }
        public string dicas_endereco { get; set; }
        public object complemento { get; set; }
        public string numero { get; set; }
        public object ddd_cel { get; set; }
        public string nome_pais { get; set; }
        public string desc_tipo_endereco { get; set; }
        public object tipo_sexo { get; set; }
        public object grau_relacionamento { get; set; }
        public string cod_mun_ibge { get; set; }
        public string pais_dest { get; set; }
        public int? cod_endereco { get; set; }
    }

    public partial class Endereco
    {
        public string logradouro { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public string cep { get; set; }
        public string ddd { get; set; }
        public string fone { get; set; }
        public object ramal { get; set; }
        public object fax { get; set; }
        public string contato { get; set; }
        public string dicas_endereco { get; set; }
        public object complemento { get; set; }
        public string numero { get; set; }
        public object ddd_cel { get; set; }
        public string nome_pais { get; set; }
        public string desc_tipo_endereco { get; set; }
        public object tipo_sexo { get; set; }
        public object grau_relacionamento { get; set; }
        public string cod_mun_ibge { get; set; }
        public string pais_dest { get; set; }
        public int? cod_endereco { get; set; }
    }

    public partial class Cliente
    {
        public string cod_cliente { get; set; }
        public string nome { get; set; }
        public string ddd_cel { get; set; }
        public string cel { get; set; }
        public string rg { get; set; }
        public string data_aniversario { get; set; }
        public string obs { get; set; }
        public string e_mail { get; set; }
        public string tipo_sexo { get; set; }
        public object tratamento { get; set; }
        public bool? maladireta { get; set; }
        public List<object> categorias { get; set; }
        public string cnpj { get; set; }
        public string pf_pj { get; set; }
        public string ie { get; set; }
        public string fantasia { get; set; }
        public string grupo_loja { get; set; }
        public string parentesco { get; set; }
        public List<EnderecoEntrega> endereco_entrega { get; set; }
        public List<Endereco> endereco { get; set; }
        public string cpf { get; set; }
        public int? vitrine { get; set; }
        public object dados_adicionais { get; set; }
        public List<object> endereco_cobranca { get; set; }
        public string e_mail_nfe { get; set; }
        public string suframa { get; set; }
        public string cargo { get; set; }
        public string ufie { get; set; }
        public string id_externo { get; set; }
        public string aplicar_icms_st { get; set; }
        public string cgc { get; set; }
    }

    public partial class Produto
    {
        public int? produto { get; set; }
        public int? cor { get; set; }
        public int? estampa { get; set; }
        public string tamanho { get; set; }
        public int? quantidade { get; set; }
        public double preco { get; set; }
        public string item { get; set; }
        public string obs_item { get; set; }
        public string altura { get; set; }
        public string comprimento { get; set; }
        public string largura { get; set; }
        public string sku { get; set; }
        public string barra { get; set; }
        public string cod_pedidov { get; set; }
        public string cfop { get; set; }
        public DateTime? data_entrega { get; set; }
        public string tipo_icms { get; set; }
        public double? b_icms { get; set; }
        public double? al_icms { get; set; }
        public double? v_icms { get; set; }
        public double? al_pis { get; set; }
        public double? v_pis { get; set; }
        public double? al_cofins { get; set; }
        public double? v_cofins { get; set; }
        public double? v_icms_uf_dest { get; set; }
        public double? v_icms_uf_remet { get; set; }
        public double? it_valor_frete { get; set; }
        public double? it_valor_desconto { get; set; }
        public double? it_valor_cortesia { get; set; }
        public string desconto { get; set; }
        public string endereco { get; set; }
        public string encomenda { get; set; }
        public string presente { get; set; }
        public string componente_kit { get; set; }
        public string cod_cor { get; set; }
        public string desc_cor { get; set; }
        public string cod_estampa { get; set; }
        public string desc_estampa { get; set; }
        public string desc_produto { get; set; }
        public string url_imagem { get; set; }
        public string brindesite { get; set; }
        public string status { get; set; }
        public string desc_status { get; set; }
        public string url_tracking_pedido { get; set; }
        public string nota_ref { get; set; }
        public double? b_pis { get; set; }
        public double? b_cofins { get; set; }
        public string cod_produto { get; set; }
        public string ncm { get; set; }
        public string vitrine_produto_sku { get; set; }
        public string cod_prefaturamento { get; set; }
        public string cst { get; set; }
        public double? icms { get; set; }
        public double? ipi { get; set; }
        public double? v_ipi { get; set; }
        public double? icmss { get; set; }
        public double? v_icmss { get; set; }
        public string unidade { get; set; }
        public double? valor_unitario { get; set; }
        public double? valor_total_imposto { get; set; }
        public double? icms_uf_dest { get; set; }
        public string id_externo { get; set; }
        public string descricao_cfop { get; set; }
        public string cod_trba_iss { get; set; }
        public double? b_iss { get; set; }
        public string preco_icms_iss { get; set; }
        public double? al_iss { get; set; }
        public string lote { get; set; }
        public double? valor_total_bruto { get; set; }
        public double? valor_total_liquido { get; set; }
        public string bcuf_dest { get; set; }
        public string pfcp_uf_dest { get; set; }
        public string vfcpufdest { get; set; }
        public string fcp_uf_dest { get; set; }
        public string v_fcp_uf_dest { get; set; }
    }

    public partial class Lancamento
    {
        public DateTime? data_vencimento { get; set; }
        public double? valor_inicial { get; set; }
        public int? tipo_pgto { get; set; }
        public int? conta { get; set; }
        public string documento { get; set; }
        public string banco { get; set; }
        public string agencia { get; set; }
        public string c_c { get; set; }
        public string nsu { get; set; }
        public string autorizacao { get; set; }
        public string rede { get; set; }
        public int? numparc { get; set; }
        public int? numcontrato { get; set; }
        public int? bandeira { get; set; }
        public int? operadora { get; set; }
        public string numero_cartao { get; set; }
        public int? mes_validade_cartao { get; set; }
        public int? ano_validade_cartao { get; set; }
        public int? cod_seguranca_cartao { get; set; }
        public string nome_portador_cartao { get; set; }
        public string cpf_portador_cartao { get; set; }
        public string transacao_aprovada { get; set; }
        public string mensagem_aprova_facil { get; set; }
        public string transacao { get; set; }
        public string cod_autorizacao_cartao { get; set; }
        public string comprovante_administradora { get; set; }
        public DateTime? data_aprova_facil { get; set; }
        public bool adiantamento { get; set; }
        public bool previsao { get; set; }
        public string duplicata { get; set; }
        public DateTime? data_pagamento { get; set; }
        public string desc_tipo { get; set; }
        public string numero_credito { get; set; }
        public int? cod_tipo_pgto { get; set; }
        public string item { get; set; }
        public string pedido { get; set; }
        public string cod_condicao_pagto { get; set; }
        public string desc_cond_pagto { get; set; }
        public int? desconto { get; set; }
        public int? perc_desconto { get; set; }
        public double? valor_liquido { get; set; }
        public double? valor_inicial_porpor { get; set; }
        public DateTime? data_emissao { get; set; }
        public string desc_bandeira { get; set; }
        public string desc_operadora { get; set; }
        public string cod_bandeira { get; set; }

    }

    public partial class Desconto
    {
        public string descricao { get; set; }
        public int? tipo_desc { get; set; }
        public string id_externo_desconto { get; set; }
        public double? desconto { get; set; }
        public string sku { get; set; }
        public string item { get; set; }
        public string vitrine_produto_sku { get; set; }
        public string barra { get; set; }
        public string nome { get; set; }
    }

    public partial class LancamentoOrigem
    {
        public DateTime? data_emissao { get; set; }
        public double? valor_inicial { get; set; }
        public string desc_tipo { get; set; }
        public string desc_bandeira { get; set; }
        public int? parcela { get; set; }
        public string nsu { get; set; }
        public string autorizacao { get; set; }
        public int? operadora { get; set; }
        public DateTime? data_pagamento { get; set; }
        public int? tipo_pgto { get; set; }
    }

    public partial class DctVitrinePedido
    {
        public string utmsource { get; set; }
        public string marketingtags { get; set; }
    }

    public partial class CargaCorrecao
    {
        public string data_hora { get; set; }
        public string texto_digitado { get; set; }

    }

    public partial class Cancelamento
    {
        public string nota { get; set; }
        public string data_cancelou { get; set; }
        public string justificativa { get; set; }

    }

    public partial class RetornoProcessamento
    {
        public bool status { get; set; }
        public string mensagem { get; set; }

    }

    public partial class Pedido
    {
        public int? saida { get; set; }
        public string tipo_operacao { get; set; }
        public string romaneio { get; set; }
        public string nf { get; set; }
        public DateTime? data { get; set; }
        public DateTime? data_atualizacao { get; set; }
        public bool cancelado { get; set; }
        public List<Cliente> cliente { get; set; }
        public List<Produto> produtos { get; set; }
        public List<Lancamento> lancamentos { get; set; }
        public int? quantidade { get; set; }
        public double? total { get; set; }
        public double? v_desconto { get; set; }
        public double? v_frete { get; set; }
        public double? valor_final { get; set; }
        public string chave_nf { get; set; }
        public string serie_nf { get; set; }
        public string protocolo_nf { get; set; }
        public string recibo_nf { get; set; }
        public DateTime? data_autorizacao_nf { get; set; }
        public string desc_evento { get; set; }
        public int trans_id { get; set; }
        public double? v_icms_uf_dest { get; set; }
        public double? v_icms_uf_remet { get; set; }
        public object datacancelamento { get; set; }
        public List<Desconto> descontos { get; set; }
        public string cif_fob { get; set; }
        public string modalidade_frete { get; set; }
        public int? cod_operacao { get; set; }
        public string n_volumes { get; set; }
        public string status { get; set; }
        public string mensagem_nfe { get; set; }
        public DateTime? data_hora_emissao { get; set; }
        public string cod_emissor { get; set; }
        public string nome_emissor { get; set; }
        public string cnpj_emissor { get; set; }
        public string ie_emissor { get; set; }
        public string cup_fis { get; set; }
        public string digitada { get; set; }
        public string modelo { get; set; }
        public double? icms { get; set; }
        public double? v_icms { get; set; }
        public double? ipi { get; set; }
        public double? v_ipi { get; set; }
        public double? icmss { get; set; }
        public double? v_icmss { get; set; }
        public double? b_pis { get; set; }
        public double? v_pis { get; set; }
        public double? b_cofins { get; set; }
        public double? v_cofins { get; set; }
        public double? valor_desp_acess { get; set; }
        public double? valor_seguro { get; set; }
        public object valor_desconto { get; set; }
        public double? valor_nf { get; set; }
        public double? valor_produtos { get; set; }
        public double? peso_l { get; set; }
        public double? peso_b { get; set; }
        public string moeda { get; set; }
        public string unidade_peso { get; set; }
        public string canal_destribuicao { get; set; }
        public string cod_transportadora { get; set; }
        public string nome_transportadora { get; set; }
        public string cidade_transportadora { get; set; }
        public string uf_transportadora { get; set; }
        public object placa_transportadora { get; set; }
        public string cnpj_transportadora { get; set; }
        public string ie_transportadora { get; set; }
        public string endereco_transportadora { get; set; }
        public object placa { get; set; }
        public string tipo_veiculo { get; set; }
        public string especie { get; set; }
        public object marca { get; set; }
        public string numeracao { get; set; }
        public string xml { get; set; }
        public int? vitrine { get; set; }
        public string cod_filial_retira { get; set; }
        public string cnpj_filial_retira { get; set; }
        public string n_pedido_cliente { get; set; }
        public string cod_pedidov { get; set; }
        public string n_pedido_cliente_ref { get; set; }
        public string cod_pedidov_ref { get; set; }
        public double? v_fcp_uf_dest { get; set; }
        public string tipo_pedido { get; set; }
        public object n_fabr_impr { get; set; }
        public List<LancamentoOrigem> lancamento_origem { get; set; }
        public string pedido_original { get; set; }
        public List<Cancelamento> cancelamentos { get; set; }
        public List<CargaCorrecao> carta_correcao { get; set; }
        public object cod_embarque { get; set; }
        public string desc_tipo_pedido { get; set; }
        public string desc_tipo_frete { get; set; }
        public string cod_pedidov_pai { get; set; }
        public DateTime? data_entrega_pedido { get; set; }
        public List<DctVitrinePedido> dct_vitrine_pedido { get; set; }
        public string cod_filial { get; set; }
        public object transportadora_original_rnl { get; set; }
        public object transportadora_rnl_id { get; set; }
        public int? id_transportadora { get; set; }
    }



}
