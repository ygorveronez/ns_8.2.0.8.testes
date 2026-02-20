using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class IntegrarViagem
    {
        public string Token { get; set; }
        public int? IdViagem { get; set; }
        public string CNPJAplicacao { get; set; }
        public string CNPJEmpresa { get; set; }
        public string CPFCNPJClienteDestino { get; set; }
        public string CPFCNPJClienteOrigem { get; set; }
        public string CPFCNPJClienteTomador { get; set; }
        public string CPFCNPJProprietario { get; set; }
        public string NomeProprietario { get; set; }
        public string RNTRC { get; set; }
        public string CNPJFilial { get; set; }
        public string RazaoSocialFilial { get; set; }
        public string CPFMotorista { get; set; }
        public int? IdCarga { get; set; }
        public string Placa { get; set; }
        public string DataColeta { get; set; }
        public string DataPrevisaoEntrega { get; set; }
        public int StatusViagem { get; set; }
        public string StatusIntegracao { get; set; }
        public List<string> Carretas { get; set; }
        public string DocumentoCliente { get; set; }
        public decimal? PesoSaida { get; set; }
        public decimal? ValorMercadoria { get; set; }
        public decimal? ValorFrete { get; set; }
        public decimal? IRRPF { get; set; }
        public decimal? INSS { get; set; }
        public decimal? SESTSENAT { get; set; }
        public string NumeroDocumento { get; set; }
        public string DataEmissao { get; set; }
        public string Produto { get; set; }
        public string Unidade { get; set; }
        public decimal? Quantidade { get; set; }
        public string Coleta { get; set; }
        public string Entrega { get; set; }
        public bool HabilitarDeclaracaoCiot { get; set; }
        public int? NaturezaCarga { get; set; }
        public string NumeroControle { get; set; }
        public bool ForcarCiotNaoEquiparado { get; set; }
        public string CEPOrigem { get; set; }
        public string CEPDestino { get; set; }
        public int? CodigoTipoCarga { get; set; }
        public int? DistanciaViagem { get; set; }
        public List<CarretaViagem> CarretasViagemV2 { get; set; }
        public DadoPagamento DadosPagamento { get; set; }
        public DadoANTT DadosANTT { get; set; }
        public List<ViagemRegra> ViagemRegra { get; set; }
        public List<ViagemEstabelecimento> ViagemEstabelecimentos { get; set; }
        public List<ViagemEvento> ViagemEventos { get; set; }
        public Pedagio Pedagio { get; set; }
        public List<DocumentoFiscal> DocumentosFiscais { get; set; }
    }
}
