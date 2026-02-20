using Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate
{
    [System.Xml.Serialization.XmlRootAttribute("Envio", IsNullable = false)]
    public class envioNFSe
    {
        public string ModeloDocumento { get; set; } = "NFSE";

        public string Versao { get; set; } = "1.00";

        [System.Xml.Serialization.XmlElementAttribute("RPS")]
        public RPS RPS { get; set; }
    }

    public class RPS
    {
		public int RPSNumero { get; set; }

		public string RPSSerie { get; set; }

        /// <summary>
        /// Informar 1 – Recibo Provisório de Serviços
        /// </summary>
        public int RPSTipo { get; set; } = 1;

        /// <summary>
        /// Formato: AAAA-MM-DDTHH:MM:SS
        /// </summary>
		public string dEmis { get; set; }

        /// <summary>
        /// Formato: AAAA-MM-DDTHH:MM:SS
        /// </summary>
        public string dCompetencia { get; set; }

        /// <summary>
        /// <para>
        /// 1-No Município Sem Retenção</para><para>
        /// 2-No Município Com Retenção</para><para> 
        /// 3-Fora do Município Sem Retenção</para><para>
        /// 4-Fora do Município Com Retenção</para><para>
        /// 5-Fora do Município com pagamento no local</para><para>
        /// 6-Outro Município(Exterior)</para><para>
        /// 7-No Municipio sem retenção Simples Nacional</para><para>
        /// 8-No Municipio Tributacao Fixa Anual</para><para>
        /// 9-No Municipio sem recolhimento</para><para>
        /// *Obs: Tópicos 7, 8 e 9 exclusivos padrão Governa</para><para>
        /// </para>
        /// </summary>
        public int? LocalPrestServ { get; set; }

        /// <summary>
        /// <para>
        /// Utilizar valores da tabela unificada de naturezas de operação que consta na aba Natureza de Operação.</para><para>
        /// Também é possível visualizar quais naturezas são aceitas no município da empresa, bem como para quais </para><para>
        /// valores será convertido através do painel de controle de NFS-e no InvoiCy.</para>
        /// </summary>
        public int natOp { get; set; }

        /// <summary>
        /// <para>
        /// 1 – Microempresa municipal</para><para>
        /// 2 – Estimativa</para><para>
        /// 3 – Sociedade de profissionais</para><para>
        /// 4 – Cooperativa</para><para>
        /// 5 – Microempresário Individual(MEI)</para><para>
        /// 6 – Microempresário e Empresa de Pequeno Porte(ME EPP)</para><para>
        /// 7 – Optante pelo Simples Nacional(Exclusivo Elotech e GLC Consultoria 2.0)</para><para>
        /// 8 - Tributação Normal(Exclusivo EEL)</para><para>
        /// 9 - Autônomo(Exclusivo EEL)</para><para>
        /// 10 - Variável(Exclusivo GLC Consultoria 2.0)</para><para>
        /// 11 - Lucro Real(Exclusivo Digifred)</para><para>
        /// 12 - Lucro Presumido(Exclusivo Digifred)</para><para>
        /// 13 - Sociedade de Profissionais Pessoa Jurídica(Exclusivo SEMFAZ)</para><para>
        /// 14 - Não(Exclusivo NF-Eletrônica)</para><para>
        /// 15 - Notas Totalizadoras(Exclusivo NF-Eletrônica)</para><para>
        /// 16 - Inscrito no PRODEVAL(Exclusivo NF-Eletrônica)</para>
        /// </summary>
        public int? RegEspTrib { get; set; }

        /// <summary>
        /// <para>
        /// 1-Sim</para><para>
        /// 2-Não</para><para>
        /// 3-SIMEI(Exclusivo do padrão INFISC)</para><para>
        /// 7 - Simples Federal(Alíquota 1,0%)</para><para>
        /// 8 - Simples Federal(Alíquota 0,5%)</para><para>
        /// 9- Simples Municipal NFPAULISTANA</para><para>
        /// As opções 7, 8 e 9 são exclusivos para o uso no padrão NF Paulista – município de São Paulo/SP e Padrão Nota Blu - Blumenau/SC)</para>
        /// </summary>
        public int OptSN { get; set; }

        /// <summary>
        /// <para>
        /// 1-Sim</para><para>
        /// 2-Não</para><para>
        /// 3 – Optante pelo Simples Nacional em início de atividade(primeiras três competências)</para><para>
        /// 4 – Serviço prestado no Programa Minha Casa Minha Vida(até 3 salários mínimos)</para><para>
        /// Opções 3 e 4 são exclusivas do padrão SigCorp Londrina</para>
        /// </summary>
        public int IncCult { get; set; }

        /// <summary>
        /// <para>
        /// 1-Normal</para><para>
        /// 2-Cancelada</para><para>
        /// 3-Extraviado</para><para>
        /// 4-Contingência</para><para>
        /// 5-Inutilização</para><para>
        /// A opção 3 é exclusiva do padrão NF Paulista – município de São Paulo/SP.
        /// Opções 4 e 5 são exclusivos do padrão INFISC.</para>
        /// </summary>
        public int Status { get; set; } = 1;

        /// <summary>
        /// Ambiente em que a empresa está cadastrada no InvoiCy.
        /// <para>
        /// 1-Produção; 2-Homologação.
        /// </para>
        /// </summary>
        public int tpAmb { get; set; } = 1;

        /// <summary>
        /// <para>
        /// Sempre será Impresso no Espelho do RPS do InvoiCy. Porém, só envia para as prefeituras do Betha, IPM, EEL, SimplISS, Prescon e INFISC.</para><para>
        /// Se você utilizar o espelho do RPS gerado pelo InvoiCy, sempre terá essa informação impressa!</para><para>
        /// Obs: Caso seja enviado menos de 58 caracteres, o layout Betha concatena o link do espelho da NFS-e neste campo.</para>
        /// </summary>
        public string NFSOutrasinformacoes { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("RPSSubs")]
        public RPSSubs RPSSubs { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Prestador")]
        public Prestador Prestador { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("ListaItens")]
        public ListaItens ListaItens { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Servico")]
        public Servico Servico { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Tomador")]
        public Tomador Tomador { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("IntermServico")]
        public IntermServico IntermServico { get; set; }
    }

    public class RPSSubs
    {
        public int SubsNumero { get; set; }

        public string SubsSerie { get; set; }

        public int SubsTipo { get; set; }

        public int? SubsNFSeNumero { get; set; }

        public string SubsDEmisNFSe { get; set; }
    }

    public class Prestador
    {
		public string CNPJ_prest { get; set; }

		public string xNome { get; set; }

		public string xFant { get; set; }

		public string IM { get; set; }

		public string IE { get; set; }

        /// <summary>
        /// Cadastro Mobiliário do Contribuinte
        /// </summary>
		public string CMC { get; set; }

        /// <summary>
        /// Número do Cadastro de Atividade Econômica da Pessoa Física do prestador do Serviço
        /// </summary>
        public string CAEPF { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("enderPrest")]
        public enderPrest enderPrest { get; set; }
    }

    public class enderPrest
    {
		public string TPEnd { get; set; }

        public string XLgr { get; set; }

		public string nro { get; set; }

		public string xBairro { get; set; }

		public string cMun { get; set; }

		public string UF { get; set; }

		public string CEP { get; set; }

		public string fone { get; set; }
    }

    public class ListaItens
    {
        [System.Xml.Serialization.XmlElementAttribute("Item")]
        public List<Item> Item { get; set; }
    }

    public class Item
    {
        public int ItemSeq { get; set; }

        public string ItemCod { get; set; }

        public string ItemDesc { get; set; }

        public string ItemQtde { get; set; }

        public string ItemvUnit { get; set; }

        public string ItemuMed { get; set; }

        public string ItemvlDed { get; set; }

        /// <summary>
        /// <para>
        /// Obrigatório nos padrões DSFNET e Elotech.</para><para>
        /// S – Item Tributável</para><para>
        /// N – Não tributável</para><para>
        /// Para o preenchimento do campo orienta-se a consultar o tópico DSFNET e Atividades de itens de serviços com marcação Não tributável – DSFNET</para>
        /// </summary>
        public string ItemTributavel { get; set; }

        public string ItemcCnae { get; set; }

        /// <summary>
        /// <para>
        /// Preencher conforme está no cadastro da empresa no município.</para><para>
        /// O Código de tributação Municipal está relacionado com o Item da Lista de serviços, que é a classificação federal para os serviços tributáveis. </para><para>
        /// Como um item da lista pode estar relacionado a várias regras tributárias, as prefeituras criaram o código de tributação municipal para manter</para><para>
        /// um controle granular e específico. Dessa forma, essa informação varia de acordo com a empresa e o município, e deve ser adquirida junto ao </para><para>
        /// órgão responsável no município.</para>
        /// </summary>
        public string ItemTributMunicipio { get; set; }

        public string ItemnAlvara { get; set; }

        public string ItemvIss { get; set; }

        public string ItemvDesconto { get; set; }

        public string ItemAliquota { get; set; }

        public string ItemVlrTotal { get; set; }

        /// <summary>
        /// Valor total do item subtraído das deduções (ItemVlrTotal - ItemvlDed) e em alguns casos também descontos.
        /// </summary>
        public string ItemBaseCalculo { get; set; }

        public string ItemvlrISSRetido { get; set; }

        /// <summary>
        /// 1 - Sim
        /// 2 - Não
        /// </summary>
        public int ItemIssRetido { get; set; }

        /// <summary>
        /// <para>
        /// Em padrões ABRASF, o campo só deve ser informado se ISSRetido = 1.No padrão IPM, o campo pode ser obrigatório(Valores 1 e 3).</para><para>
        /// 1 – Tomador / Não Tributar para Prestador;</para><para>
        /// 2 – Intermediário;</para><para>
        /// 3 – Prestador / Tributar para Prestador.</para>
        /// </summary>
        public int? ItemRespRetencao { get; set; }

        /// <summary>
        /// <para>
        /// O Item da Lista de Serviço é uma classificação Federal dos serviços tributáveis, regulamentado pela Lei Complementar 116/2003. </para><para>
        /// Os itens da legislação podem ser consultados aqui - http://www.planalto.gov.br/ccivil_03/leis/LCP/Lcp116.htm. </para><para>
        /// Mas vale ressaltar que cada prefeitura cadastra o item da empresa, então pode variar. </para><para>
        /// A empresa deve consultar junto ao órgão competente no município qual item utilizar.</para>
        /// </summary>
        public string ItemIteListServico { get; set; }

        /// <summary>
        /// <para>
        /// Código da Natureza de Operação / Exigibilidade de ISS do item</para><para>
        /// Mesmos valores da natureza da operação do RPS.</para>
        /// </summary>
        public string ItemExigibilidadeISS { get; set; }

        public string ItemcMunIncidencia { get; set; }

        public string ItemNumProcesso { get; set; }

        /// <summary>
        /// M - Material;
        /// S - Sub empreitada.
        /// </summary>
        public string ItemDedTipo { get; set; }

        /// <summary>
        /// Exclusivo do padrão Elotech
        /// </summary>
        public string ItemDedCPFRef { get; set; }

        /// <summary>
        /// Exclusivo do padrão Elotech
        /// </summary>
        public string ItemDedCNPJRef { get; set; }

        /// <summary>
        /// Exclusivo do padrão Elotech
        /// </summary>
        public int? ItemDedNFRef { get; set; }

        /// <summary>
        /// Exclusivo do padrão Elotech
        /// </summary>
        public string ItemDedvlTotRef { get; set; }

        public string ItemDedPer { get; set; }

        public string ItemVlrLiquido { get; set; }

        public string ItemValAliqINSS { get; set; }

        public string ItemValINSS { get; set; }

        public string ItemValAliqIR { get; set; }

        public string ItemValIR { get; set; }

        public string ItemValAliqCOFINS { get; set; }

        public string ItemValCOFINS { get; set; }

        public string ItemValAliqCSLL { get; set; }

        public string ItemValCSLL { get; set; }

        public string ItemValAliqPIS { get; set; }

        public string ItemValPIS { get; set; }

        public string ItemRedBC { get; set; }

        public string ItemRedBCRetido { get; set; }

        public string ItemBCRetido { get; set; }

        /// <summary>
        /// Informar preferencialmente no formato decimal. Ex: 0,05 para 5%.
        /// </summary>
        public string ItemValAliqISSRetido { get; set; }

        /// <summary>
        /// "BR" para Brasil e "EX" para exterior.Lista de siglas https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2
        /// </summary>
        public string ItemPaisImpDevido { get; set; }

        public string ItemJustDed { get; set; }

        public string ItemvOutrasRetencoes { get; set; }

        public string ItemDescIncondicionado { get; set; }

        public string ItemDescCondicionado { get; set; }

        public string ItemTotalAproxTribServ { get; set; }
    }

    public class Servico
    {
        [System.Xml.Serialization.XmlElementAttribute("Valores")]
        public Valores Valores { get; set; }

		public string IteListServico { get; set; }

		public string Cnae { get; set; }

        /// <summary>
        /// <para></para>
        /// Padrões Betha e Awatar:</para><para>
        /// 1 - À vista</para><para>
        /// 2 - Na apresentação</para><para>
        /// 3 - À prazo</para><para>
        /// 4 - Cartão débito</para><para>
        /// 5 - Cartão crédito</para><para>
        /// GLC Consulturia 2.0:</para><para>
        /// texto livre até 40 caracteres.</para>
        /// </summary>
        public string fPagamento { get; set; }

		public string TributMunicipio { get; set; }

		public string Discriminacao { get; set; }

        /// <summary>
        /// Código de identificação do município de prestação do serviço. Consulte nosso artigo para mais informações.
        /// </summary>
		public string cMun { get; set; }

        /// <summary>
        /// Código do município onde é a incidência do imposto. Consulte nosso artigo para mais informações.
        /// </summary>
		public string cMunIncidencia { get; set; }
    }

    public class Valores
    {
		public string ValServicos { get; set; }

        /// <summary>
        /// Valor percentual padrão para dedução/redução do valor do serviço - Uso específico para NFS-e Modelo Nacional
        /// </summary>
        public string ValPercDeducoes { get; set; }

        public string ValDeducoes { get; set; }

        public string ValPIS { get; set; }

        public string ValBCPIS { get; set; }

        public string ValCOFINS { get; set; }

        public string ValBCCOFINS { get; set; }

        public string ValINSS { get; set; }

        public string ValBCINSS { get; set; }

        public string ValIR { get; set; }

        public string ValBCIRRF { get; set; }

        public string ValCSLL { get; set; }

        public string ValBCCSLL { get; set; }

        /// <summary>
        /// <para>
        /// 1- Tomador, ou não tributar para o prestador.</para><para>
        /// 2 - Intermediário.</para><para>
        /// 3 - Prestador.</para>
        /// </summary>
        public int? RespRetencao { get; set; }

        /// <summary>
        /// <para>
        /// Obrigatório nos padrões DSFNET e Elotech.</para><para>
        /// S – Item Tributável</para><para>
        /// N – Não tributável</para><para>
        /// Para o preenchimento do campo orienta-se a consultar o tópico DSFNET e Atividades de itens de serviços com marcação Não tributável – DSFNET</para>
        /// </summary>
        public string Tributavel { get; set; }

        public string ValISS { get; set; }

        /// <summary>
        /// <para>
        /// 1 - Sim;</para><para>
        /// 2 - Não;</para><para>
        /// 3 - Substituição Tributária - exclusivo para o padrão Betha 1.0</para>
        /// </summary>
        public int ISSRetido { get; set; }

        public string ValISSRetido { get; set; }

        /// <summary>
        /// Valor do Serviço - descontos
        /// </summary>
        public string ValTotal { get; set; }

        /// <summary>
        /// Valor Total Recebido em R$ (inclusive valores repassados a terceiros). Exclusivo NF Paulistana
        /// </summary>
        public string ValTotalRecebido { get; set; }

        /// <summary>
        /// (Valor dos serviços - Valor das deduções - descontos incondicionais)
        /// </summary>
        public string ValBaseCalculo { get; set; }

        /// <summary>
        /// Valor das outras retenções
        /// </summary>
        public string ValOutrasRetencoes { get; set; }

        public string ValAliqISS { get; set; }

        public string ValAliqPIS { get; set; }

        /// <summary>
        /// <para>
        /// 1 - Sim;</para><para>
        /// 2 - Não;</para><para>
        /// Exclusivo padrão Tributos Web - Elotech 2.03</para>
        /// </summary>
        public int? PISRetido { get; set; }
                
        public string ValAliqCOFINS { get; set; }

        /// <summary>
        /// <para>
        /// 1 - Sim;</para><para>
        /// 2 - Não;</para><para>
        /// Exclusivo padrão Tributos Web - Elotech 2.03</para>
        /// </summary>
        public int? COFINSRetido { get; set; }
                
        public string ValAliqIR { get; set; }

        /// <summary>
        /// <para>
        /// 1 - Sim;</para><para>
        /// 2 - Não;</para><para>
        /// Exclusivo padrão Tributos Web - Elotech 2.03</para>
        /// </summary>
        public int? IRRetido { get; set; }

        public string ValAliqCSLL { get; set; }

        /// <summary>
        /// <para>
        /// 1 - Sim;</para><para>
        /// 2 - Não;</para><para>
        /// Exclusivo padrão Tributos Web - Elotech 2.03</para>
        /// </summary>
        public int? CSLLRetido { get; set; }

        public string ValAliqINSS { get; set; }

        /// <summary>
        /// <para>
        /// 1 - Sim;</para><para>
        /// 2 - Não;</para><para>
        /// Exclusivo padrão Tributos Web - Elotech 2.03</para>
        /// </summary>
        public int? INSSRetido { get; set; }
                
        public string ValBCOutrasRetencoes { get; set; }
                
        public string ValAliqOutrasRetencoes { get; set; }
                
        public string ValAliqTotTributos { get; set; }

        /// <summary>
        /// <para>
        /// O valor informado neste campo NORMALMENTE compreende o seguinte cálculo:</para><para>
        /// Valor dos Serviços - Valor de PIS - Valor de COFINS - Valor de INSS - Valor de IR - Valor de CSLL - Outras Retenções - Valor IISS Retido - Desconto Incondicionado - Desconto Condicionado</para>
        /// </summary>
        public string ValLiquido { get; set; }

        public string ValDescIncond { get; set; }

        public string ValDescCond { get; set; }
    }

    public class Tomador
    {
		public string TomaCNPJ { get; set; }

        public string TomaCPF { get; set; }

        public string TomaRazaoSocial { get; set; }

        /// <summary>
        /// Tipo do logradouro do tomador (Rua, Avenida, Travessa, etc.)
        /// </summary>
        public string TomatpLgr { get; set; }

        public string TomaEndereco { get; set; }

		public string TomaNumero { get; set; }

		public string TomaBairro { get; set; }

        public string TomaComplemento { get; set; }

        public string TomacMun { get; set; }

		public string TomaxMun { get; set; }

        public string TomaUF { get; set; }

        public string TomaPais { get; set; }

		public string TomaCEP { get; set; }

        /// <summary>
        /// <para>
        /// Exclusivo do padrão BSIT-BR.</para><para>
        /// Valores Aceitos:</para><para>
        /// CE - Celular</para><para>
        /// CO - Comercial</para><para>
        /// RE - Residencial</para>
        /// </summary>
        public string TomaTipoTelefone { get; set; }

        public string TomaTelefone { get; set; }

		public string TomaEmail { get; set; }

		public string TomaIE { get; set; }

        public string TomaIM { get; set; }

        /// <summary>
        /// Exclusivo do padrão INFISC
        /// </summary>
        public string TomaIME { get; set; }
    }

    public class IntermServico
    {
        public string IntermCNPJ { get; set; }

        public string IntermCPF { get; set; }

        public string IntermRazaoSocial { get; set; }

        public string IntermIM { get; set; }

        public string IntermEmail { get; set; }

        public string IntermEndereco { get; set; }

        public string IntermNumero { get; set; }

        public string IntermComplemento { get; set; }

        public string IntermBairro { get; set; }

        public string IntermCep { get; set; }

        public string IntermCmun { get; set; }

        public string IntermXmun { get; set; }

        public string IntermFone { get; set; }

        public string ItermIE { get; set; }
    }
}