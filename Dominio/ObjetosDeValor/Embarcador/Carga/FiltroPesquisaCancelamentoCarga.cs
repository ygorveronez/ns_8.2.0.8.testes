using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaCancelamentoCarga
    {
        public List<TipoPropostaMultimodal> TiposPropostasMultimodal { get; set; }
        public string NumeroBooking { get; set; }
        public int CodigoPedidoViagemDirecao { get; set; }
        public int CodigoTerminalOrigem { get; set; }
        public int CodigoTerminalDestino { get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroCTe { get; set; }
        public int CodigoOperador { get; set; }
        public double CpfCnpjPessoa { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public SituacaoCancelamentoCarga? Situacao { get; set; }
        public TipoCancelamentoCarga? Tipo { get; set; }
        public TipoCancelamentoCargaDocumento? TipoCancelamentoCargaDocumento { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosEmpresas { get; set; }
        public List<double> CodigosRecebedores { get; set; }
        public bool CargasLiberadasParaCancelamentoComRejeicaoIntegracao { get; set; }  

    }
}
