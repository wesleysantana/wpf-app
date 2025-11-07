using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp.Models
{
    public class Pedido
    {
        public int Id { get; set; }                             
        public int PessoaId { get; set; }                     
        public string PessoaNome { get; set; }
        public List<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
        public decimal ValorTotal => Itens.Sum(i => i.Subtotal); 
        public DateTime DataVenda { get; set; } = DateTime.Now;  
        public FormaPagamento FormaPagamento { get; set; }      
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;
        public bool Finalizado { get; set; }
    }
}