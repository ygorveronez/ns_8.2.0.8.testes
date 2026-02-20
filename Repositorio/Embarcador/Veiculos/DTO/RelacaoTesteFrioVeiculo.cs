using System;

namespace Repositorio.Embarcador.Veiculos
{
    public class RelacaoTesteFrioVeiculo
    {
        public int CodigoVeiculo { get; set; }
        public DateTime? Vencimento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca Status { get; set; }
        
    }
}
