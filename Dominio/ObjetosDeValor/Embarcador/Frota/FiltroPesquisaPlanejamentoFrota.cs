using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroPesquisaPlanejamentoFrota
    {
        #region Propriedades

        public DateTime DataConsultaVigencia { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoMotorista { get; set; }
        public Enumeradores.SituacaoFrota? SituacaoDaFrota { get; set; }
        public Enumeradores.SituacaoDoConjuntoFrota? SituacaoDoConjunto { get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino { get; set; }
        public bool VeiculoNecessitaManutencao { get; set; }
        public bool VeiculoComCarga { get; set; }
        public bool MotoristaNecessitaIrCasa { get; set; }


        //FILTROS USADOS PARA BUSCA DE FROTAS POR PRIORIDADE (QUANDO SELECIONADO A CARGA)
        public decimal latitudeOrigem { get; set; }
        public decimal longitudeOrigem { get; set; }
        public int CodigoFrota { get; set; }


        #endregion
    }
}
