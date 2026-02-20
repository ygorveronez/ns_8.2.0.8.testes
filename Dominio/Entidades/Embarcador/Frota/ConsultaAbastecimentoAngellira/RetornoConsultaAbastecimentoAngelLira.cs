using System;

namespace Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RETORNO_CONSULTA_ABASTECIMENTO_ANGELLIRA", EntityName = "RetornoConsultaAbastecimentoAngelLira", Name = "Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira", NameType = typeof(RetornoConsultaAbastecimentoAngelLira))]

    public class RetornoConsultaAbastecimentoAngelLira : EntidadeBase, IEquatable<RetornoConsultaAbastecimentoAngelLira>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RBA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Abastecimento", Column = "ABA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Abastecimento Abastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConsultaAbastecimentoAngelLira", Column = "CAA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConsultaAbastecimentoAngelLira ConsultaAbastecimentoAngelLira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Posto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "RBA_PLACA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condutor", Column = "RBA_CONDUTOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Condutor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraSemFormato", Column = "RBA_DATA_HORA_SEM_FORMATO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DataHoraSemFormato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "RBA_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cordenada", Column = "RBA_CORDENADA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Cordenada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "RBA_LATITUDE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "RBA_LONGITUDE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OdometroSemFormato", Column = "RBA_ODOMETRO_SEM_FORMATO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string OdometroSemFormato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Odometro", Column = "RBA_ODOMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int Odometro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "RBA_SITUACAO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        public virtual string Descricao
        {
            get { return this.Placa; }
        }

        public virtual bool Equals(RetornoConsultaAbastecimentoAngelLira other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
