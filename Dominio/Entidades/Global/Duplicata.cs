using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DUPLICATA", EntityName = "Duplicata", Name = "Dominio.Entidades.Duplicata", NameType = typeof(Duplicata))]

    public class Duplicata : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DUP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "DUP_FUNCIONARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "DUP_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "DUP_TIPO", TypeType = typeof(Enumeradores.TipoDuplicata), NotNull = true)]
        public virtual Enumeradores.TipoDuplicata Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "DUP_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamento", Column = "DUP_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "DUP_DOCUMENTO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Documento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDocumento", Column = "DUP_DATA_DOCUMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "DUP_VALOR", TypeType = typeof(decimal), NotNull = true, Scale = 6, Precision = 18)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Acrescimo", Column = "DUP_ACRESCIMO", TypeType = typeof(decimal), NotNull = true, Scale = 6, Precision = 18)]
        public virtual decimal Acrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "DUP_DESCONTO", TypeType = typeof(decimal), NotNull = true, Scale = 6, Precision = 18)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "DUP_VEICULO1", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo1 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "DUP_VEICULO2", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo2 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "DUP_VEICULO3", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo3 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "DUP_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoDeConta PlanoDeConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "DUP_OBS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntrada", Column = "DOE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntrada DocumentoEntrada { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Embarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoVeiculo", Column = "VTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoVeiculo TipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DUP_DADOS_BANCARIOS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string DadosBancarios { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_ADICIONAIS_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade AdicionaisCidadeOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_ADICIONAIS_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade AdicionaisCidadeDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DUP_ADICIONAIS_PESO", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal AdicionaisPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DUP_ADICIONAIS_VOLUME", TypeType = typeof(int), NotNull = false)]
        public virtual int AdicionaisVolumes { get; set; }
    }
}
