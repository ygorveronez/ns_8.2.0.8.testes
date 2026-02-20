using Infrastructure.Services.Cache;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class ControleThread : RepositorioBase<Dominio.Entidades.ControleThread>
    {

        private const string ControleThreadKey = "ControleThread";
        #region Construtores

        public ControleThread(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ControleThread(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        //TODO: Ajustar para async
        public Dominio.Entidades.ControleThread BuscarPorThread(string Thread)
        {
            return CacheProvider.Instance.GetOrCreate(ControleThreadKey + Thread,
                () =>
                {
                    var query = this.SessionNHiBernate.Query<Dominio.Entidades.ControleThread>();
                    var result = from obj in query where obj.Thread == Thread select obj;
                    return result.FirstOrDefault();
                }, TimeSpan.FromMinutes(1));
            
        }

        public Task<List<Dominio.Entidades.ControleThread>> BuscarPorThreadsAsync(List<string> threads)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ControleThread>()
                .Where(obj => threads.Contains(obj.Thread));

            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.ControleThread> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaControleThread filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaControleThread = Consultar(filtrosPesquisa);

            return ObterLista(consultaControleThread, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaControleThread filtrosPesquisa)
        {
            var consultaControleThread = Consultar(filtrosPesquisa);

            return consultaControleThread.Count();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.ControleThread> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaControleThread filtrosPesquisa)
        {
            var consultaControleThread = this.SessionNHiBernate.Query<Dominio.Entidades.ControleThread>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaControleThread = consultaControleThread.Where(o => o.Thread.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo.HasValue)
            {
                if (filtrosPesquisa.Ativo.Value)
                    consultaControleThread = consultaControleThread.Where(o => o.Ativo);
                else if (!filtrosPesquisa.Ativo.Value)
                    consultaControleThread = consultaControleThread.Where(o => !o.Ativo);
            }

            return consultaControleThread;
        }

        #endregion Métodos Privados
    }
}
