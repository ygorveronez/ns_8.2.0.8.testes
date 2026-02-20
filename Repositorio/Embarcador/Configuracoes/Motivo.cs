using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class Motivo : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.Motivo>
    {
        #region Construtores

        public Motivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Motivo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.Motivo> Consultar(Dominio.ObjetosDeValor.FiltroPesquisaMotivoRejeicao filtroMotivo)
        {
            var consultaMotivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Motivo>();

            if (!string.IsNullOrWhiteSpace(filtroMotivo.Descricao))
                consultaMotivo = consultaMotivo.Where(obj => obj.Descricao.Contains(filtroMotivo.Descricao));

            if (filtroMotivo.Ativo == SituacaoAtivoPesquisa.Ativo)
                consultaMotivo = consultaMotivo.Where(obj => obj.Ativo);
            else if (filtroMotivo.Ativo == SituacaoAtivoPesquisa.Inativo)
                consultaMotivo = consultaMotivo.Where(obj => !obj.Ativo);

            if (filtroMotivo.TipoMotivo == TipoMotivo.RejeicaoDadosNFeColeta)
                consultaMotivo = consultaMotivo.Where(obj => obj.Tipo == (TipoMotivo.RejeicaoDadosNFeColeta));

            return consultaMotivo;
        }

        #endregion

        #region Métodos Públicos

        public Task<List<Dominio.Entidades.Embarcador.Configuracoes.Motivo>> ConsultarAsync(Dominio.ObjetosDeValor.FiltroPesquisaMotivoRejeicao filtroMotivo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotivo = Consultar(filtroMotivo);

            return ObterListaAsync(consultaMotivo, parametrosConsulta);
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.FiltroPesquisaMotivoRejeicao filtroMotivo)
        {
            var consultaMotivo = Consultar(filtroMotivo);

            return consultaMotivo.CountAsync();
        }

        #endregion
    }
}
