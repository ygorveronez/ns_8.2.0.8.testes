using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class Usuario : RepositorioBase<Dominio.Entidades.Usuario>, Dominio.Interfaces.Repositorios.Usuario
    {
        private CancellationToken _cancellationToken;

        #region Construtores

        public Usuario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Usuario(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { this._cancellationToken = cancellationToken; }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Usuario BuscarPorLoginESenha(string login, string senha, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Login.Equals(login) && obj.Senha.Equals(senha) && obj.TipoAcesso == tipoAcesso select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorLoginVendedorOuGerente(string login)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Login.Equals(login) && (obj.TipoComercial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComercial.Vendedor || obj.TipoComercial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComercial.Gerente) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarUsuarioPorCPFCNPJ(string cpfCnpj, int codigoUsuario, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CPF.Equals(cpfCnpj) && obj.Tipo.Equals("U") && obj.TipoAcesso == tipoAcesso select obj;

            if (codigoUsuario > 0)
                result = result.Where(o => o.Codigo != codigoUsuario);

            return result.FirstOrDefault();
        }

        public void SetarTodosParaAlterarSenhaProximoLogin()
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE Usuario obj set obj.AlterarSenhaAcesso = 1 where obj.Tipo = 'U' and obj.TipoAcesso = 3 ").ExecuteUpdate();
        }

        public void RemoverTodosParaAlterarSenhaProximoLogin()
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE Usuario obj set obj.AlterarSenhaAcesso = 0 where obj.Tipo = 'U' and obj.TipoAcesso = 3 ").ExecuteUpdate();
        }

        public void DeletarMotorista(int codigoMotoristaDeletar)
        {
            UnitOfWork.Sessao.CreateSQLQuery("delete from T_FUNCIONARIO where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetTimeout(6000).ExecuteUpdate();
        }
        public int DeletarCargasMotoristas(int codigoCarga, List<int> codigosMotoristasDeletar)
        {
            if (codigosMotoristasDeletar == null || codigosMotoristasDeletar.Count == 0)
                return 0;

            var placeholders = string.Join(", ", codigosMotoristasDeletar.Select((_, index) => $":codigoMotoristaDeletar{index}"));

            var sql = $"delete from T_CARGA_MOTORISTA where CAR_CODIGO = {codigoCarga} AND CAR_MOTORISTA IN ({placeholders})"; // SQL-INJECTION-SAFE

            var query = UnitOfWork.Sessao.CreateSQLQuery(sql);

            for (int i = 0; i < codigosMotoristasDeletar.Count; i++)
            {
                query.SetInt32($"codigoMotoristaDeletar{i}", codigosMotoristasDeletar[i]);
            }

            return query.SetTimeout(6000).ExecuteUpdate();
        }

        public void UnificarFichaMotorista(int codigoMotoristaDeletar, int codigoMotoristaPermanecer)
        {
            UnitOfWork.Sessao.CreateSQLQuery("update T_VEICULO_SITUACAO set FUM_CODIGO_MOTORISTA = :codigoMotoristaPermanecer where FUM_CODIGO_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_MOVIMENTO_FINANCEIRO_ENTIDADE set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_DOCUMENTO_FATURAMENTO_MOTORISTA set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_CARGA_MOTORISTA set CAR_MOTORISTA = :codigoMotoristaPermanecer where CAR_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_COMISSAO_FUNCIONARIO set FUN_CODIGO_MOTORISTA = :codigoMotoristaPermanecer where FUN_CODIGO_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_COMISSAO_FUNCIONARIO set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_PEDIDO_MOTORISTA set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_CANHOTO_FRETE_MOTORISTAS_RESPONSAVEIS set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_COMISSAO_FUNCIONARIO_MOTORISTA set FUN_CODIGO_MOTORISTA = :codigoMotoristaPermanecer where FUN_CODIGO_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_FROTA_ORDEM_SERVICO set FUN_MOTORISTA = :codigoMotoristaPermanecer where FUN_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_PEDAGIO set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_ABASTECIMENTO set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_INFRACAO set FUN_CODIGO_MOTORISTA = :codigoMotoristaPermanecer where FUN_CODIGO_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_ACERTO_DE_VIAGEM set FUN_CODIGO_MOTORISTA = :codigoMotoristaPermanecer where FUN_CODIGO_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_CARGA_MDFE_MANUAL set FUN_CODIGO_MOTORISTA = :codigoMotoristaPermanecer where FUN_CODIGO_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_CARGA_MDFE_MANUAL_MOTORISTA set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_VEICULO_MOTORISTA set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_FROTA set FUN_CODIGO_MOTORISTA = :codigoMotoristaPermanecer where FUN_CODIGO_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_VEICULO set FUN_CODIGO_MOTORISTA = :codigoMotoristaPermanecer where FUN_CODIGO_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_CARGA_DADOS_MOTORISTA set PED_CODIGO = :codigoMotoristaPermanecer where PED_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_COLABORADOR_LANCAMENTO set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_DOCUMENTO_FATURAMENTO_MOTORISTA set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_CARGA_INTEGRACAO_EMBARCADOR_MOTORISTA set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_HISTORICO_VEICULO_VINCULO_MOTORISTA set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_GUARITA_TMS set FUN_CODIGO_MOTORISTA = :codigoMotoristaPermanecer where FUN_CODIGO_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_NOTIFICACAO set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_REQUISICAO_MERCADORIA set FUN_CODIGO_REQUISITADO = :codigoMotoristaPermanecer where FUN_CODIGO_REQUISITADO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_PAGAMENTO_MOTORISTA_TMS set FUN_CODIGO_MOTORISTA = :codigoMotoristaPermanecer where FUN_CODIGO_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_CARREGAMENTO_MOTORISTAS set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_NOTIFICACAO_QUANTIDADE_NAO_VISUALIZADA set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_GUARITA_CHECK_LIST set FUN_MOTORISTA = :codigoMotoristaPermanecer where FUN_MOTORISTA = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_FUNCIONARIO_ANEXO set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_MOTORISTA_LICENCA set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_PEDIDO_AUTORIZACAO set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_AUTORIZACAO_ALCADA_ORDEM_COMPRA set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("update T_HISTORICO_OBJETO set FUN_CODIGO = :codigoMotoristaPermanecer where FUN_CODIGO = :codigoMotoristaDeletar").SetInt32("codigoMotoristaDeletar", codigoMotoristaDeletar).SetInt32("codigoMotoristaPermanecer", codigoMotoristaPermanecer).SetTimeout(6000).ExecuteUpdate();


        }

        public List<Dominio.Entidades.Usuario> BuscarPorTodosCodigoMobile(int codigoMobile)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CodigoMobile == codigoMobile && obj.Status == "A" select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Usuario BuscarPorCodigoMobile(int codigoMobile)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CodigoMobile == codigoMobile && obj.Status == "A" select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorUsuarioMultisoftware()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.UsuarioMultisoftware select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorUsuarioCallCenter()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.UsuarioCallCenter select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorUsuarioAtendimento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.UsuarioAtendimento select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorAmbienteAdmin()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query 
                         where obj.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin 
                         select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorCodigo(int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Codigo == codigoUsuario select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Usuario> BuscarPorCodigoAsync(int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Codigo == codigoUsuario select obj;
            return await result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Usuario BuscarPorNome(string nomeUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(usuario => usuario.Nome.ToLower().Equals(nomeUsuario.ToLower()));

            return query.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorIntergacao(int codigo, int inicio, int limite)
        {
            var queryMotoristaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();
            var resultMotoristaIntegracao = from obj in queryMotoristaIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultMotoristaIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntergacao(int codigo)
        {
            var queryMotoristaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();
            var resultMotoristaIntegracao = from obj in queryMotoristaIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultMotoristaIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarIntergacaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public string BuscarSession(int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Codigo == codigoUsuario select obj.Session;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorCodigoFetchEmpresaConfiguracao(int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Codigo == codigoUsuario select obj;
            return result.Fetch(o => o.Empresa).ThenFetch(o => o.EmpresaPai).ThenFetch(o => o.Configuracao).Fetch(o => o.Empresa).ThenFetch(o => o.Configuracao).FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> BuscarPorTipoAcesso(Dominio.Enumeradores.TipoAcesso tipoAcesso, string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.TipoAcesso == tipoAcesso select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Contains(nome));

            return result.ToList();
        }

        public int ContarPorTipoAcesso(Dominio.Enumeradores.TipoAcesso tipoAcesso, string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.TipoAcesso == tipoAcesso select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Contains(nome));

            return result.Count();
        }

        public Task<Dominio.Entidades.Usuario> BuscarMotoristaPorCodigoAsync(int codigoMotorista)
        {
            IQueryable<Dominio.Entidades.Usuario> query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(o => o.Codigo == codigoMotorista && o.Tipo == "M");

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Usuario BuscarMotoristaPorCodigo(int codigoMotorista)
        {
            IQueryable<Dominio.Entidades.Usuario> query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(o => o.Codigo == codigoMotorista && o.Tipo == "M");

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristaPorCodigo(int[] codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where codigoMotorista.Contains(obj.Codigo) && obj.Tipo == "M" select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristaPorCodigo(List<int> codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where codigoMotorista.Contains(obj.Codigo) && obj.Tipo == "M" select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristaBloqueadoPorCodigo(int[] codigoMotorista, List<Dominio.Entidades.Usuario> motoristas)
        {
            if (motoristas != null && motoristas.Count > 0)
                return motoristas.Where(x => x.Tipo == "M" && x.Bloqueado == true).ToList();
            else
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
                var result = from obj in query where codigoMotorista.Contains(obj.Codigo) && obj.Tipo == "M" && obj.Bloqueado == true select obj;
                return result.ToList();
            }
        }

        public List<Dominio.Entidades.Usuario> BuscarUsuariosPorCodigos(int[] codigosUsuarios, string tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where codigosUsuarios.Contains(obj.Codigo) select obj;

            if (!string.IsNullOrWhiteSpace(tipo))
                result = result.Where(o => o.Tipo == tipo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarUsuariosPorSetor(int codigoSetor, int codigoEmpresa, int codigoUsuarioDesconsiderar)
        {
            var consultaUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(o =>
                    o.Setor.Codigo == codigoSetor &&
                    (o.SituacaoColaborador == SituacaoColaborador.Trabalhando || o.SituacaoColaborador == null) &&
                    o.Status == "A" &&
                    o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador
                );

            if (codigoEmpresa > 0)
                consultaUsuario = consultaUsuario.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (codigoUsuarioDesconsiderar > 0)
                consultaUsuario = consultaUsuario.Where(o => o.Codigo != codigoUsuarioDesconsiderar);

            return consultaUsuario.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarUsuariosPorSetores(List<int> codigosSetores)
        {
            var consultaUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(usuario =>
                    codigosSetores.Contains(usuario.Setor.Codigo) &&
                    (usuario.SituacaoColaborador == SituacaoColaborador.Trabalhando || usuario.SituacaoColaborador == null) &&
                    usuario.Status == "A" &&
                    usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador
                );

            return consultaUsuario.ToList();
        }

        public int ContarUsuariosPorSetores(List<int> codigosSetores)
        {
            var consultaUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(usuario =>
                    codigosSetores.Contains(usuario.Setor.Codigo) &&
                    (usuario.SituacaoColaborador == SituacaoColaborador.Trabalhando || usuario.SituacaoColaborador == null) &&
                    usuario.Status == "A" &&
                    usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador
                );

            return consultaUsuario.Count();
        }

        public List<Dominio.Entidades.Usuario> BuscarPorSetor(int codigoSetor)
        {
            var consultaUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(o =>
                    o.Setor.Codigo == codigoSetor &&
                    o.Status == "A"
                );

            return consultaUsuario.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarPorPerfil(int codigoEmpresa, int codigoPerfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.PerfilPermissao.Codigo == codigoPerfil select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarPorPerfilEmbarcador(int perfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.PerfilAcesso.Codigo == perfil select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarPorPerfilEmbarcadorMobile(int perfilMobile)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.PerfilAcessoMobile.Codigo == perfilMobile select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarPorPerfilEmbarcador(List<int> codigosPerfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where codigosPerfil.Contains(obj.PerfilAcesso.Codigo) && obj.Status.Equals("A") select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Usuario BuscarPrimeiroPorEmpresa(int codigoEmpresa, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAcesso == tipoAcesso && obj.Status.Equals("A") && (obj.Tipo.Equals("U") || obj.Tipo.Equals("A")) select obj;
            return result.OrderBy(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPrimeiro()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Status.Equals("A") && (obj.Tipo.Equals("U") || obj.Tipo.Equals("A")) select obj;
            return result.OrderBy(o => o.Codigo).FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Usuario> BuscarPrimeiroAsync()
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(obj => obj.Status.Equals("A") && (obj.Tipo.Equals("U") || obj.Tipo.Equals("A")))
                .OrderBy(o => o.Codigo).FirstOrDefaultAsync(_cancellationToken);
        }

        public Dominio.Entidades.Usuario BuscarPorPorCodigoEEmpresa(int codigoEmpresa, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Codigo == codigoUsuario && obj.Empresa.Codigo == codigoEmpresa && !obj.Tipo.Equals("M") select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarMotoristaPorCodigoEEmpresa(int codigoEmpresa, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Codigo == codigoUsuario && obj.Tipo.Equals("M") select obj;


            if (codigoEmpresa > 0)
                result = result.Where(obj => (
                            obj.Empresa.Codigo == codigoEmpresa
                            || obj.Empresa.Matriz.Any(o => o.Codigo == codigoEmpresa)
                            || obj.Empresa.Filiais.Any(o => o.Codigo == codigoEmpresa)
                         ));


            return result.FirstOrDefault();
        }

        
        public Dominio.Entidades.Usuario BuscarMotoristaPorCPF(int codigoEmpresa, string cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = (from obj in query where obj.CPF.Equals(cpfCnpj) && obj.Tipo.Equals("M") select obj);

            if (codigoEmpresa > 0)
                result = result.Where(obj => (
                            obj.Empresa.Codigo == codigoEmpresa
                            || obj.Empresa.Matriz.Any(o => o.Codigo == codigoEmpresa)
                            || obj.Empresa.Filiais.Any(o => o.Codigo == codigoEmpresa)
                         ));

            return result.FirstOrDefault();
        }
        public List<int> BuscarMotoristaPorEmpresaCodigosMotoristas(int codigoEmpresa, List<int> codigoMotoristas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = (from obj in query where codigoMotoristas.Contains(obj.Codigo) && obj.Tipo.Equals("M") select obj);

            if (codigoEmpresa > 0)
                result = result.Where(obj => (
                            obj.Empresa.Codigo == codigoEmpresa
                            || obj.Empresa.Matriz.Any(o => o.Codigo == codigoEmpresa)
                            || obj.Empresa.Filiais.Any(o => o.Codigo == codigoEmpresa)
                         ));

            return result.Select(x => x.Codigo).ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristaPorCPFs(int codigoEmpresa, List<string> cpfs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = (from obj in query where cpfs.Contains(obj.CPF) && obj.Tipo.Equals("M") select obj);

            if (codigoEmpresa > 0)
                result = result.Where(obj => (
                            obj.Empresa.Codigo == codigoEmpresa
                            || obj.Empresa.Matriz.Any(o => o.Codigo == codigoEmpresa)
                            || obj.Empresa.Filiais.Any(o => o.Codigo == codigoEmpresa)
                         ));

            return result.ToList();
        }

        public List<int> BuscarTodosMotoristas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query
                         where
                         obj.Status == "A"
                         && obj.CodigoMobile == 0
                         && obj.Tipo.Equals("M")
                         select obj;
            return result.Select(obj => obj.Codigo).ToList();
        }

        public Dominio.Entidades.Usuario BuscarMotoristaPorNomeETelefone(int codigoEmpresa, string nome, string telefone)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query
                         where
                         obj.Empresa.Codigo == codigoEmpresa
                         && obj.Nome == nome
                         && obj.Telefone == telefone
                         && obj.Tipo.Equals("M")
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristaPorNome(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query
                         where
                         obj.Status.Equals("A")
                         && obj.Nome == nome
                         && obj.Tipo.Equals("M")
                         select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Usuario BuscarPrimeiroMotoristaPorNome(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query
                         where
                         obj.Status.Equals("A")
                         && obj.Nome.Contains(nome)
                         && obj.Tipo.Equals("M")
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> BuscarTodosMotoristasAtivos(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query
                         where
                         obj.Status.Equals("A")
                         && obj.Tipo.Equals("M")
                         select obj;
            return result.Skip(inicio).Take(limite).ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarTodosMotoristasAtivosSemOrdemAberta(int inicio, int limite)
        {
            var queryMotoristasComOrdemAberta = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Compras.OrdemCompra>()
                .Where(obj => obj.Situacao == SituacaoOrdemCompra.Aberta && obj.Motorista != null)
                .Select(obj => obj.Motorista.Codigo)
                .ToList();

            var query = this.SessionNHiBernate
                .Query<Dominio.Entidades.Usuario>()
                .Where(obj => obj.Status.Equals("A") && obj.Tipo.Equals("M") && !queryMotoristasComOrdemAberta.Contains(obj.Codigo))
                .Skip(inicio)
                .Take(limite)
                .ToList();

            return query;
        }


        public int ContarTodosMotoristasAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query
                         where
                         obj.Status.Equals("A")
                         && obj.Tipo.Equals("M")
                         select obj;
            return result.Count();
        }

        public Dominio.Entidades.Usuario BuscarMotoristaPorCPFVarrendoFiliais(int codigoEmpresa, string cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Filiais.Any(emp => emp.Codigo == codigoEmpresa)) && obj.CPF.Equals(cpfCnpj) && obj.Tipo.Equals("M") select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarMotoristaPorCPF(string cpfCnpj, string status = "A")
        {
            IQueryable<Dominio.Entidades.Usuario> query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(obj => obj.CPF.Equals(cpfCnpj) && obj.Tipo.Equals("M"));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status == status);

            return query.OrderBy(o => o.Status).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Usuario> BuscarMotoristaPorCPFAsync(string cpfCnpj, CancellationToken cancellationToken, string status = "A")
        {
            IQueryable<Dominio.Entidades.Usuario> query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(obj => obj.CPF.Equals(cpfCnpj) && obj.Tipo.Equals("M"));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status == status);

            return query.OrderBy(o => o.Status).FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Usuario> BuscarListaMotoristaPorCPF(string cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CPF.Equals(cpfCnpj) select obj;

            return result.OrderBy("Codigo").ToList();
        }

        public bool ExisteMotoristaMesmoCpfBloqueado(string cpf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CPF.Equals(cpf) && obj.Tipo.Equals("M") && obj.Bloqueado && obj.Status == "A" select obj;

            return result.FirstOrDefault() != null;
        }

        public bool ExisteUsuarioParaOhSetor(long codigoSetor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var resultado = query.Where(x => x.Setor.Codigo == codigoSetor).Any();

            return resultado;
        }

        public bool ExisteMotoristaPorCPFEEmpresa(string cpf, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(obj => obj.CPF.Equals(cpf) && obj.Tipo.Equals("M") && obj.Status == "A" && obj.Empresa.Codigo == codigoEmpresa);

            return query.Any();
        }

        public List<Dominio.Entidades.Usuario> BuscarTodosPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) && obj.Status == "A" select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Usuario BuscarMotoristaPorCPFEEmpresa(string cpfCnpj, int empresa, string status = "A")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CPF.Equals(cpfCnpj) && obj.Tipo.Equals("M") select obj;

            if (empresa > 0)
                result = result.Where(o => o.Empresa.Codigo == empresa);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status == status);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarMotoristaPorCPFECNPJEmpresa(string cpfCnpj, string cnpjEmpresa, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CPF.Equals(cpfCnpj) && obj.Tipo.Equals("M") select obj;

            if (!string.IsNullOrWhiteSpace(cnpjEmpresa))
                result = result.Where(o => o.Empresa.CNPJ == cnpjEmpresa);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status == status);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarMotoristaMobilePorCPF(string cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CPF.Equals(cpfCnpj) && obj.Tipo.Equals("M") && obj.Status == "A" && obj.CodigoMobile > 0 select obj;
            return result.FirstOrDefault();
        }


        public int AtualizarVersaoAppPorCPFMotorista(string cpfCnpj, string versaoApp)
        {
            string hql = "update Usuario obj set obj.VersaoAPP = :versao where obj.CPF = :cpf and obj.Tipo = 'M' and obj.Status = 'A' and obj.CodigoMobile > 0 and (obj.VersaoAPP != :versaodif OR obj.VersaoAPP is null) ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetString("versao", versaoApp);
            query.SetString("cpf", cpfCnpj);
            query.SetString("versaodif", versaoApp);
            return query.ExecuteUpdate();
        }
        public Dominio.Entidades.Usuario BuscarMotoristaPorCPF(string cpfCnpj, int codigoDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CPF.Equals(cpfCnpj) && obj.Tipo.Equals("M") && !obj.Codigo.Equals(codigoDiferente) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarUsuarioMobilePorCPF(string cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CPF.Equals(cpfCnpj) && obj.Status == "A" && obj.CodigoMobile > 0 select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorLogin(string login, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Login.Equals(login) && obj.Status == "A" && !obj.UsuarioMultisoftware && !obj.UsuarioCallCenter && !obj.UsuarioAtendimento select obj;
            result = result.Where(o => o.TipoAcesso == tipoAcesso);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarTerceiroPorLogin(string login, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Login.Equals(login) && obj.Status == "A" && !obj.UsuarioMultisoftware && !obj.UsuarioCallCenter && !obj.UsuarioAtendimento select obj;
            result = result.Where(o => o.TipoAcesso == tipoAcesso);
            result = result.Where(o => o.ClienteTerceiro != null);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorLogin(string login, Dominio.Enumeradores.TipoAcesso tipoAcesso, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Login.Equals(login) && obj.TipoAcesso == tipoAcesso && obj.Status == "A" && !obj.UsuarioMultisoftware && !obj.UsuarioCallCenter && !obj.UsuarioAtendimento select obj;

            if (codigoUsuario > 0)
                result = result.Where(o => o.Codigo != codigoUsuario);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorLogin(string login)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Login.Equals(login) && !obj.UsuarioMultisoftware && !obj.UsuarioCallCenter && !obj.UsuarioAtendimento select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorLoginETipo(string login, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Login.Equals(login) && obj.TipoAcesso == tipoAcesso && !obj.UsuarioMultisoftware && !obj.UsuarioCallCenter && !obj.UsuarioAtendimento select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorCliente(double cpfCnpj, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpj && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorCliente(double cpfCnpj, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.ClienteTerceiro.CPF_CNPJ == cpfCnpj && obj.TipoAcesso == tipoAcesso select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorClienteTerceiroAdicionais(string cpfCnpj, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CPF == cpfCnpj && obj.TipoAcesso == tipoAcesso select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> BuscarPorClienteTerceiroAdicionais(double cpfCnpj, Dominio.Entidades.Usuario usuarioTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.ClienteTerceiro.CPF_CNPJ == cpfCnpj && obj.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro && obj.Login != usuarioTerceiro.Login select obj;
            return result.ToList();
        }

        public bool ValidarControleDupliciadade(int valorUK, double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.ControleDuplicidadeUK == valorUK && obj.ClienteTerceiro.CPF_CNPJ == cpfCnpj select obj;
            return result.Count() == 0;
        }

        public bool ValidarControleDupliciadadeUsuarioTerceiroAdicional(int valorUK, string cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.ControleDuplicidadeUK == valorUK && obj.CPF == cpfCnpj select obj;
            return result.Count() == 0;
        }

        public Dominio.Entidades.Usuario BuscarPorTokenConfirmacaoEmail(string tokenConfirmacaoEmail)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.TokenConfirmacaoConta.Equals(tokenConfirmacaoEmail) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorClienteFornecedor(double cpfCnpj, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.ClienteFornecedor.CPF_CNPJ == cpfCnpj && obj.TipoAcesso == tipoAcesso select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> BuscarTodosCadastros()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = (from obj in query select obj);
            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarTodosComFetchLocalidadePerfilAcesso()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            return query
                .Fetch(obj => obj.Localidade)
                .Fetch(obj => obj.PerfilAcesso)
                .ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarTodos(int codigoEmpresa, Dominio.Enumeradores.TipoAcesso tipoAcesso, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = (from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAcesso == tipoAcesso orderby obj.Nome ascending select obj).Skip(inicioRegistros).Take(maximoRegistros);
            return result.ToList();
        }

        public List<string> BuscarEmailsUsuariosParaNotificacaoDevolucaoImprocedente()
        {
            var consultaUsuarios = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = consultaUsuarios
                .Where(usuario =>
                    usuario.Status == "A" &&
                    usuario.Setor != null &&
                    usuario.Setor.NotificarCenarioPosEntregaImprocedenteGestaoDevolucao
                )
                .Select(usuario => usuario.Email);

            return result.ToList();
        }

        public int ContarTodos(int codigoEmpresa, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAcesso == tipoAcesso select obj.Codigo;
            return result.Count();
        }

        public List<Dominio.Entidades.Usuario> ConsultaUsuarios(int codigoEmpresa, string nome, string login, Dominio.Enumeradores.TipoAcesso tipoAcesso, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = (from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAcesso == tipoAcesso && obj.Tipo.Equals("U") select obj);

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(login))
                result = result.Where(o => o.Login.Contains(login));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContaConsultaUsuarios(int codigoEmpresa, string nome, string login, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = (from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAcesso == tipoAcesso && obj.Tipo.Equals("U") select obj);

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(login))
                result = result.Where(o => o.Login.Contains(login));
            return result.Count();
        }

        public List<Dominio.Entidades.Usuario> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario filtrosPesquisa, int inicioRegistros, int maximoRegistros, string propOrdenacao = "", string dirOrdenacao = "")
        {
            var criteria = Consultar(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                criteria = criteria.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                criteria = criteria.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                criteria = criteria.Take(maximoRegistros);

            return criteria.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario filtrosPesquisa)
        {
            var criteria = Consultar(filtrosPesquisa);

            return criteria.Count();
        }

        public IList<Dominio.Entidades.Usuario> ConsultarUsuarios(int codigoEmpresa, string nome, Dominio.Enumeradores.TipoAcesso tipoAcesso, int inicioRegistros, int maximoRegistros, string cpfCnpj = "", string login = "")
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Usuario>();
            criteria.Add(Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(nome))
                criteria.Add(Restrictions.InsensitiveLike("Nome", nome, MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(login))
                criteria.Add(Restrictions.InsensitiveLike("Login", login, MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                criteria.Add(Restrictions.InsensitiveLike("CPF", cpfCnpj, MatchMode.Exact));
            criteria.Add(Restrictions.Eq("TipoAcesso", tipoAcesso));
            criteria.Add(Restrictions.Not(Restrictions.Eq("Tipo", "M")));
            criteria.SetFirstResult(inicioRegistros);
            criteria.SetMaxResults(maximoRegistros);
            return criteria.List<Dominio.Entidades.Usuario>();
        }

        public int ContarConsultaUsuarios(int codigoEmpresa, string nome, Dominio.Enumeradores.TipoAcesso tipoAcesso, string cpfCnpj = "", string login = "")
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Usuario>();
            criteria.Add(Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.Add(Restrictions.Eq("TipoAcesso", tipoAcesso));
            criteria.Add(Restrictions.Not(Restrictions.Eq("Tipo", "M")));
            if (!string.IsNullOrWhiteSpace(nome))
                criteria.Add(Restrictions.InsensitiveLike("Nome", nome, MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                criteria.Add(Restrictions.InsensitiveLike("CPF", cpfCnpj, MatchMode.Exact));
            if (!string.IsNullOrWhiteSpace(login))
                criteria.Add(Restrictions.InsensitiveLike("Login", login, MatchMode.Anywhere));
            criteria.SetProjection(Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public IList<Dominio.Entidades.Usuario> ConsultarUsuariosAtivos(int codigoEmpresa, string nome, Dominio.Enumeradores.TipoAcesso tipoAcesso, int inicioRegistros, int maximoRegistros, string cpfCnpj = "", string login = "")
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Usuario>();
            criteria.Add(Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(nome))
                criteria.Add(Restrictions.InsensitiveLike("Nome", nome, MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(login))
                criteria.Add(Restrictions.InsensitiveLike("Login", login, MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                criteria.Add(Restrictions.InsensitiveLike("CPF", cpfCnpj, MatchMode.Exact));
            criteria.Add(Restrictions.Eq("TipoAcesso", tipoAcesso));
            criteria.Add(Restrictions.Not(Restrictions.Eq("Tipo", "M")));
            criteria.Add(Restrictions.Eq("Status", "A"));
            criteria.SetFirstResult(inicioRegistros);
            criteria.SetMaxResults(maximoRegistros);
            return criteria.List<Dominio.Entidades.Usuario>();
        }

        public int ContarConsultaUsuariosAtivos(int codigoEmpresa, string nome, Dominio.Enumeradores.TipoAcesso tipoAcesso, string cpfCnpj = "", string login = "")
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Usuario>();
            criteria.Add(Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.Add(Restrictions.Eq("TipoAcesso", tipoAcesso));
            criteria.Add(Restrictions.Not(Restrictions.Eq("Tipo", "M")));
            criteria.Add(Restrictions.Eq("Status", "A"));
            if (!string.IsNullOrWhiteSpace(nome))
                criteria.Add(Restrictions.InsensitiveLike("Nome", nome, MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                criteria.Add(Restrictions.InsensitiveLike("CPF", cpfCnpj, MatchMode.Exact));
            if (!string.IsNullOrWhiteSpace(login))
                criteria.Add(Restrictions.InsensitiveLike("Login", login, MatchMode.Anywhere));
            criteria.SetProjection(Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<Dominio.Entidades.Usuario> ConsultarMotoristas(int codigoEmpresa, string nome, Dominio.Enumeradores.TipoAcesso tipoAcesso, string status, int inicioRegistros, int maximoRegistros, string cpfCnpj = "", bool consultarFiliais = false)
        {
            //var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Usuario>();
            //criteria.Add(Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            //if (!string.IsNullOrWhiteSpace(nome))
            //    criteria.Add(Restrictions.InsensitiveLike("Nome", nome, MatchMode.Anywhere));
            //if (!string.IsNullOrWhiteSpace(cpfCnpj))
            //    criteria.Add(Restrictions.InsensitiveLike("CPF", cpfCnpj, MatchMode.Exact));
            //criteria.Add(Restrictions.Eq("TipoAcesso", tipoAcesso));
            //criteria.Add(Restrictions.Eq("Tipo", "M"));

            //if (!string.IsNullOrWhiteSpace(status))
            //    criteria.Add(Restrictions.Eq("Status", status));

            //criteria.SetFirstResult(inicioRegistros);
            //criteria.SetMaxResults(maximoRegistros);
            //return criteria.List<Dominio.Entidades.Usuario>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = (from obj in query where obj.Tipo.Equals("M") && obj.TipoAcesso == tipoAcesso select obj);

            if (codigoEmpresa > 0)
            {
                if (consultarFiliais)
                    result = result.Where(obj => (
                                obj.Empresa.Codigo == codigoEmpresa
                                || obj.Empresa.Matriz.Any(o => o.Codigo == codigoEmpresa)
                                || obj.Empresa.Filiais.Any(o => o.Codigo == codigoEmpresa)
                             ));
                else
                    result = result.Where(obj => (obj.Empresa.Codigo == codigoEmpresa));
            }

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                result = result.Where(o => o.CPF.Equals(cpfCnpj));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaMotoristas(int codigoEmpresa, string nome, Dominio.Enumeradores.TipoAcesso tipoAcesso, string status, string cpfCnpj, bool consultarFiliais = false)
        {
            //var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Usuario>();
            //criteria.Add(Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            ////criteria.Add(Restrictions.Eq("TipoAcesso", tipoAcesso));
            //criteria.Add(Restrictions.Eq("Tipo", "M"));
            //if (!string.IsNullOrWhiteSpace(nome))
            //    criteria.Add(Restrictions.InsensitiveLike("Nome", nome, MatchMode.Anywhere));

            //if (!string.IsNullOrWhiteSpace(status))
            //    criteria.Add(Restrictions.Eq("Status", status));

            //criteria.SetProjection(Projections.RowCount());
            //return criteria.UniqueResult<int>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = (from obj in query where obj.Tipo.Equals("M") && obj.TipoAcesso == tipoAcesso select obj);

            if (codigoEmpresa > 0)
            {
                if (consultarFiliais)
                    result = result.Where(obj => (
                                obj.Empresa.Codigo == codigoEmpresa
                                || obj.Empresa.Matriz.Any(o => o.Codigo == codigoEmpresa)
                                || obj.Empresa.Filiais.Any(o => o.Codigo == codigoEmpresa)
                             ));
                else
                    result = result.Where(obj => (obj.Empresa.Codigo == codigoEmpresa));
            }

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                result = result.Where(o => o.CPF.Equals(cpfCnpj));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }

        public List<Dominio.Entidades.Usuario> ConsultarTodosMotoristasAtivos(int codigoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista tipoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Status.Equals("A") && obj.Tipo.Equals("M"));

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Codigo == codigoMotorista);

            if ((tipoMotorista) > 0)
                result = result.Where(obj => obj.TipoMotorista == tipoMotorista);

            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> ConsultarMotoristas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador, string cpfCnpj, string nome, int empresa, int codigoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista tipoMotorista, bool somenteAtivos, int inicio, int limite, string propOrdenacao, string dirOrdenacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Tipo.Equals("M"));

            string ativo = somenteAtivos ? "A" : "I";

            result = result.Where(o => o.Status == ativo);

            if (situacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando)
                result = result.Where(obj => obj.SituacaoColaborador.Value == situacaoColaborador || obj.SituacaoColaborador == null);

            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                result = result.Where(obj => obj.CPF.Equals(cpfCnpj));
            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            if (codigoMotorista != 0)
                result = result.Where(obj => obj.Codigo == codigoMotorista);

            if ((int)tipoMotorista > 0)
                result = result.Where(obj => obj.TipoMotorista == tipoMotorista);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite).ToList();
        }

        public List<Dominio.Entidades.Usuario> ConsultarAjudantes(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador, string cpfCnpj, string nome, int empresa, int codigoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista tipoMotorista, bool somenteAtivos, int inicio, int limite, string propOrdenacao, string dirOrdenacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Ajudante == true);

            string ativo = somenteAtivos ? "A" : "I";

            result = result.Where(o => o.Status == ativo);

            if (situacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando)
                result = result.Where(obj => obj.SituacaoColaborador.Value == situacaoColaborador || obj.SituacaoColaborador == null);

            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                result = result.Where(obj => obj.CPF.Equals(cpfCnpj));
            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            if (codigoMotorista != 0)
                result = result.Where(obj => obj.Codigo == codigoMotorista);

            if ((int)tipoMotorista > 0)
                result = result.Where(obj => obj.TipoMotorista == tipoMotorista);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsultaAjudantes(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador, string cpfCnpj, string nome, int empresa, int codigoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista tipoMotorista, bool somenteAtivos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Ajudante == true);

            string ativo = somenteAtivos ? "A" : "I";

            result = result.Where(o => o.Status == ativo);

            if (situacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando)
                result = result.Where(obj => obj.SituacaoColaborador.Value == situacaoColaborador || obj.SituacaoColaborador == null);

            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                result = result.Where(obj => obj.CPF.Equals(cpfCnpj));
            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            if (codigoMotorista != 0)
                result = result.Where(obj => obj.Codigo == codigoMotorista);

            if ((int)tipoMotorista > 0)
                result = result.Where(obj => obj.TipoMotorista == tipoMotorista);

            return result.Count();
        }

        public int ContarConsultaMotoristas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador, string cpfCnpj, string nome, int empresa, int codigoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista tipoMotorista, bool somenteAtivos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Tipo.Equals("M"));

            string ativo = somenteAtivos ? "A" : "I";

            result = result.Where(o => o.Status == ativo);

            if (situacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando)
                result = result.Where(obj => obj.SituacaoColaborador.Value == situacaoColaborador || obj.SituacaoColaborador == null);

            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                result = result.Where(obj => obj.CPF.Equals(cpfCnpj));
            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            if (codigoMotorista != 0)
                result = result.Where(obj => obj.Codigo == codigoMotorista);

            if ((int)tipoMotorista > 0)
                result = result.Where(obj => obj.TipoMotorista == tipoMotorista);

            return result.Count();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoEmpresa"></param>
        /// <param name="nome"></param>
        /// <param name="cpfCnpj"></param>
        /// <param name="tipo"></param> 
        /// M = Motorista
        /// U = Usuário
        /// <param name="propOrdenacao"></param>
        /// <param name="dirOrdenacao"></param>
        /// <param name="inicioRegistros"></param>
        /// <param name="maximoRegistros"></param>
        /// <returns></returns>
        public List<Dominio.Entidades.Usuario> ConsultarEmbarcador(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.PendenteIntegracaoEmbarcador)
                result = result.Where(obj => obj.PendenteIntegracaoEmbarcador);

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status.Equals("A"));

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status.Equals("I"));

            if (filtrosPesquisa.SituacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando)
                result = result.Where(obj => obj.SituacaoColaborador.Value == filtrosPesquisa.SituacaoColaborador || obj.SituacaoColaborador == null);


            if (filtrosPesquisa.Empresa != null)
            {
                Dominio.Entidades.Empresa empresaMatriz = filtrosPesquisa.Empresa.Matriz?.FirstOrDefault() ?? filtrosPesquisa.Empresa;
                List<int> codigoFiliais = empresaMatriz?.Filiais?.Select(o => o.Codigo).ToList() ?? new List<int>();

                result = result.Where(obj => obj.Empresas.Contains(filtrosPesquisa.Empresa) || obj.Empresa.Codigo == empresaMatriz.Codigo || codigoFiliais.Contains(obj.Empresa.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Nome))
                result = result.Where(obj => obj.Nome.Contains(filtrosPesquisa.Nome));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfCnpj))
                result = result.Where(obj => obj.CPF == filtrosPesquisa.CpfCnpj);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Tipo))
                result = result.Where(obj => obj.Tipo == filtrosPesquisa.Tipo);

            if (filtrosPesquisa.TipoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos)
                result = result.Where(obj => obj.TipoMotorista == filtrosPesquisa.TipoMotorista);

            if (filtrosPesquisa.TipoMotoristaAjudante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotoristaAjudante.Ajudante)
            {
                result = result.Where(obj => obj.Ajudante == Convert.ToBoolean(filtrosPesquisa.TipoMotoristaAjudante));
            }

            if (filtrosPesquisa.TipoMotoristaAjudante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotoristaAjudante.Motorista)
            {
                result = result.Where(obj => obj.Ajudante == Convert.ToBoolean(filtrosPesquisa.TipoMotoristaAjudante) || !obj.Ajudante.HasValue);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PlacaVeiculo))
            {
                //var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
                //result = result.Where(o => (from obj in queryVeiculos where obj.Motorista.Codigo == o.Codigo && obj.Placa.Contains(placaVeiculo) select obj.Motorista.Codigo).Contains(o.Codigo));

                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                result = result.Where(o => (from obj in queryVeiculoMotorista where obj.Motorista.Codigo == o.Codigo && obj.Veiculo.Placa.Contains(filtrosPesquisa.PlacaVeiculo) select obj.Motorista.Codigo).Contains(o.Codigo));
            }
            if (filtrosPesquisa.SomentePendenteDeVinculo)
            {
                //var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
                //result = result.Where(o => !(from obj in queryVeiculos where obj.Motorista.Codigo == o.Codigo select obj.Motorista.Codigo).Contains(o.Codigo));

                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                result = result.Where(o => !(from obj in queryVeiculoMotorista where obj.Motorista.Codigo == o.Codigo select obj.Motorista.Codigo).Contains(o.Codigo));
            }

            if (filtrosPesquisa.CodigoCargo > 0)
                result = result.Where(o => o.CargoMotorista.Codigo == filtrosPesquisa.CodigoCargo);

            if (filtrosPesquisa.ProprietarioTerceiro > 0d)
            {
                if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                {
                    var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                    result = result.Where(o => queryVeiculoMotorista.Any(obj => obj.Veiculo.Proprietario.CPF_CNPJ == filtrosPesquisa.ProprietarioTerceiro && obj.Motorista == o));
                }
                else
                {
                    result = result.Where(o => o.ClienteTerceiro.CPF_CNPJ == filtrosPesquisa.ProprietarioTerceiro);
                }
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroMatricula))
                result = result.Where(o => o.NumeroMatricula == filtrosPesquisa.NumeroMatricula);

            if (filtrosPesquisa.CnpjRemetenteLocalCarregamentoAutorizado > 0)
            {
                var queryClienteMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado>();
                result = result.Where(o => (from obj in queryClienteMotorista where obj.Motorista.Codigo == o.Codigo && obj.Cliente.CPF_CNPJ.Equals(filtrosPesquisa.CnpjRemetenteLocalCarregamentoAutorizado) select obj.Motorista.Codigo).Contains(o.Codigo));
            }

            if (filtrosPesquisa.NaoBloqueado)
                result = result.Where(o => !o.Bloqueado);

            if (filtrosPesquisa?.CodigosEmpresa?.Count > 0)
                result = result.Where(o => filtrosPesquisa.CodigosEmpresa.Contains(o.Empresa.Codigo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroFrota))
            {
                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                result = result.Where(o => (from obj in queryVeiculoMotorista where obj.Motorista.Codigo == o.Codigo && obj.Veiculo.NumeroFrota.Contains(filtrosPesquisa.NumeroFrota) select obj.Motorista.Codigo).Contains(o.Codigo));
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaEmbarcador(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.PendenteIntegracaoEmbarcador)
                result = result.Where(obj => obj.PendenteIntegracaoEmbarcador);

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status.Equals("A"));

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status.Equals("I"));

            if (filtrosPesquisa.SituacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando)
                result = result.Where(obj => obj.SituacaoColaborador.Value == filtrosPesquisa.SituacaoColaborador || obj.SituacaoColaborador == null);

            if (filtrosPesquisa.Empresa != null)
            {
                Dominio.Entidades.Empresa empresaMatriz = filtrosPesquisa.Empresa.Matriz?.FirstOrDefault() ?? filtrosPesquisa.Empresa;
                List<int> codigoFiliais = empresaMatriz?.Filiais?.Select(o => o.Codigo).ToList() ?? new List<int>();

                result = result.Where(obj => obj.Empresas.Contains(filtrosPesquisa.Empresa) || obj.Empresa.Codigo == empresaMatriz.Codigo || codigoFiliais.Contains(obj.Empresa.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Nome))
                result = result.Where(obj => obj.Nome.Contains(filtrosPesquisa.Nome));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfCnpj))
                result = result.Where(obj => obj.CPF == filtrosPesquisa.CpfCnpj);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Tipo))
                result = result.Where(obj => obj.Tipo == filtrosPesquisa.Tipo);

            if (filtrosPesquisa.TipoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos)
                result = result.Where(obj => obj.TipoMotorista == filtrosPesquisa.TipoMotorista);

            if (filtrosPesquisa.TipoMotoristaAjudante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotoristaAjudante.Ajudante)
            {
                result = result.Where(obj => obj.Ajudante == Convert.ToBoolean(filtrosPesquisa.TipoMotoristaAjudante));
            }

            if (filtrosPesquisa.TipoMotoristaAjudante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotoristaAjudante.Motorista)
            {
                result = result.Where(obj => obj.Ajudante == Convert.ToBoolean(filtrosPesquisa.TipoMotoristaAjudante) || !obj.Ajudante.HasValue);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PlacaVeiculo))
            {
                //var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
                //result = result.Where(o => (from obj in queryVeiculos where obj.Motorista.Codigo == o.Codigo && obj.Placa.Contains(placaVeiculo) select obj.Motorista.Codigo).Contains(o.Codigo));

                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                result = result.Where(o => (from obj in queryVeiculoMotorista where obj.Motorista.Codigo == o.Codigo && obj.Veiculo.Placa.Contains(filtrosPesquisa.PlacaVeiculo) select obj.Motorista.Codigo).Contains(o.Codigo));
            }
            if (filtrosPesquisa.SomentePendenteDeVinculo)
            {
                //var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
                //result = result.Where(o => !(from obj in queryVeiculos where obj.Motorista.Codigo == o.Codigo select obj.Motorista.Codigo).Contains(o.Codigo));

                var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                result = result.Where(o => !(from obj in queryVeiculoMotorista where obj.Motorista.Codigo == o.Codigo select obj.Motorista.Codigo).Contains(o.Codigo));
            }

            if (filtrosPesquisa.CodigoCargo > 0)
                result = result.Where(o => o.CargoMotorista.Codigo == filtrosPesquisa.CodigoCargo);

            if (filtrosPesquisa.ProprietarioTerceiro > 0d)
            {
                if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                {
                    var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                    result = result.Where(o => queryVeiculoMotorista.Any(obj => obj.Veiculo.Proprietario.CPF_CNPJ == filtrosPesquisa.ProprietarioTerceiro && obj.Motorista == o));
                }
                else
                {
                    result = result.Where(o => o.ClienteTerceiro.CPF_CNPJ == filtrosPesquisa.ProprietarioTerceiro);
                }
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroMatricula))
                result = result.Where(o => o.NumeroMatricula == filtrosPesquisa.NumeroMatricula);

            if (filtrosPesquisa.CnpjRemetenteLocalCarregamentoAutorizado > 0)
            {
                var queryClienteMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado>();
                result = result.Where(o => (from obj in queryClienteMotorista where obj.Motorista.Codigo == o.Codigo && obj.Cliente.CPF_CNPJ.Equals(filtrosPesquisa.CnpjRemetenteLocalCarregamentoAutorizado) select obj.Motorista.Codigo).Contains(o.Codigo));
            }

            if (filtrosPesquisa.NaoBloqueado)
                result = result.Where(o => !o.Bloqueado);

            if (filtrosPesquisa?.CodigosEmpresa?.Count > 0)
                result = result.Where(o => filtrosPesquisa.CodigosEmpresa.Contains(o.Empresa.Codigo));

            return result.Count();
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristasProprios(Dominio.Entidades.Empresa empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.Tipo == "M" && obj.Status == "A" && obj.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio select obj;

            if (empresa != null)
                result = result.Where(obj => obj.Empresas.Contains(empresa) || obj.Empresa.Codigo == empresa.Codigo);

            return result.ToList();

        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristasProprios(Dominio.Entidades.Empresa empresa, bool buscarComCargo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.Tipo == "M" && obj.Status == "A" && obj.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio select obj;

            if (buscarComCargo)
                result = result.Where(obj => obj.CargoMotorista != null);

            if (empresa != null)
                result = result.Where(obj => obj.Empresas.Contains(empresa) || obj.Empresa.Codigo == empresa.Codigo);

            return result.ToList();

        }

        public bool ContemVinculoEmTracao(Dominio.Entidades.Usuario motorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Status == "A" && obj.Codigo == motorista.Codigo select obj;
            //var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            //result = result.Where(o => (from obj in queryVeiculos where obj.Motorista.Codigo == o.Codigo select obj.Motorista.Codigo).Contains(o.Codigo));

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            result = result.Where(o => (from obj in queryVeiculoMotorista where obj.Motorista.Codigo == o.Codigo select obj.Motorista.Codigo).Contains(o.Codigo));

            return result.Count() >= 1;
        }

        public string NomeVinculoEmTracao(Dominio.Entidades.Usuario motorista)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            //var result = from obj in query where obj.Ativo && obj.Motorista.Codigo == motorista.Codigo select obj;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Ativo select obj;

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == motorista.Codigo select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            if (result.Count() >= 1)
                return result.FirstOrDefault().Placa;
            else
                return "";
        }

        public string TipoVeiculoVinculado(Dominio.Entidades.Usuario motorista)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            //var result = from obj in query where obj.Ativo && obj.Motorista.Codigo == motorista.Codigo select obj;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
            var result = from obj in query where obj.Ativo select obj;

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == motorista.Codigo select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            if (result.Count() >= 1)
                return result.FirstOrDefault().IsTipoVeiculoTracao() ? "tração" : "reboque";
            else
                return "";
        }

        public Dominio.Entidades.Usuario BuscarPorCPF(string cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.CPF.Equals(cpfCnpj) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristasPorCPF(string cpf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = query.Where(obj => obj.CPF.Equals(cpf) && obj.Tipo == "M" && !obj.Bloqueado);

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Usuario>> BuscarCPFsMotoristasPorCPFsAsync(List<string> cpfs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = query.Where(obj => cpfs.Contains(obj.CPF) && obj.Tipo == "M" && !obj.Bloqueado);

            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristasPorCPFS(List<string> cpfs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = query.Where(obj => cpfs.Contains(obj.CPF) && obj.Tipo == "M" && !obj.Bloqueado);

            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarTodosMotoristasPorCPF(string cpf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = query.Where(obj => obj.CPF.Equals(cpf) && obj.Tipo == "M");

            return result.ToList();
        }

        public Dominio.Entidades.Usuario BuscarMotoristaPorCelular(string telefoneSemFormado, string telefoneFormatado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where (obj.Celular.Equals(telefoneSemFormado) || obj.Celular.Equals(telefoneFormatado)) && obj.Status == "A" && obj.Tipo == "M" select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorCPF(int codigoEmpresa, string cpfCnpjMotorista, string tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.CPF.Equals(cpfCnpjMotorista) && obj.Tipo.Equals(tipo) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Usuario BuscarPorCPF(string cpfCnpj, TipoComercial tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.CPF.Equals(cpfCnpj) select obj;

            if (tipo != TipoComercial.NaoComercial)
                result = result.Where(o => o.TipoComercial == tipo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> BuscarPorEmpresa(int codigoEmpresa, string tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Tipo.Equals(tipo) && obj.Status.Equals("A") select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarPorEmpresaTipoAcesso(int codigoEmpresa, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAcesso == tipoAcesso && obj.Status.Equals("A") select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarUsuariosParaRecados(int codigoEmpresa, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAcesso == tipoAcesso && obj.Status.Equals("A") && obj.Callcenter select obj;

            //var queryPermissao = this.SessionNHiBernate.Query<Dominio.Entidades.PaginaUsuario>();
            //result = result.Where(o => (from obj in queryPermissao where obj.Pagina.Formulario.ToLower().Equals("empresas.aspx") && obj.PermissaoDeAcesso.Equals("A") && obj.PermissaoDeAlteracao.Equals("A") select obj.Usuario.Codigo).Contains(o.Codigo));

            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarUsuariosAcessoTransportador(int codigoEmpresa)
        {
            var consultaUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(o =>
                    o.Empresa.Codigo == codigoEmpresa &&
                    o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao &&
                    o.Status == "A" &&
                    o.Tipo == "U"
                );

            return consultaUsuario.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristasPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Tipo.Equals("M") && obj.Status.Equals("A") select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Usuario BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> ConsultarUsuarios(int codigoEmpresa, string tipo, string status, string nome, string cpfCnpj, string login, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(tipo))
                result = result.Where(o => o.Tipo.Equals(tipo));

            if (!string.IsNullOrWhiteSpace(tipo))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Equals(nome));

            if (!string.IsNullOrWhiteSpace(login))
                result = result.Where(o => o.Login.Equals(login));

            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                result = result.Where(o => o.CPF.Equals(cpfCnpj));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarUsuarios(int codigoEmpresa, string tipo, string status, string nome, string cpfCnpj, string login)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(tipo))
                result = result.Where(o => o.Tipo.Equals(tipo));

            if (!string.IsNullOrWhiteSpace(tipo))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Equals(nome));

            if (!string.IsNullOrWhiteSpace(login))
                result = result.Where(o => o.Login.Equals(login));

            if (!string.IsNullOrWhiteSpace(cpfCnpj))
                result = result.Where(o => o.CPF.Equals(cpfCnpj));

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Motorista> ConsultarRelatorioMotorista(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaMotorista().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Motorista)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.Motorista>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Motorista>> ConsultarRelatorioMotoristaAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaMotorista().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Motorista)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.Motorista>();
        }

        public int ContarConsultaRelatorioMotorista(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaMotorista().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario> ConsultarRelatorioUsuarios(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioUsuarios(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario>> ConsultarRelatorioUsuariosAsync(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioUsuarios(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario>();
        }

        public int ContarConsultaRelatorioUsuarios(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioUsuarios(filtrosPesquisa, true, propriedades, "", "", "", "", 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public async Task<int> ContarConsultaRelatorioUsuariosAsync(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioUsuarios(filtrosPesquisa, true, propriedades, "", "", "", "", 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return await query.SetTimeout(600).UniqueResultAsync<int>();
        }

        public List<Dominio.Entidades.Usuario> ConsultarMotoristaMobile(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotoristaMobile filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotoristaMobile = ConsultarMotoristaMobile(filtrosPesquisa);

            return ObterLista(consultaMotoristaMobile, parametrosConsulta);
        }

        public int ContarConsultaMotoristaMobile(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotoristaMobile filtrosPesquisa)
        {
            var consultaMotoristaMobile = ConsultarMotoristaMobile(filtrosPesquisa);

            return consultaMotoristaMobile.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Notificacao.ChatUsuario> BuscarUsuariosChat(int codigoFuncionarioLogado, int codigoEmpresa, int codigoEmpresaPai)
        {
            string query = @" SELECT T.Codigo, T.Login, T.Nome,
                                    (SELECT count(1)
                                       FROM T_CHAT chat
                                       INNER JOIN T_FUNCIONARIO funcionarioRecebedor ON chat.FUN_CODIGO_RECEBEDOR = funcionarioRecebedor.FUN_CODIGO
                                       INNER JOIN T_FUNCIONARIO funcionarioEnvio ON chat.FUN_CODIGO_ENVIO = funcionarioEnvio.FUN_CODIGO
                                       WHERE funcionarioRecebedor.FUN_CODIGO = " + codigoFuncionarioLogado + @" AND funcionarioEnvio.FUN_CODIGO = T.Codigo
                                         AND NOT (chat.CHT_LIDA = 1)) TotalMensagensNaoLidas,
                                    (SELECT count(1)
                                       FROM T_CHAT chat
                                       LEFT OUTER JOIN T_FUNCIONARIO funcionarioEnvio ON chat.FUN_CODIGO_ENVIO = funcionarioEnvio.FUN_CODIGO
                                       LEFT OUTER JOIN T_FUNCIONARIO funcionarioRecebedor ON chat.FUN_CODIGO_RECEBEDOR = funcionarioRecebedor.FUN_CODIGO
                                       WHERE (funcionarioEnvio.FUN_CODIGO = " + codigoFuncionarioLogado + @" OR funcionarioRecebedor.FUN_CODIGO = " + codigoFuncionarioLogado + @")
                                         AND (funcionarioEnvio.FUN_CODIGO = T.Codigo OR funcionarioRecebedor.FUN_CODIGO = T.Codigo)) TotalMensagens
                              FROM (
                              SELECT F.FUN_CODIGO Codigo,
                                    F.FUN_NOME Nome,
                                    F.FUN_LOGIN Login
                               FROM T_FUNCIONARIO F
                               LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = F.EMP_CODIGO
                               WHERE F.FUN_TIPO = 'U' AND F.FUN_LOGIN <> '' AND F.FUN_LOGIN IS NOT NULL AND F.FUN_STATUS = 'A' AND F.FUN_CODIGO != " + codigoFuncionarioLogado;

            if (codigoEmpresa > 0)//MultiNFe
                query += " AND (F.EMP_CODIGO = " + codigoEmpresa + " OR E.EMP_EMPRESA = " + codigoEmpresa + ") AND E.EMP_STATUS = 'A' AND (E.EMP_STATUS_FINANCEIRO = 'N' OR E.EMP_STATUS_FINANCEIRO IS NULL)";
            else
                query += " AND (F.FUN_FISJUR = 'F' OR F.FUN_FISJUR IS NULL)";

            if (codigoEmpresaPai > 0)
            {
                query += @" UNION

                            SELECT F.FUN_CODIGO Codigo,
                                    F.FUN_NOME Nome,
                                    F.FUN_LOGIN Login
                               FROM T_FUNCIONARIO F
                               WHERE F.FUN_TIPO = 'U' AND F.FUN_LOGIN <> '' AND F.FUN_LOGIN IS NOT NULL AND F.FUN_STATUS = 'A' AND F.FUN_CODIGO != " + codigoFuncionarioLogado + " AND F.EMP_CODIGO = " + codigoEmpresaPai;

                if (codigoEmpresa == 0)
                    query += " AND (F.FUN_FISJUR = 'F' OR F.FUN_FISJUR IS NULL)";
            }

            query += ") AS T";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Notificacao.ChatUsuario)));

            return nhQuery.List<Dominio.ObjetosDeValor.Embarcador.Notificacao.ChatUsuario>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Transportadores.ConsultaPagamentoMotorista> ConsultarConsultaPagamentoMotorista(string observacao, string valor, string tipoDoPagamento, string data, int codigoContaCredito, int codigoContaDebito, int codigoTipoPagamentoMotorista, int codigoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista tipoMotorista, bool somenteAtivos, int codigoCentroResultado, int inicio, int limite, string propOrdenacao, string dirOrdenacao)
        {
            string query = @"SELECT Motorista.FUN_CODIGO Codigo,
                            " + codigoTipoPagamentoMotorista + @" CodigoTipoPagamentoMotorista,
                            " + codigoContaDebito + @" CodigoContaDebito,
                            " + codigoContaCredito + @" CodigoContaCredito,
                            Motorista.FUN_CPF CPF,
                            Motorista.FUN_NOME Nome,
                            '" + data + @"' Data,
                            '" + tipoDoPagamento + @"' TipoDoPagamento,
                            '" + valor + @"' Valor,
                            '" + observacao + @"' Observacao,
                            SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA
                            FROM T_VEICULO_MOTORISTA VMT
			                INNER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = VMT.VEI_CODIGO
                            WHERE VMT.FUN_CODIGO = Motorista.FUN_CODIGO FOR XML PATH('')), 3, 1000) Veiculos
                            from T_FUNCIONARIO Motorista
                            WHERE Motorista.FUN_TIPO = 'M' ";

            if (somenteAtivos)
                query += " AND Motorista.FUN_STATUS = 'A'";

            if (codigoMotorista != 0)
                query += " AND Motorista.FUN_CODIGO = " + codigoMotorista + "";

            if ((int)tipoMotorista > 0)
                query += " AND Motorista.FUN_TIPO_MOTORISTA = " + (int)tipoMotorista + "";

            if (codigoCentroResultado > 0)
                query += " AND Motorista.CRE_CODIGO = " + codigoCentroResultado;

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                query += " order by " + propOrdenacao + " " + dirOrdenacao;

            if (limite > 0)
                query += " OFFSET " + inicio + " ROWS FETCH FIRST " + limite + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Transportadores.ConsultaPagamentoMotorista)));

            return nhQuery.List<Dominio.ObjetosDeValor.Embarcador.Transportadores.ConsultaPagamentoMotorista>();
        }

        public int ContarConsultarConsultaPagamentoMotorista(int codigoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista tipoMotorista, bool somenteAtivos, int codigoCentroResultado)
        {
            string query = @"SELECT COUNT(0) as CONTADOR                             
                            from T_FUNCIONARIO Motorista
                            WHERE Motorista.FUN_TIPO = 'M'";

            if (somenteAtivos)
                query += " AND Motorista.FUN_STATUS = 'A'";

            if (codigoMotorista != 0)
                query += " AND Motorista.FUN_CODIGO = " + codigoMotorista + "";

            if ((int)tipoMotorista > 0)
                query += " AND Motorista.FUN_TIPO_MOTORISTA = " + (int)tipoMotorista + "";

            if (codigoCentroResultado > 0)
                query += " AND Motorista.CRE_CODIGO = " + codigoCentroResultado;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Frota.ProgramacaoLogistica> ConsultarProgramacaoLogistica(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoLogistica filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            string query = @"SELECT Motorista.FUN_CODIGO Codigo, Motorista.FUN_NOME Motorista,

                (SELECT TOP(1) Situacao.CSI_DESCRICAO + ' ' + CASE WHEN SituacaoExecucao.CLS_SITUACAO = 1 THEN '(Agendado)' ELSE '' END FROM T_COLABORADOR_LANCAMENTO SituacaoExecucao
                JOIN T_COLABORADOR_SITUACAO Situacao on Situacao.CSI_CODIGO = SituacaoExecucao.CSI_CODIGO
                where SituacaoExecucao.CLS_SITUACAO in (3,1) AND SituacaoExecucao.FUN_CODIGO = Motorista.FUN_CODIGO 
                order by SituacaoExecucao.CLS_DATA_INICIAL) SituacaoColaborador,

                (SELECT (SELECT TOP(1) Convert(varchar(10), SituacaoExecucao.CLS_DATA_INICIAL,103) + ' à ' + Convert(varchar(10), SituacaoExecucao.CLS_DATA_FINAL,103)
				  FROM T_COLABORADOR_LANCAMENTO SituacaoExecucao
				  JOIN T_COLABORADOR_SITUACAO Situacao on Situacao.CSI_CODIGO = SituacaoExecucao.CSI_CODIGO
				  where SituacaoExecucao.CLS_SITUACAO in (3,1) AND SituacaoExecucao.FUN_CODIGO = Motorista.FUN_CODIGO 
				  order by SituacaoExecucao.CLS_DATA_INICIAL)) as PeriodoSituacaoColaborador,

                (SELECT TOP(1) Situacao.CSI_COR FROM T_COLABORADOR_LANCAMENTO SituacaoExecucao
                JOIN T_COLABORADOR_SITUACAO Situacao on Situacao.CSI_CODIGO = SituacaoExecucao.CSI_CODIGO
                where SituacaoExecucao.CLS_SITUACAO in (3,1) AND SituacaoExecucao.FUN_CODIGO = Motorista.FUN_CODIGO
                order by SituacaoExecucao.CLS_DATA_INICIAL) CorSituacaoColaborador,

                SUBSTRING((SELECT DISTINCT ', ' + ISNULL(Veiculo.VEI_NUMERO_FROTA, '0') + ' / ' +
                ISNULL(SUBSTRING((SELECT DISTINCT ', ' + ISNULL(Reboque.VEI_NUMERO_FROTA, '0')
                FROM T_VEICULO Reboque
                JOIN T_VEICULO_CONJUNTO Conjunto on Reboque.VEI_CODIGO = Conjunto.VEC_CODIGO_FILHO
                WHERE Conjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO AND Reboque.VEI_ATIVO = 1
                FOR XML PATH('')), 3, 1000), '')
                FROM T_VEICULO Veiculo
				JOIN T_VEICULO_MOTORISTA Motoristas on Motoristas.VEI_CODIGO = Veiculo.VEI_CODIGO
                WHERE Motoristas.FUN_CODIGO = Motorista.FUN_CODIGO AND Veiculo.VEI_ATIVO = 1 and Veiculo.VEI_TIPOVEICULO = '0'
                FOR XML PATH('')), 3, 1000) Veiculos,

                SUBSTRING((SELECT DISTINCT ', ' + ISNULL(Veiculo.VEI_NUMERO_FROTA, '0') + ' / ' +
                ISNULL(SUBSTRING((SELECT DISTINCT ', ' + ISNULL(Reboque.VEI_NUMERO_FROTA, '0')
                FROM T_VEICULO Reboque
                JOIN T_VEICULO_CONJUNTO Conjunto on Reboque.VEI_CODIGO = Conjunto.VEC_CODIGO_FILHO
                JOIN T_FROTA_ORDEM_SERVICO OrdemReboque on OrdemReboque.VEI_CODIGO = Reboque.VEI_CODIGO and OrdemReboque.OSE_SITUACAO = 3
                WHERE Conjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO AND Reboque.VEI_ATIVO = 1
                FOR XML PATH('')), 3, 1000), '')
                FROM T_VEICULO Veiculo
                JOIN T_FROTA_ORDEM_SERVICO OrdemVeiculo on OrdemVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO and OrdemVeiculo.OSE_SITUACAO = 3
				JOIN T_VEICULO_MOTORISTA Motoristas on Motoristas.VEI_CODIGO = Veiculo.VEI_CODIGO
                WHERE Motoristas.FUN_CODIGO = Motorista.FUN_CODIGO AND Veiculo.VEI_ATIVO = 1
                FOR XML PATH('')), 3, 1000) Manutencao,

                (SELECT top(1) Carga.CAR_SITUACAO FROM T_CARGA Carga 
                JOIN T_CARGA_MOTORISTA CargaMotorista on CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO
                WHERE CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO
                AND Carga.CAR_SITUACAO NOT IN (13,18,11)
                ORDER BY Carga.CAR_CODIGO desc) SituacaoCarga,

                SUBSTRING((SELECT ', ' + ISNULL(ModCarroceria.MCA_DESCRICAO, '') + ' ' +
                ISNULL(SUBSTRING((SELECT ', ' + ISNULL(ModCarroceria.MCA_DESCRICAO, '')
                FROM T_VEICULO Reboque
                JOIN T_VEICULO_CONJUNTO Conjunto on Reboque.VEI_CODIGO = Conjunto.VEC_CODIGO_FILHO
                LEFT OUTER JOIN T_MODELO_CARROCERIA ModCarroceria on ModCarroceria.MCA_CODIGO = Reboque.MCA_CODIGO
                WHERE Conjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO AND Reboque.VEI_ATIVO = 1
                FOR XML PATH('')), 3, 1000), '')
                FROM T_VEICULO Veiculo
                LEFT OUTER JOIN T_MODELO_CARROCERIA ModCarroceria on ModCarroceria.MCA_CODIGO = Veiculo.MCA_CODIGO
				JOIN T_VEICULO_MOTORISTA Motoristas on Motoristas.VEI_CODIGO = Veiculo.VEI_CODIGO
                WHERE Motoristas.FUN_CODIGO = Motorista.FUN_CODIGO AND Veiculo.VEI_ATIVO = 1
                FOR XML PATH('')), 3, 1000) TipoCarroceria,

                SUBSTRING((SELECT ', ' + ISNULL(ModCarroceria.MVC_DESCRICAO, '') + ' ' + 
                ISNULL(SUBSTRING((SELECT ', ' + ISNULL(ModCarroceria.MVC_DESCRICAO, '')
                FROM T_VEICULO Reboque
                JOIN T_VEICULO_CONJUNTO Conjunto on Reboque.VEI_CODIGO = Conjunto.VEC_CODIGO_FILHO
                LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModCarroceria on ModCarroceria.MVC_CODIGO = Reboque.MVC_CODIGO
                WHERE Conjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO AND Reboque.VEI_ATIVO = 1
                FOR XML PATH('')), 3, 1000), '')
                FROM T_VEICULO Veiculo
                LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModCarroceria on ModCarroceria.MVC_CODIGO = Veiculo.MVC_CODIGO
				JOIN T_VEICULO_MOTORISTA Motoristas on Motoristas.VEI_CODIGO = Veiculo.VEI_CODIGO
                WHERE Motoristas.FUN_CODIGO = Motorista.FUN_CODIGO AND Veiculo.VEI_ATIVO = 1
                FOR XML PATH('')), 3, 1000) TipoVeiculo,

                SUBSTRING((SELECT ', ' + ISNULL(Plotagem.VTP_DESCRICAO, '') + ' ' +
                ISNULL(SUBSTRING((SELECT ', ' + ISNULL(Plotagem.VTP_DESCRICAO, '')
                FROM T_VEICULO Reboque
                JOIN T_VEICULO_CONJUNTO Conjunto on Reboque.VEI_CODIGO = Conjunto.VEC_CODIGO_FILHO
                LEFT OUTER JOIN T_VEICULO_TIPO_PLOTAGEM Plotagem on Plotagem.VTP_CODIGO = Reboque.VEI_TIPO_PLOTAGEM
                WHERE Conjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO AND Reboque.VEI_ATIVO = 1
                FOR XML PATH('')), 3, 1000), '')
                FROM T_VEICULO Veiculo
                LEFT OUTER JOIN T_VEICULO_TIPO_PLOTAGEM Plotagem on Plotagem.VTP_CODIGO = Veiculo.VEI_TIPO_PLOTAGEM
				JOIN T_VEICULO_MOTORISTA Motoristas on Motoristas.VEI_CODIGO = Veiculo.VEI_CODIGO
                WHERE Motoristas.FUN_CODIGO = Motorista.FUN_CODIGO AND Veiculo.VEI_ATIVO = 1
                FOR XML PATH('')), 3, 1000) TipoPlotagem,

                CASE 
					WHEN (SELECT top(1) Guarita.GUA_ENTRADA_SAIDA 
						FROM T_GUARITA_TMS Guarita
						where Guarita.VEI_CODIGO in (SELECT veiculos.VEI_CODIGO FROM T_VEICULO veiculos JOIN T_VEICULO_MOTORISTA Motoristas on Motoristas.VEI_CODIGO = veiculos.VEI_CODIGO WHERE Motoristas.FUN_CODIGO = Motorista.FUN_CODIGO)						
						order by Guarita.GUA_DATA_SAIDA_ENTRADA desc) = 2
					THEN NULL
					ELSE (SELECT top(1) Guarita.GUA_DATA_SAIDA_ENTRADA 
							FROM T_GUARITA_TMS Guarita
						   where Guarita.VEI_CODIGO in (SELECT veiculos.VEI_CODIGO FROM T_VEICULO veiculos JOIN T_VEICULO_MOTORISTA Motoristas on Motoristas.VEI_CODIGO = veiculos.VEI_CODIGO WHERE Motoristas.FUN_CODIGO = Motorista.FUN_CODIGO)
							 and Guarita.GUA_ENTRADA_SAIDA = 1
						order by Guarita.GUA_DATA_SAIDA_ENTRADA desc) END DataUltimaEntragaGuarita,

                (SELECT top(1) Guarita.GUA_OBSERVACAO 
                FROM T_GUARITA_TMS Guarita
                where Guarita.VEI_CODIGO in (SELECT veiculos.VEI_CODIGO FROM T_VEICULO veiculos JOIN T_VEICULO_MOTORISTA Motoristas on Motoristas.VEI_CODIGO = veiculos.VEI_CODIGO WHERE Motoristas.FUN_CODIGO = Motorista.FUN_CODIGO)
                and Guarita.GUA_ENTRADA_SAIDA = 1
                order by Guarita.GUA_DATA_SAIDA_ENTRADA desc) ObservacaoUltimaEntragaGuarita,

                (CASE 
                    WHEN EXISTS(SELECT Acerto.ACV_CODIGO FROM T_ACERTO_DE_VIAGEM Acerto where Acerto.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO and Acerto.ACV_SITUACAO = 1) THEN 0
					WHEN EXISTS(SELECT Lancamento.CLS_DATA_FINAL FROM T_COLABORADOR_LANCAMENTO Lancamento where Lancamento.FUN_CODIGO = Motorista.FUN_CODIGO and Lancamento.CLS_SITUACAO = 3) THEN 0
					WHEN EXISTS(SELECT Lancamento.CLS_DATA_FINAL FROM T_COLABORADOR_LANCAMENTO Lancamento where Lancamento.FUN_CODIGO = Motorista.FUN_CODIGO and Lancamento.CLS_SITUACAO = 4)  
					AND (Motorista.FUN_DATA_FECHAMENTO_ACERTO IS NULL OR (SELECT TOP(1) Lancamento.CLS_DATA_FINAL FROM T_COLABORADOR_LANCAMENTO Lancamento where Lancamento.FUN_CODIGO = Motorista.FUN_CODIGO and Lancamento.CLS_SITUACAO = 4 ORDER BY Lancamento.CLS_DATA_FINAL DESC) > Motorista.FUN_DATA_FECHAMENTO_ACERTO)
					THEN DAY(GETDATE() -(SELECT TOP(1) Lancamento.CLS_DATA_FINAL FROM T_COLABORADOR_LANCAMENTO Lancamento where Lancamento.FUN_CODIGO = Motorista.FUN_CODIGO and Lancamento.CLS_SITUACAO = 4 ORDER BY Lancamento.CLS_DATA_FINAL DESC)) - 1
					WHEN Motorista.FUN_DATA_FECHAMENTO_ACERTO IS NOT NULL AND NOT EXISTS(SELECT A.FUN_CODIGO_MOTORISTA FROM T_ACERTO_DE_VIAGEM A WHERE A.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO AND A.ACV_SITUACAO = 1) THEN DAY(GETDATE() - Motorista.FUN_DATA_FECHAMENTO_ACERTO) - 1
					ELSE 0 END
				) Ociosidade

                FROM T_FUNCIONARIO Motorista
                WHERE Motorista.FUN_TIPO = 'M' AND Motorista.FUN_STATUS = 'A'";

            if (filtrosPesquisa.TipoGuarita == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Entrada)
                query += @" AND (SELECT top(1) Guarita.GUA_ENTRADA_SAIDA 
						FROM T_GUARITA_TMS Guarita
						where Guarita.VEI_CODIGO in (SELECT veiculos.VEI_CODIGO FROM T_VEICULO veiculos WHERE veiculos.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO)						
						order by Guarita.GUA_DATA_SAIDA_ENTRADA desc) = 1 ";
            else if (filtrosPesquisa.TipoGuarita == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Saida)
                query += @" AND (SELECT top(1) Guarita.GUA_ENTRADA_SAIDA 
						FROM T_GUARITA_TMS Guarita
						where Guarita.VEI_CODIGO in (SELECT veiculos.VEI_CODIGO FROM T_VEICULO veiculos WHERE veiculos.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO)						
						order by Guarita.GUA_DATA_SAIDA_ENTRADA desc) = 2 ";

            if (filtrosPesquisa.EmManutencao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                query += @" AND (Motorista.FUN_CODIGO IN (SELECT Veiculo.FUN_CODIGO_MOTORISTA 
				    FROM T_VEICULO Veiculo
                    JOIN T_FROTA_ORDEM_SERVICO OrdemVeiculo on OrdemVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO and OrdemVeiculo.OSE_SITUACAO = 3
                    WHERE Veiculo.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO)
				    OR
				    Motorista.FUN_CODIGO IN (SELECT Veiculo.FUN_CODIGO_MOTORISTA 
				    FROM T_VEICULO Reboque
                    JOIN T_VEICULO_CONJUNTO Conjunto on Reboque.VEI_CODIGO = Conjunto.VEC_CODIGO_FILHO
                    JOIN T_FROTA_ORDEM_SERVICO OrdemReboque on OrdemReboque.VEI_CODIGO = Reboque.VEI_CODIGO and OrdemReboque.OSE_SITUACAO = 3
				    JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Conjunto.VEC_CODIGO_PAI
                    WHERE Conjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO)) ";
            else if (filtrosPesquisa.EmManutencao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                query += @" AND Motorista.FUN_CODIGO NOT IN (SELECT ISNULL(Veiculo.FUN_CODIGO_MOTORISTA,0) 
				    FROM T_VEICULO Veiculo
                    JOIN T_FROTA_ORDEM_SERVICO OrdemVeiculo on OrdemVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO and OrdemVeiculo.OSE_SITUACAO = 3
                    WHERE Veiculo.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO)
				    AND
				    Motorista.FUN_CODIGO NOT IN (SELECT ISNULL(Veiculo.FUN_CODIGO_MOTORISTA,0) 
				    FROM T_VEICULO Reboque
                    JOIN T_VEICULO_CONJUNTO Conjunto on Reboque.VEI_CODIGO = Conjunto.VEC_CODIGO_FILHO
                    JOIN T_FROTA_ORDEM_SERVICO OrdemReboque on OrdemReboque.VEI_CODIGO = Reboque.VEI_CODIGO and OrdemReboque.OSE_SITUACAO = 3
				    JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Conjunto.VEC_CODIGO_PAI
                    WHERE Conjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO) ";

            if (filtrosPesquisa.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio)
                query += " AND Motorista.FUN_TIPO_MOTORISTA = 1";
            else if (filtrosPesquisa.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro)
                query += " AND Motorista.FUN_TIPO_MOTORISTA = 2";

            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND Motorista.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista;

            if (filtrosPesquisa.CodigoProgramacaoAlocacao > 0)
                query += " AND Motorista.PMO_CODIGO IN (SELECT Prog.PMO_CODIGO FROM T_PROGRAMACAO_MOTORISTA Prog where Prog.PAL_CODIGO = " + filtrosPesquisa.CodigoProgramacaoAlocacao + ")"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigosTipoVeiculo.Count > 0)
            {
                var tiposVeiculos = string.Join(",", from o in filtrosPesquisa.CodigosTipoVeiculo select o.ToString());
                query += @" AND (Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic where Veic.MVC_CODIGO IN (" + tiposVeiculos + @"))
                OR  Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic JOIN T_VEICULO_CONJUNTO Conj on Conj.VEC_CODIGO_PAI = Veic.VEI_CODIGO JOIN T_VEICULO Reb on Reb.VEI_CODIGO = Conj.VEC_CODIGO_FILHO where Reb.MVC_CODIGO IN (" + tiposVeiculos + @")))"; // SQL-INJECTION-SAFE
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += @" AND (Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic where Veic.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo + @")
                            OR  Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic JOIN T_VEICULO_CONJUNTO Conj on Conj.VEC_CODIGO_PAI = Veic.VEI_CODIGO JOIN T_VEICULO Reb on Reb.VEI_CODIGO = Conj.VEC_CODIGO_FILHO where Reb.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo + @"))"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigosPlotagemVeiculo.Count > 0)
            {
                var plotagensVeiculos = string.Join(",", filtrosPesquisa.CodigosPlotagemVeiculo);
                query += @" AND (Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic where Veic.VEI_TIPO_PLOTAGEM IN (" + plotagensVeiculos + @"))
                            OR  Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic JOIN T_VEICULO_CONJUNTO Conj on Conj.VEC_CODIGO_PAI = Veic.VEI_CODIGO JOIN T_VEICULO Reb on Reb.VEI_CODIGO = Conj.VEC_CODIGO_FILHO where Reb.VEI_TIPO_PLOTAGEM IN (" + plotagensVeiculos + @")))"; // SQL-INJECTION-SAFE
            }

            if (filtrosPesquisa.CodigosTipoCarroceria.Count > 0)
            {
                var tiposCarroceria = string.Join(",", filtrosPesquisa.CodigosTipoCarroceria);
                query += @" AND (Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic where Veic.MCA_CODIGO IN (" + tiposCarroceria + @"))
                            OR  Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic JOIN T_VEICULO_CONJUNTO Conj on Conj.VEC_CODIGO_PAI = Veic.VEI_CODIGO JOIN T_VEICULO Reb on Reb.VEI_CODIGO = Conj.VEC_CODIGO_FILHO where Reb.MCA_CODIGO IN (" + tiposCarroceria + @")))"; // SQL-INJECTION-SAFE
            }

            if (filtrosPesquisa.EmViagem == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                query += " AND Motorista.FUN_CODIGO in (SELECT a.FUN_CODIGO_MOTORISTA FROM T_ACERTO_DE_VIAGEM a Where a.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO and a.ACV_SITUACAO = 1)";
            else if (filtrosPesquisa.EmViagem == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                query += " AND Motorista.FUN_CODIGO not in (SELECT a.FUN_CODIGO_MOTORISTA FROM T_ACERTO_DE_VIAGEM a Where a.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO and a.ACV_SITUACAO = 1)";

            if (filtrosPesquisa.SituacaoProgramacaoLogistica == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProgramacaoLogistica.EmCarga)
                query += @" AND Motorista.FUN_CODIGO IN (SELECT CargaMotorista.CAR_MOTORISTA FROM T_CARGA Carga 
                    JOIN T_CARGA_MOTORISTA CargaMotorista on CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO
                    WHERE CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO
                    AND Carga.CAR_SITUACAO NOT IN (13,18,11)) ";
            else if (filtrosPesquisa.SituacaoProgramacaoLogistica == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProgramacaoLogistica.Vazio)
                query += " AND Motorista.FUN_CODIGO IN (SELECT Veic.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic WHERE Veic.VEI_VAZIO = 1)";
            else if (filtrosPesquisa.SituacaoProgramacaoLogistica == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProgramacaoLogistica.NaoVazio)
                query += " AND Motorista.FUN_CODIGO IN (SELECT Veic.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic WHERE Veic.VEI_VAZIO = 0 or Veic.VEI_VAZIO IS NULL)";

            if (filtrosPesquisa.CodigosSituacaoColaborador.Count > 0)
                query += @" AND Motorista.FUN_CODIGO IN (SELECT SituacaoExecucao.FUN_CODIGO FROM T_COLABORADOR_LANCAMENTO SituacaoExecucao                
                            where SituacaoExecucao.CLS_SITUACAO in (3,1) AND SituacaoExecucao.CSI_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosSituacaoColaborador) + @"))";

            if (filtrosPesquisa.SomenteMotoristaComOciosidade)
            {
                query += @" AND (CASE 
					WHEN EXISTS(SELECT Lancamento.CLS_DATA_FINAL FROM T_COLABORADOR_LANCAMENTO Lancamento where Lancamento.FUN_CODIGO = Motorista.FUN_CODIGO and Lancamento.CLS_SITUACAO = 3)  THEN DAY(GETDATE() -(SELECT TOP(1) Lancamento.CLS_DATA_FINAL FROM T_COLABORADOR_LANCAMENTO Lancamento where Lancamento.FUN_CODIGO = Motorista.FUN_CODIGO and Lancamento.CLS_SITUACAO = 3 ORDER BY Lancamento.CLS_DATA_FINAL DESC)) - 1
					WHEN Motorista.FUN_DATA_FECHAMENTO_ACERTO IS NOT NULL AND NOT EXISTS(SELECT A.FUN_CODIGO_MOTORISTA FROM T_ACERTO_DE_VIAGEM A WHERE A.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO AND A.ACV_SITUACAO = 1) THEN DAY(GETDATE() - Motorista.FUN_DATA_FECHAMENTO_ACERTO) - 1
					ELSE 0 END ) > 0";
            }

            if (!string.IsNullOrWhiteSpace(propOrdena))
                query += " order by " + propOrdena + " " + dirOrdena; 

            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Frota.ProgramacaoLogistica)));

            return nhQuery.List<Dominio.ObjetosDeValor.Embarcador.Frota.ProgramacaoLogistica>();
        }

        public int ContarConsultarProgramacaoLogistica(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaProgramacaoLogistica filtrosPesquisa)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                FROM T_FUNCIONARIO Motorista
                WHERE Motorista.FUN_TIPO = 'M' AND Motorista.FUN_STATUS = 'A'";

            if (filtrosPesquisa.TipoGuarita == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Entrada)
                query += @" AND (SELECT top(1) Guarita.GUA_ENTRADA_SAIDA 
						FROM T_GUARITA_TMS Guarita
						where Guarita.VEI_CODIGO in (SELECT veiculos.VEI_CODIGO FROM T_VEICULO veiculos WHERE veiculos.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO)						
						order by Guarita.GUA_DATA_SAIDA_ENTRADA desc) = 1 ";
            else if (filtrosPesquisa.TipoGuarita == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Saida)
                query += @" AND (SELECT top(1) Guarita.GUA_ENTRADA_SAIDA 
						FROM T_GUARITA_TMS Guarita
						where Guarita.VEI_CODIGO in (SELECT veiculos.VEI_CODIGO FROM T_VEICULO veiculos WHERE veiculos.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO)						
						order by Guarita.GUA_DATA_SAIDA_ENTRADA desc) = 2 ";

            if (filtrosPesquisa.EmManutencao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                query += @" AND (Motorista.FUN_CODIGO IN (SELECT Veiculo.FUN_CODIGO_MOTORISTA 
				    FROM T_VEICULO Veiculo
                    JOIN T_FROTA_ORDEM_SERVICO OrdemVeiculo on OrdemVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO and OrdemVeiculo.OSE_SITUACAO = 3
                    WHERE Veiculo.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO)
				    OR
				    Motorista.FUN_CODIGO IN (SELECT Veiculo.FUN_CODIGO_MOTORISTA 
				    FROM T_VEICULO Reboque
                    JOIN T_VEICULO_CONJUNTO Conjunto on Reboque.VEI_CODIGO = Conjunto.VEC_CODIGO_FILHO
                    JOIN T_FROTA_ORDEM_SERVICO OrdemReboque on OrdemReboque.VEI_CODIGO = Reboque.VEI_CODIGO and OrdemReboque.OSE_SITUACAO = 3
				    JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Conjunto.VEC_CODIGO_PAI
                    WHERE Conjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO)) ";
            else if (filtrosPesquisa.EmManutencao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                query += @" AND Motorista.FUN_CODIGO NOT IN (SELECT ISNULL(Veiculo.FUN_CODIGO_MOTORISTA,0) 
				    FROM T_VEICULO Veiculo
                    JOIN T_FROTA_ORDEM_SERVICO OrdemVeiculo on OrdemVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO and OrdemVeiculo.OSE_SITUACAO = 3
                    WHERE Veiculo.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO)
				    AND
				    Motorista.FUN_CODIGO NOT IN (SELECT ISNULL(Veiculo.FUN_CODIGO_MOTORISTA,0) 
				    FROM T_VEICULO Reboque
                    JOIN T_VEICULO_CONJUNTO Conjunto on Reboque.VEI_CODIGO = Conjunto.VEC_CODIGO_FILHO
                    JOIN T_FROTA_ORDEM_SERVICO OrdemReboque on OrdemReboque.VEI_CODIGO = Reboque.VEI_CODIGO and OrdemReboque.OSE_SITUACAO = 3
				    JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Conjunto.VEC_CODIGO_PAI
                    WHERE Conjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO) ";

            if (filtrosPesquisa.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio)
                query += " AND Motorista.FUN_TIPO_MOTORISTA = 1";
            else if (filtrosPesquisa.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro)
                query += " AND Motorista.FUN_TIPO_MOTORISTA = 2";

            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND Motorista.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista;

            if (filtrosPesquisa.CodigoProgramacaoAlocacao > 0)
                query += " AND Motorista.PMO_CODIGO IN (SELECT Prog.PMO_CODIGO FROM T_PROGRAMACAO_MOTORISTA Prog where Prog.PAL_CODIGO = " + filtrosPesquisa.CodigoProgramacaoAlocacao + ")"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigosTipoVeiculo.Count > 0)
            {
                var tiposVeiculos = string.Join(",", from o in filtrosPesquisa.CodigosTipoVeiculo select o.ToString());
                query += @" AND (Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic where Veic.MVC_CODIGO IN (" + tiposVeiculos + @"))
                    OR  Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic JOIN T_VEICULO_CONJUNTO Conj on Conj.VEC_CODIGO_PAI = Veic.VEI_CODIGO JOIN T_VEICULO Reb on Reb.VEI_CODIGO = Conj.VEC_CODIGO_FILHO where Reb.MVC_CODIGO IN (" + tiposVeiculos + @")))"; // SQL-INJECTION-SAFE
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += @" AND (Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic where Veic.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo + @")
                            OR  Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic JOIN T_VEICULO_CONJUNTO Conj on Conj.VEC_CODIGO_PAI = Veic.VEI_CODIGO JOIN T_VEICULO Reb on Reb.VEI_CODIGO = Conj.VEC_CODIGO_FILHO where Reb.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo + @"))"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigosPlotagemVeiculo.Count > 0)
            {
                var plotagensVeiculos = string.Join(",", filtrosPesquisa.CodigosPlotagemVeiculo);
                query += @" AND (Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic where Veic.VEI_TIPO_PLOTAGEM IN (" + plotagensVeiculos + @"))
                    OR  Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic JOIN T_VEICULO_CONJUNTO Conj on Conj.VEC_CODIGO_PAI = Veic.VEI_CODIGO JOIN T_VEICULO Reb on Reb.VEI_CODIGO = Conj.VEC_CODIGO_FILHO where Reb.VEI_TIPO_PLOTAGEM IN (" + plotagensVeiculos + @")))"; // SQL-INJECTION-SAFE
            }

            if (filtrosPesquisa.CodigosTipoCarroceria.Count > 0)
            {
                var tiposCarroceria = string.Join(",", filtrosPesquisa.CodigosTipoCarroceria);
                query += @" AND (Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic where Veic.MCA_CODIGO IN (" + tiposCarroceria + @"))
                    OR  Motorista.FUN_CODIGO IN (SELECT VEIC.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic JOIN T_VEICULO_CONJUNTO Conj on Conj.VEC_CODIGO_PAI = Veic.VEI_CODIGO JOIN T_VEICULO Reb on Reb.VEI_CODIGO = Conj.VEC_CODIGO_FILHO where Reb.MCA_CODIGO IN (" + tiposCarroceria + @")))"; // SQL-INJECTION-SAFE
            }

            if (filtrosPesquisa.EmViagem == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                query += " AND Motorista.FUN_CODIGO in (SELECT a.FUN_CODIGO_MOTORISTA FROM T_ACERTO_DE_VIAGEM a Where a.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO and a.ACV_SITUACAO = 1)";
            else if (filtrosPesquisa.EmViagem == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                query += " AND Motorista.FUN_CODIGO not in (SELECT a.FUN_CODIGO_MOTORISTA FROM T_ACERTO_DE_VIAGEM a Where a.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO and a.ACV_SITUACAO = 1)";

            if (filtrosPesquisa.SituacaoProgramacaoLogistica == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProgramacaoLogistica.EmCarga)
                query += @" AND Motorista.FUN_CODIGO IN (SELECT CargaMotorista.CAR_MOTORISTA FROM T_CARGA Carga 
                    JOIN T_CARGA_MOTORISTA CargaMotorista on CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO
                    WHERE CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO
                    AND Carga.CAR_SITUACAO NOT IN (13,18,11)) ";
            else if (filtrosPesquisa.SituacaoProgramacaoLogistica == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProgramacaoLogistica.Vazio)
                query += " AND Motorista.FUN_CODIGO IN (SELECT Veic.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic WHERE Veic.VEI_VAZIO = 1)";
            else if (filtrosPesquisa.SituacaoProgramacaoLogistica == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProgramacaoLogistica.NaoVazio)
                query += " AND Motorista.FUN_CODIGO IN (SELECT Veic.FUN_CODIGO_MOTORISTA FROM T_VEICULO Veic WHERE Veic.VEI_VAZIO = 0 or Veic.VEI_VAZIO IS NULL)";

            if (filtrosPesquisa.CodigosSituacaoColaborador.Count > 0)
                query += @" AND Motorista.FUN_CODIGO IN (SELECT SituacaoExecucao.FUN_CODIGO FROM T_COLABORADOR_LANCAMENTO SituacaoExecucao                
                            where SituacaoExecucao.CLS_SITUACAO in (3,1) AND SituacaoExecucao.CSI_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosSituacaoColaborador) + @"))";

            if (filtrosPesquisa.SomenteMotoristaComOciosidade)
            {
                query += @" AND (CASE 
					WHEN EXISTS(SELECT Lancamento.CLS_DATA_FINAL FROM T_COLABORADOR_LANCAMENTO Lancamento where Lancamento.FUN_CODIGO = Motorista.FUN_CODIGO and Lancamento.CLS_SITUACAO = 3)  THEN DAY(GETDATE() -(SELECT TOP(1) Lancamento.CLS_DATA_FINAL FROM T_COLABORADOR_LANCAMENTO Lancamento where Lancamento.FUN_CODIGO = Motorista.FUN_CODIGO and Lancamento.CLS_SITUACAO = 3 ORDER BY Lancamento.CLS_DATA_FINAL DESC)) - 1
					WHEN Motorista.FUN_DATA_FECHAMENTO_ACERTO IS NOT NULL AND NOT EXISTS(SELECT A.FUN_CODIGO_MOTORISTA FROM T_ACERTO_DE_VIAGEM A WHERE A.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO AND A.ACV_SITUACAO = 1) THEN DAY(GETDATE() - Motorista.FUN_DATA_FECHAMENTO_ACERTO) - 1
					ELSE 0 END ) > 0";
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public List<Dominio.Entidades.Usuario> ConsultarProgramacaoMotorista(int codigoMotorista, int codigoSituacao, int codigoAlocacao, int codigoEspecialidade, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaVeiculo = ConsultarProgramacaoVeiculo(codigoMotorista, codigoSituacao, codigoAlocacao, codigoEspecialidade);

            return consultaVeiculo
                         .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros).ToList();
        }

        public int ContarConsultarProgramacaoMotorista(int codigoMotorista, int codigoSituacao, int codigoAlocacao, int codigoEspecialidade)
        {
            var consultaVeiculo = ConsultarProgramacaoVeiculo(codigoMotorista, codigoSituacao, codigoAlocacao, codigoEspecialidade);

            return consultaVeiculo.Count();
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristasAlertarVencimentoCnh(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta filtrosPesquisa)
        {
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataVencimentoCnhAlertar = dataAtual.AddDays(filtrosPesquisa.DiasAlertarAntesVencimento);

            var consultaMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(o => (o.Tipo == "M") && (o.Status == "A") && o.DataVencimentoHabilitacao != null && o.DataVencimentoHabilitacao <= dataVencimentoCnhAlertar);

            if (!filtrosPesquisa.AlertarAposVencimento)
                consultaMotorista = consultaMotorista.Where(o => o.DataVencimentoHabilitacao >= dataAtual);

            if (filtrosPesquisa.DiasRepetirAlerta > 0)
            {
                DateTime dataVencimentoCnhRepetirAlerta = dataAtual.AddDays(-filtrosPesquisa.DiasRepetirAlerta);

                consultaMotorista = consultaMotorista.Where(o => (o.DataUltimoAlertaVencimentoCnh == null) || (o.DataUltimoAlertaVencimentoCnh <= dataVencimentoCnhRepetirAlerta));
            }
            else
                consultaMotorista = consultaMotorista.Where(o => o.DataUltimoAlertaVencimentoCnh == null);

            return consultaMotorista.ToList();
        }

        public List<Dominio.Entidades.Usuario> ConsultarPorSistema(string situacao, string sistema, List<int> codigosEmpresaUsuariosMultiCTe, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Tipo == "U" select obj;

            if (!string.IsNullOrWhiteSpace(situacao))
            {
                if (situacao == "Ativos")
                    result = result.Where(o => o.Status == "A");
                else if (situacao == "Inativos")
                    result = result.Where(o => o.Status == "I");
            }

            if (!string.IsNullOrWhiteSpace(sistema) && (sistema == "MultiEmbarcador" || sistema == "MultiCTe"))
            {
                if (sistema == "MultiEmbarcador")
                    result = result.Where(o => o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador);
                else if (sistema == "MultiCTe")
                {
                    if (codigosEmpresaUsuariosMultiCTe != null && codigosEmpresaUsuariosMultiCTe.Count > 0)
                        result = result.Where(o => o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao && codigosEmpresaUsuariosMultiCTe.Contains(o.Empresa.Codigo));
                    else
                        result = result.Where(o => o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao);
                }
            }
            else
            {
                if (codigosEmpresaUsuariosMultiCTe != null && codigosEmpresaUsuariosMultiCTe.Count > 0)
                    result = result.Where(o => (o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador) || (o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao && codigosEmpresaUsuariosMultiCTe.Contains(o.Empresa.Codigo)));
                else
                    result = result.Where(o => (o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador) || o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao);
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarPorSistema(string situacao, string sistema, List<int> codigosEmpresaUsuariosMultiCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = from obj in query where obj.Tipo == "U" select obj;

            if (!string.IsNullOrWhiteSpace(situacao))
            {
                if (situacao == "Ativos")
                    result = result.Where(o => o.Status == "A");
                else if (situacao == "Inativos")
                    result = result.Where(o => o.Status == "I");
            }


            if (!string.IsNullOrWhiteSpace(sistema) && (sistema == "MultiEmbarcador" || sistema == "MultiCTe"))
            {
                if (sistema == "MultiEmbarcador")
                    result = result.Where(o => o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador);
                else if (sistema == "MultiCTe")
                {
                    if (codigosEmpresaUsuariosMultiCTe != null && codigosEmpresaUsuariosMultiCTe.Count > 0)
                        result = result.Where(o => o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao && codigosEmpresaUsuariosMultiCTe.Contains(o.Empresa.Codigo));
                    else
                        result = result.Where(o => o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao);
                }
            }
            else
            {
                if (codigosEmpresaUsuariosMultiCTe != null && codigosEmpresaUsuariosMultiCTe.Count > 0)
                    result = result.Where(o => (o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador) || (o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao && codigosEmpresaUsuariosMultiCTe.Contains(o.Empresa.Codigo)));
                else
                    result = result.Where(o => (o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador) || o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao);
            }

            return result.Count();
        }

        public List<string> BuscarNomesMotoristasDisponibilidade()
        {
            IQueryable<Dominio.Entidades.Usuario> query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(o => o.Nome != null && o.Nome != "" && o.Status.Equals("A") && o.Tipo.Equals("M") && o.PossuiControleDisponibilidade);

            return query.Select(o => (o.Apelido != null && o.Apelido != "") ? o.Apelido : o.Nome).Distinct().ToList();
        }

        public IList<Dominio.Entidades.Usuario> BuscarTodosUsuarios(Dominio.Enumeradores.TipoAcesso tipoAcesso, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Usuario>();
            criteria.Add(Restrictions.Not(Restrictions.Eq("Tipo", "M")));
            criteria.Add(Restrictions.Eq("TipoAcesso", tipoAcesso));
            criteria.SetFirstResult(inicioRegistros);
            criteria.SetMaxResults(maximoRegistros);
            return criteria.List<Dominio.Entidades.Usuario>();
        }

        public int ContarTodosUsuarios(Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Usuario>();
            criteria.Add(Restrictions.Not(Restrictions.Eq("Tipo", "M")));
            criteria.Add(Restrictions.Eq("TipoAcesso", tipoAcesso));
            criteria.SetProjection(Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<Dominio.Entidades.Usuario> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Usuario>> BuscarPorCodigosAsync(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToListAsync(CancellationToken);
        }

        public List<string> BuscarDescricaoPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Usuario> query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(o => codigos.Contains(o.Codigo));
            List<Dominio.Entidades.Usuario> listaUsuario = query.ToList();
            return listaUsuario.Select(o => o.Descricao).ToList();
        }

        public Task<List<Dominio.Entidades.Usuario>> BuscarUsuarioResposavelChamadoNivelFilialAsync(int codigoFilial, EscalationList nivel, int codigoSetor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            query = query.Where(u => (u.Filial.Codigo == codigoFilial || u.Filial == null) && u.NivelEscalationList == nivel && u.NivelEscalationList != EscalationList.SemNivel);

            if (codigoSetor > 0)
                query = query.Where(c => c.Setor.Codigo == codigoSetor);

            return query.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Usuario>> BuscarUsuarioResposavelChamadoAsync(double codigosClientes, EscalationList nivel)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            query = query.Where(c => c.ClientesSetor.Any(o => o.CPF_CNPJ == codigosClientes) && c.NivelEscalationList == nivel);

            return query.ToListAsync(CancellationToken);
        }

        public List<string> BuscarEmailUsuariosPorSetor(int codigoSetor)
        {
            var consultaUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(o =>
                    o.Setor.Codigo == codigoSetor &&
                    (o.SituacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando || o.SituacaoColaborador == null) &&
                    o.Status == "A" &&
                    o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador
                ).Select(o => o.Email);

            return consultaUsuario.ToList();
        }

        public List<Dominio.Entidades.Usuario> BuscarListaUsuariosParaInativacaoAposXDiasSemAcesso(int diasParaInativarUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(o => o.UltimoAcesso.HasValue && o.Login != string.Empty && o.Status == "A" && o.UltimoAcesso.Value.Date < DateTime.Now.AddDays(-diasParaInativarUsuario).Date);

            return query.ToList();
        }

        public Dominio.Entidades.Usuario BuscarPorCodigoIntegracaoClienteFornecedor(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(o => o.ClienteFornecedor != null && o.ClienteFornecedor.CodigoIntegracao.Equals(codigoIntegracao));

            return query.FirstOrDefault();
        }
        #endregion

        #region Métodos Públicos Asíncronos
        public Task<List<Dominio.Entidades.Usuario>> BuscarMotoristasPorCPFeParteDoNomeAsync(CancellationToken cancellationToken, string cpf, string nomeUsuario, int? codigoEmpresa = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(usuario => usuario.Tipo == "M" && usuario.Nome.ToLower().Contains(nomeUsuario.ToLower()) && usuario.CPF.Equals(cpf));

            if (codigoEmpresa != null && codigoEmpresa > 0)
                query = query.Where(usuario => usuario.Empresa.Codigo == codigoEmpresa.Value);

            return query.ToListAsync(cancellationToken);
        }

        public Task<List<Dominio.Entidades.Usuario>> BuscarMotoristasPorCPFAsync(CancellationToken cancellationToken, string cpfCnpj, int? codigoEmpresa = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            query = query.Where(obj => obj.Tipo == "M" && obj.CPF.Equals(cpfCnpj));

            if (codigoEmpresa != null && codigoEmpresa > 0)
                query = query.Where(usuario => usuario.Empresa.Codigo == codigoEmpresa.Value);

            return query.ToListAsync(cancellationToken);
        }

        public Task<List<Dominio.Entidades.Usuario>> BuscarMotoristasPorCPFsAsync(List<string> cpfs, int? codigoEmpresa = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            query = query.Where(obj => obj.Tipo == "M" && cpfs.Contains(obj.CPF));

            if (codigoEmpresa != null && codigoEmpresa > 0)
                query = query.Where(usuario => usuario.Empresa.Codigo == codigoEmpresa.Value);

            return query.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Usuario>> BuscarMotoristasPorParteDoNomeAsync(CancellationToken cancellationToken, string nomeUsuario, int? codigoEmpresa = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(usuario => usuario.Tipo == "M" && usuario.Nome.ToLower().Contains(nomeUsuario.ToLower()));

            if (codigoEmpresa != null && codigoEmpresa > 0)
                query = query.Where(usuario => usuario.Empresa.Codigo == codigoEmpresa.Value);

            return query.ToListAsync(cancellationToken);
        }

        public Task<List<Dominio.Entidades.Usuario>> BuscarMotoristasPorCodigoAsync(CancellationToken cancellationToken, int codigo, int? codigoEmpresa = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(usuario => usuario.Tipo == "M" && usuario.Codigo == codigo);

            if (codigoEmpresa != null && codigoEmpresa > 0)
                query = query.Where(usuario => usuario.Empresa.Codigo == codigoEmpresa.Value);

            return query.ToListAsync(cancellationToken);
        }

        public Task<List<Dominio.Entidades.Usuario>> BuscarMotoristasPorCPFeCodigoAsync(CancellationToken cancellationToken, string cpf, int codigoUsuario, int? codigoEmpresa = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(usuario => usuario.Tipo == "M" && usuario.Codigo == codigoUsuario && usuario.CPF.Equals(cpf));

            if (codigoEmpresa != null && codigoEmpresa > 0)
                query = query.Where(usuario => usuario.Empresa.Codigo == codigoEmpresa.Value);

            return query.ToListAsync(cancellationToken);
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Usuario> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario filtrosPesquisa)
        {



            var criteria = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();


            var configuracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(UnitOfWork).BuscarConfiguracaoPadrao();

            if (configuracaoMotorista.HabilitarControleSituacaoColaboradorParaMotoristasTerceiros)
                criteria = criteria.Where(c => c.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro ||
                                               c.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio ||
                                               c.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos);
            else
                criteria = criteria.Where(c => c.TipoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro || c.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos);

            if (filtrosPesquisa.TipoUsuario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUsuario.Funcionarios)
                criteria = criteria.Where(c => c.ClienteTerceiro == null && c.ClienteFornecedor == null && c.Cliente == null);
            else if (filtrosPesquisa.TipoUsuario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUsuario.Pessoas)
                criteria = criteria.Where(c => c.ClienteTerceiro != null || c.ClienteFornecedor != null || c.Cliente == null);
            else if (filtrosPesquisa.SomenteFuncionarios)
                criteria = criteria.Where(c => c.ClienteTerceiro == null && c.ClienteFornecedor == null && c.Cliente == null);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                criteria = criteria.Where(c => c.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Nome))
                criteria = criteria.Where(c => c.Nome.Contains(filtrosPesquisa.Nome));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoPessoa))
                criteria = criteria.Where(c => c.TipoPessoa.Equals(filtrosPesquisa.TipoPessoa));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfCnpj))
                criteria = criteria.Where(c => c.CPF.Equals(filtrosPesquisa.CpfCnpj));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Usuario))
                criteria = criteria.Where(c => c.Login.Equals(filtrosPesquisa.Usuario));

            if (filtrosPesquisa.PerfilAcesso > 0)
                criteria = criteria.Where(c => c.PerfilAcesso.Codigo == filtrosPesquisa.PerfilAcesso);

            if (!filtrosPesquisa.IgnorarSituacaoMotorista && filtrosPesquisa.SituacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando)
                criteria = criteria.Where(c => c.SituacaoColaborador == filtrosPesquisa.SituacaoColaborador || c.SituacaoColaborador == null);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status))
                criteria = criteria.Where(c => c.Status.Equals(filtrosPesquisa.Status));

            if (filtrosPesquisa.TipoAcesso != null && filtrosPesquisa.TipoAcesso.HasValue)
                criteria = criteria.Where(c => c.TipoAcesso == filtrosPesquisa.TipoAcesso.Value);

            if (filtrosPesquisa.OcultarUsuarioMultiCTe)
                criteria = criteria.Where(c => c.CPF != c.Empresa.CNPJ && c.Senha != "multi@2015");

            if (!filtrosPesquisa.UsuarioMultisoftware)
                criteria = criteria.Where(c => c.UsuarioMultisoftware == false);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Tipo))
                criteria = criteria.Where(c => c.Tipo.Equals(filtrosPesquisa.Tipo));

            if (filtrosPesquisa.TipoCargoFuncionario != null && filtrosPesquisa.TipoCargoFuncionario.Count > 0)
                criteria = criteria.Where(c => c.Setor.TipoCargoFuncionario.Any(s => filtrosPesquisa.TipoCargoFuncionario.Contains(s)));

            if (filtrosPesquisa.CodigoSetor > 0)
                criteria = criteria.Where(c => c.Setor.Codigo == filtrosPesquisa.CodigoSetor);

            if (filtrosPesquisa.TipoComercial > 0)
                criteria = criteria.Where(c => c.TipoComercial == filtrosPesquisa.TipoComercial);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroMatricula))
                criteria = criteria.Where(c => c.NumeroMatricula == filtrosPesquisa.NumeroMatricula);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
            {
                criteria = criteria.Where(c => c.CodigoIntegracao == filtrosPesquisa.CodigoIntegracao);
            }

            if (filtrosPesquisa.Localidade > 0)
            {
                criteria = criteria.Where(c => c.Localidade.Codigo == filtrosPesquisa.Localidade);
            }





            return criteria;
        }

        private IQueryable<Dominio.Entidades.Usuario> ConsultarProgramacaoVeiculo(int codigoMotorista, int codigoSituacao, int codigoAlocacao, int codigoEspecialidade)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            consulta = consulta.Where(o => o.Status == "A" && o.Tipo == "M");

            if (codigoMotorista > 0)
                consulta = consulta.Where(o => o.Codigo == codigoMotorista);

            if (codigoSituacao > 0)
                consulta = consulta.Where(o => o.ProgramacaoMotorista.ProgramacaoSituacao.Codigo == codigoSituacao);

            if (codigoAlocacao > 0)
                consulta = consulta.Where(o => o.ProgramacaoMotorista.ProgramacaoAlocacao.Codigo == codigoAlocacao);

            if (codigoEspecialidade > 0)
                consulta = consulta.Where(o => o.ProgramacaoMotorista.ProgramacaoEspecialidade.Codigo == codigoEspecialidade);

            return consulta;
        }

        private IQueryable<Dominio.Entidades.Usuario> ConsultarMotoristaMobile(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotoristaMobile filtrosPesquisa)
        {
            var consultaMotoristaMobile = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(o => (o.Tipo == "M") && (o.Status == "A"));

            if (!filtrosPesquisa.UtilizarProgramacaoCarga)
                consultaMotoristaMobile = consultaMotoristaMobile.Where(o => o.CodigoMobile > 0);

            if (filtrosPesquisa.Transportador != null)
            {
                if (filtrosPesquisa.Transportador.Matriz?.FirstOrDefault() != null)
                    consultaMotoristaMobile = consultaMotoristaMobile.Where(o => (
                        o.Empresas.Contains(filtrosPesquisa.Transportador) ||
                        (o.Empresa.Codigo == filtrosPesquisa.Transportador.Codigo) ||
                        (o.Empresa.Codigo == filtrosPesquisa.Transportador.Matriz.FirstOrDefault().Codigo)
                    ));
                else
                    consultaMotoristaMobile = consultaMotoristaMobile.Where(o => o.Empresas.Contains(filtrosPesquisa.Transportador) || (o.Empresa.Codigo == filtrosPesquisa.Transportador.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Cpf))
                consultaMotoristaMobile = consultaMotoristaMobile.Where(o => o.CPF == filtrosPesquisa.Cpf);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Nome))
                consultaMotoristaMobile = consultaMotoristaMobile.Where(o => o.Nome.Contains(filtrosPesquisa.Nome));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PlacaVeiculo) || (filtrosPesquisa.CodigoModeloVeicularCarga > 0) || (filtrosPesquisa.CodigoCentroCarregamento > 0))
            {
                var consultaVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();

                if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                    consultaVeiculoMotorista = consultaVeiculoMotorista.Where(o => o.Veiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PlacaVeiculo))
                    consultaVeiculoMotorista = consultaVeiculoMotorista.Where(o => o.Veiculo.Placa.Contains(filtrosPesquisa.PlacaVeiculo));

                if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                {
                    var consultaCentroCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                        .Where(o => o.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

                    consultaVeiculoMotorista = consultaVeiculoMotorista.Where(o => (
                        from obj in consultaCentroCarregamento
                        where obj.Veiculos.Contains(o.Veiculo)
                        select o.Codigo
                    ).Contains(o.Codigo));
                }

                consultaMotoristaMobile = consultaMotoristaMobile.Where(o => (from obj in consultaVeiculoMotorista where obj.Motorista.Codigo == o.Codigo select obj.Motorista.Codigo).Contains(o.Codigo));
            }

            return consultaMotoristaMobile;
        }

        private string ObterSelectConsultaRelatorioUsuarios(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaUsuarios(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaUsuarios(filtrosPesquisa, ref where, ref groupBy, ref joins);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaUsuarios(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_FUNCIONARIO Usuario ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaUsuarios(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Nome":
                    if (!select.Contains(" Nome "))
                    {
                        select += "Usuario.FUN_NOME Nome, ";
                        groupBy += "Usuario.FUN_NOME, ";
                    }
                    break;
                case "CPFFormatado":
                    if (!select.Contains(" CPF "))
                    {
                        select += "Usuario.FUN_CPF CPF, ";
                        groupBy += "Usuario.FUN_CPF, ";
                    }
                    break;
                case "RG":
                    if (!select.Contains(" RG "))
                    {
                        select += "Usuario.FUN_RG RG, ";
                        groupBy += "Usuario.FUN_RG, ";
                    }
                    break;
                case "Telefone":
                    if (!select.Contains(" Telefone "))
                    {
                        select += "Usuario.FUN_FONE Telefone, ";
                        groupBy += "Usuario.FUN_FONE, ";
                    }
                    break;
                case "DataNascimentoFormatada":
                    if (!select.Contains(" DataNascimento "))
                    {
                        select += "Usuario.FUN_DATANASC DataNascimento, ";
                        groupBy += "Usuario.FUN_DATANASC, ";
                    }
                    break;
                case "DataAdmissaoFormatada":
                    if (!select.Contains(" DataAdmissao "))
                    {
                        select += "Usuario.FUN_DATAADMISAO DataAdmissao, ";
                        groupBy += "Usuario.FUN_DATAADMISAO, ";
                    }
                    break;
                case "Email":
                    if (!select.Contains(" Email "))
                    {
                        select += "Usuario.FUN_EMAIL Email, ";
                        groupBy += "Usuario.FUN_EMAIL, ";
                    }
                    break;
                case "Salario":
                    if (!select.Contains(" Salario "))
                    {
                        select += "Usuario.FUN_SALARIO Salario, ";
                        groupBy += "Usuario.FUN_SALARIO, ";
                    }
                    break;
                case "Cidade":
                    if (!select.Contains(" Cidade "))
                    {
                        select += "CONCAT(Localidade.LOC_DESCRICAO, ' - ', Localidade.UF_SIGLA) Cidade, ";
                        groupBy += "Localidade.LOC_DESCRICAO, Localidade.UF_SIGLA, ";

                        if (!joins.Contains(" Localidade "))
                            joins += " LEFT JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Usuario.LOC_CODIGO ";
                    }
                    break;
                case "Endereco":
                    if (!select.Contains(" Endereco "))
                    {
                        select += "Usuario.FUN_ENDERECO Endereco, ";
                        groupBy += "Usuario.FUN_ENDERECO, ";
                    }
                    break;
                case "Complemento":
                    if (!select.Contains(" Complemento "))
                    {
                        select += "Usuario.FUN_COMPLEMENTO Complemento, ";
                        groupBy += "Usuario.FUN_COMPLEMENTO, ";
                    }
                    break;
                case "Login":
                    if (!select.Contains(" Login "))
                    {
                        select += "Usuario.FUN_LOGIN Login, ";
                        groupBy += "Usuario.FUN_LOGIN, ";
                    }
                    break;
                case "AcessoSistemaFormatado":
                    if (!select.Contains(" AcessoSistema "))
                    {
                        select += "Usuario.FUN_USUARIO_ACESSO_BLOQUEADO AcessoSistema, ";
                        groupBy += "Usuario.FUN_USUARIO_ACESSO_BLOQUEADO, ";
                    }
                    break;
                case "Situacao":
                    if (!select.Contains(" Status "))
                    {
                        select += "Usuario.FUN_STATUS Status, ";

                        if (!groupBy.Contains("Usuario.FUN_STATUS, "))
                            groupBy += "Usuario.FUN_STATUS, ";
                    }
                    break;
                case "PerfilAcesso":
                    if (!select.Contains(" PerfilAcesso "))
                    {
                        select += "PerfilAcesso.PAC_DESCRICAO PerfilAcesso, ";
                        groupBy += "PerfilAcesso.PAC_DESCRICAO, ";

                        if (!joins.Contains(" PerfilAcesso "))
                            joins += " LEFT JOIN T_PERFIL_ACESSO PerfilAcesso on PerfilAcesso.PAC_CODIGO = Usuario.PAC_CODIGO ";
                    }
                    break;

                case "SituacaoSenha":
                    if (!select.Contains(" SituacaoSenha, "))
                    {
                        select += @"case when isnull((select top 1 PLS_PRAZO_EXPIRA_SENHA from T_POLITICA_SENHA order by PLS_CODIGO desc), 0) > 0 and 
	                                        Usuario.FUN_DATA_ULTIMA_ALTERACAO_SENHA_OBRIGATORIA + (select top 1 PLS_PRAZO_EXPIRA_SENHA from T_POLITICA_SENHA order by PLS_CODIGO desc) < GETDATE() then 'Expirado'
	                                     when Usuario.FUN_ULTIMO_ACESSO IS null then 'Nunca Logado'
	                                     when Usuario.FUN_STATUS <> 'A' then 'Senha Inativa'
	                                else 'Senha Ativa' 
                                    end SituacaoSenha, ";

                        if (!groupBy.Contains("Usuario.FUN_DATA_ULTIMA_ALTERACAO_SENHA_OBRIGATORIA, "))
                            groupBy += "Usuario.FUN_DATA_ULTIMA_ALTERACAO_SENHA_OBRIGATORIA, ";
                        if (!groupBy.Contains("Usuario.FUN_ULTIMO_ACESSO, "))
                            groupBy += "Usuario.FUN_ULTIMO_ACESSO, ";
                        if (!groupBy.Contains("Usuario.FUN_STATUS, "))
                            groupBy += "Usuario.FUN_STATUS, ";
                    }
                    break;
                case "AposentadoriaFormatada":
                    if (!select.Contains(" Aposentadoria "))
                    {
                        select += "Usuario.FUN_APOSENTADORIA Aposentadoria, ";
                        groupBy += "Usuario.FUN_APOSENTADORIA, ";
                    }
                    break;
                case "Filial":
                    if (!select.Contains(" Filial "))
                    {
                        select += "Filial.FIL_DESCRICAO Filial, ";
                        groupBy += "Filial.FIL_DESCRICAO, ";

                        joins += " LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Usuario.FIL_CODIGO ";
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "Usuario.FUN_OBSERVACAO Observacao, ";
                        if (!groupBy.Contains("Usuario.FUN_OBSERVACAO"))
                            groupBy += "Usuario.FUN_OBSERVACAO, ";
                    }
                    break;

                case "DataCadastroFormatada":
                    if (!select.Contains(" DataCadastro "))
                    {
                        select += "Usuario.FUN_DATA_CADASTRO DataCadastro, ";
                        groupBy += "Usuario.FUN_DATA_CADASTRO, ";
                    }
                    break;

                case "UltimoAcessoFormatada":
                    if (!select.Contains(" UltimoAcesso "))
                    {
                        select += "Usuario.FUN_ULTIMO_ACESSO UltimoAcesso, ";

                        if (!groupBy.Contains("Usuario.FUN_ULTIMO_ACESSO, "))
                            groupBy += "Usuario.FUN_ULTIMO_ACESSO, ";
                    }

                    break;

                case "DataUltimaAlteracaoSenhaFormatada":
                    if (!select.Contains(" DataUltimaAlteracaoSenha, "))
                    {
                        select += "Usuario.FUN_DATA_ULTIMA_ALTERACAO_SENHA_OBRIGATORIA DataUltimaAlteracaoSenha, ";

                        if (!groupBy.Contains("Usuario.FUN_DATA_ULTIMA_ALTERACAO_SENHA_OBRIGATORIA, "))
                            groupBy += "Usuario.FUN_DATA_ULTIMA_ALTERACAO_SENHA_OBRIGATORIA, ";
                    }
                    break;

                case "DataDemissaoFormatada":
                    if (!select.Contains(" DataDemissao, "))
                    {
                        select += "Usuario.FUN_DATA_DEMISSAO DataDemissao, ";
                        groupBy += "Usuario.FUN_DATA_DEMISSAO, ";
                    }
                    break;

                case "HoraInicialAcesso":
                    if (!select.Contains(" HoraInicialAcesso, "))
                    {
                        select += @"FORMAT(Usuario.FUN_HORA_INICIAL_ACESSO, N'hh\:mm') HoraInicialAcesso, ";
                        groupBy += "Usuario.FUN_HORA_INICIAL_ACESSO, ";
                    }
                    break;

                case "HoraFinalAcesso":
                    if (!select.Contains(" HoraFinalAcesso, "))
                    {
                        select += @"FORMAT(Usuario.FUN_HORA_FINAL_ACESSO, N'hh\:mm') HoraFinalAcesso, ";
                        groupBy += "Usuario.FUN_HORA_FINAL_ACESSO, ";
                    }
                    break;

                case "UsuarioAdministradorFormatado":
                    if (!select.Contains(" UsuarioAdministrador "))
                    {
                        select += "Usuario.FUN_USUARIO_ADMINISTRADOR UsuarioAdministrador, ";
                        groupBy += "Usuario.FUN_USUARIO_ADMINISTRADOR, ";
                    }
                    break;

                case "Setor":
                    if (!select.Contains(" Setor "))
                    {
                        select += "Setor.SET_DESCRICAO Setor, ";
                        groupBy += "Setor.SET_DESCRICAO, ";

                        joins += " LEFT JOIN T_SETOR Setor ON Setor.SET_CODIGO = Usuario.SET_CODIGO ";
                    }
                    break;

                case "PermiteAuditarDescricao":
                    if (!select.Contains(" PermiteAuditarDescricao "))
                    {
                        select += "CASE Usuario.FUN_PERMITE_AUDITAR WHEN 1 THEN 'Sim' ELSE 'Não' END AS PermiteAuditarDescricao,";
                        groupBy += "Usuario.FUN_PERMITE_AUDITAR,";
                    }
                    break;


                default:
                    if (!count && propriedade.Contains("QuantidadeEPI"))
                    {
                        select += @"(select SUM(funcionarioEPI.FEP_QUANTIDADE) from T_FUNCIONARIO_EPI funcionarioEPI
                                    where funcionarioEPI.FUN_CODIGO = Usuario.FUN_CODIGO and funcionarioEPI.EPI_CODIGO = " + codigoDinamico + ") " + propriedade + ", ";

                        if (!groupBy.Contains("Usuario.FUN_CODIGO"))
                            groupBy += "Usuario.FUN_CODIGO, ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaUsuarios(Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa, ref string where, ref string groupBy, ref string joins)
        {
            string pattern = "yyyy-MM-dd";

            where += " AND (Usuario.FUN_TIPO_MOTORISTA <> 2 or Usuario.FUN_TIPO_MOTORISTA is null)";
            //where += " AND (Usuario.FUN_TIPO = 'U' or (Usuario.FUN_SENHA is not null and Usuario.FUN_TIPO = 'M')) ";//Na pesquisa do cadastro de usuário não possui essa condição, deixado igual lá

            //if (filtrosPesquisa.tipoServicoMultisoftware.HasValue && filtrosPesquisa.tipoServicoMultisoftware.Value == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            //    where += " AND (Usuario.CLI_CGCCPF IS NULL AND Usuario.CLI_FORNECEDOR IS NULL AND Usuario.CLI_CLIENTE IS NULL) ";

            if (filtrosPesquisa.DataCadastroInicial != DateTime.MinValue)
                where += " AND CAST(Usuario.FUN_DATA_CADASTRO AS DATE) >= '" + filtrosPesquisa.DataCadastroInicial.ToString(pattern) + "'";
            if (filtrosPesquisa.DataCadastroFinal != DateTime.MinValue)
                where += " AND CAST(Usuario.FUN_DATA_CADASTRO AS DATE) <= '" + filtrosPesquisa.DataCadastroFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.UltimoAcessoInicial != DateTime.MinValue)
                where += " AND CAST(Usuario.FUN_ULTIMO_ACESSO AS DATE) >= '" + filtrosPesquisa.UltimoAcessoInicial.ToString(pattern) + "'";
            if (filtrosPesquisa.UltimoAcessoFinal != DateTime.MinValue)
                where += " AND CAST(Usuario.FUN_ULTIMO_ACESSO AS DATE) <= '" + filtrosPesquisa.UltimoAcessoFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.CodigoLocalidade > 0)
                where += " AND Usuario.LOC_CODIGO = " + filtrosPesquisa.CodigoLocalidade.ToString();

            if (filtrosPesquisa.CodigoPerfilAcesso > 0)
                where += " AND Usuario.PAC_CODIGO = " + filtrosPesquisa.CodigoPerfilAcesso.ToString();

            if (filtrosPesquisa.Operador.HasValue)
                where += " AND Usuario.FUN_CODIGO " + (filtrosPesquisa.Operador.Value ? " IN " : " NOT IN ") + " (SELECT FUN_CODIGO FROM T_OPERADOR_LOGISTICA WHERE OPL_ATIVO = 1)";

            if (filtrosPesquisa.AcessoSistema.HasValue)
                where += " AND Usuario.FUN_USUARIO_ACESSO_BLOQUEADO = " + (filtrosPesquisa.AcessoSistema.Value ? "1" : "0");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status))
                where += " AND Usuario.FUN_STATUS = '" + filtrosPesquisa.Status + "'";

            if (filtrosPesquisa.Ambiente.HasValue)
                where += " AND Usuario.FUN_AMBIENTE = " + ((int)filtrosPesquisa.Ambiente.Value).ToString();

            if (filtrosPesquisa.SituacaoColaborador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Todos)
                where += " AND Usuario.FUN_SITUACAO_COLABORADOR = " + filtrosPesquisa.SituacaoColaborador.ToString("D");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where += " AND Usuario.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (!filtrosPesquisa.UsuarioMultisoftware)
                where += " AND (Usuario.FUN_USUARIO_MULTISOFTWARE = 0 OR Usuario.FUN_USUARIO_MULTISOFTWARE IS NULL)";

            if (filtrosPesquisa.Aposentadoria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Aposentadoria.Todos)
                where += " AND Usuario.FUN_APOSENTADORIA = " + filtrosPesquisa.Aposentadoria.ToString("D");

            if (filtrosPesquisa.TipoUsuario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUsuario.Todos)
            {
                if (filtrosPesquisa.TipoUsuario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUsuario.Funcionarios)
                    where += "  AND ((Usuario.CLI_CGCCPF IS NULL AND Usuario.CLI_FORNECEDOR IS NULL AND Usuario.CLI_CLIENTE IS NULL) OR Usuario.FUN_FISJUR = 'F') ";
                else if (filtrosPesquisa.TipoUsuario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUsuario.Pessoas)
                    where += "  AND ((Usuario.CLI_CGCCPF IS NOT NULL OR Usuario.CLI_FORNECEDOR IS NOT NULL OR Usuario.CLI_CLIENTE IS NULL) AND Usuario.FUN_FISJUR <> 'F') ";
            }

            if (filtrosPesquisa.SomenteUsuariosAtivo)
                where += " AND (Usuario.FUN_LOGIN <> '' AND Usuario.FUN_LOGIN IS NOT NULL AND Usuario.FUN_SENHA <> '' AND Usuario.FUN_SENHA IS NOT NULL) ";

            if (filtrosPesquisa.Ambiente.HasValue)
                where += $" AND Usuario.FUN_AMBIENTE = {(int)filtrosPesquisa.Ambiente.Value}";
        }

        #endregion
    }
}