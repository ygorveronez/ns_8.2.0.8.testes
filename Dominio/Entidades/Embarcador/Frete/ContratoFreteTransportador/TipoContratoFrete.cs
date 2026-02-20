namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_CONTRATO_FRETE", EntityName = "TipoContratoFrete", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador.TipoContratoFrete", NameType = typeof(TipoContratoFrete))]
    public class TipoContratoFrete : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCF_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCF_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCF_TIPO_ADITIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoAditivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoContratoFrete", Column = "TCF_CONTRATO_FRETE_ADITIVO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoContratoFrete ContratoFreteAditivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCF_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

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

        public virtual bool Equals(TipoContratoFrete other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
