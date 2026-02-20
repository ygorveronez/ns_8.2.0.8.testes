using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public class SugestaoProgramacaoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga>
    {
        #region Construtores

        public SugestaoProgramacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaSugestaoProgramacaoCarga filtrosPesquisa)
        {
            var consultaSugestaoProgramacaoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga>();

            if (filtrosPesquisa.CodigoConfiguracaoProgramacaoCarga > 0)
                consultaSugestaoProgramacaoCarga = consultaSugestaoProgramacaoCarga.Where(sugestaoProgramacaoCarga => sugestaoProgramacaoCarga.ConfiguracaoProgramacaoCarga.Codigo == filtrosPesquisa.CodigoConfiguracaoProgramacaoCarga);

            if (filtrosPesquisa.DataProgramacaoInicial.HasValue)
                consultaSugestaoProgramacaoCarga = consultaSugestaoProgramacaoCarga.Where(sugestaoProgramacaoCarga => sugestaoProgramacaoCarga.DataProgramacao >= filtrosPesquisa.DataProgramacaoInicial.Value.Date);

            if (filtrosPesquisa.DataProgramacaoFinal.HasValue)
                consultaSugestaoProgramacaoCarga = consultaSugestaoProgramacaoCarga.Where(sugestaoProgramacaoCarga => sugestaoProgramacaoCarga.DataProgramacao < filtrosPesquisa.DataProgramacaoFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao.Count > 0 )
                consultaSugestaoProgramacaoCarga = consultaSugestaoProgramacaoCarga.Where(sugestaoProgramacaoCarga => filtrosPesquisa.Situacao.Contains(sugestaoProgramacaoCarga.Situacao));

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                consultaSugestaoProgramacaoCarga = consultaSugestaoProgramacaoCarga.Where(sugestaoProgramacaoCarga => filtrosPesquisa.CodigosFilial.Contains(sugestaoProgramacaoCarga.Filial.Codigo));

            if (filtrosPesquisa.CodigosModeloVeicularCarga?.Count > 0)
                consultaSugestaoProgramacaoCarga = consultaSugestaoProgramacaoCarga.Where(sugestaoProgramacaoCarga => filtrosPesquisa.CodigosModeloVeicularCarga.Contains(sugestaoProgramacaoCarga.ModeloVeicularCarga.Codigo));

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaSugestaoProgramacaoCarga = consultaSugestaoProgramacaoCarga.Where(sugestaoProgramacaoCarga => filtrosPesquisa.CodigosTipoCarga.Contains(sugestaoProgramacaoCarga.TipoCarga.Codigo));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaSugestaoProgramacaoCarga = consultaSugestaoProgramacaoCarga.Where(sugestaoProgramacaoCarga => filtrosPesquisa.CodigosTipoOperacao.Contains(sugestaoProgramacaoCarga.TipoOperacao.Codigo));

            return consultaSugestaoProgramacaoCarga;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<(int Codigo, string ModeloveicularCarga, string TipoCarga, decimal QuantidadeSugerida, int QuantidadeValidada)> BuscarParaResumo(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaSugestaoProgramacaoCarga filtrosPesquisa)
        {
            var consultaSugestaoProgramacaoCarga = Consultar(filtrosPesquisa);

            return consultaSugestaoProgramacaoCarga
                .Select(sugestao => ValueTuple.Create(sugestao.Codigo, sugestao.ModeloVeicularCarga.Descricao, sugestao.TipoCarga.Descricao, sugestao.Quantidade, sugestao.QuantidadeValidada))
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaSugestaoProgramacaoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSugestaoProgramacaoCarga = Consultar(filtrosPesquisa);

            consultaSugestaoProgramacaoCarga = consultaSugestaoProgramacaoCarga
                .Fetch(o => o.ConfiguracaoProgramacaoCarga)
                .Fetch(o => o.Filial)
                .Fetch(o => o.TipoCarga)
                .Fetch(o => o.TipoOperacao);

            return ObterLista(consultaSugestaoProgramacaoCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaSugestaoProgramacaoCarga filtrosPesquisa)
        {
            var consultaSugestaoProgramacaoCarga = Consultar(filtrosPesquisa);

            return consultaSugestaoProgramacaoCarga.Count();
        }

        public bool ExistePorDataProgramacao(int codigoConfiguracaoProgramacaoCarga, DateTime dataProgramacao)
        {
            var consultaSugestaoProgramacaoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga>()
                .Where(sugestaoProgramacaoCarga =>
                    sugestaoProgramacaoCarga.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga &&
                    sugestaoProgramacaoCarga.DataProgramacao.Date == dataProgramacao.Date &&
                    sugestaoProgramacaoCarga.Situacao != SituacaoSugestaoProgramacaoCarga.Cancelada
                );

            return consultaSugestaoProgramacaoCarga.Count() > 0;
        }

        public bool ExistePorDataProgramacao(int codigoFilial, int codigoModeloVeicularCarga, int codigoTipoCarga, int codigoTipoOperacao, DateTime dataProgramacao)
        {
            var consultaSugestaoProgramacaoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga>()
                .Where(sugestaoProgramacaoCarga =>
                    sugestaoProgramacaoCarga.Filial.Codigo == codigoFilial &&
                    sugestaoProgramacaoCarga.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga &&
                    sugestaoProgramacaoCarga.TipoCarga.Codigo == codigoTipoCarga &&
                    sugestaoProgramacaoCarga.TipoOperacao.Codigo == codigoTipoOperacao &&
                    sugestaoProgramacaoCarga.DataProgramacao.Date == dataProgramacao.Date &&
                    sugestaoProgramacaoCarga.Situacao != SituacaoSugestaoProgramacaoCarga.Cancelada
                );

            return consultaSugestaoProgramacaoCarga.Count() > 0;
        }

        #endregion Métodos Públicos
    }
}
