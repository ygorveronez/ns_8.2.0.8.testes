namespace Dominio.Entidades.Embarcador.TorreControle
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_QUALIDADE_ENTREGA", EntityName = "ConfiguracaoQualidadeEntrega", Name = "Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega", NameType = typeof(ConfiguracaoQualidadeEntrega))]
    public class ConfiguracaoQualidadeEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CQE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VerificarDataConfirmacaoIntervaloRaio", Column = "CQE_VERIFICAR_DATA_CONFIRMACAO_INTERVALO_RAIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VerificarDataConfirmacaoIntervaloRaio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarDataHoraConfirmacaoIntervaloRaio", Column = "CQE_CONSIDERAR_DATA_HORA_CONFIRMACAO_INTERVALO_RAIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarDataHoraConfirmacaoIntervaloRaio { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
