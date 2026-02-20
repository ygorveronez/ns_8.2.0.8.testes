using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCancelamentoIntegracaoEDI : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>
    {
        public CargaCancelamentoIntegracaoEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaCancelamentoIntegracaoEDI(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }


        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorCargaCancelamento(int codigoCargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }


        public int ContarPorCancelamento(int cancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == cancelamento && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        public int ContarPorCancelamento(int cancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao[] situacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == cancelamento && situacoes.Contains(obj.SituacaoIntegracao) select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI> BuscarPorCancelamento(int cancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == cancelamento select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI> _Consultar(int cancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>();
            var result = from obj in query select obj;

            if (cancelamento > 0)
                result = result.Where(obj => obj.CargaCancelamento.Codigo == cancelamento);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI> Consultar(int cancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(cancelamento, situacao);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(int lote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var result = _Consultar(lote, situacao);

            return result.Count();
        }

        public bool VerificarSeExistePorCarga(int codigoCarga, int codigoTipoIntegracao, int codigoLayoutEDI, double tomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>();

            if (tomador > 0)
            {
                query = query.Where(obj => obj.CargaCancelamento.Carga.Pedidos.Any(p =>
                            (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && p.Pedido.Remetente.CPF_CNPJ == tomador) ||
                            (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && p.Pedido.Destinatario.CPF_CNPJ == tomador) ||
                            (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && p.Tomador.CPF_CNPJ == tomador) ||
                            (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && p.Recebedor.CPF_CNPJ == tomador) ||
                            (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && p.Expedidor.CPF_CNPJ == tomador))
                    );
            }

            var result = from obj in query
                         where
                            obj.CargaCancelamento.Carga.Codigo == codigoCarga
                            && obj.TipoIntegracao.Codigo == codigoTipoIntegracao
                            && obj.LayoutEDI.Codigo == codigoLayoutEDI
                         select obj.Codigo;


            return result.Any();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>> BuscarIntegracoesPendentesAsync(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>();

            var result = from obj in query
                         where obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                               obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToListAsync(CancellationToken);
        }

        public int ContarPorCargaCancelamento(int codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao[] situacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI>();

            var result = from obj in query where obj.CargaCancelamento.Codigo == codigoCargaCancelamento && situacoes.Contains(obj.SituacaoIntegracao) select obj;

            return result.Count();
        }
    }
}

