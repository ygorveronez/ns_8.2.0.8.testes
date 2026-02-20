using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class DepositoPosicao : RepositorioBase<Dominio.Entidades.Embarcador.WMS.DepositoPosicao>
    {
        public DepositoPosicao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.DepositoPosicao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoPosicao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.WMS.DepositoPosicao BuscarPorProduto(int codigoProduto, decimal peso, decimal metroCubito, decimal qtdPalet)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoPosicao>();
            var result = from obj in query where obj.Produto.Codigo == codigoProduto select obj;

            result = result.Where(obj => (obj.PesoMaximo >= (obj.PesoAtual + peso) || obj.MetroCubicoMaximo >= (obj.MetroCubicoAtual + metroCubito) || obj.QuantidadePaletes >= (obj.QuantidadePaletesAtual + qtdPalet))
                   || (obj.PesoMaximo == 0 && obj.MetroCubicoMaximo == 0 && obj.QuantidadePaletes == 0));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.WMS.DepositoPosicao BuscarProximaPosicao(decimal peso, decimal metroCubito, decimal qtdPalet)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoPosicao>();
            var result = from obj in query select obj;

            result = result.Where(obj => (obj.PesoMaximo >= (obj.PesoAtual + peso) || obj.MetroCubicoMaximo >= (obj.MetroCubicoAtual + metroCubito) || obj.QuantidadePaletes >= (obj.QuantidadePaletesAtual + qtdPalet))
                   || (obj.PesoMaximo == 0 && obj.MetroCubicoMaximo == 0 && obj.QuantidadePaletes == 0));

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.WMS.DepositoPosicao> BuscarPorBloco(int bloco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoPosicao>();
            var result = from obj in query where obj.Bloco.Codigo == bloco select obj;
            return result.ToList();
        }

        public void AtualizarAbreviacoesCascata(int deposito, int rua, int bloco, int posicao)
        {
            string sql = @"UPDATE 
	                           Posicao
                           SET
	                           Posicao.DPO_ABREVIACAO = 
		                           (Posicao.DPO_DESCRICAO + '.' +
		                           Bloco.DEB_DESCRICAO  + '.' +
		                           Rua.DER_DESCRICAO  + '.' +
		                           Deposito.DEP_DESCRICAO)
		                            
                           FROM 
	                           T_DEPOSITO_POSICAO Posicao

                           JOIN T_DEPOSITO_BLOCO Bloco
	                           ON Posicao.DEB_CODIGO = Bloco.DEB_CODIGO
                           JOIN T_DEPOSITO_RUA Rua
	                           ON Bloco.DER_CODIGO = Rua.DER_CODIGO
                           JOIN T_DEPOSITO Deposito
	                           ON Rua.DEP_CODIGO = Deposito.DEP_CODIGO
                           WHERE ";

            if (deposito > 0)
                sql += "Deposito.DEP_CODIGO = " + deposito.ToString();

            if (rua > 0)
                sql += "Rua.DER_CODIGO = " + rua.ToString();

            if (bloco > 0)
                sql += "Bloco.DEB_CODIGO = " + bloco.ToString();

            if (posicao > 0)
                sql += "Posicao.DPO_CODIGO = " + posicao.ToString();

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.WMS.DepositoPosicao BuscarPorCodigoEAbreviacao(int codigo, string abreviacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoPosicao>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            if (codigo > 0)
                result = result.Where(o => o.Codigo == codigo);

            if (!string.IsNullOrWhiteSpace(abreviacao))
                result = result.Where(o => o.Abreviacao == abreviacao);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.WMS.DepositoPosicao> Consultar(bool comLugarVago, int codigoDepositoPosicao, int codigoDepositoBloco, int codigoDepositoRua, int codigoDeposito, int codigoProdutoEmbarcador, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoPosicao>();

            var result = from obj in query select obj;

            if (codigoDepositoPosicao > 0)
                result = result.Where(obj => obj.Codigo == codigoDepositoPosicao);

            if (codigoDepositoBloco > 0)
                result = result.Where(obj => obj.Bloco.Codigo == codigoDepositoBloco);

            if (codigoDepositoRua > 0)
                result = result.Where(obj => obj.Bloco.Rua.Codigo == codigoDepositoRua);

            if (codigoDeposito > 0)
                result = result.Where(obj => obj.Bloco.Rua.Deposito.Codigo == codigoDeposito);

            if (comLugarVago)
            {
                result = result.Where(obj => (obj.PesoMaximo > obj.PesoAtual || obj.MetroCubicoMaximo > obj.MetroCubicoAtual || obj.QuantidadePaletes > obj.QuantidadePaletesAtual)
                    || (obj.PesoMaximo == 0 && obj.MetroCubicoMaximo == 0 && obj.QuantidadePaletes == 0));
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(bool comLugarVago, int codigoDepositoPosicao, int codigoDepositoBloco, int codigoDepositoRua, int codigoDeposito, int codigoProdutoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoPosicao>();

            var result = from obj in query select obj;

            if (codigoDepositoPosicao > 0)
                result = result.Where(obj => obj.Codigo == codigoDepositoPosicao);

            if (codigoDepositoBloco > 0)
                result = result.Where(obj => obj.Bloco.Codigo == codigoDepositoBloco);

            if (codigoDepositoRua > 0)
                result = result.Where(obj => obj.Bloco.Rua.Codigo == codigoDepositoRua);

            if (codigoDeposito > 0)
                result = result.Where(obj => obj.Bloco.Rua.Deposito.Codigo == codigoDeposito);

            if (comLugarVago)
            {
                result = result.Where(obj => (obj.PesoMaximo > obj.PesoAtual || obj.MetroCubicoMaximo > obj.MetroCubicoAtual || obj.QuantidadePaletes > obj.QuantidadePaletesAtual)
                    || (obj.PesoMaximo == 0 && obj.MetroCubicoMaximo == 0 && obj.QuantidadePaletes == 0));
            }

            return result.Count();
        }

        public int ContarPosicoesPorBloco(int bloco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoPosicao>();
            var result = from obj in query where obj.Bloco.Codigo == bloco select obj;
            return result.Count();
        }
        private IQueryable<Dominio.Entidades.Embarcador.WMS.DepositoPosicao> _ConsultarPosicoes(string apreviacao, string descricao, int deposito, int rua, int bloco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoPosicao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(apreviacao))
                result = result.Where(o => o.Abreviacao.Contains(apreviacao));

            if (deposito > 0)
                result = result.Where(o => o.Bloco.Rua.Deposito.Codigo == deposito);

            if (rua > 0)
                result = result.Where(o => o.Bloco.Rua.Codigo == rua);

            if (bloco > 0)
                result = result.Where(o => o.Bloco.Codigo == bloco);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.WMS.DepositoPosicao> ConsultarPosicoes(string apreviacao, string descricao, int deposito, int rua, int bloco, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarPosicoes(apreviacao, descricao, deposito, rua, bloco);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPosicoes(string apreviacao, string descricao, int deposito, int rua, int bloco)
        {
            var result = _ConsultarPosicoes(apreviacao, descricao, deposito, rua, bloco);

            return result.Count();
        }
    }
}
