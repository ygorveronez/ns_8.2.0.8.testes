using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FiltroPesquisaSolicitacaoLicitacao
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public SituacaoSolicitacaoLicitacao Situacao { get; set; }
        public int CodigoFuncionarioSolicitante { get; set; }
        public int CodigoFuncionarioCotacao { get; set; }
        public int CodigoEmpresa { get; set; }
        public int Numero { get; set; }
    }
}
