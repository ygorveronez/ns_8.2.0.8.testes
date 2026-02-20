using Dominio.Entidades.Embarcador.Filiais;
using Dominio.Entidades.Embarcador.Veiculos;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_OFERTA_ROTA", EntityName = "BiddingOfertaRota", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota", NameType = typeof(BiddingOfertaRota))]
    public class BiddingOfertaRota : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingOferta", Column = "TBO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Bidding.BiddingOferta BiddingOferta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_FREQUENCIA", TypeType = typeof(Int32), NotNull = false)]
        public virtual int Frequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_VOLUME", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Volume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_NUMERO_ENTREGA", TypeType = typeof(Int32), NotNull = false)]
        public virtual int NumeroEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_VALOR_CARGA_MES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCargaMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_FRANQUIA_KM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal QuilometragemMedia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_PESO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_ADICIONAL_A_PARTIR_DA_ENTREGA_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int AdicionalAPartirDaEntregaNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_OBSERVACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_FLAG_ORIGEM", TypeType = typeof(string), NotNull = false)]
        public virtual string FlagOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_FLAG_DESTINO", TypeType = typeof(string), NotNull = false)]
        public virtual string FlagDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoModeloVeicular", Column = "MVG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular GrupoModeloVeicular { get; set; }

        [Obsolete("Campo solicitado de forma errônea.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Localidades.Regiao Regiao { get; set; }

        [Obsolete("Campo não está mais sendo utilizado.")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "CarroceriaVeiculo", Column = "TBR_CARROCERIA_VEICULO", TypeType = typeof(CarroceriaVeiculo), NotNull = false)]
        public virtual CarroceriaVeiculo? CarroceriaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloImportacao", Column = "TBR_PROTOCOLO_IMPORTACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int ProtocoloImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_FREQUENCIA_MENSAL_COM_AJUDANTE", TypeType = typeof(int), NotNull = false)]
        public virtual int FrequenciaMensalComAjudante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_QUANTIDADE_AJUDANTE_POR_VEICULO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeAjudantePorVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_MEDIA_ENTREGAS_FRACIONADA", TypeType = typeof(int), NotNull = false)]
        public virtual int MediaEntregasFracionada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_MAXIMA_ENTREGAS_FRACIONADA", TypeType = typeof(int), NotNull = false)]
        public virtual int MaximaEntregasFacionada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_QUANTIDADE_VIAGENS_POR_ANO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeViagensPorAno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Inconterm", Column = "TBR_INCONTERM", TypeType = typeof(Inconterm), NotNull = false)]
        public virtual Inconterm? Inconterm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_VOLUME_TON_ANO", TypeType = typeof(int), NotNull = false)]
        public virtual int VolumeTonAno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_VOLUME_TON_VIAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int VolumeTonViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_VALOR_MEDIO_NFE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMedioNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoColeta", Column = "TBR_TEMPO_COLETA", TypeType = typeof(TimeSpan), NotNull = false)]
        public virtual TimeSpan? TempoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDescarga", Column = "TBR_TEMPO_DESCARGA", TypeType = typeof(TimeSpan), NotNull = false)]
        public virtual TimeSpan? TempoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Compressor", Column = "TBR_COMPRESSOR", TypeType = typeof(SimNaoNA), NotNull = false)]
        public virtual SimNaoNA? Compressor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_ALICOTA_PADRAO_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? AlicotaPadraoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_DESCRICAO_ORIGEM", TypeType = typeof(string), NotNull = false)]
        public virtual string DescricaoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBR_DESCRICAO_DESTINO", TypeType = typeof(string), NotNull = false)]
        public virtual string DescricaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "CAR_CODIGO")]
        public virtual ICollection<Cargas.TipoDeCarga> TiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeiculares", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "VEI_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> ModelosVeiculares { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FiliaisParticipante", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_FILIAL_PARTICIPANTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Filial> FiliaisParticipante { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Origens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> Origens { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EstadosOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_ESTADO_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> EstadosOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PaisesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_PAIS_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Pais", Column = "PAI_CODIGO")]
        public virtual ICollection<Pais> PaisesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegioesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_REGIAO_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Regiao", Column = "REG_CODIGO")]
        public virtual ICollection<Localidades.Regiao> RegioesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_CLIENTE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RotasOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_ROTA_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFrete", Column = "ROF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFrete> RotasOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CEPsOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_CEP_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BiddingOfertaRotaCEPOrigem", Column = "TCO_CODIGO")]
        public virtual IList<BiddingOfertaRotaCEPOrigem> CEPsOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Destinos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> Destinos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EstadosDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_ESTADO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> EstadosDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PaisesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_PAIS_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Pais", Column = "PAI_CODIGO")]
        public virtual ICollection<Pais> PaisesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegioesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_REGIAO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Regiao", Column = "REG_CODIGO")]
        public virtual ICollection<Localidades.Regiao> RegioesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_CLIENTE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RotasDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_ROTA_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFrete", Column = "ROF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFrete> RotasDestino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CEPsDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_OFERTA_ROTA_CEP_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BiddingOfertaRotaCEPDestino", Column = "TCD_CODIGO")]
        public virtual IList<BiddingOfertaRotaCEPDestino> CEPsDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloCarroceria", Column = "MCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloCarroceria ModeloCarroceria { get; set; }
    }
}
