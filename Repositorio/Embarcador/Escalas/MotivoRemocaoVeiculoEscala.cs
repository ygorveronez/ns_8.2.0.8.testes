using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Escalas
{
    public sealed class MotivoRemocaoVeiculoEscala : RepositorioBase<Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala>
    {
        #region Construtores

        public MotivoRemocaoVeiculoEscala(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala> Consultar(string descricao, SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoRemocaoVeiculoEscala = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaMotivoRemocaoVeiculoEscala = consultaMotivoRemocaoVeiculoEscala.Where(o => o.Descricao.Contains(descricao));

            if (situacaoAtivo == SituacaoAtivoPesquisa.Ativo)
                consultaMotivoRemocaoVeiculoEscala = consultaMotivoRemocaoVeiculoEscala.Where(o => o.Ativo);
            else if (situacaoAtivo == SituacaoAtivoPesquisa.Inativo)
                consultaMotivoRemocaoVeiculoEscala = consultaMotivoRemocaoVeiculoEscala.Where(o => !o.Ativo);

            return consultaMotivoRemocaoVeiculoEscala;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala> Consultar(string descricao, SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
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
