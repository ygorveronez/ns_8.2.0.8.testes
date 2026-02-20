using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public class PunicaoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo>
    {
        #region Construtores

        public PunicaoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo> Consultar(int codigoVeiculo, int codigoMotivoPunicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoPunicaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo>();

            if (codigoVeiculo > 0)
                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (codigoMotivoPunicao > 0)
                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Motivo.Codigo == codigoMotivoPunicao);

            if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Ativo);
            else if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => !o.Ativo);

            return consultaMotivoPunicaoVeiculo;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo BuscarPorCodigo(int codigo)
        {
            var punicaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return punicaoVeiculo;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo> BuscarPorVeiculo(int codigo)
        {
            var listaPunicaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo>()
                .Where(o => o.Veiculo.Codigo == codigo)
                .ToList();

            return listaPunicaoVeiculo;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo> Consultar(int codigoVeiculo, int codigoMotivoPunicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaPunicaoVeiculo = Consultar(codigoVeiculo, codigoMotivoPunicao, situacaoAtivo);

            return ObterLista(consultaPunicaoVeiculo, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoVeiculo, int codigoMotivoPunicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaPunicaoVeiculo = Consultar(codigoVeiculo, codigoMotivoPunicao, situacaoAtivo);

            return consultaPunicaoVeiculo.Count();
        }

        #endregion
    }
}
