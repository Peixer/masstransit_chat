using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chat.Core;
using MassTransit;

namespace Chat
{
    public partial class Form1 : Form
    {
        private IBusControl busControl;
        readonly Guid guid = Guid.NewGuid();

        public Form1()
        {
            InitializeComponent();
            IniciarBusControl();

            id_label.Text = guid.ToString();
        }

        private void IniciarBusControl()
        {
            busControl = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var config = ObterConfiguracoesRabbitMQ();

                var host = sbc.Host(new Uri(config.Servidor), h =>
                {
                    h.Username(config.Usuario);
                    h.Password(config.Senha);
                });
                
                sbc.AutoDelete = true;
                sbc.ReceiveEndpoint(host, $"{guid.ToString()}", e =>
                {
                    e.Handler<Mensagem>(context =>
                    {
                        richTextBox.Invoke((MethodInvoker)delegate
                        {
                            EscreverMensagem($"{context.Message.Guid}:{context.Message.Mensagem_Enviada}");
                        });
                        return Task.FromResult(0);
                    });

                    e.Handler<MensagemEntrada>(context =>
                    {
                        richTextBox.Invoke((MethodInvoker)delegate
                        {
                            EscreverMensagem(context.Message.Mensagem);
                        });
                        return Task.FromResult(0);
                    });
                });
            });
            busControl.Start();
        }

        private ConfiguracaoRabbitMQ ObterConfiguracoesRabbitMQ()
        {
            string servidor = ConfigurationManager.AppSettings["servidor"];
            string usuario = ConfigurationManager.AppSettings["usuario"];
            string senha = ConfigurationManager.AppSettings["senha"];

            return new ConfiguracaoRabbitMQ(servidor, usuario, senha);
        }

        private void EscreverMensagem(string mensagem)
        {
            richTextBox.AppendText($"{mensagem} \n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var sendEndpoint = busControl.GetSendEndpoint(new Uri($"rabbitmq://localhost:5672/{guid}")).Result;

            //sendEndpoint.Send(new Mensagem(guid, input_textBox.Text));

            busControl.Publish(new Mensagem(guid, input_textBox.Text));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            busControl.Publish(new MensagemEntrada($"GUID {guid} entrou na sala"));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            busControl.Stop();
        }
    }
}
