using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HudsonLimaDm106.Models;
using HudsonLimaDm106.br.com.correios.ws;
using HudsonLimaDm106.CRMClient;
using System.Globalization;

namespace HudsonLimaDm106.Controllers
{
    
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public List<Order> GetOrders()
        {
             List<Order> orders = db.Orders.ToList();
             return orders;

            //return db.Orders.AsQueryable();
        }

        // GET: api/Orders/5
        [Authorize]
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {            
            
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return BadRequest("Pedido de código " + id + " não existe");
            }
            else if (User.IsInRole("ADMIN"))
            {
                return Ok(order);
            }
            else if (User.IsInRole("USER") && User.Identity.Name == order.userEmail)
            {
                return Ok(order);
            }
            else
            {
                return BadRequest("Não foi possível processar sua solicitação");
            }

        }

        // GET: api/Orders/useremail?userEmail=hudsonlima.ti@gmail.com
        [Authorize]
        [ResponseType(typeof(Order))]
        [HttpGet]
        [Route("useremail")]        
        public IHttpActionResult GetOrdersByUserEmail(string userEmail)
        {
            if (User.IsInRole("ADMIN") || User.Identity.Name == userEmail)
            {
                List<Order> orders = db.Orders.Where(p => p.userEmail == userEmail).ToList();
                if (orders == null)
                {

                }
                else
                {
                }
                return Ok(orders);
            }
            else if (User.IsInRole("USER") || User.Identity.Name == userEmail)
            {
                return BadRequest("Não existe pedidos para o email informado (" + userEmail + ")");
            }
            else
            {
                return BadRequest("Acesso não autorizado.");
            }

        }


        // PUT: api/Orders/5
        [Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [Authorize]
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            order.statusPedido = "novo";
            order.pesoPedido = 0;
            order.precoFrete = 0;
            order.precoPedido = 0;
            order.dataPedido = DateTime.Now;
            order.userEmail = User.Identity.Name;  

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [Authorize]
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return BadRequest("Pedido de código " + id + " não existe.");
            }
            else if ((User.IsInRole("ADMIN")) || (order.userEmail == User.Identity.Name))
            {
                db.Orders.Remove(order);
                db.SaveChanges();
            }
            else
            {
                return BadRequest("Acesso não autorizado.");
            }

            return Ok(order);
        }


        //api/orders/closeorder?orderid=5
        [Authorize]
        [ResponseType(typeof(Order))]
        [HttpPut]
        [Route("closeorder")]
        public IHttpActionResult CloseOrder(int orderId)
        {

            Order order = db.Orders.Where(p => p.Id == orderId).FirstOrDefault();

            if (order == null)
            {
                return BadRequest("Pedido de código "+ orderId + " não existe.");
            }
            else if ((User.IsInRole("ADMIN")) || (order.userEmail == User.Identity.Name))
            {
                if (order.precoFrete == (decimal)0.00)
                {
                    return BadRequest("Valor do frete zerado. Não será possível fechar o pedido "+orderId+".");
                }

                order.statusPedido = "fechado";
                db.SaveChanges();
                return Ok("Pedido " + orderId + " fechado com sucesso!");
            }
            else
            {
                return BadRequest("Acesso não autorizado.");
            }
        }


        /*
        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("frete")]
        public IHttpActionResult CalculaFrete()
        {
            string frete;
            CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
            cResultado resultado = correios.CalcPrecoPrazo("", "","40010", "37540000", "37002970", "1", 1, 30, 30, 30, 30, "N",	100,	"S");

                    if (resultado.Servicos[0].Erro.Equals("0"))
            {
                frete = "Valor	do	frete:	" + resultado.Servicos[0].Valor + "	-	Prazo	de	entrega:	" + resultado.Servicos[0].PrazoEntrega + "	dia(s)";
                return Ok(frete);
            }
            else
            {
                return BadRequest("Código	do	erro:	" + resultado.Servicos[0].Erro + "-" + resultado.Servicos[0].MsgErro);
            }
        }
        */



        [Authorize]
        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("frete")]
        public IHttpActionResult GetFrete(int orderId)
        {

            decimal pesoPedido = 0, alturaTotal = 0, largura = 0, comprimento = 0, diametro = 0, precoTotal = 0, valorFrete = 0;
            String CEPDestino, prazoEntrega;
            cResultado resultado;
            Order order = db.Orders.Where(p => p.Id == orderId).FirstOrDefault();

            if (order == null)
            {
                return BadRequest("Pedido de N. " + orderId + " não exisrte.");
            }
            else if (order.OrderItems.Count == 0)
            { 
                return BadRequest("Pedido de N. " + orderId + " não possui itens.");
            }
            else if (order.statusPedido != "novo")
            {            
                return BadRequest("Pedido de N. " + orderId + " com status diferente de 'novo'");
            }


            if ((order.userEmail == User.Identity.Name) || (User.IsInRole("ADMIN")))
            {


                CRMRestClient crmClient = new CRMRestClient();
                try
                {
                    Customer customer = crmClient.GetCustomerByEmail(User.Identity.Name);
                    CEPDestino = customer.zip;
                }
                catch
                {
                    return BadRequest("Não foi possivel acessar o serviço de CRM.Verifique se o e-mail do usuário existe no crm");
                }

                for (int itensPed = 0; itensPed < order.OrderItems.Count; itensPed++)
                {
                    pesoPedido += (order.OrderItems.ElementAt(itensPed).quantidade * order.OrderItems.ElementAt(itensPed).Product.peso);
                    alturaTotal += order.OrderItems.ElementAt(itensPed).Product.altura;
                    precoTotal += (order.OrderItems.ElementAt(itensPed).quantidade * order.OrderItems.ElementAt(itensPed).Product.preco);
                    

                    if (order.OrderItems.ElementAt(itensPed).Product.largura > largura)
                        largura = order.OrderItems.ElementAt(itensPed).Product.largura;

                    if (order.OrderItems.ElementAt(itensPed).Product.comprimento > comprimento)
                        comprimento = (order.OrderItems.ElementAt(itensPed).quantidade * order.OrderItems.ElementAt(itensPed).Product.comprimento);

                    diametro = order.OrderItems.ElementAt(itensPed).Product.diametro;
                }

                CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
                try
                {
                    resultado = correios.CalcPrecoPrazo("", "", "40010", "37550000", CEPDestino, pesoPedido.ToString(), 1, comprimento, alturaTotal, largura, diametro, "N", 0, "S");
                    prazoEntrega = resultado.Servicos.ElementAt(0).PrazoEntrega;
                }
                catch
                {
                    return BadRequest("Ocorreu um erro ao acessar o servicço dos Correios");
                }

                if (resultado.Servicos[0].Erro.Equals("0"))
                {
                    NumberFormatInfo nfi = new CultureInfo("pt-BR", false).NumberFormat;
                    valorFrete = decimal.Parse(resultado.Servicos[0].Valor, nfi);
                    int prazo = int.Parse(resultado.Servicos.Single().PrazoEntrega);
                    DateTime atual = DateTime.Now;

                    atual = atual.AddDays(prazo);

                    order.pesoPedido = pesoPedido;
                    order.precoFrete = valorFrete;
                    order.precoPedido = precoTotal;
                    order.dataEntrega = atual;

                    db.SaveChanges();

                    return Ok("Frete Pedido N. " + orderId + " calculado com Sucesso. Frete: R$ " + resultado.Servicos.Single().Valor + " - Prazo: " + resultado.Servicos.Single().PrazoEntrega + " dias )");
                }
                else
                {
                    return BadRequest("Código do erro: " + resultado.Servicos[0].Erro + "-" + resultado.Servicos[0].MsgErro);
                }

            }
            else
            {
                return Ok("Acesso não autorizado.");
            }
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}