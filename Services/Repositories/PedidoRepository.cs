using System.Collections.Generic;
using System.Linq;
using WpfApp.Models;

namespace WpfApp.Services.Repositories
{
    public class PedidoRepository
    {
        private readonly IDataStore _ds;
        private const string File = "pedidos.json";
        private List<Pedido> _ListPedidos;
        private readonly List<string> _listErrors = new List<string>();
        public IReadOnlyCollection<string> ListErrors => _listErrors;

        public PedidoRepository(IDataStore ds) 
        { 
            _ds = ds; 
            _ListPedidos = _ds.Load<Pedido>(File); 
        }

        public IEnumerable<Pedido> GetPessoaId(int pessoaId) => _ListPedidos.Where(p => p.PessoaId == pessoaId);

        public Pedido Add(Pedido p)
        {
            // Finalização definida na VM; aqui apenas persiste
            p.Id = _ListPedidos.Any() ? _ListPedidos.Max(x => x.Id) + 1 : 1;
            _ListPedidos.Add(p); 
            _ds.Save(File, _ListPedidos); 
            return p;
        }

        public void UpdateStatus(int id, StatusPedido status)
        {
            var p = _ListPedidos.FirstOrDefault(x => x.Id == id);

            if (p == null)
            {
                _listErrors.Add("Pedido não encontrado");
                return;
            }
            
            p.Status = status; 
            _ds.Save(File, _ListPedidos);
        }

        public List<Pedido> All() => _ListPedidos.ToList();
    }
}