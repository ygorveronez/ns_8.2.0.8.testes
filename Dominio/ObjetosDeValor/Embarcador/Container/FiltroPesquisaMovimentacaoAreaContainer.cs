using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Container
{
    public sealed class FiltroPesquisaMovimentacaoAreaContainer
    {
        public string NumeroCarga { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino { get; set; }
        public int CodigoVeiculo { get; set; }
        public double CpfCnpjAreaContainer { get; set; }
        public SituacaoEntrega? SituacaoEntrega { get; set; }
        public int CodigoEmpresa { get; set; }
        public List<double> CpfCnpjEmpresaColeta { get; set; }
        public TipoCargaEntrega? TipoCargaEntrega { get; set; }
        public string NumeroEXP { get; set; }
        public string NumeroContainer { get; set; }
    }
}