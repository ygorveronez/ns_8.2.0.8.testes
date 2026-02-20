using System;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_MEDIA_MODELO_PESO", EntityName = "TabelaMediaModeloPeso", Name = "Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso", NameType = typeof(TabelaMediaModeloPeso))]
    public class TabelaMediaModeloPeso : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMP_MEDIA_IDEAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MediaIdeal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMP_PESO_INICIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PesoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMP_PESO_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PesoFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeiculo", Column = "VMO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeiculo Modelo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.MediaIdeal.ToString("n2") + " " + (this.Modelo?.Descricao ?? "");
            }
        }

        public virtual bool Equals(TabelaMediaModeloPeso other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
