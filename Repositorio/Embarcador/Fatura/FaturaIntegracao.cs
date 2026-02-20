using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>
    {
        public FaturaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public FaturaIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemIntegracaPentende(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado && obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao BuscarLayoutFaturaPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.TipoIntegracaoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();

            query = query.Where(obj => obj.ArquivosIntegracao.Any(o => o.Codigo == codigoArquivo));

            return query.FirstOrDefault();
        }

        public int ContarPorFatura(int codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura? tipo = null, int? codigoLayoutEDI = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracaoFatura == tipo);

            if (codigoLayoutEDI.HasValue && codigoLayoutEDI > 0)
                query = query.Where(o => o.LayoutEDI.Codigo == codigoLayoutEDI);

            return query.Count();
        }

        public int ContarPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura? tipo = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            queryFaturaDocumento = queryFaturaDocumento.Where(c => c.Documento.CargaPagamento.Codigo == codigoCarga);
            query = query.Where(c => c.SituacaoIntegracao == situacao && queryFaturaDocumento.Any(f => f.Fatura.Codigo == c.Fatura.Codigo));

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracaoFatura == tipo);

            return query.Count();
        }

        public int TotalArquivos(int codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.TipoIntegracaoFatura == tipo select obj;
            return result.Count();
        }

        public int TotalArquivosStatus(int codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.TipoIntegracaoFatura == tipo && obj.SituacaoIntegracao == status select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> BuscarPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.Fatura.Codigo == codigoFatura);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> BuscarPorFatura(int codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.Fatura.Codigo == codigoFatura && obj.TipoIntegracaoFatura == tipo);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> BuscarPorFatura(int codigoFatura, int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura tipo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var result = from obj in query select obj;

            result = result.Where(obj => obj.TipoIntegracaoFatura == tipo);

            if (situacao.HasValue)
                result = result.Where(obj => obj.SituacaoIntegracao == situacao.Value);

            if (codigoFatura > 0)
                result = result.Where(obj => obj.Fatura.Codigo == codigoFatura);

            if (codigoCarga > 0)
            {
                var queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
                queryFaturaDocumento = queryFaturaDocumento.Where(c => c.Documento.CargaPagamento.Codigo == codigoCarga);
                result = result.Where(c => queryFaturaDocumento.Any(f => f.Fatura.Codigo == c.Fatura.Codigo));
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorFatura(int codigoFatura, int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var result = from obj in query select obj;

            result = result.Where(obj => obj.TipoIntegracaoFatura == tipo);

            if (situacao.HasValue)
                result = result.Where(obj => obj.SituacaoIntegracao == situacao.Value);

            if (codigoFatura > 0)
                result = result.Where(obj => obj.Fatura.Codigo == codigoFatura);

            if (codigoCarga > 0)
            {
                var queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
                queryFaturaDocumento = queryFaturaDocumento.Where(c => c.Documento.CargaPagamento.Codigo == codigoCarga);
                result = result.Where(c => queryFaturaDocumento.Any(f => f.Fatura.Codigo == c.Fatura.Codigo));
            }

            return result.Count();
        }

        public Task<List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>> BuscarIntegracoesPendentesEnvioAsync(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();

            if(tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                query = query.Where(obj => obj.Fatura.Situacao != SituacaoFatura.Cancelado &&
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao && obj.DataEnvio == null)
                               ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao && obj.DataEnvio.Value <= DateTime.Now)
                               ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno 
                               && obj.TipoIntegracao.Tipo != TipoIntegracao.SAP
                               && obj.TipoIntegracao.Tipo != TipoIntegracao.SAP_ESTORNO_FATURA
                               && obj.DataEnvio.Value <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                               ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.Tentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataEnvio <= DateTime.Now.AddMinutes(-minutosACadaTentativa)));
            }
            else
            {
                query = query.Where(obj => (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao && obj.DataEnvio == null)
                               ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao && obj.DataEnvio.Value <= DateTime.Now)
                               ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno && obj.DataEnvio.Value <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                               ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.Tentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataEnvio <= DateTime.Now.AddMinutes(-minutosACadaTentativa)));
            }

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToListAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao BuscarIntegracoesPorCargaEnvioEmail(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();

            query = query.Where(obj => obj.Fatura.Codigo == codigoFatura);

            return query.FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>> BuscarIntegracoesPendentesEnvioEmailAsync(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();

            query = query.Where(obj => obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email
                                && obj.TipoIntegracaoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura.Fatura &&
                                (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                 obj.Tentativas < 4 && obj.DataEnvio <= DateTime.Now.AddMinutes(-minutosACadaTentativa))));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> BuscarIntegracoesPendentesRetorno(string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                query = query.Where(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno 
                                    && obj.Fatura.Situacao != SituacaoFatura.Cancelado);
            }
            else
            {
                query = query.Where(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);
            }

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao BuscarPorFaturaETipo(int codigoFatura, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>().
                Where(obj => obj.Fatura.Codigo == codigoFatura 
                && obj.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao BuscarPorCTeETipo(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            var queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(obj => obj.TipoIntegracao.Tipo == tipoIntegracao &&
                                 queryFaturaDocumento.Where(fd => fd.Fatura.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado
                                                            && fd.Fatura.Codigo == obj.Fatura.Codigo
                                                            && fd.Documento.CTe.Codigo == codigoCTe).Any());

            return query.FirstOrDefault();
        }

        public int ObterNumeroDoTitulo(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o =>  o.FaturaDocumento.Fatura.Codigo == codigoFatura);

            return query.Select(o => o.Codigo).FirstOrDefault();
        }
    }
}