namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_CANHOTO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoCanhoto", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCanhoto", NameType = typeof(ConfiguracaoTipoOperacaoCanhoto))]
    public class ConfiguracaoTipoOperacaoCanhoto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNH_NOTIFICAR_CANHOTOS_PENDENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarCanhotosPendentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNH_NOTIFICAR_CANHOTOS_PENDENTES_DIARIAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarCanhotosPendentesDiariamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNH_NOTIFICAR_CANHOTOS_REJEITADOS_DIARIAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarCanhotosRejeitadosDiariamente { get; set; }

        /// <summary>
        /// Prazo em dias
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CNH_PRAZO_APOS_DATA_EMISSAO_CANHOTO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoAposDataEmissaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNH_DIA_SEMANA_NOTIFICAR_CANHOTOS_PENDENTES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DiaSemana DiaSemanaNotificarCanhotosPendentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNH_NOTIFICAR_CANHOTOS_REJEITADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarCanhotosRejeitados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNH_NAO_PERMITE_UPLOAD_DE_CANHOTOS_COM_CTE_NAO_AUTORIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteUploadDeCanhotosComCTeNaoAutorizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNH_NAO_GERAR_CANHOTO_EM_CARGAS_COM_AO_MENOS_UM_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarCanhotoAvulsoEmCargasComAoMenosUmRecebedor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configurações para canhoto";
            }
        }
    }
}
