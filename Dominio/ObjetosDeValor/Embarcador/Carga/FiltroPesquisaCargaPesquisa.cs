using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaPesquisa
    {
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> TiposPropostasMultimodal { get; set; }

        public int NumeroMDFe { get; set; }

        public DateTime? DataInicialEmissao { get; set; }

        public DateTime? DataFinalEmissao { get; set; }

        public DateTime? DataInicialCarga { get; set; }

        public DateTime? DataFinalCarga { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public List<int> CodigosEmpresa { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<int> CodigosFilialVenda { get; set; }

        public int CodigoGrupoPessoas { get; set; }

        public int CodigoVeiculo { get; set; }

        public int CodigoMotorista { get; set; }

        public string NumeroFrota { get; set; }

        public int NumeroNF { get; set; }

        public int NumeroCTe { get; set; }

        public List<Enumeradores.SituacaoCarga> Situacao { get; set; }

        public List<Enumeradores.TipoIntegracao> TipoIntegracao { get; set; }

        public bool? PossuiDTNatura { get; set; }

        public double ProprietarioVeiculo { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public bool CargasNaoAgrupadas { get; set; }

        public bool CargasAguardandoImportacaoPreCte { get; set; }

        public double CpfCnpjRemetente { get; set; }

        public double CpfCnpjExpedidor { get; set; }

        public double CpfCnpjRecebedor { get; set; }

        public List<double> CodigosRecebedores { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public int CodigoOrigem { get; set; }

        public int CodigoDestino { get; set; }

        public int NumeroCTeSubcontratacao { get; set; }

        public bool? CargaTransbordo { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public bool CargasNaoFechadas { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

        public bool CargasDisponiveisParaJanela { get; set; }

        public int CodigoTipoOcorrencia { get; set; }

        public List<int> CodigosEmpresas { get; set; }

        public bool? SomenteCargasDeRedespacho { get; set; }

        public string NumeroContainer { get; set; }

        public bool NaoRetornarSubCarga { get; set; }

        public long CodigoClienteTerceiro { get; set; }

        public bool NaoExibirCargasCanceladas { get; set; }

        public List<Enumeradores.SituacaoCarga> SituacaoDiferente { get; set; }

        public double CpfCnpjFornecedor { get; set; }

        public bool SomenteNaoFechadas { get; set; }

        public string NumeroPedidoCliente { get; set; }

        public bool TelaRedespacho { get; set; }

        #region Propriedades com Regras

        public bool IsParametrosInformados()
        {
            return !string.IsNullOrWhiteSpace(CodigoCargaEmbarcador) || NumeroCTe > 0 || NumeroMDFe > 0 || NumeroNF > 0 || DataInicialEmissao.HasValue || DataFinalEmissao.HasValue;
        }

        #endregion
    }
}
