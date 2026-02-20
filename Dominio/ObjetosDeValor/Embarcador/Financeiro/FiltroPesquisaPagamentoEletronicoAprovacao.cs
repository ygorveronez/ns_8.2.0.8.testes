using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaPagamentoEletronicoAprovacao
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataLimite { get; set; }
        public int Numero { get; set; }
        public SituacaoAutorizacaoPagamentoEletronico? Situacao { get; set; }        
        public int CodigoBoletoConfiguracao { get; set; }
        public int CodigoUsuario { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
