using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio
{
    public class CFOP : RepositorioBase<Dominio.Entidades.CFOP>, Dominio.Interfaces.Repositorios.CFOP
    {
        public CFOP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.CFOP> BuscarTodos(Dominio.Enumeradores.TipoCFOP tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = from obj in query where obj.Status.Equals("A") && obj.Tipo == tipo select obj;
            return result.OrderBy(o => o.CodigoCFOP).ToList();
        }

        public Dominio.Entidades.CFOP BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.CFOP> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.CFOP BuscarPorCFOP(int cfop, Dominio.Enumeradores.TipoCFOP tipo, List<Dominio.Entidades.CFOP> lstCFOP = null)
        {
            if (lstCFOP != null)
                return lstCFOP.Where(obj => obj.CodigoCFOP == cfop && obj.Status.Equals("A") && obj.Tipo == tipo).FirstOrDefault();
            else
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
                var result = query.Where(obj => obj.CodigoCFOP == cfop && obj.Status.Equals("A") && obj.Tipo == tipo);
                return result.FirstOrDefault();
            }
        }

        public Dominio.Entidades.CFOP BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoCFOP tipo)
        {
            IQueryable<Dominio.Entidades.CFOP> query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();

            query = query.Where(obj => obj.Status == "A" && obj.Tipo == tipo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.CFOP> BuscarPorCFOPPorTipo(Dominio.Enumeradores.TipoCFOP tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = query.Where(obj => obj.Status.Equals("A") && obj.Tipo == tipo);
            return result.ToList();
        }

        public Dominio.Entidades.CFOP BuscarPorCFOPEmpresa(int cfop, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = query.Where(obj => obj.CodigoCFOP == cfop && obj.Status.Equals("A") && obj.Empresa.Codigo == empresa);//(obj.Empresa == null || obj.Empresa.Codigo == empresa));
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.CFOP> BuscarCFOPEmpresa(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = query.Where(obj => obj.Empresa.Codigo == empresa && obj.Status.Equals("A"));
            return result.ToList();
        }

        public List<Dominio.Entidades.CFOP> BuscarPorNaturezaDaOperacao(int idNaturezaOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = from obj in query where obj.NaturezaDaOperacao.Codigo == idNaturezaOperacao && obj.Status.Equals("A") orderby obj.CodigoCFOP select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.CFOP> BuscarPorNaturezaDaOperacao(int idNaturezaOperacao, Dominio.Enumeradores.TipoCFOP tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();

            var result = from obj in query where obj.NaturezaDaOperacao.Codigo == idNaturezaOperacao && obj.Status.Equals("A") && obj.Tipo == tipo orderby obj.CodigoCFOP select obj;

            return result.ToList();
        }

        public Dominio.Entidades.CFOP BuscarPorId(int idCFOP)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = from obj in query where obj.Codigo == idCFOP select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.CFOP> Consultar(int cfop, string naturezaDaOperacao, Dominio.Enumeradores.TipoCFOP? tipo, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();

            var result = from obj in query select obj;

            if (cfop > 0)
                result = result.Where(o => o.CodigoCFOP == cfop);

            if (tipo != null)
                result = result.Where(o => o.Tipo == tipo.Value);

            if (!string.IsNullOrWhiteSpace(naturezaDaOperacao))
                result = result.Where(o => o.NaturezaDaOperacao.Descricao.Contains(naturezaDaOperacao));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int cfop, string naturezaDaOperacao, Dominio.Enumeradores.TipoCFOP? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();

            var result = from obj in query select obj;

            if (cfop > 0)
                result = result.Where(o => o.CodigoCFOP == cfop);

            if (tipo != null)
                result = result.Where(o => o.Tipo == tipo.Value);

            if (!string.IsNullOrWhiteSpace(naturezaDaOperacao))
                result = result.Where(o => o.NaturezaDaOperacao.Descricao.Contains(naturezaDaOperacao));

            return result.Count();
        }

        public List<Dominio.Entidades.CFOP> Consultar(int cfop, int naturezaDaOperacao, Dominio.Enumeradores.TipoCFOP? tipoCFOP, bool? ativo, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();

            var result = from obj in query select obj;

            if (cfop > 0)
                result = result.Where(o => o.CodigoCFOP == cfop);

            if (naturezaDaOperacao > 0)
                result = result.Where(o => o.NaturezaDaOperacao.Codigo == naturezaDaOperacao);

            if (tipoCFOP != null)
                result = result.Where(o => o.Tipo == tipoCFOP.Value);

            if (ativo.HasValue)
                result = result.Where(o => o.Status == (ativo.Value ? "A" : "I"));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int cfop, int naturezaDaOperacao, Dominio.Enumeradores.TipoCFOP? tipoCFOP, bool? ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();

            var result = from obj in query select obj;

            if (cfop > 0)
                result = result.Where(o => o.CodigoCFOP == cfop);

            if (naturezaDaOperacao > 0)
                result = result.Where(o => o.NaturezaDaOperacao.Codigo == naturezaDaOperacao);

            if (tipoCFOP != null)
                result = result.Where(o => o.Tipo == tipoCFOP.Value);

            if (ativo.HasValue)
                result = result.Where(o => o.Status == (ativo.Value ? "A" : "I"));

            return result.Count();
        }

        public List<Dominio.Entidades.CFOP> Consultar(int numeroCFOP, string descricaoCFOP, Dominio.Enumeradores.TipoCFOP? tipoCFOP, int empresa, int codigoNaturezaOperacao, string dentroEstado, string status, string extensao, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();

            if (numeroCFOP > 0)
                result = result.Where(o => o.CodigoCFOP.Equals(numeroCFOP));

            if (empresa > 0)
                result = result.Where(o => o.Empresa.Codigo.Equals(empresa));

            if (!string.IsNullOrWhiteSpace(extensao))
                result = result.Where(o => o.Extensao.Equals(extensao));

            if (!string.IsNullOrWhiteSpace(descricaoCFOP))
                result = result.Where(o => o.Descricao.Contains(descricaoCFOP));

            if (tipoCFOP != null)
                result = result.Where(o => o.Tipo == tipoCFOP.Value);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(dentroEstado))
            {
                if (dentroEstado == "S")
                {
                    string[] incidenciaDentroEstado = new string[] { "1", "3", "5", "7" };
                    result = result.Where(o => incidenciaDentroEstado.Contains(o.CodigoCFOP.ToString().Substring(0, 1)));
                }
                else if (dentroEstado == "N")
                {
                    string[] incidenciaForaEstado = new string[] { "2", "3", "6", "7" };
                    result = result.Where(o => incidenciaForaEstado.Contains(o.CodigoCFOP.ToString().Substring(0, 1)));
                }
            }

            if (codigoNaturezaOperacao > 0)
                result = result.Where(o => o.NaturezasOperacoes.Any(n => n.Codigo == codigoNaturezaOperacao));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int numeroCFOP, string descricaoCFOP, Dominio.Enumeradores.TipoCFOP? tipoCFOP, int empresa, int codigoNaturezaOperacao, string dentroEstado, string status, string extensao)
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();

            if (numeroCFOP > 0)
                result = result.Where(o => o.CodigoCFOP.Equals(numeroCFOP));

            if (!string.IsNullOrWhiteSpace(extensao))
                result = result.Where(o => o.Extensao.Equals(extensao));

            if (empresa > 0)
                result = result.Where(o => o.Empresa.Codigo.Equals(empresa));

            if (!string.IsNullOrWhiteSpace(descricaoCFOP))
                result = result.Where(o => o.Descricao.Contains(descricaoCFOP));

            if (tipoCFOP != null)
                result = result.Where(o => o.Tipo == tipoCFOP.Value);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(dentroEstado))
            {
                if (dentroEstado == "S")
                {
                    string[] incidenciaDentroEstado = new string[] { "1", "3", "5", "7" };
                    result = result.Where(o => incidenciaDentroEstado.Contains(o.CodigoCFOP.ToString().Substring(0, 1)));
                }
                else if (dentroEstado == "N")
                {
                    string[] incidenciaForaEstado = new string[] { "2", "3", "6", "7" };
                    result = result.Where(o => incidenciaForaEstado.Contains(o.CodigoCFOP.ToString().Substring(0, 1)));
                }
            }

            if (codigoNaturezaOperacao > 0)
                result = result.Where(o => o.NaturezasOperacoes.Any(n => n.Codigo == codigoNaturezaOperacao));

            return result.Count();
        }

        public Dominio.Entidades.CFOP BuscarPorNumero(int cfop, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = from obj in query where obj.CodigoCFOP == cfop select obj;
            if (codigoEmpresa > 0)
                result = result.Where(c => c.Empresa.Codigo == codigoEmpresa && c.Status == "A");
            return result.Fetch(o => o.NaturezaDaOperacao).FirstOrDefault();
        }
        
        public Task<Dominio.Entidades.CFOP> BuscarPorNumeroAsync(int cfop, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = from obj in query where obj.CodigoCFOP == cfop select obj;
            if (codigoEmpresa > 0)
                result = result.Where(c => c.Empresa.Codigo == codigoEmpresa && c.Status == "A");
            return result.Fetch(o => o.NaturezaDaOperacao).FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.CFOP> BuscarPorNumeros(List<int> codigosCfop)
        {
            var consultaCFOP = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>()
                .Where(cfop => codigosCfop.Contains(cfop.CodigoCFOP));

            return consultaCFOP.ToList();
        }

        public bool ExisteDuplicado(int numero, string descricao, int codigo,int prioridade, out string msg)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();

            if (query.Any(o => o.Descricao.Equals(descricao) && o.Codigo != codigo && o.CodigoCFOP == numero && o.GrupoPrioridade == prioridade ))
                {
                msg = "Já existe um CFOP com a mesma Descrição";
                return true;
            }

            msg = string.Empty;
            return false;
        }

        #endregion

        #region Relatório

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.CFOP> RelatorioCFOP(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCFOP filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string query = @"SELECT CF.CFO_CFOP NumeroCFOP,
                CF.CFO_EXTENSAO Extensao,
                CF.CFO_DESCRICAO Descricao,
                CF.CFO_TIPO Tipo,
                CF.CFO_GERA_ESTOQUE GeraEstoque,
                CF.CFO_CST_CSOSN CSTICMS,
                CF.CFO_ALIQUOTA_ICMS_INTERNA AliquotaInterna,
                CF.CFO_ALIQUOTA_ICMS_INTERESTADUAL AliquotaInterestadual,
                CF.CFO_ALIQUOTA_DIFERENCIAL AliquotaDiferencial,
                CF.CFO_BLOQUEIO_DOCUMENTO_ENTRADA BloqueioDocumentoEntrada,
                CF.CFO_ALIQUOTA_PIS AliquotaRetencaoPIS,
                CF.CFO_ALIQUOTA_RETENCAO_COFINS AliquotaRetencaoCOFINS,
                CF.CFO_ALIQUOTA_RETENCAO_INSS AliquotaRetencaoINSS,
                CF.CFO_ALIQUOTA_IPI AliquotaRetencaoIPI,
                CF.CFO_ALIQUOTA_RETENCAO_CSLL AliquotaRetencaoCSLL,
                CF.CFO_ALIQUOTA_RETENCAO_OUTRAS AliquotaOutrasRetencoes,
                CF.CFO_ALIQUOTA_RETENCAO_IR AliquotaRetencaoIR,
                CF.CFO_ALIQUOTA_RETENCAO_ISS AliquotaRetençãoISS,
                CF.CFO_REALIZAR_RATEIO_DESPESA_VEICULO RealizarRateioDespesaVeiculo,

                USO.TIM_DESCRICAO USO,
                REVERSAO.TIM_DESCRICAO REVERSAO,
                USO_DESCONTO.TIM_DESCRICAO USODESCONTO,
                REVERSAO_DESCONTO.TIM_DESCRICAO REVERSAODESCONTO,
                USO_OUTRAS_DESPESAS.TIM_DESCRICAO USOOUTRASDESPESAS,
                REVERSAO_OUTRAS_DESPESAS.TIM_DESCRICAO REVERSAOOUTRASDESPESAS,
                USO_FRETE.TIM_DESCRICAO USOFRETE,
                REVERSAO_FRETE.TIM_DESCRICAO REVERSAOFRETE,
                USO_ICMS.TIM_DESCRICAO USOICMS,
                REVERSAO_ICMS.TIM_DESCRICAO REVERSAOICMS,
                USO_PIS.TIM_DESCRICAO USOPIS,
                REVERSAO_PIS.TIM_DESCRICAO REVERSAOPIS,
                USO_COFINS.TIM_DESCRICAO USOCOFINS,
                REVERSAO_COFINS.TIM_DESCRICAO REVERSAOCOFINS,
                USO_IPI.TIM_DESCRICAO USOIPI,
                REVERSAO_IPI.TIM_DESCRICAO REVERSAOIPI,
                USO_ICMSST.TIM_DESCRICAO USOICMSST,
                REVERSAO_ICMSST.TIM_DESCRICAO REVERSAOICMSST,
                USO_DIFERENCIAL.TIM_DESCRICAO USODIFERENCIAL,
                REVERSAO_DIFERENCIAL.TIM_DESCRICAO REVERSAODIFERENCIAL,
                USO_SEGURO.TIM_DESCRICAO USOSEGURO,
                REVERSAO_SEGURO.TIM_DESCRICAO REVERSAOSEGURO,
                USO_FRETE_FORA.TIM_DESCRICAO USOFRETEFORA,
                REVERSAO_FRETE_FORA.TIM_DESCRICAO REVERSAOFRETEFORA,
                USO_OUTRAS_FORA.TIM_DESCRICAO USOOUTRASFORA,
                REVERSAO_OUTRAS_FORA.TIM_DESCRICAO REVERSAOOUTRASFORA,
                USO_DESCONTO_FORA.TIM_DESCRICAO USODESCONTOFORA,
                REVERSAO_DESCONTO_FORA.TIM_DESCRICAO REVERSAODESCONTOFORA,
                USO_IMPOSTO_FORA.TIM_DESCRICAO USOIMPOSTOFORA,
                REVERSAO_IMPOSTO_FORA.TIM_DESCRICAO REVERSAOIMPOSTOFORA,
                USO_DIFERENCIAL_FRETE_FORA.TIM_DESCRICAO USODIFERENCIALFRETEFORA,
                REVERSAO_DIFERENCIAL_FRETE_FORA.TIM_DESCRICAO REVERSAODIFERENCIALFRETEFORA,
                USO_ICMS_FRETE_FORA.TIM_DESCRICAO USOICMSFRETEFORA,
                REVERSAO_ICMS_FRETE_FORA.TIM_DESCRICAO REVERSAOICMSFRETEFORA,
                USO_CUSTO.TIM_DESCRICAO USOCUSTO,
                REVERSAO_CUSTO.TIM_DESCRICAO REVERSAOCUSTO,
                USO_RETENCAO_PIS.TIM_DESCRICAO USORETENCAOPIS,
                REVERSAO_RETENCAO_PIS.TIM_DESCRICAO REVERSAORETENCAOPIS,
                USO_RETENCAO_COFINS.TIM_DESCRICAO USORETENCAOCOFINS,
                REVERSAO_RETENCAO_COFINS.TIM_DESCRICAO REVERSAORETENCAOCOFINS,
                USO_RETENCAO_INSS.TIM_DESCRICAO USORETENCAOINSS,
                REVERSAO_RETENCAO_INSS.TIM_DESCRICAO REVERSAORETENCAOINSS,
                USO_RETENCAO_IPI.TIM_DESCRICAO USORETENCAOIPI,
                REVERSAO_RETENCAO_IPI.TIM_DESCRICAO REVERSAORETENCAOIPI,
                USO_RETENCAO_CSLL.TIM_DESCRICAO USORETENCAOCSLL,
                REVERSAO_RETENCAO_CSLL.TIM_DESCRICAO REVERSAORETENCAOCSLL,
                USO_RETENCAO_OUTRAS.TIM_DESCRICAO USORETENCAOOUTRAS,
                REVERSAO_RETENCAO_OUTRAS.TIM_DESCRICAO REVERSAORETENCAOOUTRAS,
                USO_TITULO_RETENCAO_PIS.TIM_DESCRICAO USOTITULORETENCAOPIS,
                REVERSAO_TITULO_RETENCAO_PIS.TIM_DESCRICAO REVERSAOTITULORETENCAOPIS,
                USO_TITULO_RETENCAO_COFINS.TIM_DESCRICAO USOTITULORETENCAOCOFINS,
                REVERSAO_TITULO_RETENCAO_COFINS.TIM_DESCRICAO REVERSAOTITULORETENCAOCOFINS,
                USO_TITULO_RETENCAO_INSS.TIM_DESCRICAO USOTITULORETENCAOINSS,
                REVERSAO_TITULO_RETENCAO_INSS.TIM_DESCRICAO REVERSAOTITULORETENCAOINSS,
                USO_TITULO_RETENCAO_IPI.TIM_DESCRICAO USOTITULORETENCAOIPI,
                REVERSAO_TITULO_RETENCAO_IPI.TIM_DESCRICAO REVERSAOTITULORETENCAOIPI,
                USO_TITULO_RETENCAO_CSLL.TIM_DESCRICAO USOTITULORETENCAOCSLL,
                REVERSAO_TITULO_RETENCAO_CSLL.TIM_DESCRICAO REVERSAOTITULORETENCAOCSLL,
                USO_TITULO_RETENCAO_OUTRAS.TIM_DESCRICAO USOTITULORETENCAOOUTRAS,
                REVERSAO_TITULO_RETENCAO_OUTRAS.TIM_DESCRICAO REVERSAOTITULORETENCAOOUTRAS,
                USO_TITULO_RETENCAO_ISS.TIM_DESCRICAO USOTITULORETENCAOISS,
                REVERSAO_TITULO_RETENCAO_ISS.TIM_DESCRICAO REVERSAOTITULORETENCAOISS,
                USO_TITULO_RETENCAO_IR.TIM_DESCRICAO USOTITULORETENCAOIR,
                REVERSAO_TITULO_RETENCAO_IR.TIM_DESCRICAO REVERSAOTITULORETENCAOIR,
                USO_RETENCAO_ISS.TIM_DESCRICAO USORETENCAOISS,
                REVERSAO_RETENCAO_ISS.TIM_DESCRICAO REVERSAORETENCAOISS,
                USO_RETENCAO_IR.TIM_DESCRICAO USORETENCAOIR,
                REVERSAO_RETENCAO_IR.TIM_DESCRICAO REVERSAORETENCAOIR

                FROM T_CFOP CF
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO ON USO.TIM_CODIGO = CF.TIM_CODIGO_USO
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO ON REVERSAO.TIM_CODIGO = TIM_CODIGO_REVERSAO
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_DESCONTO ON USO_DESCONTO.TIM_CODIGO = TIM_CODIGO_USO_DESCONTO
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_DESCONTO ON REVERSAO_DESCONTO.TIM_CODIGO = TIM_CODIGO_REVERSAO_DESCONTO
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_OUTRAS_DESPESAS ON USO_OUTRAS_DESPESAS.TIM_CODIGO = TIM_CODIGO_USO_OUTRAS_DESPESAS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_OUTRAS_DESPESAS ON REVERSAO_OUTRAS_DESPESAS.TIM_CODIGO = TIM_CODIGO_REVERSAO_OUTRAS_DESPESAS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_FRETE ON USO_FRETE.TIM_CODIGO = TIM_CODIGO_USO_FRETE
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_FRETE ON REVERSAO_FRETE.TIM_CODIGO = TIM_CODIGO_REVERSAO_FRETE
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_ICMS ON USO_ICMS.TIM_CODIGO = TIM_CODIGO_USO_ICMS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_ICMS ON REVERSAO_ICMS.TIM_CODIGO = TIM_CODIGO_REVERSAO_ICMS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_PIS ON USO_PIS.TIM_CODIGO = TIM_CODIGO_USO_PIS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_PIS ON REVERSAO_PIS.TIM_CODIGO = TIM_CODIGO_REVERSAO_PIS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_COFINS ON USO_COFINS.TIM_CODIGO = TIM_CODIGO_USO_COFINS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_COFINS ON REVERSAO_COFINS.TIM_CODIGO = TIM_CODIGO_REVERSAO_COFINS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_IPI ON USO_IPI.TIM_CODIGO = TIM_CODIGO_USO_IPI
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_IPI ON REVERSAO_IPI.TIM_CODIGO = TIM_CODIGO_REVERSAO_IPI
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_ICMSST ON USO_ICMSST.TIM_CODIGO = TIM_CODIGO_USO_ICMSST
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_ICMSST ON REVERSAO_ICMSST.TIM_CODIGO = TIM_CODIGO_REVERSAO_ICMSST
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_DIFERENCIAL ON USO_DIFERENCIAL.TIM_CODIGO = TIM_CODIGO_USO_DIFERENCIAL
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_DIFERENCIAL ON REVERSAO_DIFERENCIAL.TIM_CODIGO = TIM_CODIGO_REVERSAO_DIFERENCIAL
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_SEGURO ON USO_SEGURO.TIM_CODIGO = TIM_CODIGO_USO_SEGURO
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_SEGURO ON REVERSAO_SEGURO.TIM_CODIGO = TIM_CODIGO_REVERSAO_SEGURO
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_FRETE_FORA ON USO_FRETE_FORA.TIM_CODIGO = TIM_CODIGO_USO_FRETE_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_FRETE_FORA ON REVERSAO_FRETE_FORA.TIM_CODIGO = TIM_CODIGO_REVERSAO_FRETE_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_OUTRAS_FORA ON USO_OUTRAS_FORA.TIM_CODIGO = TIM_CODIGO_USO_OUTRAS_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_OUTRAS_FORA ON REVERSAO_OUTRAS_FORA.TIM_CODIGO = TIM_CODIGO_REVERSAO_OUTRAS_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_DESCONTO_FORA ON USO_DESCONTO_FORA.TIM_CODIGO = TIM_CODIGO_USO_DESCONTO_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_DESCONTO_FORA ON REVERSAO_DESCONTO_FORA.TIM_CODIGO = TIM_CODIGO_REVERSAO_DESCONTO_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_IMPOSTO_FORA ON USO_IMPOSTO_FORA.TIM_CODIGO = TIM_CODIGO_USO_IMPOSTO_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_IMPOSTO_FORA ON REVERSAO_IMPOSTO_FORA.TIM_CODIGO = TIM_CODIGO_REVERSAO_IMPOSTO_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_DIFERENCIAL_FRETE_FORA ON USO_DIFERENCIAL_FRETE_FORA.TIM_CODIGO = TIM_CODIGO_USO_DIFERENCIAL_FRETE_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_DIFERENCIAL_FRETE_FORA ON REVERSAO_DIFERENCIAL_FRETE_FORA.TIM_CODIGO = TIM_CODIGO_REVERSAO_DIFERENCIAL_FRETE_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_ICMS_FRETE_FORA ON USO_ICMS_FRETE_FORA.TIM_CODIGO = TIM_CODIGO_USO_ICMS_FRETE_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_ICMS_FRETE_FORA ON REVERSAO_ICMS_FRETE_FORA.TIM_CODIGO = TIM_CODIGO_REVERSAO_ICMS_FRETE_FORA
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_CUSTO ON USO_CUSTO.TIM_CODIGO = TIM_CODIGO_USO_CUSTO
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_CUSTO ON REVERSAO_CUSTO.TIM_CODIGO = TIM_CODIGO_REVERSAO_CUSTO
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_RETENCAO_PIS ON USO_RETENCAO_PIS.TIM_CODIGO = TIM_CODIGO_USO_RETENCAO_PIS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_RETENCAO_PIS ON REVERSAO_RETENCAO_PIS.TIM_CODIGO = TIM_CODIGO_REVERSAO_RETENCAO_PIS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_RETENCAO_COFINS ON USO_RETENCAO_COFINS.TIM_CODIGO = TIM_CODIGO_USO_RETENCAO_COFINS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_RETENCAO_COFINS ON REVERSAO_RETENCAO_COFINS.TIM_CODIGO = TIM_CODIGO_REVERSAO_RETENCAO_COFINS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_RETENCAO_INSS ON USO_RETENCAO_INSS.TIM_CODIGO = TIM_CODIGO_USO_RETENCAO_INSS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_RETENCAO_INSS ON REVERSAO_RETENCAO_INSS.TIM_CODIGO = TIM_CODIGO_REVERSAO_RETENCAO_INSS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_RETENCAO_IPI ON USO_RETENCAO_IPI.TIM_CODIGO = TIM_CODIGO_USO_RETENCAO_IPI
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_RETENCAO_IPI ON REVERSAO_RETENCAO_IPI.TIM_CODIGO = TIM_CODIGO_REVERSAO_RETENCAO_IPI
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_RETENCAO_CSLL ON USO_RETENCAO_CSLL.TIM_CODIGO = TIM_CODIGO_USO_RETENCAO_CSLL
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_RETENCAO_CSLL ON REVERSAO_RETENCAO_CSLL.TIM_CODIGO = TIM_CODIGO_REVERSAO_RETENCAO_CSLL
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_RETENCAO_OUTRAS ON USO_RETENCAO_OUTRAS.TIM_CODIGO = TIM_CODIGO_USO_RETENCAO_OUTRAS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_RETENCAO_OUTRAS ON REVERSAO_RETENCAO_OUTRAS.TIM_CODIGO = TIM_CODIGO_REVERSAO_RETENCAO_OUTRAS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_TITULO_RETENCAO_PIS ON USO_TITULO_RETENCAO_PIS.TIM_CODIGO = TIM_CODIGO_USO_TITULO_RETENCAO_PIS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_TITULO_RETENCAO_PIS ON REVERSAO_TITULO_RETENCAO_PIS.TIM_CODIGO = TIM_CODIGO_REVERSAO_TITULO_RETENCAO_PIS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_TITULO_RETENCAO_COFINS ON USO_TITULO_RETENCAO_COFINS.TIM_CODIGO = TIM_CODIGO_USO_TITULO_RETENCAO_COFINS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_TITULO_RETENCAO_COFINS ON REVERSAO_TITULO_RETENCAO_COFINS.TIM_CODIGO = TIM_CODIGO_REVERSAO_TITULO_RETENCAO_COFINS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_TITULO_RETENCAO_INSS ON USO_TITULO_RETENCAO_INSS.TIM_CODIGO = TIM_CODIGO_USO_TITULO_RETENCAO_INSS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_TITULO_RETENCAO_INSS ON REVERSAO_TITULO_RETENCAO_INSS.TIM_CODIGO = TIM_CODIGO_REVERSAO_TITULO_RETENCAO_INSS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_TITULO_RETENCAO_IPI ON USO_TITULO_RETENCAO_IPI.TIM_CODIGO = TIM_CODIGO_USO_TITULO_RETENCAO_IPI
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_TITULO_RETENCAO_IPI ON REVERSAO_TITULO_RETENCAO_IPI.TIM_CODIGO = TIM_CODIGO_REVERSAO_TITULO_RETENCAO_IPI
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_TITULO_RETENCAO_CSLL ON USO_TITULO_RETENCAO_CSLL.TIM_CODIGO = TIM_CODIGO_USO_TITULO_RETENCAO_CSLL
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_TITULO_RETENCAO_CSLL ON REVERSAO_TITULO_RETENCAO_CSLL.TIM_CODIGO = TIM_CODIGO_REVERSAO_TITULO_RETENCAO_CSLL
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_TITULO_RETENCAO_OUTRAS ON USO_TITULO_RETENCAO_OUTRAS.TIM_CODIGO = TIM_CODIGO_USO_TITULO_RETENCAO_OUTRAS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_TITULO_RETENCAO_OUTRAS ON REVERSAO_TITULO_RETENCAO_OUTRAS.TIM_CODIGO = TIM_CODIGO_REVERSAO_TITULO_RETENCAO_OUTRAS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_TITULO_RETENCAO_ISS ON USO_TITULO_RETENCAO_ISS.TIM_CODIGO = TIM_CODIGO_USO_TITULO_RETENCAO_ISS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_TITULO_RETENCAO_ISS ON REVERSAO_TITULO_RETENCAO_ISS.TIM_CODIGO = TIM_CODIGO_REVERSAO_TITULO_RETENCAO_ISS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_TITULO_RETENCAO_IR ON USO_TITULO_RETENCAO_IR.TIM_CODIGO = TIM_CODIGO_USO_TITULO_RETENCAO_IR
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_TITULO_RETENCAO_IR ON REVERSAO_TITULO_RETENCAO_IR.TIM_CODIGO = TIM_CODIGO_REVERSAO_TITULO_RETENCAO_IR
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_RETENCAO_ISS ON USO_RETENCAO_ISS.TIM_CODIGO = TIM_CODIGO_USO_RETENCAO_ISS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_RETENCAO_ISS ON REVERSAO_RETENCAO_ISS.TIM_CODIGO = TIM_CODIGO_REVERSAO_RETENCAO_ISS
                LEFT OUTER JOIN T_TIPO_MOVIMENTO USO_RETENCAO_IR ON USO_RETENCAO_IR.TIM_CODIGO = TIM_CODIGO_USO_RETENCAO_IR
                LEFT OUTER JOIN T_TIPO_MOVIMENTO REVERSAO_RETENCAO_IR ON REVERSAO_RETENCAO_IR.TIM_CODIGO = TIM_CODIGO_REVERSAO_RETENCAO_IR";

            query += " WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoCFOP > 0)
                query += " AND CF.CFO_CODIGO = " + filtrosPesquisa.CodigoCFOP.ToString();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND CF.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Extensao))
                query += " AND CF.CFO_EXTENSAO = '" + filtrosPesquisa.Extensao + "'";

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                query += " AND CF.CFO_DESCRICAO like '%" + filtrosPesquisa.Descricao + "%'";

            if (!string.IsNullOrEmpty(filtrosPesquisa.Status))
                query += " AND CF.CFO_STATUS = '" + filtrosPesquisa.Status + "'";

            if (filtrosPesquisa.GerarEstoque)
                query += " AND CF.CFO_GERA_ESTOQUE = 1";

            if (filtrosPesquisa.RealizaRateioDespesaVeiculo)
                query += " AND CF.CFO_REALIZAR_RATEIO_DESPESA_VEICULO = 1";

            if (filtrosPesquisa.TipoCFOP.HasValue)
                query += " AND CF.CFO_TIPO = " + filtrosPesquisa.TipoCFOP.Value.ToString("D");

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.CFOP)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.CFOP>();
        }

        public int ContarCFOP(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCFOP filtrosPesquisa)
        {
            string query = @"SELECT COUNT(0) as CONTADOR FROM T_CFOP CF";

            query += " WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoCFOP > 0)
                query += " AND CF.CFO_CODIGO = " + filtrosPesquisa.CodigoCFOP.ToString();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND CF.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Extensao))
                query += " AND CF.CFO_EXTENSAO = '" + filtrosPesquisa.Extensao + "'";

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                query += " AND CF.CFO_DESCRICAO like '%" + filtrosPesquisa.Descricao + "%'";

            if (!string.IsNullOrEmpty(filtrosPesquisa.Status))
                query += " AND CF.CFO_STATUS = '" + filtrosPesquisa.Status + "'";

            if (filtrosPesquisa.GerarEstoque)
                query += " AND CF.CFO_GERA_ESTOQUE = 1";

            if (filtrosPesquisa.RealizaRateioDespesaVeiculo)
                query += " AND CF.CFO_REALIZAR_RATEIO_DESPESA_VEICULO = 1";

            if (filtrosPesquisa.TipoCFOP.HasValue)
                query += " AND CF.CFO_TIPO = " + filtrosPesquisa.TipoCFOP.Value.ToString("D");

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        #endregion
    }
}
