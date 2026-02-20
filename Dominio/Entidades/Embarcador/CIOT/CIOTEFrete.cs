namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_EFRETE", EntityName = "CIOTEFrete", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTEFrete", NameType = typeof(CIOTEFrete))]
    public class CIOTEFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegradorEFrete", Column = "CEF_CODIGO_INTEGRADOR_EFRETE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoIntegradorEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioEFrete", Column = "CEF_USUARIO_EFRETE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaEFrete", Column = "CEF_SENHA_EFRETE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaEFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "CEF_EMPRESA_MATRIZ_EFRETE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa MatrizEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTipoCarga", Column = "CEF_CODIGO_TIPO_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int? CodigoTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmissaoGratuita", Column = "CEF_EMISSAO_GRATUITA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EmissaoGratuita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "CEF_TIPO_PAGAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete? TipoPagamento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

    }
}
