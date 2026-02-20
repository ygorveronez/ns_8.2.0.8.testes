using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.FaturamentoMensal
{
    public class FaturamentoMensalCliente : RepositorioBase<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente>
    {
        public FaturamentoMensalCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente BuscarPorCNPJCliente(double cnpjCPF, int codigoGrupoFaturamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente>();
            var result = from obj in query where obj.Pessoa.CPF_CNPJ == cnpjCPF && obj.Ativo == true && obj.FaturamentoMensalGrupo.Codigo == codigoGrupoFaturamento select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente> Consulta(bool selecionarTodos, List<int> codigosFaturamentos, int codigoEmpresa, int codigoFaturamentoMensal, int codigoConfiguracaoBoleto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoGrupoFaturamento, double cnpjPessoa, int diaFatura, int codigoServico, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente>();

            var result = from obj in query select obj;

            if (selecionarTodos && codigosFaturamentos != null)
                result = result.Where(o => !codigosFaturamentos.Contains(o.Codigo));
            else if (codigosFaturamentos != null)
                result = result.Where(o => codigosFaturamentos.Contains(o.Codigo));

            if (codigoFaturamentoMensal < 0 && codigoGrupoFaturamento == 0 && cnpjPessoa == 0 && diaFatura == 0 && codigoServico == 0 && codigoConfiguracaoBoleto == 0)
            {
                result = result.Where(obj => obj.FaturamentoMensalGrupo.Codigo == -1);
            }
            else
            {
                if (codigoGrupoFaturamento > 0)
                    result = result.Where(obj => obj.FaturamentoMensalGrupo.Codigo == codigoGrupoFaturamento);
                if (cnpjPessoa > 0)
                    result = result.Where(obj => obj.Pessoa.CPF_CNPJ == cnpjPessoa);
                if (diaFatura > 0)
                    result = result.Where(obj => obj.DiaFatura == diaFatura);
                if (codigoServico > 0)
                    result = result.Where(obj => obj.Servico.Codigo == codigoServico);
                if (codigoConfiguracaoBoleto > 0)
                    result = result.Where(obj => obj.BoletoConfiguracao.Codigo == codigoConfiguracaoBoleto);
                if (codigoEmpresa > 0)
                    result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            }

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.ToList();
        }

        public int ContaConsulta(int codigoEmpresa, int codigoFaturamentoMensal, int codigoConfiguracaoBoleto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoGrupoFaturamento, double cnpjPessoa, int diaFatura, int codigoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente>();

            var result = from obj in query select obj;

            if (codigoFaturamentoMensal < 0 && codigoGrupoFaturamento == 0 && cnpjPessoa == 0 && diaFatura == 0 && codigoServico == 0 && codigoConfiguracaoBoleto == 0)
            {
                result = result.Where(obj => obj.FaturamentoMensalGrupo.Codigo == -1);
            }
            else
            {
                if (codigoGrupoFaturamento > 0)
                    result = result.Where(obj => obj.FaturamentoMensalGrupo.Codigo == codigoGrupoFaturamento);
                if (cnpjPessoa > 0)
                    result = result.Where(obj => obj.Pessoa.CPF_CNPJ == cnpjPessoa);
                if (diaFatura > 0)
                    result = result.Where(obj => obj.DiaFatura == diaFatura);
                if (codigoServico > 0)
                    result = result.Where(obj => obj.Servico.Codigo == codigoServico);
                if (codigoConfiguracaoBoleto > 0)
                    result = result.Where(obj => obj.BoletoConfiguracao.Codigo == codigoConfiguracaoBoleto);
                if (codigoEmpresa > 0)
                    result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            }

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }
    }
}
