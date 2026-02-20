using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_A52", EntityName = "IntegracaoA52", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52", NameType = typeof(IntegracaoA52))]
    public class IntegracaoA52 : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CPF_CNPJ", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string CPFCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_SENHA", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_URL", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_UTILIZAR_DATA_AGENDAMENTO_PEDIDO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UtilizarDataAgendamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_INTEGRAR_MACROS_DADOS_TRANSPORTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarMacrosDadosTransporteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_INTEGRAR_SITUACAO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarSituacaoMotorista { get; set; }

        [Obsolete("Flag descontinuada")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_PRODUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Producao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_URL_NOVA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLNova { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoIntegracao", Column = "COA_VERSAO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoA52Enum), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoA52Enum? VersaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_APLICAR_REGRA_lOCAL_PALLETIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AplicarRegraLocalPalletizacao { get; set; }
    }
}
