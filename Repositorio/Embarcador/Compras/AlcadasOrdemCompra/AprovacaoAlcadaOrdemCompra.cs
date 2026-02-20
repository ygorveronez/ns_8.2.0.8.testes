using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras.AlcadasOrdemCompra
{
    public class AprovacaoAlcadaOrdemCompra : RepositorioBase<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>
    {
        public AprovacaoAlcadaOrdemCompra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool VerificarSePodeAprovar(int codigoOrdem, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.OrdemCompra.Codigo == codigoOrdem
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> ConsultarAutorizacoesPorOrdem(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var result = from obj in query where obj.OrdemCompra.Codigo == codigo /*&& ((obj.OrdemCompra.DataAlteracao == null) || obj.DataCriacao >= obj.OrdemCompra.DataAlteracao)*/ select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorOrdem(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var result = from obj in query where obj.OrdemCompra.Codigo == codigo /*&& ((obj.OrdemCompra.DataAlteracao == null) || obj.DataCriacao >= obj.OrdemCompra.DataAlteracao)*/ select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> BuscarPorOrdemUsuarioSituacao(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var result = from obj in query
                         where
                            obj.OrdemCompra.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario &&
                            obj.Situacao == situacao
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> BuscarPorOrdemEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var result = from obj in query
                         where
                             obj.OrdemCompra.Codigo == codigo
                         /*&& ((obj.OrdemCompra.DataAlteracao == null) || obj.DataCriacao >= obj.OrdemCompra.DataAlteracao)*/
                         select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public int ContarRejeitadas(int codigoOrdem, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var resut = from obj in query
                        where
                            obj.OrdemCompra.Codigo == codigoOrdem
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                            && (obj.RegraOrdemCompra.Codigo == codigoRegra || obj.RegraOrdemCompra == null)
                        /*&& ((obj.OrdemCompra.DataAlteracao == null) || obj.DataCriacao >= obj.OrdemCompra.DataAlteracao)*/
                        select obj;

            return resut.Count();
        }

        public int ContarPendentes(int codigoOrdem, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var resut = from obj in query
                        where
                            obj.OrdemCompra.Codigo == codigoOrdem
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                            && (obj.RegraOrdemCompra.Codigo == codigoRegra || obj.RegraOrdemCompra == null)
                        /*&& ((obj.OrdemCompra.DataAlteracao == null) || obj.DataCriacao >= obj.OrdemCompra.DataAlteracao)*/
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesSolicitacao(int codigoOrdem, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var resut = from obj in query
                        where
                            obj.OrdemCompra.Codigo == codigoOrdem
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                            && (obj.RegraOrdemCompra.Codigo == codigoRegra || obj.RegraOrdemCompra == null)
                        /*&& ((obj.OrdemCompra.DataAlteracao == null) || obj.DataCriacao >= obj.OrdemCompra.DataAlteracao)*/
                        select obj;

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> BuscarRegrasOrdem(int codigoOrdem)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra>();

            var resultGroup = from obj in queryGroup
                              where obj.OrdemCompra.Codigo == codigoOrdem
                              /*&& ((obj.OrdemCompra.DataAlteracao == null) || obj.DataCriacao >= obj.OrdemCompra.DataAlteracao)*/
                              select obj.RegraOrdemCompra;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> Consultar(int codigoEmpresa, int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra? situacao, int numero, double fornecedor, int operador, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, fornecedor, operador);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra? situacao, int numero, double fornecedor, int operador)
        {
            var result = _Consultar(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, fornecedor, operador);

            return result.Count();
        }

        public int ContarAprovacoesNecessarias(int codigo)
        {
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra>();

            var resutAutorizacao = from aut in queryAutorizacao where aut.OrdemCompra.Codigo == codigo /*&& ((aut.OrdemCompra.DataAlteracao == null) || aut.DataCriacao >= aut.OrdemCompra.DataAlteracao)*/ select aut.RegraOrdemCompra;
            var resut = from obj in query where resutAutorizacao.Contains(obj) select obj;

            return resut.Sum(o => (int?)o.NumeroAprovadores) ?? 0;
        }

        public int ContarAprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var resut = from obj in query
                        where
                            obj.OrdemCompra.Codigo == codigo
                            /*&& ((obj.OrdemCompra.DataAlteracao == null) || obj.DataCriacao >= obj.OrdemCompra.DataAlteracao)*/
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                        select obj;

            return resut.Count();
        }

        public int ContarReprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var resut = from obj in query
                        where
                            obj.OrdemCompra.Codigo == codigo
                            /*&& ((obj.OrdemCompra.DataAlteracao == null) || obj.DataCriacao >= obj.OrdemCompra.DataAlteracao)*/
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                        select obj;

            return resut.Count();
        }

        public List<Dominio.Entidades.Usuario> BuscarAprovadores(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();
            var result = from obj in query where obj.OrdemCompra.Codigo == codigo && obj.Usuario != null && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada select obj;

            return result.Select(o => o.Usuario).ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Compras.OrdemCompra> _Consultar(int codigoEmpresa, int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra? situacao, int numero, double fornecedor, int operador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.OrdemCompra;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFinal);

            if (situacao.HasValue)
                result = result.Where(obj => obj.Situacao == situacao.Value);

            if (fornecedor > 0)
                result = result.Where(obj => obj.Fornecedor.CPF_CNPJ == fornecedor);

            if (operador > 0)
                result = result.Where(obj => obj.Usuario.Codigo == operador);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.Autorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.Autorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // O filtro que mais influencia é o de situacao da ocorrencia, pois AgAprovacao, AgAutorizacaoEmissao e AutorizacaoPendente tem o mesmo comportamento
            bool situacaoPendentes = situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.AgAprovacao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacaoAutorizacaoPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente;

            if (situacaoPendentes)
                result = result.Where(obj => obj.Autorizacoes.Any(aut => usuario > 0 ? (aut.Usuario.Codigo == usuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));


            return result;
        }

        #endregion
    }
}
