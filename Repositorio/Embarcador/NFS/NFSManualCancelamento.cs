using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.NFS
{
    public class NFSManualCancelamento : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>
    {
        public NFSManualCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> BuscarPorSituacao(SituacaoNFSManualCancelamento situacao, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>();
            query = query.Where(o => o.SituacaoNFSManualCancelamento == situacao);
            return query.Skip(inicio).Take(limite).ToList();
        }

        public Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento BuscarPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>();

            query = query.Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> Consultar(int codigoLocalidadePrestacao, int filial, double tomador, int numeroNFS, int numeroDOC, int codigoCarga, int empresa, SituacaoNFSManualCancelamento? situacao, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var consulta = ConsultarNFSManualCancelamento(codigoLocalidadePrestacao, filial, tomador, numeroNFS, numeroDOC, codigoCarga, empresa, situacao);

            return consulta.Fetch(o => o.LancamentoNFSManual)
                        .ThenFetch(o => o.LocalidadePrestacao)
                        .Fetch(o => o.LancamentoNFSManual)
                        .ThenFetch(o => o.Filial)
                        .Fetch(o => o.LancamentoNFSManual)
                        .ThenFetch(o => o.Transportador)
                         .Fetch(o => o.LancamentoNFSManual)
                        .ThenFetch(o => o.Tomador)
                        .OrderBy(propOrdenar + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsulta(int codigoLocalidadePrestacao, int filial, double tomador, int numeroNFS, int numeroDOC, int codigoCarga, int empresa, SituacaoNFSManualCancelamento? situacao)
        {
            var consulta = ConsultarNFSManualCancelamento(codigoLocalidadePrestacao, filial, tomador, numeroNFS, numeroDOC, codigoCarga, empresa, situacao);

            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> BuscarLancamentosAgIntegracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>();

            query = query.Where(obj => obj.SituacaoNFSManualCancelamento == SituacaoNFSManualCancelamento.AgIntegracao && obj.GerandoIntegracoes);

            return query.ToList();
        }

        public bool PossuiCTeSemIntegracao(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>();

            IQueryable<int> subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>().Where(o => o.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.NFSManualCancelamento.Codigo);

            query = query.Where(o => o.Codigo == codigoNFSManualCancelamento && !subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Any();
        }

        public bool PossuiCTeComIntegracao(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>();

            IQueryable<int> subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>().Where(o => o.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.NFSManualCancelamento.Codigo);

            query = query.Where(o => o.Codigo == codigoNFSManualCancelamento && subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Any();
        }

        private IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> ConsultarNFSManualCancelamento(int codigoLocalidadePrestacao, int filial, double tomador, int numeroNFS, int numeroDOC, int codigoCarga, int empresa, SituacaoNFSManualCancelamento? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>();

            List<SituacaoLancamentoNFSManual> lancamentoNFSManualCancelado = new List<SituacaoLancamentoNFSManual>()
                {
                    SituacaoLancamentoNFSManual.Cancelada,
                    SituacaoLancamentoNFSManual.Reprovada,
                    SituacaoLancamentoNFSManual.Anulada
                };

            var consultaCargaDocumentoParaEmissaoNFSManualCancelada = this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada>()
                    .Where(o => lancamentoNFSManualCancelado.Contains(o.LancamentoNFSManual.Situacao));

            if (codigoLocalidadePrestacao > 0)
                query = query.Where(o => o.LancamentoNFSManual.LocalidadePrestacao.Codigo == codigoLocalidadePrestacao);

            if (filial > 0)
                query = query.Where(o => o.LancamentoNFSManual.Filial.Codigo == filial);

            if (codigoCarga > 0)
            {
                var consulta = consultaCargaDocumentoParaEmissaoNFSManualCancelada;

                consulta = consulta.Where(lancamentoNFSManualCancelado => lancamentoNFSManualCancelado.CargaDocumentoParaEmissaoNFSManual.CargaOrigem.Codigo == codigoCarga);
                query = query.Where(nfsManualCancelamento => consulta.Where(cargaDocumentoCancelada => nfsManualCancelamento.LancamentoNFSManual.Codigo == cargaDocumentoCancelada.LancamentoNFSManual.Codigo).Any() || nfsManualCancelamento.LancamentoNFSManual.Documentos.Any(doc => doc.Carga.Codigo == codigoCarga));
            }

            if (empresa > 0)
                query = query.Where(o => o.LancamentoNFSManual.Transportador.Codigo == empresa);

            if (tomador > 0)
                query = query.Where(o => o.LancamentoNFSManual.Tomador.CPF_CNPJ.Equals(tomador));

            if (numeroDOC > 0)
            {
                var consulta = consultaCargaDocumentoParaEmissaoNFSManualCancelada;

                consulta = consulta.Where(o => o.CargaDocumentoParaEmissaoNFSManual.Numero == numeroDOC);
                query = query.Where(nfsManualCancelamento => consulta.Where(cargaDocumentoCancelada => nfsManualCancelamento.LancamentoNFSManual.Codigo == cargaDocumentoCancelada.LancamentoNFSManual.Codigo).Any() || nfsManualCancelamento.LancamentoNFSManual.Documentos.Any(doc => doc.Numero == numeroDOC));
            }

            if (numeroNFS > 0)
                query = query.Where(o => o.LancamentoNFSManual.DadosNFS.Numero == numeroNFS);

            if (situacao.HasValue && situacao != SituacaoNFSManualCancelamento.todos)
                query = query.Where(o => o.SituacaoNFSManualCancelamento == situacao);

            return query;
        }

    }
}
