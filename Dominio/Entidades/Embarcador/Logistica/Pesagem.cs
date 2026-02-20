using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESAGEM", EntityName = "Pesagem", Name = "Dominio.Entidades.Embarcador.Logistica.Pesagem", NameType = typeof(Pesagem))]
    public class Pesagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PSG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Ticket retornado na integração da balança
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPesagem", Column = "PSG_CODIGO_PESAGEM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPesagem", Column = "PSG_DATA_PESAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoInicial", Column = "PSG_PESO_INICIAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoFinal", Column = "PSG_PESO_FINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusBalanca", Column = "PSG_STATUS_BALANCA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusBalanca), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusBalanca StatusBalanca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoGuarita", Column = "JCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita Guarita { get; set; }

        public virtual string Descricao
        {
            get { return CodigoPesagem; }
        }
    }
}
