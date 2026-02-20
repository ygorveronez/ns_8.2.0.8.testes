namespace Dominio.ObjetosDeValor.NFSe
{
    public class DocumentoRetornoPDFZeus
    {
        public bool Sucesso { get; set; }
        public string ChaveDocumento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus TipoRetorno { get; set; }
        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }
    }
}
