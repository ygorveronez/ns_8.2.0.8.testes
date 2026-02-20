using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>
    {
        public CentroCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CentroCarregamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> BuscarPorCodigoAsync(int codigo, CancellationToken cancellationToken)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                .Where(x => x.Codigo == codigo).FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>();

            var result = from obj in query where obj.Ativo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamento BuscarPorFilial(int codigoFilial)
        {
            var centroCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                .Where(o => (o.Filial.Codigo == codigoFilial) && o.Ativo)
                .FirstOrDefault();

            return centroCarregamento;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> BuscarPorFiliais(List<int> codigosFiliais)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                .Where(o => (codigosFiliais.Contains(o.Filial.Codigo)) && o.Ativo)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>> BuscarPorFiliaisAsync(List<int> codigosFiliais)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                .Where(o => (codigosFiliais.Contains(o.Filial.Codigo)) && o.Ativo)
                .ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> BuscarPorOperadorLogistica(int codigoUsuario)
        {
            var consultaOperadorFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorFilial>()
                .Where(o => (o.OperadorLogistica.Usuario.Codigo == codigoUsuario) && o.OperadorLogistica.Ativo);

            var consultaCentroCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                .Where(centroCarregamento => centroCarregamento.Ativo && consultaOperadorFilial.Where(operadorFilial => operadorFilial.Filial.Codigo == centroCarregamento.Filial.Codigo).Any());

            return consultaCentroCarregamento.ToList();
        }

        public int ContarPorTipoCargaEFilial(int codigoTipoCarga, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>();

            var result = from obj in query where obj.TiposCarga.Any(o => o.Codigo == codigoTipoCarga) && obj.Filial.Codigo == codigoFilial && obj.Ativo == true select obj;

            return result.Count();
        }




        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamento BuscarPorCargaPedido(int codigoCarga)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>().
                Where(x => x.Carga.Codigo == codigoCarga && x.Pedido.CentroCarregamento != null).Select(x => x.Pedido.CentroCarregamento).FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamento BuscarPorTipoCargaEFilial(int codigoTipoCarga, int codigoFilial, bool ativo, Dominio.Entidades.Embarcador.Cargas.Carga carga = null, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = null)
        {
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = null;

            if (carga != null && configuracaoPedido == null)
                configuracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(this.UnitOfWork).BuscarConfiguracaoPadrao();

            if (carga != null && (configuracaoPedido?.PermitirSelecionarCentroDeCarregamentoNoPedido ?? false))
                centroCarregamento = BuscarPorCargaPedido(carga.Codigo);

            if (centroCarregamento != null)
                return centroCarregamento;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>();
            var result = from obj in query where obj.TiposCarga.Any(o => o.Codigo == codigoTipoCarga) && obj.Filial.Codigo == codigoFilial && obj.Ativo == ativo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamento BuscarPorVeiculo(int codigoCentroCarregamento, int codigoVeiculo)
        {
            var consultaCentroCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                .Where(o => o.Veiculos.Any(veiculo => veiculo.Codigo == codigoVeiculo));

            if (codigoCentroCarregamento > 0)
                consultaCentroCarregamento = consultaCentroCarregamento.Where(o => o.Codigo != codigoCentroCarregamento);

            return consultaCentroCarregamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> BuscarPorVeiculo(int codigoVeiculo)
        {
            var consultaCentroCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                .Where(o => o.Veiculos.Any(veiculo => veiculo.Codigo == codigoVeiculo));

            return consultaCentroCarregamento.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamento BuscarPorCodigoIntegracao(string codigoIntegracao, bool somenteAtivos = false)
        {
            var consultaCentroCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao);

            if (somenteAtivos)
                consultaCentroCarregamento = consultaCentroCarregamento.Where(o => o.Ativo);

            return consultaCentroCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamento BuscarPorDescricao(string descricao, bool somenteAtivos = false)
        {
            var consultaCentroCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                .Where(o => o.Descricao.Equals(descricao));

            if (somenteAtivos)
                consultaCentroCarregamento = consultaCentroCarregamento.Where(o => o.Ativo);

            return consultaCentroCarregamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroCarregamento filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroCarregamento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                result = result.Where(o => filtrosPesquisa.CodigosFilial.Contains(o.Filial.Codigo));

            if (filtrosPesquisa.CodigoTipoCarga > 0)
                result = result.Where(o => o.TiposCarga.Any(tipoCarga => tipoCarga.Codigo == filtrosPesquisa.CodigoTipoCarga));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => !obj.Ativo);

            if (filtrosPesquisa.SomenteCentrosOperadorLogistica && filtrosPesquisa.CodigoOperadorLogistica > 0)
                result = result.Where(o => o.OperadoresLogistica.Any(opl => opl.Codigo == filtrosPesquisa.CodigoOperadorLogistica));

            if (filtrosPesquisa.SomenteCentrosManobra)
                result = result.Where(obj => obj.UtilizarControleManobra);

            return result;
        }

        #endregion
    }
}
