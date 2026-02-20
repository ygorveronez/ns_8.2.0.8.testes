using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Financeiro
{
    public sealed class AprovacaoAlcadaPagamentoProvedor : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor,
        Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor
    >
    {
        #region Construtores

        public AprovacaoAlcadaPagamentoProvedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion
    }
}
