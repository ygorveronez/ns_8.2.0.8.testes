using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OBSERVACAO_CONTRIBUINTE", EntityName = "ObservacaoContribuinte", Name = "Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte", NameType = typeof(ObservacaoContribuinte))]
    public class ObservacaoContribuinte : EntidadeBase, IEquatable<ObservacaoContribuinte>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_IDENTIFICADOR", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Identificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Texto", Column = "OCO_TEXTO", TypeType = typeof(string), Length = 160, NotNull = true)]
        public virtual string Texto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }
        
        public virtual string Descricao
        {
            get
            {
                return this.Texto;
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual bool Equals(ObservacaoContribuinte other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual ObjetosDeValor.CTe.Observacao ObterObservacaoCTe()
        {
            return new ObjetosDeValor.CTe.Observacao()
            {
                Descricao = this.Texto,
                Identificador = this.Identificador
            };
        }
    }
}