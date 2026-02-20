using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_VALE", EntityName = "ValeDoAcertoDeViagem", Name = "Dominio.Entidades.ValeDoAcertoDeViagem", NameType = typeof(ValeDoAcertoDeViagem))]
    public class ValeDoAcertoDeViagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "AVA_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoDeViagem", Column = "ACE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AcertoDeViagem AcertoDeViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "AVA_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoValeAcertoViagem), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoValeAcertoViagem Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "AVA_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "AVA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "AVA_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "AVA_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case Enumeradores.TipoValeAcertoViagem.Vale:
                        return "Vale";
                    case Enumeradores.TipoValeAcertoViagem.Devolucao:
                        return "Devolução";
                    default:
                        return "";
                }
            }
        }

        public virtual int NumeroAcerto
        {
            get
            {
                return AcertoDeViagem?.Numero ?? 0;
            }
        }
    }
}
