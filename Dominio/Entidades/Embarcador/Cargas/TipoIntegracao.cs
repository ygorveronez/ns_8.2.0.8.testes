using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_INTEGRACAO", EntityName = "TipoIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.TipoIntegracao", NameType = typeof(TipoIntegracao))]
    public class TipoIntegracao : EntidadeBase, IEquatable<TipoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TPI_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Grupo", Column = "TPI_GRUPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GrupoTipoIntegracao), NotNull = false)]
        public virtual GrupoTipoIntegracao Grupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEnvio", Column = "TPI_TIPO_ENVIO", TypeType = typeof(TipoEnvioIntegracao), NotNull = true)]
        public virtual TipoEnvioIntegracao TipoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeMaximaEnvioLote", Column = "TPI_QTD_MAXIMA_ENVIO_LOTE", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMaximaEnvioLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarIntegracaoNasOcorrencias", Column = "TPI_GERAR_INTEGRACAO_NAS_OCORRENCIAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarIntegracaoNasOcorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TPI_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_CONTROLE_POR_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlePorLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_INTEGRACAO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_INTEGRAR_CARGA_TRANSBORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarCargaTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_GERAR_INTEGRACAO_FECHAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarIntegracaoFechamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_GERAR_INTEGRACAO_DADOS_TRANSPORTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarIntegracaoDadosTransporteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TPI_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; } //true

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_NAO_SUBTRAIR_VALE_PEDAGIO_DO_CONTRATO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSubtrairValePedagioDoContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tentativas", Column = "TPI_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int Tentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_INTEGRAR_VEICULO_TROCA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarVeiculoTrocaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_INTEGRAR_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_INTEGRAR_COM_PLATAFORMA_NSTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarComPlataformaNstech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_TEMPO_CONSULTA_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoConsultaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rastreador", Column = "TPI_RASTREADOR", TypeType = typeof(EnumTecnologiaRastreador), NotNull = false)]
        public virtual EnumTecnologiaRastreador Rastreador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_BLOQUEAR_ETAPA_TRANSPORTE_SE_REJEITAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEtapaTransporteSeRejeitar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_PERMITIR_REENVIO_EXCECAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirReenvioExcecao { get; set; }


        public virtual string DescricaoTipo
        {
            get { return Tipo.ObterDescricao(); }
        }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual string DescricaoGrupo
        {
            get { return Grupo.ObterDescricao(); }
        }

        public virtual bool Equals(TipoIntegracao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
