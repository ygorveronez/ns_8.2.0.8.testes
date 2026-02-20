namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CLIENTE_FREQUENCIA_CARREGAMENTO", EntityName = "ClienteFrequenciaCarregamento", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento", NameType = typeof(Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento))]
    public class ClienteFrequenciaCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFE_DIA_SEMANA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana DiaSemana { get; set; }

    }
}
