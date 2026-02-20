using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class IntegracaoMDFe : RepositorioBase<Dominio.Entidades.IntegracaoMDFe>, Dominio.Interfaces.Repositorios.IntegracaoMDFe
    {
        public IntegracaoMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.IntegracaoMDFe> BuscarPorCarga(string numeroCarga, Dominio.Enumeradores.StatusMDFe? statusMDFe = null, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();

            var result = from obj in query where obj.NumeroDaCarga == numeroCarga  && obj.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao select obj;

            if (statusMDFe != null)
                result = result.Where(o => o.MDFe.Status == statusMDFe);

            if (codigoEmpresa > 0)
                result = result.Where(o => o.MDFe.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<Dominio.Entidades.IntegracaoMDFe> BuscarPorCargaEmpresa(string numeroCarga, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();

            var result = from obj in query where obj.NumeroDaCarga == numeroCarga && obj.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.MDFe.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<Dominio.Entidades.IntegracaoMDFe> BuscarPorMDFeETipo(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();

            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            if (tipo != Dominio.Enumeradores.TipoIntegracaoMDFe.Todos)
                result = result.Where(o => o.Tipo == tipo);

            return result.ToList();
        }

        public List<Dominio.Entidades.IntegracaoMDFe> Buscar(int codigoEmpresaPai, string numeroUnidade, string numeroCarga, int quantidadeRegistros, Dominio.Enumeradores.StatusIntegracao status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();

            var result = from obj in query where obj.MDFe.Empresa.EmpresaPai.Codigo == codigoEmpresaPai && obj.Status == status select obj;

            if (!string.IsNullOrWhiteSpace(numeroUnidade) && numeroUnidade != "0")
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            if (!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0")
                result = result.Where(o => o.NumeroDaCarga == numeroCarga);

            return result.OrderBy(o => o.MDFe.Status).ThenBy(o => o.MDFe.Codigo).Take(quantidadeRegistros).ToList();
        }

        public List<Dominio.Entidades.IntegracaoMDFe> Buscar(int codigoEmpresaPai, string numeroUnidade, string numeroCarga, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();

            var result = from obj in query where obj.MDFe.Empresa.EmpresaPai.Codigo == codigoEmpresaPai && obj.Tipo == tipoIntegracao select obj;

            if (!string.IsNullOrWhiteSpace(numeroUnidade) && numeroUnidade != "0")
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            if (!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0")
                result = result.Where(o => o.NumeroDaCarga == numeroCarga);

            return result.OrderByDescending(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.IntegracaoMDFe> Buscar(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();

            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipoIntegracao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.IntegracaoMDFe> BuscarParaEncerramento(DateTime dataEncerramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();

            var result = from obj in query where obj.DataEncerramento != null && obj.DataEncerramento <= dataEncerramento && obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado select obj;

            return result.ToList();
        }

        public int ContarMDFeExistente(int codigoEmpresaPai, string numeroCarga, string numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();

            var result = from obj in query where obj.MDFe.Empresa.EmpresaPai.Codigo == codigoEmpresaPai && (obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado) select obj;

            if (!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0")
                result = result.Where(o => o.NumeroDaCarga == numeroCarga);

            if (!string.IsNullOrWhiteSpace(numeroUnidade) && numeroUnidade != "0")
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            return result.Count();
        }

        public Dominio.Entidades.IntegracaoMDFe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<int> BuscarPendentesIntegracao(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();
            var result = from obj in query where 
                obj.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao 
                && obj.GerouCargaEmbarcador == false 
                && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Enviado select obj;

            if (maximoRegistros > 0)
                return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
            
            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarPendentesIntegracaoMDFe(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();
            var result = from obj in query
                where
                    obj.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao
                    && obj.GerouCargaEmbarcador == false
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Enviado
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.EmDigitacao
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento
                select obj;

            if (maximoRegistros > 0)
                return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();

            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).ToList();
        }

        public int ContarPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();
            var result = from obj in query where 
                obj.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao 
                && obj.GerouCargaEmbarcador == false
                && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Enviado
                select obj;

            return result.Count();
        }

        public int ContarPendentesIntegracaoMDFe()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();
            var result = from obj in query
                where
                    obj.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao
                    && obj.GerouCargaEmbarcador == false
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Enviado
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.EmDigitacao
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento
                select obj;

            return result.Count();
        }
    }
}
