using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GerenciamentoIrregularidades
{
    public class MotivoDesacordo : RepositorioBase<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo>
    {
        #region Construtores

        public MotivoDesacordo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoDesacordo filtrosPesquisa)
        {
            var consultaMotivoDesacordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaMotivoDesacordo = consultaMotivoDesacordo.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao != SituacaoAtivaPesquisa.Todos)
            {
                if (filtrosPesquisa.Situacao == SituacaoAtivaPesquisa.Ativa)
                    consultaMotivoDesacordo = consultaMotivoDesacordo.Where(obj => obj.Situacao == true);
                else
                    consultaMotivoDesacordo = consultaMotivoDesacordo.Where(obj => obj.Situacao == false);
            }

            if (filtrosPesquisa.SubstituiCTe == true)
                consultaMotivoDesacordo = consultaMotivoDesacordo.Where(obj => obj.SubstituiCTe == true);

            else if (filtrosPesquisa.SubstituiCTe == false)
                consultaMotivoDesacordo = consultaMotivoDesacordo.Where(obj => obj.SubstituiCTe == false);

            if (filtrosPesquisa.CodigoIrregularidade > 0)
                consultaMotivoDesacordo = consultaMotivoDesacordo.Where(obj => obj.Irregularidade.Codigo == filtrosPesquisa.CodigoIrregularidade);

            return consultaMotivoDesacordo;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo BuscarPorCodigo(int codigo)
        {
            var consultaMotivoDesacordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo>()
                .Where(obj => obj.Codigo == codigo);

            return consultaMotivoDesacordo.FirstOrDefault();
        }

        public bool ExisteCadastrado()
        {
            var consultaMotivoDesacordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo>();

            return consultaMotivoDesacordo.Any();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoDesacordo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotivoDesacordo = Consultar(filtrosPesquisa);

            return ObterLista(consultaMotivoDesacordo, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoDesacordo filtrosPesquisa)
        {
            var consultaMotivoDesacordo = Consultar(filtrosPesquisa);

            return consultaMotivoDesacordo.Count();
        }

        public bool ExisteDuplicidade(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo motivo)
        {
            var consultaMotivoDesacordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo>();

            consultaMotivoDesacordo = consultaMotivoDesacordo.Where(p => p.Codigo != motivo.Codigo);
            consultaMotivoDesacordo = consultaMotivoDesacordo.Where(p => p.Descricao.Equals(motivo.Descricao));
            consultaMotivoDesacordo = consultaMotivoDesacordo.Where(p => p.Irregularidade.Codigo.Equals(motivo.Irregularidade.Codigo));
            consultaMotivoDesacordo = consultaMotivoDesacordo.Where(p => p.Situacao.Equals(motivo.Situacao));
            consultaMotivoDesacordo = consultaMotivoDesacordo.Where(p => p.SubstituiCTe.Equals(motivo.SubstituiCTe));

            return consultaMotivoDesacordo.Any();
        }

        #endregion Métodos Públicos
    }
}
