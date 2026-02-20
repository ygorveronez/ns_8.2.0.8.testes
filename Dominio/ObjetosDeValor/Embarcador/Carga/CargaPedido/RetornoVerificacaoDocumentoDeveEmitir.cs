namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class RetornoVerificacaoDocumentoDeveEmitir
    {
        public bool PossuiCTe { get; set; }
        public bool PossuiNFS { get; set; }
        public bool PossuiNFSManual { get; set; }
        public bool SempreDisponibilizarDocumentoNFSManual { get; set; }
        public Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalIntramunicipal { get; set; }
    }
}
