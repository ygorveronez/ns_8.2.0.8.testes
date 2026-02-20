using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MIC_DTA", EntityName = "CargaMICDTA", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMICDTA", NameType = typeof(CargaMICDTA))]
    public class CargaMICDTA : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CMD_NUMERO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CMD_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "CMD_SITUACAO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencial", Column = "CMD_NUMERO_SEQUENCIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSequencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SiglaPaisOrigem", Column = "CMD_SIGLA_PAIS_ORIGEM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SiglaPaisOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLicencaTNTI", Column = "CMD_NUMERO_LICENCA_TNTI", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroLicencaTNTI { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMICDTA Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaMICDTA)this.MemberwiseClone();
        }
        public virtual string Descricao
        {
            get
            {
                return (this.Carga?.Descricao ?? string.Empty) + " - " + (this.Numero ?? string.Empty);
            }
        }

        public virtual bool Equals(CargaMICDTA other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
