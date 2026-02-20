using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Produtos
{
    public class LocalArmazenamentoHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico>
    {
        public LocalArmazenamentoHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico BuscarUltimoHistoricoPorLocalArmazenamento(int codigoLocalArmazenamento)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico>()
                .Where(obj => obj.LocalArmazenamentoProduto.Codigo == codigoLocalArmazenamento)
                .OrderByDescending(obj => obj.Data) 
                .FirstOrDefault(); 
        }

        public Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico BuscarPorMovimentoEntrada(int codigoMovimentoEntrada)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico>()
                .Where(obj => obj.MovimentoEntradaTanque.Codigo == codigoMovimentoEntrada)
                .OrderByDescending(obj => obj.Data)
                .FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico BuscarPorMovimentoSaida(int codigoMovimentoSaida)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico>()
                .Where(obj => obj.MovimentacaoAbastecimentoSaida.Codigo == codigoMovimentoSaida)
                .OrderByDescending(obj => obj.Data)
                .FirstOrDefault();
        }
        #endregion

        #region Métodos Privados


        #endregion
    }
}
