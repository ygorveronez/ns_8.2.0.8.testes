using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class FiltroPesquisaRelatorioTemposGestaoPatio
    {
        public DateTime DataInicioCarregamento { get; set; }
        public DateTime DataFimCarregamento { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoFilial { get; set; }
        public List<int> CodigosFilial { get; set; }
        public SituacaoEtapaFluxoGestaoPatio? Situacao { get; set; }
        public EtapaFluxoGestaoPatio EtapaFluxoGestaoPatio { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public int CodigoTipoCarga { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public bool ListarCargasCanceladas { get; set; }
        public int CodigoRota { get; set; }
    }
}
