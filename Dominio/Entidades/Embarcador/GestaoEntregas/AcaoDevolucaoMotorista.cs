using System;

namespace Dominio.Entidades.Embarcador.GestaoEntregas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACAO_DEVOLUCAO_MOTORISTA", EntityName = "AcaoDevolucaoMotorista", Name = "Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista", NameType = typeof(AcaoDevolucaoMotorista))]
    public class AcaoDevolucaoMotorista : EntidadeBase, IEquatable<AcaoDevolucaoMotorista>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ADM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ADM_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "ADM_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ADM_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(AcaoDevolucaoMotorista other)
        {
            return (this.Codigo == other.Codigo);
        }

    }
}
