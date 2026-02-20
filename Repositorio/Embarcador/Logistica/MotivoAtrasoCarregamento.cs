using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;


namespace Repositorio.Embarcador.Logistica
{
    public class MotivoAtrasoCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento>
    {
        #region Construtores

        public MotivoAtrasoCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion


        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoPunicaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Descricao.Contains(descricao));

            if (situacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Status == ativo);
            }

            return consultaMotivoPunicaoVeiculo;
        }

        #endregion

        #region Métodos Públicos


        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoPunicaoVeiculo = Consultar(descricao, situacaoAtivo);

            return consultaMotivoPunicaoVeiculo.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaMotivoPunicaoVeiculo = Consultar(descricao, situacaoAtivo);

            return ObterLista(consultaMotivoPunicaoVeiculo, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }


        #endregion
    }
}
