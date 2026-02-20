using System;

namespace Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONSULTA_ABASTECIMENTO_ANGELLIRA", EntityName = "ConsultaAbastecimentoAngelLira", Name = "Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLira", NameType = typeof(ConsultaAbastecimentoAngelLira))]
    public class ConsultaAbastecimentoAngelLira : EntidadeBase, IEquatable<ConsultaAbastecimentoAngelLira>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConsulta", Column = "CAA_DATA_CONSULTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "CAA_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "CAA_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "CAA_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        public virtual string Descricao
        {
            get { return this.DataConsulta > DateTime.MinValue ? this.DataConsulta.ToString("dd/MM/yyyy") : "Sem data"; }
        }

        public virtual bool Equals(ConsultaAbastecimentoAngelLira other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
