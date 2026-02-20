namespace Dominio.ObjetosDeValor.WebService.NFS
{
    public class DocumentoPendenteNotaManual
    {
        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal NFe { get; set; }
        public Dominio.ObjetosDeValor.WebService.CTe.CTe CTeAnterior { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Tomador { get; set; }
        public Dominio.ObjetosDeValor.Localidade LocalidadePrestacao { get; set; }
        public decimal ValorFrete { get; set; }
        public int ProtocoloDocumento { get; set; }
    }
}
