using Dominio.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaRelatorioFreteTerceirizado
    {
        public List<double> CpfCnpjTerceiros { get; set; }
        public DateTime DataEmissaoContratoInicial { get; set; }
        public DateTime DataEmissaoContratoFinal { get; set; }
        public DateTime DataAprovacaoInicial { get; set; }
        public DateTime DataAprovacaoFinal { get; set; }
        public DateTime DataEncerramentoInicial { get; set; }
        public DateTime DataEncerramentoFinal { get; set; }
        public DateTime DataEncerramentoCIOTInicial { get; set; }
        public DateTime DataEncerramentoCIOTFinal { get; set; }
        public DateTime DataAberturaCIOTInicial { get; set; }
        public DateTime DataAberturaCIOTFinal { get; set; }
        public int Veiculo { get; set; }
        public int NumeroContrato { get; set; }
        public int NumeroCTe { get; set; }
        public int ModeloVeicular { get; set; }
        public int TipoOperacao { get; set; }
        public int Empresa { get; set; }
        public string NumeroCarga { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete> Situacao { get; set; }
        public TiposCargaTerceiros TiposCargaTerceiros { get; set; }
        public string StatusCTe { get; set; }
        public string NumeroCIOT { get; set; }
        public List<TipoCTE> TipoCTe { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo? TipoProprietario { get; set; }
    }
}
