namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public sealed class ItemNaoConformidadeParticipantes
    {
        public int Codigo { get; set; }

        public int CodigoItemNaoConformidade { get; set; }

        public Enumeradores.TipoParticipante Participante { get; set; }
    }
}
