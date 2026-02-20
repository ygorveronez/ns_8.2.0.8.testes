using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Atendimento
{
    public class AtendimentoTipo : RepositorioBase<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo>
    {
        public AtendimentoTipo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo TipoAtendimentoPadrao(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo>();
            var result = from obj in query where obj.Empresa.Codigo == empresa && obj.TipoPadrao == true && obj.Status == true && obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo.Suporte select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo TipoAtendimentoPadraoEmissao(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo>();
            var result = from obj in query where obj.Empresa.Codigo == empresa && obj.TipoPadrao == true && obj.Status == true && obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo.Emissao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo> Consultar(int empresaPai, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int empresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status == true);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status == false);

            if (tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                if (empresaPai > 0 && tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    result = result.Where(obj => obj.Empresa.Codigo == empresaPai);
                else
                    result = result.Where(obj => obj.Empresa.Codigo == empresa);
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int empresaPai, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status == true);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status == false);

            if (tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                if (empresaPai > 0 && tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    result = result.Where(obj => obj.Empresa.Codigo == empresaPai);
                else
                    result = result.Where(obj => obj.Empresa.Codigo == empresa);
            }

            return result.Count();
        }




        private IQueryable<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo> _ConsultarMultiCTe(int empresaPai, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo>();

            var result = from obj in query
                         where obj.Empresa.Codigo == empresaPai && obj.Tipo == tipo select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status == true);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status == false);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo> ConsultarMultiCTe(int empresaPai, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo tipo, int inicioRegistros, int maximoRegistros)
        {
            var query = _ConsultarMultiCTe(empresaPai, descricao, status, tipo);


            return query
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsultaMultiCTe(int empresaPai, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo tipo)
        {
            var query = _ConsultarMultiCTe(empresaPai, descricao, status, tipo);

            return query
                .Count();
        }
    }
}
