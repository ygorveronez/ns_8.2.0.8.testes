using System;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANEJAMENTO_FROTA_DIA_VEICULO", EntityName = "PlanejamentoFrotaDiaVeiculo", Name = "Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo", NameType = typeof(PlanejamentoFrotaDiaVeiculo))]
    public class PlanejamentoFrotaDiaVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        private string _descricao = string.Empty;
        public virtual string Descricao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_descricao))
                    _descricao = "PlanejamentoFrotaDiaVeiculo Cod.: " + Codigo.ToString() + " Placa: " + PlacaVeiculo;
                return _descricao;
            }
            set
            {
                _descricao = value;
            }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaVeiculo", Column = "PDV_PLACA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string PlacaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "CAR_VEICULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanejamentoFrotaDia", Column = "PFD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanejamentoFrotaDia PlanejamentoFrotaDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoMarfrig", Column = "PDV_OBSERVACAO_MARFRIG", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoMarfrig { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoTransportador", Column = "PDV_OBSERVACAO_TRANSPORTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoTransportador { get; set; }

        //GeradoPeloSistema == true quer dizer que o registro foi gerado pelo sistema, é uma sugestão. False indica que foi incluido pelo usuario
        [NHibernate.Mapping.Attributes.Property(0, Name = "GeradoPeloSistema", Column = "PDV_GERADO_PELO_SISTEMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradoPeloSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Roteirizado", Column = "PFV_ROTEIRIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Roteirizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Indisponivel", Column = "PFV_INDISPONIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Indisponivel { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JustificativaDeIndisponibilidadeDeFrota", Column = "JIF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual JustificativaDeIndisponibilidadeDeFrota JustificativaIndisponibilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rodizio", Column = "PDV_RODIZIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Rodizio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoEmbarque", Column = "PDV_ULTIMO_EMBARQUE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? UltimoEmbarque { get; set; } // Data último embarque dessa placa no Multi,

        [NHibernate.Mapping.Attributes.Property(0, Name = "RotaDeConhecimento", Column = "PDV_ROTA_CONHECIMENTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string RotaDeConhecimento { get; set; }
        //

        //Carga vinculada (quando já houver, informação vinda na integração das Nfs, somente informativo sem opção de edição, considerando cargas de redespacho e data/hora de corte de faturamento.),
    }
}
