using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.RH
{
    public class ComissaoFuncionarioMotorista : RepositorioBase<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista>
    {
        public ComissaoFuncionarioMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista BuscarPorComissaoEMotorista(int codigoMotorista, int codigoComissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista>();
            var result = from obj in query where obj.ComissaoFuncionario.Codigo == codigoComissao && obj.Motorista.Codigo == codigoMotorista select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotoristaComissao> BuscarPorComissaoParaEBS(int codigoComissao, int codigoEvento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista>();

            query = query.Where(obj => obj.ComissaoFuncionario.Codigo == codigoComissao && obj.GerarComissao);

            return query.Select(o => new Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotoristaComissao()
            {
                Codigo = o.Motorista.CodigoIntegracao,
                Evento = codigoEvento,
                Valor = o.ValorComissao > 0m ? o.ValorComissao : 0.01m
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotoristaComissao> BuscarDiariasPorComissaoParaEBS(int codigoComissao, int codigoEvento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista>();

            query = query.Where(obj => obj.ComissaoFuncionario.Codigo == codigoComissao && obj.GerarComissao);

            return query.Select(o => new Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotoristaComissao()
            {
                Codigo = o.Motorista.CodigoIntegracao,
                Evento = codigoEvento,
                Valor = o.ComissaoFuncionario.ValorDiaria * o.NumeroDiasEmViagem
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotoristaComissao> BuscarMediasPorComissaoParaEBS(int codigoComissao, int codigoEvento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista>();

            query = query.Where(obj => obj.ComissaoFuncionario.Codigo == codigoComissao && obj.GerarComissao);

            return query.Select(o => new Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotoristaComissao()
            {
                Codigo = o.Motorista.CodigoIntegracao,
                Evento = codigoEvento,
                Valor = o.AtingiuMedia ? ((o.ValoFreteLiquido * (o.PercentualAtingirMedia ?? 1.5m)) / 100) : 0
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotoristaComissao> BuscarProdutividadePorComissaoParaEBS(int codigoComissao, int codigoEvento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista>();

            query = query.Where(obj => obj.ComissaoFuncionario.Codigo == codigoComissao && obj.GerarComissao);

            return query.Select(o => new Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotoristaComissao()
            {
                Codigo = o.Motorista.CodigoIntegracao,
                Evento = codigoEvento,
                Valor = o.TabelaProdutividadeValores != null ? o.TabelaProdutividadeValores.Valor : 0m
            }).ToList();
        }

        public List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista> Consultar(bool motoristaComDoisModelos, int cargoMotorista, int codigoMotorista, int codigoComissao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista>();
            var result = from obj in query where obj.ComissaoFuncionario.Codigo == codigoComissao select obj;

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);
            if (cargoMotorista > 0)
                result = result.Where(obj => obj.Motorista.CargoMotorista.Codigo == cargoMotorista);
            if (motoristaComDoisModelos)
                result = result.Where(obj => obj.PossuiDuasFrotas == true);

            return ObterLista(result, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(bool motoristaComDoisModelos, int cargoMotorista, int codigoMotorista, int codigoComissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista>();

            var result = from obj in query where obj.ComissaoFuncionario.Codigo == codigoComissao select obj;

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);
            if (cargoMotorista > 0)
                result = result.Where(obj => obj.Motorista.CargoMotorista.Codigo == cargoMotorista);
            if (motoristaComDoisModelos)
                result = result.Where(obj => obj.PossuiDuasFrotas == true);

            return result.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.RH.ExportacaoComissaoMotorista> ConsultarParaExportacao(int codigoComissao)
        {
            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery($@"SELECT 
                                                    Motorista.FUN_NOME NomeMotorista, 
                                                    Motorista.FUN_CPF CPFMotorista, 
                                                    Motorista.FUN_CODIGO_INTEGRACAO CodigoIntegracao, 
                                                    ComissaoFuncionarioMotorista.CFM_VALOR_FRETE_LIQUIDO ValorFreteLiquido,
                                                    (SELECT TOP(1) ModeloVeicularCarga.MVC_DESCRICAO FROM T_VEICULO Veiculo INNER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicularCarga ON ModeloVeicularCarga.MVC_CODIGO = Veiculo.MVC_CODIGO WHERE Veiculo.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO and Veiculo.VEI_TIPOVEICULO = '1') ModeloVeicularCarga,
                                                    Motorista.FUN_CODIGO_INTEGRACAO_CONTABILIDADE CodigoContabil,
													ComissaoFuncionarioMotorista.CFM_VALOR_COMISSAO ValorComissao,
													ComissaoFuncionarioMotorista.CFM_VALOR_BONIFICACAO ValorBonificacao
                                                    FROM T_COMISSAO_FUNCIONARIO_MOTORISTA ComissaoFuncionarioMotorista
                                                    INNER JOIN T_FUNCIONARIO Motorista on ComissaoFuncionarioMotorista.FUN_CODIGO_Motorista = Motorista.FUN_CODIGO
                                                    WHERE 
                                                    ComissaoFuncionarioMotorista.CMF_CODIGO = {codigoComissao}");

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.RH.ExportacaoComissaoMotorista)));

            return query.SetTimeout(300).List<Dominio.ObjetosDeValor.Embarcador.RH.ExportacaoComissaoMotorista>();
        }

        #endregion
    }
}
