using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class CargaFrimesa : RepositorioBase<Dominio.Entidades.CargaFrimesa>, Dominio.Interfaces.Repositorios.CargaFrimesa
    {
        public CargaFrimesa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CargaFrimesa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.CargaFrimesa BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesaDocumentos>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj.CargaFrimesa;

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.CargaFrimesa> BuscarPorData(DateTime dataCarga, string embarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesa>();
            var result = from obj in query select obj;

            if (dataCarga.Date > DateTime.MinValue)
                result = result.Where(obj => obj.DataCarga == dataCarga);
            
            if (!string.IsNullOrEmpty(embarcador))
                result = result.Where(obj => (obj.Documentos.Any(o => o.CTe != null ? o.CTe.Remetente.CPF_CNPJ.Equals(embarcador) : o.NFSe != null ? o.NFSe.Tomador.CPF_CNPJ.Equals(embarcador) : 1 == 0) ));

            return result.ToList();
        }


        public List<Dominio.Entidades.CargaFrimesa> Consultar(DateTime dataCarga, string placa, string embarcador, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesa>();
            var result = from obj in query select obj;

            if (dataCarga.Date > DateTime.MinValue)
                result = result.Where(obj => obj.DataCarga == dataCarga);

            if (!string.IsNullOrEmpty(placa))
                result = result.Where(obj => obj.Veiculo.Placa.Equals(placa));
    
            if (!string.IsNullOrEmpty(embarcador))
                result = result.Where(obj => (obj.Documentos.Any(o => o.CTe != null ? o.CTe.Remetente.CPF_CNPJ.Equals(embarcador) : o.NFSe != null ? o.NFSe.Tomador.CPF_CNPJ.Equals(embarcador) : 1 == 0)));

            return result.OrderByDescending(o => o.DataCarga).ThenBy(o => o.DescricaoTransportadora).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(DateTime dataCarga, string placa, string embarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesa>();
            var result = from obj in query
                         where
                            1 == 1
                         select obj;

            if (dataCarga.Date > DateTime.MinValue)
                result = result.Where(obj => obj.DataCarga == dataCarga);

            if (!string.IsNullOrEmpty(placa))
                result = result.Where(obj => obj.Veiculo.Placa.Equals(placa));
            
            if (!string.IsNullOrEmpty(embarcador))
                result = result.Where(obj => (obj.Documentos.Any(o => o.CTe != null ? o.CTe.Remetente.CPF_CNPJ.Equals(embarcador) : o.NFSe != null ? o.NFSe.Tomador.CPF_CNPJ.Equals(embarcador) : 1 == 0)));

            return result.Count();
        }

        public Dominio.Entidades.CargaFrimesa BuscarPorVeiculoRotaData(int codigoEmpresa, int codigoVeiculo, int codigoRota, DateTime dataCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesa>();
            var result = from obj in query
                         where
                            obj.DataCarga == dataCarga 
                            //&& (obj.CTe == null || obj.CTe.Status.Equals("C"))
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (codigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (codigoRota > 0)
                result = result.Where(obj => obj.Rota.Codigo == codigoRota);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.CargaFrimesa> BuscarPorVeiculoRotaData(string transportadora, string placa, string rota, DateTime dataCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesa>();
            var result = from obj in query
                         where
                            obj.DataCarga == dataCarga
                         select obj;

            if (!string.IsNullOrWhiteSpace(transportadora))
                result = result.Where(obj => obj.DescricaoTransportadora.Contains(transportadora));

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(obj => obj.DescricaoVeiculo.Contains(placa));

            if (!string.IsNullOrWhiteSpace(rota))
                result = result.Where(obj => obj.DescricaoRota.Contains(rota));

            return result.ToList();
        }

    }
}