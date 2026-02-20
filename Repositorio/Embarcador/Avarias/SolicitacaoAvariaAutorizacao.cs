using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class SolicitacaoAvariaAutorizacao : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>
    {
        public SolicitacaoAvariaAutorizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> BuscarPorSolicitacaoUsuarioSituacao(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var result = from obj in query
                         where
                            obj.SolicitacaoAvaria.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario &&
                            obj.Situacao == situacao
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> BuscarPorSolicitacaoUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == codigo select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> ResponsavelAvaria(int solicitacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var queryUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == solicitacao select obj.Usuario;
            var resultUsuario = from obj in queryUsuario where result.Contains(obj) select obj;


            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> ResponsaveisSolicitacaoAvaria(List<int> solicitacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            query = query.Where(obj => solicitacoes.Contains(obj.SolicitacaoAvaria.Codigo));

            return query.Fetch(obj => obj.Usuario)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ToList();
        }

        public int ContarAutorizacaoPorSolicitacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == codigo select obj;

            return result.Count();
        }

        public int ContarAprovacoesPorSolicitacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var result = from obj in query
                         where
                            obj.SolicitacaoAvaria.Codigo == codigo &&
                            obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Aprovada
                         select obj;

            return result.Count();
        }

        public int ContarReprovacoesPorSolicitacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var result = from obj in query
                         where
                            obj.SolicitacaoAvaria.Codigo == codigo &&
                            obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Rejeitada
                         select obj;

            return result.Count();
        }

        /// <summary>
        /// Retorna regras distintas da SOLICITACAO
        /// </summary>
        public List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> BuscarRegrasAvaria(int codigoSolicitacao)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria>();

            var resultGroup = from obj in queryGroup where obj.SolicitacaoAvaria.Codigo == codigoSolicitacao select obj.RegrasAutorizacaoAvaria;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        public int ContarRejeitadas(int codigoSolicitacao, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var resut = from obj in query
                        where
                            obj.SolicitacaoAvaria.Codigo == codigoSolicitacao
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Rejeitada
                            && (obj.RegrasAutorizacaoAvaria.Codigo == codigoRegra || obj.RegrasAutorizacaoAvaria == null)
                        select obj;

            return resut.Count();
        }

        public int ContarPendentes(int codigoSolicitacao, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var resut = from obj in query
                        where
                            obj.SolicitacaoAvaria.Codigo == codigoSolicitacao
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente
                            && (obj.RegrasAutorizacaoAvaria.Codigo == codigoRegra || obj.RegrasAutorizacaoAvaria == null)
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesSolicitacao(int codigoSolicitacao, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var resut = from obj in query
                        where
                            obj.SolicitacaoAvaria.Codigo == codigoSolicitacao
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Aprovada
                            && (obj.RegrasAutorizacaoAvaria.Codigo == codigoRegra || obj.RegrasAutorizacaoAvaria == null)
                        select obj;

            return resut.Count();
        }

        public bool VerificarSePodeAprovar(int codigoSolicitacao, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.SolicitacaoAvaria.Codigo == codigoSolicitacao
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> ConsultarAutorizacoesPorSolicitacaoEEtapa(int codigoSolicitacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == codigoSolicitacao && obj.EtapaAutorizacaoAvaria == etapa select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorSolicitacao(int codigoSolicitacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == codigoSolicitacao && obj.EtapaAutorizacaoAvaria == etapa select obj;

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> _Consultar(int empresa, Dominio.ObjetosDeValor.Embarcador.AutorizacaoAvaria.FiltroPesquisaAutorizacaoAvaria filtroPesquisaAutorizacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.SolicitacaoAvaria;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros da ocorrencia
            if (!string.IsNullOrWhiteSpace(filtroPesquisaAutorizacaoAvaria.NumeroCarga))
                result = result.Where(obj => obj.Carga.CodigoCargaEmbarcador.Equals(filtroPesquisaAutorizacaoAvaria.NumeroCarga));

            if (filtroPesquisaAutorizacaoAvaria.NumeroAvaria > 0)
                result = result.Where(obj => obj.NumeroAvaria == filtroPesquisaAutorizacaoAvaria.NumeroAvaria);

            if (empresa > 0)
                result = result.Where(obj => obj.Carga.Empresa.Codigo == empresa);

            if (filtroPesquisaAutorizacaoAvaria.DataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataAvaria.Date >= filtroPesquisaAutorizacaoAvaria.DataInicial);

            if (filtroPesquisaAutorizacaoAvaria.DataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataAvaria.Date <= filtroPesquisaAutorizacaoAvaria.DataFinal);

            //if (situacaoAvaria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Todas && situacaoAvaria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao)
            if (filtroPesquisaAutorizacaoAvaria.SituacaoAvaria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Todas)
                result = result.Where(obj => obj.Situacao == filtroPesquisaAutorizacaoAvaria.SituacaoAvaria);

            if (filtroPesquisaAutorizacaoAvaria.CodigoProduto > 0)
                result = result.Where(obj => obj.ProdutosAvariados.Select(o => o.Codigo).Contains(filtroPesquisaAutorizacaoAvaria.CodigoProduto));

            // Filtros da autorizacao
            if (filtroPesquisaAutorizacaoAvaria.CodigoUsuario > 0)
                result = result.Where(obj => obj.SolicitacaoAvariaAutorizacoes.Any(aut => aut.Usuario.Codigo == filtroPesquisaAutorizacaoAvaria.CodigoUsuario));

            if (filtroPesquisaAutorizacaoAvaria.CodigoTransportador > 0)
                result = result.Where(obj => obj.Transportador.Codigo == filtroPesquisaAutorizacaoAvaria.CodigoTransportador);

            if (filtroPesquisaAutorizacaoAvaria.CodigoFilial > 0)
                result = result.Where(obj => obj.Carga.Filial.Codigo == filtroPesquisaAutorizacaoAvaria.CodigoFilial);

            // O filtro que mais influencia é o de situacao da ocorrencia, pois AgAprovacao, AgAutorizacaoEmissao e AutorizacaoPendente tem o mesmo comportamento
            bool situacaoPendentes = filtroPesquisaAutorizacaoAvaria.SituacaoAvaria == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao situacaoAutorizacaoPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente;

            if (situacaoPendentes)
            {
                result = result.Where(obj => obj.SolicitacaoAvariaAutorizacoes.Any(aut => filtroPesquisaAutorizacaoAvaria.CodigoUsuario > 0 ? (aut.Usuario.Codigo == filtroPesquisaAutorizacaoAvaria.CodigoUsuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));
                if (filtroPesquisaAutorizacaoAvaria.EtapaAutorizacaoAvaria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Todas)
                    result = result.Where(obj => obj.SolicitacaoAvariaAutorizacoes.Any(aut => filtroPesquisaAutorizacaoAvaria.CodigoUsuario > 0 ? (aut.Usuario.Codigo == filtroPesquisaAutorizacaoAvaria.CodigoUsuario && aut.RegrasAutorizacaoAvaria.EtapaAutorizacaoAvaria == filtroPesquisaAutorizacaoAvaria.EtapaAutorizacaoAvaria && aut.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente) : aut.RegrasAutorizacaoAvaria.EtapaAutorizacaoAvaria == filtroPesquisaAutorizacaoAvaria.EtapaAutorizacaoAvaria && aut.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente));
            }
            else
            {
                if (filtroPesquisaAutorizacaoAvaria.EtapaAutorizacaoAvaria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Todas)
                    result = result.Where(obj => obj.SolicitacaoAvariaAutorizacoes.Any(aut => filtroPesquisaAutorizacaoAvaria.CodigoUsuario > 0 ? (aut.Usuario.Codigo == filtroPesquisaAutorizacaoAvaria.CodigoUsuario && aut.RegrasAutorizacaoAvaria.EtapaAutorizacaoAvaria == filtroPesquisaAutorizacaoAvaria.EtapaAutorizacaoAvaria) : aut.RegrasAutorizacaoAvaria.EtapaAutorizacaoAvaria == filtroPesquisaAutorizacaoAvaria.EtapaAutorizacaoAvaria));
            }

            if (situacaoPendentes)
                result = result.Where(obj => obj.SolicitacaoAvariaAutorizacoes.Any(aut => filtroPesquisaAutorizacaoAvaria.CodigoUsuario > 0 ? (aut.Usuario.Codigo == filtroPesquisaAutorizacaoAvaria.CodigoUsuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> Consultar(int empresa, Dominio.ObjetosDeValor.Embarcador.AutorizacaoAvaria.FiltroPesquisaAutorizacaoAvaria filtroPesquisaAutorizacaoAvaria, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(empresa, filtroPesquisaAutorizacaoAvaria);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.Transportador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.MotivoAvaria)
                .ToList();
        }

        public int ContarConsulta(int empresa, Dominio.ObjetosDeValor.Embarcador.AutorizacaoAvaria.FiltroPesquisaAutorizacaoAvaria filtroPesquisaAutorizacaoAvaria)
        {
            var result = _Consultar(empresa, filtroPesquisaAutorizacaoAvaria);

            return result.Count();
        }
    }
}
