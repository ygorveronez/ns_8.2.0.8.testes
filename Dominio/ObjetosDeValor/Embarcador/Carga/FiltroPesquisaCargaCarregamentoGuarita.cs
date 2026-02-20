using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaCargaCarregamentoGuarita
    {
        public List<int> CodigosCentrosCarregamento { get; set; }

        public DateTime? DataInicialCarregamento { get; set; }

        public DateTime? DataFinalCarregamento { get; set; }

        public DateTime? DataInicialChegada { get; set; }

        public DateTime? DataFinalChegada { get; set; }

        public SituacaoCargaGuarita? Situacao { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoVeiculo { get; set; }

        public int CodigoMotorista { get; set; }

        public List<int> CodigosTransportadores { get; set; }

        public List<int> CodigosVeiculos { get; set; }

        public List<int> CodigosMotoristas { get; set; }

        public int CentroCarregamento { get; set; }

        public DateTime? DataAgendada { get; set; }

        public List<int> CodigosTipoOperacoes { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public string NumeroCarga { get; set; }

        public List<int> ListaCodigoCarga { get; set; }

        public List<int> CodigosFiliais { get; set; }

        public double CpfCnpjDestinatario { get; set; }
    }
}
