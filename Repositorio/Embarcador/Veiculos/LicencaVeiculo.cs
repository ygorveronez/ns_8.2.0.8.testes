using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Veiculos
{
    public class LicencaVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>
    {
        #region Contrutores

        public LicencaVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Contrutores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaLicencaVeiculo filtrosPesquisa)
        {
            var consultaLicencaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();

            if (filtrosPesquisa.StatusLicenca.HasValue)
                consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.Status == filtrosPesquisa.StatusLicenca.Value);

            if (filtrosPesquisa.DataEmissaoInicial.HasValue)
                consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.DataEmissao.Value.Date >= filtrosPesquisa.DataEmissaoInicial.Value.Date);

            if (filtrosPesquisa.DataEmissaoLimite.HasValue)
                consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.DataEmissao.Value.Date <= filtrosPesquisa.DataEmissaoLimite.Value.Date);

            if (filtrosPesquisa.DataVencimentoInicial.HasValue)
                consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.DataVencimento.Value.Date >= filtrosPesquisa.DataVencimentoInicial.Value.Date);

            if (filtrosPesquisa.DataVencimentoLimite.HasValue)
                consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.DataVencimento.Value.Date <= filtrosPesquisa.DataVencimentoLimite.Value.Date);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.Veiculo.Placa.Contains(filtrosPesquisa.Placa) || o.Veiculo.NumeroFrota.Equals(filtrosPesquisa.Placa));

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == filtrosPesquisa.CodigoMotorista select obj;

                consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());
            }

            if (filtrosPesquisa.Empresa != null)
            {
                double cnpjEmpresa = filtrosPesquisa.Empresa.CNPJ.ToDouble();

                if ((filtrosPesquisa.Empresa.Matriz != null) && (filtrosPesquisa.Empresa.Matriz.FirstOrDefault() != null))
                    consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.Veiculo.Empresa.Codigo == filtrosPesquisa.Empresa.Codigo || o.Veiculo.Empresas.Contains(filtrosPesquisa.Empresa) || (o.Veiculo.Empresa.Codigo == filtrosPesquisa.Empresa.Matriz.FirstOrDefault().Codigo) || o.Veiculo.Proprietario.CPF_CNPJ == cnpjEmpresa);
                else if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.Veiculo.Empresa.Codigo == filtrosPesquisa.Empresa.Codigo || o.Veiculo.Empresas.Contains(filtrosPesquisa.Empresa));
                else
                    consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.Veiculo.Empresa.Codigo == filtrosPesquisa.Empresa.Codigo || o.Veiculo.Empresas.Contains(filtrosPesquisa.Empresa) || o.Veiculo.Proprietario.CPF_CNPJ == cnpjEmpresa);

                if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.Veiculo.Empresa.Codigo == filtrosPesquisa.Empresa.Codigo);
            }

            if (filtrosPesquisa.Proprietario != null)
                consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => (o.Veiculo.Proprietario.CPF_CNPJ == filtrosPesquisa.Proprietario.CPF_CNPJ || o.Veiculo.Empresa.CNPJ == filtrosPesquisa.Proprietario.CPF_CNPJ_SemFormato));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroContainer))
                consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.NumeroContainer.Contains(filtrosPesquisa.NumeroContainer));

            if (filtrosPesquisa.StatusVigencia.HasValue)
            {
                if (filtrosPesquisa.StatusVigencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca.Vencido)
                    consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => o.Vencido);
                else if (filtrosPesquisa.StatusVigencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca.Vigente)
                    consultaLicencaVeiculo = consultaLicencaVeiculo.Where(o => ((bool?)o.Vencido ?? false) == false);
            }

            return consultaLicencaVeiculo;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<RelacaoTesteFrioVeiculo> ObterRelacaoVencimentoTesteFrioVeiculo(List<int> codigosVeiculos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();
            query = query.Where(x => codigosVeiculos.Contains(x.Veiculo.Codigo));
            query = query.OrderByDescending(x => x.DataVencimento);

            return query.Select(x => new RelacaoTesteFrioVeiculo
            {
                CodigoVeiculo = x.Veiculo.Codigo,
                Vencimento = x.DataVencimento,
                Status = x.Status
            }).ToList().DistinctBy(x => x.CodigoVeiculo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> BuscarPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();
            query = query.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> BuscarNaoUtilizadasPorVeiculo(int codigoVeiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaLicenca> queryCargaLicenca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLicenca>();

            query = query.Where(obj => obj.Veiculo.Codigo == codigoVeiculo && !queryCargaLicenca.Any(o => o.LicencaVeiculo.Codigo == obj.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> BuscarPorVeiculoTipoDeCarga(string placaVeiculo, int codigoFaixaTemperatura, int codigoTipoCarga, int codigoContainer, bool somenteNaoUtilizadas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>()
                .Where(obj => obj.Veiculo.Placa == placaVeiculo && !obj.Vencido);

            if (somenteNaoUtilizadas)
            {
                var queryCargaLicenca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLicenca>()
                    .Where(o => o.LicencaVeiculo != null && o.LicencaVeiculo.Veiculo.Placa == placaVeiculo);

                query = query.Where(obj => !queryCargaLicenca.Where(a => a.LicencaVeiculo.Codigo == obj.Codigo).Any());
            }

            if (codigoFaixaTemperatura > 0)
                query = query.Where(obj => obj.FaixasTemperatura.Any(f => f.Codigo == codigoFaixaTemperatura));

            if (codigoTipoCarga > 0)
                query = query.Where(obj => obj.TipoCarga.Codigo == codigoTipoCarga || obj.TipoCarga == null);

            if (codigoContainer > 0)
                query = query.Where(obj => obj.Container.Codigo == codigoTipoCarga || obj.Container == null);

            return query.ToList();
        }
        
        public Task<List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>> BuscarPorVeiculoTipoDeCargaAsync(string placaVeiculo, int codigoFaixaTemperatura, int codigoTipoCarga, int codigoContainer, bool somenteNaoUtilizadas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>()
                .Where(obj => obj.Veiculo.Placa == placaVeiculo && !obj.Vencido);

            if (somenteNaoUtilizadas)
            {
                var queryCargaLicenca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLicenca>()
                    .Where(o => o.LicencaVeiculo != null && o.LicencaVeiculo.Veiculo.Placa == placaVeiculo);

                query = query.Where(obj => !queryCargaLicenca.Where(a => a.LicencaVeiculo.Codigo == obj.Codigo).Any());
            }

            if (codigoFaixaTemperatura > 0)
                query = query.Where(obj => obj.FaixasTemperatura.Any(f => f.Codigo == codigoFaixaTemperatura));

            if (codigoTipoCarga > 0)
                query = query.Where(obj => obj.TipoCarga.Codigo == codigoTipoCarga || obj.TipoCarga == null);

            if (codigoContainer > 0)
                query = query.Where(obj => obj.Container.Codigo == codigoTipoCarga || obj.Container == null);

            return query.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> BuscarLicencasParaAlerta()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();
            var result = from obj in query where obj.Veiculo.Ativo select obj;

            var queryAlerta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Alerta>();
            var resultAlerta = from obj in queryAlerta where obj.TelaAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaTela.Veiculo select obj;

            result = result.Where(o => !resultAlerta.Any(c => c.CodigoEntidade == o.Codigo));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> BuscarLicencasParaBloqueioPedido(List<int> codigosVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();
            var result = from obj in query where codigosVeiculo.Contains(obj.Veiculo.Codigo) && (obj.DataVencimento.Value.Date < DateTime.Now.Date || obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca.Vencido) && obj.BloquearCriacaoPedidoLicencaVencida select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo BuscarLicencaParaBloqueioPedido(List<int> codigosVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();
            var result = from obj in query where codigosVeiculo.Contains(obj.Veiculo.Codigo) && (obj.DataVencimento.Value.Date < DateTime.Now.Date || obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca.Vencido) && obj.BloquearCriacaoPedidoLicencaVencida select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo BuscarLicencaParaBloqueioPlanejamentoPedido(List<int> codigosVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();
            var result = from obj in query where codigosVeiculo.Contains(obj.Veiculo.Codigo) && (obj.DataVencimento.Value.Date < DateTime.Now.Date || obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca.Vencido) && obj.BloquearCriacaoPlanejamentoPedidoLicencaVencida select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaLicencaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaLicencaVeiculo = Consultar(filtrosPesquisa);

            consultaLicencaVeiculo = consultaLicencaVeiculo
                .Fetch(o => o.ClassificacaoRiscoONU)
                .Fetch(o => o.Licenca)
                .Fetch(o => o.Veiculo).ThenFetch(o => o.Empresa)
                .Fetch(o => o.Filial);

            return ObterLista(consultaLicencaVeiculo, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaLicencaVeiculo filtrosPesquisa)
        {
            var consultaLicencaVeiculo = Consultar(filtrosPesquisa);

            return consultaLicencaVeiculo.Count();
        }

        public bool ContemLicencaVeiculoDuplicada(int codigo, int codigoVeiculo, int codigoLicenca, int codigoFilial, DateTime dataEmissao, DateTime dataVencimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();
            var result = from obj in query
                         where obj.Veiculo.Codigo == codigoVeiculo &&
                               obj.DataEmissao == dataEmissao && obj.DataVencimento == dataVencimento
                         select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo != codigo);

            if (codigoLicenca > 0)
                result = result.Where(obj => obj.Licenca.Codigo == codigoLicenca);

            if (codigoFilial > 0)
                result = result.Where(obj => obj.Filial.Codigo == codigoFilial);

            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> BuscarLicencasVencidas(DateTime horaBase)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();

            var result = from obj in query
                         where
                             obj.DataVencimento.Value.Date < horaBase
                             && ((bool?)obj.Vencido ?? false) == false
                         select obj;

            return result.ToList();
        }

        public bool ContemLicencaValida(int codigoLicenca, DateTime dataAtual, int codigoVeiculo, bool pedido = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>();
            var result = from obj in query 
                         where obj.Veiculo.Codigo == codigoVeiculo 
                            && obj.Licenca.Codigo == codigoLicenca 
                            && ((!obj.DataVencimento.HasValue)
                              || (obj.DataVencimento >= dataAtual)
                              || (obj.DataVencimento < dataAtual && (!pedido || !obj.BloquearCriacaoPedidoLicencaVencida))
                              )
                         select obj;

            return result.Any();
        }

        #endregion
    }
}
