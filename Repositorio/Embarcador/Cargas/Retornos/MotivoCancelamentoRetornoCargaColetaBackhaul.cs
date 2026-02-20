using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Cargas.Retornos
{
    public sealed class MotivoCancelamentoRetornoCargaColetaBackhaul : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul>
    {
        #region Construtores

        public MotivoCancelamentoRetornoCargaColetaBackhaul(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoCancelamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaMotivoCancelamento = consultaMotivoCancelamento.Where(o => o.Descricao.Contains(descricao));

            if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaMotivoCancelamento = consultaMotivoCancelamento.Where(o => o.Ativo);
            else if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaMotivoCancelamento = consultaMotivoCancelamento.Where(o => !o.Ativo);

            return consultaMotivoCancelamento;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul BuscarPorCodigo(int codigo)
        {
            var consultaMotivoCancelamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul>()
                .Where(o => o.Codigo == codigo);                

            return consultaMotivoCancelamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotivoCancelamento = Consultar(descricao, situacaoAtivo);

            return ObterLista(consultaMotivoCancelamento, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoCancelamento = Consultar(descricao, situacaoAtivo);

            return consultaMotivoCancelamento.Count();
        }

        #endregion
    }
}
