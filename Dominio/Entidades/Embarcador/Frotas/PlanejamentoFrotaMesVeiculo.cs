
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANEJAMENTO_FROTA_MES_VEICULO", EntityName = "PlanejamentoFrotaMesVeiculo", Name = "Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo", NameType = typeof(PlanejamentoFrotaMesVeiculo))]
    public class PlanejamentoFrotaMesVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        public virtual string Descricao { get; set; } = string.Empty;

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaVeiculo", Column = "PFV_PLACA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string PlacaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "CAR_VEICULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanejamentoFrotaMes", Column = "PFM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanejamentoFrotaMes PlanejamentoFrotaMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PFV_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; } //Observacao Marfrig

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoTransportador", Column = "PFV_OBSERVACAO_TRANSPORTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "PFV_SITUACAO", TypeType = typeof(SituacaoPlanejamentoFrota), NotNull = true)]
        public virtual SituacaoPlanejamentoFrota Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RespostaDoTransportador", Column = "PFV_SITUACAO_TRANSPORTADOR", TypeType = typeof(SituacaoPlanejamentoFrota), NotNull = true)]
        public virtual RespostaTransportadorPlanejamentoFrota RespostaDoTransportador { get; set; } //Status definido pelo Transp. ou Embarcador (Confirmado ou Rejeitado)

        //[NHibernate.Mapping.Attributes.Property(0, Name = "RespostaDoEmbarcador", Column = "PFV_SITUACAO_EMBARCADOR", TypeType = typeof(RespostaEmbarcadorPlanejamentoFrota), NotNull = true)]
        //public virtual RespostaEmbarcadorPlanejamentoFrota RespostaDoEmbarcador { get; set; } //Status definido pelo EMBARCADOR/MARFRIG (Confirmado ou Rejeitado)

        //GeradoPeloSistema == true quer dizer que o registro foi gerado pelo sistema, é uma sugestão. False indica que foi incluido pelo usuario
        [NHibernate.Mapping.Attributes.Property(0, Name = "GeradoPeloSistema", Column = "PFV_GERADO_PELO_SISTEMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradoPeloSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluidoPeloTransportador", Column = "PFV_INCLUIDO_PELO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluidoPeloTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluidoPeloEmbarcador", Column = "PFV_INCLUIDO_PELO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluidoPeloEmbarcador { get; set; }

        public virtual bool TransportadorPodeExcluir()
        {
            return Situacao == SituacaoPlanejamentoFrota.EmAnaliseTransportador;
        }

        public virtual bool EmbarcadorPodeExcluir()
        {
            return Situacao == SituacaoPlanejamentoFrota.EmAnaliseEmbarcador;
        }

        
        public virtual long CodigoPorModelo
        {
            get
            {
                return $"{PlanejamentoFrotaMes.Codigo}{(ModeloVeicular?.Codigo ?? 0).ToString().PadLeft(6, '0')}".ToLong();
            }
        }


        public virtual string DescricaoPorModelo
        {
            get
            {
                return $"{PlanejamentoFrotaMes.Descricao} - {(ModeloVeicular?.Descricao ?? "")}";
            }
        }
    }
}
