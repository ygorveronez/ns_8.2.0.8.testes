using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium
{
    public partial class RetornoListaPedidos
    {
        [JsonProperty("value")]
        public List<Pedido> Value { get; set; }
    }

    public partial class Pedido
    {
        [JsonProperty("pedidov")]
        public long Pedidov { get; set; }

        [JsonProperty("cod_pedidov")]
        public string CodPedidov { get; set; }

        [JsonProperty("status_workflow_desc")]
        public string StatusWorkflowDesc { get; set; }

        [JsonProperty("data_emissao")]
        public DateTime DataEmissao { get; set; }

        [JsonProperty("data_entrega")]
        public DateTime DataEntrega { get; set; }

        [JsonProperty("data_atualizacao")]
        public DateTime DataAtualizacao { get; set; }

        [JsonProperty("aprovado")]
        public bool Aprovado { get; set; }

        [JsonProperty("cancelado")]
        public object Cancelado { get; set; }

        [JsonProperty("cliente")]
        public List<Cliente> Cliente { get; set; }

        [JsonProperty("produtos")]
        public List<Produto> Produtos { get; set; }

        [JsonProperty("lancamentos")]
        public List<Lancamento> Lancamentos { get; set; }

        [JsonProperty("quantidade")]
        public long Quantidade { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("valor_final")]
        public double ValorFinal { get; set; }

        [JsonProperty("nome_transportadora")]
        public string NomeTransportadora { get; set; }

        [JsonProperty("desc_tipo_frete")]
        public string DescTipoFrete { get; set; }

        [JsonProperty("cod_tipo_frete")]
        public string CodTipoFrete { get; set; }

        [JsonProperty("efetuado")]
        public bool Efetuado { get; set; }

        [JsonProperty("n_pedido_cliente")]
        public string NPedidoCliente { get; set; }

        [JsonProperty("desc_status")]
        public string DescStatus { get; set; }

        [JsonProperty("nome_cliente")]
        public string NomeCliente { get; set; }

        [JsonProperty("nome_vendedor")]
        public object NomeVendedor { get; set; }

        [JsonProperty("nf")]
        public object Nf { get; set; }

        public List<object> RetiradasAutorizadas { get; set; }

        [JsonProperty("cod_filial")]
        public string CodFilial { get; set; }

        [JsonProperty("desc_tipo_pedido")]
        public string DescTipoPedido { get; set; }

        [JsonProperty("apolice")]
        public object Apolice { get; set; }

        [JsonProperty("cod_pedidov_pai")]
        public object CodPedidovPai { get; set; }

        [JsonProperty("givex")]
        public List<object> Givex { get; set; }

        [JsonProperty("dct_vitrine_pedido")]
        public List<object> DctVitrinePedido { get; set; }

        [JsonProperty("pedido_original")]
        public object PedidoOriginal { get; set; }

        [JsonProperty("cod_embarque")]
        public string CodEmbarque { get; set; }

        [JsonProperty("data_embarque")]
        public object DataEmbarque { get; set; }

        [JsonProperty("data_entrega_cliente")]
        public object DataEntregaCliente { get; set; }

        [JsonProperty("data_conclusao_embarque")]
        public DateTime DataConclusaoEmbarque { get; set; }

        [JsonProperty("liberado")]
        public string Liberado { get; set; }

        [JsonProperty("pedido_origem")]
        public object PedidoOrigem { get; set; }

        [JsonProperty("ddd")]
        public string Ddd { get; set; }

        [JsonProperty("transportadora_original_rnl")]
        public object TransportadoraOriginalRnl { get; set; }

        [JsonProperty("transportadora_rnl_id")]
        public object TransportadoraRnlId { get; set; }

        [JsonProperty("transportadora_original_rnl_id")]
        public string TransportadoraOriginalRnlId { get; set; }

        [JsonProperty("numero_objeto")]
        public string NumeroObjeto { get; set; }
    }

    public partial class Cliente
    {
        [JsonProperty("cod_cliente")]
        public string CodCliente { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("ddd_cel")]
        public string DddCel { get; set; }

        [JsonProperty("cel")]
        public string Cel { get; set; }

        [JsonProperty("rg")]
        public object Rg { get; set; }

        [JsonProperty("data_aniversario")]
        public object DataAniversario { get; set; }

        [JsonProperty("obs")]
        public string Obs { get; set; }

        [JsonProperty("e_mail")]
        public string EMail { get; set; }

        [JsonProperty("tipo_sexo")]
        public string TipoSexo { get; set; }

        [JsonProperty("tratamento")]
        public object Tratamento { get; set; }

        [JsonProperty("maladireta")]
        public bool? Maladireta { get; set; }

        [JsonProperty("categorias")]
        public List<object> Categorias { get; set; }

        [JsonProperty("cnpj")]
        public object Cnpj { get; set; }

        [JsonProperty("pf_pj")]
        public string PfPj { get; set; }

        [JsonProperty("ie")]
        public object Ie { get; set; }

        [JsonProperty("fantasia")]
        public object Fantasia { get; set; }

        [JsonProperty("grupo_loja")]
        public object GrupoLoja { get; set; }

        [JsonProperty("parentesco")]
        public object Parentesco { get; set; }

        [JsonProperty("endereco_entrega")]
        public List<Endereco> EnderecoEntrega { get; set; }

        [JsonProperty("endereco")]
        public List<Endereco> Endereco { get; set; }

        [JsonProperty("cpf")]
        public string Cpf { get; set; }

        [JsonProperty("vitrine")]
        public object Vitrine { get; set; }

        [JsonProperty("dados_adicionais")]
        public object DadosAdicionais { get; set; }

        [JsonProperty("endereco_cobranca")]
        public List<object> EnderecoCobranca { get; set; }

        [JsonProperty("e_mail_nfe")]
        public object EMailNfe { get; set; }

        [JsonProperty("suframa")]
        public object Suframa { get; set; }

        [JsonProperty("cargo")]
        public object Cargo { get; set; }

        [JsonProperty("ufie")]
        public object Ufie { get; set; }

        [JsonProperty("desc_grupo_loja")]
        public object DescGrupoLoja { get; set; }

        [JsonProperty("desc_categorias")]
        public List<object> DescCategorias { get; set; }

        [JsonProperty("desc_regiao")]
        public object DescRegiao { get; set; }

        [JsonProperty("origem_cliente")]
        public object OrigemCliente { get; set; }

        [JsonProperty("aplicar_icms_st")]
        public object AplicarIcmsSt { get; set; }

        [JsonProperty("permite_enviar_sms")]
        public object PermiteEnviarSms { get; set; }

        [JsonProperty("permite_enviar_newsletter")]
        public object PermiteEnviarNewsletter { get; set; }

        [JsonProperty("cgc")]
        public object Cgc { get; set; }

        [JsonProperty("cod_clscliente")]
        public object CodClscliente { get; set; }

        [JsonProperty("cod_filial")]
        public object CodFilial { get; set; }

        [JsonProperty("malaeletronica")]
        public object Malaeletronica { get; set; }

        [JsonProperty("contatos")]
        public List<object> Contatos { get; set; }
    }

    public partial class Endereco
    {
        [JsonProperty("logradouro")]
        public string Logradouro { get; set; }

        [JsonProperty("bairro")]
        public string Bairro { get; set; }

        [JsonProperty("cidade")]
        public string Cidade { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }

        [JsonProperty("cep")]
        public string Cep { get; set; }

        [JsonProperty("ddd")]
        public string Ddd { get; set; }

        [JsonProperty("fone")]
        public string Fone { get; set; }

        [JsonProperty("ramal")]
        public object Ramal { get; set; }

        [JsonProperty("fax")]
        public object Fax { get; set; }

        [JsonProperty("contato")]
        public string Contato { get; set; }

        [JsonProperty("dicas_endereco")]
        public string DicasEndereco { get; set; }

        [JsonProperty("complemento")]
        public object Complemento { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }

        [JsonProperty("ddd_cel")]
        public object DddCel { get; set; }

        [JsonProperty("nome_pais")]
        public object NomePais { get; set; }

        [JsonProperty("desc_tipo_endereco")]
        public object DescTipoEndereco { get; set; }

        [JsonProperty("tipo_sexo")]
        public object TipoSexo { get; set; }

        [JsonProperty("grau_relacionamento")]
        public object GrauRelacionamento { get; set; }

        [JsonProperty("cod_filial_retira")]
        public object CodFilialRetira { get; set; }

        [JsonProperty("cnpj_filial_retira")]
        public object CnpjFilialRetira { get; set; }

        [JsonProperty("cod_endereco")]
        public object CodEndereco { get; set; }
    }

    public partial class Lancamento
    {

        [JsonProperty("valor_inicial")]
        public double ValorInicial { get; set; }

        [JsonProperty("tipo_pgto")]
        public object TipoPgto { get; set; }

        [JsonProperty("conta")]
        public object Conta { get; set; }

        [JsonProperty("documento")]
        public string Documento { get; set; }

        [JsonProperty("banco")]
        public object Banco { get; set; }

        [JsonProperty("agencia")]
        public object Agencia { get; set; }

        [JsonProperty("c_c")]
        public object CC { get; set; }

        [JsonProperty("nsu")]
        public object Nsu { get; set; }

        [JsonProperty("autorizacao")]
        public object Autorizacao { get; set; }

        [JsonProperty("rede")]
        public object Rede { get; set; }

        [JsonProperty("numparc")]
        public long Numparc { get; set; }

        [JsonProperty("numcontrato")]
        public object Numcontrato { get; set; }

        [JsonProperty("bandeira")]
        public object Bandeira { get; set; }

        [JsonProperty("operadora")]
        public object Operadora { get; set; }

        [JsonProperty("numero_cartao")]
        public object NumeroCartao { get; set; }

        [JsonProperty("mes_validade_cartao")]
        public object MesValidadeCartao { get; set; }

        [JsonProperty("ano_validade_cartao")]
        public object AnoValidadeCartao { get; set; }

        [JsonProperty("cod_seguranca_cartao")]
        public object CodSegurancaCartao { get; set; }

        [JsonProperty("nome_portador_cartao")]
        public object NomePortadorCartao { get; set; }

        [JsonProperty("cpf_portador_cartao")]
        public object CpfPortadorCartao { get; set; }

        [JsonProperty("transacao_aprovada")]
        public object TransacaoAprovada { get; set; }

        [JsonProperty("mensagem_aprova_facil")]
        public object MensagemAprovaFacil { get; set; }

        [JsonProperty("transacao")]
        public object Transacao { get; set; }

        [JsonProperty("cod_autorizacao_cartao")]
        public object CodAutorizacaoCartao { get; set; }

        [JsonProperty("comprovante_administradora")]
        public object ComprovanteAdministradora { get; set; }

        [JsonProperty("data_aprova_facil")]
        public object DataAprovaFacil { get; set; }

        [JsonProperty("adiantamento")]
        public object Adiantamento { get; set; }

        [JsonProperty("previsao")]
        public object Previsao { get; set; }

        [JsonProperty("duplicata")]
        public object Duplicata { get; set; }

        [JsonProperty("desc_tipo")]
        public string DescTipo { get; set; }

        [JsonProperty("tef")]
        public object Tef { get; set; }

        [JsonProperty("numero_credito")]
        public string NumeroCredito { get; set; }

        [JsonProperty("cod_tipo_pgto")]
        public object CodTipoPgto { get; set; }

        [JsonProperty("item")]
        public object Item { get; set; }

        [JsonProperty("pedido")]
        public object Pedido { get; set; }

        [JsonProperty("cod_condicao_pagto")]
        public string CodCondicaoPagto { get; set; }

        [JsonProperty("desc_cond_pagto")]
        public string DescCondPagto { get; set; }

        [JsonProperty("desconto")]
        public object Desconto { get; set; }

        [JsonProperty("perc_desconto")]
        public object PercDesconto { get; set; }

        [JsonProperty("valor_liquido")]
        public object ValorLiquido { get; set; }

        [JsonProperty("valor_inicial_porpor")]
        public object ValorInicialPorpor { get; set; }

        [JsonProperty("resumo_venda")]
        public object ResumoVenda { get; set; }

        [JsonProperty("parcela")]
        public object Parcela { get; set; }

        [JsonProperty("gateway_pagamento")]
        public object GatewayPagamento { get; set; }

        [JsonProperty("url_boleto")]
        public object UrlBoleto { get; set; }

        [JsonProperty("token_cartao_gp")]
        public object TokenCartaoGp { get; set; }

        [JsonProperty("transacao_gp")]
        public object TransacaoGp { get; set; }

        [JsonProperty("tabela_financiamento")]
        public object TabelaFinanciamento { get; set; }

        [JsonProperty("plano_financiamento")]
        public object PlanoFinanciamento { get; set; }

        [JsonProperty("barra_leitora")]
        public object BarraLeitora { get; set; }

        [JsonProperty("barra_digitada")]
        public object BarraDigitada { get; set; }

        [JsonProperty("juros_financiamento")]
        public object JurosFinanciamento { get; set; }

        [JsonProperty("desc_bandeira")]
        public string DescBandeira { get; set; }

        [JsonProperty("desc_operadora")]
        public object DescOperadora { get; set; }

        [JsonProperty("cod_bandeira")]
        public string CodBandeira { get; set; }

        [JsonProperty("cod_operadora")]
        public object CodOperadora { get; set; }

        [JsonProperty("lancamento_filial")]
        public object LancamentoFilial { get; set; }

        [JsonProperty("nao_validar_valor_titulo")]
        public object NaoValidarValorTitulo { get; set; }

        [JsonProperty("status_aprovacao")]
        public object StatusAprovacao { get; set; }

        [JsonProperty("efetuado")]
        public object Efetuado { get; set; }

        [JsonProperty("n_cheque")]
        public object NCheque { get; set; }

        [JsonProperty("nosso_numero")]
        public string NossoNumero { get; set; }

        [JsonProperty("numero_cartao_ok")]
        public string NumeroCartaoOk { get; set; }
    }

    public partial class Produto
    {
        [JsonProperty("produto")]
        public long ProdutoProduto { get; set; }

        [JsonProperty("cor")]
        public long Cor { get; set; }

        [JsonProperty("estampa")]
        public long Estampa { get; set; }

        [JsonProperty("tamanho")]
        public string Tamanho { get; set; }

        [JsonProperty("quantidade")]
        public long Quantidade { get; set; }

        [JsonProperty("preco")]
        public double Preco { get; set; }

        [JsonProperty("item")]
        public string Item { get; set; }

        [JsonProperty("obs_item")]
        public object ObsItem { get; set; }

        [JsonProperty("altura")]
        public object Altura { get; set; }

        [JsonProperty("comprimento")]
        public object Comprimento { get; set; }

        [JsonProperty("largura")]
        public object Largura { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("barra")]
        public string Barra { get; set; }

        [JsonProperty("cod_pedidov")]
        public string CodPedidov { get; set; }

        [JsonProperty("cfop")]
        public object Cfop { get; set; }

        [JsonProperty("data_entrega")]
        public object DataEntrega { get; set; }

        [JsonProperty("tipo_icms")]
        public object TipoIcms { get; set; }

        [JsonProperty("b_icms")]
        public object BIcms { get; set; }

        [JsonProperty("al_icms")]
        public object AlIcms { get; set; }

        [JsonProperty("v_icms")]
        public object VIcms { get; set; }

        [JsonProperty("al_pis")]
        public object AlPis { get; set; }

        [JsonProperty("v_pis")]
        public object VPis { get; set; }

        [JsonProperty("al_cofins")]
        public object AlCofins { get; set; }

        [JsonProperty("v_cofins")]
        public object VCofins { get; set; }

        [JsonProperty("v_icms_uf_dest")]
        public object VIcmsUfDest { get; set; }

        [JsonProperty("v_icms_uf_remet")]
        public object VIcmsUfRemet { get; set; }

        [JsonProperty("it_valor_frete")]
        public object ItValorFrete { get; set; }

        [JsonProperty("it_valor_desconto")]
        public object ItValorDesconto { get; set; }

        [JsonProperty("it_valor_cortesia")]
        public object ItValorCortesia { get; set; }

        [JsonProperty("desconto")]
        public object Desconto { get; set; }

        [JsonProperty("endereco")]
        public object Endereco { get; set; }

        [JsonProperty("encomenda")]
        public object Encomenda { get; set; }

        [JsonProperty("presente")]
        public object Presente { get; set; }

        [JsonProperty("componente_kit")]
        public object ComponenteKit { get; set; }

        [JsonProperty("cod_cor")]
        public string CodCor { get; set; }

        [JsonProperty("desc_cor")]
        public string DescCor { get; set; }

        [JsonProperty("cod_estampa")]
        public string CodEstampa { get; set; }

        [JsonProperty("desc_estampa")]
        public string DescEstampa { get; set; }

        [JsonProperty("desc_produto")]
        public string DescProduto { get; set; }

        [JsonProperty("url_imagem")]
        public object UrlImagem { get; set; }

        [JsonProperty("brindesite")]
        public object Brindesite { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("desc_status")]
        public string DescStatus { get; set; }

        [JsonProperty("url_tracking_pedido")]
        public object UrlTrackingPedido { get; set; }

        [JsonProperty("nota_ref")]
        public object NotaRef { get; set; }

        [JsonProperty("b_pis")]
        public object BPis { get; set; }

        [JsonProperty("b_cofins")]
        public object BCofins { get; set; }

        [JsonProperty("cod_produto")]
        public string CodProduto { get; set; }

        [JsonProperty("ncm")]
        public object Ncm { get; set; }

        [JsonProperty("vitrine_produto_sku")]
        public string VitrineProdutoSku { get; set; }

        [JsonProperty("cod_prefaturamento")]
        public string CodPrefaturamento { get; set; }

        [JsonProperty("cst")]
        public object Cst { get; set; }

        [JsonProperty("icms")]
        public object Icms { get; set; }

        [JsonProperty("ipi")]
        public object Ipi { get; set; }

        [JsonProperty("v_ipi")]
        public object VIpi { get; set; }

        [JsonProperty("icmss")]
        public object Icmss { get; set; }

        [JsonProperty("v_icmss")]
        public object VIcmss { get; set; }

        [JsonProperty("unidade")]
        public object Unidade { get; set; }

        [JsonProperty("valor_unitario")]
        public object ValorUnitario { get; set; }

        [JsonProperty("valor_total_imposto")]
        public object ValorTotalImposto { get; set; }

        [JsonProperty("icms_uf_dest")]
        public object IcmsUfDest { get; set; }

        [JsonProperty("id_externo")]
        public string IdExterno { get; set; }

        [JsonProperty("descricao_cfop")]
        public object DescricaoCfop { get; set; }

        [JsonProperty("cod_trba_iss")]
        public object CodTrbaIss { get; set; }

        [JsonProperty("b_iss")]
        public object BIss { get; set; }

        [JsonProperty("preco_icms_iss")]
        public object PrecoIcmsIss { get; set; }

        [JsonProperty("al_iss")]
        public object AlIss { get; set; }

        [JsonProperty("lote")]
        public object Lote { get; set; }

        [JsonProperty("valor_total_bruto")]
        public object ValorTotalBruto { get; set; }

        [JsonProperty("valor_total_liquido")]
        public object ValorTotalLiquido { get; set; }

        [JsonProperty("bcuf_dest")]
        public object BcufDest { get; set; }

        [JsonProperty("pfcp_uf_dest")]
        public object PfcpUfDest { get; set; }

        [JsonProperty("vfcpufdest")]
        public object Vfcpufdest { get; set; }

        [JsonProperty("fcp_uf_dest")]
        public object FcpUfDest { get; set; }

        [JsonProperty("v_fcp_uf_dest")]
        public object VFcpUfDest { get; set; }
    }
}

