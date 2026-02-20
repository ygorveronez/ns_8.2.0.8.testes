using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios.AlcadaComissao
{
    public class AprovacaoAlcadaFuncionarioComissao : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>
    {
        public AprovacaoAlcadaFuncionarioComissao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool VerificarSePodeAprovar(int codigoFuncionarioComissao, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.FuncionarioComissao.Codigo == codigoFuncionarioComissao
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> ConsultarAutorizacoesPorFuncionarioComissao(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();
            var result = from obj in query where obj.FuncionarioComissao.Codigo == codigo select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorFuncionarioComissao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();
            var result = from obj in query where obj.FuncionarioComissao.Codigo == codigo select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> BuscarPorFuncionarioComissaoUsuarioSituacao(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();
            var result = from obj in query
                         where
                            obj.FuncionarioComissao.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario &&
                            obj.Situacao == situacao
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> BuscarPorFuncionarioComissaoEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();
            var result = from obj in query where obj.FuncionarioComissao.Codigo == codigo select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public int ContarRejeitadas(int codigoFuncionarioComissao, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();
            var resut = from obj in query
                        where
                            obj.FuncionarioComissao.Codigo == codigoFuncionarioComissao
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                            && (obj.RegraFuncionarioComissao.Codigo == codigoRegra || obj.RegraFuncionarioComissao == null)
                        select obj;

            return resut.Count();
        }

        public int ContarPendentes(int codigoFuncionarioComissao, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();
            var resut = from obj in query
                        where
                            obj.FuncionarioComissao.Codigo == codigoFuncionarioComissao
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                            && (obj.RegraFuncionarioComissao.Codigo == codigoRegra || obj.RegraFuncionarioComissao == null)
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesSolicitacao(int codigoFuncionarioComissao, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();
            var resut = from obj in query
                        where
                            obj.FuncionarioComissao.Codigo == codigoFuncionarioComissao
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                            && (obj.RegraFuncionarioComissao.Codigo == codigoRegra || obj.RegraFuncionarioComissao == null)
                        select obj;

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao> BuscarRegraFuncionarioComissao(int codigoFuncionarioComissao)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao>();

            var resultGroup = from obj in queryGroup
                              where obj.FuncionarioComissao.Codigo == codigoFuncionarioComissao
                              select obj.RegraFuncionarioComissao;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> _Consultar(int codigoEmpresa, int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao, int numero, int funcionario, int operador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.FuncionarioComissao;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal.Date <= dataFinal);

            if (situacao.HasValue)
                result = result.Where(obj => obj.Situacao == situacao.Value);

            if (funcionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == funcionario);

            if (operador > 0)
                result = result.Where(obj => obj.Operador.Codigo == operador);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.Autorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.Autorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // O filtro que mais influencia é o de situacao da ocorrencia, pois AgAprovacao, AgAutorizacaoEmissao e AutorizacaoPendente tem o mesmo comportamento
            bool situacaoPendentes = situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.AgAprovacao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacaoAutorizacaoPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente;

            if (situacaoPendentes)
                result = result.Where(obj => obj.Autorizacoes.Any(aut => usuario > 0 ? (aut.Usuario.Codigo == usuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));


            return result;
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> Consultar(int codigoEmpresa, int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao, int numero, int funcionario, int operador, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, funcionario, operador);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao, int numero, int funcionario, int operador)
        {
            var result = _Consultar(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, funcionario, operador);

            return result.Count();
        }
    }
}
