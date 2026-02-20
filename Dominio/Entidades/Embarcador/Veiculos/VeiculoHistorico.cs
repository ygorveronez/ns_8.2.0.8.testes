using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_HISTORICO", EntityName = "VeiculoHistorico", Name = "Dominio.Entidades.Embarcador.Veiculos.VeiculoHistorico", NameType = typeof(VeiculoHistorico))]
    public class VeiculoHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VHI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VHI_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "VHI_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetodoAlteracao", Column = "VHI_METODO_ALTERACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MetodosAlteracaoVeiculo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MetodosAlteracaoVeiculo MetodoAlteracao { get; set; }
    }
}
