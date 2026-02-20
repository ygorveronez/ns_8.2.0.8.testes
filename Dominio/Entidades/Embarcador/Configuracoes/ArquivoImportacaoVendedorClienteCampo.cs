namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_IMPORTACAO_VENDEDOR_CLIENTE_CAMPOS", EntityName = "ArquivoImportacaoVendedorClienteCampo", Name = "Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoVendedorClienteCampo", NameType = typeof(ArquivoImportacaoVendedorClienteCampo))]
    public class ArquivoImportacaoVendedorClienteCampo : Configuracoes.ArquivoImportacao<ArquivoImportacaoVendedorCliente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoImportacaoVendedorCliente", Column = "AIC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ArquivoImportacaoVendedorCliente Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Propriedade", Column = "ACC_PROPRIEDADE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public override string Propriedade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "ACC_POSICAO", TypeType = typeof(int), NotNull = true)]
        public override int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropriedade", Column = "ACC_TIPO_PROPRIEDADE", TypeType = typeof(int), NotNull = true)]
        public override Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo TipoPropriedade { get; set; }
      
    }
}
