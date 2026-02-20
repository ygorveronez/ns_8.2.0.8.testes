using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto
    {
        public double CpfCnpjTerceiro { get; set; }
        public DateTime DataEmissaoContratoInicial { get; set; }
        public DateTime DataEmissaoContratoFinal { get; set; }
        public int Veiculo { get; set; }
        public int NumeroContrato { get; set; }
        public string NumeroCarga { get; set; }
        public List<SituacaoContratoFrete> Situacao { get; set; }
    }
}
