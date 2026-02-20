using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual
    {
        public double CpfCnpjRemetente { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public double CpfCnpjTomador { get; set; }

        public int CodigoCarga { get; set; }

        public int CodigoEmpresa { get; set; }

        public List<int> CodigosFiliais { get; set; }

        public List<double> CodigosRecebedores { get; set; }

        public int CodigoGrupoPessoas { get; set; }

        public int CodigoLocalidadePrestacao { get; set; }

        public DateTime? DataEmissaoInicial { get; set; }

        public DateTime? DataEmissaoFinal { get; set; }

        public DateTime? DataEmissaoNFSInicial { get; set; }

        public DateTime? DataEmissaoNFSFinal { get; set; }

        public string EstadoLocalidadePrestacao { get; set; }

        public int NumeroInicial { get; set; }

        public int NumeroFinal { get; set; }

        public int NumeroInicialNFS { get; set; }

        public int NumeroFinalNFS { get; set; }

        public bool? PossuiNFSGerada { get; set; }

        public List<Enumeradores.SituacaoLancamentoNFSManual> SituacaoNFS { get; set; }

        public string NumeroPedidoCliente { get; set; }

        public int CodigoTipoOperacao { get; set; }
        public int CodigoTransportador { get; set; }
    }
}
