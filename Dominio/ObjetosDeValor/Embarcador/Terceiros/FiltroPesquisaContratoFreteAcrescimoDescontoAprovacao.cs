using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Terceiros
{
    public class FiltroPesquisaContratoFreteAcrescimoDescontoAprovacao
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataLimite { get; set; }
        public int NumeroContrato { get; set; }
        public SituacaoContratoFreteAcrescimoDesconto Situacao { get; set; }
        public int CodigoJustificativa { get; set; }
        public int CodigoUsuario { get; set; }
    }
}
