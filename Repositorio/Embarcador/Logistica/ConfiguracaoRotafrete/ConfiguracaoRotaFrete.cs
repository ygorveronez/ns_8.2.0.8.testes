using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class ConfiguracaoRotaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete>
    {
        #region Construtores

        public ConfiguracaoRotaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaConfiguracaoRotaFrete filtrosPesquisa)
        {
            var consultaConfiguracaoRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaConfiguracaoRotaFrete = consultaConfiguracaoRotaFrete.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Ativo)
                consultaConfiguracaoRotaFrete = consultaConfiguracaoRotaFrete.Where(o => o.Ativo == true);
            else if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Inativo)
                consultaConfiguracaoRotaFrete = consultaConfiguracaoRotaFrete.Where(o => o.Ativo == false);

            return consultaConfiguracaoRotaFrete;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete BuscarPorCodigo(int codigo)
        {
            var consultaConfiguracaoRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete>()
                .Where(o => o.Codigo == codigo);

            return consultaConfiguracaoRotaFrete.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete BuscarPorCodigoFilial(int codigo)
        {
            var consultaConfiguracaoRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete>()
                .Where(o => o.Filial.Codigo == codigo);

            return consultaConfiguracaoRotaFrete.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete BuscarPrimeiraDisponivel(int codigoLocalidadeOrigem, int codigoFilial, List<string> listaUfDestino, List<int> listaCodigoLocalidadeDestino, int codigoTipoCarga, int codigoModeloVeicularCarga, bool considerarTransportadores = true, bool considerarTipoOperacao = false)
        {
            var consultaConfiguracaoRotaFreteEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa>();
            var consultaTipoOperacaoConfiguracaoRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TipoOperacaoConfiguracaoRotaFrete>();
            var consultaConfiguracaoRotaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete>()
                .Where(o => o.Ativo == true && o.LocalidadesOrigem.Any(l => l.Codigo == codigoLocalidadeOrigem) && o.Estados.Any(e => listaUfDestino.Contains(e.Sigla)));

            if (considerarTransportadores) consultaConfiguracaoRotaFrete = consultaConfiguracaoRotaFrete.Where(o => consultaConfiguracaoRotaFreteEmpresa.Any(e => e.ConfiguracaoRotaFrete.Codigo == o.Codigo));
            if (considerarTipoOperacao) consultaConfiguracaoRotaFrete = consultaConfiguracaoRotaFrete.Where(o => consultaTipoOperacaoConfiguracaoRotaFrete.Any(e => e.ConfiguracaoRotaFrete.Codigo == o.Codigo));

            if (codigoFilial > 0)
                consultaConfiguracaoRotaFrete = consultaConfiguracaoRotaFrete.Where(o => o.Filial == null || o.Filial.Codigo == codigoFilial);

            if (listaCodigoLocalidadeDestino?.Count > 0)
                consultaConfiguracaoRotaFrete = consultaConfiguracaoRotaFrete.Where(o =>
                    !o.PossuiLocalidadesDestino ||
                    o.LocalidadesDestino.Any(l => listaCodigoLocalidadeDestino.Contains(l.Codigo))
                );

            if (codigoTipoCarga > 0)
            {
                var consultaConfiguracaoRotaFreteTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga>();

                consultaConfiguracaoRotaFrete = consultaConfiguracaoRotaFrete.Where(o =>
                    !o.PossuiTiposCarga ||
                    consultaConfiguracaoRotaFreteTipoCarga.Any(t => t.ConfiguracaoRotaFrete.Codigo == o.Codigo && t.TipoCarga.Codigo == codigoTipoCarga)
                );
            }

            if (codigoModeloVeicularCarga > 0)
            {
                var consultaConfiguracaoRotaFreteModeloVeicularCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga>();

                consultaConfiguracaoRotaFrete = consultaConfiguracaoRotaFrete.Where(o =>
                    !o.PossuiModelosVeicularesCarga ||
                    consultaConfiguracaoRotaFreteModeloVeicularCarga.Any(m => m.ConfiguracaoRotaFrete.Codigo == o.Codigo && m.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga)
                );
            }

            return consultaConfiguracaoRotaFrete
                .OrderByDescending(o => o.Filial != null)
                .OrderByDescending(o => o.PossuiLocalidadesDestino)
                .OrderByDescending(o => o.PossuiTiposCarga)
                .OrderByDescending(o => o.PossuiModelosVeicularesCarga)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaConfiguracaoRotaFrete filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaConfiguracaoRotaFrete = Consultar(filtrosPesquisa);

            return ObterLista(consultaConfiguracaoRotaFrete, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaConfiguracaoRotaFrete filtrosPesquisa)
        {
            var consultaConfiguracaoRotaFrete = Consultar(filtrosPesquisa);

            return consultaConfiguracaoRotaFrete.Count();
        }

        #endregion
    }
}
