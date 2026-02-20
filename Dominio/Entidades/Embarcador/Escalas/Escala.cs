using System;

namespace Dominio.Entidades.Embarcador.Escalas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ESCALAS", EntityName = "Escala", Name = "Dominio.Entidades.Embarcador.Escalas.Escala", NameType = typeof(Escala))]
    public class Escala : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ESC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ESC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ESC_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ESC_CLASSIFICACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala Classificacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ESC_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ESC_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                return Status ? "Ativo" : "Inativo";
            }
        }

        public virtual string DescricaoClassificacao
        {
            get
            {
                switch (Classificacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala.A:
                        return "A";
                    case ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala.B:
                        return "B";
                    case ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala.C:
                        return "C";
                    default:
                        return "";
                }
            }
        }
    }
}
