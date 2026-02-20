using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class FiltroPesquisaCIOT
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public double CpfCnpjTransportador { get; set; }
        public List<TipoProprietarioVeiculo> TiposTransportador { get; set; }
        public SituacaoCIOT? Situacao { get; set; }
        public List<SituacaoCIOT> Situacoes { get; set; }
        public string NumeroCarga { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoMotorista { get; set; }
        public string Numero { get; set; }
        public OperadoraCIOT? Operadora { get; set; }
        public string CodigoVerificador { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<int> CodigosCIOTs { get; set; }
    }
}
