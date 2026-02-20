namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_SEGMENTO", EntityName = "SegmentoVeiculo", Name = "Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo", NameType = typeof(SegmentoVeiculo))]
    public class SegmentoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VSE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VSE_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Descricao { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "VSE_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimo", Column = "VSE_VALOR_MINIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorMinimo { get; set; }
       
        [NHibernate.Mapping.Attributes.Property(0, Name = "MetaMensal", Column = "VSE_META_MENSAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? MetaMensal { get; set; }

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
