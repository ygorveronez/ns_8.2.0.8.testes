using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteOutroEndereco : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>
    {
        public ClienteOutroEndereco(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> BuscarEnderecosSemCoordenada(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>()
                .Where(obj => !obj.Cliente.NaoAtualizarDados && (obj.Latitude == string.Empty || obj.Longitude == string.Empty || obj.Latitude == null || obj.Longitude == null) && obj.GeoLocalizacaoStatus == Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.NaoGerado)
                .Fetch(obj => obj.Localidade)
                .Take(limite);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> BuscarPorPessoa(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>()
                .Where(obj => obj.Cliente.CPF_CNPJ == cpfCnpj)
                .Fetch(obj => obj.Localidade);
            return query.Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco BuscarPorCodigoEmbarcador(string codigoEmbarcador, double cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            var result = from obj in query where obj.CodigoEmbarcador == codigoEmbarcador && obj.Cliente.CPF_CNPJ == cliente select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco BuscarPorCodigoLocalidade(int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            var result = from obj in query where obj.Localidade.Codigo == codigoLocalidade select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco BuscarClientePorCodigoIntegracao(string codigoIntegracao, List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> lstClienteOutroEndereco = null)
        {
            if (lstClienteOutroEndereco != null)
                return lstClienteOutroEndereco.Where(x => x.CodigoEmbarcador == codigoIntegracao).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            var result = from obj in query where obj.CodigoEmbarcador == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco BuscarPorIE(string inscricao, Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco, double cliente, List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> lstClienteOutroEndereco = null)
        {
            if (lstClienteOutroEndereco != null)
                return lstClienteOutroEndereco.Find(obj => obj.IE_RG == inscricao && obj.Cliente.CPF_CNPJ == cliente
                                                && (endereco.Logradouro == obj.Endereco || endereco.Bairro == obj.Bairro
                                                || endereco.CEP == obj.CEP || endereco.CodigoIntegracao == obj.CodigoEmbarcador));

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();

            return query
                .Where(obj => obj.IE_RG == inscricao && obj.Cliente.CPF_CNPJ == cliente
                    && (endereco.Logradouro == obj.Endereco || endereco.Bairro == obj.Bairro || endereco.CEP == obj.CEP || endereco.CodigoIntegracao == obj.CodigoEmbarcador))
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco BuscarPorCEPNumeroLocalidade(string cep, string numero, int localidade, double cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            var result = from obj in query where obj.CEP == cep && obj.Numero == numero && obj.Cliente.CPF_CNPJ == cliente select obj;

            if (localidade > 0)
                result = result.Where(obj => obj.Localidade.Codigo == localidade);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            var result = from obj in query where obj.CodigoEmbarcador == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco BuscarPorCodigoEPessoa(int codigo, double cpfcnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            var result = from obj in query where obj.Codigo == codigo && obj.Cliente.CPF_CNPJ == cpfcnpj select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> Consultar(double cliente, int localidade, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);

            if (localidade > 0)
                result = result.Where(obj => obj.Localidade.Codigo == localidade);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(double cliente, int localidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);
            if (localidade > 0)
                result = result.Where(obj => obj.Localidade.Codigo == localidade);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> BuscarPorEnderecosNaoMantidosPorPessoa(List<int> enderecosMantidos, double cnpjcpfCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cnpjcpfCliente && !enderecosMantidos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pessoas.EnderecosSecundarios> ConsultarRelatorioEnderecoSecundario(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroEnderecoSecundario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaGrupoPessoas = new ConsultaEnderecoSecundario().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaGrupoPessoas.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pessoas.EnderecosSecundarios)));

            return consultaGrupoPessoas.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.EnderecosSecundarios>();
        }

        public int ContarConsultaRelatorioEnderecoSecundario(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroEnderecoSecundario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaGrupoPessoas = new ConsultaEnderecoSecundario().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaGrupoPessoas.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
