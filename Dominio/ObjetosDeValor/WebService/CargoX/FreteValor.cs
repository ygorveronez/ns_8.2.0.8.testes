using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.CargoX
{
    public class FreteValor
    {
        public List<ComponenteAdicional> ComponentesAdicionais { get; set; }

        public decimal Frete { get; set; }
        public decimal ValorPrestacaoServico { get; set; }
    }
}
