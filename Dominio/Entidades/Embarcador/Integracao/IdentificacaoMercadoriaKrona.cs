namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IDENTIFICACAO_MERCADORIA_KRONA", EntityName = "IdentificacaoMercadoriaKrona", Name = "Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona", NameType = typeof(IdentificacaoMercadoriaKrona))]
    public class IdentificacaoMercadoriaKrona : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IMK_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identificador", Column = "IMK_IDENTIFICADOR", TypeType = typeof(int), NotNull = true)]
        public virtual int Identificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificadorDescricao", Column = "IMK_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string IdentificadorDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "IMK_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao
        {
            get { return $"{Identificador} - {IdentificadorDescricao}"; }
        }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
