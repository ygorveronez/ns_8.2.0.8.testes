namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COMPONENTE_FRETE", EntityName = "ComponenteFrete", Name = "Dominio.Entidades.Embarcador.Frete.ComponenteFrete", NameType = typeof(ComponenteFrete))]
    public class ComponenteFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CFR_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CFR_CODIGO_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComponenteFrete", Column = "CFR_TIPO_COMPONENTE_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComponenteFreteDOCCOB", Column = "CFR_TIPO_COMPONENTE_FRETE_DOCCOB", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFreteDOCCOB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "CFR_TIPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCampoAcertoViagem", Column = "CFR_TIPO_ACERTO_VIAGEM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCampoAcertoViagem), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCampoAcertoViagem TipoCampoAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarValorTotalAReceber", Column = "CFR_DESCONTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcrescentaValorTotalAReceber", Column = "CFR_ACRESCENTA_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcrescentaValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalAReceber", Column = "CFR_NAO_SOMAR_VALOR_TOTA_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFR_NAO_SOMAR_VALOR_TOTAL_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFR_NAO_INCLUIR_BASE_CALCULO_IMPOSTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIncluirBaseCalculoImpostos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFR_NAO_DEVE_INCIDIR_SOBRE_NOTAS_FISCAIS_PALETES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoDeveIncidirSobreNotasFiscaisPateles { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomarComponenteFreteLiquido", Column = "CFR_SOMAR_COMPONENTE_FRETE_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomarComponenteFreteLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarComponenteFreteLiquido", Column = "CFR_DESCONTAR_COMPONENTE_FRETE_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarComponenteFreteLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarComponenteNotaFiscalServico", Column = "CFR_DESCONTAR_COMPONENTE_NOTA_FISCAL_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarComponenteNotaFiscalServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComponentePertenceComposicaoFreteValor", Column = "CFR_COMPONENTE_PERTENCE_COMPOSICAO_FRETE_VALOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComponentePertenceComposicaoFreteValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComponenteApenasInformativoDocumentoEmitido", Column = "CRF_COMPONENTE_APENAS_INFORMATIVO_DOCUMENTO_EMITIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComponenteApenasInformativoDocumentoEmitido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CFR_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimirOutraDescricaoCTe", Column = "CFR_IMPRIMIR_OUTRA_DESCRICAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirOutraDescricaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimirDescricaoComponenteEmComplementos", Column = "CFR_IMPRIMIR_DESCRICAO_COMPONENTES_EM_COMPLEMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirDescricaoComponenteEmComplementos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoCTe", Column = "CFR_DESCRICAO_CTE", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string DescricaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMovimentoAutomatico", Column = "CFR_GERAR_MOVIMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_EMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_ANULACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_ANULACAO_NOTA_ANULACAO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoAnulacaoNotaAnulacaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomenteParaCargaPerigosa", Column = "CFR_SOMENTE_PARA_CARGA_PERIGOSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomenteParaCargaPerigosa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChargeId", Column = "CFR_CHARGE_ID", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChargeId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChargeCodeNet", Column = "CFR_CHARGE_CODE_NET", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChargeCodeNet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChargeCodeGross", Column = "CFR_CHARGE_CODE_GROSS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChargeCodeGross { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularPISComponente", Column = "CFR_CALCULAR_PIS_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularPISComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularICMSComponente", Column = "CFR_CALCULAR_ICMS_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularICMSComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarComponenteNFTP", Column = "CFR_ENVIAR_COMPONENTE_NFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarComponenteNFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DefinicaoDataEnvioIntegracao", Column = "CFR_DEFINICAO_DATA_ENVIO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DefinicaoDataEnvioIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DefinicaoDataEnvioIntegracao DefinicaoDataEnvioIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiBundle", Column = "CFR_POSSUI_BUNDLE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiBundle { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_COMPONENTE_FRETE_BUNDLE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteBundle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DefinicaoDataEnvioIntegracaoEmbarque", Column = "CFR_DEFINICAO_DATA_ENVIO_INTEGRACAO_EMBARQUE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DefinicaoDataEnvioIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DefinicaoDataEnvioIntegracao DefinicaoDataEnvioIntegracaoEmbarque { get; set; }

        public virtual string DescricaoComponente
        {
            get
            {
                switch (this.TipoComponenteFrete)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.FRETE:
                        return "Valor de Frete";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA:
                        return "Descarga";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM:
                        return "AD VALOREM";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS:
                        return "ICMS";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS:
                        return "ISS";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.OUTROS:
                        return "Outros";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO:
                        return "Pedágio";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS:
                        return "PIS e COFINS";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

    }
}
