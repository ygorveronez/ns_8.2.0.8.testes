namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_LEITURA_DINAMICA_XML", EntityName = "GrupoPessoasLeituraDinamicaXml", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml", NameType = typeof(GrupoPessoasLeituraDinamicaXml))]
    public class GrupoPessoasLeituraDinamicaXml : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GLX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "GLX_TIPO_DOCUMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GLX_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LeituraDinamicaXmlOrigem", Column = "LDO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LeituraDinamicaXmlOrigem LeituraDinamicaXmlOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LeituraDinamicaXmlOrigemTagFilha", Column = "LDF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LeituraDinamicaXmlOrigemTagFilha LeituraDinamicaXmlOrigemTagFilha { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LeituraDinamicaXmlDestino", Column = "LDD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LeituraDinamicaXmlDestino LeituraDinamicaXmlDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiltrarTag", Column = "GLX_FILTRAR_TAG", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FiltrarTag { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GLX_FILTRAR_PRIMEIRO_DISPONIVEL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FiltrarPrimeiroDisponivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GLX_HABILITAR_FILTRAR_CONTEUDO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool HabilitarFiltrarConteudo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiltrarConteudoTextoInicial", Column = "GLX_FILTRAR_CONTEUDO_TEXTO_INICIAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FiltrarConteudoTextoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiltrarConteudoTextoFinal", Column = "GLX_FILTRAR_CONTEUDO_TEXTO_FINAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FiltrarConteudoTextoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFiltrarConteudo", Column = "GLX_TIPO_FILTRAR_CONTEUDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFiltrarConteudo TipoFiltrarConteudo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GLX_SUBSTITUIR_VIRGULA_POR_PONTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SubstituirVirgulaPorPonto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MaximoCaracteres", Column = "GLX_MAXIMO_CARACTERES", TypeType = typeof(int), NotNull = false)]
        public virtual int MaximoCaracteres { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RemoverCaracteres", Column = "GLX_REMOVER_CARACTERES", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string RemoverCaracteres { get; set; }

        public virtual string DescricaoTipoFiltrarConteudo
        {
            get
            {
                if (this.TipoFiltrarConteudo == ObjetosDeValor.Embarcador.Enumeradores.TipoFiltrarConteudo.TextoLivre)
                    return "Texto Livre";
                else
                    return "Expressão Regular";
            }
        }
    }
}