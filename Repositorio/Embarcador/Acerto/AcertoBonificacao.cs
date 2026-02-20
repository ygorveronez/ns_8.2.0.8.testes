using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoBonificacao : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao>
    {
        public AcertoBonificacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public decimal TotalBonificacaoPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto);

            return query.Sum(o => (decimal?)o.ValorBonificacao) ?? 0m;
        }

        public decimal BuscarValorMoedaestrangeira(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao>();
            query = query.Where(obj => obj.AcertoViagem.Codigo == codigo);

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && (obj.MoedaCotacaoBancoCentral == null || moedas.Contains(obj.MoedaCotacaoBancoCentral.Value)));
            else
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && obj.MoedaCotacaoBancoCentral != null && moedas.Contains(obj.MoedaCotacaoBancoCentral.Value));

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                return query.Sum(o => (decimal?)o.ValorBonificacao) ?? 0m;
            else
                return query.Sum(o => (decimal?)o.ValorOriginalMoedaEstrangeira) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao> BuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao> BuscarPorAcerto(int codigoAcerto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.Count();
        }

        #region Relatorios
        public IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.BonificacaoAcertoViagem> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioBonificacaoAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaBonificacaoAcertoViagem().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.BonificacaoAcertoViagem)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.BonificacaoAcertoViagem>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioBonificacaoAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaBonificacaoAcertoViagem().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }
        #endregion
    }
}
