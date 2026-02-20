using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frete
{
    public class AprovacaoAlcadaContratoFreteTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>
    {
        #region Construtores

        public AprovacaoAlcadaContratoFreteTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, SituacaoContratoFreteTransportador? situacao, int numero, int transportador, TipoAprovadorRegra? tipoAprovadorRegra)
        {
            var consultaContratoFreteTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();
            var consultaAprovacaoAlcadaContratoFreteTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>()
                .Where(o => !o.Bloqueada);

            if (numero > 0)
                consultaContratoFreteTransportador = consultaContratoFreteTransportador.Where(obj => obj.NumeroSequencial == numero);

            if (dataInicial != DateTime.MinValue)
                consultaContratoFreteTransportador = consultaContratoFreteTransportador.Where(obj => obj.DataAlteracao >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                consultaContratoFreteTransportador = consultaContratoFreteTransportador.Where(obj => obj.DataAlteracao.Value.Date <= dataFinal);

            if (situacao.HasValue)
                consultaContratoFreteTransportador = consultaContratoFreteTransportador.Where(obj => obj.Situacao == situacao.Value);

            if (transportador > 0)
                consultaContratoFreteTransportador = consultaContratoFreteTransportador.Where(obj => obj.Transportador.Codigo == transportador);

            if (usuario > 0)
                consultaAprovacaoAlcadaContratoFreteTransportador = consultaAprovacaoAlcadaContratoFreteTransportador.Where(o => o.Usuario.Codigo == usuario);

            if (tipoAprovadorRegra.HasValue)
                consultaAprovacaoAlcadaContratoFreteTransportador = consultaAprovacaoAlcadaContratoFreteTransportador.Where(o => ((TipoAprovadorRegra?)o.TipoAprovadorRegra ?? TipoAprovadorRegra.Usuario) == tipoAprovadorRegra.Value);

            bool situacaoPendentes = situacao == SituacaoContratoFreteTransportador.AgAprovacao;

            if (situacaoPendentes)
            {
                SituacaoAlcadaRegra situacaoAutorizacaoPendente = SituacaoAlcadaRegra.Pendente;

                consultaAprovacaoAlcadaContratoFreteTransportador = consultaAprovacaoAlcadaContratoFreteTransportador.Where(o => o.Situacao == situacaoAutorizacaoPendente);
            }

            return consultaContratoFreteTransportador.Where(o => consultaAprovacaoAlcadaContratoFreteTransportador.Where(a => a.ContratoFreteTransportador.Codigo == o.Codigo).Any());
        }

        public IQueryable<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> ConsultarAltorizacoesDesbloqueadas(int codigoContrato)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>()
                .Where(o =>
                    o.ContratoFreteTransportador.Codigo == codigoContrato &&
                    !o.Bloqueada &&
                    (o.ContratoFreteTransportador.DataAlteracao == null || o.DataCriacao >= o.ContratoFreteTransportador.DataAlteracao)
                );

            return aprovacoes;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool VerificarSePodeAprovar(int codigoContrato, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.ContratoFreteTransportador.Codigo == codigoContrato
                            && obj.Usuario.Codigo == codigoUsuario
                            && !obj.Bloqueada
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> ConsultarAutorizacoesPorContrato(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var result = from obj in query where obj.ContratoFreteTransportador.Codigo == codigo && ((obj.ContratoFreteTransportador.DataAlteracao == null) || obj.DataCriacao >= obj.ContratoFreteTransportador.DataAlteracao) select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorContrato(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var result = from obj in query where obj.ContratoFreteTransportador.Codigo == codigo && ((obj.ContratoFreteTransportador.DataAlteracao == null) || obj.DataCriacao >= obj.ContratoFreteTransportador.DataAlteracao) select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> BuscarPorContratoUsuarioSituacao(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var result = from obj in query
                         where
                            obj.ContratoFreteTransportador.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario &&
                            !obj.Bloqueada &&
                            obj.Situacao == situacao
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> BuscarPorContrato(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var result = from obj in query
                         where
                             obj.ContratoFreteTransportador.Codigo == codigo && !obj.Bloqueada
                             && ((obj.ContratoFreteTransportador.DataAlteracao == null) || obj.DataCriacao >= obj.ContratoFreteTransportador.DataAlteracao)
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> BuscarPorContratoEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var result = from obj in query
                         where
                             obj.ContratoFreteTransportador.Codigo == codigo && !obj.Bloqueada
                             && ((obj.ContratoFreteTransportador.DataAlteracao == null) || obj.DataCriacao >= obj.ContratoFreteTransportador.DataAlteracao)
                         select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> BuscarPendentesPorContrato(int codigoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var result = from obj in query
                         where
                             obj.ContratoFreteTransportador.Codigo == codigoContrato
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                             && obj.Bloqueada
                         select obj;

            return result.ToList();
        }

        public int BuscarNumeroAprovacoesNecessariasPorRegra(int codigoContrato, int codigoRegra)
        {
            var aprovacoes = ConsultarAltorizacoesDesbloqueadas(codigoContrato);

            int numeroAprovacoesNecessarias = aprovacoes
                .Where(aprovacao => aprovacao.RegraContratoFreteTransportador.Codigo == codigoRegra)
                .Select(aprovacao => aprovacao.NumeroAprovadores)
                .FirstOrDefault();

            return numeroAprovacoesNecessarias;
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> BuscarRegrasDesbloqueadas(int codigoContrato)
        {
            var aprovacoes = ConsultarAltorizacoesDesbloqueadas(codigoContrato);

            var regras = aprovacoes
                .Where(aprovacao => aprovacao.RegraContratoFreteTransportador != null)
                .Select(aprovacao => aprovacao.RegraContratoFreteTransportador)
                .Distinct()
                .ToList();

            return regras;
        }

        public int ContarRejeitadas(int codigoContrato, int codigoRegra)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>()
                .Where(o =>
                    o.ContratoFreteTransportador.Codigo == codigoContrato &&
                    o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada &&
                    (o.RegraContratoFreteTransportador.Codigo == codigoRegra || o.RegraContratoFreteTransportador == null) &&
                    (o.ContratoFreteTransportador.DataAlteracao == null || o.DataCriacao >= o.ContratoFreteTransportador.DataAlteracao)
                );

            return aprovacoes.Count();
        }

        public int ContarPendentes(int codigoContrato, int codigoRegra)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>()
                .Where(o =>
                    o.ContratoFreteTransportador.Codigo == codigoContrato &&
                    o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente &&
                    (o.RegraContratoFreteTransportador.Codigo == codigoRegra || o.RegraContratoFreteTransportador == null) &&
                    (o.ContratoFreteTransportador.DataAlteracao == null || o.DataCriacao >= o.ContratoFreteTransportador.DataAlteracao)
                );

            return aprovacoes.Count();
        }

        public int ContarAprovacoesSolicitacao(int codigoContrato, int codigoRegra)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>()
                .Where(o =>
                    o.ContratoFreteTransportador.Codigo == codigoContrato &&
                    o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada &&
                    (o.RegraContratoFreteTransportador.Codigo == codigoRegra || o.RegraContratoFreteTransportador == null) &&
                    (o.ContratoFreteTransportador.DataAlteracao == null || o.DataCriacao >= o.ContratoFreteTransportador.DataAlteracao)
                );

            return aprovacoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> BuscarRegrasContrato(int codigoContrato)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador>();

            var resultGroup = from obj in queryGroup
                              where obj.ContratoFreteTransportador.Codigo == codigoContrato
       && ((obj.ContratoFreteTransportador.DataAlteracao == null) || obj.DataCriacao >= obj.ContratoFreteTransportador.DataAlteracao)
                              select obj.RegraContratoFreteTransportador;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, SituacaoContratoFreteTransportador? situacao, int numero, int transportador, TipoAprovadorRegra? tipoAprovadorRegra, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(usuario, dataInicial, dataFinal, situacao, numero, transportador, tipoAprovadorRegra);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int usuario, DateTime dataInicial, DateTime dataFinal, SituacaoContratoFreteTransportador? situacao, int numero, int transportador, TipoAprovadorRegra? tipoAprovadorRegra)
        {
            var result = Consultar(usuario, dataInicial, dataFinal, situacao, numero, transportador, tipoAprovadorRegra);

            return result.Count();
        }

        public int ContarAprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var resut = from obj in query
                        where
                            obj.ContratoFreteTransportador.Codigo == codigo
                            && ((obj.ContratoFreteTransportador.DataAlteracao == null) || obj.DataCriacao >= obj.ContratoFreteTransportador.DataAlteracao)
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                        select obj;

            return resut.Count();
        }

        public int ContarReprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();
            var resut = from obj in query
                        where
                            obj.ContratoFreteTransportador.Codigo == codigo
                            && ((obj.ContratoFreteTransportador.DataAlteracao == null) || obj.DataCriacao >= obj.ContratoFreteTransportador.DataAlteracao)
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                        select obj;

            return resut.Count();
        }

        #endregion
    }
}
