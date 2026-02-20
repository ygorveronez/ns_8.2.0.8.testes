using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Terceiros
{
    public class FiltroPesquisaContratoFreteAcrescimoDesconto
    {
        public int ContratoFrete { get; set; }
        public int Justificativa { get; set; }
        public SituacaoContratoFreteAcrescimoDesconto Situacao { get; set; }
        public List<int> CodigosCarga { get; set; }
        public List<int> CodigosCIOT { get; set; }
        public List<double> NomeSubcontratado { get; set; }
        public List<string> NomeMotorista { get; set; }
        public TipoJustificativa TipoJustificativa { get; set; }
        public double CodigoTransportadorContratoFreteOrigem { get; set; }
    }
}
