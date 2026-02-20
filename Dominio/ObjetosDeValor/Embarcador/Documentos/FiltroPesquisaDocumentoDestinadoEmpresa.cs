using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public sealed class FiltroPesquisaDocumentoDestinadoEmpresa
    {
        public bool? Cancelado { get; set; }
        public string Chave { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CpfCnpjFornecedor { get; set; }
        public DateTime? DataAutorizacaoInicial { get; set; }
        public DateTime? DataAutorizacaoFinal { get; set; }
        public DateTime? DataEmissaoInicial { get; set; }
        public DateTime? DataEmissaoFinal { get; set; }
        public string NomeFornecedor { get; set; }
        public int NumeroDe { get; set; }
        public int NumeroAte { get; set; }
        public bool? PossuiDocumentoEntrada { get; set; }
        public int Serie { get; set; }
        public List<TipoDocumentoDestinadoEmpresa> TiposDocumento { get; set; }
        public List<SituacaoManifestacaoDestinatario> SituacaoManifestacaoDestinatario { get; set; }
        public TipoOperacaoNFe? TipoOperacao { get; set; }
        public SituacaoIntegracao? SituacaoIntegracao { get; set; }
    }
}
