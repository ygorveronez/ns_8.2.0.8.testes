namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_FLUXO_PATIO", EntityName = "ConfiguracaoFluxoPatio", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoFluxoPatio", NameType = typeof(ConfiguracaoFluxoPatio))]
    public class ConfiguracaoFluxoPatio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComprovanteSaida", Column = "COF_TIPO_COMPROVANTE_SAIDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComprovanteSaida), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComprovanteSaida? TipoComprovanteSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_PERMITIR_BAIXAR_ROMANEIO_NA_ETAPA_FIM_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirBaixarRomaneioNaEtapaFimCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_VALIDAR_PESO_CARGA_COM_PESAGEM_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ValidarPesoCargaComPesagemVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_PERMITE_ALOCAR_VEICULO_SEM_CONJUNTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlocarVeiculoSemConjuntoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_REGISTRAR_POSICAO_VEICULO_SUBAREA_AO_RECEBER_EVENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegistrarPosicaoVeiculoSubareaAoReceberEvento { get; set; }
    }
}
