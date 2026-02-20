using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NFS
{
    public class LancamentoNFSAutorizacao : RepositorioBase<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>
    {
        #region Construtores

        public LancamentoNFSAutorizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> _Consultar(int numero, int filial, int transportador, double tomador, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual situacao, DateTime dataInicial, DateTime dataFinal, string codigoCargaEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>()
                .Where(obj => !obj.Bloqueada);

            var resultAutorizacao = from obj in queryAutorizacao select obj.LancamentoNFSManual;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros da ocorrencia
            if (numero > 0)
                result = result.Where(obj => obj.DadosNFS.Numero == numero);

            if (!string.IsNullOrEmpty(codigoCargaEmbarcador))
            {
                result = result.Where(obj => obj.Documentos.Any(nfse => nfse.Carga.CodigoCargaEmbarcador == codigoCargaEmbarcador));
            }

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);

            if (transportador > 0)
                result = result.Where(obj => obj.Transportador.Codigo == transportador);

            if (tomador > 0)
                result = result.Where(obj => obj.Tomador.CPF_CNPJ == tomador);
                
            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataCriacao.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataCriacao.Date <= dataFinal);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Todas)
                result = result.Where(obj => obj.Situacao == situacao);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.LancamentoNFSAutorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            bool situacaoPendente = situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgAprovacao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada situacaoAlcadaPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Pendente;

            if (situacaoPendente)
                result = result.Where(obj => obj.LancamentoNFSAutorizacoes.Any(aut => usuario > 0 ? (aut.Usuario.Codigo == usuario && aut.Situacao == situacaoAlcadaPendente) : (aut.Situacao == situacaoAlcadaPendente)));

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> _ConsultarAutorizacoesPorNSF(int codigonfs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var result = from obj in query
                         where
                         obj.LancamentoNFSManual.Codigo == codigonfs
                         &&
                            (
                                (obj.LancamentoNFSManual.AlcadaComRequisito && !obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas && obj.RegrasAutorizacaoNFSManual.Requisito == null)
                                || (obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas)
                                || !obj.LancamentoNFSManual.AlcadaComRequisito
                            )
                         select obj;

            return result;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> BuscarPendentesBloqueadas(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>()
                .Where(aprovacao =>
                    (aprovacao.LancamentoNFSManual.Codigo == codigoOrigem) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Pendente) &&
                    aprovacao.Bloqueada
                )
                .ToList();

            return aprovacoes;
        }

        public Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> BuscarPorNFSUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var result = from obj in query
                         where
                         obj.LancamentoNFSManual.Codigo == codigo
                         && !obj.Bloqueada
                         &&
                            (
                                (obj.LancamentoNFSManual.AlcadaComRequisito && !obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas && obj.RegrasAutorizacaoNFSManual.Requisito == null)
                                || (obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas)
                                || !obj.LancamentoNFSManual.AlcadaComRequisito
                            )
                         select obj;


            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> BuscarPorNFSUsuarioSituacao(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var result = from obj in query
                         where
                            obj.LancamentoNFSManual.Codigo == codigo &&
                            !obj.Bloqueada &&
                            obj.Usuario.Codigo == usuario &&
                            obj.Situacao == situacao
                            &&
                            (
                                (obj.LancamentoNFSManual.AlcadaComRequisito && !obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas && obj.RegrasAutorizacaoNFSManual.Requisito == null)
                                || (obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas && obj.RegrasAutorizacaoNFSManual.Requisito != null)
                                || !(obj.LancamentoNFSManual.AlcadaComRequisito)
                            )
                         select obj;
            return result.ToList();
        }

        /// <summary>
        /// Retorna regras distintas do LANCAMENTO
        /// </summary>
        public List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> BuscarRegrasLancamentoDesbloqueadas(int codigoLancamento)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual>();

            var resultGroup = from obj in queryGroup
                              where
                              obj.LancamentoNFSManual.Codigo == codigoLancamento
                               && !obj.Bloqueada
                               && (
                                    (obj.LancamentoNFSManual.AlcadaComRequisito && !obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas && obj.RegrasAutorizacaoNFSManual.Requisito == null)
                                    || (obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas && obj.RegrasAutorizacaoNFSManual.Requisito != null)
                                    || !obj.LancamentoNFSManual.AlcadaComRequisito
                                  )
                              select obj.RegrasAutorizacaoNFSManual;

            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> ConsultarAutorizacoesPorNSF(int codigonfs, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarAutorizacoesPorNSF(codigonfs);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorNSF(int codigonfs)
        {
            var result = _ConsultarAutorizacoesPorNSF(codigonfs);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> Consultar(int numero, int filial, int transportador, double tomador, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual situacao, DateTime dataInicial, DateTime dataFinal,string codigoCargaEmbarcador, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(numero, filial, transportador, tomador, usuario, situacao, dataInicial, dataFinal, codigoCargaEmbarcador);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int numero, int filial, int transportador, double tomador, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual situacao, DateTime dataInicial, DateTime dataFinal, string codigoCargaEmbarcador)
        {
            var result = _Consultar(numero, filial, transportador, tomador, usuario, situacao, dataInicial, dataFinal, codigoCargaEmbarcador);

            return result.Count();
        }

        public int ContarAprovacoes(int codigoNFS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var resut = from obj in query
                        where
                            obj.LancamentoNFSManual.Codigo == codigoNFS
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Aprovada
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesLancamento(int codigoLancamento, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var resut = from obj in query
                        where
                            obj.LancamentoNFSManual.Codigo == codigoLancamento
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Aprovada
                            && (obj.RegrasAutorizacaoNFSManual.Codigo == codigoRegra || obj.RegrasAutorizacaoNFSManual == null)
                            &&
                              (
                                (obj.LancamentoNFSManual.AlcadaComRequisito && !obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas && obj.RegrasAutorizacaoNFSManual.Requisito == null)
                                || (obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas && obj.RegrasAutorizacaoNFSManual.Requisito != null)
                                || !obj.LancamentoNFSManual.AlcadaComRequisito
                              )
                        select obj;

            return resut.Count();
        }

        public int ContarReprovacoes(int codigoNFS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var resut = from obj in query
                        where
                            obj.LancamentoNFSManual.Codigo == codigoNFS
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Rejeitada
                        select obj;

            return resut.Count();
        }

        public int ContarRejeitadas(int codigoLancamento, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var resut = from obj in query
                        where
                            obj.LancamentoNFSManual.Codigo == codigoLancamento
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Rejeitada
                            && (obj.RegrasAutorizacaoNFSManual.Codigo == codigoRegra || obj.RegrasAutorizacaoNFSManual == null)
                            &&
                              (
                                (obj.LancamentoNFSManual.AlcadaComRequisito && !obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas && obj.RegrasAutorizacaoNFSManual.Requisito == null)
                                || (obj.LancamentoNFSManual.AlcadaComRequisitoAprovadas && obj.RegrasAutorizacaoNFSManual.Requisito != null)
                                || !obj.LancamentoNFSManual.AlcadaComRequisito
                              )
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesNecessarias(int codigoNFS)
        {
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual>();

            var resutAutorizacao = from aut in queryAutorizacao where aut.LancamentoNFSManual.Codigo == codigoNFS select aut.RegrasAutorizacaoNFSManual;
            var resut = from obj in query where resutAutorizacao.Contains(obj) select obj;

            return resut.Sum(o => (int?)o.NumeroAprovadores) ?? 0;
        }

        public bool VerificarSePodeAprovar(int codigoLancamentoNFS, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && !obj.Bloqueada
                            && obj.LancamentoNFSManual.Codigo == codigoLancamentoNFS
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        #endregion
    }
}
