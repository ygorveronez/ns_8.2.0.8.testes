namespace Dominio.ObjetosDeValor.WebService.Atendimento
{
    public class AdicionarAtendimento
    {
        public Carga Carga { get; set; }

        public NotaFiscal NotaFiscal { get; set; }

        public MotivoAtendimento Motivo { get; set; }

        public string Observacao { get; set; }
    }
}
