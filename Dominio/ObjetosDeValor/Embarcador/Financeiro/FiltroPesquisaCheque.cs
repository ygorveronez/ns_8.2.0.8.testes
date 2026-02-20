using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaCheque
    {
        public int CodigoTitulo { get; set; }
        public double CpfCnpjPessoa { get; set; }
        public string NumeroCheque { get; set; }
        public Enumeradores.StatusCheque? Status { get; set; }
        public Enumeradores.TipoCheque? Tipo { get; set; }
        public List<Enumeradores.TipoCheque> Tipos { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
