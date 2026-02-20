using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Integracao.Monitoramento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_TECNOLOGIA_MONITORAMENTO_CONTA", EntityName = "ConfiguracaoIntegracaoTecnologiaMonitoramentoConta", Name = "Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta", NameType = typeof(ConfiguracaoIntegracaoTecnologiaMonitoramentoConta))]
    public class ConfiguracaoIntegracaoTecnologiaMonitoramentoConta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoIntegracaoTecnologiaMonitoramento", Column = "CIT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento Configuracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_NOME", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_HABILITADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Habilitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_TIPO_COMUNICACAO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao TipoComunicacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_PROTOCOLO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_SERVIDOR", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Servidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_PORTA", TypeType = typeof(int), NotNull = false)]
        public virtual int Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_URI", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_USUARIO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_SENHA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_BANCO_DADOS", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string BancoDeDados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_CHARSET", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Charset { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_DIRETORIO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Diretorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_ARQUIVO_CONTROLE", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ArquivoControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_PARAMETROS_ADICIONAIS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ParametrosAdicionais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_SOLICITANTE_SENHA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string SolicitanteSenha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_SOLICITANTE_ID", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string SolicitanteId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_RASTREADOR_ID", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string RastreadorId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_BUSCAR_DADOS_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarDadosVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_USA_POSICAO_FROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsaPosicaoFrota { get; set; }

        public virtual string Descricao { get { return Nome; } }
    }
}
