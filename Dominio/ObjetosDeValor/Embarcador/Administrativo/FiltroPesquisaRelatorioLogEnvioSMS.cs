using System;

namespace Dominio.ObjetosDeValor.Embarcador.Administrativo
{
    public sealed class FiltroPesquisaRelatorioLogEnvioSMS
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public double CodigoPessoa { get; set; }
        public int NumeroNotaInicial { get; set; }
        public int NumeroNotaFinal { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
