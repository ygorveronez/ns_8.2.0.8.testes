using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Criterion;
using NHibernate.Linq;
using Repositorio.Embarcador.Veiculos.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class Veiculo : RepositorioBase<Dominio.Entidades.Veiculo>, Dominio.Interfaces.Repositorios.Veiculo
    {
        #region Construtores

        public Veiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Veiculo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Veiculo> BuscarPorCodigo(IList<int> codigos)
        {
            List<Dominio.Entidades.Veiculo> result = new();
            int take = 1000;
            int start = 0;

            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>()
                    .Where(o => tmp.Contains(o.Codigo));

                result.AddRange(query.ToList());

                start += take;
            }

            return result;
        }

        public Dominio.Entidades.Veiculo BuscarPorCodigo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Codigo == codigoVeiculo select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Veiculo> BuscarPorCodigoAsync(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Codigo == codigoVeiculo select obj;
            return await result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Veiculo BuscarPorCodigo(int codigoEmpresa, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Codigo == codigoVeiculo && obj.Empresa.Codigo == codigoEmpresa /*&& obj.Ativo*/ select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Veiculo> BuscarPorCodigo(int[] codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where codigoVeiculo.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarTodosVeiculos()
        {
            IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            return query.ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarCodigosVeiculosConsultarAbastecimento()
        {
            IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            query = query.Where(c => c.AtivarConsultarAbastecimentoAngelLira == true);

            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> BuscarTodosParaIntegracao()
        {
            IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Ativo select obj;
            return result.Select(o => new Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento { Codigo = o.Codigo, Placa = o.Placa, NumeroEquipamentoRastreador = o.NumeroEquipamentoRastreador }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> BuscarTodosParaIntegracao(Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador)
        {
            IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.TecnologiaRastreador == tecnologiaRastreador select obj;

            return result.Select(o => new Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento { Codigo = o.Codigo, Placa = o.Placa, NumeroEquipamentoRastreador = o.NumeroEquipamentoRastreador }).ToList();
        }

        public Dominio.Entidades.Veiculo BuscarPorEquipamento(int codigoEquipamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Equipamentos.Any(e => e.Codigo == codigoEquipamento) select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Veiculo> BuscarPorEquipamentoAsync(int codigoEquipamento, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Equipamentos.Any(e => e.Codigo == codigoEquipamento) select obj;

            return result.FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Veiculo BuscarPorNumeroEquipamento(string numeroEquipamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Equipamentos.Any(e => e.Numero == numeroEquipamento) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorNumeroFogo(string numeroFogo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Pneus.Any(o => o.Pneu.NumeroFogo == numeroFogo) || obj.Estepes.Any(o => o.Pneu.NumeroFogo == numeroFogo) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlaca(int codigoEmpresa, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Placa.Equals(placa) && obj.Ativo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa); //|| veiculo.Empresa.Filiais.Any(emp => emp.Codigo == codigoEmpresa)

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Veiculo> BuscarPorPlacaAsync(int codigoEmpresa, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Placa.Equals(placa) && obj.Ativo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefaultAsync();
        }

        public async Task<List<Dominio.Entidades.Veiculo>> BuscarVeiculosPorEmpresaModeloVeicularPlaca(int codigoEmpresa, int codigoModeloVeicular, string placa)
        {
            if (codigoEmpresa <= 0)
                return [];

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Ativo && obj.Empresa.Codigo == codigoEmpresa select obj;

            if (codigoModeloVeicular > 0)
                result = result.Where(veiculo => veiculo.ModeloVeicularCarga.Codigo == codigoModeloVeicular);

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(veiculo => veiculo.Placa.Contains(placa));

            return await result.ToListAsync();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaETerceiro(string placa, double cnpjTerceiroplaca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Placa.Equals(placa) && obj.Ativo && obj.Tipo == "T" && obj.Proprietario.CPF_CNPJ == cnpjTerceiroplaca select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarTodosPorPlaca(int codigoEmpresa, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Placa.Equals(placa) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa); //|| veiculo.Empresa.Filiais.Any(emp => emp.Codigo == codigoEmpresa)

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaTodas(int codigoEmpresa, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Placa.Equals(placa) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa); //|| veiculo.Empresa.Filiais.Any(emp => emp.Codigo == codigoEmpresa)

            return result.FirstOrDefault();
        }

        public object BuscarPorPlaca()
        {
            throw new NotImplementedException();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaEmpresaDiferente(string placa, int codigoEmpresaDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Placa.Equals(placa) && obj.Ativo select obj;

            if (codigoEmpresaDiferente > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo != codigoEmpresaDiferente);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaVarrendoFiliais(int codigoEmpresa, string placa, bool apenasAtivos = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Placa.Equals(placa) select obj;

            if (apenasAtivos)
                result = result.Where(veiculo => veiculo.Ativo);

            if (codigoEmpresa > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa || veiculo.Empresa.Filiais.Any(emp => emp.Codigo == codigoEmpresa) || veiculo.Empresa.Matriz.Any(emp => emp.Codigo == codigoEmpresa));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaESemEmpresa(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Placa.Equals(placa) && obj.Ativo select obj;

            result = result.Where(veiculo => veiculo.Empresa == null);

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Veiculo> BuscarPorPlacaESemEmpresaAsync(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Placa.Equals(placa) && obj.Ativo select obj;

            result = result.Where(veiculo => veiculo.Empresa == null);

            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaIncluiInativos(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Placa.Equals(placa) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaIncluiInativos(int codigoEmpresa, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Placa.Equals(placa) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa || veiculo.Empresa.Filiais.Any(emp => emp.Codigo == codigoEmpresa));

            return result.OrderBy(o => o.Ativo).FirstOrDefault();
        }

        public List<int> BuscarPorEmpresaCodigoVeiculo(int codigoEmpresa, List<int> codigoVeiculos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where codigoVeiculos.Contains(obj.Codigo) select obj;

            result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa || veiculo.Empresa.Filiais.Any(emp => emp.Codigo == codigoEmpresa));

            return result.Select(x => x.Codigo).ToList();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaComFetch(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Placa.Equals(placa) && obj.Ativo select obj;
            return result.Fetch(o => o.VeiculosVinculados).FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlaca(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Placa.Equals(placa) && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Veiculo> BuscarPorPlacaAsync(string placa, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Placa.Equals(placa) && obj.Ativo select obj;

            return result.FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaMaisRecente(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Placa.Equals(placa) && obj.Ativo select obj;
            return result.OrderByDescending(o => o.DataCadastro).FirstOrDefault();
        }

        public List<Dominio.Entidades.Veiculo> BuscarPorPlacas(List<string> placas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where placas.Contains(obj.Placa) && obj.Ativo select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Veiculo>> BuscarPorPlacasAsync(List<string> placas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where placas.Contains(obj.Placa) && obj.Ativo select obj;
            return result.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Veiculo>> BuscarPlacasPorListaPlacaCodigoEmpresa(List<string> placas, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where placas.Contains(obj.Placa) && obj.Ativo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Veiculo BuscarPlaca(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where placa.Contains(obj.Placa) && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Veiculo> BuscarListaPorPlaca(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where placa.Contains(obj.Placa) && obj.Ativo select obj;
            return result.ToList();
        }

        public List<int> BuscarCodigoVeiculoPorPlaca(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where placa.Contains(obj.Placa) && obj.Ativo select obj;
            return result.Select(x => x.Codigo).ToList();
        }

        public List<int> BuscarCodigosVeiculosPorModelosVeiculares(List<int> codigosModelosVeiculares)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.ModeloVeicularCarga != null && codigosModelosVeiculares.Contains(obj.ModeloVeicularCarga.Codigo) select obj.Codigo;
            return result.ToList();
        }

        public List<int> BuscarCodigosVeiculosAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Ativo select obj.Codigo;
            return result.ToList();
        }

        public bool VeiculoDuplicado(string placa, int codigoDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Placa.Equals(placa) && obj.Codigo != codigoDiferente select obj;
            return result.Count() > 0;
        }

        public void RemoverMotoristaVeiculos(int codigoMotorista)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE Veiculo obj set obj.Motorista = null where obj.Motorista.Codigo = :codigoMotorista")
                .SetInt32("codigoMotorista", codigoMotorista)
                .ExecuteUpdate();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlaca(int codigoVeiculo, string numeroFrota, int codigoMotorista, string tipoVeiculo, string placaVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Ativo select obj;
            Dominio.Entidades.Veiculo reboque = null;

            if (codigoMotorista > 0)
            {
                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

                result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());
            }

            if (codigoVeiculo > 0)
            {
                reboque = BuscarPorPlacaETipoVeiculo(0, codigoVeiculo, "1");
                if (reboque != null)
                    result = result.Where(veiculo => veiculo.VeiculosVinculados.Contains(reboque));
                else
                    result = result.Where(veiculo => veiculo.Codigo == codigoVeiculo);
            }
            else if (!string.IsNullOrWhiteSpace(placaVeiculo) || !string.IsNullOrWhiteSpace(numeroFrota))
            {
                reboque = BuscarPorPlacaETipoVeiculo(0, numeroFrota, placaVeiculo, "1");
                if (reboque != null)
                    result = result.Where(veiculo => veiculo.VeiculosVinculados.Contains(reboque));
                else if (!string.IsNullOrWhiteSpace(placaVeiculo))
                    result = result.Where(veiculo => veiculo.Placa.Contains(placaVeiculo));
            }

            if (reboque == null)
            {
                if (!string.IsNullOrWhiteSpace(tipoVeiculo))
                    result = result.Where(veiculo => veiculo.TipoVeiculo.Equals(tipoVeiculo));

                if (!string.IsNullOrWhiteSpace(numeroFrota))
                    result = result.Where(veiculo => veiculo.NumeroFrota.Equals(numeroFrota));
            }

            return result.Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaETipoVeiculo(int codigoEmpresa, int codigoVeiculo, string tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Codigo.Equals(codigoVeiculo) && obj.TipoVeiculo.Equals(tipo) && obj.Ativo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa);

            //var result = from obj in query where obj.Placa.Equals(placa) && obj.Empresa.Codigo == codigoEmpresa && obj.TipoVeiculo.Equals(tipo) && obj.Ativo select obj;
            return result.Timeout(120).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorIntergacao(int codigo, int inicio, int limite)
        {
            var queryVeiculoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();
            var resultVeiculoIntegracao = from obj in queryVeiculoIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultVeiculoIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntergacao(int codigo)
        {
            var queryVeiculoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao>();
            var resultVeiculoIntegracao = from obj in queryVeiculoIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultVeiculoIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarIntergacaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorPlacaETipoVeiculo(int codigoEmpresa, string numeroFrota, string placa, string tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.TipoVeiculo.Equals(tipo) && obj.Ativo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrEmpty(placa))
                result = result.Where(veiculo => veiculo.Placa.Contains(placa));

            if (!string.IsNullOrEmpty(numeroFrota))
                result = result.Where(veiculo => veiculo.NumeroFrota.Equals(numeroFrota));

            //var result = from obj in query where obj.Placa.Equals(placa) && obj.Empresa.Codigo == codigoEmpresa && obj.TipoVeiculo.Equals(tipo) && obj.Ativo select obj;
            return result.Timeout(120).FirstOrDefault();
        }

        public List<Dominio.Entidades.Veiculo> BuscarPorPlaca(int codigoEmpresa, string[] placas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where placas.Contains(obj.Placa) && obj.Empresa.Codigo == codigoEmpresa && obj.Ativo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Veiculo BuscarPorMotorista(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.TipoVeiculo == "0" select obj;

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Veiculo> BuscarPorMotoristaAsync(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.TipoVeiculo == "0" select obj;

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Veiculo BuscarPorMotorista(int codigoMotorista, string tipoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.TipoVeiculo.Equals(tipoVeiculo) select obj;

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorCodigoMobile(int cobigoMobile)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query select obj;

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.CodigoMobile == cobigoMobile select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Veiculo> BuscarPorCPFMotorista(string cpfMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query select obj;

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.CPF.Equals(cpfMotorista) || (obj.Motorista != null && obj.Motorista.CPF.Equals(cpfMotorista)) select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            return result.ToList();
        }

        public Dominio.Entidades.Veiculo BuscarPorReboque(int codigoReboque)
        {
            Dominio.Entidades.Veiculo reboque = BuscarPorCodigo(codigoReboque);
            if (reboque != null)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
                var result = from obj in query where obj.Ativo && obj.TipoVeiculo.Equals("0") && obj.VeiculosVinculados.Contains(reboque) select obj;
                return result.FirstOrDefault();
            }
            else
                return null;
        }

        public async Task<Dominio.Entidades.Veiculo> BuscarPorReboqueAsync(int codigoReboque, CancellationToken cancellationToken)
        {
            Dominio.Entidades.Veiculo reboque = await BuscarPorCodigoAsync(codigoReboque);

            if (reboque != null)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
                var result = from obj in query where obj.Ativo && obj.TipoVeiculo.Equals("0") && obj.VeiculosVinculados.Contains(reboque) select obj;

                return await result.FirstOrDefaultAsync(cancellationToken);
            }
            else
                return null;
        }

        public int ContarVeiculoVinculado(int codigoEmpresa, string placa, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = (from obj in query where (from x in obj.VeiculosVinculados where x.Ativo && x.Placa == placa select obj).Count() > 0 && obj.Codigo != codigoVeiculo && obj.Ativo && obj.Empresa.Codigo == codigoEmpresa select obj).Count();
            return result;
        }

        public IList<Dominio.Entidades.Veiculo> Consultar(int codigoEmpresa, string placa, string renavam, string tipoVeiculo, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Veiculo>();

            criteria.Add(Restrictions.Eq("Empresa.Codigo", codigoEmpresa));

            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(Restrictions.Eq("Ativo", status == "A"));
            else
                criteria.Add(Restrictions.Eq("Ativo", true));

            if (!string.IsNullOrWhiteSpace(placa))
                criteria.Add(Restrictions.InsensitiveLike("Placa", placa, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(renavam))
                criteria.Add(Restrictions.InsensitiveLike("Renavam", renavam, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(tipoVeiculo))
                criteria.Add(Restrictions.Eq("TipoVeiculo", tipoVeiculo));

            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);

            return criteria.List<Dominio.Entidades.Veiculo>();
        }

        public int ContarConsulta(int codigoEmpresa, string placa, string renavam, string tipoVeiculo, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Veiculo>();

            criteria.Add(Restrictions.Eq("Empresa.Codigo", codigoEmpresa));

            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(Restrictions.Eq("Ativo", status == "A"));
            else
                criteria.Add(Restrictions.Eq("Ativo", true));

            if (!string.IsNullOrWhiteSpace(placa))
                criteria.Add(Restrictions.InsensitiveLike("Placa", placa, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(renavam))
                criteria.Add(Restrictions.InsensitiveLike("Renavam", renavam, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(tipoVeiculo))
                criteria.Add(Restrictions.Eq("TipoVeiculo", tipoVeiculo));

            criteria.SetProjection(Projections.RowCount());

            return criteria.UniqueResult<int>();
        }

        public List<Dominio.Entidades.Veiculo> ConsultarEmbarcadorGestaoCarga(string placa, int codigoMotorista, bool apenasTracao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Ativo select obj;

            if (codigoMotorista > 0)
            {
                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

                result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());
            }

            if (apenasTracao)
                result = result.Where(veiculo => veiculo.TipoVeiculo == "0");

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(veiculo => veiculo.Placa.Contains(placa));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ConsultarEmbarcadorGestaoCarga(string placa, int codigoMotorista, bool apenasTracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Ativo select obj;

            if (codigoMotorista > 0)
            {
                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

                result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());
            }

            if (apenasTracao)
                result = result.Where(veiculo => veiculo.TipoVeiculo == "0");

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(veiculo => veiculo.Placa.Contains(placa));

            return result.Count();
        }

        public List<Dominio.Entidades.Veiculo> ConsultarEmbarcadorMovimentosDePlacas(string placa, int tipoReboque, string numeroFrota, bool somenteReboquesPendentes, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Ativo && obj.TipoVeiculo.Equals("1") select obj;

            if (tipoReboque > 0)
                result = result.Where(veiculo => veiculo.ModeloVeicularCarga.Codigo == tipoReboque);

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(veiculo => veiculo.Placa.Contains(placa));

            if (!string.IsNullOrWhiteSpace(numeroFrota))
                result = result.Where(veiculo => veiculo.NumeroFrota.Equals(numeroFrota));

            if (somenteReboquesPendentes)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
                result = result.Where(o => (from obj in queryVeiculos where obj.Ativo && obj.TipoVeiculo.Equals("0") && obj.VeiculosVinculados.Contains(o) select obj.Codigo).Count() == 0);
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public bool ContemVinculoEmTracao(Dominio.Entidades.Veiculo reboque)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Ativo && obj.TipoVeiculo.Equals("1") && obj.Codigo == reboque.Codigo select obj;
            var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            result = result.Where(o => (from obj in queryVeiculos where obj.Ativo && obj.TipoVeiculo.Equals("0") && obj.VeiculosVinculados.Contains(o) select obj.Codigo).Count() >= 1);
            return result.Count() >= 1;
        }

        public string PlacaVinculoEmTracao(Dominio.Entidades.Veiculo reboque)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Ativo && obj.TipoVeiculo.Equals("0") && obj.VeiculosVinculados.Contains(reboque) select obj;
            if (result.Count() >= 1)
                return result.FirstOrDefault().Placa;
            else
                return "";
        }

        public int ConsultarEmbarcadorMovimentosDePlacas(string placa, int tipoReboque, string numeroFrota, bool somenteReboquesPendentes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Ativo && obj.TipoVeiculo.Equals("1") select obj;

            if (tipoReboque > 0)
                result = result.Where(veiculo => veiculo.ModeloVeicularCarga.Codigo == tipoReboque);

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(veiculo => veiculo.Placa.Contains(placa));

            if (!string.IsNullOrWhiteSpace(numeroFrota))
                result = result.Where(veiculo => veiculo.NumeroFrota.Equals(numeroFrota));

            if (somenteReboquesPendentes)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
                result = result.Where(o => (from obj in queryVeiculos where obj.Ativo && obj.TipoVeiculo.Equals("0") && obj.VeiculosVinculados.Contains(o) select obj.Codigo).Count() == 0);
            }

            return result.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramentoTecnologia> ConsultarVeiculoMonitoramento(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = ConsultaVeiculoMonitoramento(filtrosPesquisa, parametroConsulta);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramentoTecnologia)));
            var veiculos = consulta.List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramentoTecnologia>();
            return veiculos;
        }

        public int ContarVeiculoMonitoramento(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa)
        {
            var consulta = ConsultaVeiculoMonitoramento(filtrosPesquisa, count: true);
            int total = consulta.UniqueResult<int>();
            return total;
        }

        public List<Dominio.Entidades.Veiculo> ConsultarEmbarcador(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaVeiculo = ConsultarEmbarcador(filtrosPesquisa);

            consultaVeiculo = consultaVeiculo
                .Fetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.Empresa)
                .Fetch(o => o.Estado)
                .Fetch(o => o.SegmentoVeiculo)
                .Fetch(o => o.Proprietario)
                .Fetch(o => o.Marca)
                .Fetch(o => o.Modelo)
                .Fetch(o => o.CentroResultado);

            return ObterLista(consultaVeiculo, parametroConsulta);
        }

        public int ContarConsultaEmbarcador(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa)
        {
            var consultaVeiculo = ConsultarEmbarcador(filtrosPesquisa);

            return consultaVeiculo.Count();
        }

        public List<Dominio.Entidades.Veiculo> ConsultarEmbarcadorSomenteDisponiveis(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaVeiculo = ConsultarEmbarcadorSomenteDisponiveis(filtrosPesquisa);

            consultaVeiculo = consultaVeiculo
                .Fetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.Empresa)
                .Fetch(o => o.Estado)
                .Fetch(o => o.SegmentoVeiculo)
                .Fetch(o => o.Proprietario)
                .Fetch(o => o.Marca)
                .Fetch(o => o.Modelo)
                .Fetch(o => o.CentroResultado);

            return ObterLista(consultaVeiculo, parametroConsulta);
        }

        public int ContarConsultaEmbarcadorSomenteDisponiveis(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa)
        {
            var consultaVeiculo = ConsultarEmbarcadorSomenteDisponiveis(filtrosPesquisa);

            return consultaVeiculo.Count();
        }

        public List<Dominio.Entidades.Veiculo> ConsultarEmbarcadorSomenteEmEscala(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaVeiculo = ConsultarEmbarcadorSomenteEmEscala(filtrosPesquisa);

            consultaVeiculo = consultaVeiculo
                .Fetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.Empresa)
                .Fetch(o => o.Estado)
                .Fetch(o => o.SegmentoVeiculo)
                .Fetch(o => o.Proprietario)
                .Fetch(o => o.Marca)
                .Fetch(o => o.Modelo)
                .Fetch(o => o.CentroResultado);

            return ObterLista(consultaVeiculo, parametroConsulta);
        }

        public int ContarConsultaEmbarcadorSomenteEmEscala(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa)
        {
            var consultaVeiculo = ConsultarEmbarcadorSomenteEmEscala(filtrosPesquisa);

            return consultaVeiculo.Count();
        }

        public List<Dominio.Entidades.Veiculo> ConsultarEmbarcadorPorTipoPropriedadeVeiculoDoTipoOperacao(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            var consultaVeiculo = ConsultarEmbarcadorPorTipoPropriedadeVeiculoDoTipoOperacao(filtrosPesquisa, tipoOperacao);

            consultaVeiculo = consultaVeiculo
                .Fetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.Empresa)
                .Fetch(o => o.Estado)
                .Fetch(o => o.SegmentoVeiculo)
                .Fetch(o => o.Proprietario)
                .Fetch(o => o.Marca)
                .Fetch(o => o.Modelo)
                .Fetch(o => o.CentroResultado);

            return ObterLista(consultaVeiculo, parametroConsulta);
        }

        public int ContarConsultaEmbarcadorPorTipoPropriedadeVeiculoDoTipoOperacao(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            var consultaVeiculo = ConsultarEmbarcadorPorTipoPropriedadeVeiculoDoTipoOperacao(filtrosPesquisa, null, tipoOperacao);

            return consultaVeiculo.Count();
        }

        public List<Dominio.Entidades.Veiculo> ConsultarPainelVeiculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaPainelVeiculo filtro, bool apenasTracao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaVeiculo = ConsultarPainelVeiculo(filtro, apenasTracao);

            return consultaVeiculo
                         .Fetch(o => o.ModeloVeicularCarga)
                         .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros).ToList();
        }

        public int ContarConsultaConsultarPainelVeiculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaPainelVeiculo filtro, bool apenasTracao)
        {
            var consultaVeiculo = ConsultarPainelVeiculo(filtro, apenasTracao);

            return consultaVeiculo.Count();
        }

        public string ContarClientesCargaViagem(int CodigoVeiculo)
        {
            string sql = @"
                                SELECT SUBSTRING((SELECT DISTINCT ', ' + CAST(CASE
                                WHEN CA.PED_TIPO_TOMADOR = 0 THEN REM.CLI_NOME
                                WHEN CA.PED_TIPO_TOMADOR = 3 THEN DEST.CLI_NOME
                                WHEN CA.PED_TIPO_TOMADOR = 4 THEN OUTRO.CLI_NOME
                                WHEN CA.PED_TIPO_TOMADOR = 2 THEN RECEB.CLI_NOME
                                WHEN CA.PED_TIPO_TOMADOR = 1 THEN EXPE.CLI_NOME
                                ELSE ''
                                END AS NVARCHAR(2000))
                                FROM T_CARGA C
                                JOIN T_CARGA_PEDIDO CA ON CA.CAR_CODIGO = C.CAR_CODIGO
                                JOIN T_PEDIDO P ON P.PED_CODIGO = CA.PED_CODIGO
                                LEFT OUTER JOIN T_CLIENTE REM ON REM.CLI_CGCCPF = P.CLI_CODIGO_REMETENTE
                                LEFT OUTER JOIN T_CLIENTE DEST ON DEST.CLI_CGCCPF = P.CLI_CODIGO
                                LEFT OUTER JOIN T_CLIENTE OUTRO ON OUTRO.CLI_CGCCPF = P.CLI_CODIGO_TOMADOR
                                LEFT OUTER JOIN T_CLIENTE RECEB ON RECEB.CLI_CGCCPF = P.CLI_CODIGO_RECEBEDOR
                                LEFT OUTER JOIN T_CLIENTE EXPE ON EXPE.CLI_CGCCPF = P.CLI_CODIGO_EXPEDIDOR
                                WHERE C.CAR_SITUACAO NOT IN (0, 1, 2, 4, 5, 6, 11, 13, 15, 17, 18)
                                AND C.CAR_VEICULO = " + CodigoVeiculo + " FOR XML PATH('')), 3, 2000) ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            return query.UniqueResult<string>();
        }




        public List<string> ObterVinculosExistentes(int codigoEmpresa, int codigoVeiculoPai, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = (from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo != codigoVeiculoPai && obj.VeiculosVinculados.Contains(new Dominio.Entidades.Veiculo() { Codigo = codigoVeiculo }) select obj.Placa);

            return result.ToList();
        }

        //public Dominio.Entidades.Veiculo BuscarVeiculoPai(int codigoEmpresa, string placa)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
        //    var result = from obj in query where (from veiVinc in obj.VeiculosVinculados where veiVinc.Placa.Equals(placa) && veiVinc.Empresa.Codigo == codigoEmpresa && veiVinc.Ativo select veiVinc.Codigo).Count() > 0 && obj.Empresa.Codigo == codigoEmpresa && obj.Ativo select obj;
        //    return result.FirstOrDefault();
        //}

        public List<Dominio.Entidades.Veiculo> BuscarVeiculoPai(int codigoEmpresa, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where (from veiVinc in obj.VeiculosVinculados where veiVinc.Placa.Equals(placa) && veiVinc.Empresa.Codigo == codigoEmpresa && veiVinc.Ativo select veiVinc.Codigo).Count() > 0 && obj.Empresa.Codigo == codigoEmpresa && obj.Ativo select obj;
            return result.Timeout(120).ToList();
        }

        public Dominio.Entidades.Veiculo BuscarVeiculoPai(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.VeiculosVinculados.Contains(new Dominio.Entidades.Veiculo() { Codigo = codigoVeiculo }) && obj.Ativo select obj;
            return result.Timeout(120).FirstOrDefault();
        }

        public List<Dominio.Entidades.Veiculo> BuscarPorEmpresa(int codigoEmpresa, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "A")
                    result = result.Where(o => o.Ativo);
                else
                    result = result.Where(o => !o.Ativo);
            }

            return result.ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarPorEmpresas(List<int> codigosEmpresas, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where codigosEmpresas.Contains(obj.Empresa.Codigo) select obj;

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "A")
                    result = result.Where(o => o.Ativo);
                else
                    result = result.Where(o => !o.Ativo);
            }

            return result.Fetch(x => x.ModeloVeicularCarga)
                         .ToList();
        }

        public List<Dominio.Entidades.Veiculo> ObterRelatorio(int codigoEmpresa, string tipoPropriedade, string tipoVeiculo, string tipoRodado, string tipoCarroceria, string status, bool somenteQueNaoEstaoVinculadosAOutroVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(tipoPropriedade))
                result = result.Where(o => o.Tipo.Equals(tipoPropriedade));

            if (!string.IsNullOrWhiteSpace(tipoVeiculo))
                result = result.Where(o => o.TipoVeiculo.Equals(tipoVeiculo));

            if (!string.IsNullOrWhiteSpace(tipoRodado))
                result = result.Where(o => o.TipoRodado.Equals(tipoRodado));

            if (!string.IsNullOrWhiteSpace(tipoCarroceria))
                result = result.Where(o => o.TipoCarroceria.Equals(tipoCarroceria));

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "A")
                    result = result.Where(o => o.Ativo);
                else
                    result = result.Where(o => !o.Ativo);
            }

            if (somenteQueNaoEstaoVinculadosAOutroVeiculo)
                result = result.Where(o => !(from obj in query where obj.VeiculosVinculados.Contains(o) select obj).Any());

            return result.Fetch(o => o.Marca).Fetch(o => o.Modelo).Fetch(o => o.VeiculosVinculados).Timeout(120).ToList();
        }

        public Dominio.Entidades.Produto BuscarCombustivelPadrao(int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Codigo == codigoVeiculo select obj;
            //if ((int)tipoAbastecimento > 0)
            //{
            //    if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla)
            //        result = result.Where(o => o.Modelo.SimNao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim && o.Modelo.Produto.CodigoNCM.StartsWith("310210"));
            //    else if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel)
            //        result = result.Where(o => o.Modelo.SimNao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao && o.Modelo.Produto.CodigoNCM.StartsWith("2710004"));
            //}

            IQueryable<Dominio.Entidades.Produto> queryProduto = result.Select(obj => obj.Modelo.Produto);
            return queryProduto.FirstOrDefault();
        }

        public Dominio.Entidades.Produto BuscarCombustivelAbastecimento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();
            var result = from obj in query select obj;
            if ((int)tipoAbastecimento > 0)
            {
                if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla)
                    result = result.Where(o => o.CodigoNCM.StartsWith("310210") && o.ProdutoCombustivel.HasValue && o.ProdutoCombustivel.Value == true);
                else if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel)
                    result = result.Where(o => o.CodigoNCM.StartsWith("2710004") && o.ProdutoCombustivel.HasValue && o.ProdutoCombustivel.Value == true);
            }
            if (result.Count() == 1)
                return result.FirstOrDefault();
            else
                return null;
        }

        public List<int> BuscarCodigosTransportadoresPorTipoVeiculo(int codigoCentroCarregamento, int numeroPallets, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Empresa != null && obj.Empresa.Status == "A" && obj.Ativo && ((modelosDisponiveis.Contains(obj.ModeloVeicularCarga) && obj.ModeloVeicularCarga.NumeroPaletes >= numeroPallets) || obj.ModeloVeicularCarga == null) && obj.Empresa.CentrosCarregamento.Any(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento) select obj.Empresa.Codigo;

            return result.Distinct().ToList();
        }

        public List<int> BuscarCodigosTransportadoresPorTipoVeiculo(int numeroPallets, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Empresa != null && obj.Empresa.Status == "A" && obj.Ativo && ((obj.ModeloVeicularCarga != null && modelosDisponiveis.Contains(obj.ModeloVeicularCarga) && obj.ModeloVeicularCarga.NumeroPaletes >= numeroPallets) || obj.ModeloVeicularCarga == null) select obj;

            return result.Select(obj => obj.Empresa.Codigo).Distinct().ToList();
        }

        public List<double> BuscarCodigosTransportadoresTerceiros()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Proprietario != null && obj.Tipo == "T" && obj.Ativo select obj;

            return result.Select(obj => obj.Proprietario.CPF_CNPJ).Distinct().ToList();
        }

        public List<double> BuscarCodigosTransportadoresTerceirosPorTipoVeiculo(int codigoCentroCarregamento, int numeroPallets, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Proprietario != null && obj.Tipo == "T" && obj.Ativo && ((modelosDisponiveis.Contains(obj.ModeloVeicularCarga) && obj.ModeloVeicularCarga.NumeroPaletes >= numeroPallets) || obj.ModeloVeicularCarga == null) && obj.Empresa.CentrosCarregamento.Any(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento) select obj.Proprietario.CPF_CNPJ;

            return result.Distinct().ToList();
        }
        public List<double> BuscarCodigosTransportadoresTerceirosPorTipoVeiculoProprietario(List<double> transportadoresTerceiros, int numeroPallets, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Proprietario != null && obj.Tipo == "T" && obj.Ativo && ((modelosDisponiveis.Contains(obj.ModeloVeicularCarga) && obj.ModeloVeicularCarga.NumeroPaletes >= numeroPallets) || obj.ModeloVeicularCarga == null) && transportadoresTerceiros.Contains(obj.Proprietario.CPF_CNPJ) select obj.Proprietario.CPF_CNPJ;

            return result.Distinct().ToList();
        }
        public List<double> BuscarCodigosTransportadoresTerceirosPorFilaCarregamento(int numeroPallets, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var queryFila = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();

            var result = from obj in query
                         where obj.Proprietario != null && obj.Tipo == "T" && obj.Ativo && ((modelosDisponiveis.Contains(obj.ModeloVeicularCarga) && obj.ModeloVeicularCarga.NumeroPaletes >= numeroPallets) || obj.ModeloVeicularCarga == null)
                         && queryFila.Any(x => x.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel && x.ConjuntoVeiculo.Tracao.Codigo == obj.Codigo)
                         select obj.Proprietario.CPF_CNPJ;

            return result.Distinct().ToList();
        }

        public List<double> BuscarCodigosTransportadoresTerceirosPorTipoVeiculo(int numeroPallets, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDisponiveis)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Proprietario != null && obj.Tipo == "T" && obj.Ativo && ((obj.ModeloVeicularCarga != null && modelosDisponiveis.Contains(obj.ModeloVeicularCarga) && obj.ModeloVeicularCarga.NumeroPaletes >= numeroPallets) || obj.ModeloVeicularCarga == null) select obj;

            return result.Select(obj => obj.Proprietario.CPF_CNPJ).Distinct().ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarPorProprietario(double cpfCnpjProprietario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Ativo && obj.Proprietario.CPF_CNPJ == cpfCnpjProprietario select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaPorProprietario(double cpfCnpjProprietario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Ativo && obj.Proprietario.CPF_CNPJ == cpfCnpjProprietario select obj.Empresa;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPrimeiroPorProprietario(double cpfCnpjProprietario)
        {
            IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(obj => obj.Ativo && obj.Proprietario.CPF_CNPJ == cpfCnpjProprietario);

            return query.FirstOrDefault();
        }

        public int BuscarRNTRCPorProprietario(double cpfCnpjProprietario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Ativo && obj.Proprietario.CPF_CNPJ == cpfCnpjProprietario && obj.RNTRC > 0 select obj.RNTRC;

            return result.FirstOrDefault();
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ClassificacaoVeiculo> ConsultarClassificacaoVeiculo(int codigoTransportador, int codigoModeloVeiculo, int codigoModeloCarroceria, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.ModeloCarroceria != null);

            if (codigoTransportador > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoTransportador);

            if (codigoModeloVeiculo > 0)
                query = query.Where(o => o.ModeloVeicularCarga.Codigo == codigoModeloVeiculo);

            if (codigoModeloCarroceria > 0)
                query = query.Where(o => o.ModeloCarroceria.Codigo == codigoModeloCarroceria);

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.Select(o => new Dominio.Relatorios.Embarcador.DataSource.Veiculos.ClassificacaoVeiculo()
            {
                Ativo = o.Ativo,
                CapacidadeKG = o.CapacidadeKG,
                CapacidadeM3 = o.CapacidadeM3,
                CNPJTransportador = o.Empresa.CNPJ,
                Codigo = o.Codigo,
                Estado = o.Estado.Sigla,
                ModeloCarroceria = o.ModeloCarroceria.Descricao,
                ModeloVeiculo = o.ModeloVeicularCarga.Descricao,
                PercentualAdicionalFrete = o.ModeloCarroceria.PercentualAdicionalFrete,
                Placa = o.Placa,
                RENAVAM = o.Renavam,
                Tara = o.Tara,
                TipoCarroceria = o.TipoCarroceria,
                TipoPropriedade = o.Tipo,
                TipoRodado = o.TipoRodado,
                TipoVeiculo = o.TipoVeiculo,
                Transportador = o.Empresa.RazaoSocial
            }).ToList();
        }

        public async Task<List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ClassificacaoVeiculo>> ConsultarClassificacaoVeiculoAsync(int codigoTransportador, int codigoModeloVeiculo, int codigoModeloCarroceria, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.ModeloCarroceria != null);

            if (codigoTransportador > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoTransportador);

            if (codigoModeloVeiculo > 0)
                query = query.Where(o => o.ModeloVeicularCarga.Codigo == codigoModeloVeiculo);

            if (codigoModeloCarroceria > 0)
                query = query.Where(o => o.ModeloCarroceria.Codigo == codigoModeloCarroceria);

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return await query.Select(o => new Dominio.Relatorios.Embarcador.DataSource.Veiculos.ClassificacaoVeiculo()
            {
                Ativo = o.Ativo,
                CapacidadeKG = o.CapacidadeKG,
                CapacidadeM3 = o.CapacidadeM3,
                CNPJTransportador = o.Empresa.CNPJ,
                Codigo = o.Codigo,
                Estado = o.Estado.Sigla,
                ModeloCarroceria = o.ModeloCarroceria.Descricao,
                ModeloVeiculo = o.ModeloVeicularCarga.Descricao,
                PercentualAdicionalFrete = o.ModeloCarroceria.PercentualAdicionalFrete,
                Placa = o.Placa,
                RENAVAM = o.Renavam,
                Tara = o.Tara,
                TipoCarroceria = o.TipoCarroceria,
                TipoPropriedade = o.Tipo,
                TipoRodado = o.TipoRodado,
                TipoVeiculo = o.TipoVeiculo,
                Transportador = o.Empresa.RazaoSocial
            }).ToListAsync();
        }


        public int ContarConsultaClassificacaoVeiculo(int codigoTransportador, int codigoModeloVeiculo, int codigoModeloCarroceria, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.ModeloCarroceria != null);

            if (codigoTransportador > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoTransportador);

            if (codigoModeloVeiculo > 0)
                query = query.Where(o => o.ModeloVeicularCarga.Codigo == codigoModeloVeiculo);

            if (codigoModeloCarroceria > 0)
                query = query.Where(o => o.ModeloCarroceria.Codigo == codigoModeloCarroceria);

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query.Count();
        }

        public List<Dominio.Entidades.Veiculo> ConsultarVeiculos(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var query = MontarConsulta(filtrosPesquisa);

            query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (limite > inicio)
                query = query.Skip(inicio).Take(limite);

            return query.ToList();
        }

        public int ContarConsultarVeiculos(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa)
        {
            var query = MontarConsulta(filtrosPesquisa);

            return query.Count();
        }

        public Dominio.Entidades.Veiculo ValidaDuplicadadeCIOT(string ciot, int veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query
                         where obj.CIOT == ciot && obj.Codigo != veiculo
                         select obj;

            result = result.Where(o => !o.VeiculosTracao.Any(vei => vei.Codigo == veiculo) && !o.VeiculosVinculados.Any(vei => vei.Codigo == veiculo));

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculosPorMotorista(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query select obj;

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            return result.ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculosPorMotorista(int codigoMotorista, int codigoVeiculoDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query select obj;

            if (codigoVeiculoDiferente > 0)
                result = result.Where(o => o.Codigo != codigoVeiculoDiferente);

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            return result.ToList();
        }

        public Dominio.Entidades.Veiculo BuscarVeiculosPorEquipamento(int codigoEquipamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Equipamentos.Any(e => e.Codigo == codigoEquipamento) select obj;

            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Veiculo BuscarVeiculoPorMotorista(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Motoristas.Any(e => e.Motorista.Codigo == codigoMotorista) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarVeiculosPorRastreador(string NumeroEquipamentoRastreador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.NumeroEquipamentoRastreador == NumeroEquipamentoRastreador select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarVeiculoPorTodosDadosRastreador(int codTecnologiaRastreador, int codTipoComunicacaoRastreador, string NumeroEquipamentoRastreador, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.TecnologiaRastreador.Codigo == codTecnologiaRastreador && obj.TipoComunicacaoRastreador.Codigo == codTipoComunicacaoRastreador && obj.NumeroEquipamentoRastreador == NumeroEquipamentoRastreador && obj.Placa != placa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Veiculo> BuscarTracaoPorReboque(int codigoReboque)
        {
            Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo
            {
                ApenasTracao = true,
                CodigoReboque = codigoReboque
            };

            var consultaVeiculo = ConsultarEmbarcador(filtrosPesquisa);

            return consultaVeiculo.ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarPorSegmento(int codigoSegmentoVeiculo)
        {
            IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.SegmentoVeiculo.Codigo == codigoSegmentoVeiculo && o.Ativo == true);

            return query.ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarPorNumeroDaFrota(string numeroFrota)
        {
            IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.NumeroFrota == numeroFrota && o.Ativo == true);

            return query.ToList();
        }

        public List<string> BuscarFotasVeiculos()
        {
            IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.NumeroFrota != null && o.NumeroFrota != "" && o.Ativo == true);

            return query.Select(o => o.NumeroFrota).Distinct().ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarTodosVeiculosAtivos(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query
                         where
                         obj.Ativo == true
                         && obj.Tipo.Equals("P")
                         select obj;
            return result.Skip(inicio).Take(limite).ToList();
        }

        public int ContarTodosVeiculosAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query
                         where
                         obj.Ativo == true
                         && obj.Tipo.Equals("P")
                         select obj;
            return result.Count();
        }

        public List<string> BuscarFotasVeiculosDisponiblidade()
        {
            IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.NumeroFrota != null && o.NumeroFrota != "" && o.Ativo == true && o.PossuiControleDisponibilidade == true);

            return query.Select(o => o.NumeroFrota).Distinct().ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Veiculo> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCheque = new ConsultaVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCheque.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.Veiculo)));

            return consultaCheque.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Veiculo>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Veiculo>> ConsultarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCheque = new ConsultaVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCheque.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.Veiculo)));

            return await consultaCheque.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Veiculo>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCheque = new ConsultaVeiculo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCheque.SetTimeout(600).UniqueResult<int>();
        }

        public List<string> BuscarPlacas(List<int> codigosVeiculos)
        {
            IQueryable<Dominio.Entidades.Veiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => codigosVeiculos.Contains(o.Codigo));

            return query.Select(o => o.Placa).ToList();
        }

        public int ContarVeiculosNaoDisponiveis()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.SituacaoVeiculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel select obj;

            return result.Count();
        }

        public Dominio.Entidades.Veiculo BuscarPorChassi(string chassi)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Chassi.Contains(chassi) && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Veiculo> BuscarPorChassiAsync(string chassi, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Chassi.Contains(chassi) && obj.Ativo select obj;

            return result.FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Veiculo BuscarPorChassiEPlaca(int codigoEmpresa, string chassi, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Chassi.Contains(chassi) && obj.Ativo && obj.Placa != placa select obj;

            if (codigoEmpresa > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa || veiculo.Empresa.Filiais.Any(emp => emp.Codigo == codigoEmpresa) || veiculo.Empresa.Matriz.Any(emp => emp.Codigo == codigoEmpresa));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarPorRenavam(int codigoEmpresa, string renavam, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Renavam.Contains(renavam) && obj.Ativo && obj.Placa != placa select obj;

            if (codigoEmpresa > 0)
                result = result.Where(veiculo => veiculo.Empresa.Codigo == codigoEmpresa || veiculo.Empresa.Filiais.Any(emp => emp.Codigo == codigoEmpresa) || veiculo.Empresa.Matriz.Any(emp => emp.Codigo == codigoEmpresa));

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculosSemCargaDesdeDataLimite(DateTime dataLimiteSemCarga)
        {
            var queryVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>()
                .Where(o => o.DataCadastro == null || o.DataCadastro.Value.Date <= dataLimiteSemCarga);

            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(o => o.DataCriacaoCarga.Date >= dataLimiteSemCarga)
                .Where(o => o.Veiculo != null);

            var result = from obj in queryVeiculo
                         where obj.Ativo
                            && !queryCarga.Any(cv => cv.Veiculo.Codigo == obj.Codigo)
                            && !queryCarga.Any(cv => cv.VeiculosVinculados.Any(vv => vv.Codigo == obj.Codigo))
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculosAtivosComMotoristasOuReboques()
        {
            var queryfrota = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Frota>()
               .Where(o => o.Ativo);

            var queryVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>()
                .Where(o => o.Ativo && !queryfrota.Any(fr => fr.Veiculo.Codigo == o.Codigo || fr.Reboque1.Codigo == o.Codigo || fr.Reboque2.Codigo == o.Codigo));

            return queryVeiculo.ToList();
        }

        public bool ExisteOutroCadastroMesmaPlacaBloqueado(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            query = query.Where(v => v.Placa == placa && v.VeiculoBloqueado);
            return query.FirstOrDefault() != null;
        }
        public List<Dominio.Entidades.Veiculo> BuscarVeiculosMesmaPlacaSemBloqueio(string placa, bool VeiculosBloqueados = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            query = query.Where(v => v.Placa == placa && v.VeiculoBloqueado == VeiculosBloqueados);
            return query.ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculosPorPeriodoAtualizacao(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>()
                .Where(v => v.DataAtualizacao >= dataInicial && v.DataAtualizacao <= dataFinal);

            return query.ToList();
        }

        public Dominio.Entidades.Veiculo BuscarRastreadorPorPlaca(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>()
                .Where(v => v.Placa.Equals(placa) && v.PossuiRastreador && v.TecnologiaRastreador != null && v.NumeroEquipamentoRastreador != null);

            return query.FirstOrDefault();
        }

        public bool ExisteVinculadoNaEmpresaPorPlaca(string placa, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            return query.Any(x => x.Placa == placa && x.Empresa.Codigo == codigoEmpresa);
        }

        public IList<int> BuscarVeiculosVinculadoACarga(int codigoCarga)
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT VEI_CODIGO");
            sql.AppendLine("FROM T_CARGA_VEICULOS_VINCULADOS ");
            sql.AppendLine("WHERE CAR_CODIGO = :codigoCarga");

            var query = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());
            query.SetInt32("codigoCarga", codigoCarga);

            return query.List<int>();
        }


        public int DeletarVeiculosCarga(int codigoCarga, List<int> codigosVeiculo)
        {
            if (codigosVeiculo == null || codigosVeiculo.Count == 0)
                return 0;

            var placeholders = string.Join(", ", codigosVeiculo.Select((_, index) => $":codigosVeiculo{index}"));

            var sql = $"delete from T_CARGA_VEICULOS_VINCULADOS where CAR_CODIGO = {codigoCarga} AND VEI_CODIGO IN ({placeholders})"; // SQL-INJECTION-SAFE

            var query = UnitOfWork.Sessao.CreateSQLQuery(sql);

            for (int i = 0; i < codigosVeiculo.Count; i++)
            {
                query.SetInt32($"codigosVeiculo{i}", codigosVeiculo[i]);
            }

            return query.SetTimeout(6000).ExecuteUpdate();
        }
        public Task<List<int>> BuscarCodigosTodosReboquesAsync()
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>()
                .Where(v => v.TipoVeiculo == "1")
                .Select(v => v.Codigo)
                .ToListAsync();
        }

        public bool TipoTerceiro(string placa)
        {
            return SessionNHiBernate
                .Query<Dominio.Entidades.Veiculo>()
                .Any(v => v.Placa == placa && v.Tipo == "T");
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Veiculo> Consultar(IQueryable<Dominio.Entidades.Veiculo> consultaVeiculo, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa)
        {
            if (filtrosPesquisa.Proprietario != null)
                consultaVeiculo = consultaVeiculo.Where(veiculo => (veiculo.Proprietario.CPF_CNPJ == filtrosPesquisa.Proprietario.CPF_CNPJ || veiculo.Empresa.CNPJ == filtrosPesquisa.Proprietario.CPF_CNPJ_SemFormato));

            if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Ativo);
            else if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaVeiculo = consultaVeiculo.Where(veiculo => !veiculo.Ativo);

            if (filtrosPesquisa.PendenteIntegracaoEmbarcador)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.PendenteIntegracaoEmbarcador);

            if (filtrosPesquisa.CodigoMarcaVeiculo > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Marca.Codigo == filtrosPesquisa.CodigoMarcaVeiculo);

            if (filtrosPesquisa.CodigoModeloVeiculo > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Modelo.Codigo == filtrosPesquisa.CodigoModeloVeiculo);

            if (filtrosPesquisa.LocalAtualFisicoDoVeiculo > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.LocalAtualFisicoDoVeiculo.CPF_CNPJ == filtrosPesquisa.LocalAtualFisicoDoVeiculo);

            if (filtrosPesquisa.CodigoTecnologiaRastreador > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.TecnologiaRastreador.Codigo == filtrosPesquisa.CodigoTecnologiaRastreador);

            if (filtrosPesquisa.FiltrarCadastrosAprovados)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.SituacaoCadastro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroVeiculo.Aprovado);

            if (filtrosPesquisa.CodigosEmpresa?.Count > 0)
            {
                var consultaEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                    .Where(o => filtrosPesquisa.CodigosEmpresa.Contains(o.Codigo));

                consultaVeiculo = consultaVeiculo.Where(veiculo =>
                    filtrosPesquisa.CodigosEmpresa.Contains(veiculo.Empresa.Codigo) ||
                    veiculo.Empresa.Filiais.Any(filial => filtrosPesquisa.CodigosEmpresa.Contains(filial.Codigo)) ||
                    veiculo.Empresa.Matriz.Any(matriz => filtrosPesquisa.CodigosEmpresa.Contains(matriz.Codigo)) ||
                    veiculo.Empresas.Any(empresa => filtrosPesquisa.CodigosEmpresa.Contains(empresa.Codigo)) ||
                    consultaEmpresa.Any(empresa => double.Parse(empresa.CNPJ) == veiculo.Proprietario.CPF_CNPJ)
                );
            }

            if ((filtrosPesquisa.ModeloVeicularCarga != null) && (filtrosPesquisa.PossiveisModelos == null))
            {
                if (filtrosPesquisa.ForcarFiltroModeloVeicularCarga)
                    consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.ModeloVeicularCarga.Codigo);
                else
                {
                    consultaVeiculo = consultaVeiculo.Where(veiculo =>
                        veiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.ModeloVeicularCarga.Codigo
                        || veiculo.ModeloVeicularCarga == null
                        || veiculo.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao
                        || veiculo.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral
                        || veiculo.ModeloVeicularCarga.CapacidadePesoTransporte >= filtrosPesquisa.ModeloVeicularCarga.CapacidadePesoTransporte
                    );
                }
            }
            else if ((filtrosPesquisa.PossiveisModelos != null) && (filtrosPesquisa.ModeloVeicularCarga != null))
                //consultaVeiculo = consultaVeiculo.Where(veiculo => (filtrosPesquisa.PossiveisModelos.Contains(veiculo.ModeloVeicularCarga) && ((veiculo.ModeloVeicularCarga.VeiculoPaletizado && veiculo.ModeloVeicularCarga.NumeroPaletes >= filtrosPesquisa.ModeloVeicularCarga.NumeroPaletes) || (!veiculo.ModeloVeicularCarga.VeiculoPaletizado && veiculo.ModeloVeicularCarga.CapacidadePesoTransporte >= filtrosPesquisa.ModeloVeicularCarga.CapacidadePesoTransporte))) || (veiculo.ModeloVeicularCarga == null || veiculo.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao));
                consultaVeiculo = consultaVeiculo.Where(veiculo => ((filtrosPesquisa.PossiveisModelos.Contains(veiculo.ModeloVeicularCarga) && (!veiculo.ModeloVeicularCarga.VeiculoPaletizado || veiculo.ModeloVeicularCarga.NumeroPaletes >= filtrosPesquisa.ModeloVeicularCarga.NumeroPaletes) && veiculo.ModeloVeicularCarga.CapacidadePesoTransporte >= filtrosPesquisa.ModeloVeicularCarga.CapacidadePesoTransporte)) || veiculo.ModeloVeicularCarga == null || veiculo.ModeloVeicularCarga.Tipo == TipoModeloVeicularCarga.Tracao || veiculo.ModeloVeicularCarga.Tipo == TipoModeloVeicularCarga.Geral);
            else if (filtrosPesquisa.PossiveisModelos?.Count > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => filtrosPesquisa.PossiveisModelos.Contains(veiculo.ModeloVeicularCarga));

            if (filtrosPesquisa.ApenasTracao)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.TipoVeiculo == "0");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoVeiculo) && (filtrosPesquisa.TipoVeiculo != "-1"))
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.TipoVeiculo.Equals(filtrosPesquisa.TipoVeiculo));

            if (filtrosPesquisa.CodigosSegmento != null && filtrosPesquisa.CodigosSegmento.Count > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => filtrosPesquisa.CodigosSegmento.Contains(veiculo.SegmentoVeiculo.Codigo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Placa.Contains(filtrosPesquisa.Placa) || veiculo.NumeroFrota.Equals(filtrosPesquisa.Placa));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Renavam))
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Renavam.Contains(filtrosPesquisa.Renavam));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroFrota))
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.NumeroFrota.Equals(filtrosPesquisa.NumeroFrota));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoPropriedade) && (filtrosPesquisa.TipoPropriedade != "A"))
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Tipo.Equals(filtrosPesquisa.TipoPropriedade));

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Empresa.CentrosCarregamento.Any(cen => cen.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento));

            if (filtrosPesquisa.SomenteEmpresasAtivas)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Empresa == null || (veiculo.Empresa.NomeCertificado != null && veiculo.Empresa.NomeCertificado != "") || veiculo.Empresa.EmissaoDocumentosForaDoSistema ||
                    (veiculo.Empresa.ModeloDocumentoFiscalCargaPropria != null && veiculo.Empresa.ModeloDocumentoFiscalCargaPropria.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros));

            if (filtrosPesquisa.CodigoAcertoViagem > 0)
            {
                var consultaAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>()
                    .Where(o => o.AcertoViagem.Codigo == filtrosPesquisa.CodigoAcertoViagem);

                consultaVeiculo = consultaVeiculo.Where(veiculo => (from p in consultaAcerto where p.AcertoViagem.Codigo == filtrosPesquisa.CodigoAcertoViagem select p.Veiculo).Contains(veiculo));
            }

            if (filtrosPesquisa.CodigoReboque > 0)
            {
                var reboque = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>()
                    .Where(o => o.Codigo == filtrosPesquisa.CodigoReboque)
                    .FirstOrDefault();

                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.VeiculosVinculados.Contains(reboque));
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == filtrosPesquisa.CodigoMotorista select obj;

                consultaVeiculo = consultaVeiculo.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());
            }

            if (filtrosPesquisa.CodigoProprietario > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Proprietario.CPF_CNPJ == filtrosPesquisa.CodigoProprietario);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chassi))
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Chassi.Contains(filtrosPesquisa.Chassi));

            if (filtrosPesquisa.SituacaoVeiculo.HasValue)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.SituacaoVeiculo == filtrosPesquisa.SituacaoVeiculo);

            if (filtrosPesquisa.CodigosEmpresas != null && filtrosPesquisa.CodigosEmpresas.Count > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Empresas.Any(e => filtrosPesquisa.CodigosEmpresas.Contains(e.Codigo)));

            return consultaVeiculo;
        }

        private IQueryable<Dominio.Entidades.Veiculo> ConsultarEmbarcador(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa)
        {
            var consultaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            return Consultar(consultaVeiculo, filtrosPesquisa);
        }

        private IQueryable<Dominio.Entidades.Veiculo> ConsultarEmbarcadorSomenteDisponiveis(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa)
        {
            var consultaVeiculoDisponivel = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento>();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaVeiculoDisponivel = consultaVeiculoDisponivel.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigosEmpresas != null && filtrosPesquisa.CodigosEmpresas.Count > 0)
                consultaVeiculoDisponivel = consultaVeiculoDisponivel.Where(obj => obj.Veiculo.Empresas.Any(e => filtrosPesquisa.CodigosEmpresas.Contains(e.Codigo)));

            var consultaVeiculo = (from obj in consultaVeiculoDisponivel where obj.Disponivel select obj.Veiculo);

            return Consultar(consultaVeiculo, filtrosPesquisa);
        }

        private IQueryable<Dominio.Entidades.Veiculo> ConsultarEmbarcadorSomenteEmEscala(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa)
        {
            var consultaEscalaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo>()
                .Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscalaVeiculo.EmEscala);

            var consultaVeiculo = consultaEscalaVeiculo.Select(o => o.Veiculo);

            if (filtrosPesquisa.CodigosEmpresas != null && filtrosPesquisa.CodigosEmpresas.Count > 0)
                consultaVeiculo = consultaVeiculo.Where(obj => obj.Empresas.Any(e => filtrosPesquisa.CodigosEmpresas.Contains(e.Codigo)));

            return Consultar(consultaVeiculo, filtrosPesquisa);
        }

        private IQueryable<Dominio.Entidades.Veiculo> ConsultarEmbarcadorPorTipoPropriedadeVeiculoDoTipoOperacao(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            var consultaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var consultaVeiculoFiltrada = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            if (tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoPropriedadeVeiculo == TipoPropriedadeVeiculo.Proprio)
            {
                consultaVeiculo = consultaVeiculo.Where(obj => obj.Tipo == "P");
            }

            if (tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoPropriedadeVeiculo == TipoPropriedadeVeiculo.Terceiros)
            {

                if (tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TiposTerceiros == null || tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TiposTerceiros.Count == 0)
                {
                    if (tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoProprietarioVeiculo != Dominio.Enumeradores.TipoProprietarioVeiculo.Todos)
                        consultaVeiculo = consultaVeiculo.Where(obj => obj.Tipo == "T" && obj.TipoProprietario == tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoProprietarioVeiculo);
                    else
                        consultaVeiculo = consultaVeiculo.Where(obj => obj.Tipo == "T");
                }
                else
                {
                    if (tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoProprietarioVeiculo != Dominio.Enumeradores.TipoProprietarioVeiculo.Todos)
                        consultaVeiculoFiltrada = consultaVeiculo.Where(obj => obj.Tipo == "T" && obj.TipoProprietario == tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoProprietarioVeiculo);
                    else
                        consultaVeiculoFiltrada = consultaVeiculo.Where(obj => obj.Tipo == "T");

                    var cpfCnpjProprietarios = consultaVeiculoFiltrada.Select(veiculo => veiculo.Proprietario.CPF_CNPJ);

                    var consultaModalidadeTransportadora = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>()
                        .Where(modalidade => cpfCnpjProprietarios
                        .Contains(modalidade.ModalidadePessoas.Cliente.CPF_CNPJ));

                    var tiposTerceiroValido = tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TiposTerceiros.Select(tipo => tipo.Codigo);

                    var consultaModalidadeFiltrada = consultaModalidadeTransportadora.Where(modalidade => tiposTerceiroValido.Contains(modalidade.TipoTerceiro.Codigo));

                    var cpfCnpjProprietariosComTipoTerceiro = consultaModalidadeFiltrada.Select(modalidade => modalidade.ModalidadePessoas.Cliente.CPF_CNPJ);

                    var consultaVeiculosFinal = consultaVeiculoFiltrada.Where(veiculo => cpfCnpjProprietariosComTipoTerceiro.Contains(veiculo.Proprietario.CPF_CNPJ));

                    consultaVeiculo = consultaVeiculosFinal;
                }
            }

            if (tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoPropriedadeVeiculo == TipoPropriedadeVeiculo.Ambos)
            {
                if (tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TiposTerceiros == null || tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TiposTerceiros.Count == 0)
                {
                    if (tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoProprietarioVeiculo != Dominio.Enumeradores.TipoProprietarioVeiculo.Todos)
                        consultaVeiculo = consultaVeiculo.Where(obj => obj.Tipo == "P" || (obj.Tipo == "T" && obj.TipoProprietario == tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoProprietarioVeiculo));
                }
                else
                {
                    if (tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoProprietarioVeiculo != Dominio.Enumeradores.TipoProprietarioVeiculo.Todos)
                        consultaVeiculoFiltrada = consultaVeiculo.Where(obj => obj.Tipo == "P" || (obj.Tipo == "T" && obj.TipoProprietario == tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TipoProprietarioVeiculo));

                    var cpfCnpjProprietarios = consultaVeiculoFiltrada.Select(veiculo => veiculo.Proprietario.CPF_CNPJ);

                    var consultaModalidadeTransportadora = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>()
                        .Where(modalidade => cpfCnpjProprietarios
                        .Contains(modalidade.ModalidadePessoas.Cliente.CPF_CNPJ));

                    var tiposTerceiroValido = tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TiposTerceiros.Select(tipo => tipo.Codigo);

                    var consultaModalidadeFiltrada = consultaModalidadeTransportadora.Where(modalidade => tiposTerceiroValido.Contains(modalidade.TipoTerceiro.Codigo));

                    var cpfCnpjProprietariosComTipoTerceiro = consultaModalidadeFiltrada.Select(modalidade => modalidade.ModalidadePessoas.Cliente.CPF_CNPJ);

                    var consultaVeiculosFinal = consultaVeiculoFiltrada.Where(veiculo => veiculo.Tipo == "P" || cpfCnpjProprietariosComTipoTerceiro.Contains(veiculo.Proprietario.CPF_CNPJ));

                    consultaVeiculo = consultaVeiculosFinal;
                }
            }

            return Consultar(consultaVeiculo, filtrosPesquisa);
        }

        private IQueryable<Dominio.Entidades.Veiculo> ConsultarPainelVeiculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaPainelVeiculo filtro, bool apenasTracao)
        {
            var consultaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            if (filtro.TipoVeiculo == "1" || filtro.TipoVeiculo == "0")
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.TipoVeiculo == filtro.TipoVeiculo);

            if (filtro.SituacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao || filtro.SituacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.SituacaoVeiculo == filtro.SituacaoVeiculo);
            else if (filtro.SituacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.SituacaoVeiculo == filtro.SituacaoVeiculo || veiculo.SituacaoVeiculo == null);
            else if (filtro.SituacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmFila)
            {
                IQueryable<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
                consultaVeiculo = consultaVeiculo.Where(veiculo => consultaFilaCarregamentoVeiculo.Any(o =>
                    o.ConjuntoVeiculo.Tracao.Placa == veiculo.Placa ||
                    o.ConjuntoVeiculo.Reboques.Any(x => x.Placa == veiculo.Placa)
                ));
            }


            if (filtro.CodigoLocalPrevisto > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.LocalidadeAtual.Codigo == filtro.CodigoLocalPrevisto);

            if (filtro.DataInicioDisponivel.HasValue)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.DataHoraPrevisaoDisponivel.Value.Date >= filtro.DataInicioDisponivel.Value.Date || veiculo.DataHoraPrevisaoDisponivel == null);

            if (filtro.DataFimDisponivel.HasValue)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.DataHoraPrevisaoDisponivel.Value.Date <= filtro.DataFimDisponivel.Value.Date || veiculo.DataHoraPrevisaoDisponivel == null);

            if (filtro.CodigoMotorista > 0)
            {
                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == filtro.CodigoMotorista select obj;

                consultaVeiculo = consultaVeiculo.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());
            }

            if (filtro.CodigoMarcaVeiculo > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Marca.Codigo == filtro.CodigoMarcaVeiculo);
            if (filtro.CodigoProprietario > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Proprietario.CPF_CNPJ == filtro.CodigoProprietario);
            if (filtro.CodigoModeloVeiculo > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Modelo.Codigo == filtro.CodigoModeloVeiculo);

            if (filtro.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaVeiculo = consultaVeiculo.Where(obj => obj.Ativo);

            if (filtro.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaVeiculo = consultaVeiculo.Where(obj => !obj.Ativo);

            if (!string.IsNullOrWhiteSpace(filtro.Placa))
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Placa.Contains(filtro.Placa) || veiculo.NumeroFrota.Equals(filtro.Placa));

            if (filtro.CodigoModeloVeicularCarga > 0)
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.ModeloVeicularCarga.Codigo == filtro.CodigoModeloVeicularCarga);

            if (!string.IsNullOrWhiteSpace(filtro.NumeroFrota))
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.NumeroFrota.Equals(filtro.NumeroFrota));

            if (!string.IsNullOrWhiteSpace(filtro.TipoPropriedade))
            {
                if (filtro.TipoPropriedade != "A")
                    consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.Tipo.Equals(filtro.TipoPropriedade));
            }
            if (filtro.TipoFrota >= 0)
            {
                consultaVeiculo = consultaVeiculo.Where(veiculo => veiculo.TipoFrota == filtro.TipoFrota);
            }
            if (filtro.CodigoTransportador > 0)
                consultaVeiculo = consultaVeiculo.Where(v => v.Empresa.Codigo == filtro.CodigoTransportador);

            if (filtro.CodigoCentroCarregamento > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.CentroCarregamento.Codigo == filtro.CodigoCentroCarregamento);
                consultaVeiculo = consultaVeiculo.Where(veiculo => consultaFilaCarregamentoVeiculo.Any(o => o.ConjuntoVeiculo.Tracao.Placa == veiculo.Placa || o.ConjuntoVeiculo.Reboques.Any(x => x.Placa == veiculo.Placa)));
            }

            return consultaVeiculo;
        }

        private NHibernate.ISQLQuery ConsultaVeiculoMonitoramento(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = null, bool count = false)
        {

            string sql = @"
                WITH VeiculoComPrioridade AS (
                    SELECT
                        Veiculo.VEI_CODIGO,
                        Veiculo.VEI_PLACA,
                        Monitoramento.MON_CODIGO,
                        Monitoramento.MON_STATUS,
                        Monitoramento.MON_DATA_FIM,
                        ROW_NUMBER() OVER (
                            PARTITION BY Veiculo.VEI_PLACA
                            ORDER BY
                                CASE 
                                    WHEN Monitoramento.MON_STATUS = 1 THEN 1
                                    WHEN Monitoramento.MON_STATUS = 0 THEN 2
                                    WHEN Monitoramento.MON_STATUS = 2 THEN 3
                                    ELSE 4
                                END,
                                CASE 
                                    WHEN Monitoramento.MON_STATUS = 2 THEN ABS(DATEDIFF(SECOND, GETDATE(), Monitoramento.MON_DATA_FIM))
                                    ELSE 0
                                END
                        ) AS RN
                    FROM T_VEICULO Veiculo
                    LEFT JOIN T_MONITORAMENTO Monitoramento 
                        ON Veiculo.VEI_CODIGO = Monitoramento.VEI_CODIGO
                )

                select ";

            if (count)
            {
                sql += " count(*)";
            }
            else
            {
                sql += @"
                    distinct                   
                    Veiculo.VEI_PLACA AS Placa,
                    Veiculo.VEI_TIPO AS Tipo,
                    Veiculo.VEI_TIPOVEICULO AS TipoVeiculo,
                    Veiculo.VEI_TIPORODADO AS TipoRodado,
                    Veiculo.VEI_TIPO_CARROCERIA AS TipoCarroceria,
                    ModeloVeiculo.MVC_DESCRICAO AS ModeloVeiculoDescricao,
                    Veiculo.VEI_ANO AS Ano,
                    Tecnologia.TRA_DESCRICAO AS Tecnologia,
                    Empresa.EMP_RAZAO AS Transportador,
                    Empresa.EMP_CNPJ AS CNPJTransportador,
                    Veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR AS Terminal,
                    Veiculo.VEI_ATIVO AS Ativo,
                    MAX(PosicaoAtual.POA_DATA_VEICULO) AS DataPosicaoAtual,
                    COUNT(Monitoramento.MON_CODIGO) AS MonitoramentosFinalizados";
            }

            sql += $@"
                from
                    T_VEICULO Veiculo 

                inner join 
                    VeiculoComPrioridade VeiculoPrioridade
                        ON Veiculo.VEI_CODIGO = VeiculoPrioridade.VEI_CODIGO AND VeiculoPrioridade.RN = 1
                left join 
                    T_EMPRESA as Empresa 
                        on Empresa.EMP_CODIGO = Veiculo.EMP_CODIGO
                left join
                    T_MODELO_VEICULAR_CARGA ModeloVeiculo 
                        on Veiculo.MVC_CODIGO = ModeloVeiculo.MVC_CODIGO 
                left join
                    T_POSICAO_ATUAL PosicaoAtual 
                        on Veiculo.VEI_CODIGO = PosicaoAtual.VEI_CODIGO
                left join
                    T_RASTREADOR_TECNOLOGIA Tecnologia 
                        on Veiculo.TRA_CODIGO = Tecnologia.TRA_CODIGO             				
				left join
                    T_POSICAO Posicao
						on Posicao.POS_CODIGO = PosicaoAtual.POS_CODIGO";

            if (!count)
            {
                sql += $@"
                left join
                    T_MONITORAMENTO Monitoramento
                        on Veiculo.VEI_CODIGO = Monitoramento.VEI_CODIGO";
            }

            sql += $@"
                WHERE 1=1 ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                sql += $@"
                AND Veiculo.VEI_PLACA like '%{filtrosPesquisa.Placa}%' ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoVeiculo) && (filtrosPesquisa.TipoVeiculo != "-1"))
                sql += $@"
                AND Veiculo.VEI_TIPOVEICULO = '{filtrosPesquisa.TipoVeiculo}' ";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                sql += $@"
                AND Veiculo.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa} ";

            if (filtrosPesquisa.CodigoTecnologiaRastreador > 0)
                sql += $@"
                AND Tecnologia.TRA_CODIGO = {filtrosPesquisa.CodigoTecnologiaRastreador} ";

            if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                sql += $@"
                AND Veiculo.VEI_ATIVO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo} ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoPropriedade) && filtrosPesquisa.TipoPropriedade != "A")
                sql += $@" AND Veiculo.VEI_TIPO = '{filtrosPesquisa.TipoPropriedade}'";

            if (filtrosPesquisa.DataPosicao.HasValue)
                sql += $@" AND PosicaoAtual.POA_DATA_VEICULO > '{filtrosPesquisa.DataPosicao.Value.ToString("yyyy-MM-dd")}'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Terminal))
                sql += $@" AND Veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR = '{filtrosPesquisa.Terminal}'";

            if (filtrosPesquisa.RastreadorPosicionado)
                sql += $@" AND DATEADD(minute, 30, PosicaoAtual.POA_DATA_VEICULO) > '{DateTime.Now.ToString("yyyy-MM-dd HH:ss")}'";

            if (filtrosPesquisa.CodigosEmpresa.Any())
                sql += $@" AND Veiculo.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosEmpresa)}) ";

            if (filtrosPesquisa.CodigosTecnologiaRastreador.Any())
                sql += $@" AND Tecnologia.TRA_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTecnologiaRastreador)}) ";

            if (filtrosPesquisa.VeiculoOnlineOffline != null)
            {
                var dataDia = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                sql += $@" AND (CASE 
                                        WHEN Posicao.POS_DATA_VEICULO IS NOT NULL 
                                                AND DATEDIFF(MINUTE, Posicao.POS_DATA_VEICULO, '{dataDia}') <= {filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal}
                                        THEN 1
                                 ELSE 0 END) = {filtrosPesquisa.VeiculoOnlineOffline.GetHashCode()}";
            }

            if (!count)
                sql += @"
                GROUP BY 
                    Veiculo.VEI_PLACA,
                    Veiculo.VEI_TIPO,
                    Veiculo.VEI_TIPOVEICULO,
                    Veiculo.VEI_TIPORODADO,
                    Veiculo.VEI_TIPO_CARROCERIA,
                    ModeloVeiculo.MVC_DESCRICAO,
                    Veiculo.VEI_ANO,
                    Empresa.EMP_RAZAO,
                    Empresa.EMP_CNPJ,
                    Tecnologia.TRA_DESCRICAO,
                    Veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR,
                    Veiculo.VEI_ATIVO";

            if (parametroConsulta != null)
            {

                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
                    sql += $@"
                    ORDER BY {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}";

                if (parametroConsulta.InicioRegistros >= 0)
                    sql += $@"
                    OFFSET {parametroConsulta.InicioRegistros} ROWS ";

                if (parametroConsulta.LimiteRegistros > 0)
                    sql += $@"
                    FETCH FIRST {parametroConsulta.LimiteRegistros} ROWS ONLY";

            }

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            return consulta;

        }

        private IQueryable<Dominio.Entidades.Veiculo> MontarConsulta(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                query = query.Where(o => o.Placa.Contains(filtrosPesquisa.Placa));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chassi))
                query = query.Where(o => o.Chassi.Contains(filtrosPesquisa.Chassi));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoVeiculo) && filtrosPesquisa.TipoVeiculo != "-1")
                query = query.Where(o => o.TipoVeiculo.Equals(filtrosPesquisa.TipoVeiculo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Tipo) && filtrosPesquisa.Tipo != "A")
                query = query.Where(o => o.Tipo.Equals(filtrosPesquisa.Tipo));

            if (filtrosPesquisa.CpfcnpjProprietario > 0)
                query = query.Where(o => o.Proprietario.CPF_CNPJ == filtrosPesquisa.CpfcnpjProprietario);

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == filtrosPesquisa.CodigoMotorista select obj;

                query = query.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());
            }

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (filtrosPesquisa.CodigosSegmento != null && filtrosPesquisa.CodigosSegmento.Count > 0)
                query = query.Where(o => filtrosPesquisa.CodigosSegmento.Contains(o.SegmentoVeiculo.Codigo));

            if (filtrosPesquisa.CodigosFuncionarioResponsavel != null && filtrosPesquisa.CodigosFuncionarioResponsavel.Count > 0)
                query = query.Where(o => filtrosPesquisa.CodigosFuncionarioResponsavel.Contains(o.FuncionarioResponsavel.Codigo));

            query.Fetch(o => o.SegmentoVeiculo);
            query.Fetch(o => o.Proprietario);
            query.Fetch(o => o.ModeloVeicularCarga);
            query.Fetch(o => o.Modelo);
            query.Fetch(o => o.ModeloCarroceria);
            query.Fetch(o => o.Estado);
            query.Fetch(o => o.Empresa);
            query.Fetch(o => o.GrupoPessoas);
            query.Fetch(o => o.VeiculosVinculados);

            return query;
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Veiculos.PainelVeiculo> ConsultaPainelVeiculoObjetoDeValor(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaPainelVeiculo filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            string sqlQuery = $@"SELECT {ObterSelectCamposPainelVeiculo(filtrosPesquisa)}
                                 FROM {ObterFromPainelVeiculo()}
                                 WHERE {ObterWherePainelVeiculo(filtrosPesquisa)}
                                 ORDER BY Veiculo.VEI_CODIGO {dirOrdenacao}
                                 OFFSET {inicioRegistros} ROWS
                                 FETCH NEXT {maximoRegistros} ROWS ONLY;";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Veiculos.PainelVeiculo)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Veiculos.PainelVeiculo>();
        }
        public int ContarPainelVeiculoObjetoDeValor(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaPainelVeiculo filtrosPesquisa)
        {
            string sqlQuery = $@"SELECT Count(1)
                                 FROM {ObterFromPainelVeiculo()}
                                 WHERE {ObterWherePainelVeiculo(filtrosPesquisa)}";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            return consulta.UniqueResult<int>();
        }

        private string ObterSelectCamposPainelVeiculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaPainelVeiculo filtrosPesquisa)
        {
            List<SituacaoFilaCarregamentoVeiculo> situacoesAtiva = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesAtivaSemEmTrasicao();
            string dataSituacao = (filtrosPesquisa.DataSituacao ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
            return $@"	Veiculo.VEI_CODIGO Codigo,
	                    Coalesce((select top 1 situacao.VSI_SITUACAO_VEICULO
	                        from T_VEICULO_SITUACAO situacao
	                        where situacao.VEI_CODIGO = Veiculo.VEI_CODIGO
		                        and ((situacao.VSI_SITUACAO_VEICULO = 4 and '{dataSituacao}' between situacao.VSI_DATA_HORA_ENTRADA_MANUTENCAO and coalesce(situacao.VSI_DATA_HORA_SAIDA_MANUTENCAO, '{dataSituacao}'))
		                            or (situacao.VSI_SITUACAO_VEICULO = 3 and '{dataSituacao}' between situacao.VSI_DATA_HORA_SAIDA_INICIO_VIAGEM and coalesce(situacao.VSI_DATA_HORA_RETORNO_VIAGEM, '{dataSituacao}')))
	                    Order by situacao.VSI_CODIGO ), Veiculo.VEI_SITUACAO_VEICULO) SituacaoVeiculo,
	                    Veiculo.VEI_NUMERO_FROTA NumeroFrota,
	                    Veiculo.VEI_PLACA Placa,
	                    Veiculo.VEI_ATIVO Ativo,
	                    Veiculo.VEI_TIPO_FROTA TipoFrota,
	                    Proprietario.CLI_NOME Proprietario,
	                    ( select top 1 Carga.CAR_CODIGO_CARGA_EMBARCADOR
				            from T_CARGA_PEDIDO CargaPedido
					            inner join T_CARGA Carga on CargaPedido.CAR_CODIGO=Carga.CAR_CODIGO
					            inner join T_VEICULO VeiculoCarga on Carga.CAR_VEICULO=VeiculoCarga.VEI_CODIGO
					            inner join T_PEDIDO Pedido on CargaPedido.PED_CODIGO=Pedido.PED_CODIGO
				            where
					            VeiculoCarga.VEI_PLACA = Veiculo.VEI_PLACA
					            and ( Carga.CAR_SITUACAO NOT IN (13,18,11,10) or Carga.CAR_SITUACAO is null )
					            and ( Pedido.PED_SITUACAO <> 3 or Pedido.PED_SITUACAO is null )
				            order by Carga.CAR_CODIGO) Carga,

	                    (select top 1 CentroCarregamento.CEC_DESCRICAO
		                    from T_FILA_CARREGAMENTO_VEICULO FilaCarregamentoVeiculoa
			                    left outer join T_FILA_CARREGAMENTO_CONJUNTO_VEICULO FilaCarregamentoConjuntoVeiculo on FilaCarregamentoVeiculoa.FCV_CODIGO=FilaCarregamentoConjuntoVeiculo.FCV_CODIGO
			                    left outer join T_VEICULO VeiculoCentroCarregamento on FilaCarregamentoConjuntoVeiculo.FCV_CODIGO_TRACAO=VeiculoCentroCarregamento.VEI_CODIGO
			                    left join T_CENTRO_CARREGAMENTO CentroCarregamento on CentroCarregamento.CEC_CODIGO = FilaCarregamentoVeiculoa.CEC_CODIGO 
		                    where ( FilaCarregamentoVeiculoa.FLV_SITUACAO in ( {string.Join(",", situacoesAtiva.Select(e => (int)e).ToList())} ) )
			                    and VeiculoCentroCarregamento.VEI_CODIGO=Veiculo.VEI_CODIGO
			                    or exists (
				                    select veiculo4_.VEI_CODIGO
				                    from T_FILA_CARREGAMENTO_CONJUNTO_VEICULO_REBOQUE reboques3_, T_VEICULO veiculo4_
				                    where
					                    FilaCarregamentoConjuntoVeiculo.FCV_CODIGO=reboques3_.FCV_CODIGO_REBOQUE
					                    and reboques3_.VEI_CODIGO=veiculo4_.VEI_CODIGO
					                    and veiculo4_.VEI_CODIGO=180
			                    )
		                    order by
			                    FilaCarregamentoVeiculoa.FLV_CODIGO desc) CentroCarregamento,

	                    VeiculoModelo.VMO_DESCRICAO Modelo,

	                    (Select STRING_AGG(VeiculoReboque.VEI_PLACA, ',')
	                        from T_VEICULO_CONJUNTO VeiculoConjunto
		                        join T_VEICULO VeiculoReboque on VeiculoReboque.VEI_CODIGO = VeiculoConjunto.VEC_CODIGO_FILHO
	                        where VeiculoConjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO) Reboque,

	                    (Select VeiculoMotorista.VMT_NOME
	                        From T_VEICULO_MOTORISTA VeiculoMotorista
	                        Where VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO
	                        And VeiculoMotorista.VMT_PRINCIPAL = 1) Motorista,
	                    Transportador.EMP_RAZAO Transportador,
	                    Cliente = (SELECT SUBSTRING((SELECT DISTINCT ', ' + CAST(CASE
					                    WHEN CP.PED_TIPO_TOMADOR = 0 THEN REM.CLI_NOME
					                    WHEN CP.PED_TIPO_TOMADOR = 3 THEN DEST.CLI_NOME
					                    WHEN CP.PED_TIPO_TOMADOR = 4 THEN OUTRO.CLI_NOME
					                    WHEN CP.PED_TIPO_TOMADOR = 2 THEN RECEB.CLI_NOME
					                    WHEN CP.PED_TIPO_TOMADOR = 1 THEN EXPE.CLI_NOME
					                    ELSE ''
					                    END AS NVARCHAR(2000))
				                    FROM T_CARGA C
					                    JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = C.CAR_CODIGO
					                    JOIN T_PEDIDO P ON P.PED_CODIGO = CP.PED_CODIGO
					                    LEFT OUTER JOIN T_CLIENTE REM ON REM.CLI_CGCCPF = P.CLI_CODIGO_REMETENTE
					                    LEFT OUTER JOIN T_CLIENTE DEST ON DEST.CLI_CGCCPF = P.CLI_CODIGO
					                    LEFT OUTER JOIN T_CLIENTE OUTRO ON OUTRO.CLI_CGCCPF = P.CLI_CODIGO_TOMADOR
					                    LEFT OUTER JOIN T_CLIENTE RECEB ON RECEB.CLI_CGCCPF = P.CLI_CODIGO_RECEBEDOR
					                    LEFT OUTER JOIN T_CLIENTE EXPE ON EXPE.CLI_CGCCPF = P.CLI_CODIGO_EXPEDIDOR
				                    WHERE C.CAR_SITUACAO NOT IN (0, 1, 2, 4, 5, 6, 11, 13, 15, 17, 18)
				                    AND C.CAR_VEICULO = Veiculo.VEI_CODIGO FOR XML PATH('')), 3, 2000)),
	                    Veiculo.VEI_VAZIO StatusVazio,
	                    Veiculo.VEI_AVISADO_CARREGAMENTO StatusAvisado,
	                    Veiculo.VEI_DATA_HORA_PREVISAO_DISPONIVEL PrevisaoDisponivel,
                        LocalidadeAtual.LOC_DESCRICAO + IIF(Estado.UF_SIGLA Is Not Null, Estado.UF_SIGLA, '') LocalPrevisto";
        }

        private string ObterFromPainelVeiculo()
        {
            return @" T_VEICULO Veiculo
                        left join T_VEICULO_MODELO VeiculoModelo on VeiculoModelo.VMO_CODIGO=Veiculo.VMO_CODIGO
                        left join T_LOCALIDADES LocalidadeAtual on Veiculo.LOC_CODIGO_ATUAL=LocalidadeAtual.LOC_CODIGO
                        left join T_UF Estado on Estado.UF_SIGLA=LocalidadeAtual.UF_SIGLA
                        left join T_CLIENTE Proprietario on Veiculo.VEI_PROPRIETARIO=Proprietario.CLI_CGCCPF
                        left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on Veiculo.MVC_CODIGO=ModeloVeicularCarga.MVC_CODIGO
                        left join T_EMPRESA Transportador on Veiculo.EMP_CODIGO=Transportador.EMP_CODIGO
                        left join T_VEICULO_MARCA VeiculoMarca on VeiculoMarca.VMA_CODIGO = Veiculo.VMA_CODIGO ";
        }

        private string ObterWherePainelVeiculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaPainelVeiculo filtrosPesquisa)
        {
            string dataSituacao = (filtrosPesquisa.DataSituacao ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
            StringBuilder where = new StringBuilder(@"1=1");

            if (filtrosPesquisa.TipoVeiculo == "1" || filtrosPesquisa.TipoVeiculo == "0")
                where.Append($" And Veiculo.VEI_TIPOVEICULO = {filtrosPesquisa.TipoVeiculo} ");

            if (filtrosPesquisa.SituacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao ||
                filtrosPesquisa.SituacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem ||
                filtrosPesquisa.SituacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel ||
                filtrosPesquisa.SituacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Indisponivel)
            {
                where.Append($@" And (Coalesce((select top 1 situacao.VSI_SITUACAO_VEICULO
	                                                from T_VEICULO_SITUACAO situacao
	                                                where situacao.VEI_CODIGO = Veiculo.VEI_CODIGO
		                                                and ((situacao.VSI_SITUACAO_VEICULO = 4 and '{dataSituacao}' between situacao.VSI_DATA_HORA_ENTRADA_MANUTENCAO and coalesce(situacao.VSI_DATA_HORA_SAIDA_MANUTENCAO, '{dataSituacao}'))
		                                                    or (situacao.VSI_SITUACAO_VEICULO = 3 and '{dataSituacao}' between situacao.VSI_DATA_HORA_SAIDA_INICIO_VIAGEM and coalesce(situacao.VSI_DATA_HORA_RETORNO_VIAGEM, '{dataSituacao}')))
	                                            Order by situacao.VSI_CODIGO ), Veiculo.VEI_SITUACAO_VEICULO) = {(int)filtrosPesquisa.SituacaoVeiculo} ");

                if (filtrosPesquisa.SituacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel)
                    where.Append($@" Or Veiculo.VEI_SITUACAO_VEICULO = null ");

                where.Append($@" ) ");
            }
            else if (filtrosPesquisa.SituacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmFila)
            {
                where.Append($@" And exists (
										select FilaCarregamentoVeiculo.FLV_CODIGO
										from T_FILA_CARREGAMENTO_VEICULO FilaCarregamentoVeiculo
										left outer join T_FILA_CARREGAMENTO_CONJUNTO_VEICULO FilaCarregamentoConjuntoVeiculo on FilaCarregamentoConjuntoVeiculo.FCV_CODIGO = FilaCarregamentoVeiculo.FCV_CODIGO
										left outer join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = FilaCarregamentoConjuntoVeiculo.FCV_CODIGO_TRACAO
										where FilaCarregamentoConjuntoVeiculo.FCV_CODIGO_TRACAO = Veiculo.VEI_CODIGO
											or exists (
												select VeiculoReboque.VEI_CODIGO
												from T_FILA_CARREGAMENTO_CONJUNTO_VEICULO_REBOQUE FilaCarregamentoConjuntoVeiculoReboque, T_VEICULO VeiculoReboque
												where FilaCarregamentoConjuntoVeiculo.FCV_CODIGO = FilaCarregamentoConjuntoVeiculoReboque.FCV_CODIGO_REBOQUE
													and FilaCarregamentoConjuntoVeiculoReboque.VEI_CODIGO = VeiculoReboque.VEI_CODIGO
													and VeiculoReboque.VEI_PLACA = Veiculo.VEI_PLACA
											)
										)");
            }

            if (filtrosPesquisa.CodigoLocalPrevisto > 0)
                where.Append($" And LocalidadeAtual.LOC_CODIGO = {filtrosPesquisa.CodigoLocalPrevisto} ");

            if (filtrosPesquisa.DataInicioDisponivel.HasValue)
                where.Append($@" And Veiculo.VEI_DATA_HORA_PREVISAO_DISPONIVEL >= '{filtrosPesquisa.DataInicioDisponivel.Value.Date.ToString("yyyy-MM-dd HH:mm:ss")}' ");

            if (filtrosPesquisa.DataFimDisponivel.HasValue)
            {
                DateTime dataFimDisponnivel = filtrosPesquisa.DataFimDisponivel.Value.AddDays(1).AddTicks(-1);
                where.Append($@" And Veiculo.VEI_DATA_HORA_PREVISAO_DISPONIVEL <= '{dataFimDisponnivel.ToString("yyyy-MM-dd HH:mm:ss")}' ");
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
                where.Append($@" And (
                                        exists (
                                            select VeiculoMotorista.VMT_CODIGO
                                            from T_VEICULO_MOTORISTA VeiculoMotorista
                                            inner join T_FUNCIONARIO Motorista on VeiculoMotorista.FUN_CODIGO=Motorista.FUN_CODIGO
                                            inner join T_VEICULO VeiculoMot on VeiculoMotorista.VEI_CODIGO=VeiculoMot.VEI_CODIGO
                                            where Motorista.FUN_CODIGO={filtrosPesquisa.CodigoMotorista}
                                                and ( VeiculoMot.VEI_CODIGO=Veiculo.VEI_CODIGO
                                                    or (VeiculoMot.VEI_CODIGO is null)
                                                    and (Veiculo.VEI_CODIGO is null))
                                        )
                                    ) ");

            if (filtrosPesquisa.CodigoMarcaVeiculo > 0)
                where.Append($" And VeiculoMarca.VMA_CODIGO = {filtrosPesquisa.CodigoMarcaVeiculo} ");

            if (filtrosPesquisa.CodigoProprietario > 0)
                where.Append($" And Proprietario.CLI_CGCCPF = {filtrosPesquisa.CodigoProprietario} ");

            if (filtrosPesquisa.CodigoModeloVeiculo > 0)
                where.Append($" And VeiculoModelo.VMO_CODIGO = {filtrosPesquisa.CodigoModeloVeiculo} ");

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                where.Append($" And Veiculo.VEI_ATIVO = 1 ");

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                where.Append($" And Veiculo.VEI_ATIVO = 0 ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                where.Append($@" And (
                                    Veiculo.VEI_PLACA like ('%'+'{filtrosPesquisa.Placa}'+'%')
                                    or Veiculo.VEI_NUMERO_FROTA='{filtrosPesquisa.Placa}'
                                ) ");

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                where.Append($" And ModeloVeicularCarga.MVC_CODIGO = {filtrosPesquisa.CodigoModeloVeicularCarga} ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroFrota))
                where.Append($" And Veiculo.VEI_NUMERO_FROTA = '{filtrosPesquisa.NumeroFrota}' ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoPropriedade) && filtrosPesquisa.TipoPropriedade != "A")
                where.Append($" And Veiculo.VEI_TIPO = '{filtrosPesquisa.TipoPropriedade}' ");

            if (filtrosPesquisa.TipoFrota > 0)
                where.Append($" And Veiculo.VEI_TIPO_FROTA = {(int)filtrosPesquisa.TipoFrota} ");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" And Transportador.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                where.Append($@" And (exists (
                                    select FilaCarregamentoVeiculo.FLV_CODIGO
                                    from T_FILA_CARREGAMENTO_VEICULO FilaCarregamentoVeiculo
                                    inner join T_CENTRO_CARREGAMENTO centrocarr10_ on FilaCarregamentoVeiculo.CEC_CODIGO=centrocarr10_.CEC_CODIGO
                                    left outer join T_FILA_CARREGAMENTO_CONJUNTO_VEICULO FilaCarregamentoConjuntoVeiculo on FilaCarregamentoVeiculo.FCV_CODIGO=FilaCarregamentoConjuntoVeiculo.FCV_CODIGO
                                    left outer join T_VEICULO VeiculoFilaCarregamento on FilaCarregamentoConjuntoVeiculo.FCV_CODIGO_TRACAO=VeiculoFilaCarregamento.VEI_CODIGO
                                    where
                                        centrocarr10_.CEC_CODIGO = {filtrosPesquisa.CodigoCentroCarregamento}
                                        and ( VeiculoFilaCarregamento.VEI_PLACA=Veiculo.VEI_PLACA
                                            or ( VeiculoFilaCarregamento.VEI_PLACA is null )
                                            and ( Veiculo.VEI_PLACA is null )
                                            or exists (
                                                select VeiculoFilaCarregamentoConjuntoVeiculoReboque.VEI_CODIGO
                                                from T_FILA_CARREGAMENTO_CONJUNTO_VEICULO_REBOQUE FilaCarregamentoConjuntoVeiculoReboque,
                                                    T_VEICULO VeiculoFilaCarregamentoConjuntoVeiculoReboque
                                                where
                                                    FilaCarregamentoConjuntoVeiculo.FCV_CODIGO=FilaCarregamentoConjuntoVeiculoReboque.FCV_CODIGO_REBOQUE
                                                    and FilaCarregamentoConjuntoVeiculoReboque.VEI_CODIGO=VeiculoFilaCarregamentoConjuntoVeiculoReboque.VEI_CODIGO
                                                    and ( VeiculoFilaCarregamentoConjuntoVeiculoReboque.VEI_PLACA=Veiculo.VEI_PLACA
                                                        or ( VeiculoFilaCarregamentoConjuntoVeiculoReboque.VEI_PLACA is null )
                                                        and ( Veiculo.VEI_PLACA is null )
                                                    )
                                            )
                                        )
                                    )
                            ) ");

            return where.ToString();
        }

        #endregion
    }
}
