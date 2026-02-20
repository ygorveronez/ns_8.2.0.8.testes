using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class CargaDadosTransporte
    {
        #region Propriedades

        public Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public List<int> CodigosApoliceSeguro { get; set; }

        public int CodigoEmpresa { get; set; }

        public int CodigoModeloVeicular { get; set; }

        public int CodigoMotorista
        {
            get { return ListaCodigoMotorista.Count > 0 ? ListaCodigoMotorista[0] : 0; }
            set { if (value > 0) ListaCodigoMotorista.Add(value); }
        }

        public int CodigoPedidoViagemNavio { get; set; }

        public int CodigoPortoDestino { get; set; }

        public int CodigoPortoOrigem { get; set; }

        public int CodigoReboque { get; set; }

        public int CodigoSegundoReboque { get; set; }

        public int CodigoTerceiroReboque { get; set; }

        public int CodigoTerminalDestino { get; set; }

        public int CodigoTerminalOrigem { get; set; }

        public int CodigoTipoCarga { get; set; }

        public int CodigoTipoOperacao { get; set; }
        public int CodigoSetor { get; set; }

        public int CodigoTracao { get; set; }

        public DateTime? DataRetiradaCtrnVeiculo { get; set; }

        public DateTime? DataRetiradaCtrnReboque { get; set; }

        public DateTime? DataRetiradaCtrnSegundoReboque { get; set; }
        public DateTime? DataRetiradaCtrnTerceiroReboque { get; set; }

        public DateTime? InicioCarregamento { get; set; }

        public List<int> ListaCodigoMotorista { get; set; }
        public List<int> ListaCodigoAjudante { get; set; }

        public string NumeroContainerReboque { get; set; }

        public string NumeroContainerSegundoReboque { get; set; }
        public string NumeroContainerTerceiroReboque { get; set; }

        public string NumeroContainerVeiculo { get; set; }

        public string GensetReboque { get; set; }

        public string GensetSegundoReboque { get; set; }
        public string GensetTerceiroReboque { get; set; }

        public string GensetVeiculo { get; set; }

        public int MaxGrossReboque { get; set; }

        public int MaxGrossSegundoReboque { get; set; }
        public int MaxGrossTerceiroReboque { get; set; }

        public int MaxGrossVeiculo { get; set; }

        public int TaraContainerReboque { get; set; }

        public int TaraContainerSegundoReboque { get; set; }

        public int TaraContainerTerceiroReboque { get; set; }

        public int TaraContainerVeiculo { get; set; }

        public DateTime? TerminoCarregamento { get; set; }

        public string NumeroPager { get; set; }

        public bool SalvarDadosTransporteSemSolicitarNFes { get; set; }

        public string ObservacaoTransportador { get; set; }

        public string ProtocoloIntegracaoGR { get; set; }

        public int CodigoTipoContainer { get; set; }

        public DateTime? DataBaseCRT { get; set; }

        public int QuantidadePaletes { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckin? TipoCheckin { get; set; }

        public DateTime? DataCheckout { get; set; }

        public bool CargaEstaEmParqueamento { get; set; }

        public int CodigoTipoCarregamento { get; set; }

        public int CodigoCentroResultado { get; set; }

        public int CodigoNavio { get; set; }
        public int CodigoBalsa { get; set; }
        public int CodigoJustificativaAutorizacaoCarga { get; set; }
        public bool LiberadaComCargaSemPlanejamento { get; set; }

        public int CodigoTransportadorSubcontratado { get; set; }

        public int Container { get; set; }
        
        #endregion

        #region Contrutores

        public CargaDadosTransporte()
        {
            ListaCodigoMotorista = new List<int>();
            NumeroContainerReboque = null;
            NumeroContainerVeiculo = null;
            NumeroContainerSegundoReboque = null;
            NumeroContainerTerceiroReboque = null;
            GensetReboque = null;
            GensetVeiculo = null;
            GensetSegundoReboque = null;
            GensetTerceiroReboque = null;
        }

        #endregion
    }
}
