using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO_FILIAL", EntityName = "CarregamentoFilial", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial", NameType = typeof(CarregamentoFilial))]
    public class CarregamentoFilial : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carregamento Carregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamentoCarga", Column = "CGF_DATA_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDescarregamentoCarga", Column = "CGF_DATA_DESCARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDescarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGF_ENCAIXAR_HORARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncaixarHorario { get; set; }
    }
}
