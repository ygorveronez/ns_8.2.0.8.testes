using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaChequeRelatorio
    {
        public int CodigoTitulo { get; set; }
        public double CpfCnpjPessoa { get; set; }
        public DateTime? DataCompensacaoInicio { get; set; }
        public DateTime? DataCompensacaoLimite { get; set; }
        public DateTime? DataTransacaoInicio { get; set; }
        public DateTime? DataTransacaoLimite { get; set; }
        public DateTime? DataVencimentoInicio { get; set; }
        public DateTime? DataVencimentoLimite { get; set; }
        public string NumeroCheque { get; set; }
        public Enumeradores.StatusCheque? Status { get; set; }
        public Enumeradores.TipoCheque? Tipo { get; set; }
        public decimal ValorInicio { get; set; }
        public decimal ValorLimite { get; set; }
        public int CodigoEmpresa { get; set; }

        public int Banco { get; set; }
    }
}
