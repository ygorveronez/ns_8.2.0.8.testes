using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class ImpostoContratoFrete : RepositorioBase<Dominio.Entidades.ImpostoContratoFrete>, Dominio.Interfaces.Repositorios.ImpostoContratoFrete
    {
        public ImpostoContratoFrete(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ImpostoContratoFrete BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpostoContratoFrete>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ImpostoContratoFrete BuscarPorEmpresaVigencia(int codigoEmpresa, DateTime dataReferencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpostoContratoFrete>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && (obj.DataVigenciaInicio == null || dataReferencia >= obj.DataVigenciaInicio) && (obj.DataVigenciaFim == null || dataReferencia <= obj.DataVigenciaFim)  select obj;

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.ImpostoContratoFrete BuscarPorTerceiro(double cpfCnpjTerceiro, int codigoTipoTerceiro, RegimeTributario? regimeTributario, string tipoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpostoContratoFrete>();

            query = query.Where(obj => obj.Empresa == null);

            if (cpfCnpjTerceiro > 0D)
                query = query.Where(obj => obj.Terceiro.CPF_CNPJ == cpfCnpjTerceiro);
            else
                query = query.Where(obj => obj.Terceiro == null);

            if (codigoTipoTerceiro > 0)
                query = query.Where(obj => obj.TipoTerceiro.Codigo == codigoTipoTerceiro);
            else
                query = query.Where(obj => obj.TipoTerceiro == null);

            if (regimeTributario.HasValue && regimeTributario.Value != RegimeTributario.NaoInformado)
                query = query.Where(obj => obj.RegimeTributario == regimeTributario);

            if (!string.IsNullOrWhiteSpace(tipoPessoa))
                query = query.Where(obj => obj.TipoPessoa == tipoPessoa);

            return query.FirstOrDefault();
        }
        public Dominio.Entidades.ImpostoContratoFrete BuscarPorRaizCNPJ(string cpfCnpjEmpresa, RegimeTributario? regimeTributario, string tipoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpostoContratoFrete>();

           // query = query.Where(obj => obj.Empresa == null);

            if (cpfCnpjEmpresa != "")
                query = query.Where(obj => obj.Empresa.CNPJ.Substring(0,8) == cpfCnpjEmpresa.Substring(0, 8));

            if (regimeTributario.HasValue && regimeTributario.Value != RegimeTributario.NaoInformado)
                query = query.Where(obj => obj.RegimeTributario == regimeTributario);

            if (!string.IsNullOrWhiteSpace(tipoPessoa))
                query = query.Where(obj => obj.TipoPessoa == tipoPessoa);

            return query.FirstOrDefault();
        }
        public List<Dominio.Entidades.ImpostoContratoFrete> Consultar(double cpfCnpjTerceiro, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpostoContratoFrete>();

            query = query.Where(obj => obj.Empresa == null);

            if (cpfCnpjTerceiro > 0D)
                query = query.Where(obj => obj.Terceiro.CPF_CNPJ == cpfCnpjTerceiro);

            return query.OrderBy(propOrdenar + " " + dirOrdenar).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(double cpfCnpjTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpostoContratoFrete>();

            query = query.Where(obj => obj.Empresa == null);

            if (cpfCnpjTerceiro > 0D)
                query = query.Where(obj => obj.Terceiro.CPF_CNPJ == cpfCnpjTerceiro);

            return query.Count();
        }
    }
}
