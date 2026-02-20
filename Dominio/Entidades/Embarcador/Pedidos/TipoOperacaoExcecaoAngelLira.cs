namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_EXCECAO_ANGEL_LIRA", EntityName = "TipoOperacaoExcecaoAngelLira", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira", NameType = typeof(TipoOperacaoExcecaoAngelLira))]
    public class TipoOperacaoExcecaoAngelLira : EntidadeBase
    
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Destino { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EAL_VALOR_MINIMO", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal ValorMinimo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "EAL_PROCEDIMENTO_EMBARQUE", TypeType = typeof(int), NotNull = true)]
        public virtual int ProcedimentoEmbarque { get; set; }
    }
}
