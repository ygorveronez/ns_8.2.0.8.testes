using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public sealed class MotivoAdvertenciaTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador>
    {
        #region Construtores

        public MotivoAdvertenciaTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador> Consultar(string descricao, SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoAdvertenciaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaMotivoAdvertenciaTransportador = consultaMotivoAdvertenciaTransportador.Where(o => o.Descricao.Contains(descricao));

            if (situacaoAtivo == SituacaoAtivoPesquisa.Ativo)
                consultaMotivoAdvertenciaTransportador = consultaMotivoAdvertenciaTransportador.Where(o => o.Ativo);
            else if (situacaoAtivo == SituacaoAtivoPesquisa.Inativo)
                consultaMotivoAdvertenciaTransportador = consultaMotivoAdvertenciaTransportador.Where(o => !o.Ativo);

            return consultaMotivoAdvertenciaTransportador;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador> Consultar(string descricao, SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotivoRemocaoVeiculoEscala = Consultar(descricao, situacaoAtivo);

            return ObterLista(consultaMotivoRemocaoVeiculoEscala, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoRemocaoVeiculoEscala = Consultar(descricao, situacaoAtivo);

            return consultaMotivoRemocaoVeiculoEscala.Count();
        }

        #endregion
    }
}
