using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ParametrosCalculoFrete
    {
        public ParametrosCalculoFrete()
        {
            QuantidadeEmissoesPorModeloDocumento = new Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int>();
        }

        public Dominio.Entidades.Cliente TransportadorTerceiro { get; set; }
        public Dominio.Entidades.Empresa Empresa { get; set; }
        public Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }
        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }
        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }
        public Dominio.Entidades.Cliente Tomador { get; set; }
        public List<Dominio.Entidades.Cliente> Remetentes { get; set; }
        public List<Dominio.Entidades.Cliente> Destinatarios { get; set; }
        public List<Dominio.Entidades.Localidade> Origens { get; set; }
        public List<Dominio.Entidades.Localidade> Destinos { get; set; }
        public List<Dominio.Entidades.Embarcador.Localidades.Regiao> RegioesDestino { get; set; }
        public List<int> CEPsRemetentes { get; set; }
        public List<int> CEPsDestinatarios { get; set; }
        public Dominio.Entidades.RotaFrete Rota { get; set; }
        public DateTime DataVigencia { get; set; }
        public Dominio.Entidades.Veiculo Veiculo { get; set; }
        public List<Dominio.Entidades.Veiculo> Reboques { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeiculo { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }
        public decimal ValorNotasFiscais { get; set; }
        public decimal ValorNotasFiscaisSemPallets { get; set; }
        public decimal Volumes { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal PesoTotalCarga { get; set; }
        public decimal PesoCubado { get; set; }
        public decimal PesoPaletizado { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade> Quantidades { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem> TiposEmbalagem { get; set; }
        public decimal Distancia { get; set; }
        public decimal NumeroEntregas { get; set; }
        public decimal NumeroPacotes { get; set; }
        public decimal NumeroPallets { get; set; }
        public decimal NumeroDeslocamento { get; set; }
        public decimal NumeroDiarias { get; set; }
        public decimal NumeroAjudantes { get; set; }
        public bool NecessarioAjudante { get; set; }
        public DateTime? DataColeta { get; set; }
        public DateTime? DataInicialViagem { get; set; }
        public DateTime? DataFinalViagem { get; set; }
        public int Minutos { get; set; }
        public int DeducaoMinutos { get; set; }
        public bool RementesEDestinatariosOpcionaisQuandoExistirLocalidade { get; set; }

        public int TotalMinutos
        {
            get
            {
                if (DeducaoMinutos > Minutos)
                    return 0;

                return Minutos - DeducaoMinutos;
            }
        }

        public bool EscoltaArmada { get; set; }
        public int QuantidadeEscolta { get; set; }
        public bool GerenciamentoRisco { get; set; }
        public bool NecessarioReentrega { get; set; }
        public bool Rastreado { get; set; }
        public bool PossuiRestricaoTrafego { get; set; }
        public bool DespachoTransitoAduaneiro { get; set; }
        public List<Dominio.Entidades.ModeloDocumentoFiscal> ModelosUtilizadosEmissao { get; set; }
        public Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int> QuantidadeEmissoesPorModeloDocumento { get; set; }
        public int QuantidadeNotasFiscais { get; set; }
        public bool Desistencia { get; set; }
        public decimal PercentualDesistencia { get; set; }
        public decimal PercentualFixoAdValorem { get; set; }
        public List<int> CodigosRotasFixas { get; set; }
        public List<Dominio.Entidades.RotaFrete> RotasDinamicas { get; set; }
        public bool PagamentoTerceiro { get; set; }
        public ParametrosCalculoFrete ParametrosCarga { get; set; }
        public int NumeroPedidos { get; set; }
        public DateTime? DataBaseCRT { get; set; }
        public Dominio.Entidades.Embarcador.Rateio.RateioFormula FormulaRateio { get; set; }
        public Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }
        public Dominio.Entidades.Embarcador.Pedidos.CanalVenda CanalVenda { get; set; }
        public bool CalcularFretePorPesoCubado { get; set; }
        public bool AplicarMaiorValorEntrePesoEPesoCubado { get; set; }
        public decimal IsencaoCubagem { get; set; }
        public decimal Cubagem { get; set; }
        public decimal MaiorAlturaProdutoEmCentimetros { get; set; }
        public decimal MaiorLarguraProdutoEmCentimetros { get; set; }
        public decimal MaiorComprimentoProdutoEmCentimetros { get; set; }
        public decimal MaiorVolumeProdutoEmCentimetros { get; set; }
        public List<Dominio.Entidades.Cliente> Fronteiras { get; set; }
        public bool NaoValidarTransportador { get; set; }
        public bool FreteTerceiro { get; set; }
        public bool CargaPerigosa { get; set; }
        public bool CargaInternacional { get; set; }
        public DateTime? DataPrevisaoEntrega { get; set; }
        public bool CalcularVariacoes { get; set; }
        public bool MultiplicarPeloResultadoDaDistancia { get; set; }
        public bool MultiplicarValorFixoFaixaDistanciaPeloPesoCarga { get; set; }
    }
}
