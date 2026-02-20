namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.DocumentoEntrada
{
    public class DocumentoCancelamentoEnvio
    {
        public string CodigoDocumento { get; set; }
        public string Usuario { get; set; }
        public bool ExcluirDocumentoFiscal { get; set; }
    }
}
