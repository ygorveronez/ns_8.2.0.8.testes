using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaPercurso : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>
    {
        public CargaPercurso(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaPercurso(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaPercurso BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public int ContarPorCarga(int codCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
            var resut = from obj in query where obj.Carga.Codigo == codCarga select obj;
            return resut.Count();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> ConsultarPorCarga(int codCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
            var result = from obj in query where obj.Carga.Codigo == codCarga select obj;
            return result.OrderBy(obj => obj.Posicao)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>> ConsultarPorCargaAsync(int codCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
            var result = from obj in query where obj.Carga.Codigo == codCarga select obj;
            return await result.OrderBy(obj => obj.Posicao)
                .Fetch(obj => obj.Origem)
                .Fetch(obj => obj.Destino)
                .ToListAsync();
        }

        public int ConsultarDistanciaTotalPorCarga(int codCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
            var result = from obj in query where obj.Carga.Codigo == codCarga select obj;
            return result.Sum(obj => (int?)obj.DistanciaKM) ?? 0;
        }

        public int ConsultarDistanciaTotalPorContratoFrete(DateTime dataInicio, DateTime dataFim, int contratoFreteTransprotador, double destinatario = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
            var result = from obj in query
                         where
                             obj.Carga.ContratoFreteTransportador.Codigo == contratoFreteTransprotador
                             && obj.Carga.DataFinalizacaoEmissao >= dataInicio && obj.Carga.DataFinalizacaoEmissao < dataFim.AddDays(1)
                             && (
                                obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento
                             )
                         select obj;

            if (destinatario > 0)
                result = result.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.Destinatario.CPF_CNPJ == destinatario));

            return result.Sum(obj => (int?)obj.DistanciaKM) ?? 0;
        }
        
        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo> ConsultarVeiculosImprodutivos(DateTime dataInicio, DateTime dataFim, int contratoFreteTransprotador)
        {
            var queryContrato = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();

            var result = from obj in query
                         where 
                             obj.Carga.ContratoFreteTransportador.Codigo == contratoFreteTransprotador
                             && obj.Carga.DataFinalizacaoEmissao >= dataInicio && obj.Carga.DataFinalizacaoEmissao < dataFim.AddDays(1)
                             && 
                             (
                                obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento
                            )
                         select obj.Carga.Veiculo;

            var resultContrato = from o in queryContrato
                                 where
                                    o.ContratoFrete.Codigo == contratoFreteTransprotador
                                    && !result.Contains(o.Veiculo)
                                 select o;

            return resultContrato.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPercurso BuscarUltimaEntrega(int codCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
            var result = from obj in query where obj.Carga.Codigo == codCarga && obj.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.descarregamento select obj;
            return result.OrderByDescending(obj => obj.Posicao).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPercurso BuscarOrigem(int codCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
            var result = from obj in query where obj.Carga.Codigo == codCarga && obj.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.carregamento select obj;
            return result.OrderBy(obj => obj.Posicao).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> ConsultarPorCargaEEstado(int codCarga, string sigla, string ordem = "desc")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
            var result = from obj in query where obj.Carga.Codigo == codCarga && obj.Destino.Estado.Sigla == sigla select obj;
            if (ordem == "desc")
                return result.OrderByDescending(obj => obj.Posicao).ToList();
            else
                return result.OrderBy(obj => obj.Posicao).ToList();
        }
    }
}
