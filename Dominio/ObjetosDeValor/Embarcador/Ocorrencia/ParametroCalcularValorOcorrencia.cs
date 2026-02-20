using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class ParametroCalcularValorOcorrencia
    {
        public bool ApenasReboque { get; set; }

        public int CodigoCarga { get; set; }

        public int CodigoParametroBooleano { get; set; }

        public int CodigoParametroData { get; set; }

        public int CodigoParametroInteiro { get; set; }

        public int CodigoParametroPeriodo { get; set; }

        public int CodigoTipoOcorrencia { get; set; }
        public int QuantidadeAjudantes { get; set; }

        public DateTime DataFim { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime ParametroData { get; set; }

        public int Minutos { get; set; }

        public int DeducaoHoras { get; set; }

        public double FronteiraOUParqueamento { get; set; }

        public int HorasSemFranquia { get; set; } = 5;

        public int KmInformado { get; set; }

        public List<Entidades.Embarcador.Cargas.CargaCTe> ListaCargaCTe { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> CargaEntregaNotaFiscals { get; set; }

        public bool PermiteInformarValor { get; set; }

        public bool DevolucaoParcial { get; set; }

        public decimal ValorOcorrencia { get; set; }

        public bool VerificarDeducaoHorasMaiorQueTempoTotal()
        {
            return (Minutos - DeducaoHoras * 60) <= 0;
        }

        public LocalFreeTime? LocalFreeTime { get; set; }

        public int? FreeTime { get; set; }
    }
}
