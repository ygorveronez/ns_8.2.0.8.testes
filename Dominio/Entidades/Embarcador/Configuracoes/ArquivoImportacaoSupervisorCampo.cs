namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_IMPORTACAO_SUPERVISOR_CAMPOS", EntityName = "ArquivoImportacaoSupervisorCampo", Name = "Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoSupervisorCampo", NameType = typeof(ArquivoImportacaoSupervisorCampo))]
    public class ArquivoImportacaoSupervisorCampo : Configuracoes.ArquivoImportacao<ArquivoImportacaoSupervisor>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ASC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoImportacaoSupervisor", Column = "AIS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ArquivoImportacaoSupervisor Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Propriedade", Column = "ASC_PROPRIEDADE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public override string Propriedade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "ASC_POSICAO", TypeType = typeof(int), NotNull = true)]
        public override int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropriedade", Column = "ASC_TIPO_PROPRIEDADE", TypeType = typeof(int), NotNull = true)]
        public override Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo TipoPropriedade { get; set; }      
    }
}
