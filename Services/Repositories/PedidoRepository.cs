using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Models;

namespace WpfApp.Services.Repositories
{
    public class PedidoRepository
    {
        private readonly IDataStore _ds;
        private const string File = "pedidos.json";
        private readonly List<Pedido> _listPedidos;

        public PedidoRepository(IDataStore ds)
        {
            _ds = ds ?? throw new ArgumentNullException(nameof(ds));
            _listPedidos = _ds.Load<Pedido>(File) ?? new List<Pedido>();
        }

        public IEnumerable<Pedido> GetPessoaId(int pessoaId)
        {
            return _listPedidos.Where(p => p.PessoaId == pessoaId);
        }        

        public Pedido Add(Pedido p)
        {
            if (p == null)
                throw new ArgumentNullException(nameof(p));

            // Defensivo: se vier sem status, nasce como Pendente
            if (p.Status == default)
                p.Status = StatusPedido.Pendente;

            p.Id = _listPedidos.Any() ? _listPedidos.Max(x => x.Id) + 1 : 1;           

            _listPedidos.Add(p);
            _ds.Save(File, _listPedidos);

            return p;
        }

        public void UpdateStatus(int id, StatusPedido status)
        {
            var pedido = _listPedidos.FirstOrDefault(x => x.Id == id);

            if (pedido == null)
                throw new InvalidOperationException("Pedido não encontrado.");

            pedido.Status = status;
            _ds.Save(File, _listPedidos);
        }

        public List<Pedido> All() => _listPedidos.ToList();
    }
}