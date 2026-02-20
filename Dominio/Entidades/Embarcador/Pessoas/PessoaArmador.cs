using Dominio.Entidades.Embarcador.Pedidos;
using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_ARMADOR", EntityName = "PessoaArmador", Name = "Dominio.Entidades.Embarcador.Pessoas.PessoaArmador", NameType = typeof(PessoaArmador))]
    public class PessoaArmador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasFreetime", Column = "PEA_DIAS_FREETIME", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasFreetime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDariaAposFreetime", Column = "PEA_VALOR_DIARIA_APOS_FREETIME", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal? ValorDariaAposFreetime { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerTipo", Column = "CTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContainerTipo ContainerTipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigenciaInicial", Column = "PEA_DATA_VIGENCIA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVigenciaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigenciaFinal", Column = "PEA_DATA_VIGENCIA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVigenciaFinal { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

    }
}
