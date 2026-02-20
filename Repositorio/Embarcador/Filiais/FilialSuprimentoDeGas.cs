using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Repositorio.Embarcador.Filiais
{
    public class FilialSuprimentoDeGas : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas>
    {
        public FilialSuprimentoDeGas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void DeletarRegistrosExcluidos(List<int> codigosDeletar)
        {
            if (codigosDeletar.Count == 0)
                return;

            var sql = "DELETE FROM T_FILIAL_SUPRIMENTO_DE_GAS WHERE SDG_CODIGO IN (:codigosDeletar)";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameterList("codigosDeletar", codigosDeletar);
            query.ExecuteUpdate();

            sql = $"DELETE FROM T_SUPRIMENTO_DE_GAS WHERE SDG_CODIGO IN (:codigosDeletar)";
            
            query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameterList("codigosDeletar", codigosDeletar);
            query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> BuscarPorFiliaisProdutos(List<(double CpfCnpjBase, int CodigoProduto)> listaBasesProdutos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas>();

            var queryAuxiliar = query;

            List<(int Codigo, double CodigoBase, int CodigoProduto)> listaPermitidos = queryAuxiliar.Select(obj => ValueTuple.Create(
                   obj.Codigo,
                   obj.Cliente != null ? obj.Cliente.CPF_CNPJ : 0,
                   obj.SuprimentoDeGas.ProdutoPadrao != null ? obj.SuprimentoDeGas.ProdutoPadrao.Codigo : 0
               )).ToList();

            List<int> codigosPermitidos = listaPermitidos.Where(obj => listaBasesProdutos.Any(x => obj.CodigoBase == x.CpfCnpjBase && obj.CodigoProduto == x.CodigoProduto)).Select(obj => obj.Codigo).ToList();
            
            query = query.Where(obj => codigosPermitidos.Contains(obj.Codigo));

            return query
                .Fetch(obj => obj.SuprimentoDeGas)
                .ThenFetch(obj => obj.SupridorPadrao)
                .Fetch(obj => obj.SuprimentoDeGas)
                .ThenFetch(obj => obj.ProdutoPadrao)
                .ToList();
        }


        public List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> BuscarPorHoraLimiteSolicitacaoEsgotada()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas>();

            query = query.Where(obj => obj.Cliente.HabilitarSolicitacaoSuprimentoDeGas == true &&
                                (obj.SuprimentoDeGas.DataUltimaNotificacaoLimite.Value.Date < DateTime.Now.Date || obj.SuprimentoDeGas.DataUltimaNotificacaoLimite.HasValue == false) &&
                                obj.SuprimentoDeGas.HoraLimiteSolicitacao.HasValue);

            var querySuprimentoDeGas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();
            querySuprimentoDeGas = querySuprimentoDeGas.Where(obj => obj.DataMedicao.Date == DateTime.Now.Date);

            List<(double CpfCnpjCliente, int CodigoProduto)> codigosDesconsiderar = querySuprimentoDeGas
                                                                                      .Select(obj => ValueTuple.Create(
                                                                                            obj.ClienteBase.CPF_CNPJ,
                                                                                            obj.Produto.Codigo
                                                                                          )).ToList();

            var queryAuxiliar = query;

            List<(int Codigo, double CpfCnpjCliente, int CodigoProduto)> registrosPossiveis = queryAuxiliar.Select(obj => ValueTuple.Create(
                                                                                                obj.Codigo,
                                                                                                obj.Cliente.CPF_CNPJ,
                                                                                                obj.SuprimentoDeGas.ProdutoPadrao.Codigo
                                                                                              )).ToList();

            List<int> codigos = registrosPossiveis.Where(obj => !codigosDesconsiderar.Any(x => obj.CpfCnpjCliente == x.CpfCnpjCliente && obj.CodigoProduto == x.CodigoProduto)).Select(obj => obj.Codigo).ToList();

            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.SuprimentoDeGas)
                .ToList()
                .Where(obj => obj.SuprimentoDeGas.HoraLimiteSolicitacao.Value <= DateTime.Now.TimeOfDay)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> BuscarPorHoraLimiteSolicitacaoGerenteEsgotada()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas>();

            query = query.Where(obj => obj.Cliente.HabilitarSolicitacaoSuprimentoDeGas == true &&
                                (obj.SuprimentoDeGas.DataUltimaNotificacaoGerente.Value.Date < DateTime.Now.Date || obj.SuprimentoDeGas.DataUltimaNotificacaoGerente.HasValue == false) &&
                                obj.SuprimentoDeGas.HoraLimiteGerente.HasValue);

            var querySuprimentoDeGas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();
            querySuprimentoDeGas = querySuprimentoDeGas.Where(obj => obj.DataMedicao.Date == DateTime.Now.Date);

            List<(double CpfCnpjCliente, int CodigoProduto)> codigosDesconsiderar = querySuprimentoDeGas
                                                                                      .Select(obj => ValueTuple.Create(
                                                                                            obj.ClienteBase.CPF_CNPJ,
                                                                                            obj.Produto.Codigo
                                                                                          )).ToList();

            var queryAuxiliar = query;

            List<(int Codigo, double CpfCnpjCliente, int CodigoProduto)> registrosPossiveis = queryAuxiliar.Select(obj => ValueTuple.Create(
                                                                                                obj.Codigo,
                                                                                                obj.Cliente.CPF_CNPJ,
                                                                                                obj.SuprimentoDeGas.ProdutoPadrao.Codigo
                                                                                              )).ToList();

            List<int> codigos = registrosPossiveis.Where(obj => !codigosDesconsiderar.Any(x => obj.CpfCnpjCliente == x.CpfCnpjCliente && obj.CodigoProduto == x.CodigoProduto)).Select(obj => obj.Codigo).ToList();

            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.SuprimentoDeGas)
                .ToList()
                .Where(obj => obj.SuprimentoDeGas.HoraLimiteGerente.Value <= DateTime.Now.TimeOfDay)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> BuscarPorHoraLimiteSolicitacaoBloqueioEsgotada()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas>();

            query = query.Where(obj => obj.Cliente.HabilitarSolicitacaoSuprimentoDeGas == true &&
                                (obj.SuprimentoDeGas.DataUltimaNotificacaoBloqueio.Value.Date < DateTime.Now.Date || obj.SuprimentoDeGas.DataUltimaNotificacaoBloqueio.HasValue == false) &&
                                obj.SuprimentoDeGas.HoraBloqueioSolicitacao.HasValue);

            var querySuprimentoDeGas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();
            querySuprimentoDeGas = querySuprimentoDeGas.Where(obj => obj.DataMedicao.Date == DateTime.Now.Date);

            List<(double CpfCnpjCliente, int CodigoProduto)> codigosDesconsiderar = querySuprimentoDeGas
                                                                                      .Select(obj => ValueTuple.Create(
                                                                                            obj.ClienteBase.CPF_CNPJ,
                                                                                            obj.Produto.Codigo
                                                                                          )).ToList();

            var queryAuxiliar = query;

            List<(int Codigo, double CpfCnpjCliente, int CodigoProduto)> registrosPossiveis = queryAuxiliar.Select(obj => ValueTuple.Create(
                                                                                                obj.Codigo,
                                                                                                obj.Cliente.CPF_CNPJ,
                                                                                                obj.SuprimentoDeGas.ProdutoPadrao.Codigo
                                                                                              )).ToList();

            List<int> codigos = registrosPossiveis.Where(obj => !codigosDesconsiderar.Any(x => obj.CpfCnpjCliente == x.CpfCnpjCliente && obj.CodigoProduto == x.CodigoProduto)).Select(obj => obj.Codigo).ToList();

            query = query.Where(obj => codigos.Contains(obj.Codigo));
            
            return query
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.SuprimentoDeGas)
                .ToList()
                .Where(obj => obj.SuprimentoDeGas.HoraBloqueioSolicitacao.Value <= DateTime.Now.TimeOfDay)
                .ToList();
        }
    }
}

