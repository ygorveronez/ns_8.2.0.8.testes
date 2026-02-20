using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class AprovacaoAlcadaRequisicaoMercadoria : RepositorioBase<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>
    {
        public AprovacaoAlcadaRequisicaoMercadoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool VerificarSePodeAprovar(int codigoRequisicao, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.RequisicaoMercadoria.Codigo == codigoRequisicao
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> ConsultarAutorizacoesPorRequisicao(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var result = from obj in query where obj.RequisicaoMercadoria.Codigo == codigo && ((obj.RequisicaoMercadoria.DataAlteracao == null) || obj.DataCriacao >= obj.RequisicaoMercadoria.DataAlteracao) select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorRequisicao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var result = from obj in query where obj.RequisicaoMercadoria.Codigo == codigo && ((obj.RequisicaoMercadoria.DataAlteracao == null) || obj.DataCriacao >= obj.RequisicaoMercadoria.DataAlteracao) select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> BuscarPorRequisicaoUsuarioSituacao(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var result = from obj in query
                         where
                            obj.RequisicaoMercadoria.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario &&
                            obj.Situacao == situacao
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> BuscarPorRequisicaoEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var result = from obj in query
                         where
                             obj.RequisicaoMercadoria.Codigo == codigo
                             && ((obj.RequisicaoMercadoria.DataAlteracao == null) || obj.DataCriacao >= obj.RequisicaoMercadoria.DataAlteracao)
                         select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public int ContarRejeitadas(int codigoRequisicao, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var resut = from obj in query
                        where
                            obj.RequisicaoMercadoria.Codigo == codigoRequisicao
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                            && (obj.RegraRequisicaoMercadoria.Codigo == codigoRegra || obj.RegraRequisicaoMercadoria == null)
                            && ((obj.RequisicaoMercadoria.DataAlteracao == null) || obj.DataCriacao >= obj.RequisicaoMercadoria.DataAlteracao)
                        select obj;

            return resut.Count();
        }

        public int ContarPendentes(int codigoRequisicao, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var resut = from obj in query
                        where
                            obj.RequisicaoMercadoria.Codigo == codigoRequisicao
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                            && (obj.RegraRequisicaoMercadoria.Codigo == codigoRegra || obj.RegraRequisicaoMercadoria == null)
                            && ((obj.RequisicaoMercadoria.DataAlteracao == null) || obj.DataCriacao >= obj.RequisicaoMercadoria.DataAlteracao)
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesSolicitacao(int codigoRequisicao, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var resut = from obj in query
                        where
                            obj.RequisicaoMercadoria.Codigo == codigoRequisicao
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                            && (obj.RegraRequisicaoMercadoria.Codigo == codigoRegra || obj.RegraRequisicaoMercadoria == null)
                            && ((obj.RequisicaoMercadoria.DataAlteracao == null) || obj.DataCriacao >= obj.RequisicaoMercadoria.DataAlteracao)
                        select obj;

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> BuscarRegrasRequisicao(int codigoRequisicao)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria>();

            var resultGroup = from obj in queryGroup
                              where obj.RequisicaoMercadoria.Codigo == codigoRequisicao
       && ((obj.RequisicaoMercadoria.DataAlteracao == null) || obj.DataCriacao >= obj.RequisicaoMercadoria.DataAlteracao)
                              select obj.RegraRequisicaoMercadoria;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> _Consultar(int codigoEmpresa, int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria? situacao, int numero, int filial, int motivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.RequisicaoMercadoria;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Filial.Codigo == codigoEmpresa);

            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFinal);

            if (situacao.HasValue)
                result = result.Where(obj => obj.Situacao == situacao.Value);

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);

            if (motivo > 0)
                result = result.Where(obj => obj.MotivoCompra.Codigo == motivo);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.Autorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.Autorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // O filtro que mais influencia é o de situacao da ocorrencia, pois AgAprovacao, AgAutorizacaoEmissao e AutorizacaoPendente tem o mesmo comportamento
            bool situacaoPendentes = situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.AgAprovacao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacaoAutorizacaoPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente;

            if (situacaoPendentes)
                result = result.Where(obj => obj.Autorizacoes.Any(aut => usuario > 0 ? (aut.Usuario.Codigo == usuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));


            return result;
        }

        public List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> Consultar(int codigoEmpresa, int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria? situacao, int numero, int filial, int motivo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, filial, motivo);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria? situacao, int numero, int filial, int motivo)
        {
            var result = _Consultar(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, filial, motivo);

            return result.Count();
        }

        public int ContarAprovacoesNecessarias(int codigo)
        {
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria>();

            var resutAutorizacao = from aut in queryAutorizacao where aut.RequisicaoMercadoria.Codigo == codigo && ((aut.RequisicaoMercadoria.DataAlteracao == null) || aut.DataCriacao >= aut.RequisicaoMercadoria.DataAlteracao) select aut.RegraRequisicaoMercadoria;
            var resut = from obj in query where resutAutorizacao.Contains(obj) select obj;

            return resut.Sum(o => (int?)o.NumeroAprovadores) ?? 0;
        }

        public int ContarAprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var resut = from obj in query
                        where
                            obj.RequisicaoMercadoria.Codigo == codigo
                            && ((obj.RequisicaoMercadoria.DataAlteracao == null) || obj.DataCriacao >= obj.RequisicaoMercadoria.DataAlteracao)
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                        select obj;

            return resut.Count();
        }

        public int ContarReprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();
            var resut = from obj in query
                        where
                            obj.RequisicaoMercadoria.Codigo == codigo
                            && ((obj.RequisicaoMercadoria.DataAlteracao == null) || obj.DataCriacao >= obj.RequisicaoMercadoria.DataAlteracao)
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                        select obj;

            return resut.Count();
        }
    }
}
