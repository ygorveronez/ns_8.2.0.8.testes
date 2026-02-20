using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public class MotivoPunicaoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MotivoPunicaoVeiculo>
    {
        #region Construtores

        public MotivoPunicaoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.MotivoPunicaoVeiculo> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoPunicaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoPunicaoVeiculo>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Descricao.Contains(descricao));

            if (situacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Ativo == ativo);
            }

            return consultaMotivoPunicaoVeiculo;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.MotivoPunicaoVeiculo BuscarPorCodigo(int codigo)
        {
            var motivoPunicaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoPunicaoVeiculo>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return motivoPunicaoVeiculo;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoPunicaoVeiculo> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaMotivoPunicaoVeiculo = Consultar(descricao, situacaoAtivo);

            return ObterLista(consultaMotivoPunicaoVeiculo, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoPunicaoVeiculo = Consultar(descricao, situacaoAtivo);

            return consultaMotivoPunicaoVeiculo.Count();
        }

        #endregion
    }
}
