using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class OrdemServicoFrota
    {
        public virtual int Numero { get; set; }
        public virtual DateTime? DataCriacao { get; set; }
        public virtual string Observacao { get; set; }
        public virtual string CondicaoPagamento { get; set; }
        public virtual string Motivo { get; set; }

    }
}
