using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Compras
{
    public class FiltroPesquisaRequisicaoMercadoria
    {
        public int CodigoEmpresa { get; set; }
        public ModoRequisicaoMercadoria? Modo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int Filial { get; set; }
        public int Motivo { get; set; }
        public SituacaoRequisicaoMercadoria? Situacao { get; set; }
        public int FuncionarioRequisitado { get; set; }
        public int Veiculo { get; set; }
        public int Numero { get; set; }
    }
}
