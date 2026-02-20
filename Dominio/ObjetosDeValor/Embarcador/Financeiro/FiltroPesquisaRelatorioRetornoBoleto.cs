using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRelatorioRetornoBoleto
    {
        public int CodigoEmpresa { get; set; }
        public int BoletoConfiguracao { get; set; }
        public int BoletoComando { get; set; }
        public DateTime DataInicialImportacao { get; set; }
        public DateTime DataFinalImportacao { get; set; }
        public DateTime DataInicialOcorrencia { get; set; }
        public DateTime DataFinalOcorrencia { get; set; }
    }
}
