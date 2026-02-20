using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CONCILIACAO_TRANSPORTADOR", EntityName = "ConfiguracaoConciliacaoTransportador", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador", NameType = typeof(ConfiguracaoConciliacaoTransportador))]
    public class ConfiguracaoConciliacaoTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Se habilitado, toda vez que passar a periodicidade definida, automaticamente será criado um ConciliacaoTransportador 
        /// com os títulos dentro do último período
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_HABILITAR_GERACAO_AUTOMATICA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool HabilitarGeracaoAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Periodicidade", Column = "CCT_PERIODICIDADE", TypeType = typeof(PeriodicidadeConciliacaoTransportador), NotNull = true)]
        public virtual PeriodicidadeConciliacaoTransportador Periodicidade { get; set; }

        /// <summary>
        /// Caso a periodicidade seja Bimestral, Trimestral ou Semestral, esse atributo guarda em quais meses o período começa.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "SequenciaPeriodicidade", Column = "CCT_SEQUENCIA_PERIODICIDADE", TypeType = typeof(SequenciaPeriodicidadeConciliacaoTransportador), NotNull = false)]
        public virtual SequenciaPeriodicidadeConciliacaoTransportador? SequenciaPeriodicidade { get; set; }

        /// <summary>
        /// Se a Conciliação será gerada para vários transportadores que compartilham a raiz (primeiros 8 dígitos) do CNPJ
        /// ou separadamente para cada um deles.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCnpj", Column = "CCT_TIPO_CNPJ", TypeType = typeof(TipoCnpjConciliacaoTransportador), NotNull = true)]
        public virtual TipoCnpjConciliacaoTransportador TipoCnpj { get; set; }

        /// <summary>
        /// Uma vez aberto a ConciliacaoTransportador, esse será a quantidade de dias mínima que será esperado até o Transportador poder
        /// assinar a anuência. Nesse período, o Transportador pode contestar a Conciliacao através de Ocorrencias.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_DIAS_PARA_CONTESTACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int DiasParaContestacao { get; set; }



        /// <summary>
        /// PROPRIEDADES USADAS PELA THREAD PARA PROCESSAR CARGA_CTE DE UM DETERMINADO PERIODO E TRANSPORTADOR PARA SER ATIVADA MANUALMENTE (NAO ESTA EM TELA)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_CODIGO_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int codEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_INICIO_PROCESSO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? InicioProcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_FIM_PROCESSO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? FimProcesso { get; set; }


        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

    }

}
