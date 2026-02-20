using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaRotaFretePontosPassagem : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>
    {
        public CargaRotaFretePontosPassagem(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaRotaFretePontosPassagem(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where obj.CargaRotaFrete.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        public int BuscarDistanciaPorCargaPedido(int codigoCarga, double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query
                         where obj.CargaRotaFrete.Carga.Codigo == codigoCarga && obj.Cliente.CPF_CNPJ == cpfCnpj
                         select obj.Distancia;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> BuscarPorCargaOrdenado(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            return query
                .Where(obj => obj.CargaRotaFrete.Carga.Codigo == codigoCarga && obj.TipoPontoPassagem == TipoPontoPassagem.Fronteira)
                .OrderBy(x => x.Ordem)
                .ToList();
        }

        public List<(string NomeFantasia, string Nome)> BuscarNomeFantasiaPorCargaOrdenado(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>()
                .Where(obj => obj.CargaRotaFrete.Carga.Codigo == codigoCarga && obj.TipoPontoPassagem == TipoPontoPassagem.Fronteira);

            return query
                .OrderBy(obj => obj.Ordem)
                .Select(obj => new ValueTuple<string, string>(obj.Cliente.NomeFantasia, obj.Cliente.Nome))
                .ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>> BuscarPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where obj.CargaRotaFrete.Carga.Codigo == codigoCarga select obj;
            return await result.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where codigosCarga.Contains(obj.CargaRotaFrete.Carga.Codigo) select obj;
            return result.Fetch(x => x.CargaRotaFrete).ThenFetch(x => x.Carga).Fetch(x => x.Cliente).Fetch(x => x.ClienteOutroEndereco).ToList();
        }

        public int BuscarDistanciaPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where obj.CargaRotaFrete.Carga.Codigo == codigoCarga select obj;
            return result.Sum(obj => (int?)obj.Distancia) ?? 0;
        }


        public int BuscarDistanciaPorCargaRotaFreteCodigo(int cargaRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where obj.CargaRotaFrete.Codigo == cargaRotaFrete select obj;
            return result.Sum(obj => (int?)obj.Distancia) ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> BuscarPorCargaRotaFrete(int cargaRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where obj.CargaRotaFrete.Codigo == cargaRotaFrete select obj;
            return result.OrderBy(obj => obj.Ordem).ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>> BuscarPorCargaRotaFreteAsync(int cargaRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where obj.CargaRotaFrete.Codigo == cargaRotaFrete select obj;
            return await result.OrderBy(obj => obj.Ordem).ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem BuscarPontoRetornoPorCarga(int codigoCarga)
        {
            var consultaPontosPassagem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>()
                .Where(o => o.CargaRotaFrete.Carga.Codigo == codigoCarga && o.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno);

            return consultaPontosPassagem.OrderBy(o => o.Ordem).FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPorRemetenteCargaRotaFrete(int cargaRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            return query.Where(obj => obj.Ordem == 0 && obj.CargaRotaFrete.Codigo == cargaRotaFrete).Select(s => s.Cliente).FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPorRemetenteCargaPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            return query.Where(obj => obj.Ordem == 0 && obj.CargaRotaFrete.Carga.Codigo == codigoCarga).Select(s => s.Cliente).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> BuscarPrimeiraRotaFretePorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            return query.Where(obj => obj.Ordem == 0 && codigosCarga.Contains(obj.CargaRotaFrete.Carga.Codigo))
                   .Fetch(f => f.Cliente)
                   .Fetch(f => f.CargaRotaFrete)
                   .ThenFetch(tf => tf.Carga)
                   .ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> ObterDestinatariosDaCargaRotaFretePorCarga(int codigoCarga)
        {
            var tiposPontoPassagem = new[] {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            return query.Where(obj => obj.CargaRotaFrete.Carga.Codigo == codigoCarga &&
                                     tiposPontoPassagem.Contains(obj.TipoPontoPassagem))
                       .Fetch(f => f.Cliente)
                       .Fetch(f => f.CargaRotaFrete)
                       .ThenFetch(tf => tf.Carga)
                       .OrderBy(x => x.Ordem).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> BuscarPorCargaRotaFreteETipoPassagem(int cargaRotaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem tipoPontoPassagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where obj.CargaRotaFrete.Codigo == cargaRotaFrete && obj.TipoPontoPassagem == tipoPontoPassagem select obj;
            return result.OrderBy(obj => obj.Ordem).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> BuscarPorCargaClienteComGeolocalizacao(int codigoCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem> tiposPontoPassagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            return query.Where(obj =>
                obj.CargaRotaFrete.Carga.Codigo == codigoCarga &&
                tiposPontoPassagem.Contains(obj.TipoPontoPassagem) &&
                obj.Cliente.Latitude != null && obj.Cliente.Longitude != null &&
                (
                    (obj.Cliente.TipoArea == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Raio && obj.Cliente.RaioEmMetros > 0) ||
                    (obj.Cliente.TipoArea == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Poligono && obj.Cliente.Area != "")
                )
            ).ToList();
        }

        public bool PossuiPorCargaRotaFrete(int cargaRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            var result = from obj in query where obj.CargaRotaFrete.Codigo == cargaRotaFrete select obj;
            return result.Any();
        }

        public void DeletarPorCargaRotaFrete(int cargaRotaFrete)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE from CargaRotaFretePontosPassagem obj WHERE obj.CargaRotaFrete.Codigo = :codigoCargaRotaFrete ")
                              .SetInt32("codigoCargaRotaFrete", cargaRotaFrete)
                              .ExecuteUpdate();

        }
        public Task DeletarPorCargaRotaFreteAsync(int cargaRotaFrete)
        {
            return UnitOfWork.Sessao.CreateQuery("DELETE from CargaRotaFretePontosPassagem obj WHERE obj.CargaRotaFrete.Codigo = :codigoCargaRotaFrete ")
                              .SetInt32("codigoCargaRotaFrete", cargaRotaFrete)
                              .ExecuteUpdateAsync();

        }

    }
}
