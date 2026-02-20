using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoAgregado
{
    public class AprovacaoAlcadaPagamentoAgregado : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>
    {
        public AprovacaoAlcadaPagamentoAgregado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> BuscarPorPagamentoEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public bool VerificarSePodeAprovar(int codigoPagamento, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.PagamentoAgregado.Codigo == codigoPagamento
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> BuscarPorPagamentoUsuarioSituacao(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();
            var result = from obj in query
                         where
                            obj.PagamentoAgregado.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario &&
                            obj.Situacao == situacao
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado> BuscarRegrasPagamento(int codigoPagamento)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado>();

            var resultGroup = from obj in queryGroup where obj.PagamentoAgregado.Codigo == codigoPagamento select obj.RegraPagamentoAgregado;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        public int ContarRejeitadas(int codigoPagamento, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();
            var resut = from obj in query
                        where
                            obj.PagamentoAgregado.Codigo == codigoPagamento
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                            && (obj.RegraPagamentoAgregado.Codigo == codigoRegra || obj.RegraPagamentoAgregado == null)
                        select obj;

            return resut.Count();
        }

        public int ContarPendentes(int codigoPagamento, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();
            var resut = from obj in query
                        where
                            obj.PagamentoAgregado.Codigo == codigoPagamento
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                            && (obj.RegraPagamentoAgregado.Codigo == codigoRegra || obj.RegraPagamentoAgregado == null)
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesSolicitacao(int codigoPagamento, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();
            var resut = from obj in query
                        where
                            obj.PagamentoAgregado.Codigo == codigoPagamento
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                            && (obj.RegraPagamentoAgregado.Codigo == codigoRegra || obj.RegraPagamentoAgregado == null)
                        select obj;

            return resut.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> _Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao, int numero, double cnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.PagamentoAgregado;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros
            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date <= dataFinal);

            if (situacao.HasValue)
                result = result.Where(obj => obj.Situacao == situacao.Value);

            if (cnpjCliente > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == cnpjCliente);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.PagamentoAgregadoAutorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // O filtro que mais influencia é o de situacao da ocorrencia, pois AgAprovacao, AgAutorizacaoEmissao e AutorizacaoPendente tem o mesmo comportamento
            bool situacaoPendentes = situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.AgAprovacao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacaoAutorizacaoPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente;

            if (situacaoPendentes)
                result = result.Where(obj => obj.PagamentoAgregadoAutorizacoes.Any(aut => usuario > 0 ? (aut.Usuario.Codigo == usuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao, int numero, double cnpjCliente, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(usuario, dataInicial, dataFinal, situacao, numero, cnpjCliente);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao, int numero, double cnpjCliente)
        {
            var result = _Consultar(usuario, dataInicial, dataFinal, situacao, numero, cnpjCliente);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> ConsultarAutorizacoesPorPagamento(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;

            return result.Count();
        }
    }
}