using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class EntregaSimplificado
    {
        public Dominio.ObjetosDeValor.Localidade Origem { get; set; }
        public Dominio.ObjetosDeValor.Localidade Destino { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorPrestacaoServico { get; set; }
        public decimal ValorAReceber { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe> NFes { get; set; }
        public List<DocumentoAnterior> DocumentosAnteriores { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> ComponentesAdicionais { get; set; }
    }
}
