using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class RegraAutorizacaoTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete>
    {
        public RegraAutorizacaoTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> ObterAtivas(int codigoTabelaFrete)
        {
            System.DateTime dataAtual = System.DateTime.Today;

            IQueryable<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete>();

            query = query.Where(o => o.Ativo && (o.TabelaFrete.Codigo == codigoTabelaFrete || o.TabelaFrete == null) && (o.Vigencia == null || o.Vigencia >= dataAtual));

            List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> listaRegrasAutorizacao = query.ToList().Where(regra => regra.IsAlcadaAtiva()).ToList();

            return listaRegrasAutorizacao;
        }

        public List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> ObterAtivasPorTabelaFrete(int codigoTabelaFrete)
        {
            System.DateTime dataAtual = System.DateTime.Today;

            IQueryable<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete>();

            query = query.Where(o => o.Ativo && (o.Vigencia == null || o.Vigencia >= dataAtual));

            if (codigoTabelaFrete > 0)
                query = query.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete);
            else
                query = query.Where(o => o.TabelaFrete == null);

            return query.ToList();
        }
    }
}
