using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class OrdemDeCompra : RepositorioBase<Dominio.Entidades.OrdemDeCompra>
    {
        public OrdemDeCompra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.OrdemDeCompra BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OrdemDeCompra>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.OrdemDeCompra BuscarPorNumero(int codigoEmpresa, int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OrdemDeCompra>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero select obj;
            return result.FirstOrDefault();
        }

        public int ObterUltimoNumero(int codigoEmpresa, Dominio.Enumeradores.TipoOrdemDeCompra? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OrdemDeCompra>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoOrdemDeCompra == tipo select obj;

            int? retorno = result.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value : 0;
        }

        private IQueryable<Dominio.Entidades.OrdemDeCompra> _ConsultarOrdemDeCompra(int codigoEmpresa, int numero, string descricao, DateTime data, string solicitante, Dominio.Enumeradores.TipoOrdemDeCompra tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OrdemDeCompra>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoOrdemDeCompra == tipo select obj;

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (data != DateTime.MinValue)
                result = result.Where(o => o.Data >= data && o.Data < data.AddDays(1));

            if (!string.IsNullOrWhiteSpace(solicitante))
                result = result.Where(o => o.Solicitante != null ? o.Solicitante.Nome.Contains(solicitante) : o.NomeSolicitante.Contains(solicitante));

            return result;
        }

        public List<Dominio.Entidades.OrdemDeCompra> ConsultarOrdemDeCompra(int codigoEmpresa, int numero, string descricao, DateTime data, string solicitante, Dominio.Enumeradores.TipoOrdemDeCompra tipo, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarOrdemDeCompra(codigoEmpresa, numero, descricao, data, solicitante, tipo);

            result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaOrdemDeCompra(int codigoEmpresa, int numero, string descricao, DateTime data, string solicitante, Dominio.Enumeradores.TipoOrdemDeCompra tipo)
        {
            var result = _ConsultarOrdemDeCompra(codigoEmpresa, numero, descricao, data, solicitante, tipo);

            return result.Count();
        }

        public bool DuplicidadeDeNumero(int codigoEmpresa, int numero, int codigo, Dominio.Enumeradores.TipoOrdemDeCompra? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OrdemDeCompra>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero && obj.TipoOrdemDeCompra == tipo select obj;

            if (codigo > 0)
                result = result.Where(o => o.Codigo != codigo);

            return result.Count() > 0;
        }

        public List<Dominio.ObjetosDeValor.Relatorios.OrdemDeCompra> EspelhoBuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OrdemDeCompra>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.OrdemDeCompra()
            {
                Codigo = o.Codigo,
                Data = o.Data,
                Numero = o.Numero,
                Servico = o.Servico,
                Solicitante = o.Solicitante != null ? o.Solicitante.Nome : o.NomeSolicitante,
                Setor = o.Setor,
                Veiculo = o.Veiculo != null ? o.Veiculo.Placa : string.Empty,
                KilometragemVeiculo = o.Veiculo != null ? o.Veiculo.KilometragemAtual : 0,
                ModeloVeiculo = o.ModeloVeiculo != null ? o.ModeloVeiculo.Descricao : string.Empty,
                Fornecedor = o.Fornecedor != null ? o.Fornecedor.Nome : string.Empty,
                Descricao = o.Descricao,
            }).ToList();
        }
    }
}
