using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class SituacaoColaboradorIntegracaoWS
    {
        public string Descricao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.Motorista Colaborador { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.SituacaoColaboradorIntegracao SituacaoColaborador { get; set; }
    }
}
