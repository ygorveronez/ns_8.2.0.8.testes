using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class PagamentoMotoristaAutorizacao : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>
    {
        public PagamentoMotoristaAutorizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao BuscarAutorizacaoPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigoPagamento select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> ConsultarAutorizacoesPorPagamento(int codigoPagamento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var result = from obj in query
                         where obj.PagamentoMotoristaTMS.Codigo == codigoPagamento
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarAutorizacoesPorPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigoPagamento select obj;


            return result.Count();
        }

        public int ContarRejeitadas(int codigoPagamento, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var resut = from obj in query
                        where
                            obj.PagamentoMotoristaTMS.Codigo == codigoPagamento
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada
                            && (obj.RegrasPagamentoMotorista.Codigo == codigoRegra || obj.RegrasPagamentoMotorista == null)
                        select obj;

            return resut.Count();
        }



        public int ContarPendentes(int codigoPagamento, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var resut = from obj in query
                        where
                            obj.PagamentoMotoristaTMS.Codigo == codigoPagamento
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente
                            && (obj.RegrasPagamentoMotorista.Codigo == codigoRegra || obj.RegrasPagamentoMotorista == null)
                        select obj;

            return resut.Count();
        }


        public int ContarAprovacoesOcorrencia(int codigoPagamento, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var resut = from obj in query
                        where
                            obj.PagamentoMotoristaTMS.Codigo == codigoPagamento
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada
                            && (obj.RegrasPagamentoMotorista.Codigo == codigoRegra || obj.RegrasPagamentoMotorista == null)
                        select obj;

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> BuscarRegrasPagamento(int codigoOcorrencia)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista>();

            var resultGroup = from obj in queryGroup where obj.PagamentoMotoristaTMS.Codigo == codigoOcorrencia select obj.RegrasPagamentoMotorista;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> BuscarPorPagamentoUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigo select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao BuscarPrimeiroPorPagamentoUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigo select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.FirstOrDefault();
        }

        public bool VerificarSePodeAprovar(int codigoPagamento, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.PagamentoMotoristaTMS.Codigo == codigoPagamento
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> BuscarPendentesPorPagamentoEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var result = from obj in query
                         where
                            obj.PagamentoMotoristaTMS.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario &&
                            obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> ResponsavelOcorrencia(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();
            var queryUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigoPagamento select obj.Usuario;
            var resultUsuario = from obj in queryUsuario where result.Contains(obj) select obj;


            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> Consultar(int usuario, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista situacao, int numeroPagamentoMotorista, string codigoCarga, List<int> codigosTipoPagamento, int codigoMotorista, int codigoCentroResultado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(usuario, dataInicio, dataFim, situacao, numeroPagamentoMotorista, codigoCarga, codigosTipoPagamento, codigoMotorista, codigoCentroResultado);

            result = result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int usuario, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista situacao, int numeroPagamentoMotorista, string codigoCarga, List<int> codigosTipoPagamento, int codigoMotorista, int codigoCentroResultado)
        {
            var result = _Consultar(usuario, dataInicio, dataFim, situacao, numeroPagamentoMotorista, codigoCarga, codigosTipoPagamento, codigoMotorista, codigoCentroResultado);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> _Consultar(int usuario, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista situacao, int numeroPagamentoMotorista, string codigoCarga, List<int> codigosTipoPagamento, int codigoMotorista, int codigoCentroResultado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.PagamentoMotoristaTMS;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros da ocorrencia
            if (!string.IsNullOrWhiteSpace(codigoCarga))
                result = result.Where(obj => obj.Carga.CodigoCargaEmbarcador.Equals(codigoCarga));

            if (numeroPagamentoMotorista > 0)
                result = result.Where(obj => obj.Numero == numeroPagamentoMotorista);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date <= dataFim);

            if(codigoCentroResultado > 0)
                result = result.Where(obj => obj.Motorista.CentroResultado.Codigo == codigoCentroResultado);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Todas && situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente)
                result = result.Where(obj => obj.SituacaoPagamentoMotorista == situacao);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente)
            {
                result = result.Where(o => (o.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgAprovacao || o.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente));
            }

            if (codigosTipoPagamento.Count > 0)
                result = result.Where(obj => codigosTipoPagamento.Contains(obj.PagamentoMotoristaTipo.Codigo));

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            result = result.Where(obj => obj.PagamentoLiberado == true);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.PagamentoMotoristaAutorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // O filtro que mais influcencia é o de situacao da ocorrencia, pois AgAprovacao, AgAutorizacaoEmissao e AutorizacaoPendente tem o mesmo comportamento
            bool situacaoPendentes = situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgAprovacao || situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao situacaoAutorizacaoPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente;

            if (situacaoPendentes)
            {
                result = result.Where(obj => obj.PagamentoMotoristaAutorizacoes.Any(aut => usuario > 0 ? (aut.Usuario.Codigo == usuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));
            }


            return result;
        }

    }
}
