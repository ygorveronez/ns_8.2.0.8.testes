namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_OBSERVACAO_CTE", EntityName = "GrupoPessoasObservacaoCTe", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoCTe", NameType = typeof(GrupoPessoasObservacaoCTe))]
    public class GrupoPessoasObservacaoCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GOC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOC_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOC_IDENTIFICADOR", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Identificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOC_TEXTO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Texto { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Identificador;
            }
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
