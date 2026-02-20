using System;

namespace Dominio.Entidades.Embarcador.Cargas.Retornos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RETORNO_CARGA_COLETA_BACKHAUL_MOTIVO_CANCELAMENTO", EntityName = "MotivoCancelamentoRetornoCargaColetaBackhaul", Name = "Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul", NameType = typeof(MotivoCancelamentoRetornoCargaColetaBackhaul))]
    public class MotivoCancelamentoRetornoCargaColetaBackhaul : EntidadeBase, IEquatable<MotivoCancelamentoRetornoCargaColetaBackhaul>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RMC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RMC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCargaColeta", Column = "RMC_GERAR_CARGA_COLETA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarCargaColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "RMC_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual string DescricaoGerarCargaColeta
        {
            get { return this.GerarCargaColeta ? "Sim" : "Não"; }
        }

        public virtual bool Equals(MotivoCancelamentoRetornoCargaColetaBackhaul other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
