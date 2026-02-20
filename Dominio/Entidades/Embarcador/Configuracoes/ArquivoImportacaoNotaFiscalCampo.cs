namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_IMPORTACAO_NOTA_FISCAL_CAMPO", EntityName = "ArquivoImportacaoNotaFiscalCampo", Name = "Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo", NameType = typeof(ArquivoImportacaoNotaFiscalCampo))]
    public class ArquivoImportacaoNotaFiscalCampo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoImportacaoNotaFiscal", Column = "AIN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ArquivoImportacaoNotaFiscal Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Propriedade", Column = "AIC_PROPRIEDADE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Propriedade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "AIC_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropriedade", Column = "AIC_TIPO_PROPRIEDADE", TypeType = typeof(int), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo TipoPropriedade { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Posicao.ToString() + " - " + this.Propriedade;
            }
        }
    }
}
