using Dominio.Interfaces.Embarcador.Entidade;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_VEICULO_CONTAINER", EntityName = "CargaVeiculoContainer", Name = "Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer", NameType = typeof(CargaVeiculoContainer))]
    public class CargaVeiculoContainer : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContainer", Column = "CVC_NUMERO_CONTAINER", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetiradaCtrn", Column = "CVC_DATA_RETIRADA_CTRN", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetiradaCtrn { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Genset", Column = "CVC_GENSET", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Genset { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TaraContainer", Column = "CVC_TARA_CONTAINER", TypeType = typeof(int), NotNull = false)]
        public virtual int TaraContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MaxGross", Column = "CVC_MAX_GROSS", TypeType = typeof(int), NotNull = false)]
        public virtual int MaxGross { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroReboque", Column = "CVC_NUMERO_REBOQUE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque NumeroReboque { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
