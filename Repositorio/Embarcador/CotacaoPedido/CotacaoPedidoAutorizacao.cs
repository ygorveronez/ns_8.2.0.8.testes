using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CotacaoPedido
{
    public class CotacaoPedidoAutorizacao : RepositorioBase<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>
    {
        public CotacaoPedidoAutorizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao BuscarAutorizacaoPedido(long codigoCotacaoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var result = from obj in query where obj.CotacaoPedido.Codigo == codigoCotacaoPedido select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> ConsultarAutorizacoesPorPedido(long codigoCotacaoPedido, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var result = from obj in query
                         where obj.CotacaoPedido.Codigo == codigoCotacaoPedido
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarAutorizacoesPorPedido(long codigoCotacaoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var result = from obj in query where obj.CotacaoPedido.Codigo == codigoCotacaoPedido select obj;


            return result.Count();
        }

        public int ContarRejeitadas(long codigoCotacaoPedido, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var resut = from obj in query
                        where
                            obj.CotacaoPedido.Codigo == codigoCotacaoPedido
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada
                            && (obj.RegrasCotacaoPedido.Codigo == codigoRegra || obj.RegrasCotacaoPedido == null)
                        select obj;

            return resut.Count();
        }



        public int ContarPendentes(long codigoCotacaoPedido, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var resut = from obj in query
                        where
                            obj.CotacaoPedido.Codigo == codigoCotacaoPedido
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente
                            && (obj.RegrasCotacaoPedido.Codigo == codigoRegra || obj.RegrasCotacaoPedido == null)
                        select obj;

            return resut.Count();
        }


        public int ContarAprovacoesOcorrencia(long codigoCotacaoPedido, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var resut = from obj in query
                        where
                            obj.CotacaoPedido.Codigo == codigoCotacaoPedido
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada
                            && (obj.RegrasCotacaoPedido.Codigo == codigoRegra || obj.RegrasCotacaoPedido == null)
                        select obj;

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> BuscarRegrasPedido(long codigoOcorrencia)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido>();

            var resultGroup = from obj in queryGroup where obj.CotacaoPedido.Codigo == codigoOcorrencia select obj.RegrasCotacaoPedido;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> BuscarPorPedidoUsuario(long codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var result = from obj in query where obj.CotacaoPedido.Codigo == codigo select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public bool VerificarSePodeAprovar(long codigoCotacaoPedido, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.CotacaoPedido.Codigo == codigoCotacaoPedido
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> BuscarPendentesPorPedidoEUsuario(long codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
            var result = from obj in query
                         where
                            obj.CotacaoPedido.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario &&
                            obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente
                         select obj;
            return result.ToList();
        }

        //public List<Dominio.Entidades.Usuario> ResponsavelOcorrencia(int codigoCotacaoPedido)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();
        //    var queryUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

        //    var result = from obj in query where obj.CotacaoPedido.Codigo == codigoCotacaoPedido select obj.Usuario;
        //    var resultUsuario = from obj in queryUsuario where result.Contains(obj) select obj;


        //    return result.ToList();
        //}

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> Consultar(int usuario, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao, int numeroPedidos, int codigoGrupoPessoa, int codigoTipoCarga, int codigoTipoOperacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(usuario, dataInicio, dataFim, situacao, numeroPedidos, codigoGrupoPessoa, codigoTipoCarga, codigoTipoOperacao);

            result = result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int usuario, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao, int numeroPedidos, int codigoGrupoPessoa, int codigoTipoCarga, int codigoTipoOperacao)
        {
            var result = _Consultar(usuario, dataInicio, dataFim, situacao, numeroPedidos, codigoGrupoPessoa, codigoTipoCarga, codigoTipoOperacao);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> _Consultar(int usuario, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao, int numeroPedidos, int codigoGrupoPessoa, int codigoTipoCarga, int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.CotacaoPedido;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros da ocorrencia
            if (codigoGrupoPessoa > 0)
                result = result.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupoPessoa);

            if (numeroPedidos > 0)
                result = result.Where(obj => obj.Numero == numeroPedidos);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Data <= dataFim);

            if ((int)situacao > 0)
            {
                if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente)
                    result = result.Where(obj => obj.SituacaoPedido == situacao);
                else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente)
                {
                    result = result.Where(o => (o.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || o.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente));
                }
            }

            if (codigoTipoCarga > 0)
                result = result.Where(obj => obj.TipoDeCarga.Codigo == codigoTipoCarga);

            if (codigoTipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == codigoTipoOperacao);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.CotacaoPedidoAutorizacao.Any(aut => aut.Usuario.Codigo == usuario));

            // O filtro que mais influcencia é o de situacao da ocorrencia, pois AgAprovacao, AgAutorizacaoEmissao e AutorizacaoPendente tem o mesmo comportamento
            bool situacaoPendentes = situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao situacaoAutorizacaoPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente;

            if (situacaoPendentes)
            {
                result = result.Where(obj => obj.CotacaoPedidoAutorizacao.Any(aut => usuario > 0 ? (aut.Usuario.Codigo == usuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));
            }


            return result;
        }
    }
}
