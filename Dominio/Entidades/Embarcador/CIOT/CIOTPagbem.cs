namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_PAGBEM", EntityName = "CIOTPagbem", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTPagbem", NameType = typeof(CIOTPagbem))]
    public class CIOTPagbem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UsuarioPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_TIPO_FILIAL_CONTRATANTE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilialContratantePagbem), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilialContratantePagbem TipoFilialContratante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_NAO_INTEGRAR_RESPONSAVEL_CARTAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIntegrarResponsavelCartaoPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_INTEGRAR_NUMERO_RPS_NFSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarNumeroRPSNFSE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_LIBERAR_VIAGEM_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarViagemManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_CNPJ_EMPRESA_CONTRATANTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CNPJEmpresaContratante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_TIPO_TOLERANCIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_FRETE_TIPO_PESO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string FreteTipoPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_QUEBRA_TIPO_COBRANCA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string QuebraTipoCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_QUEBRA_TOLERANCIA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuebraTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCP_UTILIZAR_CNPJ_CONTRATANTE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCnpjContratanteIntegracao { get; set; }       

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
