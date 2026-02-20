using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Documentos
{
    public class HistoricoIrregularidade : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>
    {
        #region Construtores 
        public HistoricoIrregularidade(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos 

        public List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> BuscarPorControleDocumento(int codigoControleDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();
            query = from obj in query
                    where obj.ControleDocumento.Codigo == codigoControleDocumento
                    select obj;

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade BuscarAtualPorControleDocumento(int codigoControleDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();
            query = from obj in query
                    where obj.ControleDocumento.Codigo == codigoControleDocumento &&
                    obj.SituacaoIrregularidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIrregularidade.AguardandoAprovacao
                    select obj;

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade BuscarPorMotivo(int codigoMotivo, int codigoControleDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();
            query = from obj in query
                    where obj.ControleDocumento.Codigo == codigoControleDocumento && obj.MotivoIrregularidade.Codigo == codigoMotivo &&
                    obj.SituacaoIrregularidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIrregularidade.AguardandoAprovacao
                    select obj;

            return query.FirstOrDefault();
        }


        public List<int> BuscarPorCodigoControleDocumentos(List<int> codigoControleDocumento, ServicoResponsavel responsavel)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();
            query = from obj in query
                    where codigoControleDocumento.Contains(obj.ControleDocumento.Codigo) && obj.SituacaoIrregularidade == SituacaoIrregularidade.AguardandoAprovacao && obj.ServicoResponsavel == responsavel
                    select obj;

            return query.Select(x => x.ControleDocumento.CTe.Codigo).Distinct().ToList();
        }


        public int BuscarPorCodigoControleDocumentos(List<int> codigoCte)
        {

            var sql = $@"SELECT Sum(_movimentacao.MCP_VALOR_MONETARIO) FROM T_CONTROLE_DOCUMENTO _controleDocumento
                        LEFT JOIN T_HISTORICO_IRREGULARRIDADE _historico on _historico.COD_CODIGO = _controleDocumento.COD_CODIGO
                        LEFT JOIN T_DOCUMENTO_FATURAMENTO _documentoFaturamento on _controleDocumento.CON_CODIGO = _documentoFaturamento.CON_CODIGO
                        LEFT JOIN T_MOVIMENTACAO_CONTA_PAGAR _movimentacao on _movimentacao.CON_CODIGO = _controleDocumento.CON_CODIGO
                        WHERE _controleDocumento.CON_CODIGO IN ({string.Join(", ", codigoCte)}) AND( _controleDocumento.COD_SITUACAO_CONTROLE_DOCUMENTO = 7 OR _historico.HII_SITUACAO_IRREGULARIDADE = 1) AND (_documentoFaturamento.DFA_BLOQUEIO IS NULL OR DFA_BLOQUEIO = '')
                        ";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }
        public Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade BuscarPorControleDocumentoEIrregularidade(int codigoControleDocumento, int codigoIrregularidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();
            query = from obj in query
                    where obj.ControleDocumento.Codigo == codigoControleDocumento && obj.Irregularidade.Codigo == codigoIrregularidade
                    select obj;

            return query.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade BuscarUltimoPorDocumentoResponsavel(int documento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ServicoResponsavel responsavel)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();
            query = from obj in query
                    where obj.ControleDocumento.Codigo == documento && obj.SituacaoIrregularidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIrregularidade.AguardandoAprovacao
                    orderby obj.Irregularidade.Sequencia, obj.Codigo descending
                    select obj;

            if (responsavel != 0)
                query = query.Where(obj => obj.ServicoResponsavel == responsavel);

            if (responsavel == ServicoResponsavel.Embarcador)
                query = from obj in query where obj.SequenciaTrataviva > 0 select obj;

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> BuscarPendentesPorControleDocumento(int documento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();
            query = from obj in query
                    where obj.ControleDocumento.Codigo == documento && (obj.SituacaoIrregularidade == SituacaoIrregularidade.AguardandoAprovacao || obj.SituacaoIrregularidade == 0)
                    select obj;

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> BuscarPorCodigoControleDocumento(List<int> codigoControleDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();
            query = from obj in query
                    where codigoControleDocumento.Contains(obj.ControleDocumento.Codigo)
                    select obj;

            return query
                    .Fetch(x => x.Irregularidade)
                    .Fetch(x =>x.Porfolio)
                    .Fetch(x => x.Setor).ToList();
        }

        public bool VerificarSeDocumentoLiberadoPorHistorico(int codigoControleDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();
            query = from obj in query
                    where obj.ControleDocumento.Codigo == codigoControleDocumento
                    select obj;

            return !query.ToList().Exists(o => o.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada);
        }


        #endregion
    }
}
