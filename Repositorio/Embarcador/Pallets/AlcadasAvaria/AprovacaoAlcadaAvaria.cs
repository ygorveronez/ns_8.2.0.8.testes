using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pallets.AlcadasAvaria
{
    public class AprovacaoAlcadaAvaria : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria,
        Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria,
        Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet
    >
    {
        #region Construtores

        public AprovacaoAlcadaAvaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAvariaAprovacao filtrosPesquisa)
        {
            var consultaAvaria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>();
            var consultaAlcadaAvaria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.Codigo > 0)
                consultaAvaria = consultaAvaria.Where(o => o.Codigo == filtrosPesquisa.Codigo);

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaAvaria = consultaAvaria.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoMotivoAvaria > 0)
                consultaAvaria = consultaAvaria.Where(o => o.MotivoAvaria.Codigo == filtrosPesquisa.CodigoMotivoAvaria);

            if (filtrosPesquisa.CodigoSetor > 0)
                consultaAvaria = consultaAvaria.Where(o => o.Setor.Codigo == filtrosPesquisa.CodigoSetor);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaAvaria = consultaAvaria.Where(o => o.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaAvaria = consultaAvaria.Where(o => o.Data >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaAvaria = consultaAvaria.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaPallet.Todas)
                consultaAvaria = consultaAvaria.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaAvaria = consultaAlcadaAvaria.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaPallet.AguardandoAprovacao)
                consultaAlcadaAvaria = consultaAlcadaAvaria.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaAvaria.Where(o => consultaAlcadaAvaria.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAvariaAprovacao filtrosPesquisa, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var avarias = Consultar(filtrosPesquisa);
            
            return ObterLista(avarias, propriedadeOrdenacao, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAvariaAprovacao filtrosPesquisa)
        {
            var avarias = Consultar(filtrosPesquisa);

            return avarias.Count();
        }

        #endregion
    }
}
