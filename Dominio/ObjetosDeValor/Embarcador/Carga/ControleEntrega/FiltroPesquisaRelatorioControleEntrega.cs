using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public class FiltroPesquisaRelatorioControleEntrega
    {
        public DateTime DataOcorrenciaInicial { get; set; }
        public DateTime DataOcorrenciaFinal { get; set; }
        public List<int> CodigosTipoOcorrencia { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public int NumeroCTe { get; set; }
        public List<int> CodigosVeiculos { get; set; }
        public List<int> CodigosMotoristas { get; set; }
        public DateTime DataPrevisaoEntregaInicial { get; set; }
        public DateTime DataPrevisaoEntregaFinal { get; set; }
        public List<string> UFsOrigem { get; set; }
        public List<string> UFsDestino { get; set; }
    }
}
