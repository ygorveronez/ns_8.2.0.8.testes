using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoStatusViagem : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>
    {
        public MonitoramentoStatusViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MonitoramentoStatusViagem(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> Consultar(string descricao, string sigla, int ordem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));
            if (!string.IsNullOrWhiteSpace(sigla))
                query = query.Where(obj => obj.Sigla.Contains(sigla));
            if (ordem >= 0)
                query = query.Where(obj => obj.Ordem == ordem);
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, string sigla, int ordem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));
            if (!string.IsNullOrWhiteSpace(sigla))
                query = query.Where(obj => obj.Sigla.Contains(sigla));
            if (ordem >= 0)
                query = query.Where(obj => obj.Ordem == ordem);
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> BuscarAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>();
            query = query = query.Where(obj => obj.Ativo == true);
            return query.OrderBy(obj => obj.Ordem).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>> BuscarAtivosAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>();
            query = query = query.Where(obj => obj.Ativo == true);
            return query.OrderBy(obj => obj.Ordem).ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem BuscarAtivoPorTipoRegra(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>();
            query = query = query.Where(obj => obj.Ativo == true && obj.TipoRegra == tipoRegra);
            return query.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> BuscarAtivoPorTipoRegraAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>();
            query = query = query.Where(obj => obj.Ativo == true && obj.TipoRegra == tipoRegra);
            return await query.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem BuscarPorTipoRegra(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>();
            query = query = query.Where(obj => obj.TipoRegra == tipoRegra);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> BuscarPorGrupo(int codigoGrupo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>();
            query = query = query.Where(obj => obj.Grupo.Codigo == codigoGrupo);
            return query.OrderBy(obj => obj.Ordem).ToList();
        }

    }
}
