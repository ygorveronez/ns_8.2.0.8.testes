using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Atendimento
{
    public class Atendimento : RepositorioBase<Dominio.Entidades.Embarcador.Atendimento.Atendimento>
    {
        public Atendimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Atendimento.Atendimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.Atendimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Atendimento.Atendimento BuscarPorCodigoEEmpresa(int codigo, int codigoEmpresaPai)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.Atendimento>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresaPai select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Atendimento.Atendimento> Consultar(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusAberto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusEmAndamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusCancelado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusFinalizado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade, int empresaFilhio, int codigoFuncionario, int empresa, string motivo, string titulo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaAtendimento = Consultar(tipoServico, numeroInicial, numeroFinal, dataInicial, dataFinal, statusAberto, statusEmAndamento, statusCancelado, statusFinalizado, prioridade, empresaFilhio, codigoFuncionario, empresa, motivo, titulo);

            return ObterLista(consultaAtendimento, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusAberto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusEmAndamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusCancelado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusFinalizado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade, int empresaFilhio, int codigoFuncionario, int empresa, string motivo, string titulo)
        {
            var consultaAtendimento = Consultar(tipoServico, numeroInicial, numeroFinal, dataInicial, dataFinal, statusAberto, statusEmAndamento, statusCancelado, statusFinalizado, prioridade, empresaFilhio, codigoFuncionario, empresa, motivo, titulo);

            return consultaAtendimento.Count();
        }

        public int BuscarUltimoNumero(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.Atendimento>();
            var result = from obj in query select obj;

            if ((tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin) && codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (result.Count() > 0)
                return result.Max(obj => obj.Numero) + 1;
            else
                return 1;
        }

        public int BuscarProximoNumero(int codigoEmpresaPai)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.Atendimento>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresaPai select obj;

            if (result.Count() > 0)
                return result.Max(obj => obj.Numero) + 1;
            else
                return 1;
        }

        public List<Dominio.Entidades.Embarcador.Atendimento.Atendimento> ConsultarAdminCTe(int empresaPai, int codigoUsuario, int empresa, string nomeEmpresa, DateTime dataInicial, DateTime dataFinal, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento? status, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarAdminCTe(empresaPai, codigoUsuario, empresa, nomeEmpresa, dataInicial, dataFinal, descricao, status);

            return result.OrderByDescending(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAdminCTe(int empresaPai, int codigoUsuario, int empresa, string nomeEmpresa, DateTime dataInicial, DateTime dataFinal, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento? status)
        {
            var result = _ConsultarAdminCTe(empresaPai, codigoUsuario, empresa, nomeEmpresa, dataInicial, dataFinal, descricao, status);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Atendimento.Atendimento> _ConsultarAdminCTe(int empresaPai, int codigoUsuario, int empresa, string nomeEmpresa, DateTime dataInicial, DateTime dataFinal, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.Atendimento>();

            var result = from obj in query where obj.Empresa.Codigo == empresaPai && obj.AtendimentoTipo.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo.Suporte select obj;

            if (codigoUsuario > 0)
                result = result.Where(o => o.Funcionario.Codigo == codigoUsuario);

            if (empresa > 0)
                result = result.Where(o => o.EmpresaFilho.Codigo == empresa);

            if (!string.IsNullOrWhiteSpace(nomeEmpresa))
                result = result.Where(o => o.EmpresaFilho.RazaoSocial.Contains(nomeEmpresa));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicial.Value.Date > dataInicial.Date);
            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataInicial.Value.Date <= dataFinal.Date);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.ObservacaoSuporte.Contains(descricao));

            if (status.HasValue)
                result = result.Where(o => o.Status == status);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Atendimento.Atendimento> ConsultarAdminCTeEmissao(int empresaPai, int codigoUsuario, int empresa, string nomeEmpresa, DateTime dataInicial, DateTime dataFinal, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento? status, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarAdminCTeEmissao(empresaPai, codigoUsuario, empresa, nomeEmpresa, dataInicial, dataFinal, descricao, status);

            return result.OrderByDescending(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAdminCTeEmissao(int empresaPai, int codigoUsuario, int empresa, string nomeEmpresa, DateTime dataInicial, DateTime dataFinal, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento? status)
        {
            var result = _ConsultarAdminCTeEmissao(empresaPai, codigoUsuario, empresa, nomeEmpresa, dataInicial, dataFinal, descricao, status);

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioAtendimento> RelatorioAtendimentos(int empresaPai, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo tipoRelatorio, DateTime dataInicial, DateTime dataFinal, int empresa, int tipoAtendimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? satisfacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento? status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistema? sistema)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.Atendimento>();

            var result = from obj in query
                         where 
                         obj.Empresa.Codigo == empresaPai 
                         && obj.AtendimentoTipo.Tipo == tipoRelatorio
                         select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicial.Value.Date > dataInicial.Date);
            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataInicial.Value.Date <= dataFinal.Date);
            
            if (empresa > 0)
                result = result.Where(o => o.EmpresaFilho.Codigo == empresa);

            if (usuario > 0)
                result = result.Where(o => o.Funcionario.Codigo == usuario);

            if (tipoAtendimento > 0)
                result = result.Where(o => o.AtendimentoTipo.Codigo == tipoAtendimento);

            if (satisfacao.HasValue)
                result = result.Where(o => o.NivelSatisfacao == satisfacao.Value);

            if (status.HasValue)
                result = result.Where(o => o.Status == status.Value);

            if (sistema.HasValue)
                result = result.Where(o => o.TipoSistema == sistema.Value);

            return result
                .Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioAtendimento() {
                    Usuario = o.Funcionario.Nome,
                    Numero = o.Numero,
                    Empresa = o.Empresa.RazaoSocial + " - " + o.Empresa.CNPJ, 
                    TipoAtendimento = o.AtendimentoTipo.Descricao,
                    Sistema = o.TipoSistema, 
                    Situacao = o.Status,
                    Satisfacao = o.NivelSatisfacao,
                    TipoContato = o.TipoContato,
                    Data = o.DataInicial != null ? o.DataInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Contato = o.ContatoAtendimento,
                    Descricao = o.Observacao,
                    Observacao = o.ObservacaoSuporte
                })
                .ToList();
        }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Atendimento.Atendimento> _ConsultarAdminCTeEmissao(int empresaPai, int codigoUsuario, int empresa, string nomeEmpresa, DateTime dataInicial, DateTime dataFinal, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.Atendimento>();

            var result = from obj in query where obj.Empresa.Codigo == empresaPai && obj.AtendimentoTipo.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo.Emissao select obj;

            if (codigoUsuario > 0)
                result = result.Where(o => o.Funcionario.Codigo == codigoUsuario);

            if (empresa > 0)
                result = result.Where(o => o.EmpresaFilho.Codigo == empresa);

            if (!string.IsNullOrWhiteSpace(nomeEmpresa))
                result = result.Where(o => o.EmpresaFilho.RazaoSocial.Contains(nomeEmpresa));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicial.Value.Date > dataInicial.Date);
            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataInicial.Value.Date <= dataFinal.Date);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.ObservacaoSuporte.Contains(descricao));

            if (status.HasValue)
                result = result.Where(o => o.Status == status);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Atendimento.Atendimento> Consultar(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusAberto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusEmAndamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusCancelado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusFinalizado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade, int empresaFilhio, int codigoFuncionario, int empresa, string motivo, string titulo)
        {
            var consultaAtendimentoTarefa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa>();

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                consultaAtendimentoTarefa = consultaAtendimentoTarefa.Where(atendimentoTarefa => atendimentoTarefa.Data >= dataInicial && atendimentoTarefa.Data <= dataFinal.AddDays(1));
            else if (dataInicial > DateTime.MinValue)
                consultaAtendimentoTarefa = consultaAtendimentoTarefa.Where(atendimentoTarefa => atendimentoTarefa.Data >= dataInicial && atendimentoTarefa.Data <= dataInicial.AddDays(1));
            else if (dataFinal > DateTime.MinValue)
                consultaAtendimentoTarefa = consultaAtendimentoTarefa.Where(atendimentoTarefa => atendimentoTarefa.Data >= dataFinal && atendimentoTarefa.Data <= dataFinal.AddDays(1));

            if ((int)prioridade > 0)
                consultaAtendimentoTarefa = consultaAtendimentoTarefa.Where(atendimentoTarefa => atendimentoTarefa.Prioridade == prioridade);

            if (!string.IsNullOrWhiteSpace(motivo))
                consultaAtendimentoTarefa = consultaAtendimentoTarefa.Where(atendimentoTarefa => atendimentoTarefa.MotivoProblema.Contains(motivo));

            if (!string.IsNullOrWhiteSpace(titulo))
                consultaAtendimentoTarefa = consultaAtendimentoTarefa.Where(atendimentoTarefa => atendimentoTarefa.Titulo.Contains(titulo));

            var consultaAtendimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.Atendimento>()
                .Where(atendimento => consultaAtendimentoTarefa.Any(atendimentoTarefa => atendimentoTarefa.Atendimento.Codigo == atendimento.Codigo));

            if (numeroInicial > 0 && numeroFinal > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Numero >= numeroInicial && atendimento.Numero <= numeroInicial);
            else if (numeroInicial > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Numero == numeroFinal);
            else if (numeroFinal > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Numero == numeroFinal);

            if (((int)statusAberto > 0 || (int)statusEmAndamento > 0) && (int)statusCancelado > 0 && (int)statusFinalizado > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto || atendimento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.EmAndamento || atendimento.Status == statusCancelado || atendimento.Status == statusFinalizado);
            else if (((int)statusAberto > 0 || (int)statusEmAndamento > 0) && (int)statusCancelado > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto || atendimento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.EmAndamento || atendimento.Status == statusCancelado);
            else if (((int)statusAberto > 0 || (int)statusEmAndamento > 0) && (int)statusFinalizado > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto || atendimento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.EmAndamento || atendimento.Status == statusFinalizado);
            else if ((int)statusCancelado > 0 && (int)statusFinalizado > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Status == statusCancelado || atendimento.Status == statusFinalizado);
            else if ((int)statusAberto > 0 || (int)statusEmAndamento > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto || atendimento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.EmAndamento);
            else if ((int)statusCancelado > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Status == statusCancelado);
            else if ((int)statusFinalizado > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Status == statusFinalizado);

            if (empresaFilhio > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.EmpresaFilho.Codigo == empresaFilhio);

            if (codigoFuncionario > 0)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Funcionario.Codigo == codigoFuncionario);

            if (tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                consultaAtendimento = consultaAtendimento.Where(atendimento => atendimento.Empresa.Codigo == empresa);

            return consultaAtendimento;
        }

        #endregion Métodos Privados
    }
}
