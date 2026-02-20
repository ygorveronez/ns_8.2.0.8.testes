using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public class ConfiguracaoProgramacaoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga>
    {
        #region Construtores

        public ConfiguracaoProgramacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaConfiguracaoProgramacaoCarga filtrosPesquisa)
        {
            var consultaConfiguracaoProgramacaoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga>();

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaConfiguracaoProgramacaoCarga = consultaConfiguracaoProgramacaoCarga.Where(configuracaoProgramacaoCarga => configuracaoProgramacaoCarga.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaConfiguracaoProgramacaoCarga = consultaConfiguracaoProgramacaoCarga.Where(configuracaoProgramacaoCarga => configuracaoProgramacaoCarga.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Ativo)
                consultaConfiguracaoProgramacaoCarga = consultaConfiguracaoProgramacaoCarga.Where(configuracaoProgramacaoCarga => configuracaoProgramacaoCarga.Ativo == true);
            else if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Inativo)
                consultaConfiguracaoProgramacaoCarga = consultaConfiguracaoProgramacaoCarga.Where(configuracaoProgramacaoCarga => configuracaoProgramacaoCarga.Ativo == false);

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
            {
                var consultaConfiguracaoModeloVeicularCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga>()
                    .Where(configuracaoModeloVeicularCarga => configuracaoModeloVeicularCarga.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);

                consultaConfiguracaoProgramacaoCarga.Where(configuracaoProgramacaoCarga => consultaConfiguracaoModeloVeicularCarga.Any(configuracaoModeloVeicularCarga => configuracaoModeloVeicularCarga.ConfiguracaoProgramacaoCarga.Codigo == configuracaoProgramacaoCarga.Codigo));
            }

            if (filtrosPesquisa.CodigoTipoCarga > 0)
            {
                var consultaConfiguracaoTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga>()
                    .Where(configuracaoTipoCarga => configuracaoTipoCarga.TipoCarga.Codigo == filtrosPesquisa.CodigoTipoCarga);

                consultaConfiguracaoProgramacaoCarga.Where(configuracaoProgramacaoCarga => consultaConfiguracaoTipoCarga.Any(configuracaoTipoCarga => configuracaoTipoCarga.ConfiguracaoProgramacaoCarga.Codigo == configuracaoProgramacaoCarga.Codigo));
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                var consultaConfiguracaoTipoOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao>()
                    .Where(configuracaoTipoOperacao => configuracaoTipoOperacao.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

                consultaConfiguracaoProgramacaoCarga.Where(configuracaoProgramacaoCarga => consultaConfiguracaoTipoOperacao.Any(configuracaoTipoOperacao => configuracaoTipoOperacao.ConfiguracaoProgramacaoCarga.Codigo == configuracaoProgramacaoCarga.Codigo));
            }

            return consultaConfiguracaoProgramacaoCarga;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public (int Codigo, int CodigoFilial) BuscarPorCodigoParaSugestao(int codigo)
        {
            var consultaConfiguracaoProgramacaoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga>()
                .Where(configuracaoProgramacaoCarga => configuracaoProgramacaoCarga.Codigo == codigo);

            return consultaConfiguracaoProgramacaoCarga
                .Select(configuracaoProgramacaoCarga => ValueTuple.Create(configuracaoProgramacaoCarga.Codigo, configuracaoProgramacaoCarga.Filial.Codigo))
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaConfiguracaoProgramacaoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaConfiguracaoProgramacaoCarga = Consultar(filtrosPesquisa);

            consultaConfiguracaoProgramacaoCarga = consultaConfiguracaoProgramacaoCarga
                .Fetch(o => o.Filial);

            return ObterLista(consultaConfiguracaoProgramacaoCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaConfiguracaoProgramacaoCarga filtrosPesquisa)
        {
            var consultaConfiguracaoProgramacaoCarga = Consultar(filtrosPesquisa);

            return consultaConfiguracaoProgramacaoCarga.Count();
        }

        #endregion Métodos Públicos
    }
}
