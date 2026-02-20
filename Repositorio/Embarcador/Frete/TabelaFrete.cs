using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFrete : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFrete>
    {
        public TabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TabelaFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarPorGrupoPessoa(int grupoPessoa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro, double? cpfCnpjTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            var result = from obj in query where obj.GrupoPessoas.Codigo == grupoPessoa && obj.Ativo && obj.Filiais.Count() == 0 && obj.PagamentoTerceiro == pagamentoTerceiro && obj.TabelaFreteMinima == tabelaFreteMinima && obj.TabelaCalculoCliente == exclusivaCalculoCliente select obj;

            if (tipoOperacao != null)
                result = result.Where(obj => obj.TiposOperacao.Any(o => o.Codigo == tipoOperacao.Codigo));
            else
                result = result.Where(obj => !obj.TiposOperacao.Any());

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                result = result.Where(o => o.TransportadoresTerceiros.Any(c => c.CPF_CNPJ == cpfCnpjTransportadorTerceiro.Value) || !o.TransportadoresTerceiros.Any());

            if (cpfCnpjTomador.HasValue && cpfCnpjTomador > 0D)
                result = result.Where(o => o.ContratoFreteTransportador == null || o.ContratoFreteTransportador.Clientes.Any(c => c.Cliente.CPF_CNPJ == cpfCnpjTomador));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarPorGrupoPessoaFilial(int grupoPessoa, int filial, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro, double? cpfCnpjTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            var result = from obj in query where obj.GrupoPessoas.Codigo == grupoPessoa && obj.Ativo && obj.PagamentoTerceiro == pagamentoTerceiro && obj.TabelaFreteMinima == tabelaFreteMinima && obj.TabelaCalculoCliente == exclusivaCalculoCliente select obj;

            if (tipoOperacao != null)
                result = result.Where(obj => obj.TiposOperacao.Any(o => o.Codigo == tipoOperacao.Codigo));
            else
                result = result.Where(obj => !obj.TiposOperacao.Any());

            if (filial > 0)
                result = result.Where(obj => obj.Filiais.Any(f => f.Codigo == filial) || (obj.ContratoFreteTransportador != null && obj.ContratoFreteTransportador.Filiais.Any(fil => fil.Codigo == filial)));
            else
                result = result.Where(obj => obj.Filiais.Count() == 0);

            if (cpfCnpjTomador.HasValue && cpfCnpjTomador > 0D)
                result = result.Where(o => o.ContratoFreteTransportador == null || o.ContratoFreteTransportador.Clientes.Any(c => c.Cliente.CPF_CNPJ == cpfCnpjTomador));

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                result = result.Where(o => o.TransportadoresTerceiros.Any(c => c.CPF_CNPJ == cpfCnpjTransportadorTerceiro.Value) || !o.TransportadoresTerceiros.Any());

            return result.FirstOrDefault();
        }

        public List<string> BuscarDescricaoPorCodigo(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query.Select(o => o.Descricao).ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarPorGrupoPessoaFilial(int codigoDiff, int grupoPessoa, IEnumerable<int> filiais, IEnumerable<int> empresas, IEnumerable<int> codigosTiposOperacao, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime localFreeTime)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            var result = from obj in query where obj.GrupoPessoas.Codigo == grupoPessoa && obj.Ativo && obj.PagamentoTerceiro == pagamentoTerceiro && obj.TabelaFreteMinima == tabelaFreteMinima && obj.TabelaCalculoCliente == exclusivaCalculoCliente select obj;

            if (codigosTiposOperacao != null && codigosTiposOperacao.Count() > 0)
                result = result.Where(obj => obj.TiposOperacao.Any(top => codigosTiposOperacao.Contains(top.Codigo)));
            else
                result = result.Where(obj => !obj.TiposOperacao.Any());

            if (filiais != null && filiais.Count() > 0)
                result = result.Where(obj => obj.Filiais.Any(fil => filiais.Contains(fil.Codigo)) || (obj.ContratoFreteTransportador != null && obj.ContratoFreteTransportador.Filiais.Any(fil => filiais.Contains(fil.Codigo))));
            else
                result = result.Where(obj => obj.Filiais.Count() == 0);

            if (empresas != null && empresas.Count() > 0)
                result = result.Where(obj => obj.Transportadores.Any(emp => empresas.Contains(emp.Codigo)) || (obj.ContratoFreteTransportador != null && empresas.Contains(obj.ContratoFreteTransportador.Transportador.Codigo)));
            else
                result = result.Where(obj => obj.Transportadores.Count() == 0);

            if (codigoDiff > 0)
                result = result.Where(obj => obj.Codigo != codigoDiff);

            if (localFreeTime == Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime.Coleta || localFreeTime == Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime.Fronteira ||
                localFreeTime == Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime.Entrega)
                result = result.Where(obj => obj.LocalFreeTime == localFreeTime);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorGrupoPessoas(int grupoPessoa, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro = null, int? codigoTipoTerceiro = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(obj => obj.GrupoPessoas.Codigo == grupoPessoa &&
                                       obj.Ativo &&
                                       obj.PagamentoTerceiro == pagamentoTerceiro &&
                                       obj.TabelaFreteMinima == tabelaFreteMinima &&
                                       obj.TabelaCalculoCliente == exclusivaCalculoCliente);

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                query = query.Where(o => o.TransportadoresTerceiros.Any(c => c.CPF_CNPJ == cpfCnpjTransportadorTerceiro.Value) || !o.TransportadoresTerceiros.Any());

            if (pagamentoTerceiro && codigoTipoTerceiro.HasValue)
                query = query.Where(o => o.TiposTerceiros.Any(c => c.Codigo == codigoTipoTerceiro.Value) || !o.TiposTerceiros.Any());

            return query
                .OrderBy(obj => obj.ContratoFreteTransportador).ThenByDescending(obj => obj.PossuiVeiculos)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarTabelasParaPrioridadeCalculo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarPorGrupoPessoa(int grupoPessoa, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            var result = from obj in query where obj.GrupoPessoas.Codigo == grupoPessoa && obj.Ativo && obj.Filiais.Count() == 0 && obj.PagamentoTerceiro == pagamentoTerceiro && obj.TabelaFreteMinima == tabelaFreteMinima && obj.TabelaCalculoCliente == exclusivaCalculoCliente select obj;

            return result.FirstOrDefault();
        }

        public bool ExisteTabelaResidual()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            var result = from obj in query where obj.UtilizarDiferencaDoValorBaseApenasFretePagos select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarPorGrupoPessoaSemTipoOperacao(int grupoPessoa, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro, double? cpfCnpjTomador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(obj => obj.GrupoPessoas.Codigo == grupoPessoa && obj.Ativo && !obj.Filiais.Any() && !obj.TiposOperacao.Any() && obj.PagamentoTerceiro == pagamentoTerceiro && obj.TabelaFreteMinima == tabelaFreteMinima && obj.TabelaCalculoCliente == exclusivaCalculoCliente);

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                query = query.Where(o => o.TransportadoresTerceiros.Any(t => t.CPF_CNPJ == cpfCnpjTransportadorTerceiro) || !o.TransportadoresTerceiros.Any());

            if (cpfCnpjTomador.HasValue && cpfCnpjTomador > 0D)
                query = query.Where(o => o.ContratoFreteTransportador == null || o.ContratoFreteTransportador.Clientes.Any(c => c.Cliente.CPF_CNPJ == cpfCnpjTomador));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarSemGrupoPessoaECodigoDiff(int codigoDiff, IEnumerable<int> codigosTiposOperacao, IEnumerable<double> cpfCnpjTerceiros, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, IEnumerable<int> tipoTerceiros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(obj => obj.Codigo != codigoDiff && obj.GrupoPessoas == null && obj.Ativo && obj.PagamentoTerceiro == pagamentoTerceiro && obj.TabelaFreteMinima == tabelaFreteMinima && obj.TabelaCalculoCliente == exclusivaCalculoCliente);

            if (codigosTiposOperacao.Any())
                query = query.Where(o => o.TiposOperacao.Any(t => codigosTiposOperacao.Contains(t.Codigo)));
            else
                query = query.Where(o => !o.TiposOperacao.Any());

            if (pagamentoTerceiro)
            {
                if (cpfCnpjTerceiros.Any())
                    query = query.Where(o => o.TransportadoresTerceiros.Any(t => cpfCnpjTerceiros.Contains(t.CPF_CNPJ)));
                else
                    query = query.Where(o => !o.TransportadoresTerceiros.Any());

                if (tipoTerceiros.Any())
                    query = query.Where(o => o.TiposTerceiros.Any(t => tipoTerceiros.Contains(t.Codigo)));
                else
                    query = query.Where(o => !o.TiposTerceiros.Any());
            }

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarSemGrupoPessoaETipoOperacao(int codigoTipoOperacao, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro, double? cpfCnpjTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(obj => obj.GrupoPessoas == null && obj.Ativo && obj.TiposOperacao.Any(o => o.Codigo == codigoTipoOperacao) && obj.PagamentoTerceiro == pagamentoTerceiro && obj.TabelaFreteMinima == tabelaFreteMinima && obj.TabelaCalculoCliente == exclusivaCalculoCliente);

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                query = query.Where(o => o.TransportadoresTerceiros.Any(t => t.CPF_CNPJ == cpfCnpjTransportadorTerceiro) || !o.TransportadoresTerceiros.Any());

            if (cpfCnpjTomador.HasValue && cpfCnpjTomador > 0D)
                query = query.Where(o => o.ContratoFreteTransportador == null || o.ContratoFreteTransportador.Clientes.Any(c => c.Cliente.CPF_CNPJ == cpfCnpjTomador));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarSemGrupoPessoaESemTipoOperacao(bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro, double? cpfCnpjTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(obj => obj.GrupoPessoas == null && obj.Ativo && !obj.TiposOperacao.Any() && obj.PagamentoTerceiro == pagamentoTerceiro && obj.TabelaFreteMinima == tabelaFreteMinima && obj.TabelaCalculoCliente == exclusivaCalculoCliente);

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                query = query.Where(o => o.TransportadoresTerceiros.Any(t => t.CPF_CNPJ == cpfCnpjTransportadorTerceiro) || !o.TransportadoresTerceiros.Any());

            if (cpfCnpjTomador.HasValue && cpfCnpjTomador > 0D)
                query = query.Where(o => o.ContratoFreteTransportador == null || o.ContratoFreteTransportador.Clientes.Any(c => c.Cliente.CPF_CNPJ == cpfCnpjTomador));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarPorCodigoFetch(int codigo)
        {
            var consultaTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>()
                .Where(o => o.Codigo == codigo);

            return consultaTabelaFrete
                .Fetch(obj => obj.ComponenteFreteAjudante)
                .Fetch(obj => obj.ComponenteFreteHora)
                .Fetch(obj => obj.ComponenteFreteNumeroEntregas)
                .Fetch(obj => obj.ComponenteFretePallet)
                .Fetch(obj => obj.ComponenteFretePeso)
                .Fetch(obj => obj.ComponenteFreteQuilometragem)
                .Fetch(obj => obj.ComponenteFreteQuilometragemExcedente)
                .Fetch(obj => obj.ComponenteFreteTempo)
                .Fetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.ContratoFreteTransportador)
                .Fetch(obj => obj.TipoOcorrenciaTabelaMinima)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarPorCodigo(int codigo)
        {
            var consultaTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>()
                .Where(o => o.Codigo == codigo);

            return consultaTabelaFrete.FirstOrDefault();
        }

        public string BuscarDescricaoPorCodigo(int codigo)
        {
            var consultaTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>()
                .Where(o => o.Codigo == codigo);

            return consultaTabelaFrete.Select(o => o.Descricao).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorCodigos(List<int> codigos)
        {
            var consultaTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>()
                .Where(o => codigos.Contains(o.Codigo));

            return consultaTabelaFrete
                .Fetch(obj => obj.TiposOperacao)
                .Fetch(obj => obj.Transportadores)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete tipoTabelaFrete, string codigoIntegracao, int codigoGrupoPessoas, bool calcularFreteDestinoPrioritario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoTabela aplicacaoTabela, int empresa, int tipoCarga, int filial, int tipoOperacao, int tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete? situacaoAlteracao, int veiculo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaTabelaFrete = Consultar(descricao, ativo, tipoTabelaFrete, codigoIntegracao, codigoGrupoPessoas, calcularFreteDestinoPrioritario, aplicacaoTabela, empresa, tipoCarga, filial, tipoOperacao, tipoOcorrencia, situacaoAlteracao, veiculo);

            return ObterLista(consultaTabelaFrete, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete tipoTabelaFrete, string codigoIntegracao, int codigoGrupoPessoas, bool calcularFreteDestinoPrioritario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoTabela aplicacaoTabela, int empresa, int tipoCarga, int filial, int tipoOperacao, int tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete? situacaoAlteracao, int veiculo)
        {
            var consultaTabelaFrete = Consultar(descricao, ativo, tipoTabelaFrete, codigoIntegracao, codigoGrupoPessoas, calcularFreteDestinoPrioritario, aplicacaoTabela, empresa, tipoCarga, filial, tipoOperacao, tipoOcorrencia, situacaoAlteracao, veiculo);

            return consultaTabelaFrete.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete tipoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            var result = from obj in query select obj;
            result = result.Where(tbf => tbf.Ativo == true);

            if (tipoTabelaFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.todas)
            {
                result = result.Where(tbf => tbf.TipoTabelaFrete == tipoTabelaFrete);
            }

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorContratoFreteTransportador(int contrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            var result = from o in query where o.ContratoFreteTransportador.Codigo == contrato select o;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete BuscarPadrao(bool pagamentoTerceiro, double? cpfCnpjTransportadorTerceiro = null, int? codigoTipoTerceiro = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(o => o.Ativo && o.Padrao && o.PagamentoTerceiro == pagamentoTerceiro);

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                query = query.Where(o => o.TransportadoresTerceiros.Any(c => c.CPF_CNPJ == cpfCnpjTransportadorTerceiro.Value) || !o.TransportadoresTerceiros.Any());

            if (pagamentoTerceiro && codigoTipoTerceiro.HasValue)
                query = query.Where(o => o.TiposTerceiros.Any(c => c.Codigo == codigoTipoTerceiro.Value) || !o.TiposTerceiros.Any());

            return query
              .Fetch(obj => obj.ContratoFreteTransportador)
              .ThenFetch(obj => obj.Transportador)
              .OrderBy(obj => obj.ContratoFreteTransportador).ThenByDescending(obj => obj.PossuiVeiculos)
              .FirstOrDefault();
        }

        public int ContarPorTipoCarga(int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            var result = from obj in query where (from tpc in obj.TiposCarga where tpc.Codigo == codigoTipoCarga select tpc.Codigo).Any() select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorTipoOperacao(int codigoTipoOperacao, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro = null, int? codigoTipoTerceiro = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(tbf => tbf.Ativo &&
                                       tbf.TiposOperacao.Any(o => o.Codigo == codigoTipoOperacao) &&
                                       tbf.PagamentoTerceiro == pagamentoTerceiro &&
                                       tbf.TabelaFreteMinima == tabelaFreteMinima &&
                                       tbf.TabelaCalculoCliente == exclusivaCalculoCliente);

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                query = query.Where(o => o.TransportadoresTerceiros.Any(c => c.CPF_CNPJ == cpfCnpjTransportadorTerceiro.Value) || !o.TransportadoresTerceiros.Any());

            if (pagamentoTerceiro && codigoTipoTerceiro.HasValue)
                query = query.Where(o => o.TiposTerceiros.Any(c => c.Codigo == codigoTipoTerceiro.Value) || !o.TiposTerceiros.Any());

            return query
                .Fetch(obj => obj.ContratoFreteTransportador)
                .ThenFetch(obj => obj.Transportador)
                .OrderBy(obj => obj.ContratoFreteTransportador).ThenByDescending(obj => obj.PossuiVeiculos)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorCanalEntrega(int codigoCanalEntrega, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro = null, int? codigoTipoTerceiro = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(tbf => tbf.Ativo && tbf.CanalEntrega.Codigo == codigoCanalEntrega && tbf.PagamentoTerceiro == pagamentoTerceiro && tbf.TabelaFreteMinima == tabelaFreteMinima && tbf.TabelaCalculoCliente == exclusivaCalculoCliente);

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                query = query.Where(o => o.TransportadoresTerceiros.Any(c => c.CPF_CNPJ == cpfCnpjTransportadorTerceiro.Value) || !o.TransportadoresTerceiros.Any());

            if (pagamentoTerceiro && codigoTipoTerceiro.HasValue)
                query = query.Where(o => o.TiposTerceiros.Any(c => c.Codigo == codigoTipoTerceiro.Value) || !o.TiposTerceiros.Any());

            return query
                .Fetch(obj => obj.ContratoFreteTransportador)
                .ThenFetch(obj => obj.Transportador)
                .OrderBy(obj => obj.ContratoFreteTransportador).ThenByDescending(obj => obj.PossuiVeiculos)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorTransportadorTerceiro(bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro, int? codigoTipoTerceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(tbf => tbf.Ativo && tbf.PagamentoTerceiro && tbf.TabelaFreteMinima == tabelaFreteMinima && tbf.TabelaCalculoCliente == exclusivaCalculoCliente);

            if (cpfCnpjTransportadorTerceiro.HasValue)
                query = query.Where(o => o.TransportadoresTerceiros.Any(c => c.CPF_CNPJ == cpfCnpjTransportadorTerceiro.Value) || !o.TransportadoresTerceiros.Any());

            if (codigoTipoTerceiro.HasValue)
                query = query.Where(o => o.TiposTerceiros.Any(c => c.Codigo == codigoTipoTerceiro.Value) || !o.TiposTerceiros.Any());

            return query
                .Fetch(obj => obj.ContratoFreteTransportador)
                .ThenFetch(obj => obj.Transportador)
                .OrderBy(obj => obj.ContratoFreteTransportador).ThenByDescending(obj => obj.PossuiVeiculos)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorTipoOperacao(List<int> codigosTipoOperacao, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(tbf => tbf.Ativo && tbf.PagamentoTerceiro == pagamentoTerceiro && tbf.TabelaFreteMinima == tabelaFreteMinima && tbf.TabelaCalculoCliente == exclusivaCalculoCliente);

            foreach (int codigoTipoOperacao in codigosTipoOperacao)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao() { Codigo = codigoTipoOperacao };
                query = query.Where(obj => obj.TiposOperacao.Contains(tipoOperacao));
            }

            return query
                .Fetch(obj => obj.ContratoFreteTransportador)
                .ThenFetch(obj => obj.Transportador).OrderBy(obj => obj.ContratoFreteTransportador)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorEmpresa(int empresa, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro = null, int? codigoTipoTerceiro = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(tbf => tbf.Ativo && tbf.PagamentoTerceiro == pagamentoTerceiro && tbf.TabelaFreteMinima == tabelaFreteMinima && tbf.TabelaCalculoCliente == exclusivaCalculoCliente);

            query = query.Where(obj =>
                (
                    (obj.ContratoFreteTransportador != null) &&
                    (obj.ContratoFreteTransportador.Transportador == null || obj.ContratoFreteTransportador.Transportador.Codigo == empresa || obj.ContratoFreteTransportador.Transportador.Filiais.Any(fil => fil.Codigo == empresa)) &&
                    (obj.Transportadores.Count() == 0)
                ) ||
                (obj.Transportadores.Any(t => t.Codigo == empresa) && obj.ContratoFreteTransportador == null)
            );

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                query = query.Where(o => o.TransportadoresTerceiros.Any(c => c.CPF_CNPJ == cpfCnpjTransportadorTerceiro.Value) || !o.TransportadoresTerceiros.Any());

            if (pagamentoTerceiro && codigoTipoTerceiro.HasValue)
                query = query.Where(o => o.TiposTerceiros.Any(c => c.Codigo == codigoTipoTerceiro.Value) || !o.TiposTerceiros.Any());

            return query
                .Fetch(obj => obj.ContratoFreteTransportador).ThenFetch(obj => obj.Transportador)
                .OrderBy(obj => obj.ContratoFreteTransportador).ThenByDescending(obj => obj.PossuiVeiculos).ThenBy(obj => obj.ContratoFreteTransportador.Transportador == null)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorFilial(int filial, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente, double? cpfCnpjTransportadorTerceiro = null, int? codigoTipoTerceiro = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(tbf => tbf.Ativo && tbf.PagamentoTerceiro == pagamentoTerceiro && tbf.TabelaFreteMinima == tabelaFreteMinima && tbf.TabelaCalculoCliente == exclusivaCalculoCliente && tbf.TabelaCalculoCliente == exclusivaCalculoCliente);

            if (filial > 0)
                query = query.Where(obj => obj.Filiais.Any(f => filial == f.Codigo) || (obj.ContratoFreteTransportador != null && obj.ContratoFreteTransportador.Filiais.Any(fil => fil.Filial.Codigo == filial)));

            if (pagamentoTerceiro && cpfCnpjTransportadorTerceiro.HasValue)
                query = query.Where(o => o.TransportadoresTerceiros.Any(c => c.CPF_CNPJ == cpfCnpjTransportadorTerceiro.Value) || !o.TransportadoresTerceiros.Any());

            if (pagamentoTerceiro && codigoTipoTerceiro.HasValue)
                query = query.Where(o => o.TiposTerceiros.Any(c => c.Codigo == codigoTipoTerceiro.Value) || !o.TiposTerceiros.Any());

            return query
                .Fetch(obj => obj.ContratoFreteTransportador)
                .ThenFetch(obj => obj.Transportador)
                .OrderBy(obj => obj.ContratoFreteTransportador).ThenByDescending(obj => obj.PossuiVeiculos)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> BuscarPorFilial(List<int> filiais, bool pagamentoTerceiro, bool tabelaFreteMinima, bool exclusivaCalculoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(tbf => tbf.Ativo && tbf.PagamentoTerceiro == pagamentoTerceiro && tbf.TabelaFreteMinima == tabelaFreteMinima && tbf.TabelaCalculoCliente == exclusivaCalculoCliente);

            if (filiais.Count > 0)
                query = query.Where(obj => obj.Filiais.Any(f => filiais.Contains(f.Codigo)));

            return query
                  .Fetch(obj => obj.ContratoFreteTransportador)
                  .ThenFetch(obj => obj.Transportador)
                  .ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoTabelaFrete> ConsultarRelatorioConfiguracao(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery consultaCargasProdutos = new ConsultaConfiguracaoTabelaFrete().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCargasProdutos.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoTabelaFrete)));

            return consultaCargasProdutos.SetTimeout(300).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoTabelaFrete>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoTabelaFrete>> ConsultarRelatorioConfiguracaoAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery consultaCargasProdutos = new ConsultaConfiguracaoTabelaFrete().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCargasProdutos.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoTabelaFrete)));

            return await consultaCargasProdutos.SetTimeout(300).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoTabelaFrete>();
        }

        public async Task<int> ContarConsultaRelatorioConfiguracao(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery consultaCargasProdutos = new ConsultaConfiguracaoTabelaFrete().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return await consultaCargasProdutos.SetTimeout(600).UniqueResultAsync<int>();
        }

        public List<int> BuscarFiliaisPorTabelaFrete(int codigoTabelaFrete)
        {
            var consultaTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>()
                .Where(o => o.Codigo == codigoTabelaFrete);

            return consultaTabelaFrete.Select(o => o.Filiais.Select(or => or.Codigo)).SelectMany(o => o).Distinct().ToList();
        }

        public List<int> BuscarTiposOperacaoPorTabelaFrete(int codigoTabelaFrete)
        {
            var consultaTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>()
                .Where(o => o.Codigo == codigoTabelaFrete);

            return consultaTabelaFrete.Select(o => o.TiposOperacao.Select(or => or.Codigo)).SelectMany(o => o).Distinct().ToList();
        }

        public List<int> BuscarTransportadoresPorTabelaFrete(int codigoTabelaFrete)
        {
            var consultaTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>()
                .Where(o => o.Codigo == codigoTabelaFrete);

            return consultaTabelaFrete.Select(o => o.Transportadores.Select(or => or.Codigo)).SelectMany(o => o).Distinct().ToList();
        }

        #endregion

        #region Relatório de Configurações de Subcontratacao da Tabela de Frete

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFrete> BuscarRelatorioConfiguracoes(List<int> codigosTabelasFrete, List<int> codigosGruposPessoas)
        {
            string sqlQuery = @"SELECT 
                                TabelaFrete.TBF_CODIGO Codigo, 
                                TabelaFrete.TBF_DESCRICAO Descricao, 
                                TabelaFrete.TBF_PERCENTUAL_COBRANCA_PADRAO_TERCEIROS PercentualCobrancaPadrao 
                                FROM T_TABELA_FRETE TabelaFrete 
                                WHERE TabelaFrete.TBF_ATIVO = 1 ";

            if (codigosTabelasFrete.Count > 0)
                sqlQuery += "AND TabelaFrete.TBF_CODIGO IN (" + string.Join(",", codigosTabelasFrete) + ") ";

            if (codigosGruposPessoas.Count > 0)
                sqlQuery += "AND TabelaFrete.GRP_CODIGO IN (" + string.Join(",", codigosGruposPessoas) + ") ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFrete)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFrete>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFreteTerceiro> BuscarRelatorioConfiguracoesTerceiros(List<int> codigosTabelasFrete, List<int> codigosGruposPessoas)
        {
            string sqlQuery = @"SELECT 
                                TabelaFreteSubcontratacao.TFS_CODIGO Codigo,
                                TabelaFreteSubcontratacao.TBF_CODIGO CodigoTabelaFrete, 
                                Terceiro.CLI_CGCCPF CPFCNPJTerceiro,
                                Terceiro.CLI_NOME NomeTerceiro,
                                Terceiro.CLI_FISJUR TipoTerceiro,
                                TabelaFreteSubcontratacao.TFS_PERCENTUAL_COBRANCA PercentualCobranca,
                                TabelaFreteSubcontratacao.TFS_PERCENTUAL_DESCONTO PercentualDesconto
                                FROM T_TABELA_FRETE_SUBCONTRATACAO TabelaFreteSubcontratacao
                                INNER JOIN T_TABELA_FRETE TabelaFrete ON TabelaFrete.TBF_CODIGO = TabelaFreteSubcontratacao.TBF_CODIGO
                                INNER JOIN T_CLIENTE Terceiro ON Terceiro.CLI_CGCCPF = TabelaFreteSubcontratacao.CLI_CGCCPF
                                WHERE TabelaFrete.TBF_ATIVO = 1 ";

            if (codigosTabelasFrete.Count > 0)
                sqlQuery += "AND TabelaFrete.TBF_CODIGO IN (" + string.Join(",", codigosTabelasFrete) + ") ";

            if (codigosGruposPessoas.Count > 0)
                sqlQuery += "AND TabelaFrete.GRP_CODIGO IN (" + string.Join(",", codigosGruposPessoas) + ") ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFreteTerceiro)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFreteTerceiro>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFreteClienteTerceiro> BuscarRelatorioConfiguracoesTerceirosTabelaFreteCliente(List<int> codigosTabelasFrete, List<int> codigosGruposPessoas)
        {
            string sqlQuery = @"SELECT 
                                TabelaFreteClienteSubcontratacao.TCS_CODIGO Codigo,
                                TabelaFrete.TBF_CODIGO CodigoTabelaFrete,
                                Terceiro.CLI_CGCCPF CPFCNPJTerceiro,
                                Terceiro.CLI_NOME NomeTerceiro,
                                Terceiro.CLI_FISJUR TipoTerceiro,
                                TabelaFreteCliente.TFC_DESCRICAO_ORIGEM Origem, 
                                TabelaFreteCliente.TFC_DESCRICAO_DESTINO Destino,
                                TabelaFreteCliente.TFC_PERCENTUAL_COBRANCA_PADRAO_TERCEIROS PercentualCobrancaPadrao,
                                TabelaFreteClienteSubcontratacao.TCS_VALOR_FIXO_SUBCONTRATACAO_PARCIAL ValorFixoSubcontratacaoParcial,
                                TabelaFreteClienteSubcontratacao.TCS_PERCENTUAL_DESCONTO PercentualDesconto
                                FROM T_TABELA_FRETE_CLIENTE_SUB_CONTRATACAO TabelaFreteClienteSubcontratacao
                                INNER JOIN T_TABELA_FRETE_CLIENTE TabelaFreteCliente ON TabelaFreteCliente.TFC_CODIGO = TabelaFreteClienteSubcontratacao.TFC_CODIGO
                                INNER JOIN T_TABELA_FRETE TabelaFrete ON TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO
                                INNER JOIN T_CLIENTE Terceiro ON Terceiro.CLI_CGCCPF = TabelaFreteClienteSubcontratacao.CLI_CGCCPF
                                WHERE TabelaFrete.TBF_ATIVO = 1 and TabelaFreteCliente.TFC_ATIVO = 1 ";

            if (codigosTabelasFrete.Count > 0)
                sqlQuery += "AND TabelaFrete.TBF_CODIGO IN (" + string.Join(",", codigosTabelasFrete) + ") ";

            if (codigosGruposPessoas.Count > 0)
                sqlQuery += "AND TabelaFrete.GRP_CODIGO IN (" + string.Join(",", codigosGruposPessoas) + ") ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFreteClienteTerceiro)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFreteClienteTerceiro>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFrete> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete tipoTabelaFrete, string codigoIntegracao, int codigoGrupoPessoas, bool calcularFreteDestinoPrioritario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoTabela aplicacaoTabela, int empresa, int tipoCarga, int filial, int tipoOperacao, int tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete? situacaoAlteracao, int veiculo)
        {
            var consultaTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaTabelaFrete = consultaTabelaFrete.Where(tbf => tbf.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaTabelaFrete = consultaTabelaFrete.Where(tbf => tbf.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaTabelaFrete = consultaTabelaFrete.Where(tbf => tbf.Ativo == false);

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                consultaTabelaFrete = consultaTabelaFrete.Where(o => o.CodigoIntegracao.Contains(codigoIntegracao));

            if (codigoGrupoPessoas > 0)
                consultaTabelaFrete = consultaTabelaFrete.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            if (tipoOperacao > 0)
                consultaTabelaFrete = consultaTabelaFrete.Where(o => o.TiposOperacao.Any(obj => obj.Codigo == tipoOperacao));

            if (veiculo > 0)
                consultaTabelaFrete = consultaTabelaFrete.Where(o => o.ContratoFreteTransportador.Veiculos.Any(obj => obj.Codigo == veiculo));

            if (empresa > 0)
                consultaTabelaFrete = consultaTabelaFrete.Where(o => o.Transportadores.Any(obj => obj.Codigo == empresa));

            if (filial > 0)
                consultaTabelaFrete = consultaTabelaFrete.Where(o => o.Filiais.Any(obj => obj.Codigo == filial));

            if (tipoCarga > 0)
                consultaTabelaFrete = consultaTabelaFrete.Where(o => o.TiposCarga.Any(obj => obj.Codigo == tipoCarga));

            if (tipoOcorrencia > 0)
                consultaTabelaFrete = consultaTabelaFrete.Where(o => o.TiposDeOcorrencia.Any(obj => obj.Codigo == tipoOcorrencia));

            if (aplicacaoTabela != Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoTabela.Todas)
                consultaTabelaFrete = consultaTabelaFrete.Where(tbf => tbf.AplicacaoTabela == aplicacaoTabela);

            if (tipoTabelaFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.todas)
                consultaTabelaFrete = consultaTabelaFrete.Where(tbf => tbf.TipoTabelaFrete == tipoTabelaFrete);

            if (calcularFreteDestinoPrioritario)
                consultaTabelaFrete = consultaTabelaFrete.Where(o => o.CalcularFreteDestinoPrioritario == true);

            if (situacaoAlteracao.HasValue)
            {
                if (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFreteHelper.IsAlteracaoTabelaFreteClienteLiberada(situacaoAlteracao.Value))
                {
                    consultaTabelaFrete = consultaTabelaFrete.Where(o =>
                        (o.SituacaoAlteracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete.Aprovada) ||
                        (o.SituacaoAlteracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete.NaoInformada)
                    );
                }
                else
                    consultaTabelaFrete = consultaTabelaFrete.Where(o => o.SituacaoAlteracao == situacaoAlteracao.Value);
            }

            return consultaTabelaFrete;
        }

        #endregion
    }
}
