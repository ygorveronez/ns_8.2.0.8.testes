using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{

    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_OCORRENCIA_COLETA_ENTREGA", EntityName = "OcorrenciaColetaEntrega", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega", NameType = typeof(OcorrenciaColetaEntrega))]
    public class OcorrenciaColetaEntrega : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataOcorrencia", Column = "OCE_DATA_OCORRENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCE_PACOTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Pacote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "OCE_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCE_PENDENTE_ENVIO_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteEnvioEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCE_RETORNO_ENVIO_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string RetornoEnvioEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "OCE_LATITUDE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "OCE_LONGITUDE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPosicao", Column = "OCE_DATA_POSICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoRecalculada", Column = "OCE_DATA_PREVISAO_RECALCULADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoRecalculada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaAteDestino", Column = "OCE_DISTANCIA_DESTINO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal DistanciaAteDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoPercurso", Column = "OCE_TEMPO_PERCURSO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TempoPercurso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoOcorrencia", Column = "OCE_OBSERVACAO_OCORRENCIA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemOcorrencia", Column = "OCE_ORIGEM_OCORRENCIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCriacaoOcorrencia), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCriacaoOcorrencia? OrigemOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TiposCausadoresOcorrencia", Column = "TPO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia TiposCausadoresOcorrencia { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
