using System;

namespace Dominio.Entidades.Embarcador.GestaoPallet
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDAMENTO_COLETA_PALLET", EntityName = "AgendamentoColetaPallet", Name = "Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet", NameType = typeof(AgendamentoColetaPallet))]
    public class AgendamentoColetaPallet : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUM_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataOrdem", Column = "ACP_DATA_ORDEM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataOrdem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePallets", Column = "ACP_QUANTIDADE_PALLETS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadePallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOrdem", Column = "ACP_NUMERO_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroOrdem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ACP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColetaPallet), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColetaPallet Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ResponsavelAgendamentoPallet", Column = "ACP_RESPONSAVEL_AGENDAMENTO_PALLET", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet ResponsavelAgendamentoPallet { get; set; }


        public virtual string Descricao
        {
            get
            {
                return $"Carga: {Carga.CodigoCargaEmbarcador} ";
            }
        }
    }
}