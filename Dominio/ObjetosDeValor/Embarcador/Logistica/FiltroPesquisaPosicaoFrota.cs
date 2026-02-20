using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaPosicaoFrota
    {
        public int CodigoVeiculo { get; set; }
        public string PlacaVeiculo { get; set; }
        public List<int> CodigosVeiculo { get; set; }

        public List<int> CodigosVeiculoFiltro { get; set; }
        public List<Enumeradores.TipoModeloVeicularCarga> TipoModeloVeicular { get; set; }
        public int Transportador { get; set; }
        public List<double> CodigosTransportador { get; set; }
        public double Cliente { get; set; }
        public int CategoriaPessoa { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public List<Enumeradores.SituacaoPosicaoFrota> Situacoes { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string CodigoUltimaCargaEmbarcador { get; set; }
        public List<int> CodigosGrupoStatusViagem { get; set; }
        public List<int> CodigosGrupoTipoOperacao { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<double> CodigosRecebedores { get; set; }
        public List<int> CodigosFilialVenda { get; set; }
        public bool EmAlvo { get; set; }
        public bool VeiculosComMonitoramento { get; set; }
        public bool VeiculosComContratoDeFrete { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<Enumeradores.SituacaoVeiculo> SituacaoVeiculo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusViagemControleEntrega? StatusViagemControleEntrega { get; set; }
        public bool ClientesComVeiculoEmAlvo { get; set; }
        public bool ClientesAlvosEstrategicos { get; set; }
        public bool BuscarPreCarga { get; set; }
        public List<double> CpfCnpjDestinatarios { get; set; }
        public List<double> CpfCnpjRemetentes { get; set; }
        public List<int> Motoristas { get; set; }
        public List<int> CodigosCentroResultado { get; set; }
        public List<int> CodigosFuncionarioResponsavel { get; set; }
        public List<int> GrupoPessoas { get; set; }
        public bool MonitoramentoCritico { get; set; }
        public List<int> CodigosContratoFrete { get; set; }
        public List<int> CodigosTipoContratoFrete { get; set; }
        public List<int> CodigosLocais { get; set; }
        public bool FiltrarCargasPorParteDoNumero { get; set; }
        public List<int> TecnologiaRastreador { get; set; }
        public int RaioFilial { get; set; }
        public bool VeiculosDentroDoRaioFilial { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int? Situacao { get; set; }
        public bool ExibirFiliaisEBases { get; set; }
        public int TempoSemPosicaoParaVeiculoPerderSinal { get; set; }
        public bool? RastreadorOnlineOffline { get; set; }
        public string DescricaoSituacaoCarga
        {
            get
            {
                if (!Situacao.HasValue)
                    return string.Empty;

                var situacaoEnum = (SituacaoCarga)Situacao.Value;

                return situacaoEnum.ObterDescricao();
            }
        }
    }
}
