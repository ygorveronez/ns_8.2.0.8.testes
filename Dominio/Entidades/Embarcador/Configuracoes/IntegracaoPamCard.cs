using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_PAMCARD", EntityName = "IntegracaoPamcard", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPamcard", NameType = typeof(IntegracaoPamcard))]
    public class IntegracaoPamcard : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIM_URL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRota", Column = "CIM_TIPO_ROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRotaPamcard), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaPamcard TipoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcoesPamcard", Column = "CIM_ACOES_PAMCARD", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoPamcard), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoPamcard AcoesPamcard { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionarValorConsultadoComoComponentePedagioCarga", Column = "CIM_ADICIONAR_VALOR_CONSULTADO_COMO_COMPONENTE_PEDAGIO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarValorConsultadoComoComponentePedagioCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCertificadoFilialMatrizCompraValePedagio", Column = "CIM_UTILIZAR_CERTIFICADO_FILIAL_MATRIZ_COMPRA_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCertificadoFilialMatrizCompraValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarCEPsNaIntegracao", Column = "CIM_ENVIAR_CEPS_NA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCEPsNaIntegracao { get; set; }

        [Obsolete("Configuração temporária, não será mais usada.", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarDadosRegulatorioANTT", Column = "CIM_ENVIAR_DADOS_REGULATORIOS_ANTT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDadosRegulatorioANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomarEixosSuspensosValePedagio", Column = "CIM_SOMAR_EIXOS_SUSPENSOS_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomarEixosSuspensosValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoEnviarIdaVoltaValePedagio", Column = "CIM_NAO_ENVIAR_IDA_VOLTA_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarIdaVoltaValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarRotaFreteDaCargaNoValePedagio", Column = "CIM_CONSIDERAR_ROTA_FRETE_DA_CARGA_NO_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarRotaFreteDaCargaNoValePedagio { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração Integração Pamcard"; }
        }
    }
}
