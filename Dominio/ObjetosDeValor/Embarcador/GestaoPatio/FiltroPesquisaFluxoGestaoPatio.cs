using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class FiltroPesquisaFluxoGestaoPatio
    {
        public List<int> CodigosAreaVeiculo { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<int> CodigosLocalCarregamento { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }
        public List<int> CodigosTransportador { get; set; }

        public int CodigoMotorista { get; set; }
        public int CodigoTipoCarregamento { get; set; }
        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoTransportador { get; set; }

        public List<double> CpfCnpjDestinatario { get; set; }

        public List<double> CpfCnpjRemetente { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataInicial { get; set; }

        public List<EtapaFluxoGestaoPatio> EtapaFluxoGestaoPatio { get; set; }

        public bool ListarCargasCanceladas { get; set; }

        public string NumeroCarga { get; set; }

        public int NumeroNotaFiscal { get; set; }

        public int NumeroNfProdutor { get; set; }

        public string Pedido { get; set; }

        public string Placa { get; set; }

        public string PreCarga { get; set; }

        public SituacaoEtapaFluxoGestaoPatio? Situacao { get; set; }

        public bool SomenteFluxosAbertos { get; set; }

        public TipoFluxoGestaoPatio? Tipo { get; set; }

        public DateTime? DataFinalChegadaVeiculo { get; set; }

        public DateTime? DataInicialChegadaVeiculo { get; set; }
    }
}
