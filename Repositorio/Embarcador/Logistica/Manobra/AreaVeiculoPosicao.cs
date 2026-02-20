using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class AreaVeiculoPosicao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao>
    {
        #region Construtores

        public AreaVeiculoPosicao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculoPosicao filtrosPesquisa)
        {
            var consultaAreaVeiculoPosicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao>();

            if (filtrosPesquisa.CodigoAreaPosicao > 0)
                consultaAreaVeiculoPosicao = consultaAreaVeiculoPosicao.Where(o => o.AreaVeiculo.Codigo == filtrosPesquisa.CodigoAreaPosicao);

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaAreaVeiculoPosicao = consultaAreaVeiculoPosicao.Where(o => o.AreaVeiculo.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.CodigoTipoRetornoCarga > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCarga  = new Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga() { Codigo = filtrosPesquisa.CodigoTipoRetornoCarga };

                consultaAreaVeiculoPosicao = consultaAreaVeiculoPosicao.Where(o => o.AreaVeiculo.TiposRetornoCarga.Contains(tipoRetornoCarga));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaAreaVeiculoPosicao = consultaAreaVeiculoPosicao.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.TipoAreaVeiculo.HasValue)
                consultaAreaVeiculoPosicao = consultaAreaVeiculoPosicao.Where(o => o.AreaVeiculo.Tipo == filtrosPesquisa.TipoAreaVeiculo.Value);

            return consultaAreaVeiculoPosicao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao BuscarPorCodigo(int codigo)
        {
            var consultaAreaVeiculoPosicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao>()
                .Where(o => o.Codigo == codigo);

            return consultaAreaVeiculoPosicao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao BuscarPorQRCode(string QRCode)
        {
            var consultaAreaVeiculoPosicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao>()
                .Where(o => o.QRCode == QRCode);

            return consultaAreaVeiculoPosicao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculoPosicao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaManobraAcao = Consultar(filtrosPesquisa);

            return ObterLista(consultaManobraAcao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculoPosicao filtrosPesquisa)
        {
            var consultaManobraAcao = Consultar(filtrosPesquisa);

            return consultaManobraAcao.Count();
        }

        #endregion
    }
}
