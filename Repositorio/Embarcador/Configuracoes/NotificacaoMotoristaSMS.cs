using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class NotificacaoMotoristaSMS : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS>
    {
        #region Construtores

        public NotificacaoMotoristaSMS(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public NotificacaoMotoristaSMS(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS>();

            var result = query.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS> BuscarPorCodigoAsync(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS>();

            var result = query.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaNotificacaoMotoristaSMS filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterListaAsync(consulta, parametrosConsulta);
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaNotificacaoMotoristaSMS filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.CountAsync(CancellationToken);
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaNotificacaoMotoristaSMS filtrosPesquisa)
        {
            var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo.HasValue)
                consulta = consulta.Where(o => o.Ativo == filtrosPesquisa.Ativo.Value);

            if (filtrosPesquisa.TipoNotificacaoSMS.HasValue)
                consulta = consulta.Where(o => o.TipoEnvio == filtrosPesquisa.TipoNotificacaoSMS.Value);

            if (filtrosPesquisa.NotificacaoSuperApp.HasValue)
                consulta = consulta.Where(o => o.NotificacaoSuperApp == filtrosPesquisa.NotificacaoSuperApp.Value);

            return consulta;
        }

        #endregion
    }
}
