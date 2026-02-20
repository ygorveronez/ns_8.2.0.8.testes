using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Filiais
{
    public class CondicaoPagamentoFilial : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.CondicaoPagamentoFilial>
    {
        #region Construtores

        public CondicaoPagamentoFilial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> BuscarObjetoPorFilial(int codigoFilial)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.CondicaoPagamentoFilial>()
                .Where(o => o.Filial.AtivarCondicao && (o.Filial.Codigo == codigoFilial))
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento
                {
                    CodigoTipoCarga = o.TipoDeCarga.Codigo,
                    CodigoTipoOperacao = o.TipoOperacao.Codigo,
                    DiaEmissaoLimite = o.DiaEmissaoLimite, 
                    DiaMes = o.DiaMes,
                    DiasDePrazoPagamento = o.DiasDePrazoPagamento,
                    DiaSemana = o.DiaSemana,
                    TipoPrazoPagamento = o.TipoPrazoPagamento,
                    VencimentoForaMes = o.VencimentoForaMes,
                    ConsiderarDiaUtilVencimento = (bool?)(o.ConsiderarDiaUtilVencimento),
                })
                .ToList();
        }

        #endregion
    }
}
