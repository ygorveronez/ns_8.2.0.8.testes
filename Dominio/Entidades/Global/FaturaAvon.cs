using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AVON_FATURA", EntityName = "FaturaAvon", Name = "Dominio.Entidades.FaturaAvon", NameType = typeof(FaturaAvon))]
    public class FaturaAvon : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "FAV_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "FAV_SERIE", TypeType = typeof(int), NotNull = true)]
        public virtual int Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "FAV_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "FAV_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "FAV_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "FAV_MENSAGEM", TypeType = typeof(string), Length=10000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FAV_STATUS", TypeType = typeof(StatusFaturaAvon), NotNull = false)]
        public virtual StatusFaturaAvon Status { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Manifestos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AVON_FATURA_MANIFESTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ManifestoAvon", Column = "MAV_CODIGO")]
        public virtual ICollection<Dominio.Entidades.ManifestoAvon> Manifestos { get; set; }
    }
}
