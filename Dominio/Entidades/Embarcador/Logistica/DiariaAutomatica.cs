using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_DIARIA_AUTOMATICA", EntityName = "DiariaAutomatica", Name = "Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica", NameType = typeof(DiariaAutomatica))]
    public class DiariaAutomatica : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DAU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        /// <summary>
        /// Quando um Chamado é cadastrado com um MotivoChamado que exige DiariaAutomatica e sua Carga é a mesma da DiariaAutomatica,
        /// sua referência fica salva aqui
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotNull = false)]
        public virtual Dominio.Entidades.Embarcador.Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "DAU_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusDiariaAutomatica), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusDiariaAutomatica Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalFreeTime", Column = "DAU_LOCAL_FREE_TIME", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime LocalFreeTime { get; set; }

        /// <summary>
        /// Tempo do FreeTime no momento que o preço da Diária Automática foi calculado pela última vez. É guardado porque o tempo pode eventualmente ser alterado no sistema,
        /// e assim mantemos o histórico. Em minutos.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "DAU_TEMPO_FREE_TIME", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoFreeTime { get; set; }

        /// <summary>
        /// Data de início da cobrança da diária. É o momento em que as horas de FreeTime estouram. Veja a documentação
        /// para entender o sistema de Free Time, caso não conheça.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "DAU_DATA_INICIO_COBRANCA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicioCobranca { get; set; }

        /// <summary>
        /// Tempo total gasto já excluindo o FreeTime configurado. Em minutos. Só é válido definitivamente quando o Status = Finalizado, mas pode ser calculando antes
        /// para o BI.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "DAU_TEMPO_TOTAL", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoTotal { get; set; }

        /// <summary>
        /// Valor total que será cobrado pela DiariaAutomatica. Só é válido definitivamente quando o Status = Finalizado, mas pode ser calculando antes
        /// para o BI.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDiaria", Column = "DAU_VALOR_DIARIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaAtualizacao", Column = "DAU_DATA_ULTIMA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaAtualizacao { get; set; }

        public virtual string Descricao { 
            get { return Codigo.ToString(); } 
        }

        public virtual decimal ValorPorHora
        {
            get {
                if (ValorDiaria > 0 && TempoTotal > 0)
                {
                    return ValorDiaria / ((decimal) TempoTotal / 60);
                }

                return 0;
            }
        }

        public virtual string StatusDescricao
        {
            get { return Status.ObterDescricao(); }
        }

        public virtual string LocalFreeTimeDescricao
        {
            get { return LocalFreeTime.ObterDescricao(); }
        }
    }
}
