using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public class RetornoConsultaTendenciasEntrega
    {
        public int CodCargaEntrega { get; set; }
        public int CodCarga { get; set; }
        public string CodCargaEmbarcador { get; set; }
        public string CPF_CNPJ { get; set; }
        public DateTime DataEntregaPrevista { get; set; }
        public DateTime DataEntregaReprogramada { get; set; }
        public DateTime DataAgendamentoEntrega { get; set; }
        public int TempoDiferenca { get; set; }

    }
}
