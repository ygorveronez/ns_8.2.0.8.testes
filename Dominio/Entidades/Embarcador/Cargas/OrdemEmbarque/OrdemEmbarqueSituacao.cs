namespace Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_EMBARQUE_SITUACAO", EntityName = "OrdemEmbarqueSituacao", Name = "Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao", NameType = typeof(OrdemEmbarqueSituacao))]
    public class OrdemEmbarqueSituacao: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "OES_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "OES_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "OES_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoComCodigoIntegracao
        {
            get { return $"{CodigoIntegracao} - {Descricao}"; }
        }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
