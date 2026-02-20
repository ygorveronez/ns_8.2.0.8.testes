using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteDescarga : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>
    {
        public ClienteDescarga(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ClienteDescarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public bool VerificarSeExisteValorDescargaCliente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            var result = from obj in query where obj.ValorPorPallet > 0m || obj.ValorPorVolume > 0m select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga BuscarPorPessoaSemFetch(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpj select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga BuscarPorPessoa(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpj select obj;

            return result
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.Distribuidor)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> BuscarPorPessoaAsync(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpj select obj;

            return result
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.Distribuidor)
                .FirstOrDefaultAsync();
        }

        public bool PossuiClienteDescarga(double cpfCnpj, List<double> codigosClienteDescarga = null)
        {
            if (codigosClienteDescarga == null)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();
                var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpj select obj;
                return result.Any();
            }
            else
                return codigosClienteDescarga.Where(x => x == cpfCnpj).Any();
        }

        public List<double> BuscarTodosCodigos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();
            var result = from obj in query select obj.Cliente.CPF_CNPJ;
            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> BuscarListaPorPessoa(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpj select obj;

            return result
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.Distribuidor)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> BuscarPorPessoas(List<double> cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            var result = from obj in query where cpfCnpj.Contains(obj.Cliente.CPF_CNPJ) select obj;

            return result
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.Distribuidor)
                .ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>> BuscarPorPessoasAsync(List<double> cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            var result = from obj in query where cpfCnpj.Contains(obj.Cliente.CPF_CNPJ) select obj;

            return await result
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.Distribuidor)
                .ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga BuscarPorOrigemEDestino(double cpfCnpjOrigem, double cpfCnpjDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            var result = from obj in query where obj.ClienteOrigem.CPF_CNPJ == cpfCnpjOrigem && obj.Cliente.CPF_CNPJ == cpfCnpjDestino select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ValorDescarga> ConsultarValorDescarga(int codigoGrupoPessoas, double cpfCnpjPessoa, int codigoLocalidade, List<int> codigosFiliais, List<double> codigosRecebedores, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            //query = query.Where(o => o.ValorPorPallet > 0m);

            if (codigoLocalidade > 0)
                query = query.Where(o => o.Cliente.Localidade.Codigo == codigoLocalidade);

            if (cpfCnpjPessoa > 0)
                query = query.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjPessoa);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.Cliente.GrupoPessoas.Codigo == codigoGrupoPessoas);

            if (codigosFiliais.Any(i => i == -1))
            {
                query = query.Where(o => codigosFiliais.Contains(o.FilialResponsavelRedespacho.Codigo) || codigosRecebedores.Contains(o.Cliente.CPF_CNPJ));
            }

            query = query.OrderBy(propOrdena + " " + dirOrdena);

            if (limite > 0 || inicio > 0)
                query = query.Skip(inicio).Take(limite);

            var resultado = query.Select(o => new Dominio.Relatorios.Embarcador.DataSource.Pallets.ValorDescarga()
            {
                Bairro = o.Cliente.Bairro,
                CEP = o.Cliente.CEP,
                Codigo = o.Codigo,
                CPFCNPJ = o.Cliente.CPF_CNPJ,
                Endereco = o.Cliente.Endereco,
                Estado = o.Cliente.Localidade.Estado.Sigla,
                IE = o.Cliente.IE_RG,
                Localidade = o.Cliente.Localidade.Descricao,
                RazaoSocial = o.Cliente.Nome,
                Telefone = o.Cliente.Telefone1,
                TipoPessoa = o.Cliente.Tipo,
                ValorDescargaPorPallet = o.ValorPorPallet,
                ValorDescargaPorVolume = ((decimal?)o.ValorPorVolume ?? 0m).ToString("n3"),
                ValorDescargaConfiguracao = "",
                Filial = "",
                TipoOperacao = "",
                ModeloVeicular = "",
            }).ToList();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = resultado.Count / quantidadeRegistrosConsultarPorVez;
            List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ValorDescargaConfiguracao> valoresDescargaConfiguracao = new List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ValorDescargaConfiguracao>();
            for (int i = 0; i <= quantidadeConsultas; i++)
                valoresDescargaConfiguracao.AddRange(ConsultarValorDescargaClienteConfiguracao(resultado.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Select(x => x.CPFCNPJ).ToList()));

            foreach (var item in resultado)
            {
                Dominio.Relatorios.Embarcador.DataSource.Pallets.ValorDescargaConfiguracao valorDescargaConfiguracao = valoresDescargaConfiguracao.Where(x => x.CPFCNPJs.Contains(item.CPFCNPJ)).FirstOrDefault();
                if (valorDescargaConfiguracao != null)
                {
                    item.ValorDescargaConfiguracao = valorDescargaConfiguracao.Valor;
                    item.Filial = valorDescargaConfiguracao.Filial;
                    item.TipoOperacao = valorDescargaConfiguracao.TipoOperacao;
                    item.ModeloVeicular = valorDescargaConfiguracao.ModeloVeicular;
                }
            }

            return resultado.ToList();
        }

        public int ContarConsultaValorDescarga(int codigoGrupoPessoas, double cpfCnpjPessoa, int codigoLocalidade, List<int> codigosFiliais, List<double> codigosRecebedores)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            //query = query.Where(o => o.ValorPorPallet > 0m);

            if (codigoLocalidade > 0)
                query = query.Where(o => o.Cliente.Localidade.Codigo == codigoLocalidade);

            if (cpfCnpjPessoa > 0)
                query = query.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjPessoa);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.Cliente.GrupoPessoas.Codigo == codigoGrupoPessoas);

            if (codigosFiliais.Any(i => i == -1))
            {
                query = query.Where(o => codigosFiliais.Contains(o.FilialResponsavelRedespacho.Codigo) || codigosRecebedores.Contains(o.Cliente.CPF_CNPJ));
            }


            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> Consultar(double cnpjPessoa, double cnpjPessoaOrigem, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            if (cnpjPessoa > 0)
                query = query.Where(o => o.Cliente.CPF_CNPJ == cnpjPessoa);

            if (cnpjPessoaOrigem > 0)
                query = query.Where(o => o.ClienteOrigem.CPF_CNPJ == cnpjPessoaOrigem);

            query = query.OrderBy(propOrdena + " " + dirOrdena);

            if (limite > 0 || inicio > 0)
                query = query.Skip(inicio).Take(limite);

            return query.ToList();
        }

        public int ContarConsulta(double cnpjPessoa, double cnpjPessoaOrigem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

            if (cnpjPessoa > 0)
                query = query.Where(o => o.Cliente.CPF_CNPJ == cnpjPessoa);

            if (cnpjPessoaOrigem > 0)
                query = query.Where(o => o.ClienteOrigem.CPF_CNPJ == cnpjPessoaOrigem);

            return query.Count();
        }

        public int BuscarFilialRedespachoPorCliente(double codigoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>()
                .Where(obj => obj.FilialResponsavelRedespacho != null && obj.Cliente.CPF_CNPJ == codigoCliente);

            return query
                .Select(obj => obj.FilialResponsavelRedespacho.Codigo)
                .FirstOrDefault();
        }
        public async Task<int> BuscarFilialRedespachoPorClienteAsync(double codigoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>()
                .Where(obj => obj.FilialResponsavelRedespacho != null && obj.Cliente.CPF_CNPJ == codigoCliente);

            return await query
                .Select(obj => obj.FilialResponsavelRedespacho.Codigo)
                .FirstOrDefaultAsync();
        }

        public decimal ConsultarValorDescargaClienteConfiguracao(double cpfCnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>()
            .Where(x => x.Clientes.FirstOrDefault(y => y.CPF_CNPJ == cpfCnpjPessoa) != null);

            return query
                .Select(obj => obj.Valor)
                .FirstOrDefault();
        }
        public List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ValorDescargaConfiguracao> ConsultarValorDescargaClienteConfiguracao(List<double> cpfCnpjPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>()
            .Where(x => x.Clientes.Any(y => cpfCnpjPessoas.Contains(y.CPF_CNPJ)))
            .Fetch(x => x.TiposOperacoes)
            .ToList();

            return query.Distinct()
                  .Select(obj => new Dominio.Relatorios.Embarcador.DataSource.Pallets.ValorDescargaConfiguracao
                  {
                      CPFCNPJs = obj.Clientes != null && obj.Clientes.Any() ? obj.Clientes.Select(c => c.CPF_CNPJ).ToList() : new List<double>(),
                      Filial = obj.Filial != null ? obj.Filial.Descricao : "",
                      ModeloVeicular = obj.ModeloVeicular != null ? obj.ModeloVeicular.Descricao : "",
                      Valor = obj.Valor.ToString("n3"),
                      TipoOperacao = obj.TiposOperacoes != null && obj.TiposOperacoes.Any() ? string.Join(", ", obj.TiposOperacoes.Select(to => to.Descricao)) : ""
                  })
              .ToList();
        }
        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pessoas.PessoaDescarga> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoaDescarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaPessoaDescarga().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pessoas.PessoaDescarga)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.PessoaDescarga>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoaDescarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaPessoaDescarga().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
