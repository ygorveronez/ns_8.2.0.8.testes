using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FiltroPesquisaContratoFreteTransportador
    {
        public string NumeroContrato { get; set; }
        public List<int> CodigoTransportador { get; set; }
        public List<int> TipoContratoFrete { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Status { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador? Situacao { get; set; }
        public string Placa { get; set; }
        public int CodigoStatusAceiteContrato { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }
    }
}
