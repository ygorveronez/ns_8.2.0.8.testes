using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Relatorios
{
    public class RelatorioControleGeracao : RepositorioBase<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>
    {
        private CancellationToken _cancellationToken;
        public RelatorioControleGeracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public RelatorioControleGeracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { this._cancellationToken = cancellationToken; }

        public Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> BuscarPorCodigoAsync(int codigo)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>()
                .Where(x => x.Codigo == codigo).FirstOrDefaultAsync(_cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao BuscarUltimoRelatorioPorEntidade(int codigoEntidade, Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>();
            var result = from obj in query where obj.CodigoEntidade == codigoEntidade && obj.Relatorio.CodigoControleRelatorios == codigoControleRelatorio select obj;
            return result.OrderBy("Codigo descending").FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> BuscarGeradosAnterioresAData(DateTime dataGerado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>();
            var result = from obj in query where obj.DataFinalGeracao < dataGerado && obj.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.Gerado || obj.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.FalhaAoGerar select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> BuscarRelatoriosEmExecucao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>();
            var result = from obj in query where obj.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao && obj.DataInicioGeracao < DateTime.Now.AddMinutes(-30) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> BuscarRelatoriosEmExecucao(int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>();
            var result = from obj in query where obj.Usuario.Codigo == usuario && obj.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao select obj;
            return result.ToList();
        }

        public int ContarRelatoriosEmExecucao(int codigoUsuario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>();

            query = query.Where(o => o.Usuario.Codigo == codigoUsuario && o.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao);

            return query.Select(o => o.Codigo).Count();
        }

        public Task<int> ContarRelatoriosEmExecucaoAsync(int codigoUsuario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>();

            query = query.Where(o => o.Usuario.Codigo == codigoUsuario && o.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao);

            return query.Select(o => o.Codigo).CountAsync(_cancellationToken);
        }

        public int BuscarCodigoRelatorioPendenteGeracao(List<int> codigosDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>();

            query = query.Where(o => o.GerarPorServico && o.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao);

            if (codigosDiff.Count > 0)
                query = query.Where(o => !codigosDiff.Contains(o.Codigo));

            return query.OrderBy(o => o.DataInicioGeracao).Select(o => o.Codigo).FirstOrDefault();
        }

        public List<int> BuscarCodigosRelatoriosPendentesGeracao(List<int> codigosDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>();

            query = query.Where(o => o.GerarPorServico && o.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao);

            if (codigosDiff.Count > 0)
                query = query.Where(o => !codigosDiff.Contains(o.Codigo));

            return query.OrderBy(o => o.DataInicioGeracao).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosRelatoriosPendentesGeracaoLocal(List<int> codigosDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>();

            query = query.Where(o => o.GerarPorServico && o.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucaoLocal);

            if (codigosDiff.Count > 0)
                query = query.Where(o => !codigosDiff.Contains(o.Codigo));

            return query.OrderBy(o => o.DataInicioGeracao).Select(o => o.Codigo).ToList();
        }

        public void DeletarPorAutomatizacao(int codigoAutomatizacaoGeracaoRelatorio)
        {
            SessionNHiBernate.CreateQuery("DELETE FROM RelatorioControleGeracao r WHERE r.AutomatizacaoGeracaoRelatorio.Codigo = :codigoAutomatizacaoGeracaoRelatorio").SetInt32("codigoAutomatizacaoGeracaoRelatorio", codigoAutomatizacaoGeracaoRelatorio).ExecuteUpdate();
        }
    }
}
