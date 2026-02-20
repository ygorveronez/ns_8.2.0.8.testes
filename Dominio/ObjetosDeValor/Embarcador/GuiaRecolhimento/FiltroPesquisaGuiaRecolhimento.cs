using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GuiaRecolhimento
{
    public sealed class FiltroPesquisaGuiaRecolhimento
    {
        public int Codigotransportador { get; set; }

        public List<SituacaoGuia> Status { get; set; }

        public DateTime DataEmissaoInicial { get; set; }

        public DateTime DataEmissaoFinal { get; set; }

        public string NumeroCarga { get; set; }

        public string CPFMotorista { get; set; }

        public int NumeroCte { get; set; }

        public int SerieCte { get; set; }

        public string ChaveCte { get; set; }

        public int CodigoVeiculo { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }
    }
}
