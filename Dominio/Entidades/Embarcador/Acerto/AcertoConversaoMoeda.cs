using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_CONVERSAO_MOEDA", EntityName = "AcertoConversaoMoeda", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda", NameType = typeof(AcertoConversaoMoeda))]

    public class AcertoConversaoMoeda : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOrigem", Column = "ACM_VALOR_ORIGEM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentralOrigem", Column = "ACM_MOEDA_COTACAO_BANCO_CENTRAL_ORIGEM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral MoedaCotacaoBancoCentralOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentralDestino", Column = "ACM_MOEDA_COTACAO_BANCO_CENTRAL_DESTINO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral MoedaCotacaoBancoCentralDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCotacao", Column = "ACM_VALOR_COTACAO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFinal", Column = "ACM_VALOR_FINAL", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorFinal { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "De " + this.MoedaCotacaoBancoCentralOrigem.ObterDescricaoSimplificada() + " para " + this.MoedaCotacaoBancoCentralDestino.ObterDescricaoSimplificada();
            }
        }
        public virtual bool Equals(AcertoConversaoMoeda other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
