using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CENTRO_CARREGAMENTO_DISPONIBILIDADE_FROTA", EntityName = "CentroCarregamentoDisponibilidadeFrota", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota", NameType = typeof(CentroCarregamentoDisponibilidadeFrota))]
    public class CentroCarregamentoDisponibilidadeFrota : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "CDF_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dia", Column = "CDF_DIA", TypeType = typeof(DiaSemana), NotNull = true)]
        public virtual DiaSemana Dia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        public virtual string Descricao
        {
            get { return Dia.ObterDescricaoResumida(); }
        }

        public virtual CentroCarregamentoDisponibilidadeFrota Clonar()
        {
            CentroCarregamentoDisponibilidadeFrota disponibilidade = (CentroCarregamentoDisponibilidadeFrota)this.MemberwiseClone();
            return disponibilidade;
        }
    }
}
