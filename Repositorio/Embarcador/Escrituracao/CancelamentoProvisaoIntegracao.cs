using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading;
using Dominio.Excecoes.Embarcador;
using System.Threading.Tasks;
using System;

namespace Repositorio.Embarcador.Escrituracao
{
    public class CancelamentoProvisaoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>
    {

        public CancelamentoProvisaoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public CancelamentoProvisaoIntegracao(UnitOfWork unitOfWork, CancellationToken cancelationToken) : base(unitOfWork, cancelationToken) { }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao BuscarPorNumeroFolha(string numeroMiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();
            var resut = from obj in query where obj.DocumentoProvisao.Stage != null && obj.DocumentoProvisao.Stage.NumeroFolha == numeroMiro select obj;
            return resut.FirstOrDefault();
        }

        public bool VerificaSeExisteUmRegistroQueNaoFoiIntegrado(int CodigoCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();
            var resut = from obj in query where obj.CancelamentoProvisao.Codigo == CodigoCancelamento select obj;
            return resut.Any(x => x.SituacaoIntegracao != SituacaoIntegracao.Integrado);
        }

        public Task<List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>> BuscarPorCancelamentoProvisaoAsync(int CancelamentoProvisao, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();
            var resut = from obj in query where obj.CancelamentoProvisao.Codigo == CancelamentoProvisao && obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao select obj;

            return resut.ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao> BuscarPorCancelamentoProvisao(List<int> CancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();
            var resut = from obj in query where CancelamentoProvisao.Contains(obj.CancelamentoProvisao.Codigo) select obj;
            return resut.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposPorProvisao(int CancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();
            var resut = from obj in query where obj.CancelamentoProvisao.Codigo == CancelamentoProvisao select obj.TipoIntegracao.Tipo;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao> BuscarPorCarga(int CancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();
            var resut = from obj in query where obj.CancelamentoProvisao.Codigo == CancelamentoProvisao select obj;
            return resut.ToList();
        }

        public int ContarPorProvisao(int CancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();

            var resut = from obj in query where obj.CancelamentoProvisao.Codigo == CancelamentoProvisao select obj;

            return resut.Count();
        }

        public int ContarPorCancelamentoProvisao(int codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigoProvisao && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        public int ContarPorProvisaoETipoIntegracao(int codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();

            var resut = from obj in query where obj.CancelamentoProvisao.Codigo == codigoProvisao && obj.TipoIntegracao.Tipo == tipoIntegracao select obj.Codigo;

            return resut.Count();
        }

        public int ContarPorCancelamentoProvisaoESituacaoDiff(int codigo, SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigo && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

        public Task<List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao>> BuscarAgIntegracaoAsync(CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgIntegracao select obj;

            return result.ToListAsync(cancellationToken);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao> _Consultar(int codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoProvisao > 0)
                result = result.Where(obj => obj.CancelamentoProvisao.Codigo == codigoProvisao);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao> Consultar(int codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoProvisao, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var result = _Consultar(codigoProvisao, situacao);

            return result.Count();
        }

        public async Task<int> AtualizarSituacaoAsync(
            IEnumerable<int> codigosCancelamentoProvisao, 
            SituacaoIntegracao situacao, 
            string problema
        )
        {
            const int maxParameters = 2000;
            const int maxBatchSize = maxParameters - 2;
            
            var listaCodigos = codigosCancelamentoProvisao.ToList();
            int succeeded = 0;

            for (int i = 0; i < listaCodigos.Count; i += maxBatchSize)
            {
                var batch = listaCodigos.Skip(i).Take(maxBatchSize).ToArray();

                succeeded += await UnitOfWork.Sessao.CreateSQLQuery(@"
                    UPDATE 
                        T_CANCELAMENTO_PROVISAO_INTEGRACAO
                    SET
                        INT_SITUACAO_INTEGRACAO = :situacao,
                        INT_PROBLEMA_INTEGRACAO = :problema
                    WHERE
                        CPV_CODIGO IN (:codigosCancelamentoProvisao);
                ")
                .SetEnum("situacao", situacao)
                .SetString("problema", problema)
                .SetParameterList("codigosCancelamentoProvisao", batch)
                .ExecuteUpdateAsync(CancellationToken);
            }

            return succeeded;
        }
    }
}
