namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_IMPORTACAO_VENDEDOR_CAMPO", EntityName = "ArquivoImportacaoVendedorCampo", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoVendedorCampo", NameType = typeof(ArquivoImportacaoVendedorCampo))]
    public class ArquivoImportacaoVendedorCampo : Configuracoes.ArquivoImportacao<ArquivoImportacaoVendedor>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoImportacaoVendedor", Column = "AIV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ArquivoImportacaoVendedor Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Propriedade", Column = "AVC_PROPRIEDADE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public override string Propriedade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "AVC_POSICAO", TypeType = typeof(int), NotNull = true)]
        public override int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropriedade", Column = "AVC_TIPO_PROPRIEDADE", TypeType = typeof(int), NotNull = true)]
        public override Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo TipoPropriedade { get; set; }      
    }
}
