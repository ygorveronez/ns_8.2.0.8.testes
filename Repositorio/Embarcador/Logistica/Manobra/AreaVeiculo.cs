using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class AreaVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo>
    {
        #region Construtores

        public AreaVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo> ConsultarInternal(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculo filtrosPesquisa)
        {
            var consultaAreaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaAreaVeiculo = consultaAreaVeiculo.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaAreaVeiculo = consultaAreaVeiculo.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Tipo.HasValue)
                consultaAreaVeiculo = consultaAreaVeiculo.Where(o => o.Tipo == filtrosPesquisa.Tipo.Value);

            if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaAreaVeiculo = consultaAreaVeiculo.Where(o => o.Ativo);
            else if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaAreaVeiculo = consultaAreaVeiculo.Where(o => !o.Ativo);

            return consultaAreaVeiculo;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.AreaVeiculo BuscarPorCodigo(int codigo)
        {
            var consultaAreaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo>()
                .Where(o => o.Codigo == codigo);

            return consultaAreaVeiculo.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.AreaVeiculo BuscarPorQRCode(string QRCode)
        {
            var consultaAreaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo>()
                .Where(o => o.QRCode == QRCode);

            return consultaAreaVeiculo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAreaVeiculo = ConsultarInternal(filtrosPesquisa);

            return ObterLista(consultaAreaVeiculo, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculo filtrosPesquisa)
        {
            var consultaAreaVeiculo = ConsultarInternal(filtrosPesquisa);

            return consultaAreaVeiculo.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo> BuscarTodosAtivos()
        {
            var consultaAreaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo>();

            return consultaAreaVeiculo
                .Where(areaVeiculo => areaVeiculo.Ativo)
                .ToList();
        }

        #endregion
    }
}
