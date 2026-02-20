using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_CARGA_BONIFICACAO", EntityName = "AcertoCargaBonificacao", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao", NameType = typeof(AcertoCargaBonificacao))]
    public class AcertoCargaBonificacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoCarga", Column = "ACC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoCarga AcertoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "ACB_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoBonificaca", Column = "ACB_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa TipoBonificaca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }
        public virtual string Descricao {
            get {
                return this.Justificativa?.Descricao;
            }
        }
        public virtual string DescricaoTipoBonificaca
        {
            get
            {
                switch (this.TipoBonificaca)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto:
                        return "Desconto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo:
                        return "Acréscimo";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(AcertoCargaBonificacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
