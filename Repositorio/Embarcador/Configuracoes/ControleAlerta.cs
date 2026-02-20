using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ControleAlerta : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta>
    {
        public ControleAlerta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> BuscarControles(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta>();
            var result = from obj in query where obj.Status == true select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> BuscarControlesPorTela(ControleAlertaTela telaAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta>();

            var result = from obj in query where obj.Status && obj.TelasAlerta.Any(a => a == telaAlerta) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> Consulta(int codigoFuncionario, int codigoEmpresa, SituacaoAtivoPesquisa status, List<ControleAlertaForma> formasAlerta, List<ControleAlertaTela> telasAlerta, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> result = Consulta(codigoFuncionario, codigoEmpresa, status, formasAlerta, telasAlerta);

            return ObterLista(result, parametroConsulta);
        }

        public int ContaConsulta(int codigoFuncionario, int codigoEmpresa, SituacaoAtivoPesquisa status, List<ControleAlertaForma> formasAlerta, List<ControleAlertaTela> telasAlerta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> result = Consulta(codigoFuncionario, codigoEmpresa, status, formasAlerta, telasAlerta);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> Consulta(int codigoFuncionario, int codigoEmpresa, SituacaoAtivoPesquisa status, List<ControleAlertaForma> formasAlerta, List<ControleAlertaTela> telasAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta>();

            var result = from obj in query select obj;

            if (status == SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status);
            else if (status == SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => !obj.Status);

            if (formasAlerta.Count > 0)
                result = result.Where(obj => obj.FormasAlerta.Any(a => formasAlerta.Contains(a)));

            if (telasAlerta.Count > 0)
                result = result.Where(obj => obj.TelasAlerta.Any(a => telasAlerta.Contains(a)));

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == codigoFuncionario);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result;
        }

        #endregion
    }
}