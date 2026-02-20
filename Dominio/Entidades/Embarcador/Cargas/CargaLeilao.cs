using System;


namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_LEILAO", EntityName = "CargaLeilao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaLeilao", NameType = typeof(CargaLeilao))]
    public class CargaLeilao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaLeilao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Leilao", Column = "LEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Leilao Leilao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LeilaoParticipante", Column = "LEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante LeilaoParticipanteEscolhido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLance", Column = "CLE_VALOR_LANCE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorLance { get; set; }

        public virtual bool Equals(CargaLeilao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
