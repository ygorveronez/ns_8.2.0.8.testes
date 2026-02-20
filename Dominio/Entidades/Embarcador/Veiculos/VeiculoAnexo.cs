namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_ANEXO", EntityName = "VeiculoAnexo", Name = "Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo", NameType = typeof(VeiculoAnexo))]
    public class VeiculoAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VEA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "VEA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "VEA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAnexoVeiculo", Column = "VEA_TIPO_ANEXO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoVeiculo), Length = 1000, NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoVeiculo TipoAnexoVeiculo { get; set; }
    }

}
