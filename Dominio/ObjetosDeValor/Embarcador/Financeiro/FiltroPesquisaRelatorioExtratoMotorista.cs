using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRelatorioExtratoMotorista
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int CodigoTipoMovimento { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoPlanoConta { get; set; }
        public int CodigoEmpresa { get; set; }
        public int Codigo { get; set; }
    }
}
