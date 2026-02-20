using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaEDIIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>
    {
        public CargaEDIIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaEDIIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<int> BuscarCodigosContainersPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Container != null);
            return query.Select(obj => obj.Container.Codigo).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>> ConsultarAsync(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, bool integracaoFilialEmissora, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();

            var result = from obj in query where obj.IntegracaoFilialEmissora == integracaoFilialEmissora select obj;

            if (codigoCarga > 0)
                result = result.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.Remetente)
                .Fetch(obj => obj.TipoIntegracao)
                .Fetch(obj => obj.LayoutEDI)
                .ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao> BuscarDadosParaReintegracaoEmMassa(int reenvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();
            var queryReenvio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI>();

            var resultReenvio = from o in queryReenvio where o.Codigo == reenvio select o;

            var result = from obj in query
                         where resultReenvio.Any(r => r.Cargas.Contains(obj.Carga) && r.Layouts.Contains(obj.LayoutEDI))
                         select obj;

            return result.ToList();
        }

        public Task<int> ContarConsultaAsync(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();

            var result = from obj in query where obj.IntegracaoFilialEmissora == integracaoFilialEmissora select obj;

            if (codigoCarga > 0)
                result = result.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.CountAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>> BuscarIntegracoesPendentesAsync(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();

            var result = from obj in query
                         where obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                               obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                               !obj.Carga.GerandoIntegracoes &&
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.ToList();
        }

        public Task<int> ContarPorCargaESituacaoDiffAsync(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao != situacaoDiff && obj.IntegracaoFilialEmissora == integracaoFilialEmissora select obj.Codigo;

            return result.CountAsync(CancellationToken);
        }

        public Task<int> ContarPorCargaAsync(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == situacao && obj.IntegracaoFilialEmissora == integracaoFilialEmissora select obj.Codigo;

            return result.CountAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>> BuscarPorCargaAsync(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.IntegracaoFilialEmissora == integracaoFilialEmissora select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>> BuscarTipoIntegracaoPorCargaAsync(int codigoCarga, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.IntegracaoFilialEmissora == integracaoFilialEmissora select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToListAsync(CancellationToken);
        }

        public bool VerificarSeExistePorCarga(int codigoCarga, int codigoTipoIntegracao, int codigoLayoutEDI, double tomador, double remetente = 0D, int codigoCTe = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();

            if (tomador > 0D)
            {
                query = query.Where(obj => obj.Pedidos.Any(p =>
                            (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && p.Pedido.Remetente.CPF_CNPJ == tomador) ||
                            (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && p.Pedido.Destinatario.CPF_CNPJ == tomador) ||
                            (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && p.Tomador.CPF_CNPJ == tomador) ||
                            (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && p.Recebedor.CPF_CNPJ == tomador) ||
                            (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && p.Expedidor.CPF_CNPJ == tomador))
                    );
            }

            if (remetente > 0D)
                query = query.Where(o => o.Remetente.CPF_CNPJ == remetente);

            if (codigoCTe > 0)
                query = query.Where(o => o.CTe.Codigo == codigoCTe);

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.LayoutEDI.Codigo == codigoLayoutEDI);

            return query.Select(o => o.Codigo).Any();
        }
    }
}
