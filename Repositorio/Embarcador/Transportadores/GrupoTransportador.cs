using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Transportadores
{
    public class GrupoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador>
    {
        #region Construtores

        public GrupoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public GrupoTransportador(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork,cancellationToken) { }

        #endregion Construtores

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaGrupoTransportador filtrosPesquisa)
        {
            var consultaGrupoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaGrupoTransportador = consultaGrupoTransportador.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                consultaGrupoTransportador = consultaGrupoTransportador.Where(obj => obj.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaGrupoTransportador = consultaGrupoTransportador.Where(obj => obj.Ativo == true);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaGrupoTransportador = consultaGrupoTransportador.Where(obj => obj.Ativo == false);

            return consultaGrupoTransportador;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador BuscarPorCodigo(int codigo)
        {
            var consultaGrupoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador>()
                .Where(o => o.Codigo == codigo);

            return consultaGrupoTransportador.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador> BuscarPorCodigo(List<int> codigos)
        {
            var consultaGrupoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador>()
                .Where(o => codigos.Contains(o.Codigo));

            return consultaGrupoTransportador.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaGrupoTransportador filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaGrupoTransportador = Consultar(filtrosPesquisa);

            return ObterLista(consultaGrupoTransportador, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaGrupoTransportador filtrosPesquisa)
        {
            var consultaGrupoTransportador = Consultar(filtrosPesquisa);

            return consultaGrupoTransportador.Count();
        }

        public bool ExisteRegistroCadastrado()
        {
            var consultaGrupoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador>();

            return consultaGrupoTransportador.Count() > 0;
        }

        #endregion Métodos Públicos
    }
}
