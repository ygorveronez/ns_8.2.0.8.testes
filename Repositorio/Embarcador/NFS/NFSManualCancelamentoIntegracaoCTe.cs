using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.NFS
{
    public class NFSManualCancelamentoIntegracaoCTe : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>
    {
        public NFSManualCancelamentoIntegracaoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public NFSManualCancelamentoIntegracaoCTe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe BuscarPorCodigoArquivo(int codigoArquivo)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> BuscarPorNFSManualCancelamento(int codigoNFSManualCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(o => o.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> BuscarPorNFSManualCancelamento(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(o => o.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorNFSManualCancelamento(int codigoNFSManualCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            return query.Select(obj => obj.TipoIntegracao.Tipo).Distinct().ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>> BuscarIntegracaoPendenteAsync(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => !obj.NFSManualCancelamento.GerandoIntegracoes && obj.TipoIntegracao.TipoEnvio == tipoEnvio && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos))));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToListAsync(CancellationToken);
        }

        public int ContarPorNFSManualCancelamentoETipoIntegracao(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(o => o.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && o.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> BuscarCTeIntegracaoSemLote(string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => !obj.NFSManualCancelamento.GerandoIntegracoes && obj.TipoIntegracao.TipoEnvio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao.Lote && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao && obj.Lote == null);

            return query.Fetch(o => o.TipoIntegracao).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarTipoIntegracaoPendente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == tipoEnvio && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) select obj.TipoIntegracao;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> Consultar(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            if (codigoNFSManualCancelamento > 0)
                query = query.Where(o => o.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracao.Tipo == tipo);

            return query.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.NFSManualCancelamento).ThenFetch(obj => obj.LancamentoNFSManual).ThenFetch(obj => obj.CTe).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            if (codigoNFSManualCancelamento > 0)
                query = query.Where(o => o.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracao.Tipo == tipo);

            return query.Count();
        }

        public int ContarPorNFSManualCancelamento(int codigoNFSManualCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            return query.Count();
        }

        public int ContarPorNFSManualCancelamentoESituacaoDiff(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && obj.SituacaoIntegracao != situacaoDiff);

            return query.Count();
        }

        public int ContarPorNFSManualCancelamento(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && obj.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public int ContarPorNFSManualCancelamento(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao[] situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && situacao.Contains(obj.SituacaoIntegracao));

            return query.Count();
        }

        public int ContarPorLote(int codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>();

            query = query.Where(obj => obj.Lote.Codigo == codigoLote && obj.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public List<int> BuscarCodigosPorNFSManualCancelamentoSemIntegracao(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>();

            IQueryable<int> queryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>()
                                                   .Where(o => o.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && o.TipoIntegracao.Tipo == tipoIntegracao)
                                                   .Select(o => o.LancamentoNFSManual.Codigo);

            query = query.Where(o => o.Codigo == codigoNFSManualCancelamento && !queryIntegracoes.Contains(o.LancamentoNFSManual.Codigo));
            
            return query.Select(o => o.LancamentoNFSManual.Codigo).Take(quantidadeRegistros).ToList();
        }
    }
}
