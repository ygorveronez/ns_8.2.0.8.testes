using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Frota
{
    public class MotivoSucateamentoPneu : RepositorioBase<Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu>
    {
        #region Construtores

        public MotivoSucateamentoPneu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMotivoSucateamentoPneu filtrosPesquisa)
        {
            var consultaMotivoSucateamentoPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu>();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaMotivoSucateamentoPneu = consultaMotivoSucateamentoPneu.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaMotivoSucateamentoPneu = consultaMotivoSucateamentoPneu.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.SituacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaMotivoSucateamentoPneu = consultaMotivoSucateamentoPneu.Where(o => o.Ativo == ativo);
            }

            return consultaMotivoSucateamentoPneu;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu BuscarPorCodigo(int codigo)
        {
            var motivoSucateamentoPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return motivoSucateamentoPneu;
        }
        public List<Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMotivoSucateamentoPneu filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotivoSucateamentoPneu = Consultar(filtrosPesquisa);

            return ObterLista(consultaMotivoSucateamentoPneu, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMotivoSucateamentoPneu filtrosPesquisa)
        {
            var consultaMotivoSucateamentoPneu = Consultar(filtrosPesquisa);

            return consultaMotivoSucateamentoPneu.Count();
        }

        #endregion
    }
}
