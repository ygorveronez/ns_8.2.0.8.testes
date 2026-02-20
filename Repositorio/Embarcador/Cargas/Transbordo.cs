using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Threading;

namespace Repositorio.Embarcador.Cargas
{
    public class Transbordo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Transbordo>
    {
        public Transbordo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Transbordo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.Transbordo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Transbordo BuscarPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>();
            var result = from obj in query where obj.Carga.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Transbordo BuscarPorCargaGerada(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>();
            var result = from obj in query where obj.CargaGerada.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.Transbordo>> BuscarTodosTransbordosPorCargaAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>();
            var result = from obj in query where obj.Carga.Codigo == codigo select obj;
            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Transbordo> BuscarPorCargaGerada(List<int> codigosCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>();
            var result = from obj in query where codigosCargas.Contains(obj.CargaGerada.Codigo) select obj;
            return result
                .Fetch(o => o.Carga)
                .ToList();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>();

            int? retorno = query.Max(o => (int?)o.NumeroTransbordo);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Transbordo> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTransbordo filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtroPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTransbordo filtroPesquisa)
        {
            var result = Consultar(filtroPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarCTesTransbordo(int transbordo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>();

            var result = from obj in query select obj;

            if (transbordo > 0)
                result = result.Where(obj => obj.Codigo == transbordo);

            return result.SelectMany(obj => obj.CargaCTesTransbordados).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsultaCTesTransbordo(int transbordo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>();

            var result = from obj in query select obj;
            if (transbordo > 0)
                result = result.Where(obj => obj.Codigo == transbordo);

            return result.SelectMany(obj => obj.CargaCTesTransbordados).Count();
        }



        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.VinculoEntreCargaEntregaComCargaEntregaTransbordada> ConsultarVinculoEntreCargaEntregaComCargaEntregaTransbordada(int codigoCargaTransbordada)
        {
            string sql = $@" select distinct
                            T.CAR_CODIGO CargaOrigem, CAR_CODIGO_GERADA CargaTransbordo,CLI_CODIGO_ENTREGA CodigoCliente, CEN_CODIGO CodigoCargaEntregaOrigem from T_CARGA_PEDIDO P
                            INNER JOIN T_CARGA C ON C.CAR_CODIGO = P.CAR_CODIGO
                            INNER JOIN T_TRANSBORDO T ON T.CAR_CODIGO_GERADA = P.CAR_CODIGO_ORIGEM
                            INNER JOIN T_CARGA_ENTREGA CE ON CE.CAR_CODIGO = T.CAR_CODIGO
                            where P.CAR_CODIGO = {codigoCargaTransbordada} ";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.VinculoEntreCargaEntregaComCargaEntregaTransbordada)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Carga.VinculoEntreCargaEntregaComCargaEntregaTransbordada>();
        }

        public List<int> BuscarPorProtocoloIntegracaoCargaOrigem(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>().OrderBy(t => t.Codigo);
            var result = from obj in query where obj.CargaGerada != null && obj.Carga.Protocolo == codigo select obj.CargaGerada.Protocolo;
            return result.ToList();
        }

        public List<int> BuscarPorProtocoloIntegracaoCargaOrigem(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>().OrderBy(t => t.Codigo);
            var result = from obj in query where obj.CargaGerada != null && codigos.Contains(obj.Carga.Protocolo) select obj.CargaGerada.Protocolo;
            return result.ToList();
        }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Transbordo> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTransbordo filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Transbordo>();

            var result = from obj in query select obj;

            if (filtroPesquisa.NumeroTransbordo > 0)
                result = result.Where(obj => obj.NumeroTransbordo == filtroPesquisa.NumeroTransbordo);

            if (filtroPesquisa.CodigoCarga > 0)
                result = result.Where(obj => obj.Carga.Codigo == filtroPesquisa.CodigoCarga);

            if (filtroPesquisa.CodigosEmpresa?.Count > 0)
                result = result.Where(obj => filtroPesquisa.CodigosEmpresa.Contains(obj.Empresa.Codigo));

            if (filtroPesquisa.CodigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == filtroPesquisa.CodigoVeiculo);

            if (filtroPesquisa.LocalidadeTransbordo > 0)
                result = result.Where(obj => obj.localidadeTransbordo.Codigo == filtroPesquisa.LocalidadeTransbordo);

            if (filtroPesquisa.DataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataTransbordo >= filtroPesquisa.DataInicio);

            if (filtroPesquisa.DataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataTransbordo <= filtroPesquisa.DataFim);

            if (filtroPesquisa.SituacaoTransbordo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.Todas)
                result = result.Where(obj => obj.SituacaoTransbordo == filtroPesquisa.SituacaoTransbordo);

            if (filtroPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
                result = result.Where(obj => filtroPesquisa.CodigosFiliais.Contains(obj.Carga.Filial.Codigo) || obj.Carga.Pedidos.Any(pedido => filtroPesquisa.CodigosRecebedores.Contains(pedido.Recebedor.CPF_CNPJ)));

            return result;
        }

        #endregion
    }
}
