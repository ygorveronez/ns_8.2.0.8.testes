using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica.GrupoMotoristas
{
    public class GrupoMotoristasIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        : Abstrato.RepositorioRelacionamentoGrupoMotoristas<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao>(
            unitOfWork,
            cancellationToken,
            "T_GRUPO_MOTORISTAS_INTEGRACAO")
    {
        public async Task<List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao>> BuscarGrupoMotoristaIntegracao(Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaRelacionamentosGrupoMotoristas filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            AplicarFiltros(out IQueryable<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao> consulta, filtro);

            return await ObterListaAsync(consulta, parametrosConsulta);
        }

        public async Task<int> ContarBuscaGrupoMotoristaIntegracao(Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaRelacionamentosGrupoMotoristas filtro)
        {
            AplicarFiltros(out IQueryable<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao> consulta, filtro);

            return await consulta.CountAsync(CancellationToken);
        }

        private void AplicarFiltros(
            out IQueryable<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao> consulta,
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaRelacionamentosGrupoMotoristas filtro)
        {
            consulta = MontarConsulta(filtro.CodigoGrupoMotoristas);

            if (filtro.SituacaoIntegracao.HasValue)
            {
                consulta = consulta.Where(o => o.SituacaoIntegracao == filtro.SituacaoIntegracao.Value);
            }

            if (filtro.TipoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                consulta = consulta.Where(o => o.TipoIntegracao.Tipo == filtro.TipoIntegracao);
        }

        public Task<List<int>> BuscarIntegracoesAguardandoAsync(int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao>();
            var result = from obj in query
                         where obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                         select obj;
            result
                .OrderBy(o => o.Codigo)
                .Select(o => o.Codigo)
                .Skip(0)
                .Take(numeroRegistrosPorVez)
                .ToList();

            return result.Select(x => (int)x.Codigo).ToListAsync<int>(CancellationToken);
        }
    }
}
