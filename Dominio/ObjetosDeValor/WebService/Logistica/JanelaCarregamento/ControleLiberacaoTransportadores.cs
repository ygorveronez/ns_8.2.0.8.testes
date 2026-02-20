using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Logistica.JanelaCarregamento
{
    public class ControleLiberacaoTransportadores
	{
		public string NumeroShipment { get; set; }
		public List<CargaLiberacaoTransportador> Cargas { get; set; }
		public Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores? Acao { get; set; }
		public bool Divisoria { get; set; }
		public bool ContemCargaPerigosa { get; set; }
		public decimal CustoPlanejado { get; set; }
		public decimal CustoAtual { get; set; }
		public DateTime DataInicio { get; set; }
		public string Razao { get; set; }
		public string Observacao { get; set; }
	}
}
