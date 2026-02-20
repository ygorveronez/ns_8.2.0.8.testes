namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_SERVICO_SERVICO_VEICULO", EntityName = "GrupoServicoServicoVeiculo", Name = "Dominio.Entidades.Embarcador.Frota.GrupoServicoServicoVeiculo", NameType = typeof(GrupoServicoServicoVeiculo))]
    public class GrupoServicoServicoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GSV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoServico", Column = "GSF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoServico GrupoServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoVeiculoFrota", Column = "SEV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ServicoVeiculoFrota ServicoVeiculoFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "GSV_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoVeiculo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServicoVeiculo Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidadeKM", Column = "GSV_VALIDADE_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int ValidadeKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaKM", Column = "GSV_TOLERANCIA_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidadeDias", Column = "GSV_VALIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int ValidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaDias", Column = "GSV_TOLERANCIA_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidadeHorimetro", Column = "GSV_VALIDADE_HORIMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int ValidadeHorimetro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaHorimetro", Column = "GSV_TOLERANCIA_HORIMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaHorimetro { get; set; }
    }
}
