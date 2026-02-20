using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class FiltroPesquisaRelatorioNFe
    {
        public List<int> CodigosGrupoPessoas { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public List<int> CodigosCarga { get; set; }
        public List<int> CodigosOrigem { get; set; }
        public List<int> CodigosDestino { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<double> CpfCnpjsRemetente { get; set; }
        public List<double> CpfCnpjsDestinatario { get; set; }
        public List<double> CpfCnpjsExpedidor { get; set; }
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public DateTime DataInicialEmissaoCTe { get; set; }
        public DateTime DataFinalEmissaoCTe { get; set; }
        public DateTime DataInicialEmissaoCarga { get; set; }
        public DateTime DataFinalEmissaoCarga { get; set; }
        public DateTime DataInicialPrevisaoEntregaPedido { get; set; }
        public DateTime DataFinalPrevisaoEntregaPedido { get; set; }
        public DateTime DataInicialInicioViagemPlanejada { get; set; }
        public DateTime DataFinalInicioViagemPlanejada { get; set; }
        public SituacaoFatura? SituacaoFatura { get; set; }
        public List<string> EstadosOrigem { get; set; }
        public List<string> EstadosDestino { get; set; }
        public TipoLocalPrestacao TipoLocalPrestacao { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public List<int> CodigosRestricoes { get; set; }
        public List<int> CodigosNotasFiscais { get; set; }
        public List<int> CodigosMotorista { get; set; }
        public List<Dominio.Enumeradores.TipoCTE> TiposCTe { get; set; }
        public int QuantidadeVolumesInicial { get; set; }
        public int QuantidadeVolumesFinal { get; set; }
        public ClassificacaoNFe ClassificacaoNFe { get; set; }
        public bool? NotasFiscaisSemCarga { get; set; }
        public bool? CargaTransbordo { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public List<SituacaoCarga> Situacoes { get; set; }
        public List<SituacaoCargaMercante> SituacoesCargaMercante { get; set; }
        public List<SituacaoEntrega> SituacoesEntrega { get; set; }
        public int NumeroCanhoto { get; set; }
        public SituacaoEntrega? StatusEntrega { get; set; }
        public string FiltroDinamico { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public int PossuiExpedidor { get; set; }
        public int PossuiRecebedor { get; set; }
        public DateTime DataPrevisaoCargaEntregaInicial { get; set; }
        public DateTime DataPrevisaoCargaEntregaFinal { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
        public double CpfCnpjClienteFornecedor { get; set; }
        public int CodigoGrupoPessoaClienteFornecedor { get; set; }

    }
}
