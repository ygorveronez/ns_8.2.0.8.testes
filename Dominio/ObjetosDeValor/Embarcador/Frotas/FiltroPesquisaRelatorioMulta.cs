using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public class FiltroPesquisaRelatorioMulta
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataVencimentoInicial { get; set; }
        public DateTime DataVencimentoFinal { get; set; }
        public DateTime DataVencimentoInicialPagar { get; set; }
        public DateTime DataVencimentoFinalPagar { get; set; }
        public DateTime DataLimiteInicial { get; set; }
        public DateTime DataLimiteFinal { get; set; }
        public int CodigoVeiculo { get; set; }
        public int NumeroMulta { get; set; }
        public int CodigoCidade { get; set; }
        public int CodigoTipoInfracao { get; set; }
        public List<int> CodigosTipoInfracoes { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoTitulo { get; set; }
        public string TipoTipoInfracao { get; set; }
        public string NivelInfracao { get; set; }
        public string NumeroAtuacao { get; set; }
        public string TipoOcorrenciaInfracao { get; set; }
        public double CnpjPessoa { get; set; }
        public double CnpjFornecedorPagar { get; set; }
        public ResponsavelPagamentoInfracao PagoPor { get; set; }
        public SituacaoInfracao StatusMulta { get; set; }
        public DateTime DataLancamentoInicial { get; set; }
        public DateTime DataLancamentoFinal { get; set; }
        public TipoMotorista TipoMotorista { get; set; }
        public DateTime DataInicialEmissaoInfracao { get; set; }
        public DateTime DataFinalEmissaoInfracao { get; set; }
    }
}
