using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_PALLETS", EntityName = "FechamentoPallets", Name = "Dominio.Entidades.Embarcador.Pallets.FechamentoPallets", NameType = typeof(FechamentoPallets))]
    public class FechamentoPallets : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEP_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEP_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "FEP_DATA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEP_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEP_DATA_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEP_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets Situacao { get; set; }

        public virtual string SituacaoDescricao
        {
            get
            {
                return Situacao.ObterDescricao();
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }
    }
}