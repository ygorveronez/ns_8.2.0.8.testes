using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class FiltroPesquisaOcorrencia
    {
        #region Propriedades

        public string CnpjTransportador { get; set; }

        public int Codigo { get; set; }

        public string CodigoCarga { get; set; }

        public int CodigoChamado { get; set; }

        public int CodigoCteComplementar { get; set; }

        public int CodigoCteOrigem { get; set; }

        public int CodigoLoteAvaria { get; set; }

        public List<int> CodigosEmpresa { get; set; }

        public List<int> CodigosFilial { get; set; }
        public List<double> CodigosRecebedor { get; set; }

        public List<int> CodigosFilialVenda { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public int CodigoGrupoPessoa { get; set; }

        public int CodigoGrupoPessoasTomadorCteComplementar { get; set; }

        public int CodigoGrupoPessoasTomadorCteOriginal { get; set; }

        public List<int> CodigosTipoOcorrencia { get; set; }
        public List<int> CodigosGrupoOcorrencia { get; set; }

        public int CodigoUsuario { get; set; }

        public double CpfCnpjPessoa { get; set; }

        public double CpfCnpjTomadorCTeComplementar { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataInicialAprovacao { get; set; }

        public DateTime? DataInicialEmissaoDocumento { get; set; }

        public DateTime? DataLimite { get; set; }

        public DateTime? DataLimiteAprovacao { get; set; }

        public DateTime? DataLimiteEmissaoDocumento { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

        public int NumeroNFe { get; set; }

        public int CodigoNotaFiscal { get; set; }

        public int NumeroOcorrencia { get; set; }

        public int NumeroAtendimento { get; set; }

        public string NumeroOcorrenciaCliente { get; set; }

        public string NumeroPedido { get; set; }

        public string NumeroOS { get; set; }

        public string NumeroBooking { get; set; }

        public string NumeroControle { get; set; }

        public int NumeroDocumentoOriginario { get; set; }

        public bool OcultarOcorrenciasAutomaticas { get; set; }

        public string ObservacaoCTe { get; set; }

        public List<Enumeradores.SituacaoOcorrencia> Situacoes { get; set; }

        public Enumeradores.TipoDocumentoCreditoDebito TipoDocumentoCreditoDebito { get; set; }

        public Enumeradores.TipoPessoa? TipoPessoa { get; set; }

        public List<int> CodigosCentroResultado { get; set; }

        public int CodigoMotorista { get; set; }

        public double CpfCnpjTomadorCTeOriginal { get; set; }

        public string CnpjTransportadorExterior { get; set; }

        public bool? AguardandoImportacaoCTe { get; set; }

        public int CodigoTiposCausadoresOcorrencia { get; set; }

        public int CodigoCausasTipoOcorrencia { get; set; }

        public string NumeroPedidoCliente { get; set; }

        public List<int> CodigosClienteComplementar { get; set; }

        public List<int> CodigosVendedor { get; set; }

        public List<int> CodigosSupervisor { get; set; }

        public List<int> CodigosGerente { get; set; }

        public List<string> CodigosUFDestino { get; set; }

        public int NumeroNF { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public int CodigoEmpresa
        {
            set
            {
                if (value > 0)
                    CodigosEmpresa = new List<int>() { value };
            }
        }

        public int CodigoTipoOcorrencia
        {
            set
            {
                if (value > 0)
                    CodigosTipoOcorrencia = new List<int>() { value };
            }
        }

        public Enumeradores.SituacaoOcorrencia Situacao
        {
            set
            {
                if (value != Enumeradores.SituacaoOcorrencia.Todas)
                    Situacoes = new List<Enumeradores.SituacaoOcorrencia>() { value };
            }
        }

        #endregion Propriedades com Regras
    }
}
