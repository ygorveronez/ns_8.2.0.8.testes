using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PreCargas
{
    public class PreCargaOfertaTransportador : RepositorioBase<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>
    {
        #region Construtores

        public PreCargaOfertaTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCargaOfertaTransportador filtrosPesquisa)
        {
            var consultaOfertaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>()
                .Where(ofertaTransportador => ofertaTransportador.Transportador.Codigo == filtrosPesquisa.CodigoTransportador && ofertaTransportador.Bloqueada == false);

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaOfertaTransportador = consultaOfertaTransportador.Where(ofertaTransportador => ofertaTransportador.PreCargaOferta.PreCarga.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);

            if (filtrosPesquisa.CodigoTipoCarga > 0)
                consultaOfertaTransportador = consultaOfertaTransportador.Where(ofertaTransportador => ofertaTransportador.PreCargaOferta.PreCarga.TipoDeCarga.Codigo == filtrosPesquisa.CodigoTipoCarga);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaOfertaTransportador = consultaOfertaTransportador.Where(ofertaTransportador => ofertaTransportador.PreCargaOferta.PreCarga.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaOfertaTransportador = consultaOfertaTransportador.Where(ofertaTransportador => ofertaTransportador.PreCargaOferta.PreCarga.DataPrevisaoEntrega >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaOfertaTransportador = consultaOfertaTransportador.Where(ofertaTransportador => ofertaTransportador.PreCargaOferta.PreCarga.DataPrevisaoEntrega <= filtrosPesquisa.DataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPreCarga))
                consultaOfertaTransportador = consultaOfertaTransportador.Where(ofertaTransportador => ofertaTransportador.PreCargaOferta.PreCarga.NumeroPreCarga == filtrosPesquisa.NumeroPreCarga);

            return consultaOfertaTransportador;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<int> BuscarCodigosPorTempoAceiteEncerrado(int limiteRegistros)
        {
            var consultaPreCargaOfertaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>()
                .Where(o =>
                    o.HorarioLimiteConfirmacao < DateTime.Now &&
                    o.Situacao == SituacaoPreCargaOfertaTransportador.AguardandoAceite
                );

            return consultaPreCargaOfertaTransportador
                .Select(o => o.Codigo)
                .Take(limiteRegistros)
                .ToList();
        }

        public List<int> BuscarCodigosTransportadoresRejeitaramOferta(int codigoOferta, TipoPreCargaOfertaTransportador tipo)
        {
            var consultaPreCargaOfertaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>()
                .Where(ofertaTransportador =>
                    ofertaTransportador.PreCargaOferta.Codigo == codigoOferta &&
                    ofertaTransportador.Situacao == SituacaoPreCargaOfertaTransportador.Rejeitada &&
                    ofertaTransportador.Tipo == tipo
                );

            return consultaPreCargaOfertaTransportador.Select(ofertaTransportador => ofertaTransportador.Transportador.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador BuscarPorOfertaETransportador(int codigoOferta, int codigoTransportador)
        {
            var consultaPreCargaOfertaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>()
                .Where(ofertaTransportador => ofertaTransportador.PreCargaOferta.Codigo == codigoOferta && ofertaTransportador.Transportador.Codigo == codigoTransportador);

            return consultaPreCargaOfertaTransportador.FirstOrDefault();
        }

        public void BloquearTodas(int codigoOferta, int codigoOfertaTransportadorDesconsiderar, TipoPreCargaOfertaTransportador tipo)
        {
            UnitOfWork.Sessao
                .CreateQuery(@"
                    update PreCargaOfertaTransportador
                       set Bloqueada = 1
                     where PreCargaOferta.Codigo = :codigoOferta
                       and Codigo <> :codigoOfertaTransportadorDesconsiderar
                       and Tipo = :tipo
                       and Situacao <> :situacao"
                )
                .SetInt32("codigoOferta", codigoOferta)
                .SetInt32("codigoOfertaTransportadorDesconsiderar", codigoOfertaTransportadorDesconsiderar)
                .SetEnum("tipo", tipo)
                .SetEnum("situacao", SituacaoPreCargaOfertaTransportador.Rejeitada)
                .ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCargaOfertaTransportador filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOfertaTransportador = Consultar(filtrosPesquisa);

            consultaOfertaTransportador = consultaOfertaTransportador
                .Fetch(o => o.PreCargaOferta).ThenFetch(oferta => oferta.PreCarga).ThenFetch(preCarga => preCarga.Filial)
                .Fetch(o => o.PreCargaOferta).ThenFetch(oferta => oferta.PreCarga).ThenFetch(preCarga => preCarga.ModeloVeicularCarga)
                .Fetch(o => o.PreCargaOferta).ThenFetch(oferta => oferta.PreCarga).ThenFetch(preCarga => preCarga.TipoDeCarga)
                .Fetch(o => o.PreCargaOferta).ThenFetch(oferta => oferta.PreCarga).ThenFetch(preCarga => preCarga.TipoOperacao);

            if (parametrosConsulta.InicioRegistros > 0)
                consultaOfertaTransportador = consultaOfertaTransportador.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                consultaOfertaTransportador = consultaOfertaTransportador.Take(parametrosConsulta.LimiteRegistros);

            return consultaOfertaTransportador.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador> ConsultarPorPreCarga(int codigoPreCarga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOfertaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>()
                .Where(ofertaTransportador => ofertaTransportador.PreCargaOferta.PreCarga.Codigo == codigoPreCarga);

            consultaOfertaTransportador = consultaOfertaTransportador
                .Fetch(o => o.Transportador).ThenFetch(transportador => transportador.Localidade).ThenFetch(localidade => localidade.Estado)
                .Fetch(o => o.Transportador).ThenFetch(transportador => transportador.Localidade).ThenFetch(localidade => localidade.Pais);

            if (parametrosConsulta.InicioRegistros > 0)
                consultaOfertaTransportador = consultaOfertaTransportador.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                consultaOfertaTransportador = consultaOfertaTransportador.Take(parametrosConsulta.LimiteRegistros);

            return consultaOfertaTransportador.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCargaOfertaTransportador filtrosPesquisa)
        {
            var consultaOfertaTransportador = Consultar(filtrosPesquisa);

            return consultaOfertaTransportador.Count();
        }

        public int ContarConsultaPorPreCarga(int codigoPreCarga)
        {
            var consultaOfertaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>()
                .Where(ofertaTransportador => ofertaTransportador.PreCargaOferta.PreCarga.Codigo == codigoPreCarga);

            return consultaOfertaTransportador.Count();
        }

        public void DesbloquearTodas(int codigoOferta, TipoPreCargaOfertaTransportador tipo)
        {
            UnitOfWork.Sessao
                .CreateQuery(@"
                    update PreCargaOfertaTransportador
                       set Bloqueada = 0
                     where PreCargaOferta.Codigo = :codigoOferta
                       and Tipo = :tipo
                       and Bloqueada = 1"
                )
                .SetInt32("codigoOferta", codigoOferta)
                .SetEnum("tipo", tipo)
                .ExecuteUpdate();
        }

        public void RejeitarTodas(int codigoOferta)
        {
            UnitOfWork.Sessao
                .CreateSQLQuery(
                    $@"insert into T_PRE_CARGA_OFERTA_TRANSPORTADOR_HISTORICO(POT_CODIGO, PTH_DATA, PTH_DESCRICAO, PTH_TIPO)
                       select POT_CODIGO, getdate(), 'Interesse no pré planejamento rejeitado para o transportador', {(int)TipoPreCargaOfertaTransportadorHistorico.RegistroAlteracao}
                         from T_PRE_CARGA_OFERTA_TRANSPORTADOR
                        where PCO_CODIGO = {codigoOferta}
                          and POT_SITUACAO <> {(int)SituacaoPreCargaOfertaTransportador.Rejeitada}"
                )
                .ExecuteUpdate();

            UnitOfWork.Sessao
                .CreateQuery(@"
                    update PreCargaOfertaTransportador
                       set Situacao = :situacao,
                           HorarioLimiteConfirmacao = null
                     where PreCargaOferta.Codigo = :codigoOferta"
                )
                .SetInt32("codigoOferta", codigoOferta)
                .SetEnum("situacao", SituacaoPreCargaOfertaTransportador.Rejeitada)
                .ExecuteUpdate();
        }

        public bool VerificarExiste(int codigoOferta, TipoPreCargaOfertaTransportador tipo)
        {
            var consultaPreCargaOfertaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>()
                .Where(o =>
                    o.PreCargaOferta.Codigo == codigoOferta &&
                    o.Tipo == tipo
                );

            return consultaPreCargaOfertaTransportador.Count() > 0;
        }

        public bool VerificarExisteBloqueadas(int codigoOferta, TipoPreCargaOfertaTransportador tipo)
        {
            var consultaPreCargaOfertaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>()
                .Where(o =>
                    o.PreCargaOferta.Codigo == codigoOferta &&
                    o.Tipo == tipo &&
                    o.Bloqueada == true
                );

            return consultaPreCargaOfertaTransportador.Count() > 0;
        }

        public bool VerificarExisteDesbloqueadasSemRejeicao(int codigoOferta, TipoPreCargaOfertaTransportador tipo)
        {
            var consultaPreCargaOfertaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>()
                .Where(o =>
                    o.PreCargaOferta.Codigo == codigoOferta &&
                    o.Situacao != SituacaoPreCargaOfertaTransportador.Rejeitada &&
                    o.Tipo == tipo &&
                    o.Bloqueada == false
                );

            return consultaPreCargaOfertaTransportador.Count() > 0;
        }

        #endregion Métodos Públicos
    }
}
