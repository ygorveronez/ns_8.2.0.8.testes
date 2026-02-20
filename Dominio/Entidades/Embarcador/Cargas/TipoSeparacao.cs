using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_SEPARACAO", EntityName = "TipoSeparacao", Name = "Dominio.Entidades.Embarcador.Cargas.TipoSeparacao", NameType = typeof(TipoSeparacao))]
    public class TipoSeparacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.TipoSeparacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TSE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTipoSeparacaoEmbarcador", Column = "TSE_CODIGO_TIPO_SEPARACAO_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoTipoSeparacaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TSE_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PadraoMontagemCarregamentoAuto", Column = "TSE_PADRAO_MONTAGEM_CARREGAMENTO_AUTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PadraoMontagemCarregamentoAuto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TSE_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }     

        public virtual bool Equals(TipoSeparacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
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
    }
}
