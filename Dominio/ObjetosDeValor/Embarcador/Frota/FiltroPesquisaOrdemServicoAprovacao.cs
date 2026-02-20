using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaOrdemServicoAprovacao
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataLimite { get; set; }
        public int Numero { get; set; }
        public SituacaoOrdemServicoFrota? Situacao { get; set; }
        public double CpfCnpjFornecedor { get; set; }
        public int CodigoOperador { get; set; }
        public int CodigoUsuario { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
