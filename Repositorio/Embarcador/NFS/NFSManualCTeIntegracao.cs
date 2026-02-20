using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.NFS
{
    public class NFSManualCTeIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>
    {
        #region Construtores

        public NFSManualCTeIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public NFSManualCTeIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ConsultarNFSManualCTeIntegracao(int codigoLancamentoNFSManual, SituacaoIntegracao? situacao, TipoIntegracao? tipo)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>();

            if (codigoLancamentoNFSManual > 0)
                consultaNFSManualCTeIntegracao = consultaNFSManualCTeIntegracao.Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);

            if (situacao.HasValue)
                consultaNFSManualCTeIntegracao = consultaNFSManualCTeIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                consultaNFSManualCTeIntegracao = consultaNFSManualCTeIntegracao.Where(o => o.TipoIntegracao.Tipo == tipo);

            return consultaNFSManualCTeIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao BuscarPorCodigo(int codigo)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o => o.Codigo == codigo);

            return consultaNFSManualCTeIntegracao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));

            return consultaNFSManualCTeIntegracao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao BuscarPrimeiroPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);

            return consultaNFSManualCTeIntegracao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao BuscarPorLancamentoNFSManualETipoIntegracao(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && o.TipoIntegracao.Tipo == tipoIntegracao);

            return consultaNFSManualCTeIntegracao.FirstOrDefault();
        }
        public List<TipoIntegracao> BuscarTipoIntegracaoPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);

            return consultaNFSManualCTeIntegracao
                .Select(o => o.TipoIntegracao.Tipo)
                .Distinct()
                .ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>> BuscarCTeIntegracaoPendenteAsync(int tentativasLimite, double tempoProximaTentativaMinutos, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o =>
                    o.LancamentoNFSManual.GerandoIntegracoes == false &&
                    o.TipoIntegracao.TipoEnvio == TipoEnvioIntegracao.Individual &&
                    (
                        o.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                        (o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && o.NumeroTentativas < tentativasLimite && o.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos))
                    )
                );

            return await ObterListaAsync(consultaNFSManualCTeIntegracao, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> BuscarCTeIntegracaoSemLote(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o =>
                    o.LancamentoNFSManual.GerandoIntegracoes == false &&
                    o.TipoIntegracao.TipoEnvio == TipoEnvioIntegracao.Lote &&
                    o.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao &&
                    o.Lote == null
                );

            return ObterLista(consultaNFSManualCTeIntegracao, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> Consultar(int codigoLancamentoNFSManual, SituacaoIntegracao? situacao, TipoIntegracao? tipo)
        {
            return Consultar(codigoLancamentoNFSManual, situacao, tipo, parametrosConsulta: null);
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> Consultar(int codigoLancamentoNFSManual, SituacaoIntegracao? situacao, TipoIntegracao? tipo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaNFSManualCTeIntegracao = ConsultarNFSManualCTeIntegracao(codigoLancamentoNFSManual, situacao, tipo);

            consultaNFSManualCTeIntegracao = consultaNFSManualCTeIntegracao
                .Fetch(o => o.LancamentoNFSManual).ThenFetch(o => o.CTe).ThenFetch(o => o.Empresa)
                .Fetch(o => o.TipoIntegracao);

            return ObterLista(consultaNFSManualCTeIntegracao, parametrosConsulta);
        }

        public int ContarConsulta(int codigoLancamentoNFSManual, SituacaoIntegracao? situacao, TipoIntegracao? tipo)
        {
            var consultaNFSManualCTeIntegracao = ConsultarNFSManualCTeIntegracao(codigoLancamentoNFSManual, situacao, tipo);

            return consultaNFSManualCTeIntegracao.Count();
        }

        public int ContarPorLancamentoNFSManualESituacaoDiff(int codigoLancamentoNFSManual, SituacaoIntegracao situacaoDiff)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && o.SituacaoIntegracao != situacaoDiff);

            return consultaNFSManualCTeIntegracao.Count();
        }

        public int ContarPorLancamentoNFSManual(int codigoLancamentoNFSManual, SituacaoIntegracao situacao)
        {
            return ContarPorLancamentoNFSManual(codigoLancamentoNFSManual, new SituacaoIntegracao[] { situacao });
        }

        public int ContarPorLancamentoNFSManual(int codigoLancamentoNFSManual, SituacaoIntegracao[] situacoes)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && situacoes.Contains(o.SituacaoIntegracao));

            return consultaNFSManualCTeIntegracao.Count();
        }

        public int ContarPorLote(int codigoLote, SituacaoIntegracao situacao)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o => o.Lote.Codigo == codigoLote && o.SituacaoIntegracao == situacao);

            return consultaNFSManualCTeIntegracao.Count();
        }

        public bool ExistePorLancamentoNFSManualETipo(int codigoLancamentoNFSManual, TipoIntegracao tipo)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>()
                .Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && o.TipoIntegracao.Tipo == tipo);

            return consultaNFSManualCTeIntegracao.Count() > 0;
        }

        #endregion
    }
}
