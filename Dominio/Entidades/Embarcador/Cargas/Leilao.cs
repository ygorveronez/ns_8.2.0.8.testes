using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LEILAO", EntityName = "Leilao", Name = "Dominio.Entidades.Embarcador.Cargas.Leilao", NameType = typeof(Leilao))]
    public class Leilao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.Leilao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorInicial", Column = "LEI_VALOR_INICIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoLeilao", Column = "LEI_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao SituacaoLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioLeilao", Column = "LEI_DATA_INICIO_LEILAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataParaEncerramentoLeilao", Column = "LEI_PARA_ENCERRAMENTO_LEILAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataParaEncerramentoLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDeEncerramentoLeilao", Column = "LEI_DE_ENCERRAMENTO_LEILAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeEncerramentoLeilao { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "MenorLance", Column = "LEI_MENOR_LANCE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MenorLance { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDeLances", Column = "LEI_NUMERO_DE_LANCES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDeLances { get; set; }



        public virtual bool Equals(Leilao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
