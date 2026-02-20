using CsvHelper.Configuration.Attributes;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class PlanilhaPreviaCusto
    {
        [Name("Cód. Integracao Filial Pedido")]
        public virtual string CodigoIntegracaoFilialPedido { get; set; }

        [Name("CNPJ Filial Pedido")]
        public virtual string CNPJFilialPedido { get; set; }

        [Name("Razão Social Filial Pedido")]
        public virtual string DescricaoFilialPedido { get; set; }

        [Name("Cód. Integracao Transportador Vinculado")]
        public virtual string CodigoIntegraoTransportador { get; set; }

        [Name("Razão Social Transportador Vinculado")]
        public virtual string RazaoSocialTransportadorVinculado { get; set; }

        [Name("Número da carga")]
        public virtual string NumeroCarga { get; set; }

        [Name("Número da etapa")]
        public virtual string NumeroEtapa { get; set; }

        [Name("Data de carregamento da carga")]
        public virtual string DataCarregamento { get; set; }

        [Name("Placa da tração")]
        public virtual string PlacaTracao { get; set; }
        
        [Name("Placa dos reboques")]
        public virtual string PlacasReboques { get; set; }

        [Name("Sistema De entrega")]
        public virtual string CanalVenda { get; set; }

        [Name("Modal de transporte")]
        public virtual string ModalTransporte { get; set; }

        [Name("Tipo de Carga")]
        public virtual string CodigoIntegracaoTipoCarga { get; set; }

        [Name("Modelo veicular")]
        public virtual string CodigoIntegracaoModeloVeicular { get; set; }

        [Name("Número da NF-e")]
        public virtual string NumeroNfe { get; set; }

        [Name("Data de Emissão da NF-e")]
        public virtual string DataEmisaoNFe { get; set; }

        [Name("Peso Bruto")]
        public virtual decimal PesoBruto { get; set; }

        [Name("Número de Volumes")]
        public virtual string NumeroVolumen { get; set; }

        [Name("Valor da Mercadoria")]
        public virtual decimal ValorMercaduria { get; set; }

        //Frete
        [Name("Frete")]
        public virtual decimal Frete { get; set; }

        [Name("AdValorem")]
        public virtual decimal AdValorem { get; set; }

        [Name("Taxa de descarga")]
        public virtual decimal TaxaDescarga { get; set; }

        [Name("GRIS")]
        public virtual string GRIS { get; set; }
        //ComponeteFreteadicionar

        //Impostos

        [Name("Base de cálculo ISS")]
        public virtual decimal BaseCalculoISS { get; set; }

        [Name("Alíquota ISS")]
        public virtual decimal AliquotaISS { get; set; }

        [Name("Valor ISS")]
        public virtual decimal ValorISS { get; set; }

        [Name("Base de cálculo ICMS")]
        public virtual decimal BaseCalculoICMS { get; set; }

        [Name("Alíquota ICMS")]
        public virtual decimal AliquotaICMS { get; set; }

        [Name("Valor ICMS")]
        public virtual decimal ValorICMS { get; set; }

        [Name("Valor total do frete")]
        public virtual decimal ValorTotalFrete { get; set; }

        [Name("Valores OutrosComponentes")]
        public virtual decimal ValorOutrosCompoentes { get; set; }

        [Name("Chave de acesso das notas fiscais")]
        public virtual string ChavesNotas { get; set; }

        //Destinatário
        [Name("Destinatário CPNJ/CPF")]
        public virtual string CNP_CPF { get; set; }

        [Name("Destinatário IE")]
        public virtual string IE { get; set; }

        [Name("Destinatário Razão Social")]
        public virtual string RazaoSocial { get; set; }

        [Name("Destinatário Telefone")]
        public virtual string Telefone { get; set; }

        [Name("Destinatário Endereço")]
        public virtual string Endereco { get; set; }

        [Name("Destinatário Endereço")]
        public virtual string Numero { get; set; }

        [Name("Destinatário Complemento")]
        public virtual string Complemento { get; set; }

        [Name("Destinatário Bairro")]
        public virtual string Bairro { get; set; }

        [Name("Destinatário Código IBGE Cidade")]
        public virtual string CodigoIBGECidade { get; set; }

        [Name("Destinatário Cidade")]
        public virtual string DescricaoCidade { get; set; }

        [Name("Destinatário CEP")]
        public virtual string CEP { get; set; }

        [Name("Destinatário UF")]
        public virtual string UF { get; set; }

        [Name("Destinatário Código país")]
        public virtual string CodigoPais { get; set; }

        [Name("Destinatário Pais")]
        public virtual string DescricaoPais { get; set; }

        //Dados emissor da nota fiscal
        [Name("Emissor Nota CNPJ/CPF")]
        public virtual string CNPJ_CPFEmissoNota { get; set; }

        [Name("Emissor IE ")]
        public virtual string IEEmissoNota { get; set; }

        [Name("Emissor Nota Razão Social ")]
        public virtual string RazaoSocialEmissoNota { get; set; }

        [Name("Emissor Nota Telefone")]
        public virtual string TelefoneEmissoNota { get; set; }

        [Name("Emissor Nota Endereço")]
        public virtual string EnderecoEmissoNota { get; set; }

        [Name("Emissor Nota Número")]
        public virtual string NumeroEmissoNota { get; set; }

        [Name("Emissor Nota Complemento")]
        public virtual string ComplementoEmissoNota { get; set; }

        [Name("Emissor Nota Bairro")]
        public virtual string BairroEmissoNota { get; set; }

        [Name("Emissor Nota Código IBGE Cidade")]
        public virtual string CodigoIBGECidadeEmissoNota { get; set; }

        [Name("Emissor Nota Cidade")]
        public virtual string DescricaoCidadeEmissoNota { get; set; }

        [Name("Emissor Nota CEP")]
        public virtual string CEPEmissoNota { get; set; }

        [Name("Emissor Nota UF")]
        public virtual string UFEmissoNota { get; set; }

        [Name("Emissor Nota Código Pais")]
        public virtual string CodigoPaisEmissoNota { get; set; }

        [Name("Emissor Nota Pais")]
        public virtual string DescricaoPaisEmissoNota { get; set; }

        [Name("Tomador do Frete CNPJ/CPF")]
        public virtual string CNPJ_CPFTomadorFrete { get; set; }

        [Name("Tomador do Frete IE")]
        public virtual string IETomadorFrete { get; set; }

        [Name("Tomador do Razão Social")]
        public virtual string RazaoSocialTomadorFrete { get; set; }

        [Name("Tomador do Frete Telefone")]
        public virtual string TelefoneTomadorFrete { get; set; }

        [Name("Tomador do Frete Endereço")]
        public virtual string EnderecoTomadorFrete { get; set; }

        [Name("Tomador do Frete Número")]
        public virtual string NumeroTomadorFrete { get; set; }

        [Name("Tomador do Frete Complemento")]
        public virtual string ComplementoTomadorFrete { get; set; }

        [Name("Tomador do Frete Bairro")]
        public virtual string BairroTomadorFrete { get; set; }

        [Name("Tomador do Frete Código IBGE Cidade")]
        public virtual string CodigoIBGECidadeTomadorFrete { get; set; }

        [Name("Tomador do Frete Cidade")]
        public virtual string DescricaoCidadeTomadorFrete { get; set; }

        [Name("Tomador do Frete CEP")]
        public virtual string CEPTomadorFrete { get; set; }

        [Name("Tomador do Frete UF")]
        public virtual string UFTomadorFrete { get; set; }

        [Name("Tomador do Frete Código País")]
        public virtual string CodigoPaisTomadorFrete { get; set; }

        [Name("Tomador do Frete Pais")]
        public virtual string DescricaoPaisTomadorFrete { get; set; }

        [Name("Recebedor CNPJ/CPF")]
        public virtual string CNPJ_CPFRecebedor { get; set; }

        [Name("Recebedor IE")]
        public virtual string IERecebedor { get; set; }

        [Name("Recebedor Razão Social")]
        public virtual string RazaoSocialRecebedor { get; set; }

        [Name("Recebedor Telefone")]
        public virtual string TelefoneRecebedor { get; set; }

        [Name("Recebedor Endereço")]
        public virtual string EnderecoRecebedor { get; set; }

        [Name("Recebedor Número")]
        public virtual string NumeroRecebedor { get; set; }

        [Name("Recebedor Complemento")]
        public virtual string ComplementoRecebedor { get; set; }

        [Name("Recebedor Bairro")]
        public virtual string BairroRecebedor { get; set; }

        [Name("Recebedor Código IBGE Cidade")]
        public virtual string CodigoIBGECidadeRecebedor { get; set; }

        [Name("Recebedor Cidade")]
        public virtual string DescricaoCidadeRecebedor { get; set; }

        [Name("Recebedor CEP")]
        public virtual string CEPRecebedor { get; set; }

        [Name("Recebedor UF")]
        public virtual string UFRecebedor { get; set; }

        [Name("Recebedor Código país")]
        public virtual string CodigoPaisRecebedor { get; set; }

        [Name("Recebedor Pais")]
        public virtual string DescricaoPaisRecebedor { get; set; }

        [Name("Transportador CNPJ/CPF")]
        public virtual string CNPJ_CPFTransportador { get; set; }

        [Name("Transportador IE")]
        public virtual string IETransportador { get; set; }

        [Name("Transportador Razão Social")]
        public virtual string RazaoSocialTransportador { get; set; }

        [Name("Transportador Telefone")]
        public virtual string TelefoneTransportador { get; set; }

        [Name("Transportador Endereço")]
        public virtual string EnderecoTransportador { get; set; }

        [Name("Transportador Número")]
        public virtual string NumeroTransportador { get; set; }

        [Name("Transportador Complemento")]
        public virtual string ComplementoTransportador { get; set; }

        [Name("Transportador Bairro")]
        public virtual string BairroTransportador { get; set; }

        [Name("Transportador Código IBGE Cidade")]
        public virtual string CodigoIBGECidadeTransportador { get; set; }

        [Name("Transportador Cidade")]
        public virtual string DescricaoCidadeTransportador { get; set; }

        [Name("Transportador CEP")]
        public virtual string CEPTransportador { get; set; }

        [Name("Transportador UF")]
        public virtual string UFTransportador { get; set; }

        [Name("Transportador Código país")]
        public virtual string CodigoPaisTransportador { get; set; }

        [Name("Transportador Pais")]
        public virtual string DescricaoPaisTransportador { get; set; }

        [Name("Expedidor CNPJ/CPF")]
        public virtual string CNPJ_CPFExpedidor { get; set; }

        [Name("Expedidor IE")]
        public virtual string IEExpedidor { get; set; }

        [Name("Expedidor Razão Social")]
        public virtual string RazaoSocialExpedidor { get; set; }

        [Name("Expedidor Telefone")]
        public virtual string TelefoneExpedidor { get; set; }

        [Name("Expedidor Endereço")]
        public virtual string EnderecoExpedidor { get; set; }

        [Name("Expedidor Número")]
        public virtual string NumeroExpedidor { get; set; }

        [Name("Expedidor Complemento")]
        public virtual string ComplementoExpedidor { get; set; }

        [Name("Expedidor Bairro")]
        public virtual string BairroExpedidor { get; set; }

        [Name("Expedidor Código IBGE Cidade")]
        public virtual string CodigoIBGECidadeExpedidor { get; set; }

        [Name("Expedidor Cidade")]
        public virtual string DescricaoCidadeExpedidor { get; set; }

        [Name("Expedidor CEP")]
        public virtual string CEPExpedidor { get; set; }

        [Name("Expedidor UF")]
        public virtual string UFExpedidor { get; set; }

        [Name("Expedidor Código País")]
        public virtual string CodigoPaisExpedidor { get; set; }

        [Name("Expedidor País")]
        public virtual string DescricaoPaisExpedidor { get; set; }

        [Name("Numeros Notas Pre-Cte")]
        public virtual string NumerosNotasPreCte { get; set; }

        [Name("Tipo de Tomador")]
        public virtual string TipoTomador { get; set; }

        [Name("Retenção ISS")]
        public virtual string ISSRetido { get; set; }
    }
}