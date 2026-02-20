using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteComplementar : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>
    {
        #region Métodos Construtores

        public ClienteComplementar(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ClienteComplementar(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Métodos Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>();
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar BuscarPorCliente(double codigoCliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>();
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> result = from obj in query where obj.Cliente.CPF_CNPJ == codigoCliente select obj;
            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>> BuscarPorClientesAsync(List<double> codigosClientes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>();
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> result = from obj in query
                                                                                          where codigosClientes.Contains(obj.Cliente.CPF_CNPJ)
                                                                                          select obj;
            return await result.ToListAsync(CancellationToken);
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>> BuscarPorGestoesDevolucaoAsync(List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> gestoDevolucoes)
        {
            List<double> codigosClientes = gestoDevolucoes
                .SelectMany(cli => cli.NotasFiscaisDevolucao)
                .Select(nota => nota.XMLNotaFiscal.Destinatario.CPF_CNPJ)
                .Distinct()
                .ToList();

            return await BuscarPorClientesAsync(codigosClientes);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteComplementar filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteComplementar filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> consultaChamado = Consultar(filtrosPesquisa);

            return ObterLista(consultaChamado, parametrosConsulta);
        }

        public Task<Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteComplementar> BuscarClienteComplementarPorClienteAsync(double codigoCliente)
        {
            var consultaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>().Where(c => c.CPF_CNPJ == codigoCliente);

            var clienteComplementar = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>()
                .Where(c => c.Cliente.CPF_CNPJ == codigoCliente)
                .Select(c => new Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteComplementar
                {
                    Codigo = c.Codigo,
                    Matriz = c.Matriz,
                    EscritorioVendas = c.EscritorioVendas,
                    CpfCnpjCliente = c.Cliente.CPF_CNPJ,
                    ParticionamentoVeiculo = c.ParticionamentoVeiculo,
                    DescricaoParticionamentoVeiculo = c.DescricaoParticionamentoVeiculo,
                    MatrizReferencia = c.MatrizReferencia,
                    SegundaRemessa = c.SegundaRemessa,
                    ExclusividadeEntrega = c.ExclusividadeEntrega,
                    Paletizacao = c.Paletizacao,
                    ClienteStrechado = c.ClienteStrechado,
                    Agendamento = c.Agendamento,
                    ClienteComMulta = c.ClienteComMulta,
                    CapacidadeRecebimento = c.CapacidadeRecebimento,
                    CustoDescarga = c.CustoDescarga,
                    TipoCusto = c.TipoCusto,
                    Ajudantes = c.Ajudantes,
                    PagamentoDescarga = c.PagamentoDescarga,
                    DescricaoPagamentoDescarga = c.DescricaoPagamentoDescarga,
                    AlturaRecebimento = c.AlturaRecebimento,
                    DescricaoAlturaRecebimento = c.DescricaoAlturaRecebimento,
                    RestricaoCarregamento = c.RestricaoCarregamento,
                    DescricaoRestricaoCarregamento = c.DescricaoRestricaoCarregamento,
                    ComposicaoPalete = c.ComposicaoPalete,
                    DescricaoComposicaoPalete = c.DescricaoComposicaoPalete,
                    SegundaFeira = c.SegundaFeira,
                    TercaFeira = c.TercaFeira,
                    QuartaFeira = c.QuartaFeira,
                    QuintaFeira = c.QuintaFeira,
                    SextaFeira = c.SextaFeira,
                    Sabado = c.Sabado,
                    Domingo = c.Domingo
                }).FirstOrDefaultAsync(CancellationToken);

            return clienteComplementar;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteComplementar filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>();

            if (filtrosPesquisa.CodigoCliente > 0)
                query = query.Where(o => o.Cliente.CPF_CNPJ == filtrosPesquisa.CodigoCliente);

            if (!string.IsNullOrEmpty(filtrosPesquisa.EscritorioVenda))
                query = query.Where(o => o.EscritorioVendas.Contains(filtrosPesquisa.EscritorioVenda));

            if (!string.IsNullOrEmpty(filtrosPesquisa.Matriz))
                query = query.Where(o => o.Matriz.Contains(filtrosPesquisa.Matriz));

            return query;
        }

        #endregion Métodos Privados
    }
}
