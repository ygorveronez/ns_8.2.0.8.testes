using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_COMISSAO_MOTORISTA", EntityName = "ConfiguracaoComissaoMotorista", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoComissaoMotorista", NameType = typeof(ConfiguracaoComissaoMotorista))]
    public class ConfiguracaoComissaoMotorista:EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCM_PERCENTUAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Percentual { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "CCM_PERCENTUAL_BASE_CALCULO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualDaBaseDeCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCM_DATA_BASE", TypeType = typeof(int), NotNull = false)]
        public virtual DataBaseComissaoMotorista DataBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCM_UTILIZA_CONTROLE_PERCENTUAL_EXECUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaControlePercentualExecucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCM_BLOQUEAR_ALTERACAO_VALOR_FRETE_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearAlteracaoValorFreteLiquido { get; set; }

    }
}