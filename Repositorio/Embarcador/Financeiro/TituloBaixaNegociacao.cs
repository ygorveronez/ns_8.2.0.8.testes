using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloBaixaNegociacao : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao>
    {
        public TituloBaixaNegociacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> BuscarPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigoTituloBaixa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> BuscarPorTituloBaixa(int codigoTituloBaixa, int codigoTituloBaixaNegociacaoDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa && o.Codigo != codigoTituloBaixaNegociacaoDiff);

            return query.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> BuscarPorTituloBaixa(int codigoTituloBaixa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigoTituloBaixa select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigoTituloBaixa select obj;
            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo ContarBuscarPorNegociacao(int codigoNegociacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.TituloBaixaNegociacao.Codigo == codigoNegociacao select obj;
            return result.FirstOrDefault();
        }

        public dynamic BuscarPorBaixaTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            if (result.Count() > 0)
            {
                return (from obj in result.ToList()
                        orderby obj.Sequencia
                        select new
                        {
                            obj.Codigo,
                            Acrescimo = obj.Acrescimo.ToString("n2"),
                            DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                            DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                            Desconto = obj.Desconto.ToString("n2"),
                            obj.DescricaoSituacao,
                            obj.Sequencia,
                            obj.SituacaoFaturaParcela,
                            Valor = obj.Valor.ToString("n2")
                        }).ToList();
            }
            else
                return null;
        }        
    }

}
