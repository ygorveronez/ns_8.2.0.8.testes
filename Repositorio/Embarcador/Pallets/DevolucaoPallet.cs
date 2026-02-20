using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.Relatorios.Embarcador.DataSource.Pallets;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Pallets
{
    public class DevolucaoPallet : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>
    {
        #region Construtores

        public DevolucaoPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> ConsultarDevolucaoSemValePallet(int numeroNotaFiscal, string numeroCarga, string cargaOuNotaFiscal, int codigoTransportador, DateTime? dataEmissaoInicial, DateTime? dataEmissaoFinal)
        {
            var consultaValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>()
                .Where(valePallet => valePallet.Situacao != SituacaoValePallet.Cancelado);

            var consultaDevolucaoPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>()
                .Where(o => !consultaValePallet.Where(v => v.Devolucao.Codigo == o.Codigo).Any());

            if (!String.IsNullOrWhiteSpace(cargaOuNotaFiscal) && !String.IsNullOrWhiteSpace(numeroCarga))
            {
                int.TryParse(cargaOuNotaFiscal, out numeroNotaFiscal);

                consultaDevolucaoPallet = consultaDevolucaoPallet.Where(devolucao => (devolucao.CargaPedido.Carga.CodigoCargaEmbarcador == numeroCarga) && (devolucao.XMLNotaFiscal.Numero == numeroNotaFiscal));
            }
            else if (!String.IsNullOrWhiteSpace(cargaOuNotaFiscal))
            {
                int.TryParse(cargaOuNotaFiscal, out numeroNotaFiscal);

                consultaDevolucaoPallet = consultaDevolucaoPallet.Where(devolucao => (devolucao.CargaPedido.Carga.CodigoCargaEmbarcador == cargaOuNotaFiscal) || (devolucao.XMLNotaFiscal.Numero == numeroNotaFiscal));
            }
            else
            {
                if (numeroNotaFiscal > 0)
                    consultaDevolucaoPallet = consultaDevolucaoPallet.Where(devolucao => devolucao.XMLNotaFiscal.Numero == numeroNotaFiscal);

                if (!string.IsNullOrWhiteSpace(numeroCarga))
                    consultaDevolucaoPallet = consultaDevolucaoPallet.Where(devolucao => devolucao.CargaPedido.Carga.CodigoCargaEmbarcador == numeroCarga);
            }

            if (codigoTransportador > 0)
                consultaDevolucaoPallet = consultaDevolucaoPallet.Where(devolucao => devolucao.Transportador.Codigo == codigoTransportador);

            if (dataEmissaoInicial.HasValue)
                consultaDevolucaoPallet = consultaDevolucaoPallet.Where(devolucao => devolucao.XMLNotaFiscal.DataEmissao >= dataEmissaoInicial.Value.Date);

            if (dataEmissaoFinal.HasValue)
                consultaDevolucaoPallet = consultaDevolucaoPallet.Where(devolucao => devolucao.XMLNotaFiscal.DataEmissao <= dataEmissaoFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaDevolucaoPallet;
        }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> ConsultaPorFechamento(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorFechamento(int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> BuscarPorData(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            var result = from obj in query
                         where
                             obj.Fechamento == null
                             && obj.DataDevolucao >= dataInicial
                             && obj.DataDevolucao <= dataFinal
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Entregue
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> BuscarPorFechamento(int fechamento, bool? adicionado = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (adicionado.HasValue)
                result = result.Where(o => o.AdicionarAoFechamento == adicionado.Value);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            int? retorno = query.Max(o => (int?)o.NumeroDevolucao);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet BuscarPrimeiroPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet BuscarPorCargaPedidoEXMLNotaFiscal(int codigoCargaPedido, int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            if (codigoCargaPedido > 0)
                query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            query = query.Where(o => o.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet BuscarPorXMLNotaFiscal(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            query = query.Where(o => o.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaDevolucaoPallet filtrosPesquisa, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            query = query.Where(o => o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Cancelado);

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
                query = query.Where(o => o.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNotaFiscal);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                query = query.Where(o => o.CargaPedido.Carga.CodigoCargaEmbarcador.Equals(filtrosPesquisa.NumeroCarga));

            if (filtrosPesquisa.CodigoFilial > 0)
                query = query.Where(o => (o.CargaPedido != null && o.CargaPedido.Carga.Filial.Codigo == filtrosPesquisa.CodigoFilial) || o.XMLNotaFiscal.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoMotorista > 0)
                query = query.Where(o => o.CargaPedido.Carga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista));

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query = query.Where(o => o.CargaPedido.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.CargaPedido.Carga.VeiculosVinculados.Any(v => v.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                query = query.Where(o => filtrosPesquisa.CodigosTransportador.Contains(o.Transportador.Codigo));

            if (filtrosPesquisa.NumeroDevolucao > 0)
                query = query.Where(o => o.NumeroDevolucao == filtrosPesquisa.NumeroDevolucao);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.XMLNotaFiscal.DataEmissao >= filtrosPesquisa.DataEmissaoInicial.Date);

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.XMLNotaFiscal.DataEmissao < filtrosPesquisa.DataEmissaoFinal.AddDays(1).Date);

            if (filtrosPesquisa.DataBaixaInicial != DateTime.MinValue)
                query = query.Where(o => o.DataDevolucao >= filtrosPesquisa.DataBaixaInicial.Date);

            if (filtrosPesquisa.DataBaixaFinal != DateTime.MinValue)
                query = query.Where(o => o.DataDevolucao < filtrosPesquisa.DataBaixaFinal.AddDays(1).Date);

            if (filtrosPesquisa.CpfCnpjRemetente > 0)
                query = query.Where(o => o.CargaPedido.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente);

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                query = query.Where(o => o.CargaPedido.Carga.GrupoPessoaPrincipal.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                query = query.Where(o => o.CargaPedido.Carga.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

           if (filtrosPesquisa.CodigoTomador > 0)
                query = query.Where(o => (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && (o.CargaPedido.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CodigoTomador)) ||
                                         (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && (o.CargaPedido.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CodigoTomador)) ||
                                         (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && (o.CargaPedido.Pedido.Recebedor.CPF_CNPJ == filtrosPesquisa.CodigoTomador)) ||
                                         (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && (o.CargaPedido.Pedido.Expedidor.CPF_CNPJ == filtrosPesquisa.CodigoTomador)));

           if(filtrosPesquisa.NaoExibirSemNotaFiscal)
                query = query.Where(obj => obj.XMLNotaFiscal != null);

            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> ConsultarDevolucaoSemValePallet(int numeroNotaFiscal, string numeroCarga, string cargaOuNotaFiscal, int codigoTransportador, DateTime? dataEmissaoInicial, DateTime? dataEmissaoFinal, string propriedadeOrdenar, string direcaoOrdenacao, int inicio, int limite)
        {
            var listaDevolucao = ConsultarDevolucaoSemValePallet(numeroNotaFiscal, numeroCarga, cargaOuNotaFiscal, codigoTransportador, dataEmissaoInicial, dataEmissaoFinal);

            return ObterLista(listaDevolucao, propriedadeOrdenar, direcaoOrdenacao, inicio, limite);
        }

        public int ContarConsultaRelatorio(int codigoCarga, List<int> codigosTransportador, int codigoVeiculo, int codigoMotorista, int numeroNotaFiscal, string numeroCarga, bool? naoExibirDevolucaoPaletesSemNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet? situacao, DateTime dataInicio, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            if (codigosTransportador?.Count() > 0)
                query = query.Where(o => codigosTransportador.Contains(o.Transportador.Codigo));

            if (codigoVeiculo > 0)
                query = query.Where(o => o.CargaPedido.Carga.Veiculo.Codigo == codigoVeiculo || o.CargaPedido.Carga.VeiculosVinculados.Any(v => v.Codigo == codigoVeiculo));

            if (codigoMotorista > 0)
                query = query.Where(o => o.CargaPedido.Carga.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (numeroNotaFiscal > 0)
                query = query.Where(o => o.XMLNotaFiscal.Numero == numeroNotaFiscal);

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.CargaPedido.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (codigoCarga != 0)
                query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao.Value);

            if (dataInicio != DateTime.MinValue)
                query = query.Where(obj => obj.DataDevolucao >= dataInicio);

            if (dataFim != DateTime.MinValue)
                query = query.Where(obj => obj.DataDevolucao <= dataFim);

            if (naoExibirDevolucaoPaletesSemNotaFiscal.HasValue && naoExibirDevolucaoPaletesSemNotaFiscal.Value)
                query = query.Where(obj => obj.XMLNotaFiscal != null);

            return query.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pallets.SituacoesPalletsCarga> ConsultarSituacaoPalletCarga(int codigoCarga)
        {
            string sql = @"SELECT ISNULL(SUBSTRING((SELECT DISTINCT ', ' +
                    CASE
                    WHEN PDE_SITUACAO = 0 THEN 'Aguardando Entrega'
                    WHEN PDE_SITUACAO = 1 THEN 'Entregue'
                    WHEN PDE_SITUACAO = 2 THEN 'Cancelado'
                    ELSE 'Nenhum'
                    END  FOR XML PATH('')), 3, 1000), '') Situacao
                    FROM T_PALLET_DEVOLUCAO Devolucao
                    JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = Devolucao.CPE_CODIGO 
                    WHERE CargaPedido.CAR_CODIGO = " + codigoCarga.ToString("D");

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pallets.SituacoesPalletsCarga)));
            return query.SetTimeout(300).List<Dominio.ObjetosDeValor.Embarcador.Pallets.SituacoesPalletsCarga>();
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Pallets.Devolucao> ConsultarRelatorio(int codigoCarga, List<int> codigosTransportador, int codigoVeiculo, int codigoMotorista, int numeroNotaFiscal, string numeroCarga, bool? naoExibirDevolucaoPaletesSemNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet? situacao, DateTime dataInicio, DateTime dataFim, List<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet> situacoesDevolucao, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            if (codigosTransportador?.Count() > 0)
                query = query.Where(o => codigosTransportador.Contains(o.Transportador.Codigo));

            if (codigoVeiculo > 0)
                query = query.Where(o => o.CargaPedido.Carga.Veiculo.Codigo == codigoVeiculo || o.CargaPedido.Carga.VeiculosVinculados.Any(v => v.Codigo == codigoVeiculo));

            if (codigoMotorista > 0)
                query = query.Where(o => o.CargaPedido.Carga.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (numeroNotaFiscal > 0)
                query = query.Where(o => o.XMLNotaFiscal.Numero == numeroNotaFiscal);

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.CargaPedido.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (codigoCarga != 0)
                query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao.Value);

            if (dataInicio != DateTime.MinValue)
                query = query.Where(obj => obj.DataDevolucao >= dataInicio);

            if (dataFim != DateTime.MinValue)
                query = query.Where(obj => obj.DataDevolucao <= dataFim);

            if (naoExibirDevolucaoPaletesSemNotaFiscal.HasValue && naoExibirDevolucaoPaletesSemNotaFiscal.Value)
                query = query.Where(obj => obj.XMLNotaFiscal != null);

            string ordenacao = string.Empty;

            if (!string.IsNullOrWhiteSpace(propAgrupa))
                ordenacao = propAgrupa + " " + dirAgrupa + ", ";

            if (propOrdena == "DescricaoSituacao")
                ordenacao += $"Situacao {dirOrdena}";
            else
                ordenacao += $"{propOrdena} {dirOrdena}";

            var retorno = query.Select(o => new Dominio.Relatorios.Embarcador.DataSource.Pallets.Devolucao()
            {
                Codigo = o.Codigo,
                DataCarregamentoCarga = o.CargaPedido.Carga.DataCarregamentoCarga ?? DateTime.MinValue,
                DataDevolucao = o.DataDevolucao ?? DateTime.MinValue,
                DataEmissaoNotaFiscal = o.XMLNotaFiscal == null ? DateTime.MinValue : o.XMLNotaFiscal.DataEmissao,
                Cliente = o.Cliente == null ? "" : o.Cliente.Nome,
                ClienteCpfCnpj = o.Cliente == null ? "" : o.Cliente.CPF_CNPJ.ToString().ObterCpfOuCnpjFormatado(o.Cliente.Tipo),
                ClienteCodigoIntegracao = o.Cliente == null ? "" : o.Cliente.CodigoIntegracao,
                Filial = o.Filial.Descricao,
                FilialCnpj = o.Filial.CNPJ.ObterCnpjFormatado(),
                FilialCodigoIntegracao = o.Filial.CodigoFilialEmbarcador,
                Motorista = o.CargaPedido.Carga.NomeMotoristas,
                NumeroCarga = o.CargaPedido.Carga.CodigoCargaEmbarcador,
                NumeroNotaFiscal = o.XMLNotaFiscal == null ? 0 : o.XMLNotaFiscal.Numero,
                NumeroPallets = o.QuantidadePallets,
                NumeroPalletsEntregues = o.Situacoes.Where(s => s.AcresceSaldo).Sum(s => (int?)s.Quantidade) ?? 0,
                ValorTotalCobrado = o.Situacoes.Where(s => s.ValorTotal > 0).Sum(s => (int?)s.ValorTotal) ?? 0,
                SituacaoPallet1 = o.Situacoes.Where(s => s.Situacao.Codigo == (situacoesDevolucao.Count > 0 ? (situacoesDevolucao[situacoesDevolucao.Count > 0 ? 0 : 0].Codigo) : 0)).Sum(s => (int?)s.Quantidade) ?? 0,
                SituacaoPallet2 = o.Situacoes.Where(s => s.Situacao.Codigo == (situacoesDevolucao.Count > 1 ? (situacoesDevolucao[situacoesDevolucao.Count > 1 ? 1 : 0].Codigo) : 0)).Sum(s => (int?)s.Quantidade) ?? 0,
                SituacaoPallet3 = o.Situacoes.Where(s => s.Situacao.Codigo == (situacoesDevolucao.Count > 2 ? (situacoesDevolucao[situacoesDevolucao.Count > 2 ? 2 : 0].Codigo) : 0)).Sum(s => (int?)s.Quantidade) ?? 0,
                SituacaoPallet4 = o.Situacoes.Where(s => s.Situacao.Codigo == (situacoesDevolucao.Count > 3 ? (situacoesDevolucao[situacoesDevolucao.Count > 3 ? 3 : 0].Codigo) : 0)).Sum(s => (int?)s.Quantidade) ?? 0,
                SituacaoPallet5 = o.Situacoes.Where(s => s.Situacao.Codigo == (situacoesDevolucao.Count > 4 ? (situacoesDevolucao[situacoesDevolucao.Count > 4 ? 4 : 0].Codigo) : 0)).Sum(s => (int?)s.Quantidade) ?? 0,
                SituacaoPallet6 = o.Situacoes.Where(s => s.Situacao.Codigo == (situacoesDevolucao.Count > 5 ? (situacoesDevolucao[situacoesDevolucao.Count > 5 ? 5 : 0].Codigo) : 0)).Sum(s => (int?)s.Quantidade) ?? 0,
                SituacaoPallet7 = o.Situacoes.Where(s => s.Situacao.Codigo == (situacoesDevolucao.Count > 6 ? (situacoesDevolucao[situacoesDevolucao.Count > 6 ? 6 : 0].Codigo) : 0)).Sum(s => (int?)s.Quantidade) ?? 0,
                SituacaoPallet8 = o.Situacoes.Where(s => s.Situacao.Codigo == (situacoesDevolucao.Count > 6 ? (situacoesDevolucao[situacoesDevolucao.Count > 7 ? 7 : 0].Codigo) : 0)).Sum(s => (int?)s.Quantidade) ?? 0,
                SituacaoPallet9 = o.Situacoes.Where(s => s.Situacao.Codigo == (situacoesDevolucao.Count > 8 ? (situacoesDevolucao[situacoesDevolucao.Count > 8 ? 8 : 0].Codigo) : 0)).Sum(s => (int?)s.Quantidade) ?? 0,
                SituacaoPallet10 = o.Situacoes.Where(s => s.Situacao.Codigo == (situacoesDevolucao.Count > 9 ? (situacoesDevolucao[situacoesDevolucao.Count > 9 ? 9 : 0].Codigo) : 0)).Sum(s => (int?)s.Quantidade) ?? 0,
                Situacao = o.Situacao,
                Veiculo = o.CargaPedido.Carga.PlacasVeiculos,
                Transportador = o.Transportador.RazaoSocial,
                TransportadorCnpj = o.Transportador.CNPJ.ObterCnpjFormatado(),
                TransportadorCodigoIntegracao = o.Transportador.CodigoIntegracao ?? "",
                Destino = o.CargaPedido.Carga.DadosSumarizados.Destinos
            });

            retorno = retorno.OrderBy(ordenacao);

            if (inicio > 0 || limite > 0)
                retorno = retorno.Skip(inicio).Take(limite);

            return retorno.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaDevolucaoPallet filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();

            query = query.Where(o => o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Cancelado);

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
                query = query.Where(o => o.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNotaFiscal);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                query = query.Where(o => o.CargaPedido.Carga.CodigoCargaEmbarcador.Equals(filtrosPesquisa.NumeroCarga));

            if (filtrosPesquisa.NumeroDevolucao > 0)
                query = query.Where(o => o.NumeroDevolucao == filtrosPesquisa.NumeroDevolucao);

            if (filtrosPesquisa.CodigoFilial > 0)
                query = query.Where(o => (o.CargaPedido != null && o.CargaPedido.Carga.Filial.Codigo == filtrosPesquisa.CodigoFilial) || o.XMLNotaFiscal.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoMotorista > 0)
                query = query.Where(o => o.CargaPedido.Carga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista));

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query = query.Where(o => o.CargaPedido.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.CargaPedido.Carga.VeiculosVinculados.Any(v => v.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                query = query.Where(o => filtrosPesquisa.CodigosTransportador.Contains(o.Transportador.Codigo));

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.XMLNotaFiscal.DataEmissao >= filtrosPesquisa.DataEmissaoInicial.Date);

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.XMLNotaFiscal.DataEmissao < filtrosPesquisa.DataEmissaoFinal.AddDays(1).Date);

            if (filtrosPesquisa.DataBaixaInicial != DateTime.MinValue)
                query = query.Where(o => o.DataDevolucao >= filtrosPesquisa.DataBaixaInicial.Date);

            if (filtrosPesquisa.DataBaixaFinal != DateTime.MinValue)
                query = query.Where(o => o.DataDevolucao < filtrosPesquisa.DataBaixaFinal.AddDays(1).Date);

            if (filtrosPesquisa.CpfCnpjRemetente > 0)
                query = query.Where(o => o.CargaPedido.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente);

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                query = query.Where(o => o.CargaPedido.Carga.GrupoPessoaPrincipal.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                query = query.Where(o => o.CargaPedido.Carga.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoTomador > 0)
                query = query.Where(o => (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && (o.CargaPedido.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CodigoTomador)) ||
                                         (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && (o.CargaPedido.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CodigoTomador)) ||
                                         (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && (o.CargaPedido.Pedido.Recebedor.CPF_CNPJ == filtrosPesquisa.CodigoTomador)) ||
                                         (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && (o.CargaPedido.Pedido.Expedidor.CPF_CNPJ == filtrosPesquisa.CodigoTomador)));

            return query.Count();
        }

        public int ContarConsultaDevolucaoSemValePallet(int numeroNotaFiscal, string numeroCarga, string cargaOuNotaFiscal, int codigoTransportador, DateTime? dataEmissaoInicial, DateTime? dataEmissaoFinal)
        {
            var listaDevolucao = ConsultarDevolucaoSemValePallet(numeroNotaFiscal, numeroCarga, cargaOuNotaFiscal, codigoTransportador, dataEmissaoInicial, dataEmissaoFinal);

            return listaDevolucao.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> BuscarPalletPendentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();
            query = query.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega);
            return query.ToList();
        }
        #endregion
    }
}
